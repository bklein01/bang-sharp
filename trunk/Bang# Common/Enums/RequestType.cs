// RequestType.cs
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
namespace BangSharp
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
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondDraw - player draws normally
		/// 	RespondUseAbility - player draws using character's ability
		/// </remarks>
		Draw,
		/// <summary>
		/// Player is requested to play a card (or end his turn)
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player plays the specified card
		/// 	RespondUseAbility - player uses his character's ability
		/// 	RespondNoAction - player decides to end his turn
		/// </remarks>
		Play,
		/// <summary>
		/// Player is requested to discard one of the cards in his hand.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player discards the specified card
		/// </remarks>
		DiscardCard,
		/// <summary>
		/// Player lost his last life and is requested to play a beer to save himself (or give up).
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player plays the specified beer card
		/// 	[RespondUseAbility - player uses his character's ability to save himself]
		/// 	RespondNoAction - player gives up and dies
		/// </remarks>
		BeerRescue,
		/// <summary>
		/// Player is being shot at.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player uses the card to avoid the shot
		/// 	RespondUseAbility - player uses his character's ability to avoid the shot
		/// 	RespondNoAction - player gives up and looses 1 life point
		/// </remarks>
		Shot,
		/// <summary>
		/// Player is requested to throw Bang! or loose 1 life point.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player throws the Bang! card
		/// 	RespondNoAction - player gives up and looses 1 life point
		/// </remarks>
		ThrowBang,
		/// <summary>
		/// Player is requested to choose a player to shoot at.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondPlayer - player chooses the specified player
		/// 	RespondNoAction - player cancels the action
		/// </remarks>
		ShotTarget,
		/// <summary>
		/// Player is requested to choose an opponent for duel.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondPlayer - player chooses the specified player
		/// 	RespondNoAction - player cancels the action
		/// </remarks>
		DuelTarget,
		/// <summary>
		/// Player is requested to choose a player to be put into jail.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondPlayer - player chooses the specified player
		/// 	RespondNoAction - player cancels the action
		/// </remarks>
		JailTarget,
		/// <summary>
		/// Player is requested to choose a player to be healed 1 life point.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondPlayer - player chooses the specified player
		/// 	RespondNoAction - player cancels the action
		/// </remarks>
		HealTarget,
		/// <summary>
		/// Player is requested to choose a card to steal from another player.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// 	RespondNoAction - player cancels the action
		/// </remarks>
		StealCard,
		/// <summary>
		/// Player is requested to choose a card to cancel from another player.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// 	RespondNoAction - player cancels the action
		/// </remarks>
		CancelCard,
		/// <summary>
		/// Player is requested to choose a card to discard in order to play a golden card.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// 	RespondNoAction - player cancels the action
		/// </remarks>
		GoldenCard,
		/// <summary>
		/// Player is requested to choose a card from the recently dead player to take in hand.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// </remarks>
		TakeDeadPlayersCard,
		/// <summary>
		/// Player is requested to choose which of his addiditonal characters is to be used for drawing cards.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCharacter - player chooses the specified character
		/// 	RespondNoAction - the default behavior is used
		/// </remarks>
		ChooseCharacterForDraw,
		/// <summary>
		/// Player is requested to choose which of his addiditonal characters is to be used for drawing the first card.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCharacter - player chooses the specified character
		/// 	RespondNoAction - the default behavior is used
		/// </remarks>
		ChooseCharacterForDrawFirstCard,
		/// <summary>
		/// Player is requested to choose which of his addiditonal characters is to be used for drawing the second card.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCharacter - player chooses the specified character
		/// 	RespondNoAction - the default behavior is used
		/// </remarks>
		ChooseCharacterForDrawSecondCard,
		/// <summary>
		/// Player is requested to choose which of his addiditonal characters is to be used for playing a card.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCharacter - player chooses the specified character
		/// 	RespondNoAction - the default behavior is used
		/// </remarks>
		ChooseCharacterForPlayCard,
		/// <summary>
		/// Player is requested to choose which of his addiditonal characters is to be used for voluntary abilty use.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCharacter - player chooses the specified character
		/// 	RespondNoAction - the default behavior is used
		/// </remarks>
		ChooseCharacterForUseAbility,
		/// <summary>
		/// Player is requested to choose which of his addiditonal characters is to be used for 'deck checking'.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCharacter - player chooses the specified character
		/// 	RespondNoAction - the default behavior is used
		/// </remarks>
		ChooseCharacterForCheckDeck,
		/// <summary>
		/// Player is requested to choose which of his addiditonal characters is to be used for avoiding the shot.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCharacter - player chooses the specified character
		/// 	RespondNoAction - the default behavior is used
		/// </remarks>
		ChooseCharacterForAvoidShot,
		#endregion
		#region Card Requests
		/// <summary>
		/// Player is requested to choose a card from the General Store (selection).
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// </remarks>
		GeneralStore,
		#endregion
		#region Character Requests
		/// <summary>
		/// Player is requested to choose 2 cards to discard  in order to shoot on a player.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player adds the specified card to those to be discarded
		/// 	RespondNoAction - player cancels the action
		/// </remarks>
		DocHolyday,
		/// <summary>
		/// Player is requested to choose a blue card to discard in order to draw 2 cards.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// 	RespondNoAction - player cancels the action
		/// </remarks>
		JoseDelgado,
		/// <summary>
		/// Player is requested to choose a card from the table to steal instead of standard draw.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// 	RespondNoAction - fallback to normal draw
		/// </remarks>
		PatBrennan,
		/// <summary>
		/// Player is requested to choose a player whose character's abilities he gains for the following round.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondPlayer - player chooses the specified player's character
		/// 	RespondNoAction - player chooses no character
		/// </remarks>
		VeraCuster,
		/// <summary>
		/// Player is requested to choose a card from the selection to draw.
		/// See  the Kit Carlson's ability for details.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// </remarks>
		KitCarlson,
		/// <summary>
		/// Player is requested to choose a card from the selection to be used for 'deck checking'.
		/// See the Lucky Duke's ability for details.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player chooses the specified card
		/// </remarks>
		LuckyDuke,
		/// <summary>
		/// Player is requested to choose 2 cards to discard in order to gain 1 life point.
		/// </summary>
		/// <remarks>
		/// Acceptable responses:
		/// 	RespondCard - player adds the specified card to those to be discarded
		/// 	RespondNoAction - player cancels the action
		/// </remarks>
		SidKetchum,
		#endregion
	}
}
