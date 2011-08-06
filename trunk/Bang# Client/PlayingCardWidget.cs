// PlayingCardWidget.cs
//  
// Author:  WOnder93 <omosnacek@gmail.com>
// 
// Copyright (c) 2011 Ondrej Mosnáček
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
using System;
using Pango;
namespace Bang.Client
{
	public class PlayingCardWidget : CardWidget
	{
		private ICard card;
		private Layout layout;

		private static string GetRankSuitMarkup(ICard card)
		{
			string color = "black";
			char suit = '\0';
			string rank = "";
			switch(card.Suit)
			{
			case CardSuit.Spades:
				color = "black";
				suit = (char)0x2660;
				break;
			case CardSuit.Hearts:
				color = "red";
				suit = (char)0x2665;
				break;
			case CardSuit.Diamonds:
				color = "red";
				suit = (char)0x2666;
				break;
			case CardSuit.Clubs:
				color = "black";
				suit = (char)0x2663;
				break;
			}
			switch(card.Rank)
			{
			case CardRank.Two:
				rank = "2";
				break;
			case CardRank.Three:
				rank = "3";
				break;
			case CardRank.Four:
				rank = "4";
				break;
			case CardRank.Five:
				rank = "5";
				break;
			case CardRank.Six:
				rank = "6";
				break;
			case CardRank.Seven:
				rank = "7";
				break;
			case CardRank.Eight:
				rank = "8";
				break;
			case CardRank.Nine:
				rank = "9";
				break;
			case CardRank.Ten:
				rank = "10";
				break;
			case CardRank.Jack:
				rank = "J";
				break;
			case CardRank.Queen:
				rank = "Q";
				break;
			case CardRank.King:
				rank = "K";
				break;
			case CardRank.Ace:
				rank = "A";
				break;
			}
			return rank + "<span color=\"" + color + "\">" + suit + "</span>";
		}

		public PlayingCardWidget(ICard card)
			: base(ResourceManager.GetPixmap(card.Type))
		{
			this.card = card;
			layout = new Layout(PangoContext);
			layout.SetMarkup(GetRankSuitMarkup(card));
		}
		protected override bool OnButtonPressEvent(Gdk.EventButton ev)
		{
			// Insert button press handling code here.
			return base.OnButtonPressEvent(ev);
		}
		protected override bool OnExposeEvent(Gdk.EventExpose ev)
		{
			if(!base.OnExposeEvent(ev))
			{
				// Draw failsafe representation
				return true;
			}

			Cairo.Context cr = Gdk.CairoHelper.Create(ev.Window);
			CairoHelper.ShowLayout(cr, layout);
			cr.LineWidth = 1.0;
			cr.Color = new Cairo.Color(0, 0, 0);
			CairoHelper.LayoutPath(cr, layout);
			cr.Stroke();
			return true;
		}
		protected override void OnSizeAllocated(Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated(allocation);
			double font_size = 0.0;
		}
	}
}

