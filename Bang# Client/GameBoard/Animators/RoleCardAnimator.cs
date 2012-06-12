// RoleCardAnimator.cs
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
using BangSharp.Client.GameBoard.States;
using BangSharp.Client.GameBoard.Widgets;

namespace BangSharp.Client.GameBoard.Animators
{
	public class RoleCardAnimator : CardAnimator<RoleCardWidget, RoleCardState>
	{
		public RoleCardAnimator(Animation anim, RoleCardWidget widget) :
			base(anim, widget)
		{
		}

		public override void Animate(double progress)
		{
			base.Animate(progress);
			if(StartState.Role != EndState.Role)
				if(progress <= 0.5)
					Widget.Constriction = Math.Cos(progress * Math.PI);
				else
					Widget.Constriction = Math.Sin((progress - 0.5) * Math.PI);
			else
				Widget.Constriction = 1.0;

			if(progress <= 0.5)
				Widget.Role = StartState.Role;
			else
				Widget.Role = EndState.Role;
		}
	}
}

