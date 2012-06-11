// AnimationLayer.cs
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using BangSharp.Client.GameBoard.Animators;

namespace BangSharp.Client.GameBoard.Widgets
{
	/// <summary>
	/// This widget is used to manage the animated part of the game board.
	/// </summary>
	/// <remarks>
	/// Animation layer listens to session events and maintains a queue of <see cref='BangSharp.Clent.GameBoard.Events.Event'/>
	/// objects ('event object'). When an event is recieved, a corresponding event object is created and added to the queue.
	/// For each event an animation is played. Whenever there is no animation playing and a new event object is available, a new
	/// <see cref='BangSharp.Client.GameBoard.Animation'/> object ('animation object') is created from that event object. The
	/// animation object is then launched, that is, the animation starts playing. When the animation ends, it notifies animation
	/// layer which starts another animation (if possible).
	/// </remarks>
	public class AnimationLayer : Widget
	{
		private class EventListener : VirtualSessionEventListener
		{
			private AnimationLayer parent;

			public EventListener(AnimationLayer parent)
			{
				this.parent = parent;
			}
			public override void OnJoinedSession(IPlayerSessionControl control)
			{
			}
			public override void OnJoinedGame(IPlayerControl control)
			{
				parent.Reset();
				Animation anim = new Animation(parent);

				IPrivatePlayerView privateView = control.PrivatePlayerView;
				IGame game = control.Game;
				int thisPlayerId = control.PrivatePlayerView.ID;
				foreach(IPublicPlayerView player in game.Players)
				{
					int playerId = player.ID;
					if(playerId == thisPlayerId)
					{
						anim.EndAllocManager.SetPlayerLifepoints(playerId, privateView.LifePoints);
						anim.GetPlayerCharacterAnimator(playerId).EndState.Type = privateView.CharacterType;
						anim.GetPlayerRoleAnimator(playerId).EndState.Role = privateView.Role;
						foreach(ICard card in privateView.Hand)
						{
							int cardId = card.ID;
							anim.EndAllocManager.AddPlayerHandCard(playerId, cardId);
							PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
							a.EndState.Type = card.Type;
							a.EndState.Rank = card.Rank;
							a.EndState.Suit = card.Suit;
						}
						foreach(ICard card in privateView.Table)
						{
							int cardId = card.ID;
							anim.EndAllocManager.AddPlayerTableCard(playerId, cardId);
							PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
							a.EndState.Type = card.Type;
							a.EndState.Rank = card.Rank;
							a.EndState.Suit = card.Suit;
						}
						foreach(ICard card in privateView.Selection)
						{
							int cardId = card.ID;
							anim.EndAllocManager.AddSelectionCard(cardId);
							PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
							a.EndState.Type = card.Type;
							a.EndState.Rank = card.Rank;
							a.EndState.Suit = card.Suit;
						}
					}
					else
					{
						anim.EndAllocManager.SetPlayerLifepoints(playerId, player.LifePoints);
						anim.GetPlayerCharacterAnimator(playerId).EndState.Type = player.CharacterType;
						anim.GetPlayerRoleAnimator(playerId).EndState.Role = player.Role;
						foreach(ICard card in player.Hand)
						{
							int cardId = card.ID;
							anim.EndAllocManager.AddPlayerHandCard(playerId, cardId);
							PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
							a.EndState.Type = card.Type;
							a.EndState.Rank = card.Rank;
							a.EndState.Suit = card.Suit;
						}
						foreach(ICard card in player.Table)
						{
							int cardId = card.ID;
							anim.EndAllocManager.AddPlayerTableCard(playerId, cardId);
							PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
							a.EndState.Type = card.Type;
							a.EndState.Rank = card.Rank;
							a.EndState.Suit = card.Suit;
						}
					}
				}
				ICard graveyardTop = game.GraveyardTop;
				if(graveyardTop != null)
				{
					int cardId = graveyardTop.ID;
					anim.EndAllocManager.AddGraveyardCard(cardId);
					PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
					a.EndState.Type = graveyardTop.Type;
					a.EndState.Rank = graveyardTop.Rank;
					a.EndState.Suit = graveyardTop.Suit;
				}

				parent.EnqueueAnimation(anim);
			}
			public override void OnNewRequest(RequestType requestType, IPublicPlayerView causedBy)
			{
			}
			public override void OnJoinedSession(ISpectatorSessionControl control)
			{
			}
			public override void OnJoinedGame(ISpectatorControl control)
			{
				parent.Reset();
				Animation anim = new Animation(parent);

				IGame game = control.Game;
				foreach(IPublicPlayerView player in game.Players)
				{
					int playerId = player.ID;
					anim.EndAllocManager.SetPlayerLifepoints(playerId, player.LifePoints);
					anim.GetPlayerCharacterAnimator(playerId).EndState.Type = player.CharacterType;
					anim.GetPlayerRoleAnimator(playerId).EndState.Role = player.Role;
					foreach(ICard card in player.Hand)
					{
						int cardId = card.ID;
						anim.EndAllocManager.AddPlayerHandCard(playerId, cardId);
						PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
						a.EndState.Type = card.Type;
						a.EndState.Rank = card.Rank;
						a.EndState.Suit = card.Suit;
					}
					foreach(ICard card in player.Table)
					{
						int cardId = card.ID;
						anim.EndAllocManager.AddPlayerTableCard(playerId, cardId);
						PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
						a.EndState.Type = card.Type;
						a.EndState.Rank = card.Rank;
						a.EndState.Suit = card.Suit;
					}
				}
				ICard graveyardTop = game.GraveyardTop;
				if(graveyardTop != null)
				{
					int cardId = graveyardTop.ID;
					anim.EndAllocManager.AddGraveyardCard(cardId);
					PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
					a.EndState.Type = graveyardTop.Type;
					a.EndState.Rank = graveyardTop.Rank;
					a.EndState.Suit = graveyardTop.Suit;
				}

				parent.EnqueueAnimation(anim);
			}
			public override void OnPlayerDrewFromDeck(IPublicPlayerView player, ReadOnlyCollection<ICard> drawnCards)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				int playerId = player.ID;
				foreach(ICard card in drawnCards.Reverse())
				{
					int cardId = card.ID;
					anim.StartAllocManager.AddDeckCard(cardId);
					anim.EndAllocManager.AddPlayerHandCard(playerId, cardId);
					PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
					a.StartState.Type = CardType.Unknown;
					a.StartState.Rank = CardRank.Unknown;
					a.StartState.Suit = CardSuit.Unknown;
					a.EndState.Type = card.Type;
					a.EndState.Rank = card.Rank;
					a.EndState.Suit = card.Suit;
				}

				parent.EnqueueAnimation(anim);
			}
			public override void OnPlayerDrewFromGraveyard(IPublicPlayerView player, ReadOnlyCollection<ICard> drawnCards)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				int playerId = player.ID;
				foreach(ICard card in drawnCards)
				{
					int cardId = card.ID;
					anim.StartAllocManager.InsertGraveyardCard(cardId);
					anim.EndAllocManager.RemoveGraveyardCard(cardId);
					anim.EndAllocManager.AddPlayerHandCard(playerId, cardId);
					PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
					a.StartState.Type = a.EndState.Type = card.Type;
					a.StartState.Rank = a.EndState.Rank = card.Rank;
					a.StartState.Suit = a.EndState.Suit = card.Suit;
				}
				ICard graveyardTop = ConnectionManager.Game.GraveyardTop;
				anim.StartAllocManager.InsertGraveyardCard(graveyardTop.ID);
				anim.EndAllocManager.InsertGraveyardCard(graveyardTop.ID);
				PlayingCardAnimator g = anim.GetPlayingCardAnimator(graveyardTop.ID);
				g.StartState.Type = g.EndState.Type = graveyardTop.Type;
				g.StartState.Rank = g.EndState.Rank = graveyardTop.Rank;
				g.StartState.Suit = g.EndState.Suit = graveyardTop.Suit;

				parent.EnqueueAnimation(anim);
			}
			public override void OnPlayerDiscardedCard(IPublicPlayerView player, ICard card)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				int cardId = card.ID;
				int playerId = player.ID;
				anim.EndAllocManager.RemovePlayerHandCard(playerId, cardId);
				anim.EndAllocManager.RemovePlayerTableCard(playerId, cardId);
				anim.EndAllocManager.AddGraveyardCard(cardId);
				PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
				a.EndState.Type = card.Type;
				a.EndState.Rank = card.Rank;
				a.EndState.Suit = card.Suit;

				parent.EnqueueAnimation(anim);
			}
			public override void OnPlayerPlayedCard(IPublicPlayerView player, ICard card)
			{
				OnPlayerDiscardedCard(player, card);
			}
			public override void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
			{
				OnPlayerDiscardedCard(player, card);
			}
			public override void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer, ICard targetCard)
			{
				OnPlayerDiscardedCard(player, card);
			}
			public override void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard)
			{
				OnPlayerDiscardedCard(player, card);
			}
			public override void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer)
			{
				OnPlayerDiscardedCard(player, card);
			}
			public override void OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer, ICard targetCard)
			{
				OnPlayerDiscardedCard(player, card);
			}
			public override void OnPlayerPlayedCardOnTable(IPublicPlayerView player, ICard card)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				int cardId = card.ID;
				int playerId = player.ID;
				anim.EndAllocManager.RemovePlayerHandCard(playerId, cardId);
				anim.EndAllocManager.AddPlayerTableCard(playerId, cardId);
				PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
				a.EndState.Type = card.Type;
				a.EndState.Rank = card.Rank;
				a.EndState.Suit = card.Suit;

				parent.EnqueueAnimation(anim);
			}
			public override void OnPassedTableCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				int cardId = card.ID;
				int playerId = player.ID;
				int targetPlayerId = targetPlayer.ID;
				anim.EndAllocManager.RemovePlayerHandCard(playerId, cardId);
				anim.EndAllocManager.RemovePlayerTableCard(playerId, cardId);
				anim.EndAllocManager.AddPlayerTableCard(targetPlayerId, cardId);
				PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
				a.EndState.Type = card.Type;
				a.EndState.Rank = card.Rank;
				a.EndState.Suit = card.Suit;

				parent.EnqueueAnimation(anim);
			}
			public override void OnPlayerPassed(IPublicPlayerView player)
			{
			}
			public override void OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card)
			{
				OnPlayerDiscardedCard(player, card);
			}
			public override void OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card, CardType asCard)
			{
				OnPlayerDiscardedCard(player, card);
			}
			public override void OnDrawnIntoSelection(ReadOnlyCollection<ICard> drawnCards)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				foreach(ICard card in drawnCards.Reverse())
				{
					int cardId = card.ID;
					anim.StartAllocManager.AddDeckCard(cardId);
					anim.EndAllocManager.AddSelectionCard(cardId);
					PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
					a.StartState.Type = CardType.Unknown;
					a.StartState.Rank = CardRank.Unknown;
					a.StartState.Suit = CardSuit.Unknown;
					a.EndState.Type = card.Type;
					a.EndState.Rank = card.Rank;
					a.EndState.Suit = card.Suit;
				}

				parent.EnqueueAnimation(anim);
			}
			public override void OnPlayerPickedFromSelection(IPublicPlayerView player, ICard card)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				int cardId = card.ID;
				int playerId = player.ID;
				anim.EndAllocManager.RemoveSelectionCard(cardId);
				anim.EndAllocManager.AddPlayerHandCard(playerId, cardId);
				PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
				a.EndState.Type = card.Type;
				a.EndState.Rank = card.Rank;
				a.EndState.Suit = card.Suit;

				parent.EnqueueAnimation(anim);
			}
			public override void OnUndrawnFromSelection(ICard card)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				int cardId = card.ID;
				anim.EndAllocManager.RemoveSelectionCard(cardId);
				anim.EndAllocManager.AddDeckCard(cardId);
				PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
				a.EndState.Type = CardType.Unknown;
				a.EndState.Rank = CardRank.Unknown;
				a.EndState.Suit = CardSuit.Unknown;

				parent.EnqueueAnimation(anim);
			}
			public override void OnPlayerStoleCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				int cardId = targetCard.ID;
				int playerId = player.ID;
				int targetPlayerId = targetPlayer.ID;
				anim.EndAllocManager.RemovePlayerHandCard(targetPlayerId, cardId);
				anim.EndAllocManager.RemovePlayerTableCard(targetPlayerId, cardId);
				anim.EndAllocManager.AddPlayerHandCard(playerId, cardId);
				PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
				a.EndState.Type = targetCard.Type;
				a.EndState.Rank = targetCard.Rank;
				a.EndState.Suit = targetCard.Suit;

				parent.EnqueueAnimation(anim);
			}
			public override void OnPlayerCancelledCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
			{
				OnPlayerDiscardedCard(targetPlayer, targetCard);
			}
			public override void OnDeckChecked(ICard card)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				int cardId = card.ID;
				anim.StartAllocManager.AddDeckCard(cardId);
				anim.EndAllocManager.AddGraveyardCard(cardId);
				PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
				a.EndState.Type = card.Type;
				a.EndState.Rank = card.Rank;
				a.EndState.Suit = card.Suit;

				parent.EnqueueAnimation(anim);
			}
			public override void OnCardCancelled(ICard card)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				int cardId = card.ID;
				anim.EndAllocManager.RemoveCard(cardId);
				anim.EndAllocManager.AddGraveyardCard(cardId);
				PlayingCardAnimator a = anim.GetPlayingCardAnimator(cardId);
				a.EndState.Type = card.Type;
				a.EndState.Rank = card.Rank;
				a.EndState.Suit = card.Suit;

				parent.EnqueueAnimation(anim);
			}
			public override void OnPlayerCheckedDeck(IPublicPlayerView player, ICard checkedCard, CardType causedBy, bool result)
			{
			}
			public override void OnLifePointsChanged(IPublicPlayerView player, int delta, IPublicPlayerView causedBy)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				anim.EndAllocManager.SetPlayerLifepoints(player.ID, player.LifePoints);

				parent.EnqueueAnimation(anim);
			}
			public override void OnPlayerDied(IPublicPlayerView player, IPublicPlayerView causedBy)
			{
				Animation anim = new Animation(parent, parent.lastAnim);

				RoleCardAnimator a = anim.GetPlayerRoleAnimator(player.ID);
				a.EndState.Role = player.Role;

				parent.EnqueueAnimation(anim);
			}
			public override void OnPlayerUsedAbility(IPublicPlayerView player, CharacterType character)
			{
			}
			public override void OnPlayerUsedAbility(IPublicPlayerView player, CharacterType character, IPublicPlayerView targetPlayer)
			{
			}
			public override void OnPlayerGainedAdditionalCharacters(IPublicPlayerView player)
			{
			}
			public override void OnPlayerLostAdditionalCharacters(IPublicPlayerView player)
			{
			}
			public override void OnDeckRegenerated()
			{
			}
		}
		private RootWidget root;
		private Dictionary<int, PlayingCardWidget> playingCardWidgets;
		private Dictionary<int, RoleCardWidget> playerRoleWidgets;
		private Dictionary<int, CharacterCardWidget> playerCharacterWidgets;

		private object animLock = new object();
		private Queue<Animation> animQueue;
		private Animation lastAnim;
		private Animation current;
		private Thread timerThread;
		private EventListener listener;

		/// <summary>
		/// Gets the animation synchronization lock.
		/// </summary>
		/// <value>
		/// The animation synchronization lock for this animation layer.
		/// </value>
		public object AnimLock
		{
			get { return animLock; }
		}
		public RootWidget RootWidget
		{
			get { return root; }
		}

		public AnimationLayer(RootWidget rootWidget)
		{
			root = rootWidget;

			animQueue = new Queue<Animation>();
			playingCardWidgets = new Dictionary<int, PlayingCardWidget>();
			playerRoleWidgets = new Dictionary<int, RoleCardWidget>(8);
			playerCharacterWidgets = new Dictionary<int, CharacterCardWidget>(8);

			timerThread = new Thread(RunTimer);
			timerThread.IsBackground = true;
			timerThread.Start();

			listener = new EventListener(this);
			ConnectionManager.SessionEventListener.AddListener((IPlayerSessionEventListener)listener);
			ConnectionManager.SessionEventListener.AddListener((ISpectatorSessionEventListener)listener);
		}

		private void RunTimer()
		{
			while(true)
			{
				lock(animLock)
				{
					while(current == null || current.Ended)
						Monitor.Wait(animLock);
					RequestResize();
					RequestRedraw();
				}
				Thread.Sleep(Animation.AnimDelay);
			}
		}

		/// <summary>
		/// Reset the animation layer.
		/// </summary>
		/// <remarks>
		/// This method is used to prepare the animation layer for a new game.
		/// </remarks>
		private void Reset()
		{
			lock(animLock)
			{
				if(current != null)
					current.Abort();
				current = null;
				lastAnim = null;
				animQueue.Clear();
				playingCardWidgets.Clear();
				playerRoleWidgets.Clear();
				playerCharacterWidgets.Clear();
				Children.Clear();

				int thisPlayerId = 0;
				if(ConnectionManager.PlayerGameControl != null)
					thisPlayerId = ConnectionManager.PlayerGameControl.PrivatePlayerView.ID;

				foreach(IPublicPlayerView player in ConnectionManager.Game.Players)
				{
					int id = player.ID;
					playerRoleWidgets[id] = new RoleCardWidget(player.Role);
					playerCharacterWidgets[id] = new CharacterCardWidget(player.CharacterType, id == thisPlayerId);
				}
			}
		}

		/// <summary>
		/// Gets the playing card widget with the specified ID.
		/// </summary>
		/// <returns>
		/// The playing card widget with the specified ID.
		/// </returns>
		/// <param name='cardId'>
		/// The card ID.
		/// </param>
		public PlayingCardWidget GetPlayingCardWidget(int cardId)
		{
			try
			{
				return playingCardWidgets[cardId];
			}
			catch(KeyNotFoundException)
			{
				return playingCardWidgets[cardId] = new PlayingCardWidget(cardId);
			}
		}
		/// <summary>
		/// Removes the playing card widget with the specified ID.
		/// </summary>
		/// <param name='cardId'>
		/// The card ID.
		/// </param>
		public void RemovePlayingCardWidget(int cardId)
		{
			if(playingCardWidgets.ContainsKey(cardId))
				playingCardWidgets.Remove(cardId);
		}

		/// <summary>
		/// Gets the role widget for the player with specified ID.
		/// </summary>
		/// <returns>
		/// The role widget for the player with specified ID.
		/// </returns>
		/// <param name='playerId'>
		/// The player ID.
		/// </param>
		public RoleCardWidget GetPlayerRoleWidget(int playerId)
		{
			return playerRoleWidgets[playerId];
		}
		/// <summary>
		/// Gets the character widget for the player with specified ID.
		/// </summary>
		/// <returns>
		/// The character widget for the player with specified ID.
		/// </returns>
		/// <param name='playerId'>
		/// The player ID.
		/// </param>
		public CharacterCardWidget GetPlayerCharacterWidget(int playerId)
		{
			return playerCharacterWidgets[playerId];
		}

		protected override void OnResized()
		{
			lock(animLock)
				if(current != null)
					current.Reallocate();
		}

		public void NextAnimation()
		{
			lock(animLock)
				if(animQueue.Count > 0)
				{
					current = animQueue.Dequeue();
					current.Start();
					Monitor.PulseAll(animLock);
				}
		}
		private void EnqueueAnimation(Animation anim)
		{
			lock(animLock)
			{
				lastAnim = anim;
				if(current == null || current.Ended)
				{
					current = anim;
					current.Start();
					Monitor.PulseAll(animLock);
				}
				else
					animQueue.Enqueue(anim);
				System.Console.Error.WriteLine("DEBUG: AnimQueue count: {0}", animQueue.Count);
			}
		}
	}
}

