using System;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace Bang.Server
{
	public sealed class SessionPlayer : MarshalByRefObject, IPlayer
	{
		private int id;
		private Session session;
		private CreatePlayerData data;
		private SessionPlayerControl control;
		private IPlayerEventListener listener;

		private int score;
		private int turnsPlayed;
		private int victories;
		private int victoriesAsSheriff;
		private int victoriesAsDeputy;
		private int victoriesAsOutlaw;
		private int victoriesAsRenegade;
		
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
					listener.Ping();
					return true;
				}
				catch(RemotingException)
				{
					UnregisterListener();
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
		public int VictoriesAsSheriff
		{
			get { return victoriesAsSheriff; }
		}
		public int VictoriesAsDeputy
		{
			get { return victoriesAsDeputy; }
		}
		public int VictoriesAsOutlaw
		{
			get { return victoriesAsOutlaw; }
		}
		public int VictoriesAsRenegade
		{
			get { return victoriesAsRenegade; }
		}

		public Session Session
		{
			get { return session; }
		}
		public SessionPlayerControl Control
		{
			get { return control; }
		}
		public IPlayerEventListener Listener
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
			victoriesAsSheriff = 0;
			victoriesAsDeputy = 0;
			victoriesAsOutlaw = 0;
			victoriesAsRenegade = 0;
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
			victoriesAsSheriff = reader.ReadInt32();
			if(victoriesAsSheriff < 0)
				throw new FormatException();
			victoriesAsDeputy = reader.ReadInt32();
			if(victoriesAsDeputy < 0)
				throw new FormatException();
			victoriesAsOutlaw = reader.ReadInt32();
			if(victoriesAsOutlaw < 0)
				throw new FormatException();
			victoriesAsRenegade = reader.ReadInt32();
			if(victoriesAsRenegade < 0)
				throw new FormatException();
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
			writer.Write(victoriesAsSheriff);
			writer.Write(victoriesAsDeputy);
			writer.Write(victoriesAsOutlaw);
			writer.Write(victoriesAsRenegade);
		}

		public void Update (CreatePlayerData data)
		{
			if (!this.data.Password.CheckPassword (data.Password))
				throw new BadPlayerPasswordException();
			this.data = data;
		}
		public void RegisterListener (IPlayerEventListener listener)
		{
			this.listener = listener;
		}
		public void UnregisterListener()
		{
			listener = null;
		}

		public void UpdateScore(int score)
		{
			this.score += score;
		}
		public void UpdateTurnsPlayed(int turnsPlayed)
		{
			this.turnsPlayed += turnsPlayed;
		}
		public void RegisterVictory(Role role)
		{
			victories++;
			switch(role)
			{
			case Role.Sheriff:
				victoriesAsSheriff++;
				break;
			case Role.Deputy:
				victoriesAsDeputy++;
				break;
			case Role.Outlaw:
				victoriesAsOutlaw++;
				break;
			case Role.Renegade:
				victoriesAsRenegade++;
				break;
			}
		}
	}
}
