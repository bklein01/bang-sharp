// AllPlayersCardGoldenCard.cs
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
	public abstract class AllPlayersCardGoldenCard : AllPlayersGoldenCard
	{
		private sealed class AllPlayersCardGoldenCardResponseHandler : ResponseHandler
		{
			private AllPlayersCardGoldenCard parent;
			private Player targetPlayer;

			public AllPlayersCardGoldenCardResponseHandler (AllPlayersCardGoldenCard parent, Player targetPlayer, Player owner)
				: base(parent.reqType, owner)
			{
				this.parent = parent;
				this.targetPlayer = targetPlayer;
			}

			protected override void OnRespondCard (Card targetCard)
			{
				if (targetCard.Owner != targetPlayer)
					throw new BadTargetCardException ();

				parent.OnPlay(RequestedPlayer, targetCard);
				End ();
			}
		}
		private RequestType reqType;

		protected AllPlayersCardGoldenCard(Game game, int id, CardType type, CardSuit suit, CardRank rank, RequestType reqType, bool includeSelf)
			: base(game, id, type, suit, rank, includeSelf)
		{
			this.reqType = reqType;
		}
		protected AllPlayersCardGoldenCard (Game game, int id, CardType type, CardSuit suit, CardRank rank, RequestType reqType)
			: base(game, id, type, suit, rank)
		{
			this.reqType = reqType;
		}
		
		protected override ResponseHandler OnPlay(Player owner, Player targetPlayer)
		{
			if(targetPlayer.Hand.Count != 0 || targetPlayer.Table.Count != 0)
				return new AllPlayersCardGoldenCardResponseHandler(this, targetPlayer, owner);
			return null;
		}
		protected abstract void OnPlay(Player owner, Card targetCard);
	}
}

