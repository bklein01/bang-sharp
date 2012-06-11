// Box.cs
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
	public class Box : Widget
	{
		private Direction dir;

		public Alignment Alignment
		{
			get;
			set;
		}

		public Box(Direction dir)
		{
			this.dir = dir;
		}

		protected override void OnResized()
		{
			int count = Children.Count;
			double[] ws = new double[count];
			double[] hs = new double[count];
			double[] vs = dir == Direction.Horizontal ? ws : hs;
			double width = Allocation.Width;
			double height = Allocation.Height;
			double variable = dir == Direction.Horizontal ? width : height;
			double rat = 0.0;
			double abs = -1.0;
			int autoV = 0;
			int autoNv = 0;
			double maxNv = -1.0;
			for(int i = 0; i < count; i++)
			{
				Widget child = Children[i];
				double w = dir == Direction.Horizontal ? -1 : width;
				double h = dir == Direction.Vertical ? -1 : height;
				double r;
				child.SizeRequest(ref w, ref h, out r);
				ws[i] = w;
				hs[i] = h;
				double v = dir == Direction.Horizontal ? w : h;
				double nv = dir == Direction.Vertical ? w : h;
				if(r < 0.0)
				{
					if(v < 0.0)
						autoV++;
					else
					{
						if(abs < 0.0)
							abs = 0.0;
						abs += v;
					}

					if(nv < 0.0)
						autoNv++;
					else if(nv > maxNv)
						maxNv = nv;
				}
				else
				{
					if(dir == Direction.Vertical)
						r = 1.0 / r;
					rat += r;
				}
			}
			if(rat != 0.0)
				throw new Exception();
			if(autoV != 0)
			{
				double vForAuto = Math.Max(0.0, variable - abs) / autoV;
				double offset = 0.0;
				for(int i = 0; i < count; i++)
				{
					Widget child = Children[i];
					double xOff = dir == Direction.Horizontal ? offset : 0.0;
					double yOff = dir == Direction.Horizontal ? 0.0 : offset;
					double v = vs[i];
					if(v < 0.0)
						v = vForAuto;
					double w = dir == Direction.Horizontal ? v : width;
					double h = dir == Direction.Horizontal ? height : v;
					child.Reallocate(new Rectangle(xOff, yOff, w, h));
					offset += v;
				}
			}
			else
			{
				double offset = 0.0;
				if(Alignment == Alignment.Center)
					offset = variable / 2 - abs / 2;
				else if(Alignment == Alignment.Right)
					offset = variable - abs;
				for(int i = 0; i < count; i++)
				{
					Widget child = Children[i];
					double xOff = dir == Direction.Horizontal ? offset : 0.0;
					double yOff = dir == Direction.Horizontal ? 0.0 : offset;
					double v = vs[i];
					double w = dir == Direction.Horizontal ? v : width;
					double h = dir == Direction.Horizontal ? height : v;
					child.Reallocate(new Rectangle(xOff, yOff, w, h));
					offset += v;
				}
			}
		}

		public override void SizeRequest(ref double width, ref double height, out double ratio)
		{
			ratio = -1;
			if(width >= 0.0 && height >= 0.0)
				return;

			int count = Children.Count;
			double rat = 0.0;
			double abs = -1.0;
			int autoV = 0;
			int autoNv = 0;
			double maxNv = -1.0;
			for(int i = 0; i < count; i++)
			{
				Widget child = Children[i];
				double w = dir == Direction.Horizontal ? -1 : width;
				double h = dir == Direction.Vertical ? -1 : height;
				double r;
				child.SizeRequest(ref w, ref h, out r);
				double v = dir == Direction.Horizontal ? w : h;
				double nv = dir == Direction.Vertical ? w : h;
				if(r < 0.0)
				{
					if(v < 0.0)
						autoV++;
					else
					{
						if(abs < 0.0)
							abs = 0.0;
						abs += v;
					}

					if(nv < 0.0)
						autoNv++;
					else if(nv > maxNv)
						maxNv = nv;
				}
				else
				{
					if(dir == Direction.Vertical)
						r = 1.0 / r;
					rat += r;
				}
			}

			double variable = dir == Direction.Horizontal ? width : height;
			double nonVariable = dir == Direction.Vertical ? width : height;

			if(variable < 0.0 && nonVariable < 0.0)
			{
				if(autoV == 0 && rat == 0.0 && abs >= 0.0)
					variable = abs;
				if(autoNv == 0 && maxNv >= 0.0)
					nonVariable = maxNv;
				if(variable < 0.0 && nonVariable >= 0.0 && autoV == 0 && rat != 0.0)
					variable = nonVariable * rat;
				if(nonVariable < 0.0 && variable >= 0.0 && autoNv == 0 && rat != 0.0)
					nonVariable = variable / rat;
				if(variable < 0.0 && nonVariable < 0.0 && autoV == 0 && autoNv == 0 && rat != 0)
					ratio = dir == Direction.Horizontal ? rat : 1 / rat;
			}
			else if(variable < 0.0)
			{
				if(abs < 0.0)
					throw new Exception();

				if(autoV == 0)
					variable = abs;
			}
			else if(nonVariable < 0.0)
			{
				if(autoNv == 0 && maxNv >= 0.0)
					nonVariable = maxNv;
				if(nonVariable < 0.0 && autoNv == 0 && rat != 0.0)
					nonVariable = variable / rat;
			}
			width = dir == Direction.Horizontal ? variable : nonVariable;
			height = dir == Direction.Vertical ? variable : nonVariable;
		}
	}
}

