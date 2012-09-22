// PreferencesDialog.cs
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
using Mono.Unix;

namespace BangSharp.Client
{
	public partial class PreferencesDialog : Gtk.Dialog
	{
		public PreferencesDialog(MainWindow parent)
		{
			TransientFor = parent;

			this.Build();

			autoHideSWWhilePlayingCheckbox.TooltipMarkup = Catalog.GetString("If checked, the Session Window automatically hides when the game starts and shows when the game ends.");
			animDelayEntry.TooltipMarkup = Catalog.GetString("Minimum time between rendering of frames of the game board. Decreasing this value may get you higher FPS, but it may also increase the CPU utilisation.");
			animDurationEntry.TooltipMarkup = Catalog.GetString("The default duration of animation (card movement, etc).");
			requestTimeoutEntry.TooltipMarkup = Catalog.GetString("The amount of time to wait for the server to return a response.");

			autoHideSWWhilePlayingCheckbox.Active = Config.Instance.GetBoolean("Client.AutoHideSWWhilePlaying", true);
			animDelayEntry.Value = Config.Instance.GetInteger("Client.AnimDelay", 100);
			animDurationEntry.Value = Config.Instance.GetInteger("Client.DefaultAnimDuration", 1000);
			requestTimeoutEntry.Value = Config.Instance.GetInteger("Client.RequestTimeout", 30000);
		}

		protected void OnResponse(object o, Gtk.ResponseArgs args)
		{
			if(args.ResponseId == Gtk.ResponseType.Ok || args.ResponseId == Gtk.ResponseType.Apply)
			{
				Config.Instance.SetBoolean("Client.AutoHideSWWhilePlaying", autoHideSWWhilePlayingCheckbox.Active);
				Config.Instance.SetInteger("Client.AnimDelay", animDelayEntry.ValueAsInt);
				Config.Instance.SetInteger("Client.DefaultAnimDuration", animDurationEntry.ValueAsInt);
				Config.Instance.SetInteger("Client.RequestTimeout", requestTimeoutEntry.ValueAsInt);
			}
			if(args.ResponseId == Gtk.ResponseType.Ok || args.ResponseId == Gtk.ResponseType.Cancel || args.ResponseId == Gtk.ResponseType.DeleteEvent)
				Destroy();
		}
	}
}

