// VirtualSessionEventListener.cs
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

namespace BangSharp
{
	/// <summary>
	/// Virtual session event listener.
	/// </summary>
	/// <remarks>
	/// A base class for session event listeners with all memebers (except for the <see cref="BangSharp.IEventListener.Ping()"/> method) implemented as virtual.
	/// </remarks>
	public class VirtualSessionEventListener : ISpectatorSessionEventListener, IPlayerSessionEventListener
	{
		protected VirtualSessionEventListener()
		{
		}

		#region IPlayerSessionEventListener implementation
		public virtual void OnJoinedSession(IPlayerSessionControl control)
		{
		}

		public virtual void OnJoinedGame(IPlayerControl control)
		{
		}

		public virtual void OnNewRequest(RequestType requestType, IPublicPlayerView causedBy)
		{
		}

		public virtual bool IsAI
		{
			get { return false; }
		}
		#endregion

		#region ISpectatorSessionEventListener implementation
		public virtual void OnJoinedSession(ISpectatorSessionControl control)
		{
		}

		public virtual void OnJoinedGame(ISpectatorControl control)
		{
		}
		#endregion

		#region ISessionEventListener implementation
		public void Ping()
		{
		}

		public virtual void OnSessionEnded()
		{
		}

		public virtual void OnGameEnded()
		{
		}

		public virtual void OnPlayerJoinedSession(IPlayer player)
		{
		}

		public virtual void OnSpectatorJoinedSession(ISpectator spectator)
		{
		}

		public virtual void OnPlayerLeftSession(IPlayer player)
		{
		}

		public virtual void OnSpectatorLeftSession(ISpectator spectator)
		{
		}

		public virtual void OnPlayerUpdated(IPlayer player)
		{
		}

		public virtual void OnPlayerDisconnected(IPlayer player)
		{
		}

		public virtual void OnChatMessage(IPlayer player, string message)
		{
		}

		public virtual void OnChatMessage(ISpectator spectator, string message)
		{
		}

		public virtual void OnPlayerDrewFromDeck(IPublicPlayerView player, System.Collections.ObjectModel.ReadOnlyCollection<ICard> drawnCards)
		{
		}

		public virtual void OnPlayerDrewFromGraveyard(IPublicPlayerView player, System.Collections.ObjectModel.ReadOnlyCollection<ICard> drawnCards)
		{
		}

		public virtual void OnPlayerDiscardedCard(IPublicPlayerView player, ICard card)
		{
		}

		public virtual void OnPlayerPlayedCard(IPublicPlayerView player, ICard card)
		{
		}

		public virtual void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
		}

		public virtual void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer, ICard targetCard)
		{
		}

		public virtual void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard)
		{
		}

		public virtual void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer)
		{
		}

		public virtual void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer, ICard targetCard)
		{
		}

		public virtual void OnPlayerPlayedCardOnTable(IPublicPlayerView player, ICard card)
		{
		}

		public virtual void OnPassedTableCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
		}

		public virtual void OnPlayerPassed(IPublicPlayerView player)
		{
		}

		public virtual void OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card)
		{
		}

		public virtual void OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card, CardType asCard)
		{
		}

		public virtual void OnDrawnIntoSelection(System.Collections.ObjectModel.ReadOnlyCollection<ICard> drawnCards)
		{
		}

		public virtual void OnPlayerPickedFromSelection(IPublicPlayerView player, ICard card)
		{
		}

		public virtual void OnUndrawnFromSelection(ICard card)
		{
		}

		public virtual void OnPlayerStoleCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
		}

		public virtual void OnPlayerCancelledCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
		}

		public virtual void OnDeckChecked(ICard card)
		{
		}

		public virtual void OnCardCancelled(ICard card)
		{
		}

		public virtual void OnPlayerCheckedDeck(IPublicPlayerView player, ICard checkedCard, CardType causedBy, bool result)
		{
		}

		public virtual void OnLifePointsChanged(IPublicPlayerView player, int delta, IPublicPlayerView causedBy)
		{
		}

		public virtual void OnPlayerDied(IPublicPlayerView player, IPublicPlayerView causedBy)
		{
		}

		public virtual void OnPlayerUsedAbility(IPublicPlayerView player, CharacterType character)
		{
		}

		public virtual void OnPlayerUsedAbility(IPublicPlayerView player, CharacterType character, IPublicPlayerView targetPlayer)
		{
		}

		public virtual void OnPlayerGainedAdditionalCharacters(IPublicPlayerView player)
		{
		}

		public virtual void OnPlayerLostAdditionalCharacters(IPublicPlayerView player)
		{
		}

		public virtual void OnDeckRegenerated()
		{
		}
		#endregion
	}
}

