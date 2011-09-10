// Game.cs
//  
// Author:  WOnder93 <omosnacek@gmail.com>
// 
// Copyright (c) 2011 Ondrej Mosnáček
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
using System.Linq;
namespace Bang.Server
{
	public sealed class Game : MarshalByRefObject, IGame
	{
		private sealed class SpectatorControl : ImmortalMarshalByRefObject, ISpectatorControl
		{
			private Game game;
			
			IGame ISpectatorControl.Game
			{
				get { return new GameProxy(game); }
			}

			public SpectatorControl (Game game)
			{
				this.game = game;
			}
		}
		private Session session;
		private Dictionary<int, Player> players;
		private List<Player> playerList;
		private GameCycle cycle;
		private GameTable table;
		private bool ended;
		private SpectatorControl spectatorControl;

		public Session Session
		{
			get { return session; }
		}

		public GameCycle GameCycle
		{
			get { return cycle; }
		}
		public GameTable GameTable
		{
			get { return table; }
		}
				
		public ReadOnlyCollection<Player> Players
		{
			get { return new ReadOnlyCollection<Player>(playerList); }
		}
		ReadOnlyCollection<IPublicPlayerView> IGame.Players
		{
			get { return new ReadOnlyCollection<IPublicPlayerView> (playerList.ConvertAll<IPublicPlayerView>(p => new PublicPlayerViewProxy(p))); }
		}
		public int AlivePlayersCount
		{
			get { return playerList.Count(p => p.IsAlive); }
		}
		
		ReadOnlyCollection<ICard> IGame.Selection
		{
			get { return table.GetSelection(null); }
		}
		ICard IGame.GraveyardTop
		{
			get { return table.GraveyardTop; }
		}
		RequestType IGame.RequestType
		{
			get { return ended ? RequestType.None : cycle.RequestType; }
		}
		IPublicPlayerView IGame.CurrentPlayer
		{
			get { return new PublicPlayerViewProxy(cycle.CurrentPlayer); }
		}
		IPublicPlayerView IGame.RequestedPlayer
		{
			get { return ended ? null : new PublicPlayerViewProxy(cycle.RequestedPlayer); }
		}
		IPublicPlayerView IGame.CausedBy
		{
			get { return ended ? null : new PublicPlayerViewProxy(cycle.CausedBy); }
		}
		
		public int MaxBangs
		{
			get { return 1; }
		}

		public bool Ended
		{
			get { return ended; }
		}
		
		public Game(Session session, int sheriffId)
		{
			this.session = session;
			table = new GameTable(this);
			cycle = new GameCycle(this);
			ended = false;
			
			int playerCount = session.Players.Count;
			players = new Dictionary<int, Player>(playerCount);
			playerList = new List<Player>(playerCount);
			IEnumerator<Role> roleEnumerator = GenerateRoles(playerCount).GetEnumerator();
			foreach(SessionPlayer p in session.Players)
			{
				Role role;
				if(p.ID == sheriffId)
					role = playerCount == 3 ? Role.Deputy : Role.Sheriff;
				else
				{
					roleEnumerator.MoveNext();
					role = roleEnumerator.Current;
				}
				Player player = new Player(this, p, role, session.NextCharacter());
				players.Add(p.ID, player);
				playerList.Add(player);
			}
			foreach(SessionPlayer p in session.Players)
			{
				Player player = players[p.ID];
				session.EventManager.SendGameController(p, player.Control);
			}
			
			spectatorControl = new SpectatorControl (this);
			foreach (SessionSpectator s in session.Spectators)
				session.EventManager.SendGameController (s, spectatorControl);
		}
		private IEnumerable<Role> GenerateRoles (int playerCount)
		{
			List<Role> roles;
			switch (playerCount)
			{
			case 2:
				roles = new List<Role>() { Role.Outlaw };
				break;
			case 3:
				roles = new List<Role>() { Role.Outlaw, Role.Renegade };
				break;
			case 4:
				roles = new List<Role>() { Role.Outlaw, Role.Outlaw, Role.Renegade };
				break;
			case 5:
				roles = new List<Role>() { Role.Deputy, Role.Outlaw, Role.Outlaw, Role.Renegade };
				break;
			case 6:
				roles = new List<Role>() { Role.Deputy, Role.Outlaw, Role.Outlaw, Role.Outlaw, Role.Renegade };
				break;
			case 7:
				roles = new List<Role>() { Role.Deputy, Role.Deputy, Role.Outlaw, Role.Outlaw, Role.Outlaw, Role.Renegade };
				break;
			case 8:
				roles = new List<Role>() { Role.Deputy, Role.Deputy, Role.Outlaw, Role.Outlaw, Role.Outlaw, Role.Renegade, Role.Renegade };
				break;
			default:
				throw new TooManyPlayersException();
			}
			roles.Shuffle();
			return roles;
		}

		public void Start ()
		{
			GameCycle.StartCycle ();
			session.EventManager.OnNewRequest(cycle.RequestType, cycle.RequestedPlayer, cycle.CausedBy);
		}
		
		public Player NextPlayer (Player player)
		{
			int i = playerList.IndexOf (player);
			do
			{
				if (++i == playerList.Count)
					i = 0;
			}
			while (!playerList[i].IsAlive);
			return playerList[i];
		}
		public int GetDistance (Player player, Player targetPlayer)
		{
			int playerCount = playerList.Count;
			int start = playerList.IndexOf (player);
			int end = playerList.IndexOf (targetPlayer);
			if(start == end)
				return 0;
			
			int dist = 0;
			int upIndex = start;
			int downIndex = start;

			while (upIndex != end && downIndex != end)
			{
				do
				{
					upIndex++;
					if (upIndex == playerCount)
						upIndex = 0;
				}
				while (!playerList[upIndex].IsAlive);
				do
				{
					if (downIndex == 0)
						downIndex = playerCount;
					downIndex--;
				}
				while (!playerList[downIndex].IsAlive);
				dist++;
			}
			return dist - player.GetDistanceOut(targetPlayer) + targetPlayer.GetDistanceIn(player);
		}

		private sealed class TakeDeadPlayersCardResponseHandler : ResponseHandler
		{
			public TakeDeadPlayersCardResponseHandler(Player dead, Player taker)
				: base(RequestType.TakeDeadPlayersCard, taker, dead)
			{
			}

			protected override void OnRespondCard(Card card)
			{
				if(card.Owner != CausedBy)
					throw new BadCardException();

				Game.GameTable.PlayerStealCard(RequestedPlayer, card);
				End();
			}
		}
		private void BuryPlayer(Player player)
		{
			foreach(Player p in playerList)
				if(p.IsAlive)
					p.Character.OnPlayerDied(player);

			List<Player> takers = new List<Player>();
			Player firstAfter = NextPlayer(player);
			Player temp = firstAfter;
			do
			{
				if(temp.Character.TakesDeadPlayersCards)
					takers.Add(temp);
				temp = NextPlayer(temp);
			}
			while(temp != firstAfter);

			if(takers.Count <= 1)
			{
				List<Card> handCopy = new List<Card>(player.Hand);
				List<TableCard> tableCopy = new List<TableCard>(player.Table);
				if(takers.Count == 0)
				{
					foreach(Card c in handCopy)
						table.CancelCard(c);

					foreach(TableCard c in tableCopy)
						table.CancelCard(c);
				}
				else
				{
					foreach(Card c in handCopy)
						table.PlayerStealCard(takers[0], c);

					foreach(Card c in tableCopy)
						table.PlayerStealCard(takers[0], c);
				}
			}
			else
			{
				IEnumerator<Player> takerEnumerator = takers.GetEnumerator();
				int cardCount = player.Hand.Count + player.Table.Count;
				List<ResponseHandler> handlers = new List<ResponseHandler>(cardCount);
				for(int i = 0; i < cardCount; i++)
				{
					if(!takerEnumerator.MoveNext())
					{
						takerEnumerator.Reset();
						takerEnumerator.MoveNext();
					}
					handlers.Add(new TakeDeadPlayersCardResponseHandler(player, takerEnumerator.Current));
				}
				cycle.PushTempHandler(new QueueResponseHandler(handlers));
			}
		}
		public void PlayerDied(Player player, Player causedBy)
		{
			player.IsAlive = false;
			session.EventManager.OnPlayerDied(player, causedBy);
			Role victoriousRole = Role.Unknown;
			if(playerList.Count == 3)
			{
				Role causedByRole = causedBy == null ? Role.Unknown : causedBy.Role;
				if(AlivePlayersCount == 1)
					victoriousRole = playerList.First(p => p.IsAlive).Role;
				else
					switch(player.Role)
					{
					case Role.Deputy:
						if(causedByRole == Role.Outlaw)
							victoriousRole = Role.Outlaw;
						break;
					case Role.Outlaw:
						if(causedByRole == Role.Renegade)
							victoriousRole = Role.Renegade;
						break;
					case Role.Renegade:
						if(causedByRole == Role.Deputy)
							victoriousRole = Role.Deputy;
						break;
					}
				if(victoriousRole == Role.Unknown && causedBy != null && causedBy != player)
					table.PlayerDrawFromDeck(causedBy, 3);
			}
			else
			{
				int good = 0;
				int outlaws = 0;
				int renegades = 0;
				foreach(Player p in playerList)
					if(p.IsAlive)
						switch(p.Role)
						{
						case Role.Sheriff:
						case Role.Deputy:
							good++;
							break;
						case Role.Outlaw:
							outlaws++;
							break;
						case Role.Renegade:
							renegades++;
							break;
						}
				if(player.IsSheriff)
					if(good == 0 && outlaws == 0 && renegades == 1)
						victoriousRole = Role.Renegade;
					else
						victoriousRole = Role.Outlaw;
				else if(outlaws == 0 && renegades == 0)
					victoriousRole = Role.Sheriff;
				else if(player.Role == Role.Outlaw && causedBy != null && causedBy != player)
					table.PlayerDrawFromDeck(causedBy, 3);
			}
			if(victoriousRole != Role.Unknown)
			{
				int playerCount = playerList.Count;
				int outlawCount = playerList.Count(p => p.Role == Role.Outlaw);
				switch(victoriousRole)
				{
				case Role.Sheriff:
				case Role.Deputy:
					bool onlySheriffAlive = playerList.All(p => !p.IsAlive || p.IsSheriff);
					foreach(Player p in playerList)
						switch(p.Role)
						{
						case Role.Sheriff:
							p.IsWinner = true;
							p.Parent.RegisterVictory(Role.Sheriff);
							if(playerList.Count >= 4)
								p.Parent.UpdateScore(1500 * outlawCount);
							break;
						case Role.Deputy:
							p.IsWinner = true;
							p.Parent.RegisterVictory(Role.Deputy);
							if(playerList.Count >= 4)
								p.Parent.UpdateScore((p.IsAlive ? 1000 : 700) * outlawCount);
							break;
						case Role.Renegade:
							if(playerList.Count >= 4 && onlySheriffAlive)
								p.Parent.UpdateScore(400 * playerCount);
							break;
						}
					break;
				case Role.Outlaw:
					foreach(Player p in playerList)
						switch(p.Role)
						{
						case Role.Deputy:
							if(playerList.Count >= 4 && p == causedBy)
								p.Parent.UpdateScore(-5000);
							break;
						case Role.Outlaw:
							p.IsWinner = true;
							p.Parent.RegisterVictory(Role.Outlaw);
							if(playerList.Count >= 4)
								p.Parent.UpdateScore((p.IsAlive ? 1000 : 800) * outlawCount);
							break;
						case Role.Renegade:
							if(playerList.Count >= 4 && p.IsAlive)
								p.Parent.UpdateScore(300 * playerCount);
							break;
						}
					break;
				case Role.Renegade:
					foreach(Player p in playerList)
						switch(p.Role)
						{
						case Role.Sheriff:
							if(playerList.Count >= 4)
								p.Parent.UpdateScore(100 * playerCount);
							break;
						case Role.Renegade:
							p.IsWinner = true;
							p.Parent.RegisterVictory(Role.Renegade);
							if(playerList.Count >= 4)
								p.Parent.UpdateScore(1500 * playerCount);
							break;
						}
					break;
				}
				foreach(Player p in playerList)
					p.Parent.UpdateTurnsPlayed(p.TurnsPlayed);
				ended = true;
				session.OnGameEnded();
			}
			else
			{
				BuryPlayer(player);
				if(player.Role == Role.Deputy && causedBy != null && causedBy.Role == Role.Sheriff)
				{
					List<Card> handCopy = new List<Card>(causedBy.Hand);
					foreach(Card c in handCopy)
						table.CancelCard(c);
					List<TableCard> tableCopy = new List<TableCard>(causedBy.Table);
					foreach(Card c in tableCopy)
						table.CancelCard(c);
				}
			}
		}
		
		public void RegisterPlayer(SessionPlayer player)
		{
			Player p = GetPlayer(player.ID);
			session.EventManager.SendGameController(player, p.Control);
			if(p == cycle.RequestedPlayer)
				session.EventManager.OnNewRequest(cycle.RequestType, p, cycle.CausedBy);
		}
		public void RegisterSpectator(SessionSpectator spectator)
		{
			session.EventManager.SendGameController(spectator, spectatorControl);
		}

		public void Dispose()
		{
			foreach(Player p in playerList)
				p.Control.Disconnect();

			spectatorControl.Disconnect();
		}
		
		public Player GetPlayer(int id)
		{
			try
			{
				return players[id];
			}
			catch(KeyNotFoundException)
			{
				throw new InvalidIdException();
			}
		}
		IPublicPlayerView IGame.GetPublicPlayerView(int id)
		{
			try
			{
				return new PublicPlayerViewProxy(players[id]);
			}
			catch(KeyNotFoundException)
			{
				throw new InvalidIdException();
			}
		}
	}
}

