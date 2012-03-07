// SessionEventManager.cs
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

namespace BangSharp.Server
{
	public sealed class SessionEventManager
	{
		private Session session;
		
		public SessionEventManager(Session session)
		{
			this.session = session;
		}

		public void SendController(SessionPlayer player)
		{
			if(!player.HasListener)
				return;

			session.Locked = true;
			try
			{
				player.Listener.OnJoinedSession(player.Control);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine("INFO: Exception thrown by client:");
				Console.Error.WriteLine(e);
				session.RemovePlayer(player);
			}
			session.Locked = false;
		}
		public void SendController(SessionSpectator spectator)
		{
			if(!spectator.HasListener)
				return;

			session.Locked = true;
			try
			{
				spectator.Listener.OnJoinedSession(spectator.Control);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine("INFO: Exception thrown by client:");
				Console.Error.WriteLine(e);
				session.RemoveSpectator(spectator);
			}
			session.Locked = false;
		}

		public void SendGameController(SessionPlayer player, IPlayerControl control)
		{
			if(!player.HasListener)
				return;

			session.Locked = true;
			try
			{
				player.Listener.OnJoinedGame(control);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine("INFO: Exception thrown by client:");
				Console.Error.WriteLine(e);
				session.RemovePlayer(player);
			}
			session.Locked = false;
		}
		public void SendGameController(SessionSpectator spectator, ISpectatorControl control)
		{
			if(!spectator.HasListener)
				return;

			session.Locked = true;
			try
			{
				spectator.Listener.OnJoinedGame(control);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine("INFO: Exception thrown by client:");
				Console.Error.WriteLine(e);
				session.RemoveSpectator(spectator);
			}
			session.Locked = false;
		}
		public void OnGameStarted()
		{
			session.Server.EventManager.OnGameStarted(session);
		}

		public void OnSessionEnded()
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnSessionEnded();
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnSessionEnded();
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnGameEnded()
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnGameEnded();
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnGameEnded();
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Server.EventManager.OnGameEnded(session);
			session.Locked = false;
		}

		public void OnPlayerJoinedSession(SessionPlayer player)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerJoinedSession(player);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerJoinedSession(player);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Server.EventManager.OnPlayerJoinedSession(session, player);
			session.Locked = false;
		}
		public void OnSpectatorJoinedSession(SessionSpectator spectator)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnSpectatorJoinedSession(spectator);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnSpectatorJoinedSession(spectator);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Server.EventManager.OnSpectatorJoinedSession(session, spectator);
			session.Locked = false;
		}
		public void OnPlayerLeftSession(SessionPlayer player)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerLeftSession(player);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerLeftSession(player);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Server.EventManager.OnPlayerLeftSession(session, player);
			session.Locked = false;
		}
		public void OnSpectatorLeftSession(SessionSpectator spectator)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnSpectatorLeftSession(spectator);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnSpectatorLeftSession(spectator);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Server.EventManager.OnSpectatorLeftSession(session, spectator);
			session.Locked = false;
		}
		public void OnPlayerUpdated(SessionPlayer player)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerUpdated(player);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerUpdated(player);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Server.EventManager.OnPlayerUpdated(session, player);
			session.Locked = false;
		}

		public void SendChatMessage(SessionPlayer player, string message)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnChatMessage(player, message);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnChatMessage(player, message);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void SendChatMessage(SessionSpectator spectator, string message)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnChatMessage(spectator, message);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnChatMessage(spectator, message);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}

		public void OnNewRequest(RequestType requestType, IPublicPlayerView requestedPlayer, IPublicPlayerView causedBy)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener && p.ID == requestedPlayer.ID)
					try
					{
						p.Listener.OnNewRequest(requestType, causedBy);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}
			session.Locked = false;
		}

		public void OnPlayerDrewFromDeck(Player player, List<Card> drawnCards, bool revealCards)
		{
			session.Locked = true;
			ReadOnlyCollection<ICard> cards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => c));
			ReadOnlyCollection<ICard> emptyCards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => c.Empty));
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						if(p == player.Parent || revealCards)
							p.Listener.OnPlayerDrewFromDeck(player, cards);
						else
							p.Listener.OnPlayerDrewFromDeck(player, emptyCards);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						if(revealCards)
							s.Listener.OnPlayerDrewFromDeck(player, cards);
						else
							s.Listener.OnPlayerDrewFromDeck(player, emptyCards);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
		}
		public void OnPlayerDrewFromGraveyard(Player player, List<Card> drawnCards)
		{
			session.Locked = true;
			ReadOnlyCollection<ICard> cards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => c));
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerDrewFromGraveyard(player, cards);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerDrewFromGraveyard(player, cards);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerDiscardedCard(Player player, Card card)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerDiscardedCard(player, card);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerDiscardedCard(player, card);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerPlayedCard(Player player, Card card)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerPlayedCard(player, card);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerPlayedCard(player, card);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerPlayedCard(Player player, Card card, Player targetPlayer)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerPlayedCard(player, card, targetPlayer);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerPlayedCard(player, card, targetPlayer);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerPlayedCard(Player player, Card card, Player targetPlayer, Card targetCard)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						if(p == targetPlayer.Parent || targetCard.IsOnTable)
							p.Listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard);
						else
							p.Listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard.Empty);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						if(targetCard.IsOnTable)
							s.Listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard);
						else
							s.Listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard.Empty);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerPlayedCard(Player player, Card card, CardType asCard)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerPlayedCard(player, card, asCard);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerPlayedCard(player, card, asCard);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerPlayedCard(Player player, Card card, CardType asCard, Player targetPlayer)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerPlayedCard(Player player, Card card, CardType asCard, Player targetPlayer, Card targetCard)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						if(p == targetPlayer.Parent || targetCard.IsOnTable)
							p.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard);
						else
							p.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard.Empty);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						if(targetCard.IsOnTable)
							s.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard);
						else
							s.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard.Empty);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerPlayedCardOnTable(Player player, Card card)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerPlayedCardOnTable(player, card);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerPlayedCardOnTable(player, card);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPassedTableCard(Player player, Card card, Player targetPlayer)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPassedTableCard(player, card, targetPlayer);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPassedTableCard(player, card, targetPlayer);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerPassed(Player player)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerPassed(player);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerPassed(player);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerRespondedWithCard(Player player, Card card)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerRespondedWithCard(player, card);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerRespondedWithCard(player, card);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerRespondedWithCard(Player player, Card card, CardType asCard)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerRespondedWithCard(player, card, asCard);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerRespondedWithCard(player, card, asCard);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnDrawnIntoSelection(List<Card> drawnCards, Player selectionOwner)
		{
			session.Locked = true;
			ReadOnlyCollection<ICard> cards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => c));
			ReadOnlyCollection<ICard> emptyCards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => c.Empty));
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						if(selectionOwner == null)
							p.Listener.OnDrawnIntoSelection(cards);
						else if(p == selectionOwner.Parent)
							p.Listener.OnDrawnIntoSelection(cards);
						else
							p.Listener.OnDrawnIntoSelection(emptyCards);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						if(selectionOwner == null)
							s.Listener.OnDrawnIntoSelection(cards);
						else
							s.Listener.OnDrawnIntoSelection(emptyCards);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerPickedFromSelection(Player player, Card card, bool revealCard)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						if(p == player.Parent || revealCard)
							p.Listener.OnPlayerPickedFromSelection(player, card);
						else
							p.Listener.OnPlayerPickedFromSelection(player, card.Empty);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						if(revealCard)
							s.Listener.OnPlayerPickedFromSelection(player, card);
						else
							s.Listener.OnPlayerPickedFromSelection(player, card.Empty);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnUndrawnFromSelection(Card card, Player selectionOwner)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						if(selectionOwner == null)
							p.Listener.OnUndrawnFromSelection(card);
						else if(p == selectionOwner.Parent)
							p.Listener.OnUndrawnFromSelection(card);
						else
							p.Listener.OnUndrawnFromSelection(card.Empty);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						if(selectionOwner == null)
							s.Listener.OnUndrawnFromSelection(card);
						else
							s.Listener.OnUndrawnFromSelection(card.Empty);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerStoleCard(Player player, Player targetPlayer, Card targetCard, bool revealCard)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						if(p == player.Parent || p == targetPlayer.Parent || targetCard.IsOnTable || revealCard)
							p.Listener.OnPlayerStoleCard(player, targetPlayer, targetCard);
						else
							p.Listener.OnPlayerStoleCard(player, targetPlayer, targetCard.Empty);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						if(targetCard.IsOnTable || revealCard)
							s.Listener.OnPlayerStoleCard(player, targetPlayer, targetCard);
						else
							s.Listener.OnPlayerStoleCard(player, targetPlayer, targetCard.Empty);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerCancelledCard(Player player, Player targetPlayer, Card targetCard)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerCancelledCard(player, targetPlayer, targetCard);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerCancelledCard(player, targetPlayer, targetCard);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnDeckChecked(Card card)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnDeckChecked(card);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnDeckChecked(card);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnCardCancelled(Card card)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnCardCancelled(card);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnCardCancelled(card);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}

		public void OnPlayerCheckedDeck(Player player, Card checkedCard, Card causedBy, bool result)
		{
			session.Locked = true;
			CardType causedByType = causedBy == null ? CardType.Unknown : causedBy.Type;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerCheckedDeck(player, checkedCard, causedByType, result);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerCheckedDeck(player, checkedCard, causedByType, result);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnLifePointsChanged(Player player, int delta, Player causedBy)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnLifePointsChanged(player, delta, causedBy);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnLifePointsChanged(player, delta, causedBy);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerDied(Player player, Player causedBy)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerDied(player, causedBy);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerDied(player, causedBy);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerUsedAbility(Player player, CharacterType character)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerUsedAbility(player, character);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerUsedAbility(player, character);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerUsedAbility(Player player, CharacterType character, Player targetPlayer)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerUsedAbility(player, character, targetPlayer);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}
			
			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerUsedAbility(player, character, targetPlayer);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerGainedAdditionalCharacters(Player player)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerGainedAdditionalCharacters(player);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerGainedAdditionalCharacters(player);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnPlayerLostAdditionalCharacters(Player player)
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnPlayerLostAdditionalCharacters(player);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}
			
			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnPlayerLostAdditionalCharacters(player);
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
		public void OnDeckRegenerated()
		{
			session.Locked = true;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnDeckRegenerated();
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemovePlayer(p);
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnDeckRegenerated();
					}
					catch(Exception e)
					{
						Console.Error.WriteLine("INFO: Exception thrown by client:");
						Console.Error.WriteLine(e);
						session.RemoveSpectator(s);
					}
			session.Locked = false;
		}
	}
}
