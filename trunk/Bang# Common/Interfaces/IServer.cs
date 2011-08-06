// IServer.cs
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
	/// Provides information about a game server.
	/// </summary>
	public interface IServer
	{
		/// <summary>
		/// Gets the name of the server.
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Gets the description of the server.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the major version of the interface implementation on the server.
		/// </summary>
		int InterfaceVersionMajor { get; }
		/// <summary>
		/// Gets the minor version of the interface implementation on the server.
		/// </summary>
		int InterfaceVersionMinor { get; }
		
		/// <summary>
		/// Gets the collection of the sessions currently hosted on the server.
		/// </summary>
		ReadOnlyCollection<ISession> Sessions { get; }
		
		/// <summary>
		/// Creates a new session on the server.
		/// </summary>
		/// <param name="sessionData">
		/// The <see cref="CreateSessionData"/> containing the information about the session.
		/// </param>
		/// <param name="playerData">
		/// The <see cref="CreatePlayerData"/> containing the information about the creator.
		/// </param>
		/// <param name="listener">
		/// The <see cref="PlayerEventListener"/> of the creator.
		/// </param>
		void CreateSession(CreateSessionData sessionData, CreatePlayerData playerData, IPlayerEventListener listener);

		ISession GetSession(int id);
	}
}

