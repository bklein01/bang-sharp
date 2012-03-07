// SidKetchum.cs
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

namespace BangSharp.Server.Daemon.Characters
{
	public sealed class SidKetchum : Character
	{
		private sealed class SidKetchumResponseHandler : ResponseHandler
		{
			private SidKetchum parent;
			private List<Card> selected;
			
			public SidKetchumResponseHandler(SidKetchum parent)
				: base(RequestType.SidKetchum, parent.Player)
			{
				this.parent = parent;
				this.selected = new List<Card>(2);
			}
			
			protected override void OnRespondCard(Card card)
			{
				if(card.Owner != RequestedPlayer)
					throw new BadCardException();
				card.AssertInHand();
				
				if(selected.Contains(card))
					throw new BadCardException();
				selected.Add(card);
				if(selected.Count == 2)
				{
					parent.OnUsedAbility();
					foreach(Card c in selected)
						Game.GameTable.CancelCard(c);
					RequestedPlayer.ModifyLifePoints(1);
					End();
				}
			}
			protected override void OnRespondNoAction()
			{
				End();
			}
		}
		public SidKetchum(Player player)
			: base(player, CharacterType.SidKetchum)
		{
		}
		
		public override void UseAbility()
		{
			if(Player.LifePoints == Player.MaxLifePoints)
				throw new BadUsageException();
			Game.GameCycle.PushTempHandler(new SidKetchumResponseHandler(this));
		}
	}
}
