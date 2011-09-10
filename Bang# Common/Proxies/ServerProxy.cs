// ServerProxy.cs
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
using System.Collections.Generic;
namespace Bang
{
	public class ServerProxy<ServerType> : MarshalByRefObject, IServer
		where ServerType : IServer, new()
	{
		private IServer raw;

		string IServer.Name
		{
			get { return raw.Name; }
		}
		string IServer.Description
		{
			get { return raw.Description; }
		}
		int IServer.InterfaceVersionMajor
		{
			get { return raw.InterfaceVersionMajor; }
		}
		int IServer.InterfaceVersionMinor
		{
			get { return raw.InterfaceVersionMinor; }
		}
		ReadOnlyCollection<ISession> IServer.Sessions
		{
			get { return raw.Sessions; }
		}

		public ServerProxy()
		{
			raw = new ServerType();
		}

		public ServerType GetServerObject(Password serverPassword)
		{
			Password password;
			try
			{
				password = new Password(Config.Instance.GetIntegerList("Server.Password").ToArray());
			}
			catch(ArgumentOutOfRangeException)
			{
				password = new Password("");
			}
			if(!password.CheckPassword(serverPassword))
				throw new BadServerPasswordException();
			return (ServerType)raw;
		}
		public void ChangePassword(Password currentPassword, Password newPassword)
		{
			Password password;
			try
			{
				password = new Password(Config.Instance.GetIntegerList("Server.Password").ToArray());
			}
			catch(ArgumentOutOfRangeException)
			{
				password = new Password("");
			}
			if(!password.CheckPassword(currentPassword))
				throw new BadServerPasswordException();
			Config.Instance.SetIntegerList("Server.Password", new List<int>(newPassword.Hash));
		}

		void IServer.CreateSession (CreateSessionData sessionData, CreatePlayerData playerData, IPlayerEventListener listener)
		{
			raw.CreateSession(sessionData, playerData, listener);
		}
		ISession IServer.GetSession (int id)
		{
			return raw.GetSession(id);
		}
	}
}

