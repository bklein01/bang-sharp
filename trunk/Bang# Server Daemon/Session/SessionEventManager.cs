using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Remoting;
namespace Bang.Server
{
	public sealed class SessionEventManager
	{
		private Session session;
		
		public SessionEventManager (Session session)
		{
			this.session = session;
		}
		
		public void SendController(SessionPlayer player)
		{
			if(!player.HasListener)
				return;
			try
			{
				player.Listener.OnJoinedSession(player.Control);
			}
			catch(RemotingException)
			{
				player.UnregisterListener();
			}
		}
		public void SendController(SessionSpectator spectator)
		{
			if(!spectator.HasListener)
				return;
			try
			{
				spectator.Listener.OnJoinedSession (spectator.Control);
			}
			catch(RemotingException)
			{
				spectator.UnregisterListener();
			}
		}
		
		public void SendGameController (SessionPlayer player, IPlayerControl control)
		{
			if(!player.HasListener)
				return;
			try
			{
				player.Listener.OnJoinedGame(control);
			}
			catch(RemotingException)
			{
				player.UnregisterListener();
			}
		}
		public void SendGameController(SessionSpectator spectator, ISpectatorControl control)
		{
			if(!spectator.HasListener)
				return;
			try
			{
				spectator.Listener.OnJoinedGame(control);
			}
			catch(RemotingException)
			{
				spectator.UnregisterListener();
			}
		}
		
		public void OnSessionEnded()
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnSessionEnded();
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnSessionEnded();
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnGameEnded ()
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnGameEnded();
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnGameEnded();
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		
		public void OnPlayerJoinedSession (SessionPlayer player)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerJoinedSession(player);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerJoinedSession(player);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnSpectatorJoinedSession (SessionSpectator spectator)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnSpectatorJoinedSession(spectator);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnSpectatorJoinedSession(spectator);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerLeftSession (SessionPlayer player)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerLeftSession(player);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerLeftSession(player);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnSpectatorLeftSession (SessionSpectator spectator)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnSpectatorLeftSession(spectator);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnSpectatorLeftSession(spectator);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerUpdated (SessionPlayer player)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerUpdated(player);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerUpdated(player);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}

		public void SendChatMessage (SessionPlayer player, string message)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnChatMessage(player, message);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnChatMessage(player, message);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void SendChatMessage (SessionSpectator spectator, string message)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnChatMessage(spectator, message);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnChatMessage(spectator, message);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}

		public void OnNewRequest(RequestType requestType, IPublicPlayerView requestedPlayer, IPublicPlayerView causedBy)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnNewRequest(requestType, requestedPlayer, causedBy);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach(SessionSpectator s in spectators)
				if(s.HasListener)
					try
					{
						s.Listener.OnNewRequest(requestType, requestedPlayer, causedBy);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		
		public void OnPlayerDrewFromDeck(Player player, List<Card> drawnCards, bool revealCards)
		{
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
					catch(RemotingException)
					{
						p.UnregisterListener();
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
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerDrewFromGraveyard (Player player, List<Card> drawnCards)
		{
			ReadOnlyCollection<ICard> cards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => c));
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerDrewFromGraveyard(player, cards);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerDrewFromGraveyard(player, cards);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerDiscardedCard (Player player, Card card)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerDiscardedCard(player, card);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerDiscardedCard(player, card);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerPlayedCard (Player player, Card card)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerPlayedCard(player, card);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerPlayedCard(player, card);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerPlayedCard (Player player, Card card, Player targetPlayer)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerPlayedCard(player, card, targetPlayer);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerPlayedCard(player, card, targetPlayer);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerPlayedCard(Player player, Card card, Player targetPlayer, Card targetCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						if(p == targetPlayer.Parent)
							p.Listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard);
						else
							p.Listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard.Empty);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard.Empty);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerPlayedCard (Player player, Card card, CardType asCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerPlayedCard(player, card, asCard);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerPlayedCard(player, card, asCard);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerPlayedCard (Player player, Card card, CardType asCard, Player targetPlayer)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerPlayedCard(Player player, Card card, CardType asCard, Player targetPlayer, Card targetCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						if(p == targetPlayer.Parent)
							p.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard);
						else
							p.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard.Empty);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard.Empty);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerPlayedCardOnTable (Player player, Card card)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerPlayedCardOnTable(player, card);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerPlayedCardOnTable(player, card);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPassedTableCard (Player player, Card card, Player targetPlayer)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPassedTableCard(player, card, targetPlayer);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPassedTableCard(player, card, targetPlayer);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerPassed (Player player)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerPassed(player);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerPassed(player);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerRespondedWithCard (Player player, Card card)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerRespondedWithCard(player, card);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerRespondedWithCard(player, card);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerRespondedWithCard (Player player, Card card, CardType asCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerRespondedWithCard(player, card, asCard);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerRespondedWithCard(player, card, asCard);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnDrawnIntoSelection(List<Card> drawnCards, Player selectionOwner)
		{
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
					catch(RemotingException)
					{
						p.UnregisterListener();
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
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerPickedFromSelection(Player player, Card card, bool revealCard)
		{
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
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						if(revealCard)
							s.Listener.OnPlayerPickedFromSelection(player, card);
						else
							s.Listener.OnPlayerPickedFromSelection(player, card.Empty);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnUndrawnFromSelection(Card card, Player selectionOwner)
		{
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
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						if(selectionOwner == null)
							s.Listener.OnUndrawnFromSelection(card);
						else
							s.Listener.OnUndrawnFromSelection(card.Empty);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerStoleCard(Player player, Player targetPlayer, Card targetCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						if(p == player.Parent || p == targetPlayer.Parent)
							p.Listener.OnPlayerStoleCard(player, targetPlayer, targetCard);
						else
							p.Listener.OnPlayerStoleCard(player, targetPlayer, targetCard.Empty);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerStoleCard(player, targetPlayer, targetCard.Empty);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerCancelledCard (Player player, Player targetPlayer, Card targetCard)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerCancelledCard(player, targetPlayer, targetCard);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerCancelledCard(player, targetPlayer, targetCard);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnDeckChecked (Card card)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnDeckChecked(card);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnDeckChecked(card);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnCardCancelled (Card card)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnCardCancelled(card);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnCardCancelled(card);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}

		public void OnPlayerCheckedDeck(Player player, Card checkedCard, Card causedBy, bool result)
		{
			CardType causedByType = causedBy == null ? CardType.Unknown : causedBy.Type;
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerCheckedDeck(player, checkedCard, causedByType, result);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerCheckedDeck(player, checkedCard, causedByType, result);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnLifePointsChanged (Player player, int delta, Player causedBy)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnLifePointsChanged(player, delta, causedBy);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnLifePointsChanged(player, delta, causedBy);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerDied (Player player, Player causedBy)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerDied(player, causedBy);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerDied(player, causedBy);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerUsedAbility (Player player)
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerUsedAbility(player);
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnPlayerUsedAbility(player);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnDeckRegenerated()
		{
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						p.Listener.OnDeckRegenerated();
					}
					catch(RemotingException)
					{
						p.UnregisterListener();
					}

			List<SessionSpectator> spectators = new List<SessionSpectator>(session.Spectators);
			foreach (SessionSpectator s in spectators)
				if (s.HasListener)
					try
					{
						s.Listener.OnDeckRegenerated();
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
	}
}
