// Player.cs
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
namespace Bang.Server
{
	public sealed class Player : ImmortalMarshalByRefObject, IPublicPlayerView
	{
		private sealed class PrivatePlayerViewProxy : ImmortalMarshalByRefObject, IPrivatePlayerView
		{
			private Player raw;

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
			ReadOnlyCollection<ICard> IPublicPlayerView.Hand
			{
				get { return ((IPublicPlayerView)raw).Hand; }
			}
			ReadOnlyCollection<ICard> IPublicPlayerView.Table
			{
				get { return ((IPublicPlayerView)raw).Table; }
			}
			CharacterType IPublicPlayerView.CharacterType
			{
				get { return raw.CharacterType; }
			}
			ReadOnlyCollection<CharacterType> IPublicPlayerView.AdditionalCharacters
			{
				get { return raw.AdditionalCharacters; }
			}
			Role IPublicPlayerView.Role
			{
				get { return ((IPublicPlayerView)raw).Role; }
			}

			ReadOnlyCollection<ICard> IPrivatePlayerView.Hand
			{
				get { return new ReadOnlyCollection<ICard>(raw.hand.ConvertAll<ICard>(c => c)); }
			}
			Role IPrivatePlayerView.Role
			{
				get { return raw.Role; }
			}
			RequestType IPrivatePlayerView.RequestType
			{
				get
				{
					GameCycle cycle = raw.game.GameCycle;
					if(cycle.RequestedPlayer != raw || raw.game.Ended)
						return RequestType.None;
					return cycle.RequestType;
				}
			}
			ReadOnlyCollection<ICard> IPrivatePlayerView.Selection
			{
				get { return raw.game.GameTable.GetSelection(raw); }
			}

			public PrivatePlayerViewProxy(Player raw)
			{
				this.raw = raw;
			}
		}
		private Game game;
		private SessionPlayer parent;
		private PlayerControl control;
		private Role role;
		private Character character;
		private List<CharacterType> additionalCharacters;
		private int lifePoints;
		private List<Card> hand;
		private List<TableCard> table;
		private bool isAlive;
		private bool isWinner;
		private int bangsPlayed;
		private bool skipTurn;
		private int turnsPlayed;
		private PrivatePlayerViewProxy privateView;
		
		public Game Game
		{
			get { return game; }
		}
		public SessionPlayer Parent
		{
			get { return parent; }
		}
		public int ID
		{
			get { return parent.ID; }
		}
		public PlayerControl Control
		{
			get { return control; }
		}
		public int LifePoints
		{
			get { return lifePoints; }
		}
		public int MaxLifePoints
		{
			get { return role == Role.Sheriff ? character.MaxLifePoints + 1 : character.MaxLifePoints; }
		}
		public ReadOnlyCollection<Card> Hand
		{
			get { return new ReadOnlyCollection<Card>(hand); }
		}
		ReadOnlyCollection<ICard> IPublicPlayerView.Hand
		{
			get { return new ReadOnlyCollection<ICard> (hand.ConvertAll<ICard>(c => c.Empty)); }
		}
		public ReadOnlyCollection<TableCard> Table
		{
			get { return new ReadOnlyCollection<TableCard> (table); }
		}
		ReadOnlyCollection<ICard> IPublicPlayerView.Table
		{
			get { return new ReadOnlyCollection<ICard> (table.ConvertAll<ICard>(c => c)); }
		}
		public Role Role
		{
			get { return role; }
		}
		Role IPublicPlayerView.Role
		{
			get
			{
				if (game.Players.Count <= 3)
					return role;
				else
					return role == Role.Sheriff ? Role.Sheriff : isAlive ? Role.Unknown : role;
			}
		}
		public bool IsSheriff
		{
			get { return role == Role.Sheriff; }
		}
		public bool BeginsRound
		{
			get { return game.Players.Count == 3 ? role == Role.Deputy : role == Role.Sheriff; }
		}
		public CharacterType CharacterType
		{
			get { return character.Type; }
		}
		public ReadOnlyCollection<CharacterType> AdditionalCharacters
		{
			get { return new ReadOnlyCollection<CharacterType>(additionalCharacters); }
		}
		public Character Character
		{
			get { return character; }
		}
		public bool IsAlive
		{
			get { return isAlive; }
			set { isAlive = value; }
		}
		public bool IsWinner
		{
			get { return isWinner; }
			set { isWinner = value; }
		}
		public int InitialCardCount
		{
			get { return character.InitialCardCount; }
		}
		public int MaxCardCount
		{
			get { return character.MaxCardCount; }
		}
		public int WeaponRange
		{
			get { return table.Count == 0 ? 1 : table.Max(c => c.WeaponRange); }
		}
		public int BangsPlayed
		{
			get { return bangsPlayed; }
		}
		public int BangPower
		{
			get { return character.BangPower; }
		}
		public int BeerPower
		{
			get { return character.BeerPower; }
		}
		public bool UnlimitedBangs
		{
			get { return table.Any(c => c.UnlimitedBangs) || character.UnlimitedBangs; }
		}
		public int TurnsPlayed
		{
			get { return turnsPlayed; }
		}
		public bool SkipTurn
		{
			get { return skipTurn; }
			set { skipTurn = skipTurn || value; }
		}
		public IPrivatePlayerView PrivatePlayerView
		{
			get { return privateView; }
		}
		
		public Player(Game game, SessionPlayer parent, Role role, CharacterType character)
		{
			this.game = game;
			this.parent = parent;
			control = new PlayerControl(this);
			this.role = role;
			this.character = Character.GetCharacter(this, character);
			additionalCharacters = new List<CharacterType>();
			lifePoints = MaxLifePoints;
			hand = new List<Card>();
			table = new List<TableCard>();
			isAlive = true;
			isWinner = false;
			bangsPlayed = 0;
			turnsPlayed = 0;
			privateView = new PrivatePlayerViewProxy(this);
		}

		public override void Disconnect()
		{
			base.Disconnect();
			privateView.Disconnect();
			control.Disconnect();
		}

		public void ResetControl()
		{
			control.Disconnect();
			control = new PlayerControl(this);
		}
		
		public void PlayCard (Card card)
		{
			character.PlayCard (card);
		}
		public void CheckDeck (Card causedBy, CheckDeckMethod checkMethod, CardResultMethod resultMethod)
		{
			character.CheckDeck (causedBy, checkMethod, resultMethod);
		}
		public int GetDistanceIn(Player origin)
		{
			int dist = 0;
			foreach(Card c in table)
				if(c.DistanceIn != 0 && origin.HasCardEffect(c))
					dist += c.DistanceIn;
			dist += character.DistanceIn;
			return dist;
		}
		public int GetDistanceOut (Player target)
		{
			int dist = 0;
			foreach (Card c in table)
				if(c.DistanceOut != 0 && target.HasCardEffect(c))
					dist += c.DistanceOut;
			dist += character.DistanceOut;
			return dist;
		}
		public bool HasCardEffect (Card card)
		{
			return character.HasCardEffect (card);
		}
		public void CheckMissed(Card card, CardResultMethod resultMethod)
		{
			card.CheckMissed((causedBy, result) => resultMethod(causedBy, result || character.IsMissed(card)));
		}
		public bool IsBang (Card card)
		{
			return character.IsBang (card);
		}
		public void CheckPlayCard(CardType card)
		{
			if(card == CardType.Bang && !UnlimitedBangs && bangsPlayed >= game.MaxBangs)
				throw new CannotPlayBangException();

			if(card == CardType.Beer && hitPoints == 0 && game.AlivePlayersCount == 2 && game.Players.Count != 2)
				throw new CannotPlayBeerException();

			if(!character.CanPlayCard(card))
				throw new BadCardException();
		}
		
		private sealed class BeerRescue : ResponseHandler
		{
			public BeerRescue (Player player, Player causedBy)
				: base(RequestType.BeerRescue, player, causedBy)
			{
			}

			protected override void OnRespondCard (Card card)
			{
				if (card.Owner != RequestedPlayer)
					throw new BadCardException ();
				card.AssertInHand ();
				if (card.Type != CardType.Beer)
					throw new BadCardException ();

				card.Play();
				End();
			}
			protected override void OnRespondNoAction ()
			{
				Game.PlayerDied (RequestedPlayer, CausedBy);
				End ();
			}
			protected override void OnRespondUseAbility()
			{
				if(RequestedPlayer.Character.SavePlayer())
				{
					RequestedPlayer.ModifyLifePoints(1);
					End();
				}
				else
				{
					Game.PlayerDied (RequestedPlayer, CausedBy);
					End ();
				}
			}
		}
		private int hitPoints = 0;
		private Player causedBy = null;
		public void ModifyLifePoints(int delta, Player causedBy)
		{
			int oldLifePoints = lifePoints;
			lifePoints += delta;
			if(lifePoints > MaxLifePoints)
				lifePoints = MaxLifePoints;

			delta = lifePoints - oldLifePoints;
			if(delta != 0)
				game.Session.EventManager.OnLifePointsChanged(this, delta, causedBy);

			if(hitPoints == 0)
			{
				hitPoints = -delta;
				this.causedBy = causedBy;
			}

			if(lifePoints <= 0)
				Game.GameCycle.PushTempHandler(new BeerRescue(this, this.causedBy));
			else
			{
				int temp = hitPoints;
				hitPoints = 0;
				if(temp > 0)
					character.OnHit(temp, this.causedBy);
			}
		}
		public void ModifyLifePoints(int delta)
		{
			ModifyLifePoints(delta, null);
		}
		
		public void AddCardToHand (Card card)
		{
			if(card == null)
				return;
			card.Owner = this;
			hand.Add(card);
		}
		public void AddCardToTable (TableCard card)
		{
			if(card == null)
				return;
			card.Owner = this;
			table.Add (card);
		}
		public bool RemoveCardFromHand(Card card)
		{
			return hand.Remove (card);
		}
		public bool RemoveCardFromTable(TableCard card)
		{
			return table.Remove(card);
		}

		public void ClearAdditionalCharacters()
		{
			additionalCharacters.Clear();
			game.Session.EventManager.OnPlayerLostAdditionalCharacters(this);
		}
		public void SetAditionalCharacters(IEnumerable<CharacterType> characters)
		{
			if(additionalCharacters.Count != 0)
				ClearAdditionalCharacters();
			additionalCharacters.AddRange(characters);
			game.Session.EventManager.OnPlayerGainedAdditionalCharacters(this);
		}

		public void CheckEmptyHand()
		{
			if(Hand.Count == 0)
				character.OnEmptyHand();
		}
		public void OnTurnStarted()
		{
			character.OnTurnStarted();
		}
		public void OnTurnEnded()
		{
			skipTurn = false;
			bangsPlayed = 0;
			turnsPlayed++;
			character.OnTurnEnded();
			foreach(TableCard card in table)
				card.OnTurnEnded();
		}
		public void OnPlayedBang()
		{
			bangsPlayed++;
		}
		public void OnPlayedCard (Card card)
		{
			character.OnPlayedCard(card);
		}
		public void OnRespondedWithCard(Card card)
		{
			character.OnRespondedWithCard(card);
		}
	}
}

