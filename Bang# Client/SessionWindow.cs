// SessionWindow.cs
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
using Mono.Unix;

namespace BangSharp.Client
{
	public partial class SessionWindow : Gtk.Window
	{
		private class EventListener : VirtualSessionEventListener
		{
			private SessionWindow parent;

			public EventListener(SessionWindow parent)
			{
				this.parent = parent;
			}

			public override void OnChatMessage(IPlayer player, string message)
			{
				Gdk.Threads.Enter();
				Gtk.TextBuffer buffer = parent.chatView.Buffer;
				Gtk.TextIter ti = buffer.EndIter;
				buffer.InsertWithTagsByName(ref ti, player.Name + ": ", "player");
				buffer.Insert(ref ti, message + "\n");
				parent.NotifyUrgent();
				Gdk.Threads.Leave();
			}
			public override void OnChatMessage(ISpectator spectator, string message)
			{
				Gdk.Threads.Enter();
				Gtk.TextBuffer buffer = parent.chatView.Buffer;
				Gtk.TextIter ti = buffer.EndIter;
				buffer.InsertWithTagsByName(ref ti, spectator.Name + ": ", "spectator");
				buffer.Insert(ref ti, message + "\n");
				parent.NotifyUrgent();
				Gdk.Threads.Leave();
			}

			public override void OnGameEnded()
			{
				Gdk.Threads.Enter();
				IPlayerSessionControl playerControl = ConnectionManager.PlayerSessionControl;
				if(playerControl != null && playerControl.Player.IsCreator)
					parent.startGameButton.Sensitive = true;
				else
					parent.startGameButton.Sensitive = false;
				Gdk.Threads.Leave();
			}
		}
		private EventListener listener;

		public SessionWindow(ServerWindow parent) :
			base(Gtk.WindowType.Toplevel)
		{
			TransientFor = parent;

			this.Build();
			this.chatSendButton.TooltipMarkup = Catalog.GetString("Sends the message");
			this.startGameButton.TooltipMarkup = Catalog.GetString("Starts the game in this session");
			this.disconnectButton.TooltipMarkup = Catalog.GetString("Disconnects from the session");
			this.endSessionButton.TooltipMarkup = Catalog.GetString("Ends the session");
			
			listener = new EventListener(this);
			ConnectionManager.SessionEventListener.AddListener((IPlayerSessionEventListener)listener);
			ConnectionManager.SessionEventListener.AddListener((ISpectatorSessionEventListener)listener);
			ConnectionManager.OnGameConnected += () => {
				Gdk.Threads.Enter();
				startGameButton.Sensitive = false;
				Gdk.Threads.Leave();
			};

			ISession session = ConnectionManager.Session;
			IPlayerSessionControl playerControl = ConnectionManager.PlayerSessionControl;
			if(playerControl != null && playerControl.Player.IsCreator)
			{
				if(session.State != SessionState.Playing && session.State != SessionState.Ended)
					startGameButton.Sensitive = true;
				else
					startGameButton.Sensitive = false;
				endSessionButton.Sensitive = true;
			}
			else
			{
				startGameButton.Sensitive = false;
				endSessionButton.Sensitive = false;
			}
			Gtk.TextTag playerTag = new Gtk.TextTag("player");
			playerTag.Weight = Pango.Weight.Bold;
			playerTag.Foreground = "blue";

			Gtk.TextTag spectatorTag = new Gtk.TextTag("spectator");
			spectatorTag.Weight = Pango.Weight.Bold;
			spectatorTag.Foreground = "yellow";

			chatView.Buffer.TagTable.Add(playerTag);
			chatView.Buffer.TagTable.Add(spectatorTag);

			sessionInfoWidget.Session = ConnectionManager.Session;
		}

		private void NotifyUrgent()
		{
			if(!HasToplevelFocus)
				UrgencyHint = true;
		}

		protected void OnDestroyEvent(object o, Gtk.DestroyEventArgs args)
		{
			ConnectionManager.SessionEventListener.RemoveListener(listener);
		}

		protected void OnChatSendButtonClicked(object sender, System.EventArgs e)
		{
			chatEntry.Sensitive = false;
			System.Threading.ThreadPool.QueueUserWorkItem((state) => {
				string message = chatEntry.Text;
				chatEntry.Text = "";
				try
				{
					if(ConnectionManager.PlayerSessionControl != null)
						ConnectionManager.PlayerSessionControl.SendChatMessage(message);
					if(ConnectionManager.SpectatorSessionControl != null)
						ConnectionManager.SpectatorSessionControl.SendChatMessage(message);
				}
				catch(Exception ex)
				{
					Gtk.Application.Invoke(delegate {
						Gdk.Threads.Enter();
						ErrorManager.ShowErrorMessage(this, MessageManager.GetErrorMessage(ex));
						chatEntry.Sensitive = true;
						Gdk.Threads.Leave();
					});
					return;
				}
				Gtk.Application.Invoke(delegate {
					Gdk.Threads.Enter();
					chatEntry.Sensitive = true;
					Gdk.Threads.Leave();
				});
			});
		}

		protected void OnStartGameButtonClicked(object sender, System.EventArgs e)
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

		protected void OnDisconnectButtonClicked(object sender, System.EventArgs e)
		{
			System.Threading.ThreadPool.QueueUserWorkItem((state) => {
				ConnectionManager.DisconnectFromSession();
			});
		}

		protected void OnEndSessionButtonClicked(object sender, System.EventArgs e)
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

		protected void OnFocusInEvent(object o, Gtk.FocusInEventArgs args)
		{
			UrgencyHint = false;
		}
	}
}

