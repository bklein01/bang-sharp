// ImageSelectorWidget.cs
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
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ImageSelectorWidget : Gtk.Bin
	{
		private bool imageSelected;

		public Gdk.Pixbuf Image
		{
			get
			{
				if(!imageSelected)
					return null;
				return image.Pixbuf;
			}
		}

		public string Filename
		{
			get { return imageSelected ? chooser.Filename : null; }
			set
			{
				if(value != null)
					chooser.SetFilename(value);
				else
					chooser.UnselectAll();
			}
		}

		public event Action OnImageChanged;

		public ImageSelectorWidget()
		{
			this.Build();
			this.clearButton.TooltipMarkup = Catalog.GetString("Clears the image");

			Gtk.FileFilter filter = new Gtk.FileFilter();
			filter.AddPixbufFormats();
			filter.Name = Catalog.GetString("Picture files");
			chooser.AddFilter(filter);
			filter = new Gtk.FileFilter();
			filter.Name = Catalog.GetString("All files");
			chooser.AddFilter(filter);
			
			image.Pixbuf = ResourceManager.GetPixbuf("Resources", "DefaultPlayerImage.png");
		}

		protected void OnClearButtonClicked(object sender, System.EventArgs e)
		{
			chooser.UnselectAll();
		}

		protected void OnChooserSelectionChanged(object sender, System.EventArgs e)
		{
			if(chooser.Filename == null)
			{
				imageSelected = false;
				image.Pixbuf = ResourceManager.GetPixbuf("Resources", "DefaultPlayerImage.png");
			}
			else
			{
				try
				{
					Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(chooser.Filename);
					image.Pixbuf = pixbuf;
					imageSelected = true;
				}
				catch
				{
					ErrorManager.ShowWarningMessage((Gtk.Window)this.GetAncestor(Gtk.Window.GType), Catalog.GetString("Unable to load image!"));

					chooser.UnselectAll();
					return;
				}
			}
			if(OnImageChanged != null)
				OnImageChanged();
		}
	}
}

