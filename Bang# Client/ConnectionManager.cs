using System;
// ConnectionManager.cs
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

namespace BangSharp.Client
{
	/// <summary>
	/// Manages server, control and listener objects.
	/// </summary>
	public static class ConnectionManager
	{
		private class EventListener : VirtualSessionEventListener
		{
			public EventListener()
			{
			}

			public override void OnJoinedSession(IPlayerSessionControl control)
			{
				lock(ConnectionManager.Lock)
				{
					DisconnectFromSession();
					ConnectionManager.playerSessionControl = control;
					ConnectionManager.session = control.Session;
				}
			}

			public override void OnJoinedGame(IPlayerControl control)
			{
				lock(ConnectionManager.Lock)
				{
					ConnectionManager.playerGameControl = control;
					ConnectionManager.game = control.Game;
				}
			}

			public override void OnJoinedSession(ISpectatorSessionControl control)
			{
				DisconnectFromSession();
				lock(ConnectionManager.Lock)
				{
					ConnectionManager.spectatorSessionControl = control;
					ConnectionManager.session = control.Session;
				}
			}

			public override void OnJoinedGame(ISpectatorControl control)
			{
				lock(ConnectionManager.Lock)
				{
					ConnectionManager.spectatorGameControl = control;
					ConnectionManager.game = control.Game;
				}
			}

			public override void OnSessionEnded()
			{
				lock(ConnectionManager.Lock)
				{
					ConnectionManager.playerSessionControl = null;
					ConnectionManager.spectatorSessionControl = null;
					ConnectionManager.session = null;

					ConnectionManager.playerGameControl = null;
					ConnectionManager.spectatorGameControl = null;
					ConnectionManager.game = null;
				}
			}
		}

		private static readonly object Lock = new object();
		private static IServer server;
		private static ISession session;
		private static IGame game;

		private static ProxyServerEventListener serverListener;
		private static ProxySessionEventListener sessionListener;

		private static IPlayerSessionControl playerSessionControl;
		private static ISpectatorSessionControl spectatorSessionControl;

		private static IPlayerControl playerGameControl;
		private static ISpectatorControl spectatorGameControl;

		/// <summary>
		/// Gets the currently connected server object.
		/// </summary>
		/// <value>
		/// The server object or <c>null<c> if not connected to a server.
		/// </value>
		public static IServer Server
		{
			get { return server; }
		}
		/// <summary>
		/// Gets the currently connected session object.
		/// </summary>
		/// <value>
		/// The session object or <c>null<c> if not connected to a session.
		/// </value>
		public static ISession Session
		{
			get { return session; }
		}
		/// <summary>
		/// Gets the currently connected game object.
		/// </summary>
		/// <value>
		/// The game object or <c>null<c> if not connected to a game.
		/// </value>
		public static IGame Game
		{
			get { return game; }
		}

		/// <summary>
		/// Gets a value indicating whether we are connected to a server.
		/// </summary>
		/// <value>
		/// <c>true</c> if connected to a server; otherwise, <c>false</c>.
		/// </value>
		public static bool ServerConnected
		{
			get { return server != null; }
		}
		/// <summary>
		/// Gets a value indicating whether we are connected to a session.
		/// </summary>
		/// <value>
		/// <c>true</c> if connected to a session; otherwise, <c>false</c>.
		/// </value>
		public static bool SessionConnected
		{
			get { return session != null; }
		}
		/// <summary>
		/// Gets a value indicating whether we are connected to a game.
		/// </summary>
		/// <value>
		/// <c>true</c> if connected to a game; otherwise, <c>false</c>.
		/// </value>
		public static bool GameConnected
		{
			get { return game != null; }
		}

		/// <summary>
		/// Gets the main server event listener.
		/// </summary>
		/// <value>
		/// The main server event listener.
		/// </value>
		/// <remarks>
		/// Secondary event listeners can be added to this proxy listener.
		/// </remarks>
		public static ProxyServerEventListener ServerEventListener
		{
			get { return serverListener; }
		}
		/// <summary>
		/// Gets the main session event listener.
		/// </summary>
		/// <value>
		/// The main session event listener.
		/// </value>
		/// <remarks>
		/// Secondary event listeners can be added to this proxy listener.
		/// </remarks>
		public static ProxySessionEventListener SessionEventListener
		{
			get { return sessionListener; }
		}

		public static event Action OnServerDisconnected;
		public static event Action OnSessionDisconnected;

		/// <summary>
		/// Gets the player session control.
		/// </summary>
		/// <value>
		/// The player session control or <c>null</c> if none is available.
		/// </value>
		public static IPlayerSessionControl PlayerSessionControl
		{
			get { return playerSessionControl; }
		}
		/// <summary>
		/// Gets the spectator session control.
		/// </summary>
		/// <value>
		/// The spectator session control or <c>null</c> if none is available.
		/// </value>
		public static ISpectatorSessionControl SpectatorSessionControl
		{
			get { return spectatorSessionControl; }
		}

		/// <summary>
		/// Gets the player game control.
		/// </summary>
		/// <value>
		/// The player game control or <c>null</c> if none is available.
		/// </value>
		public static IPlayerControl PlayerGameControl
		{
			get { return playerGameControl; }
		}
		/// <summary>
		/// Gets the spectator game control.
		/// </summary>
		/// <value>
		/// The spectator game control or <c>null</c> if none is available.
		/// </value>
		public static ISpectatorControl SpectatorGameControl
		{
			get { return spectatorGameControl; }
		}

		static ConnectionManager()
		{
			lock(Lock)
			{
				serverListener = new ProxyServerEventListener();
				sessionListener = new ProxySessionEventListener();
				EventListener l = new EventListener();
				sessionListener.AddListener((IPlayerSessionEventListener)l);
				sessionListener.AddListener((ISpectatorSessionEventListener)l);
			}
		}

		/// <summary>
		/// Connects to the specified server.
		/// </summary>
		/// <param name="address">
		/// The server address.
		/// </param>
		/// <param name="port">
		/// The server port.
		/// </param>
		public static void ConnectToServer(string address, int port)
		{
			lock(Lock)
			{
				DisconnectFromServer();
				server = Utils.Connect(address, port);
				server.RegisterListener(serverListener);
			}
		}
		/// <summary>
		/// Disconnects from the curently connected server.
		/// </summary>
		public static void DisconnectFromServer()
		{
			lock(Lock)
			{
				if(!ServerConnected)
					return;
				server.UnregisterListener(serverListener);
				server = null;
				if(OnServerDisconnected != null)
					OnServerDisconnected();
			}
		}

		/// <summary>
		/// Disconnects from the currently connected session.
		/// </summary>
		public static void DisconnectFromSession()
		{
			lock(Lock)
			{
				if(!SessionConnected)
					return;
				if(ConnectionManager.playerSessionControl != null)
					ConnectionManager.playerSessionControl.Disconnect();
				if(ConnectionManager.spectatorSessionControl != null)
					ConnectionManager.spectatorSessionControl.Disconnect();
				session = null;
				if(OnSessionDisconnected != null)
					OnSessionDisconnected();
			}
		}
	}
}

