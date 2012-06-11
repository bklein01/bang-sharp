// CardManager.cs
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
using System.Collections.Generic;
using System.IO;
using Gdk;

namespace BangSharp.Client.GameBoard
{
	public static class CardManager
	{
		private static readonly string CardsPath = Path.Combine("Resources", "Cards");
		private static readonly string PlayingCardsPath = Path.Combine(CardsPath, "Playing");
		private static readonly string CharacterCardsPath = Path.Combine(CardsPath, "Characters");
		private static readonly string RoleCardsPath = Path.Combine(CardsPath, "Roles");

		private static Card LoadCard(CardType type)
		{
			try
			{
				return new Card(new Pixbuf(Path.Combine(PlayingCardsPath, type + ".png")));
			}
			catch
			{
				return null;
			}
		}
		private static Card LoadCard(CharacterType type)
		{
			try
			{
				return new Card(new Pixbuf(Path.Combine(CharacterCardsPath, type + ".png")));
			}
			catch
			{
				return null;
			}
		}
		private static Card LoadCard(Role type)
		{
			try
			{
				return new Card(new Pixbuf(Path.Combine(RoleCardsPath, type + ".png")));
			}
			catch
			{
				return null;
			}
		}

		private static Dictionary<CardType, Card> playingCards;
		private static Dictionary<CharacterType, Card> characterCards;
		private static Dictionary<Role, Card> roleCards;

		public static void Init(ISession session = null)
		{
			List<CardType> cardTypes = Utils.GetCardTypes(session, true);
			playingCards = new Dictionary<CardType, Card>(cardTypes.Count);
			foreach(CardType type in cardTypes)
				playingCards.Add(type, LoadCard(type));

			List<CharacterType> characterTypes = Utils.GetCharacterTypes(session, true);
			characterCards = new Dictionary<CharacterType, Card>(characterTypes.Count);
			foreach(CharacterType type in characterTypes)
				characterCards.Add(type, LoadCard(type));

			List<Role> roles = Utils.GetRoles(true);
			roleCards = new Dictionary<Role, Card>(roles.Count);
			foreach(Role type in roles)
				roleCards.Add(type, LoadCard(type));
		}
		public static void Flush()
		{
			if(playingCards != null)
			{
				foreach(Card c in playingCards.Values)
					if(c != null)
						c.Dispose();
				playingCards = null;
			}

			if(characterCards != null)
			{
				foreach(Card c in characterCards.Values)
					if(c != null)
						c.Dispose();
				characterCards = null;
			}

			if(roleCards != null)
			{
				foreach(Card c in roleCards.Values)
					if(c != null)
						c.Dispose();
				roleCards = null;
			}
		}

		public static Card GetCard(CardType type)
		{
			if(playingCards == null)
				return null;
			try
			{
				return playingCards[type];
			}
			catch(KeyNotFoundException)
			{
				return null;
			}
		}
		public static Card GetCard(CharacterType character)
		{
			if(characterCards == null)
				return null;
			try
			{
				return characterCards[character];
			}
			catch(KeyNotFoundException)
			{
				return null;
			}
		}
		public static Card GetCard(Role role)
		{
			if(roleCards == null)
				return null;
			try
			{
				return roleCards[role];
			}
			catch(KeyNotFoundException)
			{
				return null;
			}
		}
	}
}

