// SessionInfoWidget.cs
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
	[System.ComponentModel.ToolboxItem(true)]
	public partial class SessionInfoWidget : Gtk.Bin
	{
		private class EventListener : VirtualServerEventListener
		{
			private SessionInfoWidget parent;

			public EventListener(SessionInfoWidget parent)
			{
				this.parent = parent;
			}

			public override void OnPlayerJoinedSession(ISession session, IPlayer player)
			{
				if(session == parent.session)
				{
					Gdk.Threads.Enter();
					PlayerNode node = parent.GetPlayerNode(player);
					parent.playerStore.RemoveNode(node);
					parent.playerStore.AddNode(node);
					Gdk.Threads.Leave();
				}
			}
			public override void OnPlayerUpdated(ISession session, IPlayer player)
			{
				if(session == parent.session)
				{
					Gdk.Threads.Enter();
					parent.GetPlayerNode(player).Update();
					Gdk.Threads.Leave();
				}
			}
			public override void OnPlayerDisconnected(ISession session, IPlayer player)
			{
				if(session == parent.session)
				{
					Gdk.Threads.Enter();
					parent.GetPlayerNode(player).Update();
					Gdk.Threads.Leave();
				}
			}
			public override void OnPlayerLeftSession(ISession session, IPlayer player)
			{
				if(session == parent.session)
				{
					Gdk.Threads.Enter();
					PlayerNode node = parent.GetPlayerNode(player);
					parent.playerStore.RemoveNode(node);
					parent.RemovePlayerNode(player);
					node.Dispose();
					Gdk.Threads.Leave();
				}
			}

			public override void OnSpectatorJoinedSession(ISession session, ISpectator spectator)
			{
				if(session == parent.session)
				{
					Gdk.Threads.Enter();
					SpectatorNode node = parent.GetSpectatorNode(spectator);
					parent.spectatorStore.RemoveNode(node);
					parent.spectatorStore.AddNode(node);
					Gdk.Threads.Leave();
				}
			}
			public override void OnSpectatorLeftSession(ISession session, ISpectator spectator)
			{
				if(session == parent.session)
				{
					Gdk.Threads.Enter();
					SpectatorNode node = parent.GetSpectatorNode(spectator);
					parent.spectatorStore.RemoveNode(node);
					parent.RemoveSpectatorNode(spectator);
					node.Dispose();
					Gdk.Threads.Leave();
				}
			}

			public override void OnGameStarted(ISession session)
			{
				if(session == parent.session)
				{
					Gdk.Threads.Enter();
					parent.stateLabel.LabelProp = MessageManager.GetSessionStateString(session.State);
					Gdk.Threads.Leave();
				}
			}
			public override void OnGameEnded(ISession session)
			{
				if(session == parent.session)
				{
					Gdk.Threads.Enter();
					parent.stateLabel.LabelProp = MessageManager.GetSessionStateString(session.State);
					parent.gamesPlayedLabel.LabelProp = session.GamesPlayed.ToString();
					Gdk.Threads.Leave();
				}
			}
		}
		[Gtk.TreeNode]
		private class PlayerNode : Gtk.TreeNode, IDisposable
		{
			private IPlayer player;
			private Gdk.Pixbuf image;

			[Gtk.TreeNodeValue(Column=0)]
			public new int ID
			{
				get { return player.ID; }
			}
			[Gtk.TreeNodeValue(Column=1)]
			public string Name
			{
				get { return player.Name; }
			}
			[Gtk.TreeNodeValue(Column=2)]
			public Gdk.Pixbuf Image
			{
				get { return image; }
			}
			[Gtk.TreeNodeValue(Column=3)]
			public bool HasPassword
			{
				get { return player.HasPassword; }
			}
			[Gtk.TreeNodeValue(Column=4)]
			public bool HasListener
			{
				get { return player.HasListener; }
			}
			[Gtk.TreeNodeValue(Column=5)]
			public bool IsAI
			{
				get { return player.IsAI; }
			}
			[Gtk.TreeNodeValue(Column=6)]
			public int Score
			{
				get { return player.Score; }
			}

			public PlayerNode(IPlayer player)
			{
				this.player = player;
				Update();
			}

			public void Update()
			{
				Dispose();
				if(player.Image == null)
				{
					image = ResourceManager.GetPixbuf("Resources", "DefaultPlayerImage.png");
					if(image != null)
						image = image.ScaleSimple(16, 16, Gdk.InterpType.Bilinear);
				}
				else
					image = PictureManager.GetPixbuf(player.Image, 16);
			}

			public void Dispose()
			{
				if(image != null)
				{
					image.Dispose();
					image = null;
				}
			}
		}
		[Gtk.TreeNode]
		private class SpectatorNode : Gtk.TreeNode, IDisposable
		{
			private ISpectator spectator;
			private Gdk.Pixbuf image;

			[Gtk.TreeNodeValue(Column=0)]
			public new int ID
			{
				get { return spectator.ID; }
			}
			[Gtk.TreeNodeValue(Column=1)]
			public string Name
			{
				get { return spectator.Name; }
			}
			[Gtk.TreeNodeValue(Column=2)]
			public Gdk.Pixbuf Image
			{
				get { return image; }
			}

			public SpectatorNode(ISpectator spectator)
			{
				this.spectator = spectator;
				Update();
			}

			public void Update()
			{
				Dispose();
				if(spectator.Image == null)
				{
					image = ResourceManager.GetPixbuf("Resources", "DefaultPlayerImage.png");
					if(image != null)
						image = image.ScaleSimple(16, 16, Gdk.InterpType.Bilinear);
				}
				else
					image = PictureManager.GetPixbuf(spectator.Image, 16);
				OnChanged();
			}

			public void Dispose()
			{
				if(image != null)
				{
					image.Dispose();
					image = null;
				}
			}
		}
		private EventListener listener;
		private ISession session;
		private Dictionary<int, PlayerNode> playerNodes;
		private Dictionary<int, SpectatorNode> spectatorNodes;
		private Gtk.NodeStore playerStore;
		private Gtk.NodeStore spectatorStore;

		public ISession Session
		{
			get { return session; }
			set
			{
				session = value;
				if(session != null)
				{
					notebook.Sensitive = true;

					nameLabel.LabelProp = session.Name;
					descriptionLabel.LabelProp = session.Description;
					stateLabel.LabelProp = MessageManager.GetSessionStateString(session.State);
					minPlayerCountLabel.LabelProp = session.MinPlayers.ToString();
					maxPlayerCountLabel.LabelProp = session.MaxPlayers.ToString();
					maxSpectatorCountLabel.LabelProp = session.MaxSpectators.ToString();
					hasPlayerPasswordLabel.LabelProp = MessageManager.GetBooleanString(session.HasPlayerPassword);
					hasSpectatorPasswordLabel.LabelProp = MessageManager.GetBooleanString(session.HasSpectatorPassword);
					dodgeCityLabel.LabelProp = MessageManager.GetBooleanString(session.DodgeCity);
					highNoonLabel.LabelProp = MessageManager.GetBooleanString(session.HighNoon);
					aFistfulOfCardsLabel.LabelProp = MessageManager.GetBooleanString(session.FistfulOfCards);
					wildWestShowLabel.LabelProp = MessageManager.GetBooleanString(session.WildWestShow);
					gamesPlayedLabel.LabelProp = session.GamesPlayed.ToString();

					ClearNodes();

					playerStore.Clear();
					spectatorStore.Clear();

					foreach(IPlayer player in session.Players)
						playerStore.AddNode(GetPlayerNode(player));
					foreach(ISpectator spectator in session.Spectators)
						spectatorStore.AddNode(GetSpectatorNode(spectator));
				}
				else
				{
					notebook.Sensitive = false;

					nameLabel.LabelProp = "";
					descriptionLabel.LabelProp = "";
					stateLabel.LabelProp = "";
					minPlayerCountLabel.LabelProp = "";
					maxPlayerCountLabel.LabelProp = "";
					maxSpectatorCountLabel.LabelProp = "";
					hasPlayerPasswordLabel.LabelProp = "";
					hasSpectatorPasswordLabel.LabelProp = "";
					dodgeCityLabel.LabelProp = "";
					highNoonLabel.LabelProp = "";
					aFistfulOfCardsLabel.LabelProp = "";
					wildWestShowLabel.LabelProp = "";
					gamesPlayedLabel.LabelProp = "";

					ClearNodes();

					playerStore.Clear();
					spectatorStore.Clear();
				}
			}
		}

		public event Action OnSelectedPlayerChanged;
		public event Action OnSelectedSpectatorChanged;

		public IPlayer SelectedPlayer
		{
			get
			{
				PlayerNode node = (PlayerNode)playersView.NodeSelection.SelectedNode;
				if(node == null)
					return null;
				return session.GetPlayer(node.ID);
			}
		}
		public ISpectator SelectedSpectator
		{
			get
			{
				SpectatorNode node = (SpectatorNode)spectatorsView.NodeSelection.SelectedNode;
				if(node == null)
					return null;
				return session.GetSpectator(node.ID);
			}
		}

		public SessionInfoWidget()
		{
			this.Build();

			playerStore = new Gtk.NodeStore(typeof(PlayerNode));
			spectatorStore = new Gtk.NodeStore(typeof(SpectatorNode));

			playerNodes = new Dictionary<int, PlayerNode>(8);
			spectatorNodes = new Dictionary<int, SpectatorNode>();

			playersView.NodeStore = playerStore;
			spectatorsView.NodeStore = spectatorStore;

			playersView.AppendColumn(Catalog.GetString("ID"), new Gtk.CellRendererSpin(), "text", 0);
			playersView.AppendColumn(Catalog.GetString("Name"), new Gtk.CellRendererText(), "text", 1);
			playersView.AppendColumn(Catalog.GetString("Image"), new Gtk.CellRendererPixbuf(), "pixbuf", 2);
			playersView.AppendColumn(Catalog.GetString("Has Pass."), new Gtk.CellRendererToggle(), "active", 3);
			playersView.AppendColumn(Catalog.GetString("Online"), new Gtk.CellRendererToggle(), "active", 4);
			playersView.AppendColumn(Catalog.GetString("AI"), new Gtk.CellRendererToggle(), "active", 5);
			playersView.AppendColumn(Catalog.GetString("Score"), new Gtk.CellRendererSpin(), "text", 6);

			spectatorsView.AppendColumn(Catalog.GetString("ID"), new Gtk.CellRendererSpin(), "text", 0);
			spectatorsView.AppendColumn(Catalog.GetString("Name"), new Gtk.CellRendererText(), "text", 1);
			spectatorsView.AppendColumn(Catalog.GetString("Image"), new Gtk.CellRendererPixbuf(), "pixbuf", 2);

			playersView.ShowExpanders = false;
			spectatorsView.ShowExpanders = false;

			playersView.NodeSelection.Changed += (sender, e) => {
				if(OnSelectedPlayerChanged != null)
					OnSelectedPlayerChanged();
			};
			spectatorsView.NodeSelection.Changed += (sender, e) => {
				if(OnSelectedSpectatorChanged != null)
					OnSelectedSpectatorChanged();
			};

			Session = null;
			listener = new EventListener(this);
			ConnectionManager.ServerEventListener.AddListener(listener);
		}

		private PlayerNode GetPlayerNode(IPlayer player)
		{
			int playerId = player.ID;
			try
			{
				return playerNodes[playerId];
			}
			catch(KeyNotFoundException)
			{
				return playerNodes[playerId] = new PlayerNode(player);
			}
		}
		private SpectatorNode GetSpectatorNode(ISpectator spectator)
		{
			int spectatorId = spectator.ID;
			try
			{
				return spectatorNodes[spectatorId];
			}
			catch(KeyNotFoundException)
			{
				return spectatorNodes[spectatorId] = new SpectatorNode(spectator);
			}
		}

		private void RemovePlayerNode(IPlayer player)
		{
			int playerId = player.ID;
			playerNodes.Remove(playerId);
		}
		private void RemoveSpectatorNode(ISpectator spectator)
		{
			int spectatorId = spectator.ID;
			spectatorNodes.Remove(spectatorId);
		}

		private void ClearNodes()
		{
			foreach(PlayerNode node in playerNodes.Values)
				node.Dispose();
			foreach(SpectatorNode node in spectatorNodes.Values)
				node.Dispose();

			playerNodes.Clear();
			spectatorNodes.Clear();
		}

		protected void OnDestroyEvent(object o, Gtk.DestroyEventArgs args)
		{
			ConnectionManager.ServerEventListener.RemoveListener(listener);
			ClearNodes();
			playerStore.Dispose();
			spectatorStore.Dispose();
		}
	}
}

