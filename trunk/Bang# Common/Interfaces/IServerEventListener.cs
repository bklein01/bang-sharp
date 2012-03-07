// IServerEventListener.cs
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
namespace BangSharp
{
	/// <summary>
	/// Contains public server and session events.
	/// </summary>
	public interface IServerEventListener
	{
		/// <summary>
		/// Used when checking the availability of the listener. Should be implemented as an empty method.
		/// </summary>
		void Ping();

		/// <summary>
		/// Fired when a session has been created on the server.
		/// </summary>
		/// <param name="session">
		/// The <see cref="BangSharp.ISession"/> that has been created.
		/// </param>
		void OnSessionCreated(ISession session);
		/// <summary>
		/// Fired when a session has ended.
		/// </summary>
		/// <param name="session">
		/// The <see cref="BangSharp.ISession"/> that has ended.
		/// </param>
		void OnSessionEnded(ISession session);

		/// <summary>
		/// Fired when a game has started in a session.
		/// </summary>
		/// <param name="session">
		/// The <see cref="BangSharp.ISession"/> that has changed.
		/// </param>
		void OnGameStarted(ISession session);
		/// <summary>
		/// Fired when a game has ended in a session.
		/// </summary>
		/// <param name="session">
		/// The <see cref="BangSharp.ISession"/> that has changed.
		/// </param>
		void OnGameEnded(ISession session);

		/// <summary>
		/// Fired when a player has joined a session.
		/// </summary>
		/// <param name="session">
		/// The <see cref="BangSharp.ISession"/> that has changed.
		/// </param>
		/// <param name="player">
		/// The <see cref="BangSharp.IPlayer"/> that has joined the session.
		/// </param>
		void OnPlayerJoinedSession(ISession session, IPlayer player);
		/// <summary>
		/// Fired when a spectator has joined a session.
		/// </summary>
		/// <param name="session">
		/// The <see cref="BangSharp.ISession"/> that has changed.
		/// </param>
		/// <param name="spectator">
		/// The <see cref="BangSharp.ISpectator"/> that has joined the session.
		/// </param>
		void OnSpectatorJoinedSession(ISession session, ISpectator spectator);
		/// <summary>
		/// Fired when a player has left a session.
		/// </summary>
		/// <param name="session">
		/// The <see cref="BangSharp.ISession"/> that has changed.
		/// </param>
		/// <param name="player">
		/// The <see cref="BangSharp.IPlayer"/> that has left the session.
		/// </param>
		void OnPlayerLeftSession(ISession session, IPlayer player);
		/// <summary>
		/// Fired when a spectator has left a session.
		/// </summary>
		/// <param name="session">
		/// The <see cref="BangSharp.ISession"/> that has changed.
		/// </param>
		/// <param name="spectator">
		/// The <see cref="BangSharp.ISpectator"/> that has left the session.
		/// </param>
		void OnSpectatorLeftSession(ISession session, ISpectator spectator);
		/// <summary>
		/// Fired when a player has updated.
		/// </summary>
		/// <param name="session">
		/// The <see cref="BangSharp.ISession"/> that has changed.
		/// </param>
		/// <param name="player">
		/// The <see cref="BangSharp.IPlayer"/> that has updated.
		/// </param>
		void OnPlayerUpdated(ISession session, IPlayer player);
	}
}

