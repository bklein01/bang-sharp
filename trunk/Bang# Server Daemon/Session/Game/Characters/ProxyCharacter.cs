// ProxyCharacter.cs
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

namespace BangSharp.Server.Daemon
{
	public class ProxyCharacter : Character
	{
		private sealed class ChooseCharacterResponseHandler : ResponseHandler
		{
			private ProxyCharacter parent;
			private Action<Character> handler;

			public ChooseCharacterResponseHandler(ProxyCharacter parent, RequestType type, Action<Character> handler) :
				base(type, parent.Player)
			{
				this.parent = parent;
				this.handler = handler;
			}

			protected override void OnRespondCharacter(CharacterType character)
			{
				Character result;
				try
				{
					result = parent.characters.First(c => c.Type == character);
				}
				catch(InvalidOperationException)
				{
					throw new BadCharacterTypeException();
				}
				handler(result);
				End();
			}
			protected override void OnRespondNoAction()
			{
				handler(null);
				End();
			}
		}
		private List<Character> characters;

		public override int InitialCardCount
		{
			get { return characters.Count == 0 ? base.InitialCardCount : characters.Max(c => c.InitialCardCount); }
		}
		public override int MaxCardCount
		{
			get { return characters.Count == 0 ? base.MaxCardCount : characters.Max(c => c.MaxCardCount); }
		}
		public override int DistanceIn
		{
			get { return characters.Count == 0 ? base.DistanceIn : characters.Sum(c => c.DistanceIn); }
		}
		public override int DistanceOut
		{
			get { return characters.Count == 0 ? base.DistanceOut : characters.Sum(c => c.DistanceOut); }
		}
		public override int BangPower
		{
			get { return characters.Count == 0 ? base.BangPower : characters.Max(c => c.BangPower); }
		}
		public override int BeerPower
		{
			get { return characters.Count == 0 ? base.BeerPower : characters.Max(c => c.BeerPower); }
		}
		public override bool UnlimitedBangs
		{
			get { return characters.Count == 0 ? base.UnlimitedBangs : characters.Any(c => c.UnlimitedBangs); }
		}
		public override bool TakesDeadPlayersCards
		{
			get { return characters.Count == 0 ? base.TakesDeadPlayersCards : characters.Any(c => c.TakesDeadPlayersCards); }
		}

		protected ProxyCharacter(Player player, CharacterType type) : base(player, type)
		{
			characters = new List<Character>();
		}

		protected void ClearCharacters()
		{
			characters.Clear();
			Player.ClearAdditionalCharacters();
		}
		protected void SetCharacters(CharacterType[] types)
		{
			if(characters.Count != 0)
				ClearCharacters();
			foreach(CharacterType type in types)
				characters.Add(Character.GetCharacter(Player, type));
			Player.SetAditionalCharacters(types);
		}

		public override bool HasCardEffect(Card card)
		{
			return characters.Count == 0 ? base.HasCardEffect(card) : !characters.Any(c => !c.HasCardEffect(card));
		}
		public override bool IsMissed(Card card)
		{
			return characters.Count == 0 ? base.IsMissed(card) : characters.Any(c => c.IsMissed(card));
		}
		public override bool IsBang(Card card)
		{
			return characters.Count == 0 ? base.IsBang(card) : characters.Any(c => c.IsBang(card));
		}

		public override void Draw()
		{
			if(characters.Count == 0)
				base.Draw();
			else if(characters.Count == 1)
				characters.First().Draw();
			else
				Game.GameCycle.PushTempHandler(new ChooseCharacterResponseHandler(this, RequestType.ChooseCharacterForDraw, c =>
				{
					if(c != null)
						c.Draw();
					else
						base.Draw();
				}));
		}
		public override int DrawCardCount
		{
			get { return characters.Count == 0 ? base.DrawCardCount : characters.Max(c => c.DrawCardCount); }
		}
		public override bool RevealFirstDrawnCard
		{
			get { return characters.Count == 0 ? base.RevealFirstDrawnCard : characters.Any(c => c.RevealFirstDrawnCard); }
		}
		public override bool RevealSecondDrawnCard
		{
			get { return characters.Count == 0 ? base.RevealFirstDrawnCard : characters.Any(c => c.RevealSecondDrawnCard); }
		}
		public override void DrawFirstCard(CardCallback callback)
		{
			if(characters.Count == 0)
				base.DrawFirstCard(callback);
			else if(characters.Count == 1)
				characters.First().DrawFirstCard(callback);
			else
				Game.GameCycle.PushTempHandler(new ChooseCharacterResponseHandler(this, RequestType.ChooseCharacterForDrawFirstCard, c =>
				{
					if(c != null)
						c.DrawFirstCard(callback);
					else
						base.DrawFirstCard(callback);
				}));

		}
		public override void DrawSecondCard(CardCallback callback)
		{
			if(characters.Count == 0)
				base.DrawSecondCard(callback);
			else if(characters.Count == 1)
				characters.First().DrawSecondCard(callback);
			else
				Game.GameCycle.PushTempHandler(new ChooseCharacterResponseHandler(this, RequestType.ChooseCharacterForDrawSecondCard, c =>
				{
					if(c != null)
						c.DrawSecondCard(callback);
					else
						base.DrawSecondCard(callback);
				}));
		}
		public override void OnDrewFirstCard(Card card)
		{
			if(characters.Count == 0)
				base.OnDrewFirstCard(card);
			else
				characters.ForEach(c => c.OnDrewFirstCard(card));
		}
		public override void OnDrewSecondCard(Card card)
		{
			if(characters.Count == 0)
				base.OnDrewSecondCard(card);
			else
				characters.ForEach(c => c.OnDrewSecondCard(card));
		}
		public override void OnAfterDraw()
		{
			if(characters.Count == 0)
				base.OnAfterDraw();
			else
				characters.ForEach(c => c.OnAfterDraw());
		}

		public override void PlayCard(Card card)
		{
			if(characters.Count == 0)
				base.PlayCard(card);
			else if(characters.Count == 1)
				characters.First().PlayCard(card);
			else
				Game.GameCycle.PushTempHandler(new ChooseCharacterResponseHandler(this, RequestType.ChooseCharacterForPlayCard, c =>
				{
					if(c != null)
						c.PlayCard(card);
					else
						base.PlayCard(card);
				}));
		}
		public override void UseAbility()
		{
			if(characters.Count == 0)
				base.UseAbility();
			else if(characters.Count == 1)
				characters.First().UseAbility();
			else
				Game.GameCycle.PushTempHandler(new ChooseCharacterResponseHandler(this, RequestType.ChooseCharacterForUseAbility, c =>
				{
					if(c != null)
						c.UseAbility();
				}));
		}
		public override void CheckDeck(Card causedBy, CheckDeckCallback checkCallback, CardResultCallback resultCallback)
		{
			if(characters.Count == 0)
				base.CheckDeck(causedBy, checkCallback, resultCallback);
			else if(characters.Count == 1)
				characters.First().CheckDeck(causedBy, checkCallback, resultCallback);
			else
				Game.GameCycle.PushTempHandler(new ChooseCharacterResponseHandler(this, RequestType.ChooseCharacterForCheckDeck, c =>
				{
					if(c != null)
						c.CheckDeck(causedBy, checkCallback, resultCallback);
					else
						base.CheckDeck(causedBy, checkCallback, resultCallback);
				}));
		}
		public override void CheckMissed(CardResultCallback resultCallback)
		{
			if(characters.Count == 0)
				base.CheckMissed(resultCallback);
			else if(characters.Count == 1)
				characters.First().CheckMissed(resultCallback);
			else
				Game.GameCycle.PushTempHandler(new ChooseCharacterResponseHandler(this, RequestType.ChooseCharacterForAvoidShot, c => {
					if(c != null)
						c.CheckMissed(resultCallback);
					else
						base.CheckMissed(resultCallback);
				}));
		}
		public override void OnHit(int hitPoints, Player causedBy)
		{
			if(characters.Count == 0)
				base.OnHit(hitPoints, causedBy);
			else
				characters.ForEach(c => c.OnHit(hitPoints, causedBy));
		}
		public override void OnEmptyHand()
		{
			if(characters.Count == 0)
				base.OnEmptyHand();
			else
				characters.ForEach(c => c.OnEmptyHand());
		}
		public override void OnPlayerDied(Player player)
		{
			if(characters.Count == 0)
				base.OnPlayerDied(player);
			else
				characters.ForEach(c => c.OnPlayerDied(player));
		}
		public override void OnTurnStarted()
		{
			if(characters.Count == 0)
				base.OnTurnStarted();
			else
				characters.ForEach(c => c.OnTurnStarted());
		}
		public override void OnPlayContinue()
		{
			if(characters.Count == 0)
				base.OnPlayContinue();
			else
				characters.ForEach(c => c.OnPlayContinue());
		}
		public override void OnTurnEnded()
		{
			if(characters.Count == 0)
				base.OnTurnEnded();
			else
				characters.ForEach(c => c.OnTurnEnded());
		}
		public override void OnPlayedCard(Card card)
		{
			if(characters.Count == 0)
				base.OnPlayedCard(card);
			else
				characters.ForEach(c => c.OnPlayedCard(card));
		}
		public override void OnRespondedWithCard(Card card)
		{
			if(characters.Count == 0)
				base.OnRespondedWithCard(card);
			else
				characters.ForEach(c => c.OnRespondedWithCard(card));
		}
		public override bool SavePlayer()
		{
			return characters.Count == 0 ? base.SavePlayer() : characters.Any(c => c.SavePlayer());
		}
	}
}
