// SpectateSessionDialog.cs
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

namespace BangSharp.Client
{
	public partial class SpectateSessionDialog : Gtk.Dialog
	{
		private ISession session;

		public SpectateSessionDialog(ServerWindow parent, ISession session)
		{
			TransientFor = parent;

			this.Build();

			this.session = session;
		}

		protected void OnResponse(object o, Gtk.ResponseArgs args)
		{
			if(args.ResponseId != Gtk.ResponseType.Ok)
			{
				Destroy();
				return;
			}

			this.Sensitive = false;
			System.Threading.ThreadPool.QueueUserWorkItem((state) => {
				CreateSpectatorData csd = spectatorDataWidget.SpectatorData;
				try
				{
					session.Spectate(new Password(sessionPasswordEntry.Text), csd, ConnectionManager.SessionEventListener);
				}
				catch(Exception ex)
				{
					Gtk.Application.Invoke(delegate {
						Gdk.Threads.Enter();
						ErrorManager.ShowErrorMessage(this, MessageManager.GetErrorMessage(ex));
						Destroy();
						Gdk.Threads.Leave();
					});
					return;
				}
				Gtk.Application.Invoke(delegate {
					Gdk.Threads.Enter();
					Destroy();
					Gdk.Threads.Leave();
				});
			});
		}
	}
}

