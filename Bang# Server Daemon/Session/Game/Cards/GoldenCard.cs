// GoldenCard.cs
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
namespace BangSharp.Server
{
	public abstract class GoldenCard : PlayableCard
	{
		private sealed class GoldenCardResponseHandler : ResponseHandler
		{
			private GoldenCard parent;
			private Card card;
			
			public GoldenCardResponseHandler(GoldenCard parent, Card card)
				: base(RequestType.GoldenCard, card.Owner)
			{
				this.parent = parent;
				this.card = card;
			}
			public GoldenCardResponseHandler(GoldenCard parent)
				: this(parent, parent)
			{
			}

			protected override void OnRespondCard(Card extraCard)
			{
				if(extraCard.Owner != RequestedPlayer)
					throw new BadCardException();

				if(extraCard == parent)
					throw new BadCardException();

				extraCard.AssertInHand();

				if(card != parent)
					parent.OnPlayVirtually(card, extraCard);
				else
					parent.OnPlay(extraCard);
				End();
			}
			protected override void OnRespondNoAction()
			{
				End();
			}
		}
		protected GoldenCard(Game game, int id, CardType type, CardSuit suit, CardRank rank)
			: base(game, id, type, suit, rank)
		{
		}
		
		protected override void OnPlay()
		{
			Game.GameCycle.PushTempHandler(new GoldenCardResponseHandler(this));
		}
		protected override void OnPlayVirtually(Card card)
		{
			Game.GameCycle.PushTempHandler(new GoldenCardResponseHandler(this, card));
		}
		protected abstract void OnPlay(Card extraCard);
		protected abstract void OnPlayVirtually(Card card, Card extraCard);
	}
}
