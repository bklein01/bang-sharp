// TargetCardGoldenCard.cs
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
namespace Bang.Server
{
	public abstract class TargetCardGoldenCard : GoldenCard
	{
		private sealed class TargetCardGoldenCardResponseHandler : ResponseHandler
		{
			private TargetCardGoldenCard parent;
			private Card card;
			private Card extraCard;

			public TargetCardGoldenCardResponseHandler (TargetCardGoldenCard parent, Card card, Card extraCard)
				: base(parent.reqType, card.Owner)
			{
				this.parent = parent;
				this.card = card;
				this.extraCard = extraCard;
			}
			public TargetCardGoldenCardResponseHandler (TargetCardGoldenCard parent, Card extraCard)
				: this(parent, parent, extraCard)
			{
			}

			protected override void OnRespondCard(Card targetCard)
			{
				Player targetPlayer = targetCard.Owner;
				if(targetPlayer == RequestedPlayer && !parent.includeSelf)
					throw new BadTargetPlayerException();
				
				if(!targetPlayer.IsAlive)
					throw new BadTargetPlayerException();
				
				if(parent.Range != 0 && parent.Range < Game.GetDistance(RequestedPlayer, targetPlayer))
					throw new BadTargetPlayerException();

				if(card != parent)
					Game.GameTable.PlayerPlayCard(card, parent.Type, targetCard);
				else
					Game.GameTable.PlayerPlayCard(card, targetCard);
				Game.GameTable.PlayerDiscardCard(extraCard);
				
				if(targetPlayer.HasCardEffect(card))
					parent.OnPlay(RequestedPlayer, targetCard);
				End ();
			}
			protected override void OnRespondNoAction ()
			{
				End ();
			}
		}
		private RequestType reqType;
		private bool includeSelf;

		protected virtual int Range
		{
			get { return 0; }
		}

		protected TargetCardGoldenCard(Game game, int id, CardType type, CardSuit suit, CardRank rank, RequestType reqType, bool includeSelf)
			: base(game, id, type, suit, rank)
		{
			this.reqType = reqType;
			this.includeSelf = includeSelf;
		}
		protected TargetCardGoldenCard (Game game, int id, CardType type, CardSuit suit, CardRank rank, RequestType reqType)
			: this(game, id, type, suit, rank, reqType, false)
		{
		}

		protected override void OnPlay(Card extraCard)
		{
			Game.GameCycle.PushTempHandler(new TargetCardGoldenCardResponseHandler(this, extraCard));
		}
		protected override void OnPlayVirtually (Card card, Card extraCard)
		{
			Game.GameCycle.PushTempHandler(new TargetCardGoldenCardResponseHandler(this, card, extraCard));
		}
		protected abstract void OnPlay (Player owner, Card targetCard);
	}
}

