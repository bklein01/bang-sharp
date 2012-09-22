// ServerListWindow.cs
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
using System.Collections.Generic;
using Mono.Unix;

namespace BangSharp.Client
{
	public partial class ServerListWindow : Gtk.Window
	{
		[Gtk.TreeNode]
		private class ServerNode : Gtk.TreeNode
		{
			private string address;
			private int port;

			[Gtk.TreeNodeValue(Column=0)]
			public string Address
			{
				get { return address; }
				set { address = value; }
			}
			[Gtk.TreeNodeValue(Column=1)]
			public int Port
			{
				get { return port; }
				set { port = value; }
			}

			public ServerNode(string address, int port)
			{
				this.address = address;
				this.port = port;
			}
		}
		private Gtk.NodeStore serverStore;
		private List<ServerNode> serverNodes;

		public ServerListWindow(MainWindow parent) :
			base(Gtk.WindowType.Toplevel)
		{
			TransientFor = parent;
			
			this.Build();
			this.connectButton.TooltipMarkup = Catalog.GetString("Connects to the selected server");
			this.addServerButton.TooltipMarkup = Catalog.GetString("Adds new server entry to the list");
			this.removeServerButton.TooltipMarkup = Catalog.GetString("Removes the selected entry from the list");

			serverStore = new Gtk.NodeStore(typeof(ServerNode));
			serverNodes = new List<ServerNode>();

			List<string> addresses = Config.Instance.GetStringList("ServerList.Addresses");
			List<int> ports = Config.Instance.GetIntegerList("ServerList.Ports");
			int count = Math.Min(addresses.Count, ports.Count);
			for(int i = 0; i < count; i++)
			{
				ServerNode node = new ServerNode(addresses[i], ports[i]);
				serverStore.AddNode(node);
				serverNodes.Add(node);
			}

			serverList.NodeStore = serverStore;

			Gtk.CellRendererText addressRenderer = new Gtk.CellRendererText();
			addressRenderer.Editable = true;
			addressRenderer.Edited += OnAddressEdited;
			Gtk.CellRendererText portRenderer = new Gtk.CellRendererText();
			portRenderer.Editable = true;
			portRenderer.Edited += OnPortEdited;
			serverList.AppendColumn(Catalog.GetString("Address"), addressRenderer, "text", 0);
			serverList.AppendColumn(Catalog.GetString("Port"), portRenderer, "text", 1);

			serverList.ShowExpanders = false;

			serverList.NodeSelection.Changed += OnServerSelectionChanged;
		}

		void OnAddressEdited(object o, Gtk.EditedArgs args)
		{
			ServerNode node = (ServerNode)serverStore.GetNode(new Gtk.TreePath(args.Path));
			node.Address = args.NewText;

			int i = serverNodes.IndexOf(node);
			List<string> addresses = Config.Instance.GetStringList("ServerList.Addresses");
			addresses[i] = node.Address;
			Config.Instance.SetStringList("ServerList.Addresses", addresses);
		}

		void OnPortEdited(object o, Gtk.EditedArgs args)
		{
			ServerNode node = (ServerNode)serverStore.GetNode(new Gtk.TreePath(args.Path));
			try
			{
				node.Port = int.Parse(args.NewText);
			}
			catch(FormatException)
			{
			}

			int i = serverNodes.IndexOf(node);
			List<int> ports = Config.Instance.GetIntegerList("ServerList.Ports");
			ports[i] = node.Port;
			Config.Instance.SetIntegerList("ServerList.Ports", ports);
		}

		void OnServerSelectionChanged(object sender, EventArgs e)
		{
			ServerNode sel = (ServerNode)serverList.NodeSelection.SelectedNode;
			if(sel == null)
			{
				connectButton.Sensitive = false;
				removeServerButton.Sensitive = false;
			}
			else
			{
				connectButton.Sensitive = true;
				removeServerButton.Sensitive = true;
			}
		}

		protected void OnConnectButtonClicked(object sender, System.EventArgs e)
		{
			Gtk.NodeSelection sel = serverList.NodeSelection;
			if(sel.SelectedNode == null)
				return;

			ServerNode node = (ServerNode)sel.SelectedNode;
			try
			{
				ConnectionManager.ConnectToServer(node.Address, node.Port);
			}
			catch(Exception ex)
			{
				ErrorManager.ShowErrorMessage(this, MessageManager.GetErrorMessage(ex));
				return;
			}
			Destroy();
		}

		protected void OnAddServerButtonClicked(object sender, System.EventArgs e)
		{
			ServerNode node = new ServerNode("", 0);
			serverStore.AddNode(node);
			serverNodes.Add(node);

			List<string> addresses = Config.Instance.GetStringList("ServerList.Addresses");
			List<int> ports = Config.Instance.GetIntegerList("ServerList.Ports");
			addresses.Add(node.Address);
			ports.Add(node.Port);
			Config.Instance.SetStringList("ServerList.Addresses", addresses);
			Config.Instance.SetIntegerList("ServerList.Ports", ports);
		}

		protected void OnRemoveServerButtonClicked(object sender, System.EventArgs e)
		{
			Gtk.NodeSelection sel = serverList.NodeSelection;
			if(sel.SelectedNode == null)
				return;

			ServerNode node = (ServerNode)sel.SelectedNode;
			int i = serverNodes.IndexOf(node);
			serverStore.RemoveNode(node);
			serverNodes.Remove(node);

			List<string> addresses = Config.Instance.GetStringList("ServerList.Addresses");
			List<int> ports = Config.Instance.GetIntegerList("ServerList.Ports");
			addresses.RemoveAt(i);
			ports.RemoveAt(i);
			Config.Instance.SetStringList("ServerList.Addresses", addresses);
			Config.Instance.SetIntegerList("ServerList.Ports", ports);
		}
	}
}

