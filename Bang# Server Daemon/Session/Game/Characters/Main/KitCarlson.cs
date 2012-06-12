// KitCarlson.cs
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
namespace BangSharp.Server.Daemon.Characters
{
	public sealed class KitCarlson : Character
	{
		private sealed class KitCarlsonResponseHandler : ResponseHandler
		{
			private KitCarlson parent;
			
			public KitCarlsonResponseHandler(KitCarlson parent) :
				base(RequestType.KitCarlson, parent.Player)
			{
				this.parent = parent;
			}
			
			protected override void OnStart()
			{
				Game.GameTable.DrawIntoSelection(3, RequestedPlayer);
			}
			
			protected override void OnRespondCard(Card card)
			{
				if(!Game.GameTable.Selection.Contains(card))
					throw new BadCardException();
				
				if(Game.GameTable.Selection.Count == 3)
				{
					Game.GameTable.PlayerPickFromSelection(RequestedPlayer, card, RequestedPlayer.RevealFirstDrawnCard);
					RequestedPlayer.OnDrewFirstCard(card);
				}
				else if(Game.GameTable.Selection.Count == 2)
				{
					Game.GameTable.PlayerPickFromSelection(RequestedPlayer, card, RequestedPlayer.RevealSecondDrawnCard);
					RequestedPlayer.OnDrewSecondCard(card);
					Game.GameTable.UndrawFromSelection(Game.GameTable.Selection[0]);
					End();
				}
			}
		}
		
		public KitCarlson(Player player) :
			base(player, CharacterType.KitCarlson)
		{
		}
		
		public override void Draw()
		{
			OnUsedAbility();
			Game.GameCycle.PushTempHandler(new KitCarlsonResponseHandler(this));
		}
	}
}
