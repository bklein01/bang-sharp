// CardWidget.cs
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
using Cairo;

namespace BangSharp.Client.GameBoard.Widgets
{
	public delegate void CardClick(CardWidget cardWidget);

	public class CardWidget : Widget
	{
		protected Card Card
		{
			get;
			set;
		}

		public double Constriction
		{
			get;
			set;
		}

		public event CardClick OnLClick;
		public event CardClick OnRClick;

		protected CardWidget() : base(0)
		{
			Constriction = 1.0;
		}

		protected void ApplyConstriction(Context cr)
		{
			if(Constriction == 0.0)
			{
				cr.Rectangle(new Rectangle(0.0, 0.0, 0.0, 0.0));
				cr.Clip();
			}
			else
			{
				cr.Translate(Allocation.Width / 2, 0.0);
				cr.Scale(Constriction, 1.0);
				cr.Translate(-Allocation.Width / 2, 0.0);
			}
		}

		protected override bool OnLeftClick(double x, double y)
		{
			if(OnLClick != null)
				OnLClick(this);
			return true;
		}
		protected override bool OnRightClick(double x, double y)
		{
			if(OnRClick != null)
				OnRClick(this);
			return true;
		}

		protected override bool OnExposed(Context cr, Rectangle area)
		{
			if(Card == null)
				return false;
			//int pixbufH = (int)Math.Round(Allocation.Height * 1.2);
			int pixbufH = (int)(Math.Pow(1.5, Math.Ceiling(Math.Log(Allocation.Height / Card.DefaultHeight, 1.5))) * Card.DefaultHeight);
			if(pixbufH == 0)
				return true;
			Gdk.Pixbuf pixbuf = Card.GetPixbuf(pixbufH);
			if(pixbuf == null)
				return false;

			ApplyConstriction(cr);
			double ratio = Allocation.Height / pixbufH;
			cr.Scale(ratio, ratio);
			Gdk.CairoHelper.SetSourcePixbuf(cr, pixbuf, 0, 0);
			cr.Paint();
			return true;
		}

		public override void SizeRequest(ref double width, ref double height, out double ratio)
		{
			if(width < 0.0 && height < 0.0)
			{
				ratio = Card.Ratio;
				return;
			}
			ratio = -1;
			if(width < 0.0)
				width = height * Card.Ratio;
			if(height < 0.0)
				height = width / Card.Ratio;
		}
	}
}

