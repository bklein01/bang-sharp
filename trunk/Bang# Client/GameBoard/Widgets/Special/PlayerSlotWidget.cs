// PlayerSlotWidget.cs
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
	public partial class PlayerSlotWidget : Bin
	{
		private RootWidget root;
		private int id;
		private CharacterCardWidget lifePointsCard;

		public GeneralPlaceholderWidget HandPlaceholder
		{
			get { return playerHandPlaceholder; }
		}
		public GeneralPlaceholderWidget TablePlaceholder
		{
			get { return playerTablePlaceholder; }
		}
		public CardPlaceholderWidget RolePlaceholder
		{
			get { return playerRolePlaceholder; }
		}
		public CardPlaceholderWidget CharacterPlaceholder
		{
			get { return characterPlaceholder; }
		}

		public PlayerSlotWidget(RootWidget root, bool thisPlayer = false)
		{
			this.root = root;
			InitLayout();
			playerPic.Pixbuf = ResourceManager.GetPixbuf("Resources", "DefaultPlayerImage.png");
			if(thisPlayer)
			{
				NoActionButtonWidget button = new NoActionButtonWidget();
				button.OnClick += delegate() {
					IPlayerControl control = ConnectionManager.PlayerGameControl;
					if(control == null)
						return;
					try
					{
						control.RespondNoAction();
						root.SetResponseType("No action");
					}
					catch(GameException e)
					{
						root.SetResponseType("No action", e);
					}
					RequestRedraw();
				};
				padding9.Children.Add(button);
			}
			lifePointsCard = new CharacterCardWidget(CharacterType.Unknown);
			onlinePic.Pixbuf = ResourceManager.GetPixbuf("Resources", "Offline.png");
		}

		public void Clear()
		{
			id = 0;
			if(lifePointsCard.Parent != null)
				characterOverlay.Children.Remove(lifePointsCard);
			playerPic.Pixbuf = ResourceManager.GetPixbuf("Resources", "DefaultPlayerImage.png");
			playerLabel.Markup = "";
			onlinePic.Pixbuf = ResourceManager.GetPixbuf("Resources", "Offline.png");
		}

		public void Update(IPlayer sessionPlayer)
		{
			id = sessionPlayer.ID;
			lifePointsCard.Type = CharacterType.Unknown;
			if(lifePointsCard.Parent == null)
				characterOverlay.Children.Add(lifePointsCard);

			Gdk.Pixbuf pixbuf = PictureManager.GetPixbuf(sessionPlayer.Image);
			if(pixbuf == null)
				pixbuf = ResourceManager.GetPixbuf("Resources", "DefaultPlayerImage.png");
			playerPic.Pixbuf = pixbuf;
			playerLabel.Markup = "<span color='orange'>" + sessionPlayer.Name + "</span>";
			onlinePic.Pixbuf = sessionPlayer.HasListener ?
				ResourceManager.GetPixbuf("Resources", "Online.png") :
					ResourceManager.GetPixbuf("Resources", "Offline.png");
		}

		protected override bool OnLeftClick(double x, double y)
		{
			IPlayerControl control = ConnectionManager.PlayerGameControl;
			if(control == null)
				return true;
			try
			{
				control.RespondPlayer(id);
				root.SetResponseType("Player #" + id);
			}
			catch(GameException e)
			{
				root.SetResponseType("Player #" + id, e);
			}
			RequestRedraw();
			return true;
		}
	}
}

