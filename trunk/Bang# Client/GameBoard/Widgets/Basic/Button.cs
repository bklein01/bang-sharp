// Button.cs
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
	public class Button : Widget
	{
		private const double EdgeSize = 5.0;

		public event Action OnClick;

		public Button() : base(0)
		{
		}

		protected override bool OnExposed(Cairo.Context cr, Cairo.Rectangle area)
		{
			Rectangle rectT = new Rectangle(0.0, 0.0, Allocation.Width, EdgeSize);
			Rectangle rectB = new Rectangle(0.0, Allocation.Height - EdgeSize, Allocation.Width, EdgeSize);
			Rectangle rectL = new Rectangle(0.0, 0.0, EdgeSize, Allocation.Height);
			Rectangle rectR = new Rectangle(Allocation.Width - EdgeSize, 0.0, EdgeSize, Allocation.Height);

			Rectangle areaT = Widget.Intersect(area, rectT);
			Rectangle areaB = Widget.Intersect(area, rectB);
			Rectangle areaL = Widget.Intersect(area, rectL);
			Rectangle areaR = Widget.Intersect(area, rectR);

			if(areaT.Width != 0.0 && areaT.Height != 0.0)
			{
				LinearGradient grT = new LinearGradient(0.0, 0.0, 0.0, EdgeSize);
				grT.AddColorStop(0.0, new Color(1.0, 1.0, 1.0, 1.0));
				grT.AddColorStop(1.0, new Color(1.0, 1.0, 1.0, 0.0));
				cr.Save();
				cr.Pattern = grT;
				//cr.Rectangle(areaT);
				cr.NewPath();
				cr.MoveTo(0.0, 0.0);
				cr.LineTo(Allocation.Width, 0.0);
				cr.LineTo(Allocation.Width - EdgeSize, EdgeSize);
				cr.LineTo(EdgeSize, EdgeSize);
				cr.ClosePath();
				cr.Clip();
				cr.Paint();
				cr.Restore();
			}

			if(areaB.Width != 0.0 && areaB.Height != 0.0)
			{
				LinearGradient grB = new LinearGradient(0.0, Allocation.Height, 0.0, Allocation.Height - EdgeSize);
				grB.AddColorStop(0.0, new Color(0.0, 0.0, 0.0, 1.0));
				grB.AddColorStop(1.0, new Color(0.0, 0.0, 0.0, 0.0));
				cr.Save();
				cr.Pattern = grB;
				//cr.Rectangle(areaB);
				cr.MoveTo(0.0, Allocation.Height);
				cr.LineTo(Allocation.Width, Allocation.Height);
				cr.LineTo(Allocation.Width - EdgeSize, Allocation.Height - EdgeSize);
				cr.LineTo(EdgeSize, Allocation.Height - EdgeSize);
				cr.ClosePath();
				cr.Clip();
				cr.Paint();
				cr.Restore();
			}

			if(areaL.Width != 0.0 && areaL.Height != 0.0)
			{
				LinearGradient grL = new LinearGradient(0.0, 0.0, EdgeSize, 0.0);
				grL.AddColorStop(0.0, new Color(1.0, 1.0, 1.0, 1.0));
				grL.AddColorStop(1.0, new Color(1.0, 1.0, 1.0, 0.0));
				cr.Save();
				cr.Pattern = grL;
				//cr.Rectangle(areaL);
				cr.MoveTo(0.0, 0.0);
				cr.LineTo(0.0, Allocation.Height);
				cr.LineTo(EdgeSize, Allocation.Height - EdgeSize);
				cr.LineTo(EdgeSize, EdgeSize);
				cr.ClosePath();
				cr.Clip();
				cr.Paint();
				cr.Restore();
			}

			if(areaR.Width != 0.0 && areaR.Height != 0.0)
			{
				LinearGradient grR = new LinearGradient(Allocation.Width, 0.0, Allocation.Width - EdgeSize, 0.0);
				grR.AddColorStop(0.0, new Color(0.0, 0.0, 0.0, 1.0));
				grR.AddColorStop(1.0, new Color(0.0, 0.0, 0.0, 0.0));
				cr.Save();
				cr.Pattern = grR;
				//cr.Rectangle(areaR);
				cr.MoveTo(Allocation.Width, 0.0);
				cr.LineTo(Allocation.Width, Allocation.Height);
				cr.LineTo(Allocation.Width - EdgeSize, Allocation.Height - EdgeSize);
				cr.LineTo(Allocation.Width - EdgeSize, EdgeSize);
				cr.ClosePath();
				cr.Clip();
				cr.Paint();
				cr.Restore();
			}
			return true;
		}

		protected override bool OnLeftClick(double x, double y)
		{
			if(OnClick != null)
				OnClick();
			return true;
		}
	}
}

