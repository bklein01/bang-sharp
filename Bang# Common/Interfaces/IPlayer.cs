// IPlayer.cs
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
namespace Bang
{
	/// <summary>
	/// Provides information about a player.
	/// </summary>
	public interface IPlayer : IIdentificable
	{
		/// <summary>
		/// Gets the name of the player.
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Gets the avatar of the player.
		/// </summary>
		byte[] Image { get; }

		/// <summary>
		/// Gets the value indicating wheter the player has a login password.
		/// </summary>
		bool HasPassword { get; }
		/// <summary>
		/// Gets the value indicating wheter the player is the creator of this session.
		/// </summary>
		bool IsCreator { get; }
		/// <summary>
		/// Gets the value indicating wheter the player is a bot.
		/// </summary>
		bool IsAI { get; }
		/// <summary>
		/// Gets the value indicating wheter the player has a controller assigned (is logged in).
		/// </summary>
		bool HasListener { get; }

		/// <summary>
		/// Gets the current score of the player calculated according to the official rules.
		/// </summary>
		/// <remarks>
		/// The scoring system description can be found here:
		/// http://www.bang.cz/en/rules-and-faq/special-rules/64-official-tournament-scoring-system.html
		/// </remarks>
		int Score { get; }
		/// <summary>
		/// Gets the number of turns that player has played.
		/// </summary>
		int TurnsPlayed { get; }

		/// <summary>
		/// Gets the number of wins of this player.
		/// </summary>
		int Victories { get; }
		/// <summary>
		/// Gets the number of wins of this player as sheriff.
		/// </summary>
		int VictoriesAsSheriff { get; }
		/// <summary>
		/// Gets the number of wins of this player as deputy.
		/// </summary>
		int VictoriesAsDeputy { get; }
		/// <summary>
		/// Gets the number of wins of this player as outlaw.
		/// </summary>
		int VictoriesAsOutlaw { get; }
		/// <summary>
		/// Gets the number of wins of this player as renegade.
		/// </summary>
		int VictoriesAsRenegade { get; }
	}
}

