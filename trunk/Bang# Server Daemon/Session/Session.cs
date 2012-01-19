// Session.cs
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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bang.Server
{
	public sealed class Session : ImmortalMarshalByRefObject, ISession
	{
		private sealed class SessionAdmin : ImmortalMarshalByRefObject, ISessionAdmin
		{
			private Session parent;

			public SessionAdmin(Session parent)
			{
				this.parent = parent;
			}

			void ISessionAdmin.End()
			{
				parent.End();
			}
		}
		private SessionAdmin admin;
		private Server server;
		private int id;
		private CreateSessionData data;
		private SessionState state;
		private Dictionary<int, SessionPlayer> players;
		private List<SessionPlayer> playerList;
		private Dictionary<int, SessionSpectator> spectators;
		private List<SessionSpectator> spectatorList;
		private SessionEventManager eventMgr;
		private Game game;
		
		private int creatorId;
		private int gamesPlayed;
		private IEnumerator<SessionPlayer> sheriffEnumerator;
		private List<CharacterType> remainingCharacters;

		public readonly object Lock = new object();
		private bool locked;
		public bool Locked
		{
			get { return locked; }
			set { server.Locked = locked = value; }
		}
		public ISessionAdmin Admin
		{
			get { return admin; }
		}
		public int ID
		{
			get { return id; }
		}
		public string Name
		{
			get { return data.Name; }
		}
		public string Description
		{
			get { return data.Description; }
		}
		public SessionState State
		{
			get { return state; }
		}
		public int MinPlayers
		{
			get { return data.MinPlayers; }
		}
		public int MaxPlayers
		{
			get { return data.MaxPlayers; }
		}
		public int MaxSpectators
		{
			get { return data.MaxSpectators; }
		}
		public bool HasPlayerPassword
		{
			get { return !data.PlayerPassword.IsEmpty; }
		}
		public bool HasSpectatorPassword
		{
			get { return !data.SpectatorPassword.IsEmpty; }
		}
		public SessionPlayer Creator
		{
			get { return players[creatorId]; }
		}
		IPlayer ISession.Creator
		{
			get { return players[creatorId]; }
		}
		public bool DodgeCity
		{
			get { return data.DodgeCity; }
		}
		public bool HighNoon
		{
			get { return data.HighNoon; }
		}
		public bool FistfulOfCards
		{
			get { return data.FistfulOfCards; }
		}
		public bool WildWestShow
		{
			get { return data.WildWestShow; }
		}
		public int GamesPlayed
		{
			get { return gamesPlayed; }
		}
		public ReadOnlyCollection<SessionPlayer> Players
		{
			get { return new ReadOnlyCollection<SessionPlayer>(playerList); }
		}
		public ReadOnlyCollection<SessionSpectator> Spectators
		{
			get { return new ReadOnlyCollection<SessionSpectator>(spectatorList); }
		}
		ReadOnlyCollection<IPlayer> ISession.Players
		{
			get { return new ReadOnlyCollection<IPlayer>(playerList.ConvertAll<IPlayer>(p => p)); }
		}
		ReadOnlyCollection<ISpectator> ISession.Spectators
		{
			get { return new ReadOnlyCollection<ISpectator>(spectatorList.ConvertAll<ISpectator>(s => s)); }
		}
		public SessionEventManager EventManager
		{
			get { return eventMgr; }
		}
		public Game Game
		{
			get { return game; }
		}

		public Session(Server server, int id, CreateSessionData sessionData)
		{
			admin = new SessionAdmin(this);
			this.server = server;
			this.id = id;
			data = sessionData;
			if(data.MinPlayers > data.MaxPlayers)
				throw new MinPlayersOutOfRangeException();
			if(data.MinPlayers < 2)
				throw new MinPlayersOutOfRangeException();
			if(data.MinPlayers > 8)
				throw new MinPlayersOutOfRangeException();
			if(data.MaxPlayers < 2)
				throw new MaxPlayersOutOfRangeException();
			if(data.MaxPlayers > 8)
				throw new MaxPlayersOutOfRangeException();
			if(data.MaxSpectators < 0)
				throw new MaxSpectatorsOutOfRangeException();
			
			state = SessionState.WaitingForPlayers;
			eventMgr = new SessionEventManager(this);
			players = new Dictionary<int, SessionPlayer>(data.MaxPlayers);
			playerList = new List<SessionPlayer>(data.MaxPlayers);
			spectators = new Dictionary<int, SessionSpectator>(data.MaxSpectators);
			spectatorList = new List<SessionSpectator>(data.MaxSpectators);
			creatorId = 0;
			gamesPlayed = 0;
			remainingCharacters = Utils.GetCharacterTypes(this);
		}
		public Session(Server server, BinaryReader reader)
		{
			admin = new SessionAdmin(this);
			this.server = server;
			id = reader.ReadInt32();
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				data = (CreateSessionData)bf.Deserialize(reader.BaseStream);
			}
			catch(InvalidCastException)
			{
				throw new FormatException();
			}
			catch(SerializationException)
			{
				throw new FormatException();
			}
			try
			{
				state = (SessionState)reader.ReadInt32();
			}
			catch(InvalidCastException)
			{
				throw new FormatException();
			}
			if(state == SessionState.Playing)
				state = SessionState.GameFinished;
			creatorId = reader.ReadInt32();
			gamesPlayed = reader.ReadInt32();

			int playerCount = reader.ReadInt32();
			if(playerCount < 0)
				throw new FormatException();
			playerList = new List<SessionPlayer>(playerCount);
			players = new Dictionary<int, SessionPlayer>(playerCount);
			for(int i = 0; i < playerCount; i++)
			{
				bool isAI = reader.ReadBoolean();
				SessionPlayer player = new SessionPlayer(this, reader);
				if(isAI && player.ID != creatorId)
					player.RegisterListener(new AI.AIPlayer());
				playerList.Add(player);
				players.Add(player.ID, player);
			}
			spectatorList = new List<SessionSpectator>();
			spectators = new Dictionary<int, SessionSpectator>();

			eventMgr = new SessionEventManager(this);
			int currentSheriffId = reader.ReadInt32();
			if(currentSheriffId != 0)
			{
				sheriffEnumerator = playerList.GetEnumerator();
				do
				{
					if(!sheriffEnumerator.MoveNext())
						throw new FormatException();
				}
				while(sheriffEnumerator.Current.ID != currentSheriffId);
			}

			int remCharCount = reader.ReadInt32();
			if(playerCount < 0)
				throw new FormatException();
			remainingCharacters = new List<CharacterType>(remCharCount);
			for(int i = 0; i < remCharCount; i++)
				remainingCharacters.Add((CharacterType)reader.ReadInt32());
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
			writer.Write((int)state);
			writer.Write(creatorId);
			writer.Write(gamesPlayed);

			writer.Write(playerList.Count);
			foreach(SessionPlayer p in playerList)
			{
				writer.Write(p.IsAI);
				p.Write(writer);
			}

			if(sheriffEnumerator != null)
				writer.Write(sheriffEnumerator.Current.ID);
			else
				writer.Write(0);

			writer.Write(remainingCharacters.Count);
			foreach(CharacterType character in remainingCharacters)
				writer.Write((int)character);
		}

		public override void Disconnect()
		{
			base.Disconnect();
			admin.Disconnect();

			foreach(SessionPlayer p in playerList)
				p.Disconnect();
			foreach(SessionSpectator s in spectatorList)
				s.Disconnect();
		}

		public void Join(Password password, CreatePlayerData data, IPlayerEventListener listener)
		{
			if(state == SessionState.Ended)
				throw new BadSessionStateException();
			lock(Lock)
			{
				if(Locked)
					throw new MethodAccessException();

				if(state != SessionState.WaitingForPlayers)
					throw new BadSessionStateException();
				if(players.Count >= this.data.MaxPlayers)
					throw new TooManyPlayersException();
				if(!this.data.PlayerPassword.CheckPassword(password))
					throw new BadSessionPasswordException();

				int id = players.GenerateID();
				if(creatorId == 0)
					creatorId = id;
				SessionPlayer player = new SessionPlayer(id, this, data);
				players.Add(id, player);
				playerList.Add(player);
				player.RegisterListener(listener);
				eventMgr.SendController(player);
				eventMgr.OnPlayerJoinedSession(player);
				server.SaveState();
			}
		}

		public void Replace(int id, Password password, CreatePlayerData data, IPlayerEventListener listener)
		{
			if(state == SessionState.Ended)
				throw new BadSessionStateException();
			lock(Lock)
			{
				if(Locked)
					throw new MethodAccessException();

				if(!this.data.PlayerPassword.CheckPassword(password))
					throw new BadSessionPasswordException();

				SessionPlayer player = GetPlayer(id);
				if(player.HasListener && !player.IsAI)
					throw new InvalidOperationException();
				player.Update(data);
				player.RegisterListener(listener);
				eventMgr.SendController(player);
				if(state == SessionState.Playing)
					game.RegisterPlayer(player);
				eventMgr.OnPlayerUpdated(player);
				server.SaveState();
			}
		}

		public void Spectate(Password password, CreateSpectatorData data, ISpectatorEventListener listener)
		{
			if(state == SessionState.Ended)
				throw new BadSessionStateException();
			lock(Lock)
			{
				if(Locked)
					throw new MethodAccessException();

				if(!this.data.SpectatorPassword.CheckPassword(password))
					throw new BadSessionPasswordException();
				if(spectators.Count >= this.data.MaxSpectators)
					throw new TooManySpectatorsException();

				int id = spectators.GenerateID();
				SessionSpectator spectator = new SessionSpectator(id, this, data);
				spectators.Add(id, spectator);
				spectatorList.Add(spectator);
				spectator.RegisterListener(listener);
				eventMgr.SendController(spectator);
				if(state == SessionState.Playing)
					game.RegisterSpectator(spectator);
				eventMgr.OnSpectatorJoinedSession(spectator);
				server.SaveState();
			}
		}
		
		public SessionPlayer GetPlayer(int id)
		{
			try
			{
				return players[id];
			}
			catch(KeyNotFoundException)
			{
				throw new InvalidIdException();
			}
		}
		public SessionSpectator GetSpectator(int id)
		{
			try
			{
				return spectators[id];
			}
			catch(KeyNotFoundException)
			{
				throw new InvalidIdException();
			}
		}

		IPlayer ISession.GetPlayer(int id)
		{
			try
			{
				return players[id];
			}
			catch(KeyNotFoundException)
			{
				throw new InvalidIdException();
			}
		}
		ISpectator ISession.GetSpectator(int id)
		{
			try
			{
				return spectators[id];
			}
			catch(KeyNotFoundException)
			{
				throw new InvalidIdException();
			}
		}

		public IEnumerable<CharacterType> GetCharacters(int count)
		{
			CharacterType[] characters = new CharacterType[count];
			CharacterType character;
			bool checkConflict = false;
			for(int i = 0; i < count; i++)
			{
				do
					character = remainingCharacters.GetRandom();
				while(checkConflict && characters.Contains(character));
				remainingCharacters.Remove(character);
				if(remainingCharacters.Count == 0)
				{
					remainingCharacters = Utils.GetCharacterTypes(this);
					checkConflict = true;
				}
				characters[i] = character;
			}
			return characters;
		}
		public void OnGameEnded()
		{
			state = SessionState.GameFinished;
			gamesPlayed++;
			if(!sheriffEnumerator.MoveNext())
			{
				sheriffEnumerator.Reset();
				sheriffEnumerator.MoveNext();
			}
			eventMgr.OnGameEnded();
			server.SaveState();
		}
		
		public void NextGame()
		{
			if(state == SessionState.Playing)
				throw new BadSessionStateException();

			lock(Lock)
			{
				if(Locked)
					throw new MethodAccessException();

				if(state == SessionState.WaitingForPlayers)
				{
					int count = data.MinPlayers - players.Count;
					for(int i = 0; i < count; i++)
					{
						AI.AIPlayer ai = new AI.AIPlayer();
						Join(data.PlayerPassword, ai.CreateData, ai);
					}
					if(data.ShufflePlayers)
						playerList.Shuffle();
					sheriffEnumerator = playerList.GetEnumerator();
					sheriffEnumerator.MoveNext();
				}
				if(game != null)
					game.Disconnect();
				game = new Game(this, sheriffEnumerator.Current.ID);
				game.Start();
				state = SessionState.Playing;
				server.SaveState();
			}
		}
		
		public void End()
		{
			lock(Lock)
			{
				if(Locked)
					throw new MethodAccessException();

				state = SessionState.Ended;
				eventMgr.OnSessionEnded();
				if(game != null)
					game.Disconnect();
				server.RemoveSession(this);
			}
		}

		
		public void RemovePlayer(SessionPlayer player)
		{
			lock(Lock)
			{
				if(Locked)
					throw new MethodAccessException();

				player.UnregisterListener();
				if(state != SessionState.WaitingForPlayers || player.IsCreator)
					return;
				player.Control.Disconnect();
				players.Remove(player.ID);
				playerList.Remove(player);
				eventMgr.OnPlayerLeftSession(player);
				server.SaveState();
			}
		}
		public void RemoveSpectator(SessionSpectator spectator)
		{
			lock(Lock)
			{
				if(Locked)
					throw new MethodAccessException();

				spectator.UnregisterListener();
				spectator.Control.Disconnect();
				spectators.Remove(spectator.ID);
				spectatorList.Remove(spectator);
				eventMgr.OnSpectatorLeftSession(spectator);
				server.SaveState();
			}
		}
	}
}
