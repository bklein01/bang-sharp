// IPublicPlayerView.cs
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
using System.Collections.ObjectModel;
namespace Bang
{
	/// <summary>
	/// Provides the player's properties as seen by other players (and spectators).
	/// </summary>
	public interface IPublicPlayerView : IIdentificable
	{
		/// <summary>
		/// Gets the value indicating wheter the player is a sheriff.
		/// </summary>
		bool IsSheriff { get; }
		/// <summary>
		/// Gets the value indicating wheter the player is alive.
		/// </summary>
		bool IsAlive { get; }
		/// <summary>
		/// Gets the value indicating wheter the player is a winner.
		/// </summary>
		bool IsWinner { get; }
		/// <summary>
		/// Gets the number of life points of the player.
		/// </summary>
		int LifePoints { get; }
		/// <summary>
		/// Gets the maximum number of life points of the player.
		/// </summary>
		int MaxLifePoints { get; }
		/// <summary>
		/// Gets the collection of the cards that the player is holding in his hand.
		/// </summary>
		ReadOnlyCollection<ICard> Hand { get; }
		/// <summary>
		/// Gets the collection of the cards that the player has on the table.
		/// </summary>
		ReadOnlyCollection<ICard> Table { get; }
		/// <summary>
		/// Gets the character of the player.
		/// </summary>
		CharacterType CharacterType { get; }
		/// <summary>
		/// Gets the collection of characters whose abilities the player gained thanks to his main character.
		/// </summary>
		ReadOnlyCollection<CharacterType> AdditionalCharacters { get; }
		/// <summary>
		/// Gets the role of the player.
		/// </summary>
		Role Role { get; }
	}
}

