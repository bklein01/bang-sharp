// IEventListener.cs
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
using System.Collections.ObjectModel;
namespace Bang
{
	/// <summary>
	/// Contains basic session and game events.
	/// </summary>
	public interface IEventListener
	{
		/// <summary>
		/// Used when checking the availability of the listener. Should be an empty method.
		/// </summary>
		void Ping();
		/// <summary>
		/// Fired when the session has ended.
		/// </summary>
		void OnSessionEnded();
		/// <summary>
		/// Fired when the game has ended.
		/// </summary>
		void OnGameEnded();

		/// <summary>
		/// Fired when a player has joined the session.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPlayer"/> that has joined the session.
		/// </param>
		void OnPlayerJoinedSession(IPlayer player);
		/// <summary>
		/// Fired when a spectator has joined the session.
		/// </summary>
		/// <param name="spectator">
		/// The <see cref="ISpectator"/> that has joined the session.
		/// </param>
		void OnSpectatorJoinedSession(ISpectator spectator);
		/// <summary>
		/// Fired when a player has left the session.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPlayer"/> that has joined the session.
		/// </param>
		void OnPlayerLeftSession (IPlayer player);
		/// <summary>
		/// Fired when a spectator has left the session.
		/// </summary>
		/// <param name="spectator">
		/// The <see cref="ISpectator"/> that has left the session.
		/// </param>
		void OnSpectatorLeftSession (ISpectator spectator);
		/// <summary>
		/// Fired when a player has updated.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPlayer"/> that has updated.
		/// </param>
		void OnPlayerUpdated (IPlayer player);

		/// <summary>
		/// Fired when a player has sent a chat message.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPlayer"/> that has sent the message.
		/// </param>
		/// <param name="message">
		/// The content of the message.
		/// </param>
		void OnChatMessage(IPlayer player, string message);
		/// <summary>
		/// Fired when a spectator has sent a chat message.
		/// </summary>
		/// <param name="spectator">
		/// The <see cref="ISpectator"/> that has sent the message.
		/// </param>
		/// <param name="message">
		/// The content of the message.
		/// </param>
		void OnChatMessage (ISpectator spectator, string message);

		/// <summary>
		/// Fired when a new request is pending.
		/// </summary>
		/// <param name="requestedPlayer">
		/// The <see cref="IPublicPlayerView"/> of the player that is now being requested.
		/// </param>
		/// <param name="requestType">
		/// The new <see cref="RequestType"/>.
		/// </param>
		void OnNewRequest (RequestType requestType, IPublicPlayerView requestedPlayer, IPublicPlayerView causedBy);

		/// <summary>
		/// Fired when a player has drawn one or more cards from the deck.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="drawnCards">
		/// The <see cref="ReadOnlyCollection<ICard>"/> that contains the drawn cards.
		/// </param>
		void OnPlayerDrewFromDeck(IPublicPlayerView player, ReadOnlyCollection<ICard> drawnCards);
		/// <summary>
		/// Fired when a player has drawn one or more cards from the graveyard.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="drawnCards">
		/// The <see cref="ReadOnlyCollection<ICard>"/> that contains the drawn cards.
		/// </param>
		void OnPlayerDrewFromGraveyard (IPublicPlayerView player, ReadOnlyCollection<ICard> drawnCards);
		/// <summary>
		/// Fired when a player has discarded one of his cards.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="card">
		/// The discarded <see cref="ICard"/>.
		/// </param>
		void OnPlayerDiscardedCard (IPublicPlayerView player, ICard card);
		/// <summary>
		/// Fired when a player has played a card.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="card">
		/// The played <see cref="ICard"/>.
		/// </param>
		void OnPlayerPlayedCard (IPublicPlayerView player, ICard card);
		/// <summary>
		/// Fired when a player has played a card on a target player.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="card">
		/// The played <see cref="ICard"/>.
		/// </param>
		/// <param name="targetPlayer">
		/// The <see cref="IPublicPlayerView"/> of the target player.
		/// </param>
		void OnPlayerPlayedCard (IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer);
		/// <summary>
		/// Fired when a player plays a card on a target card.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="card">
		/// The played <see cref="ICard"/>.
		/// </param>
		/// <param name="targetPlayer">
		/// The <see cref="IPublicPlayerView"/> of the owner of the target card.
		/// </param>
		/// <param name="targetCard">
		/// The target <see cref="ICard"/>.
		/// </param>
		void OnPlayerPlayedCard (IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer, ICard targetCard);
		/// <summary>
		/// Fired when a player has played a card with the effect of another card type.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="card">
		/// The played <see cref="ICard"/>.
		/// </param>
		/// <param name="asCard">
		/// The <see cref="CardType"/> whose effect the card has had.
		/// </param>
		void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard);
		/// <summary>
		/// Fired when a player plays a card with the effect of another card type on a target player.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="card">
		/// The played <see cref="ICard"/>.
		/// </param>
		/// <param name="asCard">
		/// The <see cref="CardType"/> whose effect the card has had.
		/// </param>
		/// <param name="targetPlayer">
		/// The <see cref="IPublicPlayerView"/> of the target player.
		/// </param>
		void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer);
		/// <summary>
		/// Fired when a player has played a card with the effect of another card type on a target card.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="card">
		/// The played <see cref="ICard"/>.
		/// </param>
		/// <param name="asCard">
		/// The <see cref="CardType"/> whose effect the card has had.
		/// </param>
		/// <param name="targetPlayer">
		/// The <see cref="IPublicPlayerView"/> of the owner of the target card.
		/// </param>
		/// <param name="targetCard">
		/// The target <see cref="ICard"/>.
		/// </param>
		void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer, ICard targetCard);
		/// <summary>
		/// Fired when a player has played a card on his table.
		/// </summary>
		/// <summary>
		/// Fired when a player discards one of his cards.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="card">
		/// The played <see cref="ICard"/>.
		/// </param>
		void OnPlayerPlayedCardOnTable(IPublicPlayerView player, ICard card);
		/// <summary>
		/// Fired when a player has passed a card to another player's table.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="card">
		/// The passed <see cref="ICard"/>.
		/// </param>
		/// <param name="targetPlayer">
		/// The <see cref="IPublicPlayerView"/> of the target player.
		/// </param>
		void OnPassedTableCard (IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer);
		/// <summary>
		/// Fired when a player has ended his turn.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		void OnPlayerPassed (IPublicPlayerView player);
		/// <summary>
		/// Fired when a player has responded with a card.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="card">
		/// The <see cref="ICard"/> responded with.
		/// </param>
		void OnPlayerRespondedWithCard (IPublicPlayerView player, ICard card);
		/// <summary>
		/// Fired when a player has responded with a card with the effect of another card type.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="card">
		/// The <see cref="ICard"/> responded with.
		/// </param>
		/// <param name="asCard">
		/// The <see cref="CardType"/> whose effect the card has had.
		/// </param>
		void OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card, CardType asCard);
		/// <summary>
		/// Fired when one or more cards have been drawn into the selection.
		/// </summary>
		/// <param name="drawnCards">
		/// The <see cref="ReadOnlyCollection<ICard>"/> containing the drawn cards.
		/// </param>
		void OnDrawnIntoSelection (ReadOnlyCollection<ICard> drawnCards);
		/// <summary>
		/// Fired when a player has picked a card from the selection.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="card">
		/// The picked <see cref="ICard"/>.
		/// </param>
		void OnPlayerPickedFromSelection (IPublicPlayerView player, ICard card);
		/// <summary>
		/// Fired when a card has been undrawn from the selection back to the deck.
		/// </summary>
		/// <param name="card">
		/// The undrawn <see cref="ICard"/>.
		/// </param>
		void OnUndrawnFromSelection (ICard card);
		/// <summary>
		/// Fired when a player has stolen a card from another player.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="targetPlayer">
		/// The <see cref="IPublicPlayerView"/> of the owner of the target card.
		/// </param>
		/// <param name="targetCard">
		/// The stolen <see cref="ICard"/>.
		/// </param>
		void OnPlayerStoleCard (IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard);
		/// <summary>
		/// Fired when a player has cancelled a card from another player.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="targetPlayer">
		/// The <see cref="IPublicPlayerView"/> of the owner of the target card.
		/// </param>
		/// <param name="targetCard">
		/// The canceled <see cref="ICard"/>.
		/// </param>
		void OnPlayerCancelledCard (IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard);
		/// <summary>
		/// Fired when a card has been taken from the deck and cancelled (while 'deck checking').
		/// </summary>
		/// <param name="card">
		/// The checked <see cref="ICard"/>.
		/// </param>
		void OnDeckChecked (ICard card);
		/// <summary>
		/// Fired when a card has been cancelled (not by a player).
		/// </summary>
		/// <param name="card">
		/// The cancelled <see cref="ICard"/>.
		/// </param>
		void OnCardCancelled (ICard card);

		/// <summary>
		/// Fired when a player has 'checked the deck'.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="checkedCard">
		/// The <see cref="ICard"/> checked from the deck.
		/// </param>
		/// <param name="causedBy">
		/// The <see cref="CardType"/> because of which the card has been checked.
		/// </param>
		/// <param name="result">
		/// The <see cref="bool"/> indicating wheter the deck checking finished with a positive result.
		/// </param>
		void OnPlayerCheckedDeck (IPublicPlayerView player, ICard checkedCard, CardType causedBy, bool result);
		/// <summary>
		/// Fired when a player's life point count has changed.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		/// <param name="delta">
		/// The differece between the new and old life point count (in this order).
		/// </param>
		/// <param name="causedBy">
		/// The <see cref="IPublicPlayerView"/> of the player that had caused the change.
		/// </param>
		void OnLifePointsChanged(IPublicPlayerView player, int delta, IPublicPlayerView causedBy);
		/// <summary>
		/// Fired when a player has died.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the dead player.
		/// </param>
		/// <param name="causedBy">
		/// The <see cref="IPublicPlayerView"/> of the player that killed the player.
		/// </param>
		void OnPlayerDied(IPublicPlayerView player, IPublicPlayerView causedBy);
		/// <summary>
		/// Fired when a player is going to use his charcter's ability.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player.
		/// </param>
		void OnPlayerUsedAbility(IPublicPlayerView player);
		/// <summary>
		/// Fired when a player has gained additional characters.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player that has gained the characters.
		/// </param>
		void OnPlayerGainedAdditionalCharacters(IPublicPlayerView player);
		/// <summary>
		/// Fired when a player has lost his additional characters.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> of the player that has lost the characters.
		/// </param>
		void OnPlayerLostAdditionalCharacters(IPublicPlayerView player);
		/// <summary>
		/// Fired when the deck has been regenerated (the graveyard peek remains unchanged, the rest of the graveyard is reversed and put to the deck).
		/// </summary>
		void OnDeckRegenerated();
	}
}

