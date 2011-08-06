// CreateSessionData.cs
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
using System;
namespace Bang
{
	/// <summary>
	/// A struct used to pass the information needed for creating a session from a client to a server.
	/// </summary>
	[Serializable]
	public struct CreateSessionData
	{
		public string Name
		{
			get;
			set;
		}
		public string Description
		{
			get;
			set;
		}
		public int MinPlayers
		{
			get;
			set;
		}
		public int MaxPlayers
		{
			get;
			set;
		}
		public int MaxSpectators
		{
			get;
			set;
		}
		public Password PlayerPassword
		{
			get;
			set;
		}
		public Password SpectatorPassword
		{
			get;
			set;
		}
		public bool ShufflePlayers
		{
			get;
			set;
		}
		public bool DodgeCity
		{
			get;
			set;
		}
		public bool HighNoon
		{
			get;
			set;
		}
		public bool FistfulOfCards
		{
			get;
			set;
		}
		public bool WildWestShow
		{
			get;
			set;
		}

		public CreateSessionData(string name, string description,
			int minPlayers, int maxPlayers, int maxSpectators,
			string playerPassword, string spectatorPassword, bool shufflePlayers,
			bool dodgeCity, bool highNoon,
			bool fistfulOfCards, bool wildWestShow)
		{
			Name = name;
			Description = description;
			MinPlayers = minPlayers;
			MaxPlayers = maxPlayers;
			MaxSpectators = maxSpectators;
			PlayerPassword = new Password(playerPassword);
			SpectatorPassword = new Password(spectatorPassword);
			ShufflePlayers = shufflePlayers;
			DodgeCity = dodgeCity;
			HighNoon = highNoon;
			FistfulOfCards = fistfulOfCards;
			WildWestShow = wildWestShow;
		}
	}
}

