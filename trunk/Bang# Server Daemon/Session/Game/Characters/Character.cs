// Character.cs
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
using System.Collections.ObjectModel;
using BangSharp.Server.Daemon.Characters;

namespace BangSharp.Server.Daemon
{
	public class Character
	{
		private Player player;
		private CharacterType type;
		
		protected Game Game
		{
			get { return player.Game; }
		}
		public Player Player
		{
			get { return player; }
		}
		public CharacterType Type
		{
			get { return type; }
		}
		
		public virtual int MaxLifePoints
		{
			get { return 4; }
		}
		public virtual int InitialCardCount
		{
			get { return MaxLifePoints; }
		}
		public virtual int MaxCardCount
		{
			get { return player.LifePoints; }
		}
		public virtual int DistanceIn
		{
			get { return 0; }
		}
		public virtual int DistanceOut
		{
			get { return 0; }
		}
		public virtual int BangPower
		{
			get { return 1; }
		}
		public virtual int BeerPower
		{
			get { return 1; }
		}
		public virtual bool UnlimitedBangs
		{
			get { return false; }
		}
		public virtual bool TakesDeadPlayersCards
		{
			get { return false; }
		}
		
		protected Character(Player player, CharacterType type)
		{
			this.player = player;
			this.type = type;
		}

		protected void OnUsedAbility()
		{
			player.Game.Session.EventManager.OnPlayerUsedAbility(player, type);
		}
		protected void OnUsedAbility(Player targetPlayer)
		{
			player.Game.Session.EventManager.OnPlayerUsedAbility(player, type, targetPlayer);
		}

		public virtual bool HasCardEffect(Card card)
		{
			return true;
		}
		public virtual bool IsMissed(Card card)
		{
			return card.Type == CardType.Missed;
		}
		public virtual bool IsBang(Card card)
		{
			return card.Type == CardType.Bang;
		}

		public virtual void Draw()
		{
			DrawFirstCard(first => {
				if(first == null)
					first = Game.GameTable.PlayerDrawFromDeck(player, 1, RevealFirstDrawnCard)[0];
				player.OnDrewFirstCard(first);
				if(DrawCardCount >= 2)
					DrawSecondCard(second => {
						if(second == null)
							second = Game.GameTable.PlayerDrawFromDeck(player, 1, RevealSecondDrawnCard)[0];
						player.OnDrewSecondCard(second);
						if(DrawCardCount > 2)
							Game.GameTable.PlayerDrawFromDeck(player, DrawCardCount - 2);
						player.OnAfterDraw();
					});
				else
					player.OnAfterDraw();
			});
		}
		public virtual int DrawCardCount
		{
			get { return 2; }
		}
		public virtual bool RevealFirstDrawnCard
		{
			get { return false; }
		}
		public virtual bool RevealSecondDrawnCard
		{
			get { return false; }
		}
		public virtual void DrawFirstCard(CardCallback callback)
		{
			callback(null);
		}
		public virtual void DrawSecondCard(CardCallback callback)
		{
			callback(null);
		}
		public virtual void OnDrewFirstCard(Card card)
		{
		}
		public virtual void OnDrewSecondCard(Card card)
		{
		}
		public virtual void OnAfterDraw()
		{
		}

		public virtual void PlayCard(Card card)
		{
			card.Play();
		}
		public virtual void UseAbility()
		{
			throw new BadUsageException();
		}
		public virtual void CheckDeck(Card causedBy, CheckDeckCallback checkCallback, CardResultCallback resultCallback)
		{
			Card checkedCard = player.Game.GameTable.CheckDeck();
			bool result = checkCallback(checkedCard);
			Game.Session.EventManager.OnPlayerCheckedDeck(player, checkedCard, causedBy, result);
			resultCallback(causedBy, result);
		}
		public virtual void CheckMissed(CardResultCallback resultCallback)
		{
			throw new BadUsageException();
		}
		
		public virtual void OnHit(int hitPoints, Player causedBy)
		{
		}
		public virtual void OnEmptyHand()
		{
		}
		public virtual void OnPlayerDied(Player player)
		{
		}
		public virtual void OnTurnStarted()
		{
		}
		public virtual void OnPlayContinue()
		{
		}
		public virtual void OnTurnEnded()
		{
		}
		public virtual void OnPlayedCard(Card card)
		{
		}
		public virtual void OnRespondedWithCard(Card card)
		{
		}
		public virtual bool SavePlayer()
		{
			return false;
		}
		
		public static Character GetCharacter(Player player, CharacterType type)
		{
			switch(type)
			{
			case CharacterType.BartCassidy:
				return new BartCassidy(player);
			case CharacterType.BlackJack:
				return new BlackJack(player);
			case CharacterType.CalamityJanet:
				return new CalamityJanet(player);
			case CharacterType.ElGringo:
				return new ElGringo(player);
			case CharacterType.JesseJones:
				return new JesseJones(player);
			case CharacterType.Jourdonnais:
				return new Jourdonnais(player);
			case CharacterType.KitCarlson:
				return new KitCarlson(player);
			case CharacterType.LuckyDuke:
				return new LuckyDuke(player);
			case CharacterType.PaulRegret:
				return new PaulRegret(player);
			case CharacterType.PedroRamirez:
				return new PedroRamirez(player);
			case CharacterType.RoseDoolan:
				return new RoseDoolan(player);
			case CharacterType.SidKetchum:
				return new SidKetchum(player);
			case CharacterType.SlabTheKiller:
				return new SlabTheKiller(player);
			case CharacterType.SuzyLafayette:
				return new SuzyLafayette(player);
			case CharacterType.VultureSam:
				return new VultureSam(player);
			case CharacterType.WillyTheKid:
				return new WillyTheKid(player);
			case CharacterType.ApacheKid:
				return new ApacheKid(player);
			case CharacterType.BelleStar:
				return new BelleStar(player);
			case CharacterType.BillNoface:
				return new BillNoface(player);
			case CharacterType.ChuckWengam:
				return new ChuckWengam(player);
			case CharacterType.DocHolyday:
				return new DocHolyday(player);
			case CharacterType.ElenaFuente:
				return new ElenaFuente(player);
			case CharacterType.GregDigger:
				return new GregDigger(player);
			case CharacterType.HerbHunter:
				return new HerbHunter(player);
			case CharacterType.JoseDelgado:
				return new JoseDelgado(player);
			case CharacterType.MollyStark:
				return new MollyStark(player);
			case CharacterType.PatBrennan:
				return new PatBrennan(player);
			case CharacterType.PixiePete:
				return new PixiePete(player);
			case CharacterType.SeanMallory:
				return new SeanMallory(player);
			case CharacterType.TequilaJoe:
				return new TequilaJoe(player);
			case CharacterType.VeraCuster:
				return new VeraCuster(player);
			default:
				throw new ArgumentOutOfRangeException("type");
				//case CharacterType.: return new (player);
			}
		}
	}
}