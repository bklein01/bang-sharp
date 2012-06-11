// CardAnimator.cs
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
using Cairo;

namespace BangSharp.Client.GameBoard.Animators
{
	public class CardAnimator<TWidget, TState> : IAnimator
		where TWidget : CardWidget
		where TState : CardState, new()
	{
		private Animation anim;
		private TWidget widget;
		private TState startState;
		private TState endState;

		public Animation Animation
		{
			get { return anim; }
		}

		public TWidget Widget
		{
			get { return widget; }
		}

		public TState StartState
		{
			get { return startState; }
		}
		public TState EndState
		{
			get { return endState; }
		}

		protected CardAnimator(Animation anim, TWidget widget)
		{
			this.anim = anim;
			this.widget = widget;
			startState = new TState();
			endState = new TState();
		}

		public TState GetState(StateType type)
		{
			switch(type)
			{
			case StateType.Start:
				return startState;
			case StateType.End:
				return endState;
			default:
				throw new OverflowException();
			}
		}

		public virtual void Animate(double progress)
		{
			progress = (Math.Sin((progress - 0.5) * Math.PI) + 1) / 2; // experimental - smooth movement
			Rectangle startAlloc = startState.Allocation;
			Rectangle endAlloc = endState.Allocation;
			Rectangle newAlloc = new Rectangle(startAlloc.X + progress * (endAlloc.X - startAlloc.X),
			                                   startAlloc.Y + progress * (endAlloc.Y - startAlloc.Y),
			                                   startAlloc.Width + progress * (endAlloc.Width - startAlloc.Width),
			                                   startAlloc.Height + progress * (endAlloc.Height - startAlloc.Height));
			widget.Reallocate(newAlloc);
		}
	}
}

