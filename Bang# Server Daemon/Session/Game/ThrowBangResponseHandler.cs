// ThrowBangResponseHandler.cs
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
namespace BangSharp.Server.Daemon
{
	public sealed class ThrowBangResponseHandler : ResponseHandler
	{
		ResultCallback callback;

		public ThrowBangResponseHandler(Player requested, Player causedBy, ResultCallback callback = null) :
			base(RequestType.ThrowBang, requested, causedBy)
		{
			this.callback = callback;
		}

		protected override void OnRespondCard(Card card)
		{
			if(card.Owner != RequestedPlayer)
				throw new BadCardException();
			
			if(!RequestedPlayer.IsBang(card))
				throw new BadCardException();

			if(card.Type != CardType.Bang)
				Game.GameTable.PlayerRespondWithCard(card, CardType.Bang);
			else
				Game.GameTable.PlayerRespondWithCard(card);

			if(callback != null)
				callback(true);
			End();
		}
		protected override void OnRespondNoAction()
		{
			RequestedPlayer.ModifyLifePoints(-1, CausedBy);
			if(callback != null)
				callback(false);
			End();
		}
	}
}
