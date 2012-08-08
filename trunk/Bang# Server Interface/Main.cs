// Main.cs
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
using System.Runtime.Serialization;
using BangSharp.ConsoleUtils;

namespace BangSharp.Server.Interface
{
	public sealed class ServerInterface : ImmortalMarshalByRefObject, IServerEventListener
	{
		private static readonly ServerInterface Instance = new ServerInterface();

		public static void Main(string[] cmdArgs)
		{
			Console.Title = "Bang# Server Interface";
			ConsoleHelper.PrintLine("Bang# Server Interface");
			ConsoleHelper.PrintLine("----------------------");
			ConsoleHelper.PrintLine("Interface version: {0}.{1}", Utils.InterfaceVersionMajor, Utils.InterfaceVersionMinor);
			ConsoleHelper.PrintLine("Server interface version: {0}.{1}", ServerUtils.InterfaceVersionMajor, ServerUtils.InterfaceVersionMinor);
			ConsoleHelper.PrintLine("Operating system: {0}", Environment.OSVersion);
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
			try
			{
				ConsoleHelper.PrintLine("Connecting to {0} on port {1}...", address, port);
				ServerUtils.OpenClientAdminChannel();
				IServerBase server = ServerUtils.ConnectAdmin(address, port);

				ConsoleHelper.PrintLine();

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
				server.RegisterListener(Instance);
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
				rootCmd["changepassword"] = new FinalCommand(cmd =>
				{
					ConsoleHelper.Print("Current password: ");
					Password currentPassword = ConsoleHelper.ReadPassword();
					ConsoleHelper.Print("New password: ");
					Password newPassword = ConsoleHelper.ReadPassword();
					ConsoleHelper.Print("Confirm new password: ");
					if(!newPassword.CheckPassword(ConsoleHelper.ReadPassword()))
					{
						ConsoleHelper.ErrorLine("The new passwords don't match!");
						return;
					}
					try
					{
						server.ChangePassword(currentPassword, newPassword);
					}
					catch(BadServerPasswordException)
					{
						ConsoleHelper.ErrorLine("Bad server password!");
						return;
					}
					ConsoleHelper.SuccessLine("Server password changed successfully!");
				});
				rootCmd["exit"] = new FinalCommand(cmd =>
				{
					server.UnregisterListener(Instance);
					Environment.Exit(0);
				});
				while(true) // command-line loop
				{
					try
					{
						rootCmd.ReadAndExecute();
					}
					catch(InvalidOperationException)
					{
						ConsoleHelper.ErrorLine("Invalid command!");
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

		#region IServerEventListener implementation
		void IServerEventListener.Ping()
		{
		}

		void IServerEventListener.OnSessionCreated(ISession session)
		{
			ConsoleHelper.ServerEvent("Session #{0} '{1}' has been created.", session.ID, session.Name);
		}
		void IServerEventListener.OnSessionEnded(ISession session)
		{
			ConsoleHelper.ServerEvent("Session #{0} '{1}' has ended.", session.ID, session.Name);
		}

		void IServerEventListener.OnGameStarted(ISession session)
		{
			ConsoleHelper.ServerEvent("The game has started in session #{0}.", session.ID, session.Name);
		}
		void IServerEventListener.OnGameEnded(ISession session)
		{
			ConsoleHelper.ServerEvent("The game has ended in session #{0}.", session.ID, session.Name);
		}

		void IServerEventListener.OnPlayerJoinedSession(ISession session, IPlayer player)
		{
			ConsoleHelper.ServerEvent("Player #{0} '{1}' joined session #{2}.", player.ID, player.Name, session.ID);
		}
		void IServerEventListener.OnSpectatorJoinedSession(ISession session, ISpectator spectator)
		{
			ConsoleHelper.ServerEvent("Spectator #{0} '{1}' joined session #{2}.", spectator.ID, spectator.Name, session.ID);
		}
		void IServerEventListener.OnPlayerLeftSession(ISession session, IPlayer player)
		{
			ConsoleHelper.ServerEvent("Player #{0} '{1}' left session #{2}.", player.ID, player.Name, session.ID);
		}
		void IServerEventListener.OnSpectatorLeftSession(ISession session, ISpectator spectator)
		{
			ConsoleHelper.ServerEvent("Spectator #{0} '{1}' left session #{2}.", spectator.ID, spectator.Name, session.ID);
		}
		void IServerEventListener.OnPlayerUpdated(ISession session, IPlayer player)
		{
			ConsoleHelper.ServerEvent("Player #{0} '{1}' was updated in session #{2}.", player.ID, player.Name, session.ID);
		}
		void IServerEventListener.OnPlayerDisconnected(ISession session, IPlayer player)
		{
			ConsoleHelper.ServerEvent("Player #{0} '{1}' disconnected from session #{2}.", player.ID, player.Name, session.ID);
		}
		#endregion
	}
}
