// Main.cs
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
using Bang.ConsoleUtils;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
namespace Bang.Server
{
	static class MainClass
	{
		public static void Main(string[] cmdArgs)
		{
			ConsoleHelper.PrintLine("Bang# Server Interface");
			ConsoleHelper.PrintLine("----------------------");

			string address;
			string portString;
			if(cmdArgs.Length != 2)
			{
				ConsoleHelper.Print("Server Address: ");
				address = ConsoleHelper.ReadLine();
				ConsoleHelper.Print("Server Administration Port: ");
				portString = ConsoleHelper.ReadLine();
			}
			else
			{
				address = cmdArgs[0];
				portString = cmdArgs[1];
			}
			int port;
			try
			{
				port = int.Parse(portString);
			}
			catch(FormatException)
			{
				ConsoleHelper.ErrorLine("Bad number format!");
				return;
			}

			ConsoleHelper.PrintLine("Connecting to {0} on port {1}...", address, port);
			IServerBase server = ServerUtils.ConnectAdmin(address, port);

			ConsoleHelper.PrintLine();

			try
			{
				if(!Utils.IsServerCompatible(server))
				{
					ConsoleHelper.ErrorLine("Server version {0}.{1} not compatible with client version {2}.{3}!",
						server.InterfaceVersionMajor, server.InterfaceVersionMinor,
						Utils.InterfaceVersionMajor, Utils.InterfaceVersionMinor);
					return;
				}
				if(!ServerUtils.IsServerCompatible(server))
				{
					ConsoleHelper.ErrorLine("Server administration version {0}.{1} not compatible with interface version {2}.{3}!",
						server.ServerInterfaceVersionMajor, server.ServerInterfaceVersionMinor,
						ServerUtils.InterfaceVersionMajor, ServerUtils.InterfaceVersionMinor);
					return;
				}
				ConsoleHelper.PrintLine("Server name: {0}", server.Name);
				ConsoleHelper.PrintLine("Server description: {0}", server.Description);
				ConsoleHelper.SuccessLine("Connection estabilished!");
				ConsoleHelper.PrintLine();

				ConsoleHelper.Print("Server password: ");
				Password password = ConsoleHelper.ReadPassword();
				IServerAdmin serverAdmin;
				try
				{
					serverAdmin = server.GetServerAdmin(password);
				}
				catch(BadServerPasswordException)
				{
					ConsoleHelper.ErrorLine("Password rejected!");
					return;
				}
				ConsoleHelper.SuccessLine("Password accepted!");
				ConsoleHelper.PrintLine();

				NestedCommand rootCmd = new NestedCommand();
				NestedCommand<IServer> serverCmd = new NestedCommand<IServer>(cmd => server);
				serverCmd.MakeServerCommand();
				rootCmd["server"] = serverCmd;
				NestedCommand<IServerAdmin> serverAdminCmd = new NestedCommand<IServerAdmin>(cmd => serverAdmin);
				serverAdminCmd.MakeServerAdminCommand();
				rootCmd["serveradmin"] = serverAdminCmd;
				rootCmd["exit"] = new FinalCommand(cmd => Environment.Exit(0));
				while (true) // command-line loop
				{
					try
					{
						rootCmd.ReadAndExecute();
					}
					catch(InvalidOperationException)
					{
						ConsoleHelper.ErrorLine("Too few arguments!");
					}
				}
			}
			catch(RemotingException e)
			{
				ConsoleHelper.ErrorLine("Remoting error!");
#if DEBUG
				ConsoleHelper.DebugLine(e.ToString());
#endif
				return;
			}
			catch(SerializationException e)
			{
				ConsoleHelper.ErrorLine("Serialization error!");
#if DEBUG
				ConsoleHelper.DebugLine(e.ToString());
#endif
				return;
			}
		}

		public static void MakeSessionAdminCommand<In>(this NestedCommand<In, ISessionAdmin> command)
		{
			command["end"] = new FinalCommand<ISessionAdmin>((sessionAdmin, cmd) => {
				sessionAdmin.End();
				ConsoleHelper.SuccessLine("Session ended!");
			});
		}
		public static void MakeServerAdminCommand<In>(this NestedCommand<In, IServerAdmin> command)
		{
			command["resetsessions"] = new FinalCommand<IServerAdmin>((serverAdmin, cmd) =>
			{
				serverAdmin.ResetSessions();
				ConsoleHelper.SuccessLine("All sessions ended!");
			});
			NestedCommand<IServerAdmin, ISessionAdmin> sessionCommand = new NestedCommand<IServerAdmin, ISessionAdmin>((serverAdmin, cmd) =>
			{
				try
				{
					int id = int.Parse(cmd.Dequeue());
					return serverAdmin.GetSessionAdmin(id);
				}
				catch(FormatException)
				{
					ConsoleHelper.ErrorLine("Bad number format!");
				}
				return null;
			});
			sessionCommand.MakeSessionAdminCommand();
			command["session"] = sessionCommand;
		}
	}
}

