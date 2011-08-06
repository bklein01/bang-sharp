// RequestType.cs
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
namespace Bang
{
	/// <summary>
	/// Represents the type of a request.
	/// </summary>
	public enum RequestType
	{
		/// <summary>
		/// Should be only used internally for container request handlers.
		/// </summary>
		None,
		
		#region Main Requests
		/// <summary>
		/// Player is requested to draw cards.
		/// Acceptable responses:
		/// 	RespondDraw - player draws normally
		/// 	RespondUseAbility - player draws using character's ability
		/// </summary>
		Draw,
		/// <summary>
		/// Player is requested to play a card (or end his turn)
		/// Acceptable responses:
		/// 	RespondCard - player plays the specified card
		/// 	RespondUseAbility - player uses his character's ability
		/// 	RespondNoAction - player decides to end his turn
		/// </summary>
		Play,
		/// <summary>
		/// Player is requested to discard one of the cards in his hand.
		/// Acceptable responses:
		/// 	RespondCard - player discards the specified card
		/// </summary>
		DiscardCard,
		/// <summary>
		/// Player lost his last life and is requested to play a beer to save himself (or give up).
		/// Acceptable responses:
		/// 	RespondCard - player plays the specified beer card
		/// 	[RespondUseAbility - player uses his character's ability to save himself]
		/// 	RespondNoAction - player gives up and dies
		/// </summary>
		BeerRescue,
		/// <summary>
		/// Player is being shot at.
		/// Acceptable responses:
		/// 	RespondCard - player uses the card to avoid the shot
		/// 	RespondUseAbility - player uses his character's ability to avoid the shot
		/// 	RespondNoAction - player gives up and looses 1 life point
		/// </summary>
		Shot,
		/// <summary>
		/// Player is requested to throw Bang! or loose 1 life point.
		/// Acceptable responses:
		/// 	RespondCard - player throws the Bang! card
		/// 	RespondNoAction - player gives up and looses 1 life point
		/// </summary>
		ThrowBang,
		/// <summary>
		/// Player is requested to choose a player to shoot at.
		/// Acceptable responses:
		/// 	RespondPlayer - player chooses the specified player
		/// 	RespondNoAction - player cancels the action
		/// </summary>
		ShotTarget,
		/// <summary>
		/// Player is requested to choose an opponent for duel.
		/// Acceptable responses:
		/// 	RespondPlayer - player chooses the specified player
		/// 	RespondNoAction - player cancels the action
		/// </summary>
		DuelTarget,
		/// <summary>
		/// Player is requested to choose a player to be put into jail.
		/// Acceptable responses:
		/// 	RespondPlayer - player chooses the specified player
		/// 	RespondNoAction - player cancels the action
		/// </summary>
		JailTarget,
		/// <summary>
		/// Player is requested to choose a player to be healed 1 life point.
		/// Acceptable responses:
		/// 	RespondPlayer - player chooses the specified player
		/// 	RespondNoAction - player cancels the action
		/// </summary>
		HealTarget,
		/// <summary>
		/// Player is requested to choose a card to steal from another player.
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// 	RespondNoAction - player cancels the action
		/// </summary>
		StealCard,
		/// <summary>
		/// Player is requested to choose a card to cancel from another player.
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// 	RespondNoAction - player cancels the action
		/// </summary>
		CancelCard,
		/// <summary>
		/// Player is requested to choose a card to discard in order to play a golden card.
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// 	RespondNoAction - player cancels the action
		/// </summary>
		GoldenCard,
		/// <summary>
		/// Player is requested to choose a card from the recently dead player to take in hand.
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// </summary>
		TakeDeadPlayersCard,
		#endregion
		#region Card Requests
		/// <summary>
		/// Player is requested to choose a card from the General Store (selection).
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// </summary>
		GeneralStore,
		#endregion
		#region Character Requests
		/// <summary>
		/// Player is requested to choose 2 cards to discard and a player to shoot at.
		/// Acceptable responses:
		/// 	RespondCard - player adds the specified card to those to be discarded
		/// 	RespondPlayer - player chooses the specified player as the target of the shot
		/// 	RespondNoAction - player cancels the action
		/// </summary>
		DocHolyday,
		/// <summary>
		/// Player is requested to choose a blue card to discard in order to draw 2 cards.
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// 	RespondNoAction - player cancels the action
		/// </summary>
		JoseDelgado,
		/// <summary>
		/// Player is requested to choose a card from the table to steal instead of standard draw.
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// </summary>
		PatBrennan,
		/// <summary>
		/// Player is requested to choose a player whose character's abilities he gains for the following round.
		/// Acceptable responses:
		/// 	RespondPlayer - player chooses the specified player's character
		/// 	RespondNoAction - player chooses no character
		/// </summary>
		VeraCuster,
		/// <summary>
		/// Player is requested to choose a card from the selection to draw.
		/// See  the Kit Carlson's ability for details.
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// </summary>
		KitCarlson,
		/// <summary>
		/// Player is requested to choose a card from the selection to be used for 'deck checking'.
		/// See the Lucky Duke's ability for details.
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// </summary>
		LuckyDuke,
		/// <summary>
		/// Player is requested to choose 2 cards to discard in order to gain 1 life point.
		/// Acceptable responses:
		/// 	RespondCard - player adds the specified card to those to be discarded
		/// 	RespondNoAction - player cancels the action
		/// </summary>
		SidKetchum,
		#endregion
	}
}

