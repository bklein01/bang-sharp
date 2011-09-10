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
				player.Listener.OnJoinedSession(new PlayerSessionControlProxy(player.Control));
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
				spectator.Listener.OnJoinedSession(new SpectatorSessionControlProxy(spectator.Control));
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
				player.Listener.OnJoinedGame(new PlayerControlProxy(control));
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
				spectator.Listener.OnJoinedGame(new SpectatorControlProxy(control));
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
						p.Listener.OnPlayerJoinedSession(new PlayerProxy(player));
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
						s.Listener.OnPlayerJoinedSession(new PlayerProxy(player));
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
						p.Listener.OnSpectatorJoinedSession(new SpectatorProxy(spectator));
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
						s.Listener.OnSpectatorJoinedSession(new SpectatorProxy(spectator));
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
						p.Listener.OnPlayerLeftSession(new PlayerProxy(player));
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
						s.Listener.OnPlayerLeftSession(new PlayerProxy(player));
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
						p.Listener.OnSpectatorLeftSession(new SpectatorProxy(spectator));
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
						s.Listener.OnSpectatorLeftSession(new SpectatorProxy(spectator));
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
						p.Listener.OnPlayerUpdated(new PlayerProxy(player));
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
						s.Listener.OnPlayerUpdated(new PlayerProxy(player));
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
						p.Listener.OnChatMessage(new PlayerProxy(player), message);
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
						s.Listener.OnChatMessage(new PlayerProxy(player), message);
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
						p.Listener.OnChatMessage(new SpectatorProxy(spectator), message);
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
						s.Listener.OnChatMessage(new SpectatorProxy(spectator), message);
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
						p.Listener.OnNewRequest(requestType, new PublicPlayerViewProxy(requestedPlayer), causedBy == null ? null : new PublicPlayerViewProxy(causedBy));
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
						s.Listener.OnNewRequest(requestType, new PublicPlayerViewProxy(requestedPlayer), causedBy == null ? null : new PublicPlayerViewProxy(causedBy));
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		
		public void OnPlayerDrewFromDeck(Player player, List<Card> drawnCards, bool revealCards)
		{
			ReadOnlyCollection<ICard> cards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => new CardProxy(c)));
			ReadOnlyCollection<ICard> emptyCards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => c.Empty));
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach(SessionPlayer p in players)
				if(p.HasListener)
					try
					{
						if(p == player.Parent || revealCards)
							p.Listener.OnPlayerDrewFromDeck(new PublicPlayerViewProxy(player), cards);
						else
							p.Listener.OnPlayerDrewFromDeck(new PublicPlayerViewProxy(player), emptyCards);
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
							s.Listener.OnPlayerDrewFromDeck(new PublicPlayerViewProxy(player), cards);
						else
							s.Listener.OnPlayerDrewFromDeck(new PublicPlayerViewProxy(player), emptyCards);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnPlayerDrewFromGraveyard (Player player, List<Card> drawnCards)
		{
			ReadOnlyCollection<ICard> cards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => new CardProxy(c)));
			List<SessionPlayer> players = new List<SessionPlayer>(session.Players);
			foreach (SessionPlayer p in players)
				if (p.HasListener)
					try
					{
						p.Listener.OnPlayerDrewFromGraveyard(new PublicPlayerViewProxy(player), cards);
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
						s.Listener.OnPlayerDrewFromGraveyard(new PublicPlayerViewProxy(player), cards);
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
						p.Listener.OnPlayerDiscardedCard(new PublicPlayerViewProxy(player), card);
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
						s.Listener.OnPlayerDiscardedCard(new PublicPlayerViewProxy(player), card);
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
						p.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card));
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
						s.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card));
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
						p.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card), new PublicPlayerViewProxy(targetPlayer));
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
						s.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card), new PublicPlayerViewProxy(targetPlayer));
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
							p.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card), new PublicPlayerViewProxy(targetPlayer), new CardProxy(targetCard));
						else
							p.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card), new PublicPlayerViewProxy(targetPlayer), targetCard.Empty);
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
						s.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card), new PublicPlayerViewProxy(targetPlayer), targetCard.Empty);
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
						p.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card), asCard);
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
						s.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card), asCard);
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
						p.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card), asCard, new PublicPlayerViewProxy(targetPlayer));
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
						s.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card), asCard, new PublicPlayerViewProxy(targetPlayer));
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
							p.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card), asCard, new PublicPlayerViewProxy(targetPlayer), new CardProxy(targetCard));
						else
							p.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card), asCard, new PublicPlayerViewProxy(targetPlayer), targetCard.Empty);
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
						s.Listener.OnPlayerPlayedCard(new PublicPlayerViewProxy(player), new CardProxy(card), asCard, new PublicPlayerViewProxy(targetPlayer), targetCard.Empty);
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
						p.Listener.OnPlayerPlayedCardOnTable(new PublicPlayerViewProxy(player), new CardProxy(card));
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
						s.Listener.OnPlayerPlayedCardOnTable(new PublicPlayerViewProxy(player), new CardProxy(card));
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
						p.Listener.OnPassedTableCard(new PublicPlayerViewProxy(player), new CardProxy(card), new PublicPlayerViewProxy(targetPlayer));
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
						s.Listener.OnPassedTableCard(new PublicPlayerViewProxy(player), new CardProxy(card), new PublicPlayerViewProxy(targetPlayer));
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
						p.Listener.OnPlayerPassed(new PublicPlayerViewProxy(player));
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
						s.Listener.OnPlayerPassed(new PublicPlayerViewProxy(player));
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
						p.Listener.OnPlayerRespondedWithCard(new PublicPlayerViewProxy(player), new CardProxy(card));
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
						s.Listener.OnPlayerRespondedWithCard(new PublicPlayerViewProxy(player), new CardProxy(card));
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
						p.Listener.OnPlayerRespondedWithCard(new PublicPlayerViewProxy(player), new CardProxy(card), asCard);
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
						s.Listener.OnPlayerRespondedWithCard(new PublicPlayerViewProxy(player), new CardProxy(card), asCard);
					}
					catch(RemotingException)
					{
						s.UnregisterListener();
					}
		}
		public void OnDrawnIntoSelection(List<Card> drawnCards, Player selectionOwner)
		{
			ReadOnlyCollection<ICard> cards = new ReadOnlyCollection<ICard>(drawnCards.ConvertAll<ICard>(c => new CardProxy(c)));
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
							p.Listener.OnPlayerPickedFromSelection(new PublicPlayerViewProxy(player), new CardProxy(card));
						else
							p.Listener.OnPlayerPickedFromSelection(new PublicPlayerViewProxy(player), card.Empty);
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
							s.Listener.OnPlayerPickedFromSelection(new PublicPlayerViewProxy(player), new CardProxy(card));
						else
							s.Listener.OnPlayerPickedFromSelection(new PublicPlayerViewProxy(player), card.Empty);
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
							p.Listener.OnUndrawnFromSelection(new CardProxy(card));
						else if(p == selectionOwner.Parent)
							p.Listener.OnUndrawnFromSelection(new CardProxy(card));
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
							s.Listener.OnUndrawnFromSelection(new CardProxy(card));
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
							p.Listener.OnPlayerStoleCard(new PublicPlayerViewProxy(player), new PublicPlayerViewProxy(targetPlayer), new CardProxy(targetCard));
						else
							p.Listener.OnPlayerStoleCard(new PublicPlayerViewProxy(player), new PublicPlayerViewProxy(targetPlayer), targetCard.Empty);
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
						s.Listener.OnPlayerStoleCard(new PublicPlayerViewProxy(player), new PublicPlayerViewProxy(targetPlayer), targetCard.Empty);
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
						p.Listener.OnPlayerCancelledCard(new PublicPlayerViewProxy(player), new PublicPlayerViewProxy(targetPlayer), new CardProxy(targetCard));
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
						s.Listener.OnPlayerCancelledCard(new PublicPlayerViewProxy(player), new PublicPlayerViewProxy(targetPlayer), new CardProxy(targetCard));
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
						p.Listener.OnDeckChecked(new CardProxy(card));
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
						s.Listener.OnDeckChecked(new CardProxy(card));
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
						p.Listener.OnCardCancelled(new CardProxy(card));
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
						s.Listener.OnCardCancelled(new CardProxy(card));
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
						p.Listener.OnPlayerCheckedDeck(new PublicPlayerViewProxy(player), new CardProxy(checkedCard), causedByType, result);
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
						s.Listener.OnPlayerCheckedDeck(new PublicPlayerViewProxy(player), new CardProxy(checkedCard), causedByType, result);
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
						p.Listener.OnLifePointsChanged(new PublicPlayerViewProxy(player), delta, causedBy == null ? null : new PublicPlayerViewProxy(causedBy));
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
						s.Listener.OnLifePointsChanged(new PublicPlayerViewProxy(player), delta, causedBy == null ? null : new PublicPlayerViewProxy(causedBy));
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
						p.Listener.OnPlayerDied(new PublicPlayerViewProxy(player), causedBy == null ? null : new PublicPlayerViewProxy(causedBy));
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
						s.Listener.OnPlayerDied(new PublicPlayerViewProxy(player), causedBy == null ? null : new PublicPlayerViewProxy(causedBy));
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
						p.Listener.OnPlayerUsedAbility(new PublicPlayerViewProxy(player));
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
						s.Listener.OnPlayerUsedAbility(new PublicPlayerViewProxy(player));
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
