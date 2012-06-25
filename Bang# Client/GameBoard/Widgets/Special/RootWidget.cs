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
using Cairo;

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

			public override void OnJoinedSession(IPlayerSessionControl control)
			{
				CardManager.Init(control.Session);
				parent.Update();
			}
			public override void OnJoinedSession(ISpectatorSessionControl control)
			{
				CardManager.Init(control.Session);
				parent.Update();
			}

			public override void OnPlayerJoinedSession(IPlayer player)
			{
				parent.Update();
			}
			public override void OnPlayerLeftSession(IPlayer player)
			{
				parent.Update();
			}

			public override void OnPlayerUpdated(IPlayer player)
			{
				if(ConnectionManager.Session != null)
					parent.playerMap[player.ID].Update(player);
			}
			public override void OnPlayerDisconnected(IPlayer player)
			{
				if(ConnectionManager.Session != null)
					parent.playerMap[player.ID].Update(player);
			}

			public override void OnSessionEnded()
			{
				parent.Clear();
				CardManager.Flush();
			}

			public override void OnNewRequest(RequestType requestType, IPublicPlayerView causedBy)
			{
				parent.SetRequestType(requestType);
			}
			public override void OnNewRequest(IPublicPlayerView requestedPlayer, IPublicPlayerView causedBy)
			{
				parent.SetRequestType(RequestType.None);
			}
		}
		private GameBoardWidget parent;
		private object layoutLock = new object();
		private EventListener listener;
		private PlayerSlotWidget[] playerSlots;
		private Dictionary<int, PlayerSlotWidget> playerMap;

		public override object LayoutLock
		{
			get { return layoutLock; }
		}

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

		public RootWidget(GameBoardWidget parent) : base()
		{
			this.parent = parent;
			listener = new EventListener(this);
			ConnectionManager.SessionEventListener.AddListener((IPlayerSessionEventListener)listener);
			ConnectionManager.SessionEventListener.AddListener((ISpectatorSessionEventListener)listener);
			ConnectionManager.OnSessionDisconnected += () => {
				Clear();
				CardManager.Flush();
			};
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
			RequestRedraw();
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
			RequestRedraw();
		}

		public void SetRequestType(RequestType type)
		{
			lock(layoutLock)
			{
				requestLabel.Markup = "<span color='lightblue'>Request type: </span>";
				if(type == RequestType.None)
					requestLabel.Markup += "<span color='orange'><b>No Request</b></span>";
				else
					requestLabel.Markup += "<span color='orange'><b>" + type.ToString() + "</b></span>";
			}
		}
		public void SetResponseType(string responseType, GameException exception = null)
		{
			lock(layoutLock)
			{
				responseLabel.Markup = "<span color='lightblue'>Last response: </span>";
				responseLabel.Markup += "<span color='orange'><b>" + responseType + " - </b></span>";
				if(exception == null)
					responseLabel.Markup += "<span color='lightgreen'>Accepted!</span>";
				else
					responseLabel.Markup += "<span color='lightsalmon'>" + exception.GetType().ToString() + "</span>";
			}
		}

#if DEBUG
		private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
#endif
		public void RootExpose(Context cr, Rectangle area)
		{
			lock(layoutLock)
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
		}

		private void PrintAlloc(Widget w, string padding = "")
		{
			Console.WriteLine(padding + "{0} - {1}", w, w.Allocation);
			foreach(Widget ch in w.Children)
				PrintAlloc(ch, padding + '\t');
		}

		public void RootReallocate(Rectangle newAlloc)
		{
			lock(layoutLock)
				Reallocate(newAlloc);
		}

		public void RootLeftClick(double x, double y)
		{
			//lock(layoutLock)
			LeftClick(x, y);
		}

		public void RootRightClick(double x, double y)
		{
			//lock(layoutLock)
			RightClick(x, y);
		}

		public override void RequestResize()
		{
			parent.QueueResize();
		}

		public override void RequestRedraw()
		{
			parent.QueueDraw();
		}
	}
}

