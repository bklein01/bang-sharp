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
			}

			public override void OnJoinedSession(ISpectatorSessionControl control)
			{
				CardManager.Init(control.Session);
			}

			public override void OnJoinedGame(IPlayerControl control)
			{
				parent.Update();
			}

			public override void OnJoinedGame(ISpectatorControl control)
			{
				parent.Update();
			}

			public override void OnPlayerUpdated(IPlayer player)
			{
				if(ConnectionManager.Game != null)
					parent.playerMap[player.ID].Update(player);
			}

			public override void OnPlayerDisconnected(IPlayer player)
			{
				if(ConnectionManager.Game != null)
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

		public RootWidget(GameBoardWidget parent) : base()
		{
			this.parent = parent;
			listener = new EventListener(this);
			ConnectionManager.SessionEventListener.AddListener((IPlayerSessionEventListener)listener);
			ConnectionManager.SessionEventListener.AddListener((ISpectatorSessionEventListener)listener);
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
		}
		public void Update()
		{
			int thisPlayerIndex = 0;
			IGame game = ConnectionManager.Game;
			ReadOnlyCollection<IPublicPlayerView> players = game.Players;
			if(ConnectionManager.PlayerGameControl != null)
			{
				IPrivatePlayerView thisPlayer = ConnectionManager.PlayerGameControl.PrivatePlayerView;
				for(int i = 0; i < players.Count; i++)
				{
					IPublicPlayerView player = players[i];
					if(player.ID == thisPlayer.ID)
					{
						thisPlayerIndex = i;
						break;
					}
				}
			}
			ISession session = ConnectionManager.Session;
			for(int i = 0; i < players.Count; i++)
			{
				int index = (thisPlayerIndex + i) % players.Count;
				int playerId = players[index].ID;
				playerSlots[i].Update(session.GetPlayer(playerId));
				playerMap.Add(playerId, playerSlots[i]);
			}
			mainTable.Update();
			SetRequestType(RequestType.None);
		}

		public void SetRequestType(RequestType type)
		{
			lock(ConnectionManager.SessionEventLock)
			{
				if(type == RequestType.None)
					requestLabel.Markup = "<span font='sans serif' color='orange'><b>No Request</b></span>";
				else
					requestLabel.Markup = "<span font='sans serif' color='orange'><b>" + type.ToString() + "</b></span>";
			}
			RequestRedraw();
		}

#if DEBUG
		private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
#endif
		public void RootExpose(Context cr, Rectangle area)
		{
			lock(ConnectionManager.SessionEventLock)
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
			lock(ConnectionManager.SessionEventLock)
			{
				//sw.Reset();
				//sw.Start();
				Reallocate(newAlloc);
				//sw.Stop();
				//Console.Error.WriteLine("DEBUG: Reallocating took: {0}", sw.Elapsed);
				//PrintAlloc(this);
			}
		}

		public void RootLeftClick(double x, double y)
		{
			//lock(ConnectionManager.SessionEventLock)
			LeftClick(x, y);
		}

		public void RootRightClick(double x, double y)
		{
			//lock(ConnectionManager.SessionEventLock)
			RightClick(x, y);
		}

		public override void RequestResize()
		{
			//parent.QueueResize();
			parent.QueueResizeNoRedraw();
			parent.QueueDraw();
		}

		public override void RequestRedraw()
		{
			parent.QueueDraw();
		}
	}
}

