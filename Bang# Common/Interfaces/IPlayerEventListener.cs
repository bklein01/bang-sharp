// IPlayerEventListener.cs
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
	/// Represents the event listener for a player.
	/// </summary>
	public interface IPlayerEventListener : IEventListener
	{
		/// <summary>
		/// Gets a value indicating wheter this listener implementation is an AI.
		/// </summary>
		bool IsAI { get; }
		/// <summary>
		/// Called when the player successfully joins a session.
		/// </summary>
		/// <param name="control">
		/// The <see cref="Bang.IPlayerSessionControl"/> instance for this player.
		/// </param>
		void OnJoinedSession(IPlayerSessionControl control);
		/// <summary>
		/// Called when the player successfully joins a game.
		/// </summary>
		/// <param name="control">
		/// The <see cref="Bang.IPlayerControl"/> instance for this player.
		/// </param>
		void OnJoinedGame(IPlayerControl control);

		/// <summary>
		/// Fired when the player is requested for an action.
		/// </summary>
		/// <param name="requestType">
		/// The new <see cref="Bang.RequestType"/>.
		/// </param>
		/// <param name="causedBy">
		/// The <see cref="Bang.IPublicPlayerView"/> of the player that caused the request.
		/// </param>
		void OnNewRequest(RequestType requestType, IPublicPlayerView causedBy);
	}
}
