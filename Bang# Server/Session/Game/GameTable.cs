// GameTable.cs
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
namespace Bang.Server
{
	public sealed class GameTable
	{
		private Game game;
		private Dictionary<int, Card> cards;
		private Stack<Card> graveyard;
		private Stack<Card> deck;
		private List<Card> selection;
		private Player selectionOwner;
		
		public Card GraveyardTop
		{
			get { return graveyard.Count == 0 ? null : graveyard.Peek(); }
		}
		public ReadOnlyCollection<Card> Selection
		{
			get { return new ReadOnlyCollection<Card>(selection); }
		}
		public ReadOnlyCollection<ICard> GetSelection(Player player)
		{
			if(player == selectionOwner || selectionOwner == null)
				return new ReadOnlyCollection<ICard>(selection.ConvertAll<ICard>(c => new CardProxy(c)));
			else
				return new ReadOnlyCollection<ICard>(selection.ConvertAll<ICard>(c => c.Empty));
		}

		public Game Game
		{
			get { return game; }
		}
		
		public GameTable (Game game)
		{
			this.game = game;
			selection = new List<Card>();
			GenerateCards ();
		}
		
		public Card GetCard (int id)
		{
			try {
				return cards[id];
			} catch (KeyNotFoundException) {
				throw new InvalidIdException ();
			}
		}
		
		public void Deal ()
		{
			bool repeat;
			do
			{
				repeat = false;
				foreach (Player p in game.Players)
				{
					int d = p.InitialCardCount - p.Hand.Count;
					if (d > 0)
					{
						PlayerDrawFromDeck (p, d >= 2 ? 2 : d);
						repeat = true;
					}
				}
			}
			while(repeat);
		}
		
		public ReadOnlyCollection<Card> PlayerDrawFromDeck (Player player, int count, bool revealCards)
		{
			List<Card> drawn = new List<Card> (count);
			for (int i = 0; i < count; i++)
			{
				Card card = PopCardFromDeck ();
				player.AddCardToHand (card);
				drawn.Add (card);
			}
			game.Session.EventManager.OnPlayerDrewFromDeck(player, drawn, revealCards);
			return new ReadOnlyCollection<Card> (drawn);
		}
		public ReadOnlyCollection<Card> PlayerDrawFromDeck (Player player, int count)
		{
			return PlayerDrawFromDeck (player, count, false);
		}
		public ReadOnlyCollection<Card> PlayerDrawFromGraveyard(Player player, int count)
		{
			List<Card> drawn = new List<Card>(count);
			for(int i = 0; i < count; i++)
			{
				Card card = PopCardFromGraveyard();
				player.AddCardToHand(card);
				drawn.Add(card);
			}
			game.Session.EventManager.OnPlayerDrewFromGraveyard(player, drawn);
			return new ReadOnlyCollection<Card>(drawn);
		}
		public void PlayerDiscardCard (Card card)
		{
			Player owner = card.Owner;
			if (owner == null)
				throw new InvalidOperationException ();
			
			if (!owner.RemoveCardFromHand (card))
				if (!owner.RemoveCardFromTable (card as TableCard))
					throw new InvalidOperationException ();
			PutCardToGraveyard(card);
			
			game.Session.EventManager.OnPlayerDiscardedCard (owner, card);
			owner.CheckEmptyHand();
		}
		public void PlayerPlayCard (Card card)
		{
			Player owner = card.Owner;
			if (owner == null)
				throw new InvalidOperationException ();
			
			if (!owner.RemoveCardFromHand (card))
				if (!owner.RemoveCardFromTable (card as TableCard))
					throw new InvalidOperationException ();
			PutCardToGraveyard (card);
			
			game.Session.EventManager.OnPlayerPlayedCard (owner, card);
			owner.OnPlayedCard (card);
			owner.CheckEmptyHand ();
		}
		public void PlayerPlayCard (Card card, Player targetPlayer)
		{
			Player owner = card.Owner;
			if (owner == null)
				throw new InvalidOperationException ();
			
			if (!owner.RemoveCardFromHand (card))
				if (!owner.RemoveCardFromTable (card as TableCard))
					throw new InvalidOperationException ();
			PutCardToGraveyard (card);
			
			game.Session.EventManager.OnPlayerPlayedCard (owner, card, targetPlayer);
			owner.OnPlayedCard (card);
			owner.CheckEmptyHand ();
		}
		public void PlayerPlayCard (Card card, Card targetCard)
		{
			Player owner = card.Owner;
			if (owner == null)
				throw new InvalidOperationException ();
			
			if (!owner.RemoveCardFromHand (card))
				if (!owner.RemoveCardFromTable (card as TableCard))
					throw new InvalidOperationException ();
			PutCardToGraveyard (card);
			
			game.Session.EventManager.OnPlayerPlayedCard (owner, card, targetCard.Owner, targetCard);
			owner.OnPlayedCard (card);
			owner.CheckEmptyHand ();
		}
		public void PlayerPlayCard(Card card, CardType asCard)
		{
			Player owner = card.Owner;
			if(owner == null)
				throw new InvalidOperationException();
			
			if(!owner.RemoveCardFromHand(card))
				if(!owner.RemoveCardFromTable(card as TableCard))
					throw new InvalidOperationException();
			PutCardToGraveyard(card);
			
			game.Session.EventManager.OnPlayerPlayedCard(owner, card, asCard);
			owner.OnPlayedCard(card);
			owner.CheckEmptyHand();
		}
		public void PlayerPlayCard(Card card, CardType asCard, Player targetPlayer)
		{
			Player owner = card.Owner;
			if(owner == null)
				throw new InvalidOperationException();
			
			if(!owner.RemoveCardFromHand(card))
				if(!owner.RemoveCardFromTable(card as TableCard))
					throw new InvalidOperationException();
			PutCardToGraveyard(card);
			
			game.Session.EventManager.OnPlayerPlayedCard(owner, card, asCard, targetPlayer);
			owner.OnPlayedCard(card);
			owner.CheckEmptyHand();
		}
		public void PlayerPlayCard(Card card, CardType asCard, Card targetCard)
		{
			Player owner = card.Owner;
			if(owner == null)
				throw new InvalidOperationException();
			
			if(!owner.RemoveCardFromHand(card))
				if(!owner.RemoveCardFromTable(card as TableCard))
					throw new InvalidOperationException();
			PutCardToGraveyard(card);
			
			game.Session.EventManager.OnPlayerPlayedCard(owner, card, asCard, targetCard.Owner, targetCard);
			owner.OnPlayedCard(card);
			owner.CheckEmptyHand();
		}
		public void PlayerPlayCardOnTable(TableCard card)
		{
			Player owner = card.Owner;
			if(owner == null)
				throw new InvalidOperationException();
			if(!owner.RemoveCardFromHand(card))
				throw new InvalidOperationException();

			owner.AddCardToTable(card);
			game.Session.EventManager.OnPlayerPlayedCardOnTable(owner, card);
			owner.CheckEmptyHand();
		}
		public void PassTableCard (TableCard card, Player targetPlayer)
		{
			Player owner = card.Owner;
			if (owner == null)
				throw new InvalidOperationException ();
			if(!owner.RemoveCardFromHand(card))
				if(!owner.RemoveCardFromTable (card))
					throw new InvalidOperationException ();
			
			targetPlayer.AddCardToTable (card);
			game.Session.EventManager.OnPassedTableCard (owner, card, targetPlayer);
		}
		public void PlayerPass (Player player)
		{
			game.Session.EventManager.OnPlayerPassed (player);
		}
		public void PlayerRespondWithCard (Card card)
		{
			Player owner = card.Owner;
			if (owner == null)
				throw new InvalidOperationException ();
			
			if (!owner.RemoveCardFromHand (card))
				if (!owner.RemoveCardFromTable (card as TableCard))
					throw new InvalidOperationException ();
			PutCardToGraveyard (card);
			
			game.Session.EventManager.OnPlayerRespondedWithCard (owner, card);
			owner.OnRespondedWithCard (card);
			owner.CheckEmptyHand ();
		}
		public void PlayerRespondWithCard(Card card, CardType asCard)
		{
			Player owner = card.Owner;
			if(owner == null)
				throw new InvalidOperationException();
			
			if(!owner.RemoveCardFromHand(card))
				if(!owner.RemoveCardFromTable(card as TableCard))
					throw new InvalidOperationException();
			PutCardToGraveyard(card);
			
			game.Session.EventManager.OnPlayerRespondedWithCard(owner, card, asCard);
			owner.OnRespondedWithCard(card);
			owner.CheckEmptyHand();
		}
		public void DrawIntoSelection (int count, Player owner)
		{
			List<Card> drawn = new List<Card> (count);
			for (int i = 0; i < count; i++)
			{
				Card card = PopCardFromDeck ();
				selection.Add (card);
				drawn.Add (card);
			}
			selectionOwner = owner;
			game.Session.EventManager.OnDrawnIntoSelection(drawn, owner);
		}
		public void PlayerPickFromSelection(Player player, Card card)
		{
			if(selectionOwner != null && selectionOwner != player)
				throw new InvalidOperationException();
			if (!selection.Remove (card))
				throw new InvalidOperationException();
			player.AddCardToHand (card);
			game.Session.EventManager.OnPlayerPickedFromSelection (player, card, selectionOwner == null);
		}
		public void UndrawFromSelection (Card card)
		{
			if (!selection.Remove (card))
				throw new InvalidOperationException ();
			PutCardToDeck (card);
			game.Session.EventManager.OnUndrawnFromSelection (card, selectionOwner);
		}
		public void CancelSelection()
		{
			List<Card> selectionCopy = new List<Card>(selection);
			foreach(Card c in selectionCopy)
				CancelCard(c);
		}
		public void PlayerStealCard (Player player, Card card)
		{
			Player owner = card.Owner;
			if (owner == null)
				throw new InvalidOperationException ();
			if (!owner.RemoveCardFromHand (card))
				if (!owner.RemoveCardFromTable (card as TableCard))
					throw new InvalidOperationException ();
			
			player.AddCardToHand (card);
			game.Session.EventManager.OnPlayerStoleCard (player, owner, card);
			owner.CheckEmptyHand ();
		}
		public void PlayerCancelCard (Player player, Card card)
		{
			Player owner = card.Owner;
			if (owner == null)
				throw new InvalidOperationException ();
			if (!owner.RemoveCardFromHand (card))
				if (!owner.RemoveCardFromTable (card as TableCard))
					throw new InvalidOperationException ();

			PutCardToGraveyard (card);
			game.Session.EventManager.OnPlayerCancelledCard (player, owner, card);
			owner.CheckEmptyHand();
		}
		public Card CheckDeck ()
		{
			Card checkedCard = PopCardFromDeck ();
			PutCardToGraveyard (checkedCard);
			game.Session.EventManager.OnDeckChecked (checkedCard);
			return checkedCard;
		}
		public void CancelCard(Card card)
		{
			Player owner = card.Owner;
			if(owner == null)
			{
				if(!selection.Remove(card))
					throw new InvalidOperationException();
			}
			else if(!owner.RemoveCardFromHand(card))
				if(!owner.RemoveCardFromTable(card as TableCard))
					throw new InvalidOperationException();
			
			PutCardToGraveyard(card);
			game.Session.EventManager.OnCardCancelled(card);
			if(owner != null)
				owner.CheckEmptyHand ();
		}
		
		private void RegenerateDeck ()
		{
			Card top = graveyard.Pop ();
			while (graveyard.Count != 0)
				deck.Push (graveyard.Pop ());
			graveyard.Push (top);
			
			game.Session.EventManager.OnDeckRegenerated();
		}
		private Card PopCardFromDeck ()
		{
			if (deck.Count == 0)
				RegenerateDeck ();
			return deck.Pop ();
		}
		private Card PopCardFromGraveyard ()
		{
			if (graveyard.Count == 0)
				throw new BadGameStateException();
			return graveyard.Pop ();
		}
		private void PutCardToDeck (Card card)
		{
			card.Owner = null;
			deck.Push(card);
		}
		private void PutCardToGraveyard (Card card)
		{
			card.Owner = null;
			graveyard.Push (card);
		}
		private void AddCardToSelection (Card card)
		{
			selection.Add (card);
		}
		private bool RemoveCardFromSelection (Card card)
		{
			return selection.Remove (card);
		}
		
		private void GenerateCards()
		{
			int count = 80;
			if(game.Session.DodgeCity)
				count += 40;
			
			cards = new Dictionary<int, Card>(count);
			List<Card> cardList = new List<Card>(count);
			List<int> idList = new List<int>(count);
			for(int id = 1; id <= count; id++)
				idList.Add(id);
			idList.Shuffle();
			Stack<int> idStack = new Stack<int>(idList);
			
			Card card;
			card = Card.GetCard(game, idStack.Peek(), CardType.Winchester, CardSuit.Spades, CardRank.Eight); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Diligenza, CardSuit.Spades, CardRank.Nine); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Volcanic, CardSuit.Spades, CardRank.Ten); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Duel, CardSuit.Spades, CardRank.Jack); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.GeneralStore, CardSuit.Spades, CardRank.Queen); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Schofield, CardSuit.Spades, CardRank.King); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Spades, CardRank.Ace); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Mustang, CardSuit.Hearts, CardRank.Eight); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Mustang, CardSuit.Hearts, CardRank.Nine); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Gatling, CardSuit.Hearts, CardRank.Ten); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Panic, CardSuit.Hearts, CardRank.Jack); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Panic, CardSuit.Hearts, CardRank.Queen); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.CatBalou, CardSuit.Hearts, CardRank.King); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Panic, CardSuit.Hearts, CardRank.Ace); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Panic, CardSuit.Diamonds, CardRank.Eight); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.CatBalou, CardSuit.Diamonds, CardRank.Nine); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.CatBalou, CardSuit.Diamonds, CardRank.Ten); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.CatBalou, CardSuit.Diamonds, CardRank.Jack); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Duel, CardSuit.Diamonds, CardRank.Queen); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Indians, CardSuit.Diamonds, CardRank.King); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Indians, CardSuit.Diamonds, CardRank.Ace); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Duel, CardSuit.Clubs, CardRank.Eight); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.GeneralStore, CardSuit.Clubs, CardRank.Nine); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Clubs, CardRank.Ten); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Clubs, CardRank.Jack); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Clubs, CardRank.Queen); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Clubs, CardRank.King); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Clubs, CardRank.Ace); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Spades, CardRank.Two); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Spades, CardRank.Three); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Spades, CardRank.Four); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Spades, CardRank.Five); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Spades, CardRank.Six); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Spades, CardRank.Seven); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Spades, CardRank.Eight); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Diligenza, CardSuit.Spades, CardRank.Nine); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Jail, CardSuit.Spades, CardRank.Ten); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Jail, CardSuit.Spades, CardRank.Jack); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Barrel, CardSuit.Spades, CardRank.Queen); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Barrel, CardSuit.Spades, CardRank.King); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Appaloosa, CardSuit.Spades, CardRank.Ace); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Dynamite, CardSuit.Hearts, CardRank.Two); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.WellsFargo, CardSuit.Hearts, CardRank.Three); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Jail, CardSuit.Hearts, CardRank.Four); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Saloon, CardSuit.Hearts, CardRank.Five); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Beer, CardSuit.Hearts, CardRank.Six); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Beer, CardSuit.Hearts, CardRank.Seven); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Beer, CardSuit.Hearts, CardRank.Eight); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Beer, CardSuit.Hearts, CardRank.Nine); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Beer, CardSuit.Hearts, CardRank.Ten); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Beer, CardSuit.Hearts, CardRank.Jack); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Hearts, CardRank.Queen); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Hearts, CardRank.King); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Hearts, CardRank.Ace); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.Two); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.Three); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.Four); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.Five); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.Six); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.Seven); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.Eight); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.Nine); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.Ten); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.Jack); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.Queen); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.King); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Diamonds, CardRank.Ace); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Clubs, CardRank.Two); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Clubs, CardRank.Three); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Clubs, CardRank.Four); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Clubs, CardRank.Five); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Clubs, CardRank.Six); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Clubs, CardRank.Seven); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Clubs, CardRank.Eight); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Clubs, CardRank.Nine); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Volcanic, CardSuit.Clubs, CardRank.Ten); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Schofield, CardSuit.Clubs, CardRank.Jack); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Schofield, CardSuit.Clubs, CardRank.Queen); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Remington, CardSuit.Clubs, CardRank.King); cardList.Add(card); cards.Add(idStack.Pop(), card);
			card = Card.GetCard(game, idStack.Peek(), CardType.Carabine, CardSuit.Clubs, CardRank.Ace); cardList.Add(card); cards.Add(idStack.Pop(), card);

			if(game.Session.DodgeCity)
			{
				card = Card.GetCard(game, idStack.Peek(), CardType.Carabine, CardSuit.Spades, CardRank.Five); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Beer, CardSuit.Spades, CardRank.Six); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Derringer, CardSuit.Spades, CardRank.Seven); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Spades, CardRank.Eight); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Howitzer, CardSuit.Spades, CardRank.Nine); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Punch, CardSuit.Spades, CardRank.Ten); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Brawl, CardSuit.Spades, CardRank.Jack); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.IronPlate, CardSuit.Spades, CardRank.Queen); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Springfield, CardSuit.Spades, CardRank.King); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.GeneralStore, CardSuit.Spades, CardRank.Ace); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Mustang, CardSuit.Hearts, CardRank.Five); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Beer, CardSuit.Hearts, CardRank.Six); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Canteen, CardSuit.Hearts, CardRank.Seven); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Knife, CardSuit.Hearts, CardRank.Eight); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.RagTime, CardSuit.Hearts, CardRank.Nine); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Bible, CardSuit.Hearts, CardRank.Ten); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Panic, CardSuit.Hearts, CardRank.Jack); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Whisky, CardSuit.Hearts, CardRank.Queen); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Dodge, CardSuit.Hearts, CardRank.King); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Dynamite, CardSuit.Hearts, CardRank.Ace); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Indians, CardSuit.Diamonds, CardRank.Five); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Remington, CardSuit.Diamonds, CardRank.Six); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Dodge, CardSuit.Diamonds, CardRank.Seven); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Missed, CardSuit.Diamonds, CardRank.Eight); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Conestoga, CardSuit.Diamonds, CardRank.Nine); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Silver, CardSuit.Diamonds, CardRank.Ten); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.TenGallonHat, CardSuit.Diamonds, CardRank.Jack); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.PonyExpress, CardSuit.Diamonds, CardRank.Queen); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Hideout, CardSuit.Diamonds, CardRank.King); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.IronPlate, CardSuit.Diamonds, CardRank.Ace); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Clubs, CardRank.Five); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Clubs, CardRank.Six); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Sombrero, CardSuit.Clubs, CardRank.Seven); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.CatBalou, CardSuit.Clubs, CardRank.Eight); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Tequila, CardSuit.Clubs, CardRank.Nine); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Pepperbox, CardSuit.Clubs, CardRank.Ten); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.CanCan, CardSuit.Clubs, CardRank.Jack); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.BuffaloRifle, CardSuit.Clubs, CardRank.Queen); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Bang, CardSuit.Clubs, CardRank.King); cardList.Add(card); cards.Add(idStack.Pop(), card);
				card = Card.GetCard(game, idStack.Peek(), CardType.Barrel, CardSuit.Clubs, CardRank.Ace); cardList.Add(card); cards.Add(idStack.Pop(), card);
			}
			//card = Card.GetCard(game, idStack.Peek(), CardType., CardSuit., CardRank.); cardList.Add(card); cards.Add(idStack.Pop(), card);
			
			cardList.Shuffle();
			deck = new Stack<Card> (cardList);
			graveyard = new Stack<Card> (count);
		}
	}
}

