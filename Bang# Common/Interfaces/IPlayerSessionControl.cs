// IPlayerSessionControl.cs
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
	/// Provides methods and properties available to the player to interact with the session.
	/// </summary>
	public interface IPlayerSessionControl
	{
		/// <summary>
		/// Gets the <see cref="IPlayer"/> controlled by this <see cref="IPlayerSessionControl"/>
		/// </summary>
		IPlayer Player { get; }

		/// <summary>
		/// Gets the <see cref="ISession"/> instance representing the session.
		/// </summary>
		ISession Session { get; }

		/// <summary>
		/// Sends a chat message to the players and spectators in the session.
		/// </summary>
		/// <param name="message">
		/// The message to be sent.
		/// </param>
		void SendChatMessage(string message);

		/// <summary>
		/// Disconnects the player from the session (and also removes if the session has not yet started).
		/// </summary>
		void Disconnect();

		/// <summary>
		/// Starts a new game. Can be only called by the creator of the session.
		/// </summary>
		void StartGame();
		
		/// <summary>
		/// Ends the session. Can be only called by the creator of the session.
		/// </summary>
		void EndSession();
	}
}
