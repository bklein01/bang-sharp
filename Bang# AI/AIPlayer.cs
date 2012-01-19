// AIPlayer.cs
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Bang.AI
{
	/// <summary>
	/// This class provides the AI player functionality.
	/// </summary>
	public sealed class AIPlayer : ImmortalMarshalByRefObject, IPlayerEventListener
	{
		private static readonly IEnumerator<string> names = new List<string>() { "John", "Mike", "George", "Kelly", "Susan", "Greg", "Andy", "Simon" }.GetEnumerator();

		private static string NextName()
		{
			if(!names.MoveNext())
			{
				names.Reset();
				names.MoveNext();
			}
			return names.Current;
		}

		private CreatePlayerData data;
		private CardHelper cardHelper;
		private PlayerHelper playerHelper;
		private IPlayerSessionControl sessionControl;
		private IPlayerControl control;
		private List<int> triedCards;
		private CharacterType characterToUse;
		private List<CharacterType> triedAbilities;

		/// <summary>
		/// Gets this AI player's <see cref="Bang.CreatePlayerData"/>. Can be ignored.
		/// </summary>
		public CreatePlayerData CreateData
		{
			get { return data; }
		}

		/// <summary>
		/// Creates a new AI player.
		/// </summary>
		public AIPlayer()
		{
			data = new CreatePlayerData(NextName(), null, "");
		}
		
		bool IPlayerEventListener.IsAI
		{
			get { return true; }
		}
		
		#region IPlayerEventListener implementation
		void IPlayerEventListener.OnJoinedSession(IPlayerSessionControl control)
		{
			sessionControl = control;
		}
		void IPlayerEventListener.OnJoinedGame(IPlayerControl control)
		{
			this.control = control;
			cardHelper = new CardHelper(control);
			switch(control.Game.Players.Count)
			{
			case 2:
				playerHelper = new TwoPlayersPlayerHelper(control);
				break;
			case 3:
				playerHelper = new ThreePlayersPlayerHelper(control);
				break;
			default:
				playerHelper = new StandardPlayerHelper(control);
				break;
			}
			triedCards = new List<int>();
			triedAbilities = new List<CharacterType>();
		}

		private bool TryRespondCard(int id)
		{
			try
			{
				control.RespondCard(id);
				return true;
			}
			catch(GameException)
			{
			}
			return false;
		}
		private bool TryRespondCardRemember(int id)
		{
			if(triedCards.Contains(id))
				return false;
			try
			{
				control.RespondCard(id);
				triedCards.Add(id);
				return true;
			}
			catch(GameException)
			{
			}
			return false;
		}
		private bool TryRespondPlayer(int id)
		{
			try
			{
				control.RespondPlayer(id);
				return true;
			}
			catch(GameException)
			{
			}
			return false;
		}
		private bool TryRespondUseAbility()
		{
			try
			{
				control.RespondUseAbility();
				return true;
			}
			catch(GameException)
			{
			}
			return false;
		}
		private bool TryRespondUseAbilityRemember(CharacterType character)
		{
			if(!cardHelper.HasAbility(character) || triedAbilities.Contains(character))
				return false;
			try
			{
				control.RespondUseAbility();
				characterToUse = character;
				triedAbilities.Add(character);
				return true;
			}
			catch(GameException)
			{
			}
			return false;
		}
		private bool TryRespondNoAction()
		{
			try
			{
				control.RespondNoAction();
				return true;
			}
			catch(GameException)
			{
			}
			return false;
		}

		private void ProcessRequest(object state)
		{
			if(control == null)
				return;

			IGame game = control.Game;
			IPrivatePlayerView player = control.PrivatePlayerView;

			switch(player.RequestType)
			{
				#region Draw
			case RequestType.Draw:
				if(cardHelper.HasAbility(CharacterType.BlackJack) ||
					cardHelper.HasAbility(CharacterType.KitCarlson) ||
					cardHelper.HasAbility(CharacterType.PixiePete))
				{

					control.RespondUseAbility();
					return;
				}
				if(cardHelper.HasAbility(CharacterType.BillNoface) &&
					player.MaxLifePoints - player.LifePoints > 2)
				{
					control.RespondUseAbility();
					return;
				}
				if(cardHelper.HasAbility(CharacterType.JesseJones) &&
					game.Players.Any(p => p.ID != player.ID && p.Hand.Count > 0))
				{
					control.RespondUseAbility();
					return;
				}
				if(cardHelper.HasAbility(CharacterType.PatBrennan) &&
					game.Players.Except(playerHelper.Allies).Any(p => p.Table.Any(c => cardHelper.IsCardWorthSkippingDraw(c.Type))))
				{
					control.RespondUseAbility();
					return;
				}
				if(cardHelper.HasAbility(CharacterType.PedroRamirez) &&
					game.GraveyardTop != null && cardHelper.IsCardWorthSkippingDraw(game.GraveyardTop.Type))
				{
					control.RespondUseAbility();
					return;
				}
				control.RespondDraw();
				return;
				#endregion
				#region Play
			case RequestType.Play:
				// First, play cards that let us gain cards:
				if(cardHelper.HasAbility(CharacterType.ChuckWengam) &&
					player.LifePoints > 2 &&
					TryRespondUseAbilityRemember(CharacterType.ChuckWengam))
					return;
				if(cardHelper.HasAbility(CharacterType.JoseDelgado) &&
					player.Hand.Any(c => c.Color == CardColor.Blue && !cardHelper.IsTableCardWorth(c.Type)) &&
					TryRespondUseAbilityRemember(CharacterType.JoseDelgado))
					return;
				foreach(ICard c in player.Hand)
					switch(c.Type)
					{
					case CardType.WellsFargo:
					case CardType.Diligenza:
						if(TryRespondCard(c.ID))
							return;
						break;
					}
				foreach(ICard c in player.Table)
					if(c.Type == CardType.PonyExpress &&
						TryRespondCard(c.ID))
						return;
				// Now get all the blue & green cards on table:
				foreach(ICard card in player.Hand)
					switch(card.Color)
					{
					case CardColor.Green:
						if(TryRespondCard(card.ID))
							return;
						break;
					case CardColor.Blue:
						switch(card.Type)
						{
						case CardType.Jail:
							if(TryRespondCardRemember(card.ID))
								return;
							break;
						case CardType.Dynamite:
							if(cardHelper.HasAbility(CharacterType.LuckyDuke) &&
								TryRespondCard(card.ID))
								return;
							break;
						case CardType.Appaloosa:
							if(!player.Table.Any(c => c.Type == CardType.Appaloosa) &&
								TryRespondCard(card.ID))
								return;
							break;
						case CardType.Silver:
							if(!player.Table.Any(c => c.Type == CardType.Silver) &&
								TryRespondCard(card.ID))
								return;
							break;
						case CardType.Mustang:
							if(!player.Table.Any(c => c.Type == CardType.Mustang) &&
								TryRespondCard(card.ID))
								return;
							break;
						case CardType.Hideout:
							if(!player.Table.Any(c => c.Type == CardType.Hideout) &&
								TryRespondCard(card.ID))
								return;
							break;
						case CardType.Barrel:
							if(!player.Table.Any(c => c.Type == CardType.Barrel) &&
								TryRespondCard(card.ID))
								return;
							break;
						case CardType.Volcanic:
							if(!cardHelper.HasAbility(CharacterType.WillyTheKid) &&
								!player.Table.Any(c => c.Type == CardType.Volcanic ||
									c.Type == CardType.Carabine ||
									c.Type == CardType.Winchester) &&
								TryRespondCard(card.ID))
								return;
							break;
						case CardType.Schofield:
							if(player.Table.Any(c => c.Type == CardType.Volcanic))
							{
								if(cardHelper.HasAbility(CharacterType.WillyTheKid) &&
									TryRespondCard(card.ID))
									return;
							}
							else
							if(!player.Table.Any(c => c.Type == CardType.Schofield ||
									c.Type == CardType.Remington ||
									c.Type == CardType.Carabine ||
									c.Type == CardType.Winchester) &&
								TryRespondCard(card.ID))
								return;
							break;
						case CardType.Remington:
							if(player.Table.Any(c => c.Type == CardType.Volcanic))
							{
								if(cardHelper.HasAbility(CharacterType.WillyTheKid) &&
									TryRespondCard(card.ID))
									return;
							}
							else
							if(!player.Table.Any(c => c.Type == CardType.Remington ||
									c.Type == CardType.Carabine ||
									c.Type == CardType.Winchester) &&
								TryRespondCard(card.ID))
								return;
							break;
						case CardType.Carabine:
							if(player.Table.Any(c => c.Type == CardType.Volcanic))
							{
								if(cardHelper.HasAbility(CharacterType.WillyTheKid) &&
									TryRespondCard(card.ID))
									return;
							}
							else
							if(!player.Table.Any(c => c.Type == CardType.Carabine ||
									c.Type == CardType.Winchester) &&
								TryRespondCard(card.ID))
								return;
							break;
						case CardType.Winchester:
							if(player.Table.Any(c => c.Type == CardType.Volcanic))
							{
								if(cardHelper.HasAbility(CharacterType.WillyTheKid) &&
									TryRespondCard(card.ID))
									return;
							}
							else
							if(!player.Table.Any(c => c.Type == CardType.Winchester) &&
								TryRespondCard(card.ID))
								return;
							break;
						}
						break;
					}
				// Then heal our and allies' lives:
				if(player.LifePoints < player.MaxLifePoints)
				{
					// First, try playing canteen and beers:
					foreach(ICard c in player.Table)
						if(c.Type == CardType.Canteen &&
							TryRespondCardRemember(c.ID))
							return;
					foreach(ICard c in player.Hand)
						if(c.Type == CardType.Beer &&
							TryRespondCardRemember(c.ID))
							return;
					// If we have the Sid Ketchum character, try to use his ability:
					if(TryRespondUseAbilityRemember(CharacterType.SidKetchum))
						return;
					// Then search for whisky and tequila:
					if(cardHelper.DiscardableCards >= 1)
					{
						// First, play whisky only if the life deficit is at least 2:
						if(player.MaxLifePoints - player.LifePoints >= 2)
							foreach(ICard c in player.Hand)
								if(c.Type == CardType.Whisky &&
									TryRespondCardRemember(c.ID))
									return;
						// Then try to play tequila on self:
						foreach(ICard c in player.Hand)
							if(c.Type == CardType.Tequila &&
								TryRespondCardRemember(c.ID))
								return;
						// Finally play whisky regardless the life deficit:
						foreach(ICard c in player.Hand)
							if(c.Type == CardType.Whisky &&
								TryRespondCardRemember(c.ID))
								return;
					}
					// If we have only 1 life remaining, try also saloon:
					if(player.LifePoints == 1)
						foreach(ICard c in player.Hand)
							if(c.Type == CardType.Saloon &&
								TryRespondCard(c.ID))
								return;
				}
				// And now we try to heal our allies' lives:
				int alliesCount = playerHelper.Allies.Count(p => p.LifePoints < p.MaxLifePoints);
				int enemiesCount = playerHelper.Enemies.Count(p => p.LifePoints < p.MaxLifePoints);
				// Try saloon if it doesn't make more harm than good:
				if(alliesCount > enemiesCount)
					foreach(ICard c in player.Hand)
						if(c.Type == CardType.Saloon &&
							TryRespondCard(c.ID))
							return;
				// Try tequila:
				if(alliesCount >= 1 && cardHelper.DiscardableCards >= 1)
					foreach(ICard c in player.Hand)
						if(c.Type == CardType.Tequila &&
							TryRespondCardRemember(c.ID))
							return;
				// Then steal and cancel cards:
				foreach(ICard c in player.Hand)
					switch(c.Type)
					{
					case CardType.Panic:
					case CardType.CatBalou:
						if(TryRespondCardRemember(c.ID))
							return;
						break;
					case CardType.Brawl:
					case CardType.RagTime:
						if(cardHelper.DiscardableCards >= 1 &&
							TryRespondCardRemember(c.ID))
							return;
						break;
					}
				foreach(ICard c in player.Table)
					switch(c.Type)
					{
					case CardType.Conestoga:
					case CardType.CanCan:
						if(TryRespondCardRemember(c.ID))
							return;
						break;
					}
				// Finally, attack:
				foreach(ICard card in player.Hand)
					switch(card.Type)
					{
					case CardType.Bang:
					case CardType.Indians:
					case CardType.Duel:
					case CardType.Gatling:
					case CardType.Punch:
					case CardType.Springfield:
						if(TryRespondCardRemember(card.ID))
							return;
						break;
					case CardType.Missed:
						if(cardHelper.HasAbility(CharacterType.CalamityJanet) &&
							player.Hand.Count(c => c.Type == CardType.Missed) >= 2 &&
							TryRespondCardRemember(card.ID))
							return;
						break;
					}
				foreach(ICard c in player.Table)
					switch(c.Type)
					{
					case CardType.Knife:
					case CardType.Derringer:
					case CardType.Howitzer:
					case CardType.Pepperbox:
					case CardType.BuffaloRifle:
						if(TryRespondCardRemember(c.ID))
							return;
						break;
					}
				if(TryRespondUseAbilityRemember(CharacterType.DocHolyday))
					return;

				// Oh, and play General Store, if you have one:
				foreach(ICard card in player.Hand)
					if(card.Type == CardType.GeneralStore &&
						TryRespondCard(card.ID))
						return;

				// End the play stage:
				control.RespondNoAction();
				triedCards.Clear();
				triedAbilities.Clear();
				return;
				#endregion
				#region DiscardCard
			case RequestType.DiscardCard:
			case RequestType.SidKetchum:
			case RequestType.GoldenCard:
				List<ICard> availableCards = new List<ICard>(player.Hand);
				while(availableCards.Count != 0)
				{
					ICard worst = cardHelper.WorstCard(availableCards);
					if(TryRespondCard(worst.ID))
						return;
					availableCards.Remove(worst);
				}
				control.RespondNoAction();
				return;
				#endregion
				#region DocHolyday
			case RequestType.DocHolyday:
				availableCards = new List<ICard>();
				availableCards.AddRange(player.Hand);
				availableCards.AddRange(player.Table);
				while(availableCards.Count != 0)
				{
					ICard worst = cardHelper.WorstCard(availableCards);
					try
					{
						control.RespondCard(worst.ID);
						return;
					}
					catch(BadCardException)
					{
					}
					availableCards.Remove(worst);
				}
				control.RespondNoAction();
				return;
				#endregion
				#region BeerRescue
			case RequestType.BeerRescue:
				foreach(ICard c in player.Hand)
					if(c.Type == CardType.Beer)
					{
						control.RespondCard(c.ID);
						return;
					}
				control.RespondNoAction();
				return;
				#endregion
				#region Shot
			case RequestType.Shot:
				// First try to use ability:
				if(TryRespondUseAbility())
					return;

				// Then try barrels:
				foreach(ICard c in player.Table)
					if(c.Type == CardType.Barrel &&
						TryRespondCard(c.ID))
						return;

				// Then seek for a Bible:
				foreach(ICard c in player.Table)
					if(c.Type == CardType.Bible &&
						TryRespondCard(c.ID))
						return;

				// Then seek for a Dodge:
				foreach(ICard c in player.Hand)
					if(c.Type == CardType.Dodge &&
						TryRespondCard(c.ID))
						return;

				// Then try other table cards:
				foreach(ICard c in player.Table)
					if(TryRespondCard(c.ID))
						return;

				// Then try Missed:
				foreach(ICard c in player.Hand)
					if(c.Type == CardType.Missed &&
						TryRespondCard(c.ID))
						return;

				// Eventually try other hand cards (Elena Fuente):
				availableCards = new List<ICard>(player.Hand);
				while(availableCards.Count != 0)
				{
					ICard worst = cardHelper.WorstCard(availableCards);
					if(TryRespondCard(worst.ID))
						return;
					availableCards.Remove(worst);
				}
				control.RespondNoAction();
				return;
				#endregion
				#region ThrowBang
			case RequestType.ThrowBang:
				// Try all hand cards:
				foreach(ICard c in player.Hand)
					if(TryRespondCard(c.ID))
						return;
				control.RespondNoAction();
				return;
				#endregion
				#region ShotTarget
			case RequestType.ShotTarget:
			case RequestType.DuelTarget:
			case RequestType.JailTarget:
				// Try only enemies then respond with no action:
				foreach(IPublicPlayerView enemy in playerHelper.Enemies)
					if(TryRespondPlayer(enemy.ID))
						return;
				control.RespondNoAction();
				return;
				#endregion
				#region HealTarget
			case RequestType.HealTarget:
				// If we have a life deficit heal us, otherwise heal an ally:
				if(player.LifePoints < player.MaxLifePoints)
				{
					control.RespondPlayer(player.ID);
					return;
				}
				IPublicPlayerView poorestAlly = null;
				int maxDeficit = 0;
				foreach(IPublicPlayerView p in playerHelper.Allies)
				{
					int deficit = p.MaxLifePoints - p.LifePoints;
					if(deficit > maxDeficit)
					{
						poorestAlly = p;
						maxDeficit = deficit;
					}
				}
				if(poorestAlly != null &&
					TryRespondPlayer(poorestAlly.ID))
					return;
				control.RespondNoAction();
				return;
				#endregion
				#region StealCard
			case RequestType.StealCard:
			case RequestType.CancelCard:
				availableCards = new List<ICard>();
				IEnumerable<IPublicPlayerView> nonAllies = game.Players.Where(p => p.IsAlive).Except(playerHelper.Allies);
				IEnumerable<IPublicPlayerView> nonEnemies = game.Players.Where(p => p.IsAlive).Except(playerHelper.Enemies);
				IEnumerable<IPublicPlayerView> allies = playerHelper.Allies;
				IEnumerable<IPublicPlayerView> enemies = playerHelper.Enemies;
				// First, look through the tables of non-allies:
				foreach(IPublicPlayerView p in nonAllies)
					availableCards.AddRange(p.Table.Where(c => c.Type != CardType.Jail));
				while(availableCards.Count != 0)
				{
					ICard best = cardHelper.BestCard(availableCards);
					if(TryRespondCard(best.ID))
						return;
					availableCards.Remove(best);
				}
				foreach(IPublicPlayerView p in allies)
					foreach(ICard c in p.Table)
						if(TryRespondCard(c.ID))
							return;

				// Then try random cards from the hands of enemies:
				foreach(IPublicPlayerView p in enemies)
					try
					{
						if(TryRespondCard(p.Hand.GetRandom().ID))
							return;
					}
					catch(InvalidOperationException)
					{
					}
				// If we can't take card from any enemy, let's change our mind:
				if(TryRespondNoAction())
					return;
				// If we have no choice (e. g. in Brawl), try the remaining players' cards:
				foreach(IPublicPlayerView p in allies)
					availableCards.AddRange(p.Table);
				while(availableCards.Count != 0)
				{
					ICard best = cardHelper.BestCard(availableCards);
					if(TryRespondCard(best.ID))
						return;
					availableCards.Remove(best);
				}

				foreach(IPublicPlayerView p in nonEnemies)
					try
					{
						if(TryRespondCard(p.Hand.GetRandom().ID))
							return;
					}
					catch(InvalidOperationException)
					{
					}
				break;
				#endregion
				#region TakeDeadPlayersCard
			case RequestType.TakeDeadPlayersCard:
				IPublicPlayerView dead = game.CausedBy;
				if(dead.Table.Count > 0)
				{
					control.RespondCard(cardHelper.BestCard(dead.Table).ID);
					return;
				}
				control.RespondCard(cardHelper.BestCard(dead.Hand).ID);
				return;
				#endregion
				#region GeneralStore
			case RequestType.KitCarlson:
			case RequestType.GeneralStore:
				control.RespondCard(cardHelper.BestCard(player.Selection).ID);
				return;
				#endregion
				#region JoseDelgado
			case RequestType.JoseDelgado:
				ICard worstCard = cardHelper.WorstCard(player.Hand.Where(c => c.Color == CardColor.Blue));
				if(worstCard != null)
				{
					control.RespondCard(worstCard.ID);
					return;
				}
				control.RespondNoAction();
				return;
				#endregion
				#region PatBrennan
			case RequestType.PatBrennan:
				availableCards = new List<ICard>();
				foreach(IPublicPlayerView p in game.Players.Except(playerHelper.Allies))
					if(p.ID != player.ID)
						availableCards.AddRange(p.Table);
				ICard bestCard = cardHelper.BestCard(availableCards);
				if(bestCard != null)
				{
					control.RespondCard(bestCard.ID);
					return;
				}
				control.RespondNoAction();
				return;
				#endregion
				#region VeraCuster
			case RequestType.VeraCuster:
				IPublicPlayerView bestPlayer = cardHelper.BestCharacter(game.Players.Where(p => p.IsAlive));
				control.RespondPlayer(bestPlayer.ID);
				return;
				#endregion
				#region LuckyDuke
			case RequestType.LuckyDuke:
				control.RespondCard(cardHelper.BestCheckDeckCard(player.Selection).ID);
				return;
				#endregion
				#region ChooseCharacter
			case RequestType.ChooseCharacterForDraw:
			case RequestType.ChooseCharacterForPlayCard:
			case RequestType.ChooseCharacterForCheckDeck:
			case RequestType.ChooseCharacterForAvoidShot:
				control.RespondCharacter(cardHelper.BestCharacter(player.AdditionalCharacters));
				return;
			case RequestType.ChooseCharacterForUseAbility:
				control.RespondCharacter(characterToUse);
				return;
				#endregion
			}
			Console.Error.WriteLine("ERROR: Reached the end of Bang.AI.AIPlayer.ProcessRequest()!");
		}
		#endregion

		#region IEventListener implementation
		void IEventListener.Ping()
		{
		}

		void IEventListener.OnSessionEnded()
		{
			sessionControl = null;
			control = null;
			playerHelper = null;
			cardHelper = null;
		}
		void IEventListener.OnGameEnded()
		{
			control = null;
			playerHelper = null;
			cardHelper = null;
		}

		void IEventListener.OnPlayerJoinedSession(IPlayer player) { }
		void IEventListener.OnSpectatorJoinedSession(ISpectator spectator) { }
		void IEventListener.OnPlayerLeftSession(IPlayer player) { }
		void IEventListener.OnSpectatorLeftSession(ISpectator spectator) { }
		void IEventListener.OnPlayerUpdated(IPlayer player) { }

		void IEventListener.OnChatMessage(IPlayer player, string message) { }
		void IEventListener.OnChatMessage(ISpectator spectator, string message) { }

		void IPlayerEventListener.OnNewRequest(RequestType requestType, IPublicPlayerView causedBy)
		{
			ThreadPool.QueueUserWorkItem(ProcessRequest);
		}

		void IEventListener.OnPlayerDrewFromDeck(IPublicPlayerView player, ReadOnlyCollection<ICard> drawnCards)
		{
			if(control == null)
				return;

			if(player.ID == control.PrivatePlayerView.ID)
			{
				triedCards.Clear();
				triedAbilities.Clear();
			}
		}
		void IEventListener.OnPlayerDrewFromGraveyard(IPublicPlayerView player, ReadOnlyCollection<ICard> drawnCards)
		{
			if(control == null)
				return;

			if(player.ID == control.PrivatePlayerView.ID)
			{
				triedCards.Clear();
				triedAbilities.Clear();
			}
		}
		void IEventListener.OnPlayerDiscardedCard(IPublicPlayerView player, ICard card) { }
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card) { }
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
			switch(card.Type)
			{
			case CardType.Bang:
			case CardType.BuffaloRifle:
			case CardType.Derringer:
			case CardType.Knife:
			case CardType.Pepperbox:
			case CardType.Punch:
			case CardType.Springfield:
				playerHelper.RegisterAttack(targetPlayer, player);
				break;
			}
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			switch(card.Type)
			{
			case CardType.CanCan:
			case CardType.CatBalou:
			case CardType.Conestoga:
			case CardType.Panic:
			case CardType.RagTime:
				if(targetCard.Type != CardType.Jail)
					playerHelper.RegisterAttack(targetPlayer, player);
				else
					playerHelper.RegisterHelp(targetPlayer, player);
				break;
			}
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard)
		{
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer)
		{
			switch(asCard)
			{
			case CardType.Bang:
			case CardType.BuffaloRifle:
			case CardType.Derringer:
			case CardType.Knife:
			case CardType.Pepperbox:
			case CardType.Punch:
			case CardType.Springfield:
				playerHelper.RegisterAttack(targetPlayer, player);
				break;
			}
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			switch(asCard)
			{
			case CardType.CanCan:
			case CardType.CatBalou:
			case CardType.Conestoga:
			case CardType.Panic:
			case CardType.RagTime:
				if(targetCard.Type != CardType.Jail)
					playerHelper.RegisterAttack(targetPlayer, player);
				else
					playerHelper.RegisterHelp(targetPlayer, player);
				break;
			}
		}
		void IEventListener.OnPlayerPlayedCardOnTable(IPublicPlayerView player, ICard card) { }
		void IEventListener.OnPassedTableCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
			if(control == null)
				return;

			if(card.Type == CardType.Jail)
				playerHelper.RegisterAttack(targetPlayer, player);
		}
		void IEventListener.OnPlayerPassed(IPublicPlayerView player) { }
		void IEventListener.OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card) { }
		void IEventListener.OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card, CardType asCard) { }
		void IEventListener.OnDrawnIntoSelection(ReadOnlyCollection<ICard> drawnCards) { }
		void IEventListener.OnPlayerPickedFromSelection(IPublicPlayerView player, ICard card) { }
		void IEventListener.OnUndrawnFromSelection(ICard card) { }
		void IEventListener.OnPlayerStoleCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			if(control == null)
				return;

			if(player.ID == control.PrivatePlayerView.ID)
			{
				triedCards.Clear();
				triedAbilities.Clear();
			}

			if(player.ID != targetPlayer.ID)
				if(targetCard.Type == CardType.Jail)
					playerHelper.RegisterHelp(targetPlayer, player);
				else
					playerHelper.RegisterAttack(targetPlayer, player);
		}
		void IEventListener.OnPlayerCancelledCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			if(control == null)
				return;

			if(player.ID != targetPlayer.ID)
				if(targetCard.Type == CardType.Jail)
					playerHelper.RegisterHelp(targetPlayer, player);
				else
					playerHelper.RegisterAttack(targetPlayer, player);
		}
		void IEventListener.OnDeckChecked(ICard card) { }
		void IEventListener.OnCardCancelled(ICard card) { }
		void IEventListener.OnPlayerCheckedDeck(IPublicPlayerView player, ICard checkedCard, CardType causedBy, bool result) { }
		void IEventListener.OnLifePointsChanged(IPublicPlayerView player, int delta, IPublicPlayerView causedBy)
		{
			if(control == null)
				return;

			if(causedBy != null && delta > 0)
				playerHelper.RegisterHelp(player, causedBy);
		}
		void IEventListener.OnPlayerDied(IPublicPlayerView player, IPublicPlayerView causedBy)
		{
			if(control == null)
				return;

			playerHelper.OnRoleRevealed(player);
		}
		void IEventListener.OnPlayerUsedAbility(IPublicPlayerView player, CharacterType character) { }
		void IEventListener.OnPlayerUsedAbility(IPublicPlayerView player, CharacterType character, IPublicPlayerView targetPlayer)
		{
			if(character == CharacterType.DocHolyday || character == CharacterType.JesseJones)
				playerHelper.RegisterAttack(player, targetPlayer);
		}
		void IEventListener.OnPlayerGainedAdditionalCharacters(IPublicPlayerView player) { }
		void IEventListener.OnPlayerLostAdditionalCharacters(IPublicPlayerView player) { }
		void IEventListener.OnDeckRegenerated() { }
		#endregion
	}
}
