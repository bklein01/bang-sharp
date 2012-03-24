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
using System.Runtime.Remoting;

namespace BangSharp.Server.Daemon
{
	public sealed class ServerEventManager
	{
		private sealed class EventSender
		{
			public delegate void Event(IServerEventListener s);

			private ServerEventManager eventMgr;

			public ServerEventManager EventMgr
			{
				get { return eventMgr; }
			}

			public EventSender(ServerEventManager eventMgr)
			{
				this.eventMgr = eventMgr;
			}

			public void SendEvent(Event ev, IServerEventListener s)
			{
				try
				{
					ev(s);
				}
				catch(RemotingTimeoutException)
				{
					OnTimeout(s);
				}
				catch(Exception e)
				{
					OnError(s, e);
				}
			}

			private void OnError(IServerEventListener s, Exception e)
			{
				Console.Error.WriteLine("INFO: Exception thrown by client:");
				Console.Error.WriteLine(e);
				eventMgr.UnregisterListener(s);
			}
			private void OnTimeout(IServerEventListener s)
			{
				Console.Error.WriteLine("INFO: Client event timed out!");
				eventMgr.UnregisterListener(s);
			}
		}
		private Server server;
		private EventSender sender;
		private List<IServerEventListener> listeners;

		public ServerEventManager(Server server)
		{
			this.server = server;
			sender = new EventSender(this);
			listeners = new List<IServerEventListener>();
		}

		public void RegisterListener(IServerEventListener listener)
		{
			lock(server.Lock)
				if(!listeners.Any(l => l == listener))
					listeners.Add(listener);
		}
		public void UnregisterListener(IServerEventListener listener)
		{
			lock(server.Lock)
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
		}

		public void OnSessionCreated(Session session)
		{
			List<IServerEventListener> listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				sender.SendEvent(li => {
					li.OnSessionCreated(session);
				}, l);
		}
		public void OnSessionEnded(Session session)
		{
			List<IServerEventListener> listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				sender.SendEvent(li => {
					li.OnSessionEnded(session);
				}, l);
		}

		public void OnGameStarted(Session session)
		{
			List<IServerEventListener> listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				sender.SendEvent(li => {
					li.OnGameStarted(session);
				}, l);
		}
		public void OnGameEnded(Session session)
		{
			List<IServerEventListener> listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				sender.SendEvent(li => {
					li.OnGameEnded(session);
				}, l);
		}

		public void OnPlayerJoinedSession(Session session, SessionPlayer player)
		{
			List<IServerEventListener> listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				sender.SendEvent(li => {
					li.OnPlayerJoinedSession(session, player);
				}, l);
		}
		public void OnSpectatorJoinedSession(Session session, SessionSpectator spectator)
		{
			List<IServerEventListener> listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				sender.SendEvent(li => {
					li.OnSpectatorJoinedSession(session, spectator);
				}, l);
		}

		public void OnPlayerLeftSession(Session session, SessionPlayer player)
		{
			List<IServerEventListener> listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				sender.SendEvent(li => {
					li.OnPlayerLeftSession(session, player);
				}, l);
		}
		public void OnSpectatorLeftSession(Session session, SessionSpectator spectator)
		{
			List<IServerEventListener> listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				sender.SendEvent(li => {
					li.OnSpectatorLeftSession(session, spectator);
				}, l);
		}

		public void OnPlayerUpdated(Session session, SessionPlayer player)
		{
			List<IServerEventListener> listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				sender.SendEvent(li => {
					li.OnPlayerUpdated(session, player);
				}, l);
		}
		public void OnPlayerDisconnected(Session session, SessionPlayer player)
		{
			List<IServerEventListener> listeners = new List<IServerEventListener>(this.listeners);
			foreach(IServerEventListener l in listeners)
				sender.SendEvent(li => {
					li.OnPlayerDisconnected(session, player);
				}, l);
		}
	}
}

