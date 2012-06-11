// GameBoard.cs
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
//#define BOX_TEST
using BangSharp.Client.GameBoard.Widgets;
using Cairo;

namespace BangSharp.Client.GameBoard
{
	[System.ComponentModel.ToolboxItem(true)]
	public class GameBoardWidget : Gtk.DrawingArea
	{
		private RootWidget root;
		private Gdk.Pixbuf bgTile;

		public GameBoardWidget()
		{
			root = new RootWidget(this);
			bgTile = ResourceManager.GetPixbuf("Resources", "Board.png");
			this.Events |= Gdk.EventMask.ButtonPressMask;
		}

		protected override bool OnButtonPressEvent(Gdk.EventButton ev)
		{
			if(ev.Button == 1)
				root.RootLeftClick(ev.X, ev.Y);
			else if(ev.Button == 3)
				root.RootRightClick(ev.X, ev.Y);
			else
				return false;
			return true;
		}

		protected override bool OnExposeEvent(Gdk.EventExpose ev)
		{
			base.OnExposeEvent(ev);
			using(Cairo.Context cr = Gdk.CairoHelper.Create(ev.Window))
			{
				if(bgTile == null)
				{
					cr.Rectangle(ev.Area.X, ev.Area.Y, ev.Area.Width, ev.Area.Height);
					cr.Color = new Cairo.Color(2, 114, 43);
					cr.Fill();
				}
				else
				{
					int pw = bgTile.Width;
					int ph = bgTile.Height;

					int xStart = ev.Area.Left / pw;
					int xEnd = ev.Area.Right / pw + (ev.Area.Right % pw != 0 ? 1 : 0);
					int yStart = ev.Area.Top / ph;
					int yEnd = ev.Area.Bottom / ph + (ev.Area.Bottom % ph != 0 ? 1 : 0);

					for(int x = xStart; x < xEnd; x++)
						for(int y = yStart; y < yEnd; y++)
							ev.Window.DrawPixbuf(Style.BaseGC(Gtk.StateType.Normal), bgTile, 0, 0, x * pw, y * ph, pw, ph, Gdk.RgbDither.Normal, 0, 0);
				}
				root.RootExpose(cr, new Rectangle(ev.Area.X, ev.Area.Y, ev.Area.Width, ev.Area.Height));
			}
			return true;
		}

		protected override void OnSizeAllocated(Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated(allocation);
			root.RootReallocate(new Rectangle(allocation.X, allocation.Y, allocation.Width, allocation.Height));
		}
	}
}

