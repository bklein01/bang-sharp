// RootWidget.cs
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
using System.Collections.ObjectModel;
using System.Threading;
using Cairo;
using Mono.Unix;

namespace BangSharp.Client.GameBoard.Widgets
{
	/// <summary>
	/// The root widget of the Game Board widget system.
	/// </summary>
	public partial class RootWidget : Bin
	{
		private class EventListener : VirtualSessionEventListener
		{
			private RootWidget parent;

			public EventListener(RootWidget parent)
			{
				this.parent = parent;
			}

			public override void OnPlayerJoinedSession(IPlayer player)
			{
				Gdk.Threads.Enter();
				parent.Update();
				parent.RequestRedraw();
				Gdk.Threads.Leave();
			}
			public override void OnPlayerLeftSession(IPlayer player)
			{
				Gdk.Threads.Enter();
				parent.Update();
				parent.RequestRedraw();
				Gdk.Threads.Leave();
			}

			public override void OnPlayerUpdated(IPlayer player)
			{
				if(ConnectionManager.Session != null)
				{
					Gdk.Threads.Enter();
					parent.playerMap[player.ID].Update(player);
					parent.RequestRedraw();
					Gdk.Threads.Leave();
				}
			}
			public override void OnPlayerDisconnected(IPlayer player)
			{
				OnPlayerUpdated(player);
			}

			public override void OnSessionEnded()
			{
				Gdk.Threads.Enter();
				parent.Clear();
				CardManager.Flush();
				parent.RequestRedraw();
				Gdk.Threads.Leave();
			}

			public override void OnNewRequest(RequestType requestType, IPublicPlayerView causedBy)
			{
				Gdk.Threads.Enter();
				parent.SetRequestType(requestType);
				parent.RequestRedraw();
				Gdk.Threads.Leave();
			}
			public override void OnNewRequest(IPublicPlayerView requestedPlayer, IPublicPlayerView causedBy)
			{
				Gdk.Threads.Enter();
				parent.SetRequestType(RequestType.None);
				parent.RequestRedraw();
				Gdk.Threads.Leave();
			}
		}
		private GameBoardWidget parent;
		private EventListener listener;
		private PlayerSlotWidget[] playerSlots;
		private Dictionary<int, PlayerSlotWidget> playerMap;

		public CardPlaceholderWidget DeckPlaceholder
		{
			get { return mainTable.DeckPlaceholder; }
		}
		public CardPlaceholderWidget GraveyardPlaceholder
		{
			get { return mainTable.GraveyardPlaceholder; }
		}
		public GeneralPlaceholderWidget SelectionPlaceholder
		{
			get { return mainTable.SelectionPlaceholder; }
		}

		public RootWidget(GameBoardWidget parent)
		{
			this.parent = parent;
			listener = new EventListener(this);
			InitLayout();
			playerMap = new Dictionary<int, PlayerSlotWidget>(8);
			playerSlots = new PlayerSlotWidget[]
			{
				playerSlot0,
				playerSlot1,
				playerSlot2,
				playerSlot3,
				playerSlot4,
				playerSlot5,
				playerSlot6,
				playerSlot7
			};
			ConnectionManager.SessionEventListener.AddListener((IPlayerSessionEventListener)listener);
			ConnectionManager.SessionEventListener.AddListener((ISpectatorSessionEventListener)listener);
			ConnectionManager.OnSessionConnected += () => {
				Gdk.Threads.Enter();
				CardManager.Init(ConnectionManager.Session);
				Update();
				RequestRedraw();
				Gdk.Threads.Leave();
			};
			ConnectionManager.OnSessionDisconnected += () => {
				Gdk.Threads.Enter();
				Clear();
				CardManager.Flush();
				RequestRedraw();
				Gdk.Threads.Leave();
			};
			ConnectionManager.OnGameConnected += () => {
				Gdk.Threads.Enter();
				Update();
				RequestRedraw();
				Gdk.Threads.Leave();
			};
		}

		public GeneralPlaceholderWidget GetPlayerHandPlaceholder(int playerId)
		{
			return playerMap[playerId].HandPlaceholder;
		}
		public GeneralPlaceholderWidget GetPlayerTablePlaceholder(int playerId)
		{
			return playerMap[playerId].TablePlaceholder;
		}
		public CardPlaceholderWidget GetPlayerRolePlaceholder(int playerId)
		{
			return playerMap[playerId].RolePlaceholder;
		}
		public CardPlaceholderWidget GetPlayerCharacterPlaceholder(int playerId)
		{
			return playerMap[playerId].CharacterPlaceholder;
		}

		public void Clear()
		{
			foreach(PlayerSlotWidget playerSlot in playerMap.Values)
				playerSlot.Clear();
			mainTable.Clear();
			playerMap.Clear();
			requestLabel.Markup = "";
			responseLabel.Markup = "";
		}
		public void Update()
		{
			Clear();
			int thisPlayerIndex = 0;
			ISession session = ConnectionManager.Session;
			ReadOnlyCollection<IPlayer> players = session.Players;
			if(ConnectionManager.PlayerSessionControl != null)
			{
				IPlayer thisPlayer = ConnectionManager.PlayerSessionControl.Player;
				for(int i = 0; i < players.Count; i++)
				{
					IPlayer player = players[i];
					if(player.ID == thisPlayer.ID)
					{
						thisPlayerIndex = i;
						break;
					}
				}
			}
			for(int i = 0; i < players.Count; i++)
			{
				int index = (thisPlayerIndex + i) % players.Count;
				playerSlots[i].Update(players[index]);
				playerMap.Add(players[index].ID, playerSlots[i]);
			}
			mainTable.Update();
			SetRequestType(RequestType.None);
		}

		public void SetRequestType(RequestType type)
		{
			requestLabel.Markup = "<span color='lightblue'>" + Catalog.GetString("Request type:") + " </span>";
			if(type == RequestType.None)
				requestLabel.Markup += "<span color='orange'><b>" + Catalog.GetString("No Request") + "</b></span>";
			else
				requestLabel.Markup += "<span color='orange'><b>" + type.ToString() + "</b></span>";
		}
		public void SetResponseType(string responseType, GameException exception = null)
		{
			responseLabel.Markup = "<span color='lightblue'>" + Catalog.GetString("Last response:") + " </span>";
			responseLabel.Markup += "<span color='orange'><b>" + responseType + " - </b></span>";
			if(exception == null)
				responseLabel.Markup += "<span color='lightgreen'>" + Catalog.GetString("Accepted!") + "</span>";
			else
				responseLabel.Markup += "<span color='lightsalmon'>" + MessageManager.GetErrorMessage(exception) + "</span>";
		}

#if DEBUG
		private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
#endif
		public void RootExpose(Context cr, Rectangle area)
		{
#if DEBUG
			sw.Reset();
			sw.Start();
#endif
			Expose(cr, area);
#if DEBUG
			sw.Stop();
			Console.Error.WriteLine("DEBUG: Rendering took: {0}", sw.Elapsed);
#endif
			if(cr.Status != Status.Success)
				Console.Error.WriteLine("WARNING: Cairo status is {0}", cr.Status);
		}

		private void PrintAlloc(Widget w, string padding = "")
		{
			Console.WriteLine(padding + "{0} - {1}", w, w.Allocation);
			foreach(Widget ch in w.Children)
				PrintAlloc(ch, padding + '\t');
		}

		public void RootReallocate(Rectangle newAlloc)
		{
			Reallocate(newAlloc);
		}

		public void RootLeftClick(double x, double y)
		{
			LeftClick(x, y);
		}

		public void RootRightClick(double x, double y)
		{
			RightClick(x, y);
		}

		public override void RequestResize()
		{
			Gtk.Application.Invoke(delegate {
				Gdk.Threads.Enter();
				RootReallocate(Allocation);
				Gdk.Threads.Leave();
			});
		}
		public override void RequestRedraw()
		{
			RequestResize();
			parent.QueueDraw();
		}
	}
}

