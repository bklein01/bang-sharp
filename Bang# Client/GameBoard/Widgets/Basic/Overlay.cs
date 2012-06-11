// Overlay.cs
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
using Cairo;

namespace BangSharp.Client.GameBoard.Widgets
{
	public class Overlay : Widget
	{
		public Overlay()
		{
		}

		protected override void OnResized()
		{
			foreach(Widget w in Children)
				w.Reallocate(new Rectangle(0.0, 0.0, Allocation.Width, Allocation.Height));
		}

		public override void SizeRequest(ref double width, ref double height, out double ratio)
		{
			if(Children.Count == 0)
			{
				base.SizeRequest(ref width, ref height, out ratio);
				return;
			}
			double maxWidth = -1.0;
			double maxHeight = -1.0;
			double rat = -1.0;
			foreach(Widget widget in Children)
			{
				double w = width;
				double h = height;
				double r;
				widget.SizeRequest(ref w, ref h, out r);
				if(r < 0.0)
				{
					if(w > maxWidth)
						maxWidth = w;
					if(h > maxHeight)
						maxHeight = h;
				}
				else
				{
					if(rat < 0.0)
						rat = r;
					else if(r != rat)
						rat = 0.0;
				}
			}
			width = maxWidth;
			height = maxHeight;
			ratio = -1.0;
			if(rat > 0.0 && maxWidth < 0.0 && maxHeight < 0.0)
				ratio = rat;
		}
	}
}

