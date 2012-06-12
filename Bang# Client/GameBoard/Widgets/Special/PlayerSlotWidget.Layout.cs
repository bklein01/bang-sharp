// PlayerSlotWidget.Layout.cs
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
	public partial class PlayerSlotWidget
	{
		private Padding padding1;
		private SolidColor color1;
		private Box box1;
		private Box box2;
		private Adapter adapter1;
		private Box box3;
		private Padding padding4;
		private Picture playerPic;
		private Box box5;
		private Label playerLabel;
		private Adapter adapter2;
		private Box box6;
		private Padding padding8;
		private Picture onlinePic;
		private Padding padding9;
		private SolidColor color2;
		private Padding padding2;
		private GeneralPlaceholderWidget playerHandPlaceholder;
		private SolidColor color3;
		private Padding padding3;
		private GeneralPlaceholderWidget playerTablePlaceholder;
		private Box box4;
		private Padding padding5;
		private CardPlaceholderWidget playerRolePlaceholder;
		private Padding padding6;
		private Overlay characterOverlay;
		private CardPlaceholderWidget characterPlaceholder;
		private Padding padding7;
		private CardPlaceholderWidget cardPlaceholder1;

		private void InitLayout()
		{
			this.padding1 = new Padding(0.02, 0.02);
			this.Children.Add(this.padding1);

			this.color1 = new SolidColor();
			this.color1.Color = new Cairo.Color(0.25, 0.25, 0.25, 0.3);
			this.padding1.Children.Add(this.color1);

			this.box1 = new Box(Direction.Horizontal);
			this.color1.Children.Add(this.box1);

			this.box2 = new Box(Direction.Vertical);
			this.box1.Children.Add(this.box2);

			this.adapter1 = new Adapter(Direction.Vertical);
			this.adapter1.Alignment = Alignment.Left;
			this.box2.Children.Add(this.adapter1);

			this.box3 = new Box(Direction.Horizontal);
			this.box3.Alignment = Alignment.Left;
			this.adapter1.Children.Add(this.box3);

			this.padding4 = new Padding(0.02, 0.02);
			this.box3.Children.Add(this.padding4);

			this.playerPic = new Picture();
			this.padding4.Children.Add(this.playerPic);

			this.box5 = new Box(Direction.Vertical);
			this.box3.Children.Add(this.box5);

			this.playerLabel = new Label();
			this.playerLabel.Font = Pango.FontDescription.FromString("Librarian 16");
			this.box5.Children.Add(this.playerLabel);

			this.adapter2 = new Adapter(Direction.Vertical);
			this.adapter2.Alignment = Alignment.Left;
			this.box5.Children.Add(this.adapter2);

			this.box6 = new Box(Direction.Horizontal);
			this.adapter2.Children.Add(this.box6);

			this.padding8 = new Padding(0.04, 0.04);
			this.box6.Children.Add(this.padding8);

			this.onlinePic = new Picture();
			this.padding8.Children.Add(this.onlinePic);

			this.padding9 = new Padding(0.04, 0.04);
			this.box6.Children.Add(this.padding9);

			this.padding2 = new Padding(0.02, 0.04);
			this.box2.Children.Add(this.padding2);

			this.color2 = new SolidColor();
			this.color2.Color = new Cairo.Color(0.25, 0.25, 0.25, 0.3);
			this.padding2.Children.Add(this.color2);

			this.playerHandPlaceholder = new GeneralPlaceholderWidget();
			this.color2.Children.Add(this.playerHandPlaceholder);

			this.padding3 = new Padding(0.02, 0.04);
			this.box2.Children.Add(this.padding3);

			this.color3 = new SolidColor();
			this.color3.Color = new Cairo.Color(0.25, 0.25, 0.25, 0.3);
			this.padding3.Children.Add(this.color3);

			this.playerTablePlaceholder = new GeneralPlaceholderWidget();
			this.color3.Children.Add(this.playerTablePlaceholder);

			this.box4 = new Box(Direction.Vertical);
			this.box1.Children.Add(this.box4);

			this.padding5 = new Padding(0.06, 0.04);
			this.box4.Children.Add(this.padding5);

			this.playerRolePlaceholder = new CardPlaceholderWidget();
			this.padding5.Children.Add(this.playerRolePlaceholder);

			this.padding6 = new Padding(0.06, 0.04);
			this.box4.Children.Add(this.padding6);

			this.characterOverlay = new Overlay();
			this.padding6.Children.Add(this.characterOverlay);

			this.characterPlaceholder = new CardPlaceholderWidget();
			this.characterOverlay.Children.Add(this.characterPlaceholder);

			this.padding7 = new Padding(0.06, 0.04);
			this.box4.Children.Add(this.padding7);

			this.cardPlaceholder1 = new CardPlaceholderWidget();
			this.padding7.Children.Add(this.cardPlaceholder1);
		}
	}
}

