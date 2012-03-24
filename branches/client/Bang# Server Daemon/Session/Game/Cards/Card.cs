// Card.cs
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
using System;
using BangSharp.Server.Daemon.Cards;

namespace BangSharp.Server.Daemon
{
	public class Card : ImmortalMarshalByRefObject, ICard
	{
		private sealed class EmptyCard : ImmortalMarshalByRefObject, ICard
		{
			private int id;
			
			public int ID
			{
				get { return id; }
			}
			public CardColor Color
			{
				get { return CardColor.Unknown; }
			}
			public CardType Type
			{
				get { return CardType.Unknown; }
			}
			public CardSuit Suit
			{
				get { return CardSuit.Unknown; }
			}
			public CardRank Rank
			{
				get { return CardRank.Unknown; }
			}
			
			public EmptyCard(Card parent)
			{
				this.id = parent.id;
			}
		}
		private Game game;
		private int id;
		private CardColor color;
		private CardType type;
		private CardSuit suit;
		private CardRank rank;
		private Player owner;
		private EmptyCard empty;
		
		public Game Game
		{
			get { return game; }
		}
		public int ID
		{
			get { return id; }
		}
		public CardColor Color
		{
			get { return color; }
		}
		public CardType Type
		{
			get { return type; }
		}
		public CardSuit Suit
		{
			get { return suit; }
		}
		public CardRank Rank
		{
			get { return rank; }
		}
		public Player Owner
		{
			get { return owner; }
			set { owner = value; }
		}
		public bool IsInHand
		{
			get { return owner == null ? false : owner.Hand.Contains(this); }
		}
		public bool IsOnTable
		{
			get { return owner == null ? false : owner.Table.Contains(this as TableCard); }
		}
		public bool IsWeapon
		{
			get
			{
				switch(type)
				{
				case CardType.Volcanic:
				case CardType.Schofield:
				case CardType.Remington:
				case CardType.Carabine:
				case CardType.Winchester:
					return true;
				}
				return false;
			}
		}

		public virtual int WeaponRange
		{
			get { return 1; }
		}
		public virtual bool UnlimitedBangs
		{
			get { return false; }
		}
		public virtual int DistanceIn
		{
			get { return 0; }
		}
		public virtual int DistanceOut
		{
			get { return 0; }
		}
		
		public ICard Empty
		{
			get { return empty; }
		}
		
		protected Card(Game game, int id, CardType type, CardSuit suit, CardRank rank)
		{
			this.game = game;
			this.id = id;
			this.type = type;
			this.suit = suit;
			this.rank = rank;
			color = GetColor(type);
			empty = new EmptyCard(this);
		}

		public override void Disconnect()
		{
			base.Disconnect();
			empty.Disconnect();
		}
		
		public void AssertInHand()
		{
			if(owner == null)
				throw new BadCardException();
			if(!owner.Hand.Contains(this))
				throw new BadCardException();
		}
		public void AssertOnTable()
		{
			if(Owner == null)
				throw new BadCardException();
			if(!Owner.Table.Contains(this as TableCard))
				throw new BadCardException();
		}

		public virtual void CheckMissed(CardResultCallback resultCallback)
		{
			throw new BadCardException();
		}
		public virtual void Play()
		{
			throw new BadUsageException();
		}
		public virtual void PlayVirtually(Card card)
		{
			throw new BadUsageException();
		}

		private static CardColor GetColor(CardType type)
		{
			switch(type)
			{
			case CardType.Bang:
			case CardType.Missed:
			case CardType.Beer:
			case CardType.Saloon:
			case CardType.WellsFargo:
			case CardType.Diligenza:
			case CardType.GeneralStore:
			case CardType.Panic:
			case CardType.CatBalou:
			case CardType.Indians:
			case CardType.Duel:
			case CardType.Gatling:
			case CardType.Dodge:
			case CardType.Punch:
				return CardColor.Brown;
			case CardType.Mustang:
			case CardType.Appaloosa:
			case CardType.Barrel:
			case CardType.Dynamite:
			case CardType.Jail:
			case CardType.Volcanic:
			case CardType.Schofield:
			case CardType.Remington:
			case CardType.Carabine:
			case CardType.Winchester:
			case CardType.Hideout:
			case CardType.Silver:
				return CardColor.Blue;
			case CardType.Springfield:
			case CardType.Brawl:
			case CardType.RagTime:
			case CardType.Tequila:
				return CardColor.Golden;
			case CardType.Sombrero:
			case CardType.IronPlate:
			case CardType.TenGallonHat:
			case CardType.Bible:
			case CardType.Canteen:
			case CardType.Knife:
			case CardType.Derringer:
			case CardType.Howitzer:
			case CardType.Pepperbox:
			case CardType.BuffaloRifle:
			case CardType.CanCan:
			case CardType.Conestoga:
			case CardType.PonyExpress:
				return CardColor.Green;
			//case CardType.:
			default:
				return CardColor.Unknown;
			}
		}
		public static Card GetCard(Game game, int id, CardType type, CardSuit suit, CardRank rank)
		{
			switch(type)
			{
			case CardType.Bang:
				return new Cards.Bang(game, id, suit, rank);
			case CardType.Missed:
				return new Missed(game, id, suit, rank);
			case CardType.Beer:
				return new Beer(game, id, suit, rank);
			case CardType.Saloon:
				return new Saloon(game, id, suit, rank);
			case CardType.WellsFargo:
				return new WellsFargo(game, id, suit, rank);
			case CardType.Diligenza:
				return new Diligenza(game, id, suit, rank);
			case CardType.GeneralStore:
				return new GeneralStore(game, id, suit, rank);
			case CardType.Panic:
				return new Panic(game, id, suit, rank);
			case CardType.CatBalou:
				return new CatBalou(game, id, suit, rank);
			case CardType.Indians:
				return new Indians(game, id, suit, rank);
			case CardType.Duel:
				return new Duel(game, id, suit, rank);
			case CardType.Gatling:
				return new Gatling(game, id, suit, rank);
			case CardType.Mustang:
				return new Mustang(game, id, suit, rank);
			case CardType.Appaloosa:
				return new Appaloosa(game, id, suit, rank);
			case CardType.Barrel:
				return new Barrel(game, id, suit, rank);
			case CardType.Dynamite:
				return new Dynamite(game, id, suit, rank);
			case CardType.Jail:
				return new Jail(game, id, suit, rank);
			case CardType.Volcanic:
				return new Volcanic(game, id, suit, rank);
			case CardType.Schofield:
				return new Schofield(game, id, suit, rank);
			case CardType.Remington:
				return new Remington(game, id, suit, rank);
			case CardType.Carabine:
				return new Carabine(game, id, suit, rank);
			case CardType.Winchester:
				return new Winchester(game, id, suit, rank);
			case CardType.Dodge:
				return new Dodge(game, id, suit, rank);
			case CardType.Punch:
				return new Punch(game, id, suit, rank);
			case CardType.Springfield:
				return new Springfield(game, id, suit, rank);
			case CardType.Brawl:
				return new Brawl(game, id, suit, rank);
			case CardType.RagTime:
				return new RagTime(game, id, suit, rank);
			case CardType.Tequila:
				return new Tequila(game, id, suit, rank);
			case CardType.Whisky:
				return new Whisky(game, id, suit, rank);
			case CardType.Hideout:
				return new Hideout(game, id, suit, rank);
			case CardType.Silver:
				return new Silver(game, id, suit, rank);
			case CardType.Sombrero:
				return new Sombrero(game, id, suit, rank);
			case CardType.IronPlate:
				return new IronPlate(game, id, suit, rank);
			case CardType.TenGallonHat:
				return new TenGallonHat(game, id, suit, rank);
			case CardType.Bible:
				return new Bible(game, id, suit, rank);
			case CardType.Canteen:
				return new Canteen(game, id, suit, rank);
			case CardType.Knife:
				return new Knife(game, id, suit, rank);
			case CardType.Derringer:
				return new Derringer(game, id, suit, rank);
			case CardType.Howitzer:
				return new Howitzer(game, id, suit, rank);
			case CardType.Pepperbox:
				return new Pepperbox(game, id, suit, rank);
			case CardType.BuffaloRifle:
				return new BuffaloRifle(game, id, suit, rank);
			case CardType.CanCan:
				return new CanCan(game, id, suit, rank);
			case CardType.Conestoga:
				return new Conestoga(game, id, suit, rank);
			case CardType.PonyExpress:
				return new PonyExpress(game, id, suit, rank);
				//case CardType.: return new(game, id, suit, rank);
			default:
				throw new ArgumentOutOfRangeException("type");
			}
		}
	}
}