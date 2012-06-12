// AllPlayersGoldenCard.cs
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
	public abstract class AllPlayersGoldenCard : GoldenCard
	{
		private bool includeSelf;

		protected AllPlayersGoldenCard(Game game, int id, CardType type, CardSuit suit, CardRank rank, bool includeSelf = false) :
			base(game, id, type, suit, rank)
		{
			this.includeSelf = includeSelf;
		}

		protected override void OnPlay(Card extraCard)
		{
			Player owner = Owner;
			Player current = owner;
			Game.GameTable.PlayerPlayCard(this);
			Game.GameTable.PlayerDiscardCard(extraCard);
			
			if(!includeSelf)
				current = Game.NextPlayer(current);
			
			List<ResponseHandler> handlers = new List<ResponseHandler>();
			do
			{
				if(current == owner || current.HasCardEffect(this))
				{
					ResponseHandler h = OnPlay(owner, current);
					if(h != null)
						handlers.Add(h);
				}
				current = Game.NextPlayer(current);
			}
			while(current != owner);
			if(handlers.Count != 0)
				Game.GameCycle.PushTempHandler(new QueueResponseHandler(handlers));
		}
		protected override void OnPlayVirtually(Card card, Card extraCard)
		{
			Player owner = card.Owner;
			Player current = owner;
			Game.GameTable.PlayerPlayCard(card, this.Type);
			Game.GameTable.PlayerDiscardCard(extraCard);
			
			if(!includeSelf)
				current = Game.NextPlayer(current);
			
			List<ResponseHandler> handlers = new List<ResponseHandler>();
			do
			{
				if(current == owner || current.HasCardEffect(this))
				{
					ResponseHandler h = OnPlay(owner, current);
					if(h != null)
						handlers.Add(h);
				}
				current = Game.NextPlayer(current);
			}
			while(current != owner);
			if(handlers.Count != 0)
				Game.GameCycle.PushTempHandler(new QueueResponseHandler(handlers));
		}

		protected abstract ResponseHandler OnPlay(Player owner, Player targetPlayer);
	}
}
