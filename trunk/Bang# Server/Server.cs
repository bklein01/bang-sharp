// Server.cs
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
namespace Bang.Server
{
	public sealed class Server : MarshalByRefObject, IServer
	{
		private static Server instance = null;
		private Dictionary<int, Session> sessions;
		private bool locked;

		public static Server Instance
		{
			get { return instance; }
		}
		public string Name
		{
			get { return Config.Instance.GetString ("Server.Name", "Bang# Server"); }
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
		public ReadOnlyCollection<ISession> Sessions
		{
			get { return new ReadOnlyCollection<ISession>(new List<Session>(sessions.Values).ConvertAll<ISession>(s => s)); }
		}
		
		public Server()
		{
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
		private static readonly char[] Magic = "Bang".ToCharArray();
		private bool LoadState()
		{
			try
			{
				Stream stream = File.OpenRead(StatePath);
				BinaryReader reader = new BinaryReader(stream);
				char[] magic = reader.ReadChars(4);
				if(!magic.SequenceEqual(Magic))
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
				return true;
			}
			catch
			{
				return false;
			}
		}
		public void SaveState()
		{
			try
			{
				if(!File.Exists(StatePath))
					Directory.CreateDirectory(Utils.ConfigFolder);
				Stream stream = File.Create(StatePath);
				BinaryWriter writer = new BinaryWriter(stream);
				writer.Write(Magic);
				Write(writer);
				writer.Close();
			}
			catch
			{
			}
		}

		public void CreateSession(CreateSessionData sessionData, CreatePlayerData playerData, IPlayerEventListener listener)
		{
			lock(this)
			{
				if(locked)
					throw new InvalidOperationException();
				locked = true;
				int id = sessions.GenerateID();
				Session session = new Session(this, id, sessionData);
				sessions.Add(id, session);

				session.Join(sessionData.PlayerPassword, playerData, listener);
				SaveState();
				locked = false;
			}
		}
		public void RemoveSession(Session session)
		{
			sessions.Remove(session.ID);
			SaveState();
		}
		public void ResetSessions()
		{
			lock(this)
			{
				List<Session> sessionList = new List<Session>(sessions.Values);
				foreach(Session s in sessionList)
					s.End();
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