// TcpServerServerSink.cs
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
using System.Runtime.Remoting.Messaging;
namespace System.Runtime.Remoting.Channels.TwoWayTcp
{
	internal class TcpServerSink : IServerChannelSink
	{
		IServerChannelSink nextSink;

		IDictionary IChannelSinkBase.Properties
		{
			get
			{
				if(nextSink == null)
					return null;
				return nextSink.Properties;
			}
		}
		IServerChannelSink IServerChannelSink.NextChannelSink
		{
			get { return nextSink; }
		}

		public TcpServerSink(IServerChannelSink next)
		{
			nextSink = next;
		}

		public void OnRequestRecieved(Message message)
		{
			TcpConnection connection = message.Connection;
			message.Headers[CommonTransportKeys.IPAddress] = connection.RemoteAddress;
			message.Headers[CommonTransportKeys.ConnectionId] = connection.ID;
			message.Headers["__CustomErrorsEnabled"] = false;

			string uri = (string)message.Headers[CommonTransportKeys.RequestUri];
			TcpChannel.ParseChannelUrl(uri, out uri);

			if(uri != null)
				message.Headers[CommonTransportKeys.RequestUri] = uri;

			ServerChannelSinkStack sinkStack = new ServerChannelSinkStack();
			sinkStack.Push(this, message);

			IMessage responseMessage;
			Message response = new Message();
			switch(nextSink.ProcessMessage(sinkStack, null, message.Headers, message.Stream, out responseMessage, out response.Headers, out response.Stream))
			{
			case ServerProcessing.Complete:
				response.Type = MessageType.Response;
				response.ID = message.ID;
				connection.SendMessage(response);
				break;
			}
		}

		void IServerChannelSink.AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			Message request = (Message)state;
			Message response = new Message { Type = MessageType.Response, ID = request.ID, Headers = headers, Stream = stream };
			request.Connection.SendMessage(response);
		}

		Stream IServerChannelSink.GetResponseStream (IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
		{
			Message request = (Message)state;
			Message response = new Message { Type = MessageType.Response, ID = request.ID, Headers = headers, Stream = null };
			return request.Connection.SendMessage(response);
		}

		ServerProcessing IServerChannelSink.ProcessMessage (IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			throw new NotSupportedException();
		}
	}
}

