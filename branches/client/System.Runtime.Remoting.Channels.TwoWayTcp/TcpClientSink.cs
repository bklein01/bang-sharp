// TcpServerClientSink.cs
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
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace System.Runtime.Remoting.Channels.TwoWayTcp
{
	internal class TcpClientSink : IClientChannelSink
	{
		private TcpConnection conn;
		private Dictionary<Guid, IClientChannelSinkStack> stacks;
		private Dictionary<Guid, Message> responseCache;
		private Dictionary<IMethodMessage, Message> requestCache;
		private int timeout;
		private Dictionary<Guid, Timer> syncTimers;
		private Dictionary<Guid, Timer> asyncTimers;

		IDictionary IChannelSinkBase.Properties
		{
			get { return null; }
		}
		IClientChannelSink IClientChannelSink.NextChannelSink
		{
			get { return null; }
		}

		public TcpClientSink(TcpConnection connection, int timeout)
		{
			conn = connection;
			conn.OnResponseRecieved += OnResponseRecieved;
			stacks = new Dictionary<Guid, IClientChannelSinkStack>();
			responseCache = new Dictionary<Guid, Message>();
			requestCache = new Dictionary<IMethodMessage, Message>();
			this.timeout = timeout;
			if(timeout > 0)
			{
				syncTimers = new Dictionary<Guid, Timer>();
				asyncTimers = new Dictionary<Guid, Timer>();
			}
		}

		void IClientChannelSink.ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			responseHeaders = null;
			responseStream = null;
			IMethodMessage methodMessage = (IMethodMessage)msg;
			bool isOneWay = RemotingServices.IsOneWay(methodMessage.MethodBase);
			if(requestHeaders == null)
				requestHeaders = new TransportHeaders();
			requestHeaders[CommonTransportKeys.RequestUri] = methodMessage.Uri;
			Message request;
			if(requestCache.ContainsKey(methodMessage))
			{
				request = requestCache[methodMessage];
				request.Stream = requestStream;
				requestCache.Remove(methodMessage);
			}
			else
				request = new Message { Type = MessageType.Request, ID = Guid.NewGuid(), Headers = requestHeaders, Stream = requestStream };
			lock(responseCache)
			{
				conn.SendMessage(request);
				if(!isOneWay)
				{
					if(timeout > 0)
						syncTimers[request.ID] = new Timer((state) => {
							Guid id = (Guid)state;
							lock(responseCache)
							{
								syncTimers[id].Dispose();
								syncTimers.Remove(id);
								Monitor.PulseAll(responseCache);
							}
						}, request.ID, timeout, Timeout.Infinite);
					while(!responseCache.ContainsKey(request.ID) && (timeout >= 0 && syncTimers.ContainsKey(request.ID)))
						Monitor.Wait(responseCache);
					if(timeout > 0 && !syncTimers.ContainsKey(request.ID))
					{
						conn.Kill();
						throw new RemotingTimeoutException("Request timed out!");
					}
					syncTimers[request.ID].Dispose();
					syncTimers.Remove(request.ID);
					Message response = responseCache[request.ID];
					responseCache.Remove(request.ID);
					responseHeaders = response.Headers;
					responseStream = response.Stream;
				}
			}
		}

		void IClientChannelSink.AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			IMethodMessage methodMessage = (IMethodMessage)msg;
			bool isOneWay = RemotingServices.IsOneWay(methodMessage.MethodBase);
			if(headers == null)
				headers = new TransportHeaders();
			headers[CommonTransportKeys.RequestUri] = methodMessage.Uri;
			Message request;
			if(requestCache.ContainsKey(methodMessage))
			{
				request = requestCache[methodMessage];
				request.Stream = stream;
				requestCache.Remove(methodMessage);
			}
			else
				request = new Message { Type = MessageType.Request, ID = Guid.NewGuid(), Headers = headers, Stream = stream };
			if(!isOneWay)
				lock(stacks)
				{
					stacks[request.ID] = sinkStack;
					if(timeout > 0)
						asyncTimers[request.ID] = new Timer((state) => {
							Guid id = (Guid)state;
							lock(stacks)
							{
								asyncTimers[id].Dispose();
								asyncTimers.Remove(id);
								conn.Kill();
								stacks[id].DispatchException(new RemotingTimeoutException("Request timed out!"));
							}
						}, request.ID, timeout, Timeout.Infinite);
				}
			conn.SendMessage(request);
		}

		private void OnResponseRecieved(Message message)
		{
			try
			{
				IClientChannelSinkStack sinkStack;
				lock(stacks)
				{
					sinkStack = stacks[message.ID];
					stacks.Remove(message.ID);
					if(timeout > 0 && !asyncTimers.ContainsKey(message.ID))
						return;
					asyncTimers[message.ID].Dispose();
					asyncTimers.Remove(message.ID);
				}
				sinkStack.AsyncProcessResponse(message.Headers, message.Stream);
			}
			catch(KeyNotFoundException)
			{
				lock(responseCache)
					if(timeout > 0 && !syncTimers.ContainsKey(message.ID))
						message.Stream.Close();
					else
					{
						responseCache[message.ID] = message;
						Monitor.PulseAll(responseCache);
					}
			}
		}

		void IClientChannelSink.AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream)
		{
			throw new NotSupportedException();
		}

		Stream IClientChannelSink.GetRequestStream(IMessage msg, ITransportHeaders headers)
		{
			IMethodMessage methodMessage = (IMethodMessage)msg;
			if(headers == null)
				headers = new TransportHeaders();
			headers[CommonTransportKeys.RequestUri] = methodMessage.Uri;
			Message request = new Message { Type = MessageType.Request, ID = Guid.NewGuid(), Headers = headers, Stream = null };
			requestCache[methodMessage] = request;
			return conn.SendMessage(request);
		}
	}
}
