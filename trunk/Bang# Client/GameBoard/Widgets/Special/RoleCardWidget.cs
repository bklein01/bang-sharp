// RoleCardWidget.cs
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
	public class RoleCardWidget : CardWidget
	{
		private Role role;

		public Role Role
		{
			get { return role; }
			set
			{
				role = value;
				Card = CardManager.GetCard(role);
			}
		}

		public RoleCardWidget() : this(Role.Unknown)
		{
		}
		public RoleCardWidget(Role role)
		{
			Role = role;
		}

		public void Update(RoleCardWidget other)
		{
			role = other.role;
			Card = other.Card;
		}

		protected override bool OnExposed(Context cr, Rectangle area)
		{
			cr.Save();
			if(!base.OnExposed(cr, area))
			{
				cr.Color = new Color(1.0, 0.0, 0.0);
				cr.SetFontSize(100.0);
				cr.SelectFontFace("Librarian", FontSlant.Normal, FontWeight.Bold);
				cr.ShowText(role.ToString());
				cr.Restore();
				return true;
			}
			cr.Restore();
			return true;
		}
	}
}

