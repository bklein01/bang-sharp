// LuckyDuke.cs
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
	public sealed class LuckyDuke : Character
	{
		private sealed class LuckyDukeResponseHandler : ResponseHandler
		{
			private LuckyDuke parent;
			private Card causedBy;
			private CheckDeckCallback checkCallback;
			private CardResultCallback resultCallback;
			
			public LuckyDukeResponseHandler(LuckyDuke parent, Card causedBy, CheckDeckCallback checkCallback, CardResultCallback resultCallback) :
				base(RequestType.LuckyDuke, parent.Player)
			{
				this.parent = parent;
				this.causedBy = causedBy;
				this.checkCallback = checkCallback;
				this.resultCallback = resultCallback;
			}
			
			protected override void OnStart()
			{
				parent.OnUsedAbility();
				Game.GameTable.DrawIntoSelection(2, null);
			}
			
			protected override void OnRespondCard(Card card)
			{
				if(!Game.GameTable.Selection.Contains(card))
					throw new BadCardException();
				
				bool result = checkCallback(card);
				Game.Session.EventManager.OnPlayerCheckedDeck(RequestedPlayer, card, causedBy, result);
				Game.GameTable.CancelSelection();
				resultCallback(causedBy, result);
				End();
			}
		}
		private sealed class LuckyDukeQueueResponseHandler : QueueResponseHandler
		{
			private LuckyDuke parent;

			public LuckyDukeQueueResponseHandler(LuckyDuke parent) :
				base(parent.Player)
			{
				this.parent = parent;
			}

			public void AddHandler(Card causedBy, CheckDeckCallback checkCallback, CardResultCallback resultCallback)
			{
				AddHandler(new LuckyDukeResponseHandler(parent, causedBy, checkCallback, resultCallback));
			}
		}
		private LuckyDukeQueueResponseHandler queue;
		
		public LuckyDuke(Player player) :
			base(player, CharacterType.LuckyDuke)
		{
			queue = new LuckyDukeQueueResponseHandler(this);
		}
		
		public override void CheckDeck(Card causedBy, CheckDeckCallback checkCallback, CardResultCallback resultCallback)
		{
			queue.AddHandler(causedBy, checkCallback, resultCallback);
			if(!queue.Active)
				Game.GameCycle.PushTempHandler(queue);
		}
	}
}
