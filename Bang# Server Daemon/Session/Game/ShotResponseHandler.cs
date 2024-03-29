// ShotResponseHandler.cs
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
using System.Collections.Generic;

namespace BangSharp.Server.Daemon
{
	public sealed class ShotResponseHandler : ResponseHandler
	{
		private int power;
		private List<Card> barrelsChecked;
		private bool abilityUsed;
		private bool canEndImmediately;

		public ShotResponseHandler(Player targetPlayer, Player causedBy, int power = 1) :
			base(RequestType.Shot, targetPlayer, causedBy)
		{
			this.power = power;
			barrelsChecked = new List<Card>();
			abilityUsed = false;
		}

		private void OnResult(Card card, bool result)
		{
			if(card != null)
				if(card.Type == CardType.Barrel)
					barrelsChecked.Add(card);
			if(result)
			{
				if(card != null && card.Type != CardType.Barrel)
					if(card.Type != CardType.Missed)
						Game.GameTable.PlayerRespondWithCard(card, CardType.Missed);
					else
						Game.GameTable.PlayerRespondWithCard(card);

				if(card != null && CausedBy != null && !CausedBy.HasCardEffect(card))
					return;
				if(--power == 0)
				{
					if(canEndImmediately)
						End();
				}
				else
				{
					barrelsChecked.Clear();
					abilityUsed = false;
				}
			}
		}
		protected override void OnRespondCard(Card card)
		{
			if(card.Owner != RequestedPlayer)
				throw new BadCardException();

			if(barrelsChecked.Contains(card))
				throw new BadCardException();

			canEndImmediately = false;
			RequestedPlayer.CheckMissed(card, OnResult);
			canEndImmediately = true;
			if(power == 0)
				End();
		}
		protected override void OnRespondUseAbility()
		{
			if(abilityUsed)
				throw new BadUsageException();
			
			canEndImmediately = false;
			RequestedPlayer.Character.CheckMissed(OnResult);
			canEndImmediately = true;
			if(power == 0)
				End();
			abilityUsed = true;
		}
		protected override void OnRespondNoAction()
		{
			RequestedPlayer.ModifyLifePoints(-1, CausedBy);
			End();
		}
	}
}
