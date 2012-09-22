// SpectatorDataWidget.cs
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

namespace BangSharp.Client
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class SpectatorDataWidget : Gtk.Bin
	{
		public CreateSpectatorData SpectatorData
		{
			get
			{
				return new CreateSpectatorData(nameEntry.Text, PictureManager.GetBytes(imageSelector.Image));
			}
		}

		public SpectatorDataWidget()
		{
			this.Build();

			nameEntry.Text = Config.Instance.GetString("Client.Spectator.Name", "");
			string savedFname = Config.Instance.GetString("Client.Spectator.ImageFilename", null);
			if(savedFname != null)
				imageSelector.Filename = savedFname;

			imageSelector.OnImageChanged += () => {
				string fname = imageSelector.Filename;
				if(fname != null)
					Config.Instance.SetString("Client.Spectator.ImageFilename", fname);
				else
					Config.Instance.Clear("Client.Spectator.ImageFilename");
			};
		}

		protected void OnNameEntryChanged(object sender, System.EventArgs e)
		{
			Config.Instance.SetString("Client.Spectator.Name", nameEntry.Text);
		}
	}
}

