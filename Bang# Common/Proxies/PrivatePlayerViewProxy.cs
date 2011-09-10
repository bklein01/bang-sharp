// PrivatePlayerViewProxy.cs
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
using System.Collections.ObjectModel;
namespace Bang
{
	public class PrivatePlayerViewProxy : MarshalByRefObject, IPrivatePlayerView
	{
		private IPrivatePlayerView raw;

		int IIdentificable.ID
		{
			get { return raw.ID; }
		}
		bool IPublicPlayerView.IsSheriff
		{
			get { return raw.IsSheriff; }
		}
		bool IPublicPlayerView.IsAlive
		{
			get { return raw.IsAlive; }
		}
		bool IPublicPlayerView.IsWinner
		{
			get { return raw.IsWinner; }
		}
		int IPublicPlayerView.LifePoints
		{
			get { return raw.LifePoints; }
		}
		int IPublicPlayerView.MaxLifePoints
		{
			get { return raw.MaxLifePoints; }
		}
		ReadOnlyCollection<ICard> IPublicPlayerView.Table
		{
			get { return raw.Table; }
		}
		CharacterType IPublicPlayerView.CharacterType
		{
			get { return raw.CharacterType; }
		}
		ReadOnlyCollection<ICard> IPublicPlayerView.Hand
		{
			get { return ((IPublicPlayerView)raw).Hand; }
		}
		Role IPublicPlayerView.Role
		{
			get { return ((IPublicPlayerView)raw).Role; }
		}
		ReadOnlyCollection<ICard> IPrivatePlayerView.Hand
		{
			get { return raw.Hand; }
		}
		Role IPrivatePlayerView.Role
		{
			get { return raw.Role; }
		}
		ReadOnlyCollection<ICard> IPrivatePlayerView.Selection
		{
			get { return raw.Selection; }
		}

		public PrivatePlayerViewProxy(IPrivatePlayerView raw)
		{
			this.raw = raw;
		}
	}
}

