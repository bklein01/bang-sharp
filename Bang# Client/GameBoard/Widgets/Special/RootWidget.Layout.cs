// RootWidget.Layout.cs
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
	// UNDONE: [client] Finish the layout!
	public partial class RootWidget
	{
		private Overlay basicOverlay;
		private Box box1;
		private Box box2;
		private Box box3;
		private Box box4;
		private PlayerSlotWidget playerSlot3;
		private PlayerSlotWidget playerSlot4;
		private PlayerSlotWidget playerSlot5;
		private PlayerSlotWidget playerSlot2;
		private MainTableWidget mainTable;
		private PlayerSlotWidget playerSlot6;
		private PlayerSlotWidget playerSlot1;
		private PlayerSlotWidget playerSlot0;
		private PlayerSlotWidget playerSlot7;
		private Label requestLabel;
		private AnimationLayer animLayer;

		private void InitLayout()
		{
			this.basicOverlay = new Overlay();
			this.Children.Add(this.basicOverlay);

			this.box1 = new Box(Direction.Vertical);
			this.basicOverlay.Children.Add(this.box1);

			this.box2 = new Box(Direction.Horizontal);
			this.box1.Children.Add(this.box2);

			this.playerSlot3 = new PlayerSlotWidget();
			this.box2.Children.Add(this.playerSlot3);

			this.playerSlot4 = new PlayerSlotWidget();
			this.box2.Children.Add(this.playerSlot4);

			this.playerSlot5 = new PlayerSlotWidget();
			this.box2.Children.Add(this.playerSlot5);

			this.box3 = new Box(Direction.Horizontal);
			this.box1.Children.Add(this.box3);

			this.playerSlot2 = new PlayerSlotWidget();
			this.box3.Children.Add(this.playerSlot2);

			this.mainTable = new MainTableWidget();
			this.box3.Children.Add(this.mainTable);

			this.playerSlot6 = new PlayerSlotWidget();
			this.box3.Children.Add(this.playerSlot6);

			this.box4 = new Box(Direction.Horizontal);
			this.box1.Children.Add(this.box4);

			this.playerSlot1 = new PlayerSlotWidget();
			this.box4.Children.Add(this.playerSlot1);

			this.playerSlot0 = new PlayerSlotWidget(true);
			this.box4.Children.Add(this.playerSlot0);

			this.playerSlot7 = new PlayerSlotWidget();
			this.box4.Children.Add(this.playerSlot7);

			this.requestLabel = new Label();
			this.requestLabel.HorizAlignment = Alignment.Center;
			this.box1.Children.Add(this.requestLabel);

			this.animLayer = new AnimationLayer(this);
			this.basicOverlay.Children.Add(this.animLayer);
		}
	}
}

