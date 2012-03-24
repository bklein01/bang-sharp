// ISession.cs
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

namespace BangSharp
{
	/// <summary>
	/// Provides information about a session.
	/// </summary>
	public interface ISession : IIdentificable
	{
		/// <summary>
		/// Gets the name of the session.
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Gets the desription of the session.
		/// </summary>
		string Description { get; }
		/// <summary>
		/// Gets the current state of the session.
		/// </summary>
		SessionState State { get; }
		
		/// <summary>
		/// Gets the minimum number of players in this session to start a game.
		/// </summary>
		int MinPlayers { get; }
		/// <summary>
		/// Gets the maximum number of players in this session.
		/// </summary>
		int MaxPlayers { get; }
		/// <summary>
		/// Gets the maximum number of spectators in this session.
		/// </summary>
		int MaxSpectators { get; }
		/// <summary>
		/// Gets a value indicating wheter the session is password protected for players.
		/// </summary>
		bool HasPlayerPassword { get; }
		/// <summary>
		/// Gets a value indicating wheter the session is password protected for spectators.
		/// </summary>
		bool HasSpectatorPassword { get; }
		/// <summary>
		/// Gets the id of the creator of the session.
		/// </summary>
		IPlayer Creator { get; }
		
		/// <summary>
		/// Gets a value indicating wheter the Dodge City extension is active in the session.
		/// </summary>
		bool DodgeCity { get; }
		/// <summary>
		/// Gets a value indicating wheter the High Noon extension is active in the session.
		/// </summary>
		bool HighNoon { get; }
		/// <summary>
		/// Gets a value indicating wheter the Fistful of Cards extension is active in the session.
		/// </summary>
		bool FistfulOfCards { get; }
		/// <summary>
		/// Gets a value indicating wheter the Wild West Show extension is active in the session.
		/// </summary>
		bool WildWestShow { get; }

		/// <summary>
		/// Gets the number of games that have been played in this session.
		/// </summary>
		int GamesPlayed { get; }
		
		/// <summary>
		/// Gets the collection of players in the session.
		/// </summary>
		ReadOnlyCollection<IPlayer> Players { get; }
		/// <summary>
		/// Gets the collection of spectators in the session.
		/// </summary>
		ReadOnlyCollection<ISpectator> Spectators { get; }
		
		/// <summary>
		/// Joins a new player to the session.
		/// </summary>
		/// <param name="password">
		/// The session password.
		/// </param>
		/// <param name="data">
		/// The <see cref="BangSharp.CreatePlayerData"/> contatining the information about the player.
		/// </param>
		/// <param name="listener">
		/// The <see cref="BangSharp.IPlayerSessionEventListener"/> of the player.
		/// </param>
		void Join(Password password, CreatePlayerData data, IPlayerSessionEventListener listener);
		/// <summary>
		/// Replaces an existing player with a new one. Only players without controller or AI players can be replaced.
		/// </summary>
		/// <param name="id">
		/// The ID of th player to be replaced.
		/// </param>
		/// <param name="password">
		/// The session password.
		/// </param>
		/// <param name="data">
		/// The <see cref="BangSharp.CreatePlayerData"/> contatining the information about the new player.
		/// </param>
		/// <param name="listener">
		/// The <see cref="BangSharp.PlayerEventListener"/> of the new player.
		/// </param>
		void Replace(int id, Password password, CreatePlayerData data, IPlayerSessionEventListener listener);
		/// <summary>
		/// Joins a new spectator to the session.
		/// </summary>
		/// <param name="password">
		/// The session password.
		/// </param>
		/// <param name="data">
		/// The <see cref="BangSharp.CreateSpectatorData"/> contatining the information about the spectator.
		/// </param>
		/// <param name="listener">
		/// The <see cref="BangSharp.ISpectatorSessionEventListener"/> of the spectator.
		/// </param>
		void Spectate(Password password, CreateSpectatorData data, ISpectatorSessionEventListener listener);
		/// <summary>
		/// Gets the player with the specified ID.
		/// </summary>
		/// <param name="id">
		/// The ID of the player.
		/// </param>
		/// <returns>
		/// The <see cref="BangSharp.IPlayer"/> instance representing the player.
		/// </returns>
		IPlayer GetPlayer(int id);
		/// <summary>
		/// Gets the spectator with the specified ID.
		/// </summary>
		/// <param name="id">
		/// The ID of the spectator.
		/// </param>
		/// <returns>
		/// The <see cref="BangSharp.ISpectator"/> instance representing the spectator.
		/// </returns>
		ISpectator GetSpectator(int id);
	}
}
