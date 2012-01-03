// GameCycle.cs
//  
// Author:  WOnder93 <omosnacek@gmail.com>
// 
// Copyright (c) 2011 Ondrej Mosnáček
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
namespace Bang.Server
{
	public sealed class Draw : ResponseHandler
	{
		public Draw(Turn parent)
			: base(RequestType.Draw, parent.RequestedPlayer)
		{
		}

		protected override void OnStart()
		{
			if(!RequestedPlayer.IsAlive)
			{
				End();
				return;
			}
			Game.GameCycle.CurrentPlayer.OnTurnStarted();
			if(RequestedPlayer.SkipTurn)
				End();
		}
		protected override void OnContinue()
		{
			if(RequestedPlayer.SkipTurn || !RequestedPlayer.IsAlive)
				End();
		}

		protected override void OnRespondDraw()
		{
			Game.GameTable.PlayerDrawFromDeck(RequestedPlayer, 2);
			End();
		}
		protected override void OnRespondUseAbility()
		{
			RequestedPlayer.Character.Draw();
			End();
		}
	}
	public sealed class Play : ResponseHandler
	{
		public Play(Turn parent)
			: base(RequestType.Play, parent.RequestedPlayer)
		{
		}

		protected override void OnStart()
		{
			if(RequestedPlayer.SkipTurn || !RequestedPlayer.IsAlive)
				End();
		}
		protected override void OnContinue()
		{
			if(RequestedPlayer.SkipTurn || !RequestedPlayer.IsAlive)
				End();
			else
				foreach(Player p in Game.Players)
					if(p.IsAlive)
						p.Character.OnPlayContinue();
		}

		protected override void OnRespondCard(Card card)
		{
			if(card.Owner != RequestedPlayer)
				throw new BadCardException();
			
			RequestedPlayer.PlayCard(card);
			if(Game.GameCycle.RequestType == RequestType.Play)
				foreach(Player p in Game.Players)
					if(p.IsAlive)
						p.Character.OnPlayContinue();
		}
		protected override void OnRespondUseAbility()
		{
			RequestedPlayer.Character.UseAbility();
			if(Game.GameCycle.RequestType == RequestType.Play)
				foreach(Player p in Game.Players)
					if(p.IsAlive)
						p.Character.OnPlayContinue();
		}
		protected override void OnRespondNoAction()
		{
			End();
		}
	}
	public sealed class Discard : ResponseHandler
	{
		public Discard(Turn parent)
			: base(RequestType.DiscardCard, parent.RequestedPlayer)
		{
		}

		protected override void OnStart()
		{
			if(RequestedPlayer.Hand.Count <= RequestedPlayer.MaxCardCount)
				End();
		}

		protected override void OnRespondCard(Card card)
		{
			if(card.Owner != RequestedPlayer)
				throw new BadCardException();

			card.AssertInHand();

			Game.GameTable.PlayerDiscardCard(card);
			
			if(RequestedPlayer.Hand.Count <= RequestedPlayer.MaxCardCount)
				End();
		}
	}
	public sealed class Turn : QueueResponseHandler
	{
		public Turn(Round parent, Player player)
			: base(player, 3)
		{
			AddHandler(new Draw(this));
			AddHandler(new Play(this));
			AddHandler(new Discard(this));
		}

		protected override void OnStart()
		{
			List<TableCard> table = new List<TableCard>(RequestedPlayer.Table);
			// the list must be copied, or any removal from the table
			// causes the enumerator to throw an InvalidOperationException
			table.Sort((a, b) => b.PredrawCheckPriority - a.PredrawCheckPriority);
			foreach(TableCard card in table)
				try
				{
					card.PredrawCheck();
				}
				catch(GameException)
				{
				}
			base.OnStart();
		}
	}
	public sealed class Round : ResponseHandler
	{
		private MainThread parent;
		private Turn turn;

		public Round(MainThread parent)
		{
			this.parent = parent;
		}

		protected override void OnStart()
		{
			turn = new Turn(this, parent.CurrentPlayer);
			PushHandler(turn);
		}

		protected override void OnNext()
		{
			Player last = parent.CurrentPlayer;
			last.OnTurnEnded();
			parent.NextPlayer();
			Game.Session.EventManager.OnPlayerPassed(last);
			if(parent.CurrentPlayer.BeginsRound)
			{
				End();
				return;
			}
			turn = new Turn(this, parent.CurrentPlayer);
			PushHandler(turn);
		}
	}
	public sealed class MainThread : ResponseHandler
	{
		private int roundNumber;
		private Round round;
		private Player current;

		public Player CurrentPlayer
		{
			get { return current; }
		}

		public MainThread(GameCycle parent)
		{
			roundNumber = 0;
		}

		public void NextPlayer()
		{
			current = Game.NextPlayer(current);
		}

		protected override void OnStart()
		{
			Game.GameTable.Deal();
			current = Game.Players.First(p => p.BeginsRound);
			round = new Round(this);
			PushHandler(round);
		}
		protected override void OnNext()
		{
			roundNumber++;
			PushHandler(round);
		}
	}
	public sealed class GameCycle : ResponseHandler
	{
		private Game game;
		private MainThread main;

		public override Game Game
		{
			get { return game; }
		}

		public Player CurrentPlayer
		{
			get { return main.CurrentPlayer; }
		}

		public GameCycle(Game game)
		{
			this.game = game;
		}

		public void StartCycle()
		{
			Start();
		}
		protected override void OnStart ()
		{
			main = new MainThread(this);
			PushHandler(main);
		}

		public void PushTempHandler(ResponseHandler handler)
		{
			PushHandler(handler);
		}

		public void PlayerRespondDraw(Player player)
		{
			if(player != RequestedPlayer || game.Ended)
				throw new BadUsageException();
			
			RespondDraw();
		}
		public void PlayerRespondCard(Player player, Card targetCard)
		{
			if(player != RequestedPlayer || game.Ended)
				throw new BadUsageException();
			
			RespondCard(targetCard);
		}
		public void PlayerRespondPlayer(Player player, Player targetPlayer)
		{
			if(player != RequestedPlayer || game.Ended)
				throw new BadUsageException();
			
			RespondPlayer(targetPlayer);
		}
		public void PlayerRespondCharacter(Player player, CharacterType character)
		{
			if(player != RequestedPlayer || game.Ended)
				throw new BadUsageException();

			RespondCharacter(character);
		}
		public void PlayerRespondNoAction(Player player)
		{
			if(player != RequestedPlayer || game.Ended)
				throw new BadUsageException();
			
			RespondNoAction();
		}
		public void PlayerRespondUseAbility(Player player)
		{
			if(player != RequestedPlayer || game.Ended)
				throw new BadUsageException();
			
			RespondUseAbility();
		}
	}
}

