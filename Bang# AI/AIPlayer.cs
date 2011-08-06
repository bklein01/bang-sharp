// AIPlayer.cs
//  
// Author:  WOnder93 <omosnacek@gmail.com>
// 
// Copyright (c) 2011 Ondrej Mosnáček
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
namespace Bang.AI
{
	/// <summary>
	/// This class provides the AI player functionality.
	/// </summary>
	public sealed class AIPlayer : MarshalByRefObject, IPlayerEventListener
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
		private bool triedAbility;

		/// <summary>
		/// Gets this AI player's <see cref="CreatePlayerData"/>. Can be ignored.
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
			data = new CreatePlayerData(NextName(), null, "asimov");
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
			triedAbility = false;
		}

		void IPlayerEventListener.OnResponseRequested()
		{
			if(control == null)
				return;

			IGame game = control.Game;
			IPrivatePlayerView player = control.PrivatePlayerView;

			switch(game.RequestType)
			{
			case RequestType.Draw:
				switch(cardHelper.Character)
				{
				case CharacterType.BillNoface:
					if(player.MaxLifePoints - player.LifePoints > 2)
						control.RespondUseAbility();
					else
						control.RespondDraw();
					return;
				case CharacterType.JesseJones:
					if(game.Players.Any(p => p.ID != player.ID && p.Hand.Count > 0))
						control.RespondUseAbility();
					else
						control.RespondDraw();
					return;
				case CharacterType.PatBrennan:
					if(game.Players.Except(playerHelper.Allies).Any(p => p.Table.Any(c => cardHelper.IsCardWorthSkippingDraw(c.Type))))
						control.RespondUseAbility();
					else
						control.RespondDraw();
					return;
				case CharacterType.PedroRamirez:
					if(game.GraveyardTop != null && cardHelper.IsCardWorthSkippingDraw(game.GraveyardTop.Type))
						control.RespondUseAbility();
					else
						control.RespondDraw();
					return;
				case CharacterType.BlackJack:
				case CharacterType.KitCarlson:
				case CharacterType.PixiePete:
					control.RespondUseAbility();
					return;
				default:
					control.RespondDraw();
					return;
				}
			case RequestType.Play:
				// First, play cards that let us gain cards:
				try
				{
					switch(cardHelper.Character)
					{
					case CharacterType.ChuckWengam:
						if(player.LifePoints > 2)
						{
							control.RespondUseAbility();
							return;
						}
						break;
					case CharacterType.JoseDelgado:
						if(player.Hand.Any(c => c.Color == CardColor.Blue && !cardHelper.IsTableCardWorth(c.Type)))
						{
							control.RespondUseAbility();
							return;
						}
						break;
					}
				}
				catch(GameException)
				{
				}
				foreach(ICard c in player.Hand)
					try
					{
						switch(c.Type)
						{
						case CardType.WellsFargo:
						case CardType.Diligenza:
							control.RespondCard(c.ID);
							return;
						}
					}
					catch(GameException)
					{
					}
				foreach(ICard c in player.Table)
					try
					{
						if(c.Type == CardType.PonyExpress)
						{
							control.RespondCard(c.ID);
							return;
						}
					}
					catch(GameException)
					{
					}
				// Now get all the blue & green cards on table:
				foreach(ICard card in player.Hand)
					try
					{
						switch(card.Color)
						{
						case CardColor.Green:
							control.RespondCard(card.ID);
							return;
						case CardColor.Blue:
							switch(card.Type)
							{
							case CardType.Jail:
								if(!triedCards.Contains(card.ID))
								{
									control.RespondCard(card.ID);
									triedCards.Add(card.ID);
									return;
								}
								break;
							case CardType.Appaloosa:
								if(!player.Table.Any(c => c.Type == CardType.Appaloosa))
								{
									control.RespondCard(card.ID);
									return;
								}
								break;
							case CardType.Silver:
								if(!player.Table.Any(c => c.Type == CardType.Silver))
								{
									control.RespondCard(card.ID);
									return;
								}
								break;
							case CardType.Mustang:
								if(!player.Table.Any(c => c.Type == CardType.Mustang))
								{
									control.RespondCard(card.ID);
									return;
								}
								break;
							case CardType.Hideout:
								if(!player.Table.Any(c => c.Type == CardType.Hideout))
								{
									control.RespondCard(card.ID);
									return;
								}
								break;
							case CardType.Barrel:
								if(!player.Table.Any(c => c.Type == CardType.Barrel))
								{
									control.RespondCard(card.ID);
									return;
								}
								break;
							case CardType.Volcanic:
								if(cardHelper.Character != CharacterType.WillyTheKid)
									if(!player.Table.Any(c => c.Type == CardType.Volcanic || c.Type == CardType.Carabine || c.Type == CardType.Winchester))
									{
										control.RespondCard(card.ID);
										return;
									}
								break;
							case CardType.Schofield:
								if(player.Table.Any(c => c.Type == CardType.Volcanic))
								{
									if(cardHelper.Character == CharacterType.WillyTheKid)
									{
										control.RespondCard(card.ID);
										return;
									}
								}
								else if(!player.Table.Any(c => c.Type == CardType.Schofield || c.Type == CardType.Remington || c.Type == CardType.Carabine || c.Type == CardType.Winchester))
								{
									control.RespondCard(card.ID);
									return;
								}
								break;
							case CardType.Remington:
								if(player.Table.Any(c => c.Type == CardType.Volcanic))
								{
									if(cardHelper.Character == CharacterType.WillyTheKid)
									{
										control.RespondCard(card.ID);
										return;
									}
								}
								else if(!player.Table.Any(c => c.Type == CardType.Remington || c.Type == CardType.Carabine || c.Type == CardType.Winchester))
								{
									control.RespondCard(card.ID);
									return;
								}
								break;
							case CardType.Carabine:
								if(player.Table.Any(c => c.Type == CardType.Volcanic))
								{
									if(cardHelper.Character == CharacterType.WillyTheKid)
									{
										control.RespondCard(card.ID);
										return;
									}
								}
								else if(!player.Table.Any(c => c.Type == CardType.Carabine || c.Type == CardType.Winchester))
								{
									control.RespondCard(card.ID);
									return;
								}
								break;
							case CardType.Winchester:
								if(player.Table.Any(c => c.Type == CardType.Volcanic))
								{
									if(cardHelper.Character == CharacterType.WillyTheKid)
									{
										control.RespondCard(card.ID);
										return;
									}
								}
								else if(!player.Table.Any(c => c.Type == CardType.Winchester))
								{
									control.RespondCard(card.ID);
									return;
								}
								break;
							}
							break;
						}
					}
					catch(GameException)
					{
					}
				// Then heal our and allies' lives:
				if(player.LifePoints < player.MaxLifePoints)
				{
					// First, try playing canteen and beers:
					foreach(ICard c in player.Table)
						try
						{
							if(c.Type == CardType.Canteen)
							{
								control.RespondCard(c.ID);
								return;
							}
						}
						catch(GameException)
						{
						}
					foreach(ICard c in player.Hand)
						try
						{
							if(c.Type == CardType.Beer)
							{
								control.RespondCard(c.ID);
								return;
							}
						}
						catch(GameException)
						{
						}
					// If we have the Sid Ketchum cardHelper.Character, try to use his ability:
					if(cardHelper.Character == CharacterType.SidKetchum && !triedAbility)
						try
						{
							control.RespondUseAbility();
							triedAbility = true;
							return;
						}
						catch(GameException)
						{
						}
					// Then search for whisky and tequila:
					if(cardHelper.DiscardableCards >= 1)
					{
						// First, play whisky only if the life deficit is at least 2:
						if(player.MaxLifePoints - player.LifePoints >= 2)
							foreach(ICard c in player.Hand)
								try
								{
									if(c.Type == CardType.Whisky && !triedCards.Contains(c.ID))
									{
										control.RespondCard(c.ID);
										triedCards.Add(c.ID);
										return;
									}
								}
								catch(GameException)
								{
								}
						// Then try to play tequila on self:
						foreach(ICard c in player.Hand)
							try
							{
								if(c.Type == CardType.Tequila && !triedCards.Contains(c.ID))
								{
									control.RespondCard(c.ID);
									triedCards.Add(c.ID);
									return;
								}
							}
							catch(GameException)
							{
							}
						// Finally play whisky regardless the life deficit:
						foreach(ICard c in player.Hand)
							try
							{
								if(c.Type == CardType.Whisky && !triedCards.Contains(c.ID))
								{
									control.RespondCard(c.ID);
									triedCards.Add(c.ID);
									return;
								}
							}
							catch(GameException)
							{
							}
					}
					// If we have only 1 life remaining, try also saloon:
					if(player.LifePoints == 1)
						foreach(ICard c in player.Hand)
							try
							{
								if(c.Type == CardType.Saloon)
								{
									control.RespondCard(c.ID);
									return;
								}
							}
							catch(GameException)
							{
							}
				}
				// And now we try to heal our allies' lives:
				int alliesCount = playerHelper.Allies.Count(p => p.LifePoints < p.MaxLifePoints);
				int enemiesCount = playerHelper.Enemies.Count(p => p.LifePoints < p.MaxLifePoints);
				// Try saloon if it doesn't make more harm than good:
				if(alliesCount > enemiesCount)
					foreach(ICard c in player.Hand)
						try
						{
							if(c.Type == CardType.Saloon)
							{
								control.RespondCard(c.ID);
								return;
							}
						}
						catch(GameException)
						{
						}
				// Try tequila:
				if(alliesCount >= 1 && cardHelper.DiscardableCards >= 1)
					foreach(ICard c in player.Hand)
						try
						{
							if(c.Type == CardType.Tequila && !triedCards.Contains(c.ID))
							{
								control.RespondCard(c.ID);
								triedCards.Add(c.ID);
								return;
							}
						}
						catch(GameException)
						{
						}
				// Then steal and cancel cards:
				foreach(ICard c in player.Hand)
					try
					{
						if(!triedCards.Contains(c.ID))
							switch(c.Type)
							{
							case CardType.Panic:
							case CardType.CatBalou:
								control.RespondCard(c.ID);
								triedCards.Add(c.ID);
								return;
							case CardType.Brawl:
							case CardType.RagTime:
								if(cardHelper.DiscardableCards >= 1)
								{
									control.RespondCard(c.ID);
									triedCards.Add(c.ID);
									return;
								}
								break;
							}
					}
					catch(GameException)
					{
					}
				foreach(ICard c in player.Table)
					try
					{
						if(!triedCards.Contains(c.ID))
							switch(c.Type)
							{
							case CardType.Conestoga:
							case CardType.CanCan:
								control.RespondCard(c.ID);
								triedCards.Add(c.ID);
								return;
							}
					}
					catch(GameException)
					{
					}
				// Finally, attack:
				foreach(ICard card in player.Hand)
					try
					{
						if(!triedCards.Contains(card.ID))
							switch(card.Type)
							{
							case CardType.Bang:
							case CardType.Indians:
							case CardType.Duel:
							case CardType.Gatling:
							case CardType.Punch:
							case CardType.Knife:
							case CardType.Derringer:
							case CardType.Howitzer:
							case CardType.Pepperbox:
							case CardType.Springfield:
								control.RespondCard(card.ID);
								triedCards.Add(card.ID);
								return;
							case CardType.Missed:
								if(cardHelper.Character == CharacterType.CalamityJanet && player.Hand.Count(c => c.Type == CardType.Missed) >= 2)
								{
									control.RespondCard(card.ID);
									triedCards.Add(card.ID);
									return;
								}
								break;
							}
					}
					catch(GameException)
					{
					}
				foreach(ICard c in player.Table)
					try
					{
						if(!triedCards.Contains(c.ID))
							switch(c.Type)
							{
							case CardType.Derringer:
							case CardType.Howitzer:
							case CardType.Pepperbox:
							case CardType.BuffaloRifle:
								control.RespondCard(c.ID);
								triedCards.Add(c.ID);
								return;
							}
					}
					catch(GameException)
					{
					}
				if(cardHelper.Character == CharacterType.DocHolyday && !triedAbility)
					try
					{
						control.RespondUseAbility();
						triedAbility = true;
						return;
					}
					catch(GameException)
					{
					}
	
				// End the play stage:
				control.RespondNoAction();
				triedCards.Clear();
				triedAbility = false;
				return;
			case RequestType.DiscardCard:
			case RequestType.SidKetchum:
			case RequestType.GoldenCard:
				List<ICard> availableCards = new List<ICard>(player.Hand);
				while(availableCards.Count != 0)
				{
					ICard worst = cardHelper.WorstCard(availableCards);
					try
					{
						control.RespondCard(worst.ID);
						return;
					}
					catch(GameException)
					{
					}
					availableCards.Remove(worst);
				}
				control.RespondNoAction();
				return;
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
					catch(BadUsageException)
					{
						break;
					}
					availableCards.Remove(worst);
				}

				// Try only enemies then respond with no action:
				foreach(IPublicPlayerView enemy in playerHelper.Enemies)
					try
					{
						control.RespondPlayer(enemy.ID);
						return;
					}
					catch(GameException)
					{
					}
				control.RespondNoAction();
				return;
			case RequestType.BeerRescue:
				try
				{
					control.RespondCard(player.Hand.First(c => c.Type == CardType.Beer).ID);
					return;
				}
				catch(InvalidOperationException)
				{
				}
				control.RespondNoAction();
				return;
			case RequestType.Shot:
				// First try to use ability:
				try
				{
					control.RespondUseAbility();
					return;
				}
				catch(GameException)
				{
				}
	
					// Then try barrels:
				foreach(ICard c in player.Table)
					if(c.Type == CardType.Barrel)
						try
						{
							control.RespondCard(c.ID);
							return;
						}
						catch(GameException)
						{
						}
	
					// Then seek for a Bible:
				foreach(ICard c in player.Table)
					if(c.Type == CardType.Bible)
						try
						{
							control.RespondCard(c.ID);
							return;
						}
						catch(GameException)
						{
						}
	
					// Then seek for a Dodge:
				foreach(ICard c in player.Hand)
					if(c.Type == CardType.Dodge)
						try
						{
							control.RespondCard(c.ID);
							return;
						}
						catch(GameException)
						{
						}
	
					// Then try other table cards:
				foreach(ICard c in player.Table)
					try
					{
						control.RespondCard(c.ID);
						return;
					}
					catch(GameException)
					{
					}
	
					// Then try Missed:
				foreach(ICard c in player.Hand)
					if(c.Type == CardType.Missed)
						try
						{
							control.RespondCard(c.ID);
							return;
						}
						catch(GameException)
						{
						}
	
					// Eventually try other hand cards (Elena Fuente):
				foreach(ICard c in player.Hand)
					try
					{
						control.RespondCard(c.ID);
						return;
					}
					catch(GameException)
					{
					}
				control.RespondNoAction();
				return;
			case RequestType.ThrowBang:
				// Try all hand cards:
				foreach(ICard c in player.Hand)
					try
					{
						control.RespondCard(c.ID);
						return;
					}
					catch(GameException)
					{
					}
				control.RespondNoAction();
				return;
			case RequestType.ShotTarget:
			case RequestType.DuelTarget:
			case RequestType.JailTarget:
				// Try only enemies then respond with no action:
				foreach(IPublicPlayerView enemy in playerHelper.Enemies)
					try
					{
						control.RespondPlayer(enemy.ID);
						return;
					}
					catch(GameException)
					{
					}
				control.RespondNoAction();
				return;
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
				if(poorestAlly != null)
				{
					control.RespondPlayer(poorestAlly.ID);
					return;
				}
				control.RespondNoAction();
				return;
			case RequestType.StealCard:
			case RequestType.CancelCard:
				{
					List<ICard> cards = new List<ICard>();
					IEnumerable<IPublicPlayerView> nonAllies = game.Players.Where(p => p.IsAlive).Except(playerHelper.Allies);
					IEnumerable<IPublicPlayerView> nonEnemies = game.Players.Where(p => p.IsAlive).Except(playerHelper.Enemies);
					IEnumerable<IPublicPlayerView> allies = playerHelper.Allies;
					IEnumerable<IPublicPlayerView> enemies = playerHelper.Enemies;
					// First, look through the tables of non-allies:
					foreach(IPublicPlayerView p in nonAllies)
						cards.AddRange(p.Table);
					while(cards.Count != 0)
					{
						ICard best = cardHelper.BestCard(cards);
						cards.Remove(best);
						try
						{
							control.RespondCard(best.ID);
							return;
						}
						catch(GameException)
						{
						}
					}
					foreach(IPublicPlayerView p in allies)
						try
						{
							control.RespondCard(p.Table.First(c => c.Type == CardType.Jail).ID);
							return;
						}
						catch(InvalidOperationException)
						{
						}
						catch(GameException)
						{
						}
	
					// Then try random cards from the hands of enemies:
					foreach(IPublicPlayerView p in enemies)
						try
						{
							control.RespondCard(p.Hand.GetRandom().ID);
							return;
						}
						catch(InvalidOperationException)
						{
						}
						catch(GameException)
						{
						}
					// If we can't take card from any enemy, let's change our mind:
					try
					{
						control.RespondNoAction();
						return;
					}
					catch(GameException)
					{
					}
					// If we have no choice (e. g. in Brawl), try the remaining players' cards:
					foreach(IPublicPlayerView p in allies)
						cards.AddRange(p.Table);
					while(cards.Count != 0)
					{
						ICard best = cardHelper.BestCard(cards);
						cards.Remove(best);
						try
						{
							control.RespondCard(best.ID);
							return;
						}
						catch(GameException)
						{
						}
					}
	
					foreach(IPublicPlayerView p in nonEnemies)
						try
						{
							control.RespondCard(p.Hand.GetRandom().ID);
							return;
						}
						catch(InvalidOperationException)
						{
						}
						catch(GameException)
						{
						}
				}
				break;
			case RequestType.TakeDeadPlayersCard:
				IPublicPlayerView dead = game.CausedBy;
				if(dead.Table.Count > 0)
				{
					control.RespondCard(cardHelper.BestCard(dead.Table).ID);
					return;
				}
				control.RespondCard(cardHelper.BestCard(dead.Hand).ID);
				return;
			case RequestType.KitCarlson:
			case RequestType.GeneralStore:
				control.RespondCard(cardHelper.BestCard(player.Selection).ID);
				return;
			case RequestType.JoseDelgado:
				try
				{
					control.RespondCard(cardHelper.WorstCard(player.Hand.Where(c => c.Color == CardColor.Blue)).ID);
					return;
				}
				catch(InvalidOperationException)
				{
				}
				control.RespondNoAction();
				return;
			case RequestType.PatBrennan:
				{
					List<ICard> cards = new List<ICard>();
					foreach(IPublicPlayerView p in game.Players.Except(playerHelper.Allies))
						if(p.ID != player.ID)
							cards.AddRange(p.Table);
					ICard best = cardHelper.BestCard(cards);
					if(best != null)
					{
						control.RespondCard(best.ID);
						return;
					}
					break;
				}
			case RequestType.VeraCuster:
				IPublicPlayerView best = cardHelper.BestCharacter(game.Players.Where(p => p.IsAlive));
				control.RespondPlayer(best.ID);
				cardHelper.Character = best.CharacterType;
				return;
			case RequestType.LuckyDuke:
				control.RespondCard(cardHelper.BestCheckDeckCard(player.Selection).ID);
				return;
			}
			throw new InvalidOperationException();
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
		void IEventListener.OnGameEnded ()
		{
			control = null;
			playerHelper = null;
			cardHelper = null;
		}

		void IEventListener.OnPlayerJoinedSession (IPlayer player) { }
		void IEventListener.OnSpectatorJoinedSession (ISpectator spectator) { }
		void IEventListener.OnPlayerLeftSession (IPlayer player) { }
		void IEventListener.OnSpectatorLeftSession (ISpectator spectator) { }
		void IEventListener.OnPlayerUpdated (IPlayer player) { }

		void IEventListener.OnChatMessage(IPlayer player, string message) { }
		void IEventListener.OnChatMessage(ISpectator spectator, string message) { }

		void IEventListener.OnNewRequest(RequestType requestType, IPublicPlayerView requestedPlayer, IPublicPlayerView causedBy)
		{
			if(causedBy != null && requestType == RequestType.Shot)
				playerHelper.RegisterAttack(requestedPlayer, causedBy);
		}

		void IEventListener.OnPlayerDrewFromDeck(IPublicPlayerView player, ReadOnlyCollection<ICard> drawnCards)
		{
			if(control == null)
				return;

			if(player.ID == control.PrivatePlayerView.ID)
			{
				triedCards.Clear();
				triedAbility = false;
			}
		}
		void IEventListener.OnPlayerDrewFromGraveyard (IPublicPlayerView player, ReadOnlyCollection<ICard> drawnCards)
		{
			if(control == null)
				return;

			if(player.ID == control.PrivatePlayerView.ID)
			{
				triedCards.Clear();
				triedAbility = false;
			}
		}
		void IEventListener.OnPlayerDiscardedCard(IPublicPlayerView player, ICard card)
		{
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card)
		{
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
		}
		void IEventListener.OnPlayerPlayedCard (IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer, ICard targetCard)
		{
		}
		void IEventListener.OnPlayerPlayedCard (IPublicPlayerView player, ICard card, CardType asCard)
		{
		}
		void IEventListener.OnPlayerPlayedCard (IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer)
		{
		}
		void IEventListener.OnPlayerPlayedCard (IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer, ICard targetCard)
		{
		}
		void IEventListener.OnPlayerPlayedCardOnTable (IPublicPlayerView player, ICard card)
		{
		}
		void IEventListener.OnPassedTableCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
			if(control == null)
				return;

			if(card.Type == CardType.Jail)
				playerHelper.RegisterAttack(targetPlayer, player);
		}
		void IEventListener.OnPlayerPassed (IPublicPlayerView player)
		{
		}
		void IEventListener.OnPlayerRespondedWithCard (IPublicPlayerView player, ICard card)
		{
		}
		void IEventListener.OnPlayerRespondedWithCard (IPublicPlayerView player, ICard card, CardType asCard)
		{
		}
		void IEventListener.OnDrawnIntoSelection (ReadOnlyCollection<ICard> drawnCards)
		{
		}
		void IEventListener.OnPlayerPickedFromSelection (IPublicPlayerView player, ICard card)
		{
		}
		void IEventListener.OnUndrawnFromSelection (ICard card)
		{
		}
		void IEventListener.OnPlayerStoleCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			if(control == null)
				return;

			if(player.ID == control.PrivatePlayerView.ID)
			{
				triedCards.Clear();
				triedAbility = false;
			}

			if(player.ID != targetPlayer.ID)
				if(targetCard.Type == CardType.Jail)
					playerHelper.RegisterHelp(targetPlayer, player);
				else
					playerHelper.RegisterAttack(targetPlayer, player);
		}
		void IEventListener.OnPlayerCancelledCard (IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			if(control == null)
				return;

			if(player.ID != targetPlayer.ID)
				if(targetCard.Type == CardType.Jail)
					playerHelper.RegisterHelp(targetPlayer, player);
				else
					playerHelper.RegisterAttack(targetPlayer, player);
		}
		void IEventListener.OnDeckChecked (ICard card)
		{
		}
		void IEventListener.OnCardCancelled (ICard card)
		{
		}
		void IEventListener.OnPlayerCheckedDeck (IPublicPlayerView player, ICard checkedCard, CardType causedBy, bool result)
		{
		}
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
		void IEventListener.OnPlayerUsedAbility (IPublicPlayerView player)
		{
		}
		void IEventListener.OnDeckRegenerated()
		{
		}
		#endregion
	}
}

