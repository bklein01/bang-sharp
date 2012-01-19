// TcpClientClientSinkProvider.cs
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
using System.Collections.Generic;

namespace System.Runtime.Remoting.Channels.TwoWayTcp
{
	internal class TcpClientSinkProvider : IClientChannelSinkProvider
	{
		private TcpConnectionPool pool;
		private Dictionary<TcpConnection, TcpClientSink> sinkCache;

		IClientChannelSinkProvider IClientChannelSinkProvider.Next
		{
			get { return null; }
			set { }
		}

		public TcpClientSinkProvider(TcpConnectionPool pool)
		{
			this.pool = pool;
			sinkCache = new Dictionary<TcpConnection, TcpClientSink>();
		}

		private TcpClientSink GetSink(TcpConnection connection)
		{
			try
			{
				return sinkCache[connection];
			}
			catch(KeyNotFoundException)
			{
				return sinkCache[connection] = new TcpClientSink(connection);
			}
		}

		IClientChannelSink IClientChannelSinkProvider.CreateSink(IChannelSender channel, string url, object remoteChannelData)
		{
			string host, port, objUri;
			if(TcpChannel.ParseTcpUrl(url, out host, out port, out objUri) == null)
				return null;

			if(port != null)
			{
				int p;
				try
				{
					p = int.Parse(port);
				}
				catch(FormatException)
				{
					return null;
				}
				TcpConnection conn = pool.GetConnection(host, p);
				if(conn == null)
					return null;
				return GetSink(conn);
			}
			else
			{
				Guid id;
				try
				{
					id = new Guid(host);
				}
				catch(FormatException)
				{
					return null;
				}
				TcpConnection conn = pool.GetConnection(id);
				if(conn == null)
					return null;
				return GetSink(conn);
			}
		}
	}
}
