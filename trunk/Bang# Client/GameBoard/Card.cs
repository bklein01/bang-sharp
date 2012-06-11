// Card.cs
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
using Gdk;

namespace BangSharp.Client.GameBoard
{
	public class Card : IDisposable
	{
		public const int DefaultWidth = 400;
		public const int DefaultHeight = 620;
		public const double Ratio = (double)DefaultWidth / DefaultHeight;
		private Pixbuf original;
		private Pixbuf resized;

		public Card(Pixbuf pixbuf)
		{
			if((double)pixbuf.Width / pixbuf.Height != Ratio)
				throw new InvalidOperationException();
			original = pixbuf;
		}

		public Pixbuf GetPixbuf(int height)
		{
			if(original == null)
				throw new ObjectDisposedException("Card");
			if(original.Height == height)
				return original;
			if(resized != null && resized.Height == height)
				return resized;
			if(resized != null)
				resized.Dispose();
			return resized = original.ScaleSimple((int)(height * Ratio), height, InterpType.Bilinear);
		}
	
		public void Dispose()
		{
			original.Dispose();
			if(resized != null)
				resized.Dispose();
			original = null;
			resized = null;
		}
	}
}

