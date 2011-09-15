// ResponseHandler.cs
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
using System.Collections.Generic;
namespace Bang.Server
{
	public class ResponseHandler
	{
		private ResponseHandler parent;
		private Stack<ResponseHandler> current;
		private RequestType type;
		private Player requested;
		private Player causedBy;
		private bool active;
		
		public virtual Game Game
		{
			get { return parent == null ? null : parent.Game; }
		}

		public ResponseHandler Parent
		{
			get { return parent; }
		}

		public RequestType RequestType
		{
			get
			{
				if (current.Count != 0)
					return current.Peek().RequestType;
				else
					return type;
			}
		}
		public Player RequestedPlayer
		{
			get
			{
				if (current.Count != 0)
					return current.Peek ().RequestedPlayer;
				else
					return requested;
			}
		}
		public Player CausedBy
		{
			get
			{
				if (current.Count != 0)
					return current.Peek ().CausedBy;
				else
					return causedBy;
			}
		}
		
		public bool Active
		{
			get { return active; }
		}

		protected ResponseHandler (RequestType type, Player requested, Player causedBy)
		{
			this.type = type;
			this.requested = requested;
			this.causedBy = causedBy;
			current = new Stack<ResponseHandler> ();
			active = false;
		}
		protected ResponseHandler(RequestType type, Player requested)
			: this(type, requested, null)
		{
		}
		protected ResponseHandler(RequestType type)
			: this(type, null, null)
		{
		}
		protected ResponseHandler (Player requested, Player causedBy)
			: this(RequestType.None, requested, causedBy)
		{
		}
		protected ResponseHandler(Player requested)
			: this(RequestType.None, requested, null)
		{
		}
		protected ResponseHandler()
			: this(RequestType.None, null, null)
		{
		}

		protected void PushHandler (ResponseHandler handler)
		{
			if (!active)
				throw new InvalidOperationException ();
			handler.parent = this;
			
			current.Push (handler);
			handler.Start ();
		}
		protected void End ()
		{
			active = false;
			if (parent == null)
				return;
			
			ResponseHandler temp = parent;
			parent = null;
			temp.Next ();
		}

		protected void Start ()
		{
			if (active)
				throw new InvalidOperationException ();
			active = true;
			OnStart ();
		}
		protected virtual void OnStart ()
		{
		}
		private void Next()
		{
			if(!active)
				throw new InvalidOperationException();
			if(current.Count == 0)
				throw new InvalidOperationException();
			
			while(!current.Peek().active)
			{
				current.Pop();
				if(current.Count == 0)
				{
					OnNext();
					return;
				}
			}
			current.Peek ().Continue ();
		}
		protected virtual void OnNext ()
		{
			End ();
		}
		private void Continue ()
		{
			if (current.Count == 0)
				OnContinue ();
			else
				current.Peek ().Continue ();
		}
		protected virtual void OnContinue ()
		{
		}

		protected void RespondDraw()
		{
			if (!active)
				throw new InvalidOperationException ();
			if(current.Count != 0)
				current.Peek().RespondDraw();
			else
				OnRespondDraw();
		}
		protected void RespondCard(Card card)
		{
			if (!active)
				throw new InvalidOperationException ();
			if (current.Count != 0)
				current.Peek ().RespondCard(card);
			else
				OnRespondCard(card);
		}
		protected void RespondPlayer(Player player)
		{
			if (current.Count != 0)
				current.Peek ().RespondPlayer(player);
			else
				OnRespondPlayer(player);
		}
		protected void RespondNoAction()
		{
			if (!active)
				throw new InvalidOperationException ();
			if (current.Count != 0)
				current.Peek ().RespondNoAction();
			else
				OnRespondNoAction();
		}
		protected void RespondUseAbility()
		{
			if (!active)
				throw new InvalidOperationException ();
			if (current.Count != 0)
				current.Peek ().RespondUseAbility();
			else
				OnRespondUseAbility();
		}

		protected virtual void OnRespondDraw ()
		{
			throw new BadUsageException();
		}
		protected virtual void OnRespondCard(Card card)
		{
			throw new BadUsageException ();
		}
		protected virtual void OnRespondPlayer(Player player)
		{
			throw new BadUsageException ();
		}
		protected virtual void OnRespondNoAction()
		{
			throw new BadUsageException ();
		}
		protected virtual void OnRespondUseAbility()
		{
			throw new BadUsageException ();
		}
	}
}

