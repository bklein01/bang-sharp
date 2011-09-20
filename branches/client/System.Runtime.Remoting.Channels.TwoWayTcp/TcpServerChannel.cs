// TcpServerChannel.cs
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
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Threading;
namespace System.Runtime.Remoting.Channels.TwoWayTcp
{
	public class TcpServerChannel : IChannelReceiver, IChannelSender
	{
		private TcpConnectionPool pool;
		private int port;
		private string host = null;

		private string name = "TcpServerChannel";
		private int priority = 1;

		private bool useIpAddress = true;
		private IPAddress bindTo = IPAddress.Any;
		private Thread serverThread;
		private TcpListener listener;

		private bool supressChannelData = false;
		private TcpServerSink serverSink;
		private IClientChannelSinkProvider clientSinkProvider;
		private ChannelDataStore channelData;

		public string ChannelName
		{
			get { return name; }
		}
		public int ChannelPriority
		{
			get { return priority; }
		}

		private void Init(IClientChannelSinkProvider clientSinkProvider, IServerChannelSinkProvider serverSinkProvider)
		{
			pool = new TcpConnectionPool();
			if(clientSinkProvider == null)
			{
				this.clientSinkProvider = new BinaryClientFormatterSinkProvider();
				this.clientSinkProvider.Next = new TcpClientSinkProvider(pool);
			}
			else
			{
				this.clientSinkProvider = clientSinkProvider;
				IClientChannelSinkProvider provider = clientSinkProvider;
				while(provider.Next != null)
					provider = provider.Next;
				provider.Next = new TcpClientSinkProvider(pool);
			}

			if(serverSinkProvider == null)
				serverSinkProvider = new BinaryServerFormatterSinkProvider();

			if(host == null)
				if(useIpAddress)
				{
					if(bindTo != IPAddress.Any)
						host = bindTo.ToString();
					else
					{
						IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
						if(addresses.Length == 0)
							throw new RemotingException("IP address could not be determined for this host!");
						host = addresses[0].ToString();
					}
				}
				else
					host = Dns.GetHostName();

			channelData = new ChannelDataStore(new string[] { "tcp://" + TcpConnection.ThisMachineID });
			for(IServerChannelSinkProvider provider = serverSinkProvider; provider != null; provider = provider.Next)
				provider.GetChannelData(channelData);

			IServerChannelSink chain = ChannelServices.CreateServerChannelSinkChain(serverSinkProvider, this);
			serverSink = new TcpServerSink(chain);

			StartListening(null);
		}

		public TcpServerChannel(int port)
		{
			this.port = port;
			Init(null, null);
		}
		public TcpServerChannel(string name, int port)
		{
			this.name = name;
			this.port = port;
			Init(null, null);
		}
		public TcpServerChannel(string name, int port, IClientChannelSinkProvider clientSinkProvider, IServerChannelSinkProvider serverSinkProvider)
		{
			this.name = name;
			this.port = port;
			Init(clientSinkProvider, serverSinkProvider);
		}
		public TcpServerChannel(IDictionary properties, IClientChannelSinkProvider clientSinkProvider, IServerChannelSinkProvider serverSinkProvider)
		{
			foreach(DictionaryEntry e in properties)
				switch((string)e.Key)
				{
				case "name":
					name = e.Value.ToString();
					break;
				case "port":
					port = Convert.ToInt32(e.Value);
					break;
				case "priority":
					priority = Convert.ToInt32(e.Value);
					break;
				case "bindTo":
					bindTo = IPAddress.Parse((string)e.Value);
					break;
				case "rejectRemoteRequests":
					if(Convert.ToBoolean(e.Value))
						bindTo = IPAddress.Loopback;
					break;
				case "supressChannelData":
					supressChannelData = Convert.ToBoolean(e.Value);
					break;
				case "useIpAddress":
					useIpAddress = Convert.ToBoolean(e.Value);
					break;
				case "machineName":
					host = e.Value as string;
					break;
				}
			Init(clientSinkProvider, serverSinkProvider);
		}

		object IChannelReceiver.ChannelData
		{
			get { return supressChannelData ? null : channelData; }
		}
		string[] IChannelReceiver.GetUrlsForUri (string objectUri)
		{
			if(!objectUri.StartsWith("/"))
				objectUri = "/" + objectUri;

			string[] chnl_uris = channelData.ChannelUris;
			string[] result = new String[chnl_uris.Length];

			for(int i = 0; i < chnl_uris.Length; i++)
				result[i] = chnl_uris[i] + objectUri;

			return result;
		}

		public void StartListening(object data)
		{
			if(serverThread != null)
				return;

			pool.OnRequestRecieved += serverSink.OnRequestRecieved;
			listener = new TcpListener(bindTo, port);
			listener.Start();

			if(port == 0)
				port = ((IPEndPoint)listener.LocalEndpoint).Port;

			serverThread = new Thread(WaitForConnections);
			serverThread.IsBackground = true;
			serverThread.Start();
		}
		public void StopListening(object data)
		{
			if(serverThread == null)
				return;

			pool.OnRequestRecieved -= serverSink.OnRequestRecieved;
			pool.PurgeConnections();
			serverThread.Interrupt();
			listener.Stop();
			serverThread.Join();
			serverThread = null;
		}

		private void WaitForConnections()
		{
			try
			{
				while(true)
					pool.WaitForConnection(listener);
			}
			catch
			{
			}
		}

		IMessageSink IChannelSender.CreateMessageSink(string url, object remoteChannelData, out string objectURI)
		{
			if(url != null && TcpChannel.ParseChannelUrl(url, out objectURI) != null)
				return (IMessageSink)clientSinkProvider.CreateSink(this, url, remoteChannelData);

			if(remoteChannelData != null)
			{
				IChannelDataStore ds = remoteChannelData as IChannelDataStore;
				if(ds != null)
				{
					foreach(string u in ds.ChannelUris)
					{
						url = u;
						if(TcpChannel.ParseChannelUrl(url, out objectURI) != null)
							return (IMessageSink)clientSinkProvider.CreateSink(this, url, remoteChannelData);
					}
				}
			}
			objectURI = null;
			return null;
		}

		public string Parse(string url, out string objectURI)
		{
			return TcpChannel.ParseChannelUrl(url, out objectURI);
		}
	}
}

