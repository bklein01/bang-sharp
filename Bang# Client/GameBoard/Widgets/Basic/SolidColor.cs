// SolidColor.cs
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
using System;

namespace BangSharp.Client.GameBoard.Widgets
{
	public class SolidColor : Bin
	{
		private double rL;
		private double rR;
		private double rT;
		private double rB;

		public Color Color
		{
			get;
			set;
		}

		public double RoundingLeft
		{
			get { return rL; }
			set { rL = Math.Max(0.0, value); }
		}
		public double RoundingRight
		{
			get { return rR; }
			set { rR = Math.Max(0.0, value); }
		}
		public double RoundingTop
		{
			get { return rT; }
			set { rT = Math.Max(0.0, value); }
		}
		public double RoundingBottom
		{
			get { return rB; }
			set { rB = Math.Max(0.0, value); }
		}

		public SolidColor()
		{
		}
		public SolidColor(double roundingHoriz, double roundingVert)
			: this(roundingHoriz, roundingHoriz, roundingVert, roundingVert)
		{
		}
		public SolidColor(double roudingLeft, double roundingRight, double roundingTop, double roundingBottom)
		{
			RoundingLeft = roudingLeft;
			RoundingRight = roundingRight;
			RoundingTop = roundingTop;
			RoundingBottom = roundingBottom;
		}

		protected override bool OnExposed(Context cr, Rectangle area)
		{
			cr.Color = Color;
			if(rL > 0.0 && rR > 0.0 && rT > 0.0 && rB > 0.0)
			{
				double width = Allocation.Width;
				double height = Allocation.Height;
				double radL = rL * width;
				double radR = rR * width;
				double radT = rT * height;
				double radB = rB * height;

				Matrix m = cr.Matrix;
				cr.Translate(radL, radT);
				cr.Scale(radL, radT);
				cr.Arc(0.0, 0.0, 1.0, 1.0 * Math.PI, 1.5 * Math.PI);

				cr.Matrix = m;
				cr.Translate(width - radR, radT);
				cr.Scale(radR, radT);
				cr.Arc(0.0, 0.0, 1.0, 1.5 * Math.PI, 2.0 * Math.PI);

				cr.Matrix = m;
				cr.Translate(width - radR, height - radB);
				cr.Scale(radR, radB);
				cr.Arc(0.0, 0.0, 1.0, 0.0 * Math.PI, 0.5 * Math.PI);

				cr.Matrix = m;
				cr.Translate(radL, height - radB);
				cr.Scale(radL, radB);
				cr.Arc(0.0, 0.0, 1.0, 0.5 * Math.PI, 1.0 * Math.PI);

				cr.ClosePath();
			}
			else
				cr.Rectangle(area);
			cr.Fill();
			return true;
		}
	}
}

