// SessionProxy.cs
//  
// Author:  WOnder93 <omosnacek@gmail.com>
// 
// Copyright (c) 2011 Ondrej Mosnáček
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
using System;
using System.Collections.ObjectModel;
namespace Bang
{
	public class SessionProxy : MarshalByRefObject, ISession
	{
		private ISession raw;

		int IIdentificable.ID
		{
			get { return raw.ID; }
		}
		string ISession.Name
		{
			get { return raw.Name; }
		}
		string ISession.Description
		{
			get { return raw.Description; }
		}
		SessionState ISession.State
		{
			get { return raw.State; }
		}
		int ISession.MinPlayers
		{
			get { return raw.MinPlayers; }
		}
		int ISession.MaxPlayers
		{
			get { return raw.MaxPlayers; }
		}
		int ISession.MaxSpectators
		{
			get { return raw.MaxSpectators; }
		}
		bool ISession.HasPlayerPassword
		{
			get { return raw.HasPlayerPassword; }
		}
		bool ISession.HasSpectatorPassword
		{
			get { return raw.HasSpectatorPassword; }
		}
		IPlayer ISession.Creator
		{
			get { return raw.Creator; }
		}
		bool ISession.DodgeCity
		{
			get { return raw.DodgeCity; }
		}
		bool ISession.HighNoon
		{
			get { return raw.HighNoon; }
		}
		bool ISession.FistfulOfCards
		{
			get { return raw.FistfulOfCards; }
		}
		bool ISession.WildWestShow
		{
			get { return raw.WildWestShow; }
		}
		int ISession.GamesPlayed
		{
			get { return raw.GamesPlayed; }
		}
		ReadOnlyCollection<IPlayer> ISession.Players
		{
			get { return raw.Players; }
		}
		ReadOnlyCollection<ISpectator> ISession.Spectators
		{
			get { return raw.Spectators; }
		}

		public SessionProxy(ISession raw)
		{
			this.raw = raw;
		}

		void ISession.Join(Password password, CreatePlayerData data, IPlayerEventListener listener)
		{
			raw.Join(password, data, listener);
		}
		void ISession.Replace(int id, Password password, CreatePlayerData data, IPlayerEventListener listener)
		{
			raw.Replace(id, password, data, listener);
		}
		void ISession.Spectate(Password password, CreateSpectatorData data, ISpectatorEventListener listener)
		{
			raw.Spectate(password, data, listener);
		}
		IPlayer ISession.GetPlayer(int id)
		{
			return raw.GetPlayer(id);
		}
		ISpectator ISession.GetSpectator(int id)
		{
			return raw.GetSpectator(id);
		}
	}
}

