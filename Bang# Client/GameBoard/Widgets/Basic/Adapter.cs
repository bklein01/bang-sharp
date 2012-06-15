// Adapter.cs
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

namespace BangSharp.Client.GameBoard.Widgets
{
	public class Adapter : Bin
	{
		private Direction dir;

		public Alignment Alignment
		{
			get;
			set;
		}

		public Adapter(Direction dir)
		{
			this.dir = dir;
		}

		protected override void OnResized()
		{
			Widget child = ChildWidget;
			if(child == null)
				return;

			double width = Allocation.Width;
			double height = Allocation.Height;

			double w = -1.0;
			double h = -1.0;
			double r;
			child.SizeRequest(ref w, ref h, out r);
			if(r > 0.0)
			{
				double rat = width / height;
				if(r > rat)
				{
					w = width;
					h = width / r;
				}
				else if(r < rat)
				{
					w = height * r;
					h = height;
				}
				else
				{
					w = width;
					h = height;
				}
			}
			else
			{
				switch(dir)
				{
				case Direction.Horizontal:
					w = width;
					h = -1.0;
					break;
				case Direction.Vertical:
					w = -1.0;
					h = height;
					break;
				default:
					throw new OverflowException();
				}
				child.SizeRequest(ref w, ref h, out r);
				if(w < 0.0)
					w = width;
				if(h < 0.0)
					h = height;
			}

			double x, y;
			switch(Alignment)
			{
			case Alignment.Left:
				x = 0.0;
				y = 0.0;
				break;
			case Alignment.Right:
				x = width;
				y = height;
				break;
			case Alignment.Center:
				x = (width - w) / 2;
				y = (height - h) / 2;
				break;
			default:
				throw new OverflowException();
			}
			child.Reallocate(new Cairo.Rectangle(x, y, w, h));
		}

		public override void SizeRequest(ref double width, ref double height, out double ratio)
		{
			width = -1;
			height = -1;
			ratio = -1;
		}
	}
}
