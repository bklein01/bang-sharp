// Animation.cs
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
using System.Diagnostics;
using BangSharp.Client.GameBoard.Animators;
using BangSharp.Client.GameBoard.Widgets;
using BangSharp.Client.GameBoard.States;

namespace BangSharp.Client.GameBoard
{
	/// <summary>
	/// Represents an animation on the game board.
	/// </summary>
	/// <remarks>
	/// The Animation class is used to play an animation from one game state to another.
	/// It uses instances of <see cref="BangSharp.Client.GameBoard.AllocationManager"/>
	/// ('allocation managers') to manage the 'start' and 'end' states of the animation.
	/// They store the states in 'animators' that are created for each card and are provided
	/// by the Animation object. The animators are also used to set the corresponding widget's
	/// state according to their start state, end state and supplied progress vlaue (a time-based
	/// value from 0.0 - 1.0) through the <see cref="BangSharp.Client.GameBoard.Animators.IAnimator"/>
	/// interface.
	/// </remarks>
	public class Animation
	{
		public static readonly TimeSpan AnimDelay = new TimeSpan(0, 0, 0, 0, 150);

		private TimeSpan animLength = new TimeSpan(0, 0, 1);

		private AnimationLayer layer;
		private bool onlyEnd;
		private bool ended;
		private Stopwatch sw;

		private Dictionary<int, PlayingCardAnimator> playingCardAnimators;
		private Dictionary<int, RoleCardAnimator> playerRoleAnimators;
		private Dictionary<int, CharacterCardAnimator> playerCharacterAnimators;

		private AllocationManager startAllocManager;
		private AllocationManager endAllocManager;

		public AnimationLayer AnimLayer
		{
			get { return layer; }
		}

		public bool Ended
		{
			get { return ended; }
		}

		public AllocationManager StartAllocManager
		{
			get { return startAllocManager; }
		}
		public AllocationManager EndAllocManager
		{
			get { return endAllocManager; }
		}

		public Animation(AnimationLayer layer, Animation lastAnim = null)
		{
			this.layer = layer;
			onlyEnd = false;
			ended = false;
			sw = new Stopwatch();

			playingCardAnimators = new Dictionary<int, PlayingCardAnimator>();
			playerRoleAnimators = new Dictionary<int, RoleCardAnimator>(8);
			playerCharacterAnimators = new Dictionary<int, CharacterCardAnimator>(8);
			foreach(IPublicPlayerView player in ConnectionManager.Game.Players)
			{
				int id = player.ID;
				playerRoleAnimators[id] = new RoleCardAnimator(this, layer.GetPlayerRoleWidget(id));
				playerCharacterAnimators[id] = new CharacterCardAnimator(this, layer.GetPlayerCharacterWidget(id));
				if(lastAnim != null)
				{
					RoleCardState lastRoleState = lastAnim.playerRoleAnimators[id].EndState;
					playerRoleAnimators[id].StartState.Update(lastRoleState);
					playerRoleAnimators[id].EndState.Update(lastRoleState);

					CharacterCardState lastCharacterState = lastAnim.playerCharacterAnimators[id].EndState;
					playerCharacterAnimators[id].StartState.Update(lastCharacterState);
					playerCharacterAnimators[id].EndState.Update(lastCharacterState);
				}
			}

			if(lastAnim != null)
			{
				startAllocManager = new AllocationManager(this, StateType.Start, lastAnim.endAllocManager);
				endAllocManager = new AllocationManager(this, StateType.End, lastAnim.endAllocManager);
			}
			else
			{
				startAllocManager = new AllocationManager(this, StateType.Start);
				endAllocManager = new AllocationManager(this, StateType.End);
				onlyEnd = true;
			}
		}

		public PlayingCardAnimator GetPlayingCardAnimator(int cardId)
		{
			try
			{
				return playingCardAnimators[cardId];
			}
			catch(KeyNotFoundException)
			{
				return playingCardAnimators[cardId] = new PlayingCardAnimator(this, layer.GetPlayingCardWidget(cardId));
			}
		}
		public void RemovePlayingCardAnimator(int cardId)
		{
			playingCardAnimators.Remove(cardId);
		}

		public CharacterCardAnimator GetPlayerCharacterAnimator(int playerId)
		{
			return playerCharacterAnimators[playerId];
		}

		public RoleCardAnimator GetPlayerRoleAnimator(int playerId)
		{
			return playerRoleAnimators[playerId];
		}

		/// <summary>
		/// Starts the animation.
		/// </summary>
		public void Start()
		{
			sw.Start();
		}
		/// <summary>
		/// Aborts the animation.
		/// </summary>
		public void Abort()
		{
			lock(layer.AnimLock)
				ended = true;
		}

		private void Reanimate()
		{
			double progress = onlyEnd ? 1.0 : (double)sw.Elapsed.Ticks / animLength.Ticks;
			if(progress > 1.0)
				progress = 1.0;

			if(progress < 0.5)
				startAllocManager.ReorderWidgets();
			else
				endAllocManager.ReorderWidgets();
			
			foreach(IAnimator a in playingCardAnimators.Values)
				a.Animate(progress);
			foreach(IAnimator a in playerRoleAnimators.Values)
				a.Animate(progress);
			foreach(IAnimator a in playerCharacterAnimators.Values)
				a.Animate(progress);

			if(progress == 1.0 || onlyEnd)
			{
				layer.NextAnimation();
				ended = true;
			}
		}

		/// <summary>
		/// Triggers recalculation of the widgets' allocation.
		/// </summary>
		public void Reallocate()
		{
			lock(layer.AnimLock)
			{
				startAllocManager.Reallocate();
				endAllocManager.Reallocate();
				Reanimate();
			}
		}
	}
}

