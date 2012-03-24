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
using Gdk;
namespace BangSharp.Client
{
	public class CardWidget : Gtk.DrawingArea
	{
		private Pixbuf original;
		private Pixbuf resized;

		protected CardWidget(Pixbuf pixbuf)
		{
			original = pixbuf;
			resized = null;
		}
		protected override bool OnExposeEvent(Gdk.EventExpose ev)
		{
			base.OnExposeEvent(ev);
			Rectangle area = ev.Area;
			if(resized != null)
				ev.Window.DrawPixbuf(Style.BaseGC(Gtk.StateType.Normal), resized, area.X, area.Y, area.X, area.Y, area.Width, area.Height, RgbDither.None, 0, 0);
			else if(original != null)
				ev.Window.DrawPixbuf(Style.BaseGC(Gtk.StateType.Normal), original, area.X, area.Y, area.X, area.Y, area.Width, area.Height, RgbDither.None, 0, 0);
			else return false;
			return true;
		}
		protected override void OnSizeAllocated(Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated(allocation);
			if(allocation.Width == original.Width && allocation.Height == original.Height)
				resized = null;
			else
			{
				resized.Dispose();
				resized = original.ScaleSimple(allocation.Width, allocation.Height, InterpType.Bilinear);
			}
		}
		protected override void OnSizeRequested(ref Gtk.Requisition requisition)
		{
			requisition.Height = 400;
			requisition.Width = 620;
		}
	}
}

