// SessionSpectator.cs
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
using System.Runtime.Remoting;

namespace BangSharp.Server.Daemon
{
	public sealed class SessionSpectator : ImmortalMarshalByRefObject, ISpectator
	{
		private int id;
		private Session session;
		private CreateSpectatorData data;
		private SessionSpectatorControl control;
		private ISpectatorSessionEventListener listener;
		
		public int ID
		{
			get { return id; }
		}
		public string Name
		{
			get { return data.Name; }
		}
		public byte[] Image
		{
			get { return data.Image; }
		}
		
		public Session Session
		{
			get { return session; }
		}
		public SessionSpectatorControl Control
		{
			get { return control; }
		}
		public ISpectatorSessionEventListener Listener
		{
			get { return listener; }
		}
		public bool HasListener
		{
			get
			{
				if(listener == null)
					return false;
				try
				{
					session.Locked = true;
					listener.Ping();
					session.Locked = false;
					return true;
				}
				catch(RemotingException)
				{
					Console.Error.WriteLine("INFO: Ping failed, removing player...");
					session.RemoveSpectator(this);
					return false;
				}
			}
		}
		
		public SessionSpectator(int id, Session session, CreateSpectatorData data)
		{
			this.id = id;
			this.session = session;
			this.data = data;
			control = new SessionSpectatorControl(this);
		}

		public override void Disconnect()
		{
			base.Disconnect();
			control.Disconnect();
		}

		public void RegisterListener(ISpectatorSessionEventListener listener)
		{
			this.listener = listener;
		}
		public void UnregisterListener()
		{
			listener = null;
		}
	}
}
