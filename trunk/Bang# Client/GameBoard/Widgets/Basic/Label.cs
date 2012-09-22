// Label.cs
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
using System;
using Cairo;

namespace BangSharp.Client.GameBoard.Widgets
{
	public class Label : Widget
	{
		private Pango.Layout layout;
		private string markup;
		private Alignment horizAlignment;
		private Alignment vertAlignment;

		public string Markup
		{
			get { return markup; }
			set { layout.SetMarkup(markup = value); }
		}

		public Pango.FontDescription Font
		{
			get { return layout.FontDescription; }
			set { layout.FontDescription = value; }
		}

		public Alignment HorizAlignment
		{
			get { return horizAlignment; }
			set
			{
				horizAlignment = value;
				layout.Alignment = (Pango.Alignment)((int)horizAlignment + 1);
			}
		}
		public Alignment VertAlignment
		{
			get { return vertAlignment; }
			set { vertAlignment = value; }
		}
		public bool Justify
		{
			get { return layout.Justify; }
			set { layout.Justify = value; }
		}

		public Pango.WrapMode WrapMode
		{
			get { return layout.Wrap; }
			set { layout.Wrap = value; }
		}
		public Pango.EllipsizeMode EllipsizeMode
		{
			get { return layout.Ellipsize; }
			set { layout.Ellipsize = value; }
		}

		public int Spacing
		{
			get { return layout.Spacing; }
			set { layout.Spacing = value; }
		}

		public double OutlineWidth
		{
			get;
			set;
		}
		public Color OutlineColor
		{
			get;
			set;
		}

		public Label() : base(0)
		{
			layout = new Pango.Layout(Gdk.PangoHelper.ContextGet());
			horizAlignment = Alignment.Left;
			vertAlignment = Alignment.Top;
			markup = "";
		}

		private void GetLayoutRectangle(out Pango.Rectangle pangoRect, out Rectangle cairoRect)
		{
			Pango.Rectangle ink;
			layout.GetExtents(out ink, out pangoRect);
			cairoRect = new Rectangle(pangoRect.X / Pango.Scale.PangoScale, pangoRect.Y / Pango.Scale.PangoScale,
			                          pangoRect.Width / Pango.Scale.PangoScale, pangoRect.Height / Pango.Scale.PangoScale);
		}

		protected override void OnResized()
		{
			layout.Width = (int)Math.Round(Allocation.Width * Pango.Scale.PangoScale);
		}
		protected override bool OnExposed(Context cr, Rectangle area)
		{
			Pango.CairoHelper.UpdateLayout(cr, layout);
			Pango.Rectangle log;
			Rectangle rect;
			GetLayoutRectangle(out log, out rect);
			double y = 0.0;
			switch(VertAlignment)
			{
			case Alignment.Top:
				y = 0.0;
				break;
			case Alignment.Center:
				y = (Allocation.Height - rect.Height) / 2;
				break;
			case Alignment.Bottom:
				y = Allocation.Height - rect.Height;
				break;
			}
			cr.Translate(0.0, y);
			Pango.CairoHelper.ShowLayout(cr, layout);
			if(OutlineWidth > 0.0)
			{
				cr.Color = OutlineColor;
				cr.LineWidth = OutlineWidth;
				cr.LineJoin = LineJoin.Round;
				Pango.CairoHelper.LayoutPath(cr, layout);
				cr.Stroke();
			}
			return true;
		}
		public override void SizeRequest(ref double width, ref double height, out double ratio)
		{
			int backup = layout.Width;
			Pango.Rectangle log;
			Rectangle rect;
			if(width < 0.0)
			{
				if(HorizAlignment != Alignment.Center)
				{
					layout.Width = -1;
					GetLayoutRectangle(out log, out rect);
					width = rect.Width;
					layout.Width = log.Width;
				}
			}
			else
				layout.Width = (int)Math.Round(width * Pango.Scale.PangoScale);

			GetLayoutRectangle(out log, out rect);
			if(height < 0.0/* || height < rect.Height*/)
			{
				if(VertAlignment != Alignment.Center)
					height = rect.Height;
			}
			layout.Width = backup;
			ratio = -1;
		}
	}
}

