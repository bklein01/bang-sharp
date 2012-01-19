// IGame.cs
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
using System.Collections.ObjectModel;

namespace Bang
{
	/// <summary>
	/// Provides the game's properties visible to the players (and spectators).
	/// </summary>
	public interface IGame
	{
		/// <summary>
		/// Gets the collection of the players in the game.
		/// </summary>
		ReadOnlyCollection<IPublicPlayerView> Players { get; }
		
		/// <summary>
		/// Gets the card that is on the top of the graveyard.
		/// </summary>
		ICard GraveyardTop { get; }

		/// <summary>
		/// Gets the public view of the temporary card selection.
		/// </summary>
		ReadOnlyCollection<ICard> Selection { get; }

		/// <summary>
		/// Gets the <see cref="Bang.IPublicPlayerView"/> of the player currently on turn.
		/// </summary>
		IPublicPlayerView CurrentPlayer { get; }
		/// <summary>
		/// Gets the <see cref="Bang.IPublicPlayerView"/> of the player currently requested for a response.
		/// </summary>
		IPublicPlayerView RequestedPlayer { get; }
		/// <summary>
		/// Gets the <see cref="Bang.IPublicPlayerView"/> of the player that caused the current request, or null if no player caused it.
		/// </summary>
		IPublicPlayerView CausedBy { get; }
		
		/// <summary>
		/// Gets the <see cref="Bang.IPublicPlayerView"/> of the player with the specified ID.
		/// </summary>
		/// <param name="id">
		/// The ID of the player.
		/// </param>
		/// <returns>
		/// The <see cref="Bang.IPublicPlayerView"/> of the player.
		/// </returns>
		IPublicPlayerView GetPublicPlayerView(int id);
	}
}
