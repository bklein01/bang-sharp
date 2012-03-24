// CardHelper.cs
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
using System.Collections.Generic;
using System.Linq;

namespace BangSharp.AI
{
	internal sealed class CardHelper
	{
		private IPlayerControl control;

		private IGame Game
		{
			get { return control.Game; }
		}
		private IPrivatePlayerView Player
		{
			get { return control.PrivatePlayerView; }
		}

		public CardHelper(IPlayerControl control)
		{
			this.control = control;
		}

		public bool HasAbility(CharacterType character)
		{
			return Player.CharacterType == character || Player.AdditionalCharacters.Contains(character);
		}
		public int DiscardableCards
		{
			get { return Player.Hand.Count(c => EvaluateCard(c.Type) < 15); }
		}
		public bool UnlimitedBangs
		{
			get { return Player.Hand.Any(c => c.Type == CardType.Volcanic) || HasAbility(CharacterType.WillyTheKid); }
		}
		public bool NeedLifeHelp
		{
			get
			{
				IPrivatePlayerView player = Player;
				int lifeDeficit = player.MaxLifePoints - player.LifePoints;
				if(lifeDeficit <= 0)
					return false;
				int lifeHelp = 0;
				int beerPower = -1;
				int discardableCards = -1;
				foreach(ICard card in player.Hand)
					lifeHelp += GetLifeHelp(card.Type, ref beerPower, ref discardableCards);
				if(HasAbility(CharacterType.SidKetchum) && discardableCards >= 2)
					lifeHelp += discardableCards / 2;
				return lifeHelp < lifeDeficit;
			}
		}

		public int EvaluateCard(CardType type)
		{
			switch(type)
			{
			case CardType.WellsFargo:
				return 35;
			case CardType.Diligenza:
				return 34;
			case CardType.PonyExpress:
				return 33;
			case CardType.Hideout:
			case CardType.Mustang:
				return 32;
			case CardType.Appaloosa:
			case CardType.Silver:
				return 31;
			case CardType.Barrel:
				return 30;
			case CardType.Dodge:
				return 29;
			case CardType.Bible:
				return 28;
			case CardType.Indians:
				return 27;
			case CardType.Gatling:
				return 26;
			case CardType.Howitzer:
				return 25;
			case CardType.Brawl:
				return 24;
			case CardType.Duel:
				return 23;
			case CardType.Jail:
				return 22;
			case CardType.BuffaloRifle:
				return 21;
			case CardType.Springfield:
				return 20;
			case CardType.Bang:
				if(Player.Hand.Any(c => c.Type == CardType.Duel))
					return 32;
				return 19;
			case CardType.Derringer:
				return 18;
			case CardType.Pepperbox:
			case CardType.Knife:
				return 17;
			case CardType.Punch:
				return 16;
			case CardType.Beer:
				return 15;
			case CardType.Panic:
				return 14;
			case CardType.Conestoga:
				return 13;
			case CardType.RagTime:
				return 12;
			case CardType.CatBalou:
				return 11;
			case CardType.CanCan:
				return 10;
			case CardType.Missed:
				return 9;
			case CardType.Sombrero:
			case CardType.IronPlate:
			case CardType.TenGallonHat:
				return 8;
			case CardType.Canteen:
				return 7;
			case CardType.Tequila:
				return 6;
			case CardType.GeneralStore:
				return 5;
			case CardType.Saloon:
				return 0;
			case CardType.Dynamite:
				return 0;
			case CardType.Volcanic:
				if(UnlimitedBangs)
					return 0;
				return 2;
			case CardType.Schofield:
				return 1;
			case CardType.Remington:
				return 2;
			case CardType.Carabine:
				return 3;
			case CardType.Winchester:
				return 4;
			}
			return -1;
		}

		private int GetLifeHelp(CardType type, ref int beerPower, ref int discardableCards)
		{
			switch(type)
			{
			case CardType.Beer:
				if(beerPower < 0)
					beerPower = HasAbility(CharacterType.TequilaJoe) ? 2 : 1;
				return Game.Players.Count == 2 ? 0 : beerPower;
			case CardType.Saloon:
				return 1;
			case CardType.Tequila:
				if(discardableCards < 0)
					discardableCards = DiscardableCards;
				if(discardableCards > 0)
				{
					discardableCards--;
					return 1;
				}
				return 0;
			case CardType.Whisky:
				if(discardableCards < 0)
					discardableCards = DiscardableCards;
				if(discardableCards > 0)
				{
					discardableCards--;
					return 2;
				}
				return 0;
			default:
				return 0;
			}
		}

		public bool IsTableCardWorth(CardType type)
		{
			IPrivatePlayerView player = Player;
			switch(type)
			{
			case CardType.Mustang:
			case CardType.Hideout:
			case CardType.Appaloosa:
			case CardType.Silver:
			case CardType.PonyExpress:
			case CardType.Bible:
			case CardType.Howitzer:
			case CardType.BuffaloRifle:
			case CardType.Conestoga:
				return true;
			case CardType.Barrel:
				return !player.Hand.Any(c => c.Type == CardType.Barrel) && !player.Table.Any(c => c.Type == CardType.Barrel);
			case CardType.Volcanic:
				return !UnlimitedBangs;
			}
			return false;
		}
		public bool IsCardWorthSkippingDraw(CardType type)
		{
			IPrivatePlayerView player = Player;
			switch(type)
			{
			case CardType.Diligenza:
			case CardType.WellsFargo:
			case CardType.PonyExpress:
			case CardType.Indians:
			case CardType.Gatling:
			case CardType.Mustang:
			case CardType.Hideout:
				return true;
			case CardType.Bang:
				return UnlimitedBangs || player.Hand.Any(c => c.Type == CardType.Duel);
			case CardType.Beer:
			case CardType.Saloon:
			case CardType.Whisky:
			case CardType.Tequila:
				return NeedLifeHelp;
			}
			return false;
		}

		public ICard WorstCard(IEnumerable<ICard> cards)
		{
			int lastValue = -1;
			try
			{
				ICard worst = cards.First();
				foreach(ICard card in cards)
				{
					int value = EvaluateCard(card.Type);
					if(value >= 0 && (lastValue < 0 || value < lastValue))
					{
						worst = card;
						lastValue = value;
					}
				}
				return worst;
			}
			catch(InvalidOperationException)
			{
				return null;
			}
		}
		public ICard BestCard(IEnumerable<ICard> cards)
		{
			int lastValue = -1;
			try
			{
				ICard best = cards.First();
				foreach(ICard card in cards)
				{
					int value = EvaluateCard(card.Type);
					if(value > lastValue)
					{
						best = card;
						lastValue = value;
					}
				}
				return best;
			}
			catch(InvalidOperationException)
			{
				return null;
			}
		}

		public int EvaluateCheckDeckCard(ICard card)
		{
			switch(card.Suit)
			{
			case CardSuit.Hearts:
				return 3;
			case CardSuit.Spades:
				if(card.Rank > CardRank.Nine || card.Rank < CardRank.Two)
					return 2;
				else
					return 1;
			default:
				return 1;
			}
		}
		public ICard BestCheckDeckCard(IEnumerable<ICard> cards)
		{
			int lastValue = -1;
			try
			{
				ICard best = cards.First();
				foreach(ICard card in cards)
				{
					int value = EvaluateCheckDeckCard(card);
					if(value > lastValue)
					{
						best = card;
						lastValue = value;
					}
				}
				return best;
			}
			catch(InvalidOperationException)
			{
				return null;
			}
		}

		public int EvaluateCharacter(CharacterType character)
		{
			switch(character)
			{
			case CharacterType.ApacheKid:
				return 30;
			case CharacterType.ChuckWengam:
				return 29;
			case CharacterType.JoseDelgado:
				return 28;
			case CharacterType.PixiePete:
				return 27;
			case CharacterType.PaulRegret:
				return 26;
			case CharacterType.SuzyLafayette:
				return 25;
			case CharacterType.BlackJack:
				return 24;
			case CharacterType.JesseJones:
				return 23;
			case CharacterType.KitCarlson:
				return 22;
			case CharacterType.ElenaFuente:
				return 21;
			case CharacterType.Jourdonnais:
				return 20;
			case CharacterType.WillyTheKid:
				return 19;
			case CharacterType.MollyStark:
				return 18;
			case CharacterType.BartCassidy:
				return 17;
			case CharacterType.ElGringo:
				return 16;
			case CharacterType.PedroRamirez:
				return 15;
			case CharacterType.PatBrennan:
				return 14;
			case CharacterType.DocHolyday:
				return 13;
			case CharacterType.SlabTheKiller:
				return 12;
			case CharacterType.BillNoface:
				return 11;
			case CharacterType.BelleStar:
				return 10;
			case CharacterType.TequilaJoe:
				return 9;
			case CharacterType.CalamityJanet:
				return 8;
			case CharacterType.LuckyDuke:
				return 7;
			case CharacterType.SidKetchum:
				return 6;
			case CharacterType.RoseDoolan:
				return 5;
			case CharacterType.SeanMallory:
				return 4;
			case CharacterType.VultureSam:
				return 3;
			case CharacterType.HerbHunter:
				return 2;
			case CharacterType.GregDigger:
				return 1;
			}
			return -1;
		}
		public IPublicPlayerView BestCharacter(IEnumerable<IPublicPlayerView> players)
		{
			int lastValue = -1;
			try
			{
				IPublicPlayerView best = players.First();
				foreach(IPublicPlayerView player in players)
				{
					int value = EvaluateCharacter(player.CharacterType);
					if(value > lastValue)
					{
						best = player;
						lastValue = value;
					}
				}
				return best;
			}
			catch(InvalidOperationException)
			{
				return null;
			}
		}
		public CharacterType BestCharacter(IEnumerable<CharacterType> characters)
		{
			int lastValue = -1;
			try
			{
				CharacterType best = characters.First();
				foreach(CharacterType character in characters)
				{
					int value = EvaluateCharacter(character);
					if(value > lastValue)
					{
						best = character;
						lastValue = value;
					}
				}
				return best;
			}
			catch(InvalidOperationException)
			{
				return CharacterType.Unknown;
			}
		}
	}
}
