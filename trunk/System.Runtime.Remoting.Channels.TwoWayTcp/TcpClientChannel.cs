// TcpClientChannel.cs
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
using System.Runtime.Remoting.Messaging;
namespace System.Runtime.Remoting.Channels.TwoWayTcp
{
	public class TcpClientChannel : IChannelSender, IChannelReceiver
	{
		private static readonly TcpConnectionPool pool = new TcpConnectionPool();
		private string name = "TcpClientChannel";
		private int priority = 1;

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
			
			channelData = new ChannelDataStore(new string[] { "tcp://" + TcpConnection.ThisMachineID });
			for(IServerChannelSinkProvider provider = serverSinkProvider; provider != null; provider = provider.Next)
				provider.GetChannelData(channelData);
			
			IServerChannelSink chain = ChannelServices.CreateServerChannelSinkChain(serverSinkProvider, this);
			serverSink = new TcpServerSink(chain);
			
			StartListening(null);
		}

		public TcpClientChannel(string name)
		{
			this.name = name;
			Init(null, null);
		}
		public TcpClientChannel(string name, IClientChannelSinkProvider clientSinkProvider, IServerChannelSinkProvider serverSinkProvider)
		{
			this.name = name;
			Init(clientSinkProvider, serverSinkProvider);
		}
		public TcpClientChannel(IDictionary properties, IClientChannelSinkProvider clientSinkProvider, IServerChannelSinkProvider serverSinkProvider)
		{
			foreach(DictionaryEntry e in properties)
				switch((string)e.Key)
				{
				case "name":
					name = e.Value.ToString();
					break;
				case "priority":
					priority = Convert.ToInt32(e.Value);
					break;
				case "supressChannelData":
					supressChannelData = Convert.ToBoolean(e.Value);
					break;
				}
			Init(clientSinkProvider, serverSinkProvider);
		}

		public void StartListening(object data)
		{
			pool.OnRequestRecieved += serverSink.OnRequestRecieved;
		}
		public void StopListening(object data)
		{
			pool.OnRequestRecieved -= serverSink.OnRequestRecieved;
			pool.PurgeConnections();
		}

		object IChannelReceiver.ChannelData
		{
			get { return supressChannelData ? null : channelData; }
		}
		string[] IChannelReceiver.GetUrlsForUri(string objectUri)
		{
			if(!objectUri.StartsWith("/"))
				objectUri = "/" + objectUri;

			string[] chnl_uris = channelData.ChannelUris;
			string[] result = new String[chnl_uris.Length];

			for(int i = 0; i < chnl_uris.Length; i++)
				result[i] = chnl_uris[i] + objectUri;

			return result;
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

		public string Parse (string url, out string objectURI)
		{
			return TcpChannel.ParseChannelUrl (url, out objectURI);
		}
	}
}

