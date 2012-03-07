// SessionSpectatorControl.cs
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
using System;

namespace BangSharp.Server.Daemon
{
	public sealed class SessionSpectatorControl : ImmortalMarshalByRefObject, ISpectatorSessionControl
	{
		private SessionSpectator spectator;

		public SessionSpectator Spectator
		{
			get { return spectator; }
		}
		public Session Session
		{
			get { return spectator.Session; }
		}
		ISession ISpectatorSessionControl.Session
		{
			get { return spectator.Session; }
		}
		ISpectator ISpectatorSessionControl.Spectator
		{
			get { return spectator; }
		}

		public SessionSpectatorControl(SessionSpectator spectator)
		{
			this.spectator = spectator;
		}

		void ISpectatorSessionControl.SendChatMessage(string message)
		{
			if(!spectator.HasListener)
				throw new InvalidOperationException();

			Session session = Session;
			lock(session)
			{
				if(session.Locked)
					throw new MethodAccessException();

				session.EventManager.SendChatMessage(spectator, message);
			}
		}
		
		void ISpectatorSessionControl.Disconnect()
		{
			if(!spectator.HasListener)
				throw new InvalidOperationException();
			Session.RemoveSpectator(spectator);
		}
	}
}
