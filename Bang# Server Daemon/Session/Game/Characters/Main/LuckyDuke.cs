// LuckyDuke.cs
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
namespace Bang.Server.Characters
{
	public sealed class LuckyDuke : Character
	{
		private sealed class LuckyDukeResponseHandler : ResponseHandler
		{
			private LuckyDuke parent;
			private Card causedBy;
			private CheckDeckMethod checkMethod;
			private ICardResultHandler handler;
			
			public LuckyDukeResponseHandler (LuckyDuke parent, Card causedBy, CheckDeckMethod checkMethod, ICardResultHandler handler)
				: base(RequestType.LuckyDuke, parent.Player)
			{
				this.causedBy = causedBy;
				this.checkMethod = checkMethod;
				this.handler = handler;
			}
			
			protected override void OnStart ()
			{
				Game.GameTable.DrawIntoSelection (2, null);
			}
			
			protected override void OnRespondCard (Card card)
			{
				if (!Game.GameTable.Selection.Contains (card))
					throw new BadCardException ();
				
				bool result = checkMethod (card);
				Game.Session.EventManager.OnPlayerCheckedDeck (RequestedPlayer, card, causedBy, result);
				Game.GameTable.CancelSelection ();
				End();
				handler.OnResult (causedBy, result);
			}
		}
		
		public LuckyDuke (Player player)
			: base(player, CharacterType.LuckyDuke)
		{
		}
		
		public override void CheckDeck (Card causedBy, CheckDeckMethod checkMethod, ICardResultHandler handler)
		{
			Game.Session.EventManager.OnPlayerUsedAbility (Player);
			Game.GameCycle.PushTempHandler(new LuckyDukeResponseHandler(this, causedBy, checkMethod, handler));
		}
	}
}

