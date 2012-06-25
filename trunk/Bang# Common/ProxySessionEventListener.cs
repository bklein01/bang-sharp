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

namespace BangSharp
{
	/// <summary>
	/// Proxy session event listener.
	/// </summary>
	/// <remarks>
	/// This class can be used to connect multiple session event listeners through one listener.
	/// </remarks>
	public class ProxySessionEventListener : ImmortalMarshalByRefObject, ISpectatorSessionEventListener, IPlayerSessionEventListener
	{
		private List<ISessionEventListener> mainListeners;
		private List<IPlayerSessionEventListener> playerListeners;
		private List<ISpectatorSessionEventListener> spectatorListeners;

		/// <summary>
		/// Gets or sets a value to return as the <see cref="BangSharp.IPlayerSessionEventListener.IsAI"/> property.
		/// </summary>
		/// <value>
		/// The value to use.
		/// </value>
		public bool IsAI
		{
			get;
			set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BangSharp.ProxySessionEventListener"/> class.
		/// </summary>
		public ProxySessionEventListener()
		{
			mainListeners = new List<ISessionEventListener>();
			playerListeners = new List<IPlayerSessionEventListener>();
			spectatorListeners = new List<ISpectatorSessionEventListener>();
		}

		/// <summary>
		/// Adds the listener to the list of member listeners.
		/// </summary>
		/// <param name="listener">
		/// The listener to add.
		/// </param>
		public void AddListener(ISessionEventListener listener)
		{
			if(!mainListeners.Contains(listener))
				mainListeners.Add(listener);
		}

		/// <summary>
		/// Adds the listener to the list of member listeners.
		/// </summary>
		/// <param name="listener">
		/// The listener to add.
		/// </param>
		public void AddListener(IPlayerSessionEventListener listener)
		{
			if(!mainListeners.Contains(listener))
				mainListeners.Add(listener);
			if(!playerListeners.Contains(listener))
				playerListeners.Add(listener);
		}

		/// <summary>
		/// Adds the listener to the list of member listeners.
		/// </summary>
		/// <param name="listener">
		/// The listener to add.
		/// </param>
		public void AddListener(ISpectatorSessionEventListener listener)
		{
			if(!mainListeners.Contains(listener))
				mainListeners.Add(listener);
			if(!spectatorListeners.Contains(listener))
				spectatorListeners.Add(listener);
		}

		/// <summary>
		/// Removes the listener from the list of member listeners.
		/// </summary>
		/// <param name="listener">
		/// The listener to remove.
		/// </param>
		public void RemoveListener(ISessionEventListener listener)
		{
			mainListeners.Remove(listener);
			playerListeners.RemoveAll(l => l == listener);
			spectatorListeners.RemoveAll(l => l == listener);
		}

		#region IPlayerSessionEventListener implementation
		public void OnJoinedSession(IPlayerSessionControl control)
		{
			foreach(IPlayerSessionEventListener listener  in playerListeners.ToArray())
				listener.OnJoinedSession(control);
		}

		public void OnJoinedGame(IPlayerControl control)
		{
			foreach(IPlayerSessionEventListener listener  in playerListeners.ToArray())
				listener.OnJoinedGame(control);
		}

		public void OnNewRequest(RequestType requestType, IPublicPlayerView causedBy)
		{
			foreach(IPlayerSessionEventListener listener  in playerListeners.ToArray())
				listener.OnNewRequest(requestType, causedBy);
		}
		#endregion

		#region ISpectatorSessionEventListener implementation
		public void OnJoinedSession(ISpectatorSessionControl control)
		{
			foreach(ISpectatorSessionEventListener listener  in spectatorListeners.ToArray())
				listener.OnJoinedSession(control);
		}

		public void OnJoinedGame(ISpectatorControl control)
		{
			foreach(ISpectatorSessionEventListener listener  in spectatorListeners.ToArray())
				listener.OnJoinedGame(control);
		}
		#endregion

		#region ISessionEventListener implementation
		public void Ping()
		{
		}

		public void OnSessionEnded()
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnSessionEnded();
		}

		public void OnGameEnded()
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnGameEnded();
		}

		public void OnPlayerJoinedSession(IPlayer player)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerJoinedSession(player);
		}

		public void OnSpectatorJoinedSession(ISpectator spectator)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnSpectatorJoinedSession(spectator);
		}

		public void OnPlayerLeftSession(IPlayer player)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerLeftSession(player);
		}

		public void OnSpectatorLeftSession(ISpectator spectator)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnSpectatorLeftSession(spectator);
		}

		public void OnPlayerUpdated(IPlayer player)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerUpdated(player);
		}

		public void OnPlayerDisconnected(IPlayer player)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerDisconnected(player);
		}

		public void OnChatMessage(IPlayer player, string message)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnChatMessage(player, message);
		}

		public void OnChatMessage(ISpectator spectator, string message)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnChatMessage(spectator, message);
		}

		public void OnPlayerDrewFromDeck(IPublicPlayerView player, System.Collections.ObjectModel.ReadOnlyCollection<ICard> drawnCards)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerDrewFromDeck(player, drawnCards);
		}

		public void OnPlayerDrewFromGraveyard(IPublicPlayerView player, System.Collections.ObjectModel.ReadOnlyCollection<ICard> drawnCards)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerDrewFromGraveyard(player, drawnCards);
		}

		public void OnPlayerDiscardedCard(IPublicPlayerView player, ICard card)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerDiscardedCard(player, card);
		}

		public void OnPlayerPlayedCard(IPublicPlayerView player, ICard card)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerPlayedCard(player, card);
		}

		public void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerPlayedCard(player, card, targetPlayer);
		}

		public void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerPlayedCard(player, card, targetPlayer, targetCard);
		}

		public void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerPlayedCard(player, card, asCard);
		}

		public void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer);
		}

		public void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard);
		}

		public void OnPlayerPlayedCardOnTable(IPublicPlayerView player, ICard card)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerPlayedCardOnTable(player, card);
		}

		public void OnPassedTableCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPassedTableCard(player, card, targetPlayer);
		}

		public void OnPlayerEndedTurn(IPublicPlayerView player)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerEndedTurn(player);
		}

		public void OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerRespondedWithCard(player, card);
		}

		public void OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card, CardType asCard)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerRespondedWithCard(player, card, asCard);
		}

		public void OnDrawnIntoSelection(System.Collections.ObjectModel.ReadOnlyCollection<ICard> drawnCards)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnDrawnIntoSelection(drawnCards);
		}

		public void OnPlayerPickedFromSelection(IPublicPlayerView player, ICard card)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerPickedFromSelection(player, card);
		}

		public void OnUndrawnFromSelection(ICard card)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnUndrawnFromSelection(card);
		}

		public void OnPlayerStoleCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerStoleCard(player, targetPlayer, targetCard);
		}

		public void OnPlayerCancelledCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerCancelledCard(player, targetPlayer, targetCard);
		}

		public void OnDeckChecked(ICard card)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnDeckChecked(card);
		}

		public void OnCardCancelled(ICard card)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnCardCancelled(card);
		}

		public void OnPlayerCheckedDeck(IPublicPlayerView player, ICard checkedCard, CardType causedBy, bool result)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerCheckedDeck(player, checkedCard, causedBy, result);
		}

		public void OnLifePointsChanged(IPublicPlayerView player, int delta, IPublicPlayerView causedBy)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnLifePointsChanged(player, delta, causedBy);
		}

		public void OnPlayerDied(IPublicPlayerView player, IPublicPlayerView causedBy)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerDied(player, causedBy);
		}

		public void OnPlayerUsedAbility(IPublicPlayerView player, CharacterType character)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerUsedAbility(player, character);
		}

		public void OnPlayerUsedAbility(IPublicPlayerView player, CharacterType character, IPublicPlayerView targetPlayer)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerUsedAbility(player, character, targetPlayer);
		}

		public void OnPlayerGainedAdditionalCharacters(IPublicPlayerView player)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerGainedAdditionalCharacters(player);
		}

		public void OnPlayerLostAdditionalCharacters(IPublicPlayerView player)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnPlayerLostAdditionalCharacters(player);
		}

		public void OnDeckRegenerated()
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnDeckRegenerated();
		}

		public void OnNewRequest(IPublicPlayerView requestedPlayer, IPublicPlayerView causedBy)
		{
			foreach(ISessionEventListener listener  in mainListeners.ToArray())
				listener.OnNewRequest(requestedPlayer, causedBy);
		}
		#endregion
	}
}

