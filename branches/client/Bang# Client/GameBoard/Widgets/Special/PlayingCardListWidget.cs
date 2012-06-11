// PlayingCardListWidget.cs
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
	public class PlayingCardListWidget : Widget
	{
		public PlayingCardListWidget()
		{
		}

		protected override void OnResized()
		{
			double width = Allocation.Width;
			double height = Allocation.Height;

			int count = Children.Count;
			if(count == 0)
				return;
			double cardWidth = height * Card.Ratio;
			double allCardsWidth = cardWidth * count;

			if(allCardsWidth < width || count == 1)
			{
				double startX = (width - allCardsWidth) / 2;
				for(int i = 0; i < count; i++)
					Children[i].Reallocate(new Rectangle(startX + i * cardWidth, 0, cardWidth, height));
			}
			else
			{
				double extra = width - cardWidth;
				if(extra < 0)
					extra = 0;
				double offset = extra / (count - 1);
				for(int i = 0; i < count; i++)
					Children[i].Reallocate(new Rectangle(i * offset, 0, cardWidth, height));
			}
		}
	}
}

