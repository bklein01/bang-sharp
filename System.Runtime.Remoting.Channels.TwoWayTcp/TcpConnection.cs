// TcpConnection.cs
//  
// Author:  WOnder93 <omosnacek@gmail.com>
// 
// Copyright (c) 2012 Ondrej Mosnáček
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
	internal delegate void MessageRecieved(Message message);
	internal delegate void Disconnected(TcpConnection conn);

	internal class TcpConnection
	{
		private class InputStream : Stream
		{
			private TcpConnection conn;
			private int chunkIndex;
			private int chunkLength;

			public InputStream(TcpConnection connection)
			{
				conn = connection;
				NextChunk();
			}

			public override void Flush()
			{
			}

			private void Unlock()
			{
				lock(conn.recieveLock)
				{
					conn.recieveLocked = false;
					Monitor.Pulse(conn.recieveLock);
				}
			}

			protected override void Dispose(bool disposing)
			{
				if(!disposing)
					return;
				try
				{
					byte[] buffer = new byte[DefaultBufferSize];
					int read;
					do
						read = Read(buffer, 0, buffer.Length);
					while(read != 0);

				}
				catch(RemotingException)
				{
				}
			}

			private void NextChunk()
			{
				try
				{
					chunkLength = conn.reader.ReadInt32();
				}
				catch(IOException e)
				{
					conn.Kill();
					throw new RemotingException("TCP error!", e);
				}
				if(chunkLength < 0)
				{
					conn.Kill();
					throw new RemotingException("TCP error: Invalid chunk length!");
				}
				chunkIndex = 0;
				if(chunkLength == 0)
					Unlock();
			}

			public override int ReadByte()
			{
				if(chunkLength == 0)
					return -1;
				byte b;
				try
				{
					b = conn.reader.ReadByte();
				}
				catch(IOException e)
				{
					conn.Kill();
					throw new RemotingException("TCP error!", e);
				}
				chunkIndex++;
				if(chunkIndex >= chunkLength)
					NextChunk();
				return b;
			}
			public override int Read(byte[] buffer, int offset, int count)
			{
				if(chunkLength == 0)
					return 0;
				int totalRead = 0;
				do
				{
					int segment = Math.Min(chunkLength, count);
					int read = conn.reader.Read(buffer, offset, segment);
					if(read != segment)
					{
						conn.Kill();
						throw new RemotingException("TCP error!");
					}
					totalRead += segment;
					offset += segment;
					count -= segment;
					chunkIndex += segment;
					if(chunkIndex >= chunkLength)
						NextChunk();
				}
				while(count > 0 && chunkLength > 0);
				return totalRead;
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			public override bool CanRead
			{
				get { return true; }
			}
			public override bool CanSeek
			{
				get { return false; }
			}
			public override bool CanWrite
			{
				get { return false; }
			}

			public override long Length
			{
				get { throw new NotSupportedException(); }
			}
			public override long Position
			{
				get { throw new NotSupportedException(); }
				set { throw new NotSupportedException(); }
			}
		}
		private class OutputStream : Stream
		{
			private TcpConnection conn;
			private MemoryStream ms;
			private bool disposed;

			public OutputStream(TcpConnection connection)
			{
				conn = connection;
				ms = new MemoryStream(conn.sendBuffer);
			}

			public override void Flush()
			{
				int length = (int)ms.Position;
				if(length == 0)
					return;
				try
				{
					conn.writer.Write(length);
					conn.writer.Write(conn.sendBuffer, 0, length);
				}
				catch(IOException e)
				{
					conn.Kill();
					throw new RemotingException("TCP error!", e);
				}
				ms.Position = 0L;
			}

			private void Unlock()
			{
				lock(conn.sendLock)
				{
					conn.sendLocked = false;
					Monitor.Pulse(conn.sendLock);
				}
			}

			protected override void Dispose(bool disposing)
			{
				if(!disposing || disposed)
					return;
				Flush();
				try
				{
					conn.writer.Write(0);
				}
				catch(IOException e)
				{
					conn.Kill();
					throw new RemotingException("TCP error!", e);
				}
				Unlock();
				disposed = true;
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override void WriteByte(byte value)
			{
				ms.WriteByte(value);
				if(ms.Position >= ms.Length)
					Flush();
			}
			public override void Write(byte[] buffer, int offset, int count)
			{
				int segment = Math.Min((int)ms.Length, count);
				do
				{
					ms.Write(buffer, offset, segment);
					offset += segment;
					count -= segment;
					if(ms.Position >= ms.Length)
						Flush();
				}
				while(count != 0);
			}

			public override bool CanRead
			{
				get { return false; }
			}
			public override bool CanSeek
			{
				get { return false; }
			}
			public override bool CanWrite
			{
				get { return true; }
			}

			public override long Length
			{
				get { throw new NotSupportedException(); }
			}
			public override long Position
			{
				get { throw new NotSupportedException(); }
				set { throw new NotSupportedException(); }
			}
		}
		public static readonly Guid ThisMachineID = Guid.NewGuid();
		private static readonly byte[] magic = new byte[] { (byte)'T', (byte)'T', (byte)'C', (byte)'P' };
		private const byte Version = 2;
		private const int DefaultBufferSize = 0x10000;
		private static int nextId = 0;

		private object recieveLock = new object();
		private object sendLock = new object();
		private bool recieveLocked = false;
		private bool sendLocked = false;
		private byte[] sendBuffer = new byte[DefaultBufferSize];

		private TcpConnectionPool pool;
		private Socket socket;
		private Guid id;
		private int connId;

		private Thread recieveThread;
		private Stream networkStream;
		private BinaryReader reader;
		private BinaryWriter writer;

		public Disconnected OnDisconnected;

		public MessageRecieved OnRequestRecieved;
		public MessageRecieved OnResponseRecieved;

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
			get { return networkStream != null; }
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
			writer = new BinaryWriter(networkStream);
		}

		public void Kill()
		{
			if(networkStream == null)
				return;
			writer.Close();
			reader.Close();

			networkStream.Close();
			networkStream = null;

			pool.RemoveConnection(this);
			StopListening();
			OnDisconnected(this);
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
				lock(recieveLock)
					while(true)
					{
						while(recieveLocked)
							Monitor.Wait(recieveLock);
						recieveLocked = true;
						Message message = InternalRecieveMessage();
						ThreadPool.QueueUserWorkItem(FireMessage, message);
					}
			}
			catch
			{
			}
			finally
			{
				Kill();
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

		public Stream SendMessage(Message message)
		{
			if(!IsAlive)
				throw new RemotingException("TCP connection closed!");

			if(message.Stream != null && message.Stream is OutputStream)
			{
				message.Stream.Close();
				return null;
			}
			lock(sendLock)
			{
				while(sendLocked)
					Monitor.Wait(sendLock);
				sendLocked = true;
				try
				{
					return InternalSendMessage(message);
				}
				catch(IOException e)
				{
					Kill();
					throw new RemotingException("TCP error!", e);
				}
			}
		}

		private bool IsVersionCompatible(byte version)
		{
			return version == Version;
		}
		private Message InternalRecieveMessage()
		{
			if(!IsAlive)
				throw new RemotingException("TCP error: Connection closed!");

			Message message = new Message();
			message.Connection = this;

			byte[] hdr = reader.ReadBytes(magic.Length);
			for(int i = 0; i < magic.Length; i++)
				if(hdr[i] != magic[i])
					throw new RemotingException("TCP error: Invalid message header!");

			if(!IsVersionCompatible(reader.ReadByte()))
				throw new RemotingException("TCP error: Invalid protocol version not compatible!");

			switch((MessageType)reader.ReadByte())
			{
			case MessageType.Request:
				message.Type = MessageType.Request;
				break;
			case MessageType.Response:
				message.Type = MessageType.Response;
				break;
			default:
				throw new RemotingException("TCP error: Invalid message type!");
			}

			id = new Guid(reader.ReadBytes(16));
			message.ID = new Guid(reader.ReadBytes(16));

			TransportHeaders headers = new TransportHeaders();
			string key;
			while((key = reader.ReadString()).Length != 0)
				headers[key] = reader.ReadString();
			message.Headers = headers;
			message.Stream = new InputStream(this);
			return message;
		}
		private Stream InternalSendMessage(Message message)
		{
			if(!IsAlive)
				throw new RemotingException("TCP error: Connection closed!");

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
			Stream outStream = new OutputStream(this);
			if(message.Stream != null)
			{
				MemoryStream ms = message.Stream as MemoryStream;
				if(ms != null)
					try
					{
						outStream.Write(ms.GetBuffer(), 0, (int)ms.Length);
					}
					catch(UnauthorizedAccessException)
					{
						ms = null;
					}
				if(ms == null)
				{
					byte[] buffer = new byte[DefaultBufferSize];
					while(true)
					{
						int read = message.Stream.Read(buffer, 0, buffer.Length);
						if(read == 0)
							break;
						outStream.Write(buffer, 0, read);
					}
				}
				outStream.Close();
				return null;
			}
			return outStream;
		}
	}
}
