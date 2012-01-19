// Server.cs
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Bang.Server
{
	public sealed class Server : ImmortalMarshalByRefObject, IServerBase
	{
		private sealed class ServerAdmin : ImmortalMarshalByRefObject, IServerAdmin
		{
			private Server parent;

			public ServerAdmin(Server parent)
			{
				this.parent = parent;
			}

			void IServerAdmin.ResetSessions()
			{
				parent.ResetSessions();
			}

			ISessionAdmin IServerAdmin.GetSessionAdmin(int id)
			{
				return parent.GetSession(id).Admin;
			}
		}
		private static Server instance = null;
		private ServerAdmin admin;
		private Dictionary<int, Session> sessions;

		public readonly object Lock = new object();
		public bool Locked
		{
			get;
			set;
		}
		public IServerAdmin Admin
		{
			get { return admin; }
		}

		public static Server Instance
		{
			get { return instance; }
		}
		public string Name
		{
			get { return Config.Instance.GetString("Server.Name", "Bang# Server"); }
		}
		public string Description
		{
			get { return Config.Instance.GetString("Server.Description", ""); }
		}
		int IServer.InterfaceVersionMajor
		{
			get { return Utils.InterfaceVersionMajor; }
		}
		int IServer.InterfaceVersionMinor
		{
			get { return Utils.InterfaceVersionMinor; }
		}
		int IServerBase.ServerInterfaceVersionMajor
		{
			get { return ServerUtils.InterfaceVersionMajor; }
		}
		int IServerBase.ServerInterfaceVersionMinor
		{
			get { return ServerUtils.InterfaceVersionMinor; }
		}
		ReadOnlyCollection<ISession> IServer.Sessions
		{
			get { return new ReadOnlyCollection<ISession>(new List<Session>(sessions.Values).ConvertAll<ISession>(s => s)); }
		}
		public ReadOnlyCollection<Session> Sessions
		{
			get { return new ReadOnlyCollection<Session>(new List<Session>(sessions.Values)); }
		}

		public Server()
		{
			admin = new ServerAdmin(this);
			if(!LoadState())
				sessions = new Dictionary<int, Session>();
			instance = this;
		}

		public void Write(BinaryWriter writer)
		{
			List<Session> sessionList = new List<Session>(sessions.Values.Where(s => s.State != SessionState.WaitingForPlayers));
			writer.Write(sessionList.Count);
			foreach(Session s in sessionList)
				s.Write(writer);
		}

		private static readonly string StatePath = Path.Combine(Utils.ConfigFolder, "ServerState.bin");
		private static readonly char[] StateMagic = "BangSharp".ToCharArray();
		private static readonly uint StateVersion = 2;
		private bool LoadState()
		{
			Console.Error.Write("INFO: Loading server state... ");
			try
			{
				Stream stream = File.OpenRead(StatePath);
				using(BinaryReader reader = new BinaryReader(stream))
				{
					char[] magic = reader.ReadChars(StateMagic.Length);
					if(!magic.SequenceEqual(StateMagic))
						return false;
					uint version = reader.ReadUInt32();
					if(version != StateVersion)
						return false;

					int sessionCount = reader.ReadInt32();
					if(sessionCount < 0)
						return false;

					sessions = new Dictionary<int, Session>(sessionCount);
					for(int i = 0; i < sessionCount; i++)
					{
						Session session = new Session(this, reader);
						sessions.Add(session.ID, session);
					}
				}
				Console.Error.WriteLine("Success!");
				return true;
			}
			catch
			{
				Console.Error.WriteLine("Error!");
				return false;
			}
		}
		public void SaveState()
		{
			lock(Lock)
			{
				Console.Error.Write("INFO: Saving server state... ");
				try
				{
					if(!new FileInfo(StatePath).Exists)
						Directory.CreateDirectory(Utils.ConfigFolder);
					Stream stream = File.Create(StatePath);
					using(BinaryWriter writer = new BinaryWriter(stream))
					{
						writer.Write(StateMagic);
						writer.Write(StateVersion);
						Write(writer);
					}
					Console.Error.WriteLine("Success!");
				}
				catch
				{
					Console.Error.WriteLine("Error!");
				}
			}
		}

		public void CreateSession(CreateSessionData sessionData, CreatePlayerData playerData, IPlayerEventListener listener)
		{
			lock(Lock)
			{
				if(Locked)
					throw new MethodAccessException();

				int id = sessions.GenerateID();
				Session session = new Session(this, id, sessionData);
				sessions.Add(id, session);
				Console.Error.WriteLine("INFO: Created session #{0}.", id);

				session.Join(sessionData.PlayerPassword, playerData, listener);
				SaveState();
			}
		}

		IServerAdmin IServerBase.GetServerAdmin(Password password)
		{
			Password serverPassword;
			try
			{
				serverPassword = new Password(Config.Instance.GetIntegerList("Server.AdminPassword").ToArray());
			}
			catch(ArgumentOutOfRangeException)
			{
				serverPassword = new Password("");
			}
			if(!serverPassword.CheckPassword(password))
				throw new BadServerPasswordException();
			return admin;
		}
		void IServerBase.ChangePassword(Password password, Password newPassword)
		{
			Password serverPassword;
			try
			{
				serverPassword = new Password(Config.Instance.GetIntegerList("Server.AdminPassword").ToArray());
			}
			catch(ArgumentOutOfRangeException)
			{
				serverPassword = new Password("");
			}
			if(!serverPassword.CheckPassword(password))
				throw new BadServerPasswordException();
			Config.Instance.SetIntegerList("Server.AdminPassword", newPassword.Hash.ToList());
		}

		public void ResetSessions()
		{
			lock(Lock)
			{
				if(Locked)
					throw new MethodAccessException();

				Console.Error.WriteLine("INFO: Resetting sessions...");
				List<Session> sessionList = new List<Session>(sessions.Values);
				foreach(Session s in sessionList)
					s.End();
				SaveState();
			}
		}
		public void RemoveSession(Session session)
		{
			lock(Lock)
			{
				Console.Error.WriteLine("INFO: Removing session #{0}...", session.ID);
				sessions.Remove(session.ID);
				session.Disconnect();
				SaveState();
			}
		}
		public Session GetSession(int id)
		{
			try
			{
				return sessions[id];
			}
			catch(KeyNotFoundException)
			{
				throw new InvalidIdException();
			}
		}
		ISession IServer.GetSession(int id)
		{
			try
			{
				return sessions[id];
			}
			catch(KeyNotFoundException)
			{
				throw new InvalidIdException();
			}
		}
	}
}