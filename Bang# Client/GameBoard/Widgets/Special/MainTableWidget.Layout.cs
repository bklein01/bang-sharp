// MainTableWidget.Layout.cs
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

namespace BangSharp.Client.GameBoard.Widgets
{
	public partial class MainTableWidget
	{
		private Padding padding1;
		private Box box1;
		private Adapter adapter1;
		private Box box2;
		private Padding padding2;
		private CardPlaceholderWidget graveyardPlaceholder;
		private Padding padding3;
		private Overlay overlay1;
		private CardPlaceholderWidget deckPlaceholder;
		private Padding padding4;
		private SolidColor solidColor1;
		private GeneralPlaceholderWidget selectionPlaceholder;

		private void InitLayout()
		{
			this.padding1 = new Padding(0.0, 1.0 / 6.0);
			this.Children.Add(this.padding1);

			this.box1 = new Box(Direction.Vertical);
			this.padding1.Children.Add(this.box1);

			this.adapter1 = new Adapter(Direction.Horizontal);
			this.box1.Children.Add(this.adapter1);

			this.box2 = new Box(Direction.Horizontal);
			this.adapter1.Children.Add(this.box2);

			this.padding2 = new Padding(0.06, 0.04);
			this.box2.Children.Add(this.padding2);

			this.graveyardPlaceholder = new CardPlaceholderWidget();
			this.padding2.Children.Add(this.graveyardPlaceholder);

			this.padding3 = new Padding(0.06, 0.04);
			this.box2.Children.Add(this.padding3);

			this.overlay1 = new Overlay();
			this.padding3.Children.Add(this.overlay1);

			this.deckPlaceholder = new CardPlaceholderWidget();
			this.overlay1.Children.Add(this.deckPlaceholder);

			this.padding4 = new Padding(0.02, 0.04);
			this.box1.Children.Add(this.padding4);

			this.solidColor1 = new SolidColor();
			this.solidColor1.Color = new Cairo.Color(0.25, 0.25, 0.25, 0.3);
			this.padding4.Children.Add(this.solidColor1);

			this.selectionPlaceholder = new GeneralPlaceholderWidget();
			this.solidColor1.Children.Add(this.selectionPlaceholder);
		}
	}
}

