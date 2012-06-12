// Padding.cs
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
	public class Padding : Bin
	{
		private double left;
		private double right;
		private double top;
		private double bottom;
		public double LeftPadding
		{
			get { return left; }
			set
			{
				if(value < 0.0 || value > 1.0)
					throw new ArgumentOutOfRangeException("value");
				left = value;
			}
		}
		public double RightPadding
		{
			get { return right; }
			set
			{
				if(value < 0.0 || value > 1.0)
					throw new ArgumentOutOfRangeException("value");
				right = value;
			}
		}
		public double TopPadding
		{
			get { return top; }
			set
			{
				if(value < 0.0 || value > 1.0)
					throw new ArgumentOutOfRangeException("value");
				top = value;
			}
		}
		public double BottomPadding
		{
			get { return bottom; }
			set
			{
				if(value < 0.0 || value > 1.0)
					throw new ArgumentOutOfRangeException("value");
				bottom = value;
			}
		}
		public double HorizontalPadding
		{
			set
			{
				if(value < 0.0 || value > 1.0)
					throw new ArgumentOutOfRangeException("value");
				left = right = value;
			}
		}
		public double VerticalPadding
		{
			set
			{
				if(value < 0.0 || value > 1.0)
					throw new ArgumentOutOfRangeException("value");
				top = bottom = value;
			}
		}

		public Padding() :
			this(0.0, 0.0, 0.0, 0.0)
		{
		}
		public Padding(double h, double v) :
			this(h, h, v, v)
		{
		}
		public Padding(double left, double right, double top, double bottom)
		{
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}

		protected override void OnResized()
		{
			Widget child = ChildWidget;
			Rectangle alloc = Allocation;
			if(child != null)
				child.Reallocate(new Rectangle(alloc.Width * left, alloc.Height * top, alloc.Width * (1 - left - right), alloc.Height * (1 - top - bottom)));
		}
		public override void SizeRequest(ref double width, ref double height, out double ratio)
		{
			Widget child = ChildWidget;
			if(child != null)
			{
				if(width >= 0.0)
					width -= width * (left + right);
				if(height >= 0.0)
					height -= height * (top + bottom);
				child.SizeRequest(ref width, ref height, out ratio);
				if(width >= 0.0)
					width += width * (left + right);
				if(height >= 0.0)
					height += height * (top + bottom);
				if(ratio >= 0.0)
					ratio *= (1 + right + left) / (1 + top + bottom);
			}
			else
				base.SizeRequest(ref width, ref height, out ratio);
		}
	}
}

