// JesseJones.cs
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
	public sealed class JesseJones : Character
	{
		private class JesseJonesResponseHandler : ResponseHandler
		{
			private JesseJones parent;

			public JesseJonesResponseHandler (JesseJones parent)
				: base(RequestType.StealCard, parent.Player)
			{
				this.parent = parent;
			}

			protected override void OnRespondCard(Card targetCard)
			{
				Player targetPlayer = targetCard.Owner;
				if(targetPlayer == RequestedPlayer)
					throw new BadTargetPlayerException();
				
				if(!targetPlayer.IsAlive)
					throw new BadTargetPlayerException();
				
				Game.Session.EventManager.OnPlayerUsedAbility(RequestedPlayer);
				Game.GameTable.PlayerStealCard(RequestedPlayer, targetCard);
				Game.GameTable.PlayerDrawFromDeck(RequestedPlayer, 1);
				End ();
			}
			protected override void OnRespondNoAction ()
			{
				End ();
			}
		}
		public JesseJones (Player player)
			: base(player, CharacterType.JesseJones)
		{
		}
		
		public override void Draw ()
		{
			Game.GameCycle.PushTempHandler(new JesseJonesResponseHandler(this));
		}
	}
}

