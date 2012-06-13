// CharacterCardWidget.cs
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
	public class CharacterCardWidget : CardWidget
	{
		private CharacterType type;

		public CharacterType Type
		{
			get { return type; }
			set
			{
				type = value;
				Card = CardManager.GetCard(type);
			}
		}

		public CharacterCardWidget(CharacterType type = CharacterType.Unknown)
		{
			Type = type;
		}

		protected override bool OnExposed(Context cr, Rectangle area)
		{
			cr.Save();
			if(!base.OnExposed(cr, area))
			{
				cr.Color = new Color(1.0, 0.0, 1.0);
				cr.Rectangle(new Rectangle(2.0, 2.0, Allocation.Width - 4.0, Allocation.Height - 4.0));
				cr.LineWidth = 4.0;
				cr.LineJoin = LineJoin.Round;
				cr.Stroke();
				cr.SetFontSize(20.0);
				cr.SelectFontFace("Librarian", FontSlant.Normal, FontWeight.Bold);
				cr.Translate(0.0, Allocation.Height / 2);
				cr.ShowText(type.ToString());
				cr.Restore();
				return true;
			}
			cr.Restore();
			return true;
		}
	}
}

