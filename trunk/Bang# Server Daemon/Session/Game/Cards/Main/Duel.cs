// Duel.cs
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
namespace BangSharp.Server.Cards
{
	public sealed class Duel : TargetPlayerCard
	{
		private class DuelResponseHandler : ResponseHandler
		{
			private Duel card;
			private Player targetPlayer;
			private Player owner;
			private Player current;

			public DuelResponseHandler(Duel card, Player targetPlayer, Player owner)
				: base(targetPlayer, owner)
			{
				this.card = card;
				this.targetPlayer = targetPlayer;
				this.owner = owner;
			}

			protected override void OnStart()
			{
				current = targetPlayer;
				PushHandler(new ThrowBangResponseHandler(current, owner, OnResult));
			}

			protected override void OnContinue()
			{
				End();
			}
			
			private void NextPlayer()
			{
				current = current == targetPlayer ? owner : targetPlayer;
			}
			
			private void OnResult(bool result)
			{
				if(result)
				{
					NextPlayer();
					PushHandler(new ThrowBangResponseHandler(current, owner, OnResult));
				}
			}
		}
		
		public Duel(Game game, int id, CardSuit suit, CardRank rank)
			: base(game, id, CardType.Duel, suit, rank, RequestType.DuelTarget)
		{
		}
		
		protected override void OnPlay(Player owner, Player targetPlayer)
		{
			Game.GameCycle.PushTempHandler(new DuelResponseHandler(this, targetPlayer, owner));
		}
	}
}
