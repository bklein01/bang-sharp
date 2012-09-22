// ServerWindow.cs
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
using Mono.Unix;

namespace BangSharp.Client
{
	public partial class ServerWindow : Gtk.Window
	{
		private class EventListener : VirtualServerEventListener
		{
			private ServerWindow parent;

			public EventListener(ServerWindow parent)
			{
				this.parent = parent;
			}

			public override void OnSessionCreated(ISession session)
			{
				Gdk.Threads.Enter();
				parent.sessionStore.AddNode(parent.GetSessionNode(session));
				Gdk.Threads.Leave();
			}
			public override void OnSessionEnded(ISession session)
			{
				Gdk.Threads.Enter();
				parent.sessionStore.RemoveNode(parent.GetSessionNode(session));
				parent.RemoveSessionNode(session);
				Gdk.Threads.Leave();
			}
		}
		[Gtk.TreeNode]
		private class SessionNode : Gtk.TreeNode
		{
			private ISession session;

			public ISession Session
			{
				get { return session; }
			}

			[Gtk.TreeNodeValue(Column=0)]
			public new int ID
			{
				get { return session.ID; }
			}
			[Gtk.TreeNodeValue(Column=1)]
			public string Name
			{
				get { return session.Name; }
			}
			[Gtk.TreeNodeValue(Column=2)]
			public string State
			{
				get { return MessageManager.GetSessionStateString(session.State); }
			}
			[Gtk.TreeNodeValue(Column=3)]
			public int PlayerCount
			{
				get { return session.Players.Count; }
			}
			[Gtk.TreeNodeValue(Column=4)]
			public int MinPlayerCount
			{
				get { return session.MinPlayers; }
			}
			[Gtk.TreeNodeValue(Column=5)]
			public int MaxPlayerCount
			{
				get { return session.MaxPlayers; }
			}
			[Gtk.TreeNodeValue(Column=6)]
			public int SpectatorCount
			{
				get { return session.Spectators.Count; }
			}
			[Gtk.TreeNodeValue(Column=7)]
			public int MaxSpectatorCount
			{
				get { return session.MaxSpectators; }
			}
			[Gtk.TreeNodeValue(Column=8)]
			public bool HasPlayerPassword
			{
				get { return session.HasPlayerPassword; }
			}
			[Gtk.TreeNodeValue(Column=9)]
			public bool HasSpectatorPassword
			{
				get { return session.HasSpectatorPassword; }
			}
			[Gtk.TreeNodeValue(Column=10)]
			public int GamesPlayed
			{
				get { return session.GamesPlayed; }
			}

			public SessionNode(ISession session)
			{
				this.session = session;
			}
		}
		private EventListener listener;
		private Gtk.NodeStore sessionStore;
		private Dictionary<int, SessionNode> sessionNodes;

		public ServerWindow(MainWindow parent) :
			base(Gtk.WindowType.Toplevel)
		{
			TransientFor = parent;
			this.Build();
			this.createSessionButton.TooltipMarkup = Catalog.GetString("Creates a new session on the server");
			this.joinSessionButton.TooltipMarkup = Catalog.GetString("Joins the selected session");
			this.replaceButton.TooltipMarkup = Catalog.GetString("Replaces the selected player of the selected session");
			this.spectateSessionButton.TooltipMarkup = Catalog.GetString("Joins the selected session as a spectator");
			this.disconnectButton.TooltipMarkup = Catalog.GetString("Disconnects from the server");
			
			listener = new EventListener(this);
			ConnectionManager.ServerEventListener.AddListener(listener);

			sessionStore = new Gtk.NodeStore(typeof(SessionNode));
			sessionNodes = new Dictionary<int, SessionNode>();

			sessionsView.NodeStore = sessionStore;

			sessionsView.AppendColumn(Catalog.GetString("ID"), new Gtk.CellRendererSpin(), "text", 0);
			sessionsView.AppendColumn(Catalog.GetString("Name"), new Gtk.CellRendererText(), "text", 1);
			sessionsView.AppendColumn(Catalog.GetString("State"), new Gtk.CellRendererText(), "text", 2);
			sessionsView.AppendColumn(Catalog.GetString("Pl."), new Gtk.CellRendererSpin(), "text", 3);
			sessionsView.AppendColumn(Catalog.GetString("Max. Pl."), new Gtk.CellRendererSpin(), "text", 4);
			sessionsView.AppendColumn(Catalog.GetString("Min. Pl."), new Gtk.CellRendererSpin(), "text", 5);
			sessionsView.AppendColumn(Catalog.GetString("Sp."), new Gtk.CellRendererSpin(), "text", 6);
			sessionsView.AppendColumn(Catalog.GetString("Max. Sp."), new Gtk.CellRendererSpin(), "text", 7);
			sessionsView.AppendColumn(Catalog.GetString("Has Pl. Pass."), new Gtk.CellRendererToggle(), "active", 8);
			sessionsView.AppendColumn(Catalog.GetString("Has Sp. Pass."), new Gtk.CellRendererToggle(), "active", 9);
			sessionsView.AppendColumn(Catalog.GetString("Games Played"), new Gtk.CellRendererSpin(), "text", 10);

			sessionsView.ShowExpanders = false;

			IServer server = ConnectionManager.Server;

			serverNameLabel.LabelProp = server.Name;
			serverDescriptionLabel.LabelProp = server.Description;
			serverInterfaceVersionLabel.LabelProp = server.InterfaceVersionMajor + "." + server.InterfaceVersionMinor;

			foreach(ISession session in server.Sessions)
				sessionStore.AddNode(GetSessionNode(session));

			sessionsView.NodeSelection.Changed += OnSessionSelectionChanged;

			sessionInfoWidget.OnSelectedPlayerChanged += OnSelectedPlayerChanged;
		}

		protected void OnDestroyEvent(object o, Gtk.DestroyEventArgs args)
		{
			ConnectionManager.ServerEventListener.RemoveListener(listener);
			sessionStore.Dispose();
		}

		void OnSessionSelectionChanged(object sender, EventArgs e)
		{
			Gtk.NodeSelection sel = sessionsView.NodeSelection;
			if(sel.SelectedNode == null)
			{
				sessionInfoWidget.Sensitive = false;
				joinSessionButton.Sensitive = false;
				spectateSessionButton.Sensitive = false;
				sessionInfoWidget.Session = null;
			}
			else
			{
				sessionInfoWidget.Sensitive = true;
				joinSessionButton.Sensitive = true;
				spectateSessionButton.Sensitive = true;
				SessionNode node = (SessionNode)sel.SelectedNode;
				sessionInfoWidget.Session = node.Session;
			}
		}
		void OnSelectedPlayerChanged()
		{
			if(sessionInfoWidget.SelectedPlayer == null)
				replaceButton.Sensitive = false;
			else
				replaceButton.Sensitive = true;
		}

		private SessionNode GetSessionNode(ISession session)
		{
			int sessionId = session.ID;
			try
			{
				return sessionNodes[sessionId];
			}
			catch(KeyNotFoundException)
			{
				return sessionNodes[sessionId] = new SessionNode(session);
			}
		}
		private void RemoveSessionNode(ISession session)
		{
			int sessionId = session.ID;
			sessionNodes.Remove(sessionId);
		}

		protected void OnCreateSessionButtonClicked(object sender, System.EventArgs e)
		{
			CreateSessionDialog dialog = new CreateSessionDialog(this);
			dialog.Show();
		}

		protected void OnJoinSessionButtonClicked(object sender, System.EventArgs e)
		{
			Gtk.NodeSelection sel = sessionsView.NodeSelection;
			if(sel.SelectedNode == null)
				return;
			SessionNode node = (SessionNode)sel.SelectedNode;

			JoinSessionDialog dialog = new JoinSessionDialog(this, node.Session);
			dialog.Show();
		}

		protected void OnReplaceButtonClicked(object sender, System.EventArgs e)
		{
			Gtk.NodeSelection sel = sessionsView.NodeSelection;
			if(sel.SelectedNode == null)
				return;
			SessionNode node = (SessionNode)sel.SelectedNode;

			IPlayer player = sessionInfoWidget.SelectedPlayer;
			if(player == null)
				return;

			ReplacePlayerDialog dialog = new ReplacePlayerDialog(this, node.Session, player);
			dialog.Show();
		}

		protected void OnSpectateSessionButtonClicked(object sender, System.EventArgs e)
		{
			Gtk.NodeSelection sel = sessionsView.NodeSelection;
			if(sel.SelectedNode == null)
				return;
			SessionNode node = (SessionNode)sel.SelectedNode;

			SpectateSessionDialog dialog = new SpectateSessionDialog(this, node.Session);
			dialog.Show();
		}

		protected void OnDisconnectButtonClicked(object sender, System.EventArgs e)
		{
			System.Threading.ThreadPool.QueueUserWorkItem((state) => {
				ConnectionManager.DisconnectFromServer();
			});
		}
	}
}

