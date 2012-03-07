// SessionPlayer.cs
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
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BangSharp.Server
{
	public sealed class SessionPlayer : ImmortalMarshalByRefObject, IPlayer
	{
		private int id;
		private Session session;
		private CreatePlayerData data;
		private SessionPlayerControl control;
		private IPlayerSessionEventListener listener;

		private int score;
		private int turnsPlayed;
		private int victories;
		private Dictionary<Role, int> roleVictories;
		private Dictionary<CharacterType, int> characterVictories;

		public int ID
		{
			get { return id; }
		}
		public string Name
		{
			get { return data.Name; }
		}
		public byte[] Image
		{
			get { return data.Image; }
		}
		public bool HasPassword
		{
			get { return !data.Password.IsEmpty; }
		}
		public bool IsCreator
		{
			get { return id == session.Creator.ID; }
		}
		public bool IsAI
		{
			get { return listener == null ? false : listener.IsAI; }
		}
		public bool HasListener
		{
			get
			{
				if(listener == null)
					return false;
				try
				{
					session.Locked = true;
					listener.Ping();
					session.Locked = false;
					return true;
				}
				catch(RemotingException)
				{
					Console.Error.WriteLine("INFO: Ping failed, removing player...");
					session.Locked = false;
					session.RemovePlayer(this);
					return false;
				}
			}
		}
		
		public int Score
		{
			get { return score; }
		}
		public int TurnsPlayed
		{
			get { return turnsPlayed; }
		}

		public int Victories
		{
			get { return victories; }
		}

		public Session Session
		{
			get { return session; }
		}
		public SessionPlayerControl Control
		{
			get { return control; }
		}
		public IPlayerSessionEventListener Listener
		{
			get { return listener; }
		}
		
		public SessionPlayer(int id, Session session, CreatePlayerData data)
		{
			this.id = id;
			this.session = session;
			this.data = data;
			control = new SessionPlayerControl(this);

			score = 0;
			turnsPlayed = 0;
			victories = 0;
			List<Role> roles = Utils.GetRoles();
			roleVictories = new Dictionary<Role, int>(roles.Count);
			foreach(Role role in roles)
				roleVictories[role] = 0;
			List<CharacterType> characters = Utils.GetCharacterTypes(session);
			characterVictories = new Dictionary<CharacterType, int>(characters.Count);
			foreach(CharacterType character in characters)
				characterVictories[character] = 0;
		}
		public SessionPlayer(Session session, BinaryReader reader)
		{
			this.session = session;
			id = reader.ReadInt32();
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				data = (CreatePlayerData)bf.Deserialize(reader.BaseStream);
			}
			catch(InvalidCastException)
			{
				throw new FormatException();
			}
			catch(SerializationException)
			{
				throw new FormatException();
			}

			control = new SessionPlayerControl(this);
			
			score = reader.ReadInt32();
			if(score < 0)
				throw new FormatException();
			turnsPlayed = reader.ReadInt32();
			if(turnsPlayed < 0)
				throw new FormatException();

			victories = reader.ReadInt32();
			if(victories < 0)
				throw new FormatException();

			int roleVicCount = reader.ReadInt32();
			roleVictories = new Dictionary<Role, int>(roleVicCount);
			for(int i = 0; i < roleVicCount; i++)
			{
				Role role = (Role)reader.ReadInt32();
				int vic = reader.ReadInt32();
				if(vic < 0)
					throw new FormatException();
				roleVictories.Add(role, vic);
			}

			int characterVicCount = reader.ReadInt32();
			characterVictories = new Dictionary<CharacterType, int>(characterVicCount);
			for(int i = 0; i < characterVicCount; i++)
			{
				CharacterType character = (CharacterType)reader.ReadInt32();
				int vic = reader.ReadInt32();
				if(vic < 0)
					throw new FormatException();
				characterVictories.Add(character, vic);
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(id);
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(writer.BaseStream, data);
			}
			catch(SerializationException)
			{
				throw new IOException();
			}
			writer.Write(score);
			writer.Write(turnsPlayed);
			writer.Write(victories);
			writer.Write(roleVictories.Count);
			foreach(KeyValuePair<Role, int> item in roleVictories)
			{
				writer.Write((int)item.Key);
				writer.Write(item.Value);
			}
			writer.Write(characterVictories.Count);
			foreach(KeyValuePair<CharacterType, int> item in characterVictories)
			{
				writer.Write((int)item.Key);
				writer.Write(item.Value);
			}
		}

		public override void Disconnect()
		{
			base.Disconnect();
			control.Disconnect();
		}

		public int GetVictories(Role role)
		{
			if(role == Role.Unknown)
				return victories;

			if(!roleVictories.ContainsKey(role))
				return 0;

			return roleVictories[role];
		}
		public int GetVictories(CharacterType character)
		{
			if(character == CharacterType.Unknown)
				return victories;
			
			if(!characterVictories.ContainsKey(character))
				return 0;

			return characterVictories[character];
		}

		public void Update(CreatePlayerData data)
		{
			if(!this.data.Password.CheckPassword(data.Password))
				throw new BadPlayerPasswordException();
			this.data = data;
		}
		public void RegisterListener(IPlayerSessionEventListener listener)
		{
			this.listener = listener;
		}
		public void UnregisterListener()
		{
			listener = null;
			ResetControl();
		}
		public void ResetControl()
		{
			control.Disconnect();
			control = new SessionPlayerControl(this);
			Game game = session.Game;
			if(game != null)
			{
				Player p = game.GetPlayer(id);
				p.ResetControl();
			}
		}

		public void UpdateScore(int score)
		{
			this.score += score;
		}
		public void UpdateTurnsPlayed(int turnsPlayed)
		{
			this.turnsPlayed += turnsPlayed;
		}
		public void RegisterVictory(Role role, CharacterType character)
		{
			victories++;
			roleVictories[role]++;
			characterVictories[character]++;
		}
	}
}
