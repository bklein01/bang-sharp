// ProxySessionEventListener.cs
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

namespace BangSharp.Client
{
	public class ProxySessionEventListener : ImmortalMarshalByRefObject, ISpectatorSessionEventListener, IPlayerSessionEventListener
	{
		private object syncLock = new object();
		private bool isAI;
		private List<ISessionEventListener> mainListeners;
		private List<IPlayerSessionEventListener> playerListeners;
		private List<ISpectatorSessionEventListener> spectatorListeners;

		public object SyncLock
		{
			get { return syncLock; }
		}

		public ProxySessionEventListener(bool isAI = false)
		{
			this.isAI = isAI;
			mainListeners = new List<ISessionEventListener>();
			playerListeners = new List<IPlayerSessionEventListener>();
			spectatorListeners = new List<ISpectatorSessionEventListener>();
		}

		public void AddListener(ISessionEventListener listener)
		{
			lock(syncLock)
			{
				if(!mainListeners.Contains(listener))
					mainListeners.Add(listener);
			}
		}
		public void AddListener(IPlayerSessionEventListener listener)
		{
			lock(syncLock)
			{
				if(!mainListeners.Contains(listener))
					mainListeners.Add(listener);
				if(!playerListeners.Contains(listener))
					playerListeners.Add(listener);
			}
		}

		public void AddListener(ISpectatorSessionEventListener listener)
		{
			lock(syncLock)
			{
				if(!mainListeners.Contains(listener))
					mainListeners.Add(listener);
				if(!spectatorListeners.Contains(listener))
					spectatorListeners.Add(listener);
			}
		}
		public void RemoveListener(ISessionEventListener listener)
		{
			lock(syncLock)
			{
				mainListeners.Remove(listener);
				playerListeners.RemoveAll(l => l == listener);
				spectatorListeners.RemoveAll(l => l == listener);
			}
		}

		#region IPlayerSessionEventListener implementation
		void IPlayerSessionEventListener.OnJoinedSession(IPlayerSessionControl control)
		{
			lock(syncLock)
			{
				foreach(IPlayerSessionEventListener listener in playerListeners)
						listener.OnJoinedSession(control);
			}
		}

		void IPlayerSessionEventListener.OnJoinedGame(IPlayerControl control)
		{
			lock(syncLock)
			{
				foreach(IPlayerSessionEventListener listener in playerListeners)
					listener.OnJoinedGame(control);
			}
		}

		void IPlayerSessionEventListener.OnNewRequest(RequestType requestType, IPublicPlayerView causedBy)
		{
			lock(syncLock)
			{
				foreach(IPlayerSessionEventListener listener in playerListeners)
					listener.OnNewRequest(requestType, causedBy);
			}
		}

		bool IPlayerSessionEventListener.IsAI
		{
			get { return isAI; }
		}
		#endregion

		#region ISpectatorSessionEventListener implementation
		void ISpectatorSessionEventListener.OnJoinedSession(ISpectatorSessionControl control)
		{
			lock(syncLock)
			{
				foreach(ISpectatorSessionEventListener listener in spectatorListeners)
					listener.OnJoinedSession(control);
			}
		}

		void ISpectatorSessionEventListener.OnJoinedGame(ISpectatorControl control)
		{
			lock(syncLock)
			{
				foreach(ISpectatorSessionEventListener listener in spectatorListeners)
					listener.OnJoinedGame(control);
			}
		}
		#endregion

		#region ISessionEventListener implementation
		void ISessionEventListener.Ping()
		{
		}

		void ISessionEventListener.OnSessionEnded()
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnSessionEnded();
			}
		}

		void ISessionEventListener.OnGameEnded()
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnGameEnded();
			}
		}

		void ISessionEventListener.OnPlayerJoinedSession(IPlayer player)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerJoinedSession(player);
			}
		}

		void ISessionEventListener.OnSpectatorJoinedSession(ISpectator spectator)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnSpectatorJoinedSession(spectator);
			}
		}

		void ISessionEventListener.OnPlayerLeftSession(IPlayer player)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerLeftSession(player);
			}
		}

		void ISessionEventListener.OnSpectatorLeftSession(ISpectator spectator)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnSpectatorLeftSession(spectator);
			}
		}

		void ISessionEventListener.OnPlayerUpdated(IPlayer player)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerUpdated(player);
			}
		}

		void ISessionEventListener.OnPlayerDisconnected(IPlayer player)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerDisconnected(player);
			}
		}

		void ISessionEventListener.OnChatMessage(IPlayer player, string message)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnChatMessage(player, message);
			}
		}

		void ISessionEventListener.OnChatMessage(ISpectator spectator, string message)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnChatMessage(spectator, message);
			}
		}

		void ISessionEventListener.OnPlayerDrewFromDeck(IPublicPlayerView player, System.Collections.ObjectModel.ReadOnlyCollection<ICard> drawnCards)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerDrewFromDeck(player, drawnCards);
			}
		}

		void ISessionEventListener.OnPlayerDrewFromGraveyard(IPublicPlayerView player, System.Collections.ObjectModel.ReadOnlyCollection<ICard> drawnCards)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerDrewFromGraveyard(player, drawnCards);
			}
		}

		void ISessionEventListener.OnPlayerDiscardedCard(IPublicPlayerView player, ICard card)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerDiscardedCard(player, card);
			}
		}

		void ISessionEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerPlayedCard(player, card);
			}
		}

		void ISessionEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerPlayedCard(player, card, targetPlayer);
			}
		}

		void ISessionEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard);
			}
		}

		void ISessionEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerPlayedCard(player, card, asCard);
			}
		}

		void ISessionEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer);
			}
		}

		void ISessionEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard);
			}
		}

		void ISessionEventListener.OnPlayerPlayedCardOnTable(IPublicPlayerView player, ICard card)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerPlayedCardOnTable(player, card);
			}
		}

		void ISessionEventListener.OnPassedTableCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPassedTableCard(player, card, targetPlayer);
			}
		}

		void ISessionEventListener.OnPlayerPassed(IPublicPlayerView player)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerPassed(player);
			}
		}

		void ISessionEventListener.OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerRespondedWithCard(player, card);
			}
		}

		void ISessionEventListener.OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card, CardType asCard)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerRespondedWithCard(player, card, asCard);
			}
		}

		void ISessionEventListener.OnDrawnIntoSelection(System.Collections.ObjectModel.ReadOnlyCollection<ICard> drawnCards)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnDrawnIntoSelection(drawnCards);
			}
		}

		void ISessionEventListener.OnPlayerPickedFromSelection(IPublicPlayerView player, ICard card)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerPickedFromSelection(player, card);
			}
		}

		void ISessionEventListener.OnUndrawnFromSelection(ICard card)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnUndrawnFromSelection(card);
			}
		}

		void ISessionEventListener.OnPlayerStoleCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerStoleCard(player, targetPlayer, targetCard);
			}
		}

		void ISessionEventListener.OnPlayerCancelledCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerCancelledCard(player, targetPlayer, targetCard);
			}
		}

		void ISessionEventListener.OnDeckChecked(ICard card)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnDeckChecked(card);
			}
		}

		void ISessionEventListener.OnCardCancelled(ICard card)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnCardCancelled(card);
			}
		}

		void ISessionEventListener.OnPlayerCheckedDeck(IPublicPlayerView player, ICard checkedCard, CardType causedBy, bool result)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerCheckedDeck(player, checkedCard, causedBy, result);
			}
		}

		void ISessionEventListener.OnLifePointsChanged(IPublicPlayerView player, int delta, IPublicPlayerView causedBy)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnLifePointsChanged(player, delta, causedBy);
			}
		}

		void ISessionEventListener.OnPlayerDied(IPublicPlayerView player, IPublicPlayerView causedBy)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerDied(player, causedBy);
			}
		}

		void ISessionEventListener.OnPlayerUsedAbility(IPublicPlayerView player, CharacterType character)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerUsedAbility(player, character);
			}
		}

		void ISessionEventListener.OnPlayerUsedAbility(IPublicPlayerView player, CharacterType character, IPublicPlayerView targetPlayer)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerUsedAbility(player, character, targetPlayer);
			}
		}

		void ISessionEventListener.OnPlayerGainedAdditionalCharacters(IPublicPlayerView player)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerGainedAdditionalCharacters(player);
			}
		}

		void ISessionEventListener.OnPlayerLostAdditionalCharacters(IPublicPlayerView player)
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnPlayerLostAdditionalCharacters(player);
			}
		}

		void ISessionEventListener.OnDeckRegenerated()
		{
			lock(syncLock)
			{
				foreach(ISessionEventListener listener in mainListeners)
					listener.OnDeckRegenerated();
			}
		}
		#endregion
	}
}
