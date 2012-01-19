// IPlayerControl.cs
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
namespace Bang
{
	/// <summary>
	/// Provides methods and properties available to the player to interact with the game.
	/// </summary>
	public interface IPlayerControl
	{
		/// <summary>
		/// Gets the current <see cref="Bang.IGame"/>.
		/// </summary>
		IGame Game { get; }

		/// <summary>
		/// Gets the <see cref="Bang.IPrivatePlayerView"/> of the player.
		/// </summary>
		IPrivatePlayerView PrivatePlayerView { get; }
		
		/// <summary>
		/// Responds to the game with the intention to draw cards.
		/// </summary>
		void RespondDraw();
		/// <summary>
		/// Responds to the game with the specified card.
		/// </summary>
		/// <param name="id">
		/// The ID of the card.
		/// </param>
		void RespondCard(int id);
		/// <summary>
		/// Responds to the game with the specified player.
		/// </summary>
		/// <param name="id">
		/// The ID of the player.
		/// </param>
		void RespondPlayer(int id);
		/// <summary>
		/// Responds to the game with the specified character.
		/// </summary>
		/// <param name="type">
		/// A <see cref="Bang.CharacterType"/>
		/// </param>
		void RespondCharacter(CharacterType type);
		/// <summary>
		/// Responds to the game with the intention to perform no more action.
		/// </summary>
		void RespondNoAction();
		/// <summary>
		/// Responds to the game with the intention to use the player's character's ability.
		/// </summary>
		void RespondUseAbility();
	}
}
