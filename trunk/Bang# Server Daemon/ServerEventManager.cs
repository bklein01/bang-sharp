// ServerEventManager.cs
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
using System.Linq;

namespace BangSharp.Server
{
	public sealed class ServerEventManager
	{
		private Server server;
		private List<IServerEventListener> listeners;

		public ServerEventManager(Server server)
		{
			this.server = server;
			listeners = new List<IServerEventListener>();
		}

		public void RegisterListener(IServerEventListener listener)
		{
			if(!listeners.Any(l => l == listener))
				listeners.Add(listener);
		}
		public void UnregisterListener(IServerEventListener listener)
		{
			int index = -1;
			for(int i = 0; i < listeners.Count; i++)
				if(listeners[i] == listener)
				{
					index = i;
					break;
				}
			if(index >= 0)
				listeners.RemoveAt(index);
		}

		public void OnSessionCreated(Session session)
		{
			session.Locked = true;
			List<IServerEventListener > listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				try
				{
					l.OnSessionCreated(session);
				}
				catch(Exception e)
				{
					Console.Error.WriteLine("INFO: Exception thrown by client:");
					Console.Error.WriteLine(e);
					UnregisterListener(l);
				}
			session.Locked = false;
		}
		public void OnSessionEnded(Session session)
		{
			session.Locked = true;
			List<IServerEventListener > listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				try
				{
					l.OnSessionEnded(session);
				}
				catch(Exception e)
				{
					Console.Error.WriteLine("INFO: Exception thrown by client:");
					Console.Error.WriteLine(e);
					UnregisterListener(l);
				}
			session.Locked = false;
		}

		public void OnGameStarted(Session session)
		{
			session.Locked = true;
			List<IServerEventListener > listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				try
				{
					l.OnGameStarted(session);
				}
				catch(Exception e)
				{
					Console.Error.WriteLine("INFO: Exception thrown by client:");
					Console.Error.WriteLine(e);
					UnregisterListener(l);
				}
			session.Locked = false;
		}
		public void OnGameEnded(Session session)
		{
			session.Locked = true;
			List<IServerEventListener > listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				try
				{
					l.OnGameEnded(session);
				}
				catch(Exception e)
				{
					Console.Error.WriteLine("INFO: Exception thrown by client:");
					Console.Error.WriteLine(e);
					UnregisterListener(l);
				}
			session.Locked = false;
		}

		public void OnPlayerJoinedSession(Session session, SessionPlayer player)
		{
			session.Locked = true;
			List<IServerEventListener> listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				try
				{
					l.OnPlayerJoinedSession(session, player);
				}
				catch(Exception e)
				{
					Console.Error.WriteLine("INFO: Exception thrown by client:");
					Console.Error.WriteLine(e);
					UnregisterListener(l);
				}
			session.Locked = false;
		}
		public void OnSpectatorJoinedSession(Session session, SessionSpectator spectator)
		{
			session.Locked = true;
			List<IServerEventListener > listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				try
				{
					l.OnSpectatorJoinedSession(session, spectator);
				}
				catch(Exception e)
				{
					Console.Error.WriteLine("INFO: Exception thrown by client:");
					Console.Error.WriteLine(e);
					UnregisterListener(l);
				}
			session.Locked = false;
		}

		public void OnPlayerLeftSession(Session session, SessionPlayer player)
		{
			session.Locked = true;
			List<IServerEventListener > listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				try
				{
					l.OnPlayerLeftSession(session, player);
				}
				catch(Exception e)
				{
					Console.Error.WriteLine("INFO: Exception thrown by client:");
					Console.Error.WriteLine(e);
					UnregisterListener(l);
				}
			session.Locked = false;
		}
		public void OnSpectatorLeftSession(Session session, SessionSpectator spectator)
		{
			session.Locked = true;
			List<IServerEventListener > listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				try
				{
					l.OnSpectatorLeftSession(session, spectator);
				}
				catch(Exception e)
				{
					Console.Error.WriteLine("INFO: Exception thrown by client:");
					Console.Error.WriteLine(e);
					UnregisterListener(l);
				}
			session.Locked = false;
		}

		public void OnPlayerUpdated(Session session, SessionPlayer player)
		{
			session.Locked = true;
			List<IServerEventListener > listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				try
				{
					l.OnPlayerUpdated(session, player);
				}
				catch(Exception e)
				{
					Console.Error.WriteLine("INFO: Exception thrown by client:");
					Console.Error.WriteLine(e);
					UnregisterListener(l);
				}
			session.Locked = false;
		}
	}
}

