// Bin.cs
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
	public class Bin : Widget
	{
		public Widget ChildWidget
		{
			get { return Children.Count >= 1 ? Children[0] : null; }
			protected set
			{
				if(Children.Count >= 1)
					Children[0] = value;
				else
					Children.Add(value);
			}
		}

		protected Bin()
			: base(1)
		{
		}

		protected override void OnResized()
		{
			Widget child = ChildWidget;
			if(child != null)
				child.Reallocate(new Rectangle(0.0, 0.0, Allocation.Width, Allocation.Height));
		}

		public override void SizeRequest(ref double width, ref double height, out double ratio)
		{
			Widget child = ChildWidget;
			if(child != null)
				child.SizeRequest(ref width, ref height, out ratio);
			else base.SizeRequest(ref width, ref height, out ratio);
		}
	}
}

