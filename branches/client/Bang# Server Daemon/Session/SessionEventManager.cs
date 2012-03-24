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
using System.Runtime.Remoting;
using System.Threading;

namespace BangSharp.Server.Daemon
{
	public sealed class SessionEventManager
	{
		private abstract class EventSender<Subject>
		{
			public delegate void Event(Subject s);

			private SessionEventManager eventMgr;

			public SessionEventManager EventMgr
			{
				get { return eventMgr; }
			}

			protected EventSender(SessionEventManager eventMgr)
			{
				this.eventMgr = eventMgr;
			}

			public void SendEvent(Event ev, Subject s)
			{
				try
				{
					ev(s);
				}
				catch(RemotingTimeoutException)
				{
					OnTimeout(s);
				}
				catch(Exception e)
				{
					OnError(s, e);
				}
			}

			protected abstract void OnError(Subject s, Exception e);
			protected abstract void OnTimeout(Subject s);
		}
		private sealed class PlayerEventSender : EventSender<SessionPlayer>
		{
			public PlayerEventSender(SessionEventManager eventMgr)
				: base(eventMgr)
			{
			}
			protected override void OnError(SessionPlayer s, Exception e)
			{
				Console.Error.WriteLine("INFO: Exception thrown by client:");
				Console.Error.WriteLine(e);
				EventMgr.session.RemovePlayer(s);
			}
			protected override void OnTimeout(SessionPlayer s)
			{
				Console.Error.WriteLine("INFO: Client event timed out!");
				EventMgr.session.RemovePlayer(s);
			}
		}
		private sealed class SpectatorEventSender : EventSender<SessionSpectator>
		{
			public SpectatorEventSender(SessionEventManager eventMgr)
				: base(eventMgr)
			{
			}
			protected override void OnError(SessionSpectator s, Exception e)
			{
				Console.Error.WriteLine("INFO: Exception thrown by client:");
				Console.Error.WriteLine(e);
				EventMgr.session.RemoveSpectator(s);
			}
			protected override void OnTimeout(SessionSpectator s)
			{
				Console.Error.WriteLine("INFO: Client event timed out!");
				EventMgr.session.RemoveSpectator(s);
			}
		}
		private Session session;
		private PlayerEventSender playerSender;
		private SpectatorEventSender spectatorSender;
		private Thread pingThread;

		public SessionEventManager(Session session)
		{
			this.session = session;
			playerSender = new PlayerEventSender(this);
			spectatorSender = new SpectatorEventSender(this);
			pingThread = new Thread(ProcessPings);
			pingThread.IsBackground = true;
		}

		private void ProcessPings()
		{
			while(true)
			{
				int interval = Config.Instance.GetInteger("Server.PollInterval", 15000);
				lock(session.Lock)
				{
					if(session.State == SessionState.Ended)
						break;

					List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
					foreach(SessionPlayer p in players)
						SendPing(p);

					List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
					foreach(SessionSpectator s in spectators)
						SendPing(s);
				}
				Thread.Sleep(interval);
			}
		}

		public void StartPolling()
		{
			pingThread.Start();
		}

		public void SendPing(SessionPlayer player)
		{
			playerSender.SendEvent(pl => {
				if(pl.HasListener)
					pl.Listener.Ping();
			}, player);
		}
		public void SendPing(SessionSpectator spectator)
		{
			spectatorSender.SendEvent(sp => {
				if(sp.HasListener)
					sp.Listener.Ping();
			}, spectator);
		}

		public void SendController(SessionPlayer player)
		{
			playerSender.SendEvent(pl => {
				if(pl.HasListener)
					pl.Listener.OnJoinedSession(player.Control);
			}, player);
		}
		public void SendController(SessionSpectator spectator)
		{
			spectatorSender.SendEvent(sp => {
				if(sp.HasListener)
					sp.Listener.OnJoinedSession(spectator.Control);
			}, spectator);
		}

		public void SendGameController(SessionPlayer player, IPlayerControl control)
		{
			playerSender.SendEvent(pl => {
				if(pl.HasListener)
					pl.Listener.OnJoinedGame(control);
			}, player);
		}
		public void SendGameController(SessionSpectator spectator, ISpectatorControl control)
		{
			spectatorSender.SendEvent(sp => {
				if(sp.HasListener)
					sp.Listener.OnJoinedGame(control);
			}, spectator);
		}
		public void OnGameStarted()
		{
			session.Server.EventManager.OnGameStarted(session);
		}

		public void OnSessionEnded()
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnSessionEnded();
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnSessionEnded();
				}, s);

			pingThread.Abort();
		}
		public void OnGameEnded()
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnGameEnded();
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnGameEnded();
				}, s);
			session.Server.EventManager.OnGameEnded(session);
		}

		public void OnPlayerJoinedSession(SessionPlayer player)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerJoinedSession(player);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerJoinedSession(player);
				}, s);
			session.Server.EventManager.OnPlayerJoinedSession(session, player);
		}
		public void OnSpectatorJoinedSession(SessionSpectator spectator)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnSpectatorJoinedSession(spectator);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnSpectatorJoinedSession(spectator);
				}, s);
			session.Server.EventManager.OnSpectatorJoinedSession(session, spectator);
		}
		public void OnPlayerLeftSession(SessionPlayer player)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerLeftSession(player);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerLeftSession(player);
				}, s);
			session.Server.EventManager.OnPlayerLeftSession(session, player);
		}
		public void OnSpectatorLeftSession(SessionSpectator spectator)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnSpectatorLeftSession(spectator);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnSpectatorLeftSession(spectator);
				}, s);
			session.Server.EventManager.OnSpectatorLeftSession(session, spectator);
		}
		public void OnPlayerUpdated(SessionPlayer player)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerUpdated(player);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerUpdated(player);
				}, s);
			session.Server.EventManager.OnPlayerUpdated(session, player);
		}
		public void OnPlayerDisconnected(SessionPlayer player)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerDisconnected(player);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerDisconnected(player);
				}, s);
			session.Server.EventManager.OnPlayerDisconnected(session, player);
		}

		public void SendChatMessage(SessionPlayer player, string message)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnChatMessage(player, message);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnChatMessage(player, message);
				}, s);
		}
		public void SendChatMessage(SessionSpectator spectator, string message)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnChatMessage(spectator, message);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnChatMessage(spectator, message);
				}, s);
		}

		public void OnNewRequest(RequestType requestType, IPublicPlayerView requestedPlayer, IPublicPlayerView causedBy)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener && pl.ID == requestedPlayer.ID)
						pl.Listener.OnNewRequest(requestType, causedBy);
				}, p);
		}

		public void OnPlayerDrewFromDeck(Player player, List<Card> drawnCards, bool revealCards)
		{
			ReadOnlyCollection<ICard> cards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => c));
			ReadOnlyCollection<ICard> emptyCards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => c.Empty));
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						if(pl.ID == player.ID || revealCards)
							pl.Listener.OnPlayerDrewFromDeck(player, cards);
						else
							pl.Listener.OnPlayerDrewFromDeck(player, emptyCards);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						if(revealCards)
							sp.Listener.OnPlayerDrewFromDeck(player, cards);
						else
							sp.Listener.OnPlayerDrewFromDeck(player, emptyCards);
				}, s);
		}
		public void OnPlayerDrewFromGraveyard(Player player, List<Card> drawnCards)
		{
			ReadOnlyCollection<ICard> cards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => c));
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerDrewFromGraveyard(player, cards);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerDrewFromGraveyard(player, cards);
				}, s);
		}
		public void OnPlayerDiscardedCard(Player player, Card card)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerDiscardedCard(player, card);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerDiscardedCard(player, card);
				}, s);
		}
		public void OnPlayerPlayedCard(Player player, Card card)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerPlayedCard(player, card);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerPlayedCard(player, card);
				}, s);
		}
		public void OnPlayerPlayedCard(Player player, Card card, Player targetPlayer)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerPlayedCard(player, card, targetPlayer);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerPlayedCard(player, card, targetPlayer);
				}, s);
		}
		public void OnPlayerPlayedCard(Player player, Card card, Player targetPlayer, Card targetCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						if(pl.ID == targetPlayer.ID || targetCard.IsOnTable)
							pl.Listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard);
						else
							pl.Listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard.Empty);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						if(targetCard.IsOnTable)
							sp.Listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard);
						else
							sp.Listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard.Empty);
				}, s);
		}
		public void OnPlayerPlayedCard(Player player, Card card, CardType asCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerPlayedCard(player, card, asCard);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerPlayedCard(player, card, asCard);
				}, s);
		}
		public void OnPlayerPlayedCard(Player player, Card card, CardType asCard, Player targetPlayer)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer);
				}, s);
		}
		public void OnPlayerPlayedCard(Player player, Card card, CardType asCard, Player targetPlayer, Card targetCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						if(pl.ID == targetPlayer.ID || targetCard.IsOnTable)
							pl.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard);
						else
							pl.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard.Empty);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						if(targetCard.IsOnTable)
							sp.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard);
						else
							sp.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard.Empty);
				}, s);
		}
		public void OnPlayerPlayedCardOnTable(Player player, Card card)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerPlayedCardOnTable(player, card);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerPlayedCardOnTable(player, card);
				}, s);
		}
		public void OnPassedTableCard(Player player, Card card, Player targetPlayer)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPassedTableCard(player, card, targetPlayer);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPassedTableCard(player, card, targetPlayer);
				}, s);
		}
		public void OnPlayerPassed(Player player)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerPassed(player);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerPassed(player);
				}, s);
		}
		public void OnPlayerRespondedWithCard(Player player, Card card)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerRespondedWithCard(player, card);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerRespondedWithCard(player, card);
				}, s);
		}
		public void OnPlayerRespondedWithCard(Player player, Card card, CardType asCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerRespondedWithCard(player, card, asCard);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerRespondedWithCard(player, card, asCard);
				}, s);
		}
		public void OnDrawnIntoSelection(List<Card> drawnCards, Player selectionOwner)
		{
			ReadOnlyCollection<ICard> cards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => c));
			ReadOnlyCollection<ICard> emptyCards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => c.Empty));
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						if(selectionOwner == null)
							pl.Listener.OnDrawnIntoSelection(cards);
						else if(pl.ID == selectionOwner.ID)
							pl.Listener.OnDrawnIntoSelection(cards);
						else
							pl.Listener.OnDrawnIntoSelection(emptyCards);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						if(selectionOwner == null)
							sp.Listener.OnDrawnIntoSelection(cards);
						else
							sp.Listener.OnDrawnIntoSelection(emptyCards);
				}, s);
		}
		public void OnPlayerPickedFromSelection(Player player, Card card, bool revealCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						if(pl.ID == player.ID || revealCard)
							pl.Listener.OnPlayerPickedFromSelection(player, card);
						else
							pl.Listener.OnPlayerPickedFromSelection(player, card.Empty);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						if(revealCard)
							sp.Listener.OnPlayerPickedFromSelection(player, card);
						else
							sp.Listener.OnPlayerPickedFromSelection(player, card.Empty);
				}, s);
		}
		public void OnUndrawnFromSelection(Card card, Player selectionOwner)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						if(selectionOwner == null)
							pl.Listener.OnUndrawnFromSelection(card);
						else if(pl.ID == selectionOwner.ID)
							pl.Listener.OnUndrawnFromSelection(card);
						else
							pl.Listener.OnUndrawnFromSelection(card.Empty);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						if(selectionOwner == null)
							sp.Listener.OnUndrawnFromSelection(card);
						else
							sp.Listener.OnUndrawnFromSelection(card.Empty);
				}, s);
		}
		public void OnPlayerStoleCard(Player player, Player targetPlayer, Card targetCard, bool revealCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						if(pl.ID == player.ID || pl.ID == targetPlayer.ID || targetCard.IsOnTable || revealCard)
							pl.Listener.OnPlayerStoleCard(player, targetPlayer, targetCard);
						else
							pl.Listener.OnPlayerStoleCard(player, targetPlayer, targetCard.Empty);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						if(targetCard.IsOnTable || revealCard)
							sp.Listener.OnPlayerStoleCard(player, targetPlayer, targetCard);
						else
							sp.Listener.OnPlayerStoleCard(player, targetPlayer, targetCard.Empty);
				}, s);
		}
		public void OnPlayerCancelledCard(Player player, Player targetPlayer, Card targetCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerCancelledCard(player, targetPlayer, targetCard);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerCancelledCard(player, targetPlayer, targetCard);
				}, s);
		}
		public void OnDeckChecked(Card card)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnDeckChecked(card);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnDeckChecked(card);
				}, s);
		}
		public void OnCardCancelled(Card card)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnCardCancelled(card);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnCardCancelled(card);
				}, s);
		}

		public void OnPlayerCheckedDeck(Player player, Card checkedCard, Card causedBy, bool result)
		{
			CardType causedByType = causedBy == null ? CardType.Unknown : causedBy.Type;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerCheckedDeck(player, checkedCard, causedByType, result);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerCheckedDeck(player, checkedCard, causedByType, result);
				}, s);
		}
		public void OnLifePointsChanged(Player player, int delta, Player causedBy)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnLifePointsChanged(player, delta, causedBy);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnLifePointsChanged(player, delta, causedBy);
				}, s);
		}
		public void OnPlayerDied(Player player, Player causedBy)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerDied(player, causedBy);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerDied(player, causedBy);
				}, s);
		}
		public void OnPlayerUsedAbility(Player player, CharacterType character)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerUsedAbility(player, character);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerUsedAbility(player, character);
				}, s);
		}
		public void OnPlayerUsedAbility(Player player, CharacterType character, Player targetPlayer)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerUsedAbility(player, character, targetPlayer);
				}, p);
			
			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerUsedAbility(player, character, targetPlayer);
				}, s);
		}
		public void OnPlayerGainedAdditionalCharacters(Player player)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerGainedAdditionalCharacters(player);
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerGainedAdditionalCharacters(player);
				}, s);
		}
		public void OnPlayerLostAdditionalCharacters(Player player)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnPlayerLostAdditionalCharacters(player);
				}, p);
			
			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnPlayerLostAdditionalCharacters(player);
				}, s);
		}
		public void OnDeckRegenerated()
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				playerSender.SendEvent(pl => {
					if(pl.HasListener)
						pl.Listener.OnDeckRegenerated();
				}, p);

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				spectatorSender.SendEvent(sp => {
					if(sp.HasListener)
						sp.Listener.OnDeckRegenerated();
				}, s);
		}
	}
}
