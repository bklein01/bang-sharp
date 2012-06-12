// AllocationManager.cs
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
using System.Collections.Generic;
using BangSharp.Client.GameBoard.Animators;
using BangSharp.Client.GameBoard.Widgets;

namespace BangSharp.Client.GameBoard
{
	public class AllocationManager
	{
		private abstract class Allocator
		{
			private AllocationManager parent;
			private PlaceholderWidget placeholder;

			public AllocationManager Parent
			{
				get { return parent; }
			}
			public PlaceholderWidget Placeholder
			{
				get { return placeholder; }
			}

			protected Allocator(AllocationManager parent, PlaceholderWidget placeholder)
			{
				this.parent = parent;
				this.placeholder = placeholder;
			}

			public abstract void Reallocate();
			public abstract void OrderWidgets(AnimationLayer layer);
		}
		private class PlayingCardListAllocator : Allocator
		{
			private List<PlayingCardAnimator> animators;

			public List<PlayingCardAnimator> Animators
			{
				get { return animators; }
			}

			public PlayingCardListAllocator(AllocationManager parent, PlaceholderWidget placeholder) :
				base(parent, placeholder)
			{
				animators = new List<PlayingCardAnimator>();
			}

			public override void Reallocate()
			{
				PlayingCardListWidget l = new PlayingCardListWidget();
				CardPlaceholderWidget[] cw = new CardPlaceholderWidget[animators.Count];
				for(int i = 0; i < animators.Count; i++)
					l.Children.Add(cw[i] = new CardPlaceholderWidget());
				l.Reallocate(Placeholder.GetAbsoluteAllocation());
				for(int i = 0; i < animators.Count; i++)
					animators[i].GetState(Parent.type).Allocation = cw[i].GetAbsoluteAllocation();
			}
			public override void OrderWidgets(AnimationLayer layer)
			{
				foreach(PlayingCardAnimator a in animators)
					layer.Children.Add(a.Widget);
			}
		}
		private class LifePointsCardAllocator : Allocator
		{
			private CharacterCardAnimator animator;

			public CharacterCardAnimator Animator
			{
				get { return animator; }
			}
			public int LifePoints
			{
				get;
				set;
			}

			public LifePointsCardAllocator(AllocationManager parent, PlaceholderWidget placeholder, CharacterCardAnimator animator) :
				base(parent, placeholder)
			{
				this.animator = animator;
			}

			public override void Reallocate()
			{
				Cairo.Rectangle area = Placeholder.GetAbsoluteAllocation();
				double w = area.Width;
				double h = w / Card.Ratio;
				int lp = LifePoints <= 0 ? 0 : (LifePoints - 1) % 5 + 1;
				double y = h * lp / 5;
				animator.GetState(Parent.type).Allocation = new Cairo.Rectangle(area.X, area.Y + y, w, h);
			}
			public override void OrderWidgets(AnimationLayer layer)
			{
				layer.Children.Add(animator.Widget);
			}
		}
		private class RoleCardAllocator : Allocator
		{
			private RoleCardAnimator animator;

			public RoleCardAnimator Animator
			{
				get { return animator; }
			}

			public RoleCardAllocator(AllocationManager parent, PlaceholderWidget placeholder, RoleCardAnimator animator) :
				base(parent, placeholder)
			{
				this.animator = animator;
			}

			public override void Reallocate()
			{
				animator.GetState(Parent.type).Allocation = Placeholder.GetAbsoluteAllocation();
			}
			public override void OrderWidgets(AnimationLayer layer)
			{
				layer.Children.Add(animator.Widget);
			}
		}
		private Animation anim;
		private StateType type;

		private PlayingCardListAllocator deckCards;
		private PlayingCardListAllocator graveyardCards;
		private PlayingCardListAllocator selectionCards;
		private Dictionary<int, PlayingCardListAllocator> playerHands;
		private Dictionary<int, PlayingCardListAllocator> playerTables;
		private Dictionary<int, LifePointsCardAllocator> playerLifePoints;
		private Dictionary<int, RoleCardAllocator> playerRoles;

		public AllocationManager(Animation anim, StateType type, AllocationManager previous = null)
		{
			this.anim = anim;
			this.type = type;
			RootWidget root = anim.AnimLayer.RootWidget;
			deckCards = new PlayingCardListAllocator(this, root.DeckPlaceholder);
			graveyardCards = new PlayingCardListAllocator(this, root.GraveyardPlaceholder);
			selectionCards = new PlayingCardListAllocator(this, root.SelectionPlaceholder);

			IGame game = ConnectionManager.Game;

			int count = game.Players.Count;
			playerHands = new Dictionary<int, PlayingCardListAllocator>(count);
			playerTables = new Dictionary<int, PlayingCardListAllocator>(count);
			playerLifePoints = new Dictionary<int, LifePointsCardAllocator>(count);
			playerRoles = new Dictionary<int, RoleCardAllocator>(count);
			foreach(IPublicPlayerView player in game.Players)
			{
				int playerId = player.ID;
				playerHands.Add(playerId, new PlayingCardListAllocator(this, root.GetPlayerHandPlaceholder(playerId)));
				playerTables.Add(playerId, new PlayingCardListAllocator(this, root.GetPlayerTablePlaceholder(playerId)));
				playerLifePoints.Add(playerId, new LifePointsCardAllocator(this, root.GetPlayerCharacterPlaceholder(playerId), anim.GetPlayerCharacterAnimator(playerId)));
				playerRoles.Add(playerId, new RoleCardAllocator(this, root.GetPlayerRolePlaceholder(playerId), anim.GetPlayerRoleAnimator(playerId)));
			}

			if(previous != null)
			{
				int thisPlayerId = 0;
				if(ConnectionManager.PlayerGameControl != null)
					thisPlayerId = ConnectionManager.PlayerGameControl.PrivatePlayerView.ID;
				List<PlayingCardAnimator> lastGraveyard = previous.graveyardCards.Animators;
				if(lastGraveyard.Count != 0)
				{
					PlayingCardAnimator prev = lastGraveyard[lastGraveyard.Count - 1];
					PlayingCardAnimator a = anim.GetPlayingCardAnimator(prev.ID);
					a.GetState(type).Update(prev.GetState(previous.type));
					this.graveyardCards.Animators.Add(a);
				}

				UpdateList(this.selectionCards, previous.selectionCards, previous.type);

				foreach(int id in previous.playerHands.Keys)
					UpdateList(this.playerHands[id], previous.playerHands[id], previous.type, type == StateType.End && id != thisPlayerId);

				foreach(int id in previous.playerTables.Keys)
					UpdateList(this.playerTables[id], previous.playerTables[id], previous.type);

				foreach(int id in previous.playerLifePoints.Keys)
					this.playerLifePoints[id].LifePoints = previous.playerLifePoints[id].LifePoints;
			}
		}

		private void UpdateList(PlayingCardListAllocator target, PlayingCardListAllocator source, StateType sourceType, bool hideCards = false)
		{
			foreach(PlayingCardAnimator prev in source.Animators)
			{
				PlayingCardAnimator a = anim.GetPlayingCardAnimator(prev.ID);
				if(!hideCards)
					a.GetState(type).Update(prev.GetState(sourceType));
				target.Animators.Add(a);
			}
		}

		public void AddDeckCard(int cardId)
		{
			PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
			if(deckCards.Animators.Contains(a))
				return;
			deckCards.Animators.Add(a);
		}
		public void RemoveDeckCard(int cardId)
		{
			deckCards.Animators.RemoveAll(a => a.ID == cardId);
		}

		public void AddGraveyardCard(int cardId)
		{
			PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
			if(graveyardCards.Animators.Contains(a))
				return;
			graveyardCards.Animators.Add(a);
		}
		public void InsertGraveyardCard(int cardId)
		{
			PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
			if(graveyardCards.Animators.Contains(a))
				return;
			graveyardCards.Animators.Insert(0, a);
		}
		public void RemoveGraveyardCard(int cardId)
		{
			graveyardCards.Animators.RemoveAll(a => a.ID == cardId);
		}

		public void AddSelectionCard(int cardId)
		{
			PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
			if(selectionCards.Animators.Contains(a))
				return;
			selectionCards.Animators.Add(a);
		}
		public void RemoveSelectionCard(int cardId)
		{
			selectionCards.Animators.RemoveAll(a => a.ID == cardId);
		}

		public void AddPlayerHandCard(int playerId, int cardId)
		{
			PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
			if(playerHands[playerId].Animators.Contains(a))
				return;
			playerHands[playerId].Animators.Add(a);
		}
		public void RemovePlayerHandCard(int playerId, int cardId)
		{
			playerHands[playerId].Animators.RemoveAll(a => a.ID == cardId);
		}

		public void AddPlayerTableCard(int playerId, int cardId)
		{
			PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
			if(playerTables[playerId].Animators.Contains(a))
				return;
			playerTables[playerId].Animators.Add(a);
		}
		public void RemovePlayerTableCard(int playerId, int cardId)
		{
			playerTables[playerId].Animators.RemoveAll(a => a.ID == cardId);
		}

		public void RemoveCard(int cardId)
		{
			RemoveDeckCard(cardId);
			RemoveGraveyardCard(cardId);
			RemoveSelectionCard(cardId);
			foreach(int playerId in playerHands.Keys)
				RemovePlayerHandCard(playerId, cardId);
			foreach(int playerId in playerTables.Keys)
				RemovePlayerTableCard(playerId, cardId);
		}

		public void SetPlayerLifepoints(int playerId, int lifePoints)
		{
			playerLifePoints[playerId].LifePoints = lifePoints;
		}

		/// <summary>
		/// Recalculates the allocations for this state.
		/// </summary>
		public void Reallocate()
		{
			deckCards.Reallocate();
			graveyardCards.Reallocate();

			selectionCards.Reallocate();

			foreach(Allocator allocator in playerHands.Values)
				allocator.Reallocate();

			foreach(Allocator allocator in playerTables.Values)
				allocator.Reallocate();

			foreach(Allocator allocator in playerLifePoints.Values)
				allocator.Reallocate();

			foreach(Allocator allocator in playerRoles.Values)
				allocator.Reallocate();
		}
		/// <summary>
		/// Reordes widgets according to this state.
		/// </summary>
		public void ReorderWidgets()
		{
			AnimationLayer layer = anim.AnimLayer;
			layer.Children.Clear();

			foreach(Allocator allocator in playerLifePoints.Values)
				allocator.OrderWidgets(layer);

			foreach(Allocator allocator in playerRoles.Values)
				allocator.OrderWidgets(layer);

			graveyardCards.OrderWidgets(layer);

			foreach(Allocator allocator in playerTables.Values)
				allocator.OrderWidgets(layer);

			foreach(Allocator allocator in playerHands.Values)
				allocator.OrderWidgets(layer);

			selectionCards.OrderWidgets(layer);

			deckCards.OrderWidgets(layer);
		}
	}
}

