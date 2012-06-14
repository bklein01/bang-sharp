// DocHolyday.cs
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
using System.Linq;

namespace BangSharp.Server.Daemon.Characters
{
	public sealed class DocHolyday : Character
	{
		private sealed class DocHolydayResponseHandler : ResponseHandler
		{
			private sealed class TargetPlayerResponseHandler : ResponseHandler
			{
				private DocHolydayResponseHandler parent;

				public TargetPlayerResponseHandler(DocHolydayResponseHandler parent) :
					base(RequestType.ShotTarget, parent.RequestedPlayer)
				{
					this.parent = parent;
				}

				protected override void OnRespondPlayer(Player targetPlayer)
				{
					if(targetPlayer != null)
						throw new BadUsageException();

					if(targetPlayer == RequestedPlayer)
						throw new BadTargetPlayerException();

					if(!targetPlayer.IsAlive)
						throw new BadTargetPlayerException();

					if(RequestedPlayer.WeaponRange < Game.GetDistance(RequestedPlayer, targetPlayer))
						throw new BadTargetPlayerException();

					parent.parent.OnUsedAbility(targetPlayer);
					foreach(Card c in parent.selected)
						Game.GameTable.PlayerDiscardCard(c);
					if(parent.selected.Any(c => targetPlayer.HasCardEffect(c)))
						Game.GameCycle.PushTempHandler(new ShotResponseHandler(targetPlayer, RequestedPlayer));
					End();
				}
				protected override void OnRespondNoAction()
				{
					End();
				}
			}
			private DocHolyday parent;
			private List<Card> selected;

			public DocHolydayResponseHandler(DocHolyday parent) :
				base(RequestType.DocHolyday, parent.Player)
			{
				this.parent = parent;
				selected = new List<Card>(2);
			}

			protected override void OnRespondCard(Card card)
			{
				if(selected.Count == 2)
					throw new BadUsageException();
				if(card.Owner != RequestedPlayer)
					throw new BadCardException();
				card.AssertInHand();
				
				if(selected.Contains(card))
					throw new BadCardException();
				selected.Add(card);
				if(selected.Count == 2)
				{
					Game.GameCycle.PushTempHandler(new TargetPlayerResponseHandler(this));
					End();
				}
			}
			protected override void OnRespondNoAction()
			{
				End();
			}
		}
		public DocHolyday(Player player) :
			base(player, CharacterType.DocHolyday)
		{
		}
		
		public override void UseAbility()
		{
			Game.GameCycle.PushTempHandler(new DocHolydayResponseHandler(this));
		}
	}
}
