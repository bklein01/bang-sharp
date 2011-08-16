// TcpConnectionPool.cs
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
using System.Collections.Generic;
using System.Net.Sockets;
namespace System.Runtime.Remoting.Channels.TwoWayTcp
{
	internal class TcpConnectionPool
	{
		public static readonly TcpConnectionPool Instance = new TcpConnectionPool();

		private Dictionary<string, TcpConnection> hostConnections;
		private List<TcpConnection> connections;

		public OnMessageRecieved OnRequestRecieved;

		private TcpConnectionPool()
		{
			hostConnections = new Dictionary<string, TcpConnection>();
			connections = new List<TcpConnection>();
		}

		public TcpConnection GetConnection(string host, int port)
		{
			string key = host + ":" + port;
			TcpConnection conn = null;
			try
			{
				conn = hostConnections[key];
			}
			catch(KeyNotFoundException)
			{
			}
			if(conn != null && !conn.IsAlive)
			{
				connections.Remove(conn);
				conn = null;
			}

			if(conn == null)
			{
				TcpClient client = new TcpClient(host, port);
				conn = new TcpConnection(this, client.Client);
				hostConnections[key] = conn;
				AddConnection(conn);
			}
			return conn;
		}
		public TcpConnection GetConnection(Guid id)
		{
			foreach(TcpConnection conn in connections)
				if(conn.MachineID == id)
					return conn;
			return null;
		}

		public TcpConnection WaitForConnection(TcpListener listener)
		{
			TcpConnection conn = new TcpConnection(this, listener.AcceptSocket());
			AddConnection(conn);
			return conn;
		}

		private void AddConnection(TcpConnection conn)
		{
			conn.OnMessageRecieved += delegate(Message message)
			{
				if(message.Type == MessageType.Request)
					OnRequestRecieved(message);
			};
			connections.Add(conn);
			conn.StartListening();
		}
		public void RemoveConnection(TcpConnection conn)
		{
			connections.Remove(conn);
		}

		public void PurgeConnections()
		{
			foreach(TcpConnection conn in new List<TcpConnection>(connections))
				conn.StopListening();
		}
	}
}

