// QueueResponseHandler.cs
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
	public class QueueResponseHandler : ResponseHandler
	{
		private Queue<ResponseHandler> queue;

		protected QueueResponseHandler (Player requested, Player causedBy)
			: base(requested, causedBy)
		{
			queue = new Queue<ResponseHandler> ();
		}
		protected QueueResponseHandler(Player requested)
			: this(requested, null)
		{
		}
		protected QueueResponseHandler() :
			this(null, null)
		{
		}
		protected QueueResponseHandler (Player requested, Player causedBy, int capacity)
			: base(requested, causedBy)
		{
			queue = new Queue<ResponseHandler> (capacity);
		}
		protected QueueResponseHandler(Player requested, int capacity)
			: this(requested, null, capacity)
		{
		}
		protected QueueResponseHandler(int capacity)
			: this(null, null, capacity)
		{
		}
		public QueueResponseHandler(List<ResponseHandler> handlers)
			: base()
		{
			queue = new Queue<ResponseHandler>(handlers.Count);
			foreach(ResponseHandler h in handlers)
				queue.Enqueue(h);
		}

		protected void AddHandler (ResponseHandler handler)
		{
			queue.Enqueue (handler);
		}

		protected override void OnStart ()
		{
			try {
				PushHandler (queue.Dequeue ());
			} catch (InvalidOperationException) {
				End ();
			}
		}
		protected override void OnNext ()
		{
			try {
				PushHandler (queue.Dequeue ());
			} catch (InvalidOperationException) {
				End ();
			}
		}
	}
}

