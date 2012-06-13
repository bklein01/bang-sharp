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
using Cairo;

namespace BangSharp.Client.GameBoard.Widgets
{
	public class PlayingCardWidget : CardWidget
	{
		private Pango.Layout layout;
		private int id;
		private CardType type;
		private CardRank rank;
		private CardSuit suit;

		private void UpdateMarkup()
		{
			string color = "black";
			char suitText = ' ';
			string rankText = "";
			switch(suit)
			{
			case CardSuit.Spades:
				color = "black";
				suitText = '\x2660';
				break;
			case CardSuit.Clubs:
				color = "black";
				suitText = '\x2663';
				break;
			case CardSuit.Hearts:
				color = "red";
				suitText = '\x2665';
				break;
			case CardSuit.Diamonds:
				color = "red";
				suitText = '\x2666';
				break;
			}
			switch(rank)
			{
			case CardRank.Two:
				rankText = "2";
				break;
			case CardRank.Three:
				rankText = "3";
				break;
			case CardRank.Four:
				rankText = "4";
				break;
			case CardRank.Five:
				rankText = "5";
				break;
			case CardRank.Six:
				rankText = "6";
				break;
			case CardRank.Seven:
				rankText = "7";
				break;
			case CardRank.Eight:
				rankText = "8";
				break;
			case CardRank.Nine:
				rankText = "9";
				break;
			case CardRank.Ten:
				rankText = "10";
				break;
			case CardRank.Jack:
				rankText = "J";
				break;
			case CardRank.Queen:
				rankText = "Q";
				break;
			case CardRank.King:
				rankText = "K";
				break;
			case CardRank.Ace:
				rankText = "A";
				break;
			}
			layout.SetMarkup("<span color='black'>" + rankText + "</span><span color='" + color + "'>" + suitText + "</span>");
		}

		public int ID
		{
			get { return id; }
			set { id = value; }
		}

		public CardType Type
		{
			get { return type; }
			set { Card = CardManager.GetCard(type = value); }
		}

		public CardRank Rank
		{
			get { return rank; }
			set
			{
				rank = value;
				UpdateMarkup();
			}
		}

		public CardSuit Suit
		{
			get { return suit; }
			set
			{
				suit = value;
				UpdateMarkup();
			}
		}

		public PlayingCardWidget() :
			this(0, CardType.Unknown, CardRank.Unknown, CardSuit.Unknown)
		{
		}

		public PlayingCardWidget(int id) :
			this(id, CardType.Unknown, CardRank.Unknown, CardSuit.Unknown)
		{
		}

		public PlayingCardWidget(ICard card) :
			this(card.ID, card.Type, card.Rank, card.Suit)
		{
		}

		public PlayingCardWidget(int id, CardType type, CardRank rank, CardSuit suit)
		{
			layout = new Pango.Layout(Gdk.PangoHelper.ContextGet());
			layout.FontDescription = Pango.FontDescription.FromString("Angleterre Book, Librarian bold 32");
			this.id = id;
			this.type = type;
			this.rank = rank;
			this.suit = suit;
			Card = CardManager.GetCard(type);
			UpdateMarkup();
		}

		public void Update(ICard card)
		{
			id = card.ID;
			type = card.Type;
			rank = card.Rank;
			suit = card.Suit;
			Card = CardManager.GetCard(type);
			UpdateMarkup();
		}

		private const double RankOffsetX = 30.0;
		private const double RankOffsetY = 555.0;

		protected override bool OnExposed(Context cr, Rectangle area)
		{
			cr.Save();
			if(!base.OnExposed(cr, area))
			{
				cr.Color = new Color(1.0, 0.0, 0.0);
				cr.SetFontSize(100.0);
				cr.SelectFontFace("Librarian", FontSlant.Normal, FontWeight.Bold);
				cr.ShowText(type.ToString());
				cr.Restore();
				return true;
			}
			cr.Restore();

			if(type != CardType.Unknown)
			{
				ApplyConstriction(cr);
				double ratio = Allocation.Height / Card.DefaultHeight;
				cr.Scale(ratio, ratio);
				cr.Translate(RankOffsetX, RankOffsetY);
				Pango.CairoHelper.ShowLayout(cr, layout);
				cr.LineWidth = 2.0;
				cr.Color = new Color(1, 1, 1);
				cr.LineCap = LineCap.Butt;
				cr.LineJoin = LineJoin.Round;
				Pango.CairoHelper.LayoutPath(cr, layout);
				cr.Stroke();
			}
			return true;
		}
	}
}

