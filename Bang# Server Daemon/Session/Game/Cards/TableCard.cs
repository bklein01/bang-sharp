// TableCard.cs
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
using System.Collections.Generic;
namespace Bang.Server
{
	public class TableCard : PlayableCard
	{
		private bool playBlocked;

		protected TableCard(Game game, int id, CardType type, CardSuit suit, CardRank rank) : base(game, id, type, suit, rank)
		{
			playBlocked = false;
		}

		public void OnTurnEnded()
		{
			if(IsOnTable)
				playBlocked = false;
		}

		protected override void OnPlay()
		{
			if(IsInHand)
			{
				List<TableCard> tableCopy = new List<TableCard>(Owner.Table);
				foreach(TableCard c in tableCopy)
					if(c.Type == Type)
						Game.GameTable.PlayerDiscardCard(c);
					else if(c.IsWeapon && IsWeapon)
						Game.GameTable.PlayerDiscardCard(c);
				Game.GameTable.PlayerPlayCardOnTable(this);
				playBlocked = true;
			}
			else
			{
				if(playBlocked)
					throw new BadUsageException();
				PlayFromTable();
			}
		}
		protected override void OnPlayVirtually (Card card)
		{
			PlayFromTableVirtually(card);
		}
		protected virtual void PlayFromTable()
		{
			throw new BadUsageException();
		}
		protected virtual void PlayFromTableVirtually(Card card)
		{
			throw new BadUsageException();
		}

		public override void CheckMissed(ICardResultHandler handler)
		{
			if(IsOnTable && !playBlocked)
				OnCheckMissed(handler);
			else
				throw new BadCardException();
		}
		protected virtual void OnCheckMissed(ICardResultHandler handler)
		{
			throw new BadCardException();
		}

		public void PredrawCheck()
		{
			AssertOnTable();
			OnPredrawCheck();
		}
		protected virtual void OnPredrawCheck()
		{
			throw new BadUsageException();
		}
	}
}

