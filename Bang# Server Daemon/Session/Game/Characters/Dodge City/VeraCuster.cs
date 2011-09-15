// VeraCuster.cs
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
namespace Bang.Server.Characters
{
	public sealed class VeraCuster : Character
	{
		private sealed class VeraCusterResponseHandler : ResponseHandler
		{
			private VeraCuster parent;

			public VeraCusterResponseHandler(VeraCuster parent)
				: base(RequestType.VeraCuster, parent.Player)
			{
				this.parent = parent;
			}

			protected override void OnRespondPlayer(Player player)
			{
				if(player == RequestedPlayer)
					throw new BadPlayerException();
				
				if(!player.IsAlive)
					throw new BadPlayerException();
				
				if(player.CharacterType == CharacterType.VeraCuster)
					// you never know...
					throw new BadPlayerException();
				
				parent.character = Character.GetCharacter(RequestedPlayer, player.CharacterType);
				End();
			}
			protected override void OnRespondNoAction()
			{
				End();
			}
		}
		private Character character;

		public override int MaxLifePoints
		{
			get { return 3; }
		}
		public override int InitialCardCount
		{
			get { return character == null ? base.InitialCardCount : character.InitialCardCount; }
		}
		public override int MaxCardCount
		{
			get { return character == null ? base.MaxCardCount : character.MaxCardCount; }
		}
		public override int DistanceIn
		{
			get { return character == null ? base.DistanceIn : character.DistanceIn; }
		}
		public override int DistanceOut
		{
			get { return character == null ? base.DistanceOut : character.DistanceOut; }
		}
		public override int BangPower
		{
			get { return character == null ? base.BangPower : character.BangPower; }
		}
		public override int BeerPower
		{
			get { return character == null ? base.BeerPower : character.BeerPower; }
		}
		public override bool UnlimitedBangs
		{
			get { return character == null ? base.UnlimitedBangs : character.UnlimitedBangs; }
		}
		public override bool TakesDeadPlayersCards
		{
			get { return character == null ? base.TakesDeadPlayersCards : character.TakesDeadPlayersCards; }
		}

		public VeraCuster(Player player)
			: base(player, CharacterType.VeraCuster)
		{
		}

		public override bool HasCardEffect(Card card)
		{
			return character == null ? base.HasCardEffect(card) : character.HasCardEffect(card);
		}
		public override bool IsMissed(Card card)
		{
			return character == null ? base.IsMissed(card) : character.IsMissed(card);
		}
		public override bool IsBang(Card card)
		{
			return character == null ? base.IsBang(card) : character.IsBang(card);
		}
		public override bool CanPlayCard(CardType card)
		{
			return character == null ? base.CanPlayCard(card) : character.CanPlayCard(card);
		}

		public override void Draw()
		{
			if(character == null)
				base.Draw();
			else
				character.Draw();
		}
		public override void PlayCard(Card card)
		{
			if(character == null)
				base.PlayCard(card);
			else
				character.PlayCard(card);
		}
		public override void UseAbility()
		{
			if(character == null)
				base.UseAbility();
			else
				character.UseAbility();
		}
		public override void CheckDeck(Card causedBy, CheckDeckMethod checkMethod, ICardResultHandler handler)
		{
			if(character == null)
				base.CheckDeck(causedBy, checkMethod, handler);
			else
				character.CheckDeck(causedBy, checkMethod, handler);
		}
		public override void CheckMissed(ICardResultHandler handler)
		{
			if(character == null)
				base.CheckMissed(handler);
			else
				character.CheckMissed(handler);
		}

		public override void OnHit(int hitPoints, Player causedBy)
		{
			if(character == null)
				base.OnHit(hitPoints, causedBy);
			else
				character.OnHit(hitPoints, causedBy);
		}
		public override void OnEmptyHand()
		{
			if(character == null)
				base.OnEmptyHand();
			else
				character.OnEmptyHand();
		}
		public override void OnPlayerDied(Player player)
		{
			if(character == null)
				base.OnPlayerDied(player);
			else
				character.OnPlayerDied(player);
		}
		public override void OnTurnStarted()
		{
			if(!Player.SkipTurn)
				Game.GameCycle.PushTempHandler(new VeraCusterResponseHandler(this));
			else
				character = null;
		}
		public override void OnPlayContinue ()
		{
			if(character == null)
				base.OnPlayContinue();
			else
				character.OnPlayContinue();
		}
		public override void OnTurnEnded()
		{
			if(character == null)
				base.OnTurnEnded();
			else
				character.OnTurnEnded();
		}
		public override void OnPlayedCard(Card card)
		{
			if(character == null)
				base.OnPlayedCard(card);
			else
				character.OnPlayedCard(card);
		}
		public override void OnRespondedWithCard(Card card)
		{
			if(character == null)
				base.OnRespondedWithCard(card);
			else
				character.OnRespondedWithCard(card);
		}
		public override bool SavePlayer()
		{
			return character == null ? base.SavePlayer() : character.SavePlayer();
		}
	}
}

