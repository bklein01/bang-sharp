// TcpConnection.cs
//  
// Author:  WOnder93 <omosnacek@gmail.com>
// 
// Copyright (c) 2011 Ondrej Mosnáček
// 
// Created with the help of the source code of KBang (http://code.google.com/p/kbang)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace System.Runtime.Remoting.Channels.TwoWayTcp
{
	internal enum MessageType
	{
		Request,
		Response
	}
	internal struct Message
	{
		public TcpConnection Connection;
		public MessageType Type;
		public Guid ID;
		public ITransportHeaders Headers;
		public Stream Stream;
	}
	internal delegate void OnMessageRecieved(Message message);

	internal class TcpConnection
	{
		public static readonly Guid ThisMachineID = Guid.NewGuid();
		private static readonly byte[] magic = new byte[] { (byte)'T', (byte)'T', (byte)'C', (byte)'P' };
		private const byte Version = 1;
		private const int DefaultBufferSize = 1000;
		private static int nextId = 0;

		private object sendLock = new object();

		private TcpConnectionPool pool;
		private Socket socket;
		private Guid id;
		private int connId;

		private Thread recieveThread;
		private Stream networkStream;
		private BinaryReader reader;
		private BinaryWriter writer;

		public OnMessageRecieved OnRequestRecieved;
		public OnMessageRecieved OnResponseRecieved;

		public Guid MachineID
		{
			get { return id; }
		}
		public int ID
		{
			get { return connId; }
		}
		public bool IsAlive
		{
			get { return networkStream == null || !socket.Poll(0, SelectMode.SelectRead); }
		}
		public IPAddress RemoteAddress
		{
			get
			{
				IPEndPoint ep = socket.RemoteEndPoint as IPEndPoint;
				if(ep == null)
					return null;
				return ep.Address;
			}
		}

		public TcpConnection(TcpConnectionPool pool, Socket socket)
		{
			this.pool = pool;
			this.socket = socket;
			socket.NoDelay = true;
			connId = nextId++;
			networkStream = new NetworkStream(socket);
			reader = new BinaryReader(networkStream);
			writer = new BinaryWriter(new BufferedStream(networkStream));
		}

		private void FireMessage(object state)
		{
			Message message = (Message)state;
			switch(message.Type)
			{
			case MessageType.Request:
				if(OnRequestRecieved != null)
					OnRequestRecieved(message);
				break;
			case MessageType.Response:
				if(OnResponseRecieved != null)
					OnResponseRecieved(message);
				break;
			}
		}
		private void ProcessMessages()
		{
			try
			{
				while(true)
				{
					Message message = InternalRecieveMessage();
					ThreadPool.QueueUserWorkItem(FireMessage, message);
				}
			}
			catch
			{
			}
			finally
			{
				lock(sendLock)
				{
					writer.Close();
					writer = null;
					reader.Close();
					reader = null;
				}
				networkStream.Close();
				networkStream = null;

				pool.RemoveConnection(this);
			}
		}

		public void StartListening()
		{
			if(recieveThread != null)
				return;
			recieveThread = new Thread(ProcessMessages);
			recieveThread.IsBackground = true;
			recieveThread.Start();
		}
		public void StopListening()
		{
			if(recieveThread == null)
				return;
			recieveThread.Abort();
			recieveThread = null;
		}

		public void SendMessage(Message message)
		{
			if(networkStream == null)
				throw new RemotingException("TCP connection closed!");
			lock(sendLock)
			{
				InternalSendMessage(message);
			}
		}

		private bool IsVersionCompatible(byte version)
		{
			return version == Version;
		}
		private Message InternalRecieveMessage()
		{
			Message message = new Message();
			message.Connection = this;

			byte[] hdr = reader.ReadBytes(magic.Length);
			for(int i = 0; i < magic.Length; i++)
				if(hdr[i] != magic[i])
					throw new RemotingException("TCP error!");

			if(!IsVersionCompatible(reader.ReadByte()))
				throw new RemotingException("Remoting protocol not compatible!");

			switch((MessageType)reader.ReadByte())
			{
			case MessageType.Request:
				message.Type = MessageType.Request;
				break;
			case MessageType.Response:
				message.Type = MessageType.Response;
				break;
			default:
				throw new RemotingException("TCP error!");
			}

			id = new Guid(reader.ReadBytes(16));
			message.ID = new Guid(reader.ReadBytes(16));

			TransportHeaders headers = new TransportHeaders();
			string key;
			while((key = reader.ReadString()).Length != 0)
				headers[key] = reader.ReadString();
			message.Headers = headers;

			int messageLength = reader.ReadInt32();
			if(messageLength != 0)
				message.Stream = new MemoryStream(reader.ReadBytes(messageLength));
			else
				message.Stream = new MemoryStream(0);
			return message;
		}
		private void InternalSendMessage(Message message)
		{
			if(writer == null)
				throw new RemotingException("TCP connection closed!");
			writer.Write(magic);
			writer.Write(Version);
			writer.Write((byte)message.Type);

			writer.Write(ThisMachineID.ToByteArray());
			writer.Write(message.ID.ToByteArray());

			foreach(DictionaryEntry entry in message.Headers)
			{
				writer.Write((string)entry.Key);
				writer.Write((string)entry.Value);
			}
			writer.Write("");

			writer.Write((int)message.Stream.Length);
			try
			{
				MemoryStream ms = (MemoryStream)message.Stream;
				byte[] buf = ms.GetBuffer();
				writer.Write(buf, 0, (int)message.Stream.Length);
			}
			catch
			{
				int read;
				byte[] buffer = new byte[DefaultBufferSize];
				while((read = message.Stream.Read(buffer, 0, buffer.Length)) > 0)
					writer.Write(buffer, 0, read);
			}
			writer.Flush();
		}
	}
}

