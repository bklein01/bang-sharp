// MainWindow.cs
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

namespace BangSharp.Client
{
	public partial class MainWindow : Gtk.Window
	{
		private class EventListener : VirtualSessionEventListener
		{
			private MainWindow parent;

			public EventListener(MainWindow parent)
			{
				this.parent = parent;
			}

			public override void OnGameEnded()
			{
				Gdk.Threads.Enter();
				if(Config.Instance.GetBoolean("Client.AutoHideSWWhilePlaying", true))
					parent.sessionWindowAction.Active = true;

				IPlayerSessionControl playerControl = ConnectionManager.PlayerSessionControl;
				if(playerControl != null && playerControl.Player.IsCreator)
					parent.startGameAction.Sensitive = true;
				else
					parent.startGameAction.Sensitive = false;
				parent.NotifyUrgent();
				Gdk.Threads.Leave();
			}

			public override void OnNewRequest(RequestType requestType, IPublicPlayerView causedBy)
			{
				Gdk.Threads.Enter();
				parent.NotifyUrgent();
				Gdk.Threads.Leave();
			}
		}
		private ServerWindow serverWindow;
		private SessionWindow sessionWindow;
		private EventListener listener;

		public ServerWindow ServerWindow
		{
			get { return serverWindow; }
		}
		public SessionWindow SessionWindow
		{
			get { return sessionWindow; }
		}

		public MainWindow() :
			base(Gtk.WindowType.Toplevel)
		{
			this.Build();

			ConnectionManager.OnServerConnected += () => {
				serverWindow = new ServerWindow(this);
				serverWindow.DeleteEvent += (o, args) => {
					serverWindowAction.Active = false;
					args.RetVal = true;
				};
				serverWindow.Show();
				connectAction.Sensitive = false;
				disconnectAction.Sensitive = true;
				serverWindowAction.Sensitive = true;
				serverWindowAction.Active = true;
			};
			ConnectionManager.OnServerDisconnected += () => {
				Gtk.Application.Invoke(delegate {
					Gdk.Threads.Enter();
					serverWindow.Destroy();
					serverWindow = null;
					connectAction.Sensitive = true;
					disconnectAction.Sensitive = false;
					serverWindowAction.Sensitive = false;
					serverWindowAction.Active = false;
					Gdk.Threads.Leave();
				});
			};
			ConnectionManager.OnSessionConnected += () => {
				Gtk.Application.Invoke(delegate {
					Gdk.Threads.Enter();
					sessionWindow = new SessionWindow(serverWindow);
					sessionWindow.DeleteEvent += (o, args) => {
						sessionWindowAction.Active = false;
						args.RetVal = true;
					};
					sessionWindowAction.Sensitive = true;
					sessionWindowAction.Active = true;
					serverWindowAction.Active = false;

					sessionDisconnectAction.Sensitive = true;
					ISession session = ConnectionManager.Session;
					IPlayerSessionControl playerControl = ConnectionManager.PlayerSessionControl;
					if(playerControl != null && playerControl.Player.IsCreator)
					{
						if(session.State != SessionState.Playing && session.State != SessionState.Ended)
							startGameAction.Sensitive = true;
						else
							startGameAction.Sensitive = false;
						endSessionAction.Sensitive = true;
					}
					else
					{
						startGameAction.Sensitive = false;
						endSessionAction.Sensitive = false;
					}
					Gdk.Threads.Leave();
				});
			};
			ConnectionManager.OnSessionDisconnected += () => {
				Gtk.Application.Invoke(delegate {
					Gdk.Threads.Enter();
					sessionWindow.Destroy();
					sessionWindow = null;
					sessionWindowAction.Sensitive = false;
					sessionWindowAction.Active = false;
					serverWindowAction.Active = true;

					sessionDisconnectAction.Sensitive = false;
					startGameAction.Sensitive = false;
					endSessionAction.Sensitive = false;
					Gdk.Threads.Leave();
				});
			};
			ConnectionManager.OnGameConnected += () => {
				if(Config.Instance.GetBoolean("Client.AutoHideSWWhilePlaying", true))
					Gtk.Application.Invoke(delegate {
						Gdk.Threads.Enter();
						sessionWindowAction.Active = false;
						NotifyUrgent();
						Gdk.Threads.Leave();
					});
			};
			listener = new EventListener(this);
			ConnectionManager.SessionEventListener.AddListener((IPlayerSessionEventListener)listener);
		}

		private void NotifyUrgent()
		{
			if(!HasToplevelFocus)
				UrgencyHint = true;
		}

		private void Quit()
		{
			if(ConnectionManager.ServerConnected)
			{
				ConnectionManager.OnServerDisconnected += () => {
					Destroy();
					Gtk.Application.Quit();
				};
				System.Threading.ThreadPool.QueueUserWorkItem((state) => {
					ConnectionManager.DisconnectFromServer();
				});
			}
			else
			{
				Destroy();
				Gtk.Application.Quit();
			}
		}

		protected void OnDeleteEvent(object o, Gtk.DeleteEventArgs args)
		{
			Quit();
			args.RetVal = true;
		}

		protected void OnConnectActionActivated(object sender, System.EventArgs e)
		{
			ServerListWindow win = new ServerListWindow(this);
			win.Show();
		}

		protected void OnDisconnectActionActivated(object sender, System.EventArgs e)
		{
			System.Threading.ThreadPool.QueueUserWorkItem((state) => {
				ConnectionManager.DisconnectFromServer();
			});
		}

		protected void OnQuitActionActivated(object sender, System.EventArgs e)
		{
			Quit();
		}

		protected void OnServerWindowActionToggled(object sender, System.EventArgs e)
		{
			if(serverWindow == null)
				return;
			if(serverWindowAction.Active)
				serverWindow.Show();
			else
				serverWindow.Hide();
		}

		protected void OnSessionWindowActionToggled(object sender, System.EventArgs e)
		{
			if(sessionWindow == null)
				return;
			if(sessionWindowAction.Active)
				sessionWindow.Show();
			else
				sessionWindow.Hide();
		}

		protected void OnSessionDisconnectActionActivated(object sender, System.EventArgs e)
		{
			System.Threading.ThreadPool.QueueUserWorkItem((state) => {
				ConnectionManager.DisconnectFromSession();
			});
		}

		protected void OnStartGameActionActivated(object sender, System.EventArgs e)
		{
			System.Threading.ThreadPool.QueueUserWorkItem((state) => {
				try
				{
					if(ConnectionManager.PlayerSessionControl != null)
						ConnectionManager.PlayerSessionControl.StartGame();
				}
				catch(Exception ex)
				{
					Gtk.Application.Invoke(delegate {
						Gdk.Threads.Enter();
						ErrorManager.ShowErrorMessage(this, MessageManager.GetErrorMessage(ex));
						Gdk.Threads.Leave();
					});
					return;
				}
			});
		}

		protected void OnEndSessionActionActivated(object sender, System.EventArgs e)
		{
			System.Threading.ThreadPool.QueueUserWorkItem((state) => {
				try
				{
					if(ConnectionManager.PlayerSessionControl != null)
						ConnectionManager.PlayerSessionControl.EndSession();
				}
				catch(Exception ex)
				{
					Gtk.Application.Invoke(delegate {
						Gdk.Threads.Enter();
						ErrorManager.ShowErrorMessage(this, MessageManager.GetErrorMessage(ex));
						Gdk.Threads.Leave();
					});
					return;
				}
			});
		}

		protected void OnPreferencesActionActivated(object sender, System.EventArgs e)
		{
			PreferencesDialog pd = new PreferencesDialog(this);
			pd.Run();
		}

		protected void OnFocusInEvent(object o, Gtk.FocusInEventArgs args)
		{
			UrgencyHint = false;
		}
	}
}

