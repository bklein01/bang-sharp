// JoseDelgado.cs
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
	public sealed class JoseDelgado : Character
	{
		private sealed class JoseDelgadoResponseHandler : ResponseHandler
		{
			private JoseDelgado parent;
			
			public JoseDelgadoResponseHandler (JoseDelgado parent)
				: base(RequestType.JoseDelgado, parent.Player)
			{
				this.parent = parent;
			}
			
			protected override void OnRespondCard (Card card)
			{
				if (card.Owner != RequestedPlayer)
					throw new BadCardException ();
				
				card.AssertInHand ();
				
				if (card.Color != CardColor.Blue)
					throw new BadCardException ();
				
				parent.abilityUses++;
				Game.Session.EventManager.OnPlayerUsedAbility (RequestedPlayer);
				Game.GameTable.CancelCard (card);
				Game.GameTable.PlayerDrawFromDeck (RequestedPlayer, 2);
				End();
			}
			protected override void OnRespondNoAction ()
			{
				End();
			}
		}
		private int abilityUses;
		
		public JoseDelgado (Player player)
			: base(player, CharacterType.JoseDelgado)
		{
		}
		
		public override void UseAbility ()
		{
			if (abilityUses >= 2)
				throw new BadUsageException ();
			Game.GameCycle.PushTempHandler(new JoseDelgadoResponseHandler(this));
		}
		
		public override void OnTurnEnded ()
		{
			abilityUses = 0;
		}
	}
}

