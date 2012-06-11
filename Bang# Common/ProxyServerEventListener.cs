// ProxyServerEventListener.cs
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

namespace BangSharp
{
	/// <summary>
	/// Proxy server event listener.
	/// </summary>
	/// <remarks>
	/// This class can be used to connect multiple server event listeners through one listener.
	/// </remarks>
	public class ProxyServerEventListener : ImmortalMarshalByRefObject, IServerEventListener
	{
		private object syncLock = new object();
		private List<IServerEventListener> listeners;

		/// <summary>
		/// Gets the synchronization lock.
		/// </summary>
		/// <value>
		/// The object that is locked while calling the member listeners' methods.
		/// </value>
		public object SyncLock
		{
			get { return syncLock; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref='BangSharp.ProxyServerEventListener'/> class.
		/// </summary>
		public ProxyServerEventListener()
		{
			listeners = new List<IServerEventListener>();
		}

		/// <summary>
		/// Adds the listener to the list of member listeners.
		/// </summary>
		/// <param name='listener'>
		/// The listener to add.
		/// </param>
		public void AddListener(IServerEventListener listener)
		{
			lock(syncLock)
			{
				if(!listeners.Contains(listener))
					listeners.Add(listener);
			}
		}

		/// <summary>
		/// Removes the listener from the list of member listeners.
		/// </summary>
		/// <param name='listener'>
		/// The listener to remove.
		/// </param>
		public void RemoveListener(IServerEventListener listener)
		{
			lock(syncLock)
				listeners.Remove(listener);
		}

		#region IServerEventListener implementation
		void IServerEventListener.Ping()
		{
		}

		void IServerEventListener.OnSessionCreated(ISession session)
		{
			lock(syncLock)
			{
				foreach(IServerEventListener listener in listeners.ToArray())
					listener.OnSessionCreated(session);
			}
		}

		void IServerEventListener.OnSessionEnded(ISession session)
		{
			lock(syncLock)
			{
				foreach(IServerEventListener listener in listeners.ToArray())
					listener.OnSessionEnded(session);
			}
		}

		void IServerEventListener.OnGameStarted(ISession session)
		{
			lock(syncLock)
			{
				foreach(IServerEventListener listener in listeners.ToArray())
					listener.OnGameStarted(session);
			}
		}

		void IServerEventListener.OnGameEnded(ISession session)
		{
			lock(syncLock)
			{
				foreach(IServerEventListener listener in listeners.ToArray())
					listener.OnGameEnded(session);
			}
		}

		void IServerEventListener.OnPlayerJoinedSession(ISession session, IPlayer player)
		{
			lock(syncLock)
			{
				foreach(IServerEventListener listener in listeners.ToArray())
					listener.OnPlayerJoinedSession(session, player);
			}
		}

		void IServerEventListener.OnSpectatorJoinedSession(ISession session, ISpectator spectator)
		{
			lock(syncLock)
			{
				foreach(IServerEventListener listener in listeners.ToArray())
					listener.OnSpectatorJoinedSession(session, spectator);
			}
		}

		void IServerEventListener.OnPlayerLeftSession(ISession session, IPlayer player)
		{
			lock(syncLock)
			{
				foreach(IServerEventListener listener in listeners.ToArray())
					listener.OnPlayerLeftSession(session, player);
			}
		}

		void IServerEventListener.OnSpectatorLeftSession(ISession session, ISpectator spectator)
		{
			lock(syncLock)
			{
				foreach(IServerEventListener listener in listeners.ToArray())
					listener.OnSpectatorLeftSession(session, spectator);
			}
		}

		void IServerEventListener.OnPlayerUpdated(ISession session, IPlayer player)
		{
			lock(syncLock)
			{
				foreach(IServerEventListener listener in listeners.ToArray())
					listener.OnPlayerUpdated(session, player);
			}
		}

		void IServerEventListener.OnPlayerDisconnected(ISession session, IPlayer player)
		{
			lock(syncLock)
			{
				foreach(IServerEventListener listener in listeners.ToArray())
					listener.OnPlayerDisconnected(session, player);
			}
		}
		#endregion
	}
}

