using System;
namespace Bang.Server
{
	public sealed class SessionPlayerControl : ImmortalMarshalByRefObject, IPlayerSessionControl
	{
		private SessionPlayer player;
		
		public SessionPlayer Player
		{
			get { return player; }
		}
		public Session Session
		{
			get { return player.Session; }
		}
		ISession IPlayerSessionControl.Session
		{
			get { return player.Session; }
		}
		IPlayer IPlayerSessionControl.Player
		{
			get { return player; }
		}

		public SessionPlayerControl (SessionPlayer player)
		{
			this.player = player;
		}
		
		void IPlayerSessionControl.SendChatMessage(string message)
		{
			if(!player.HasListener)
				throw new InvalidOperationException();

			Session session = Session;
			lock(session)
			{
				if(session.Locked)
					throw new InvalidOperationException();
				session.Locked = true;

				try
				{
					session.EventManager.SendChatMessage(player, message);
				}
				catch
				{
					session.Locked = false;
					throw;
				}
				session.Locked = false;
			}
		}
		
		void IPlayerSessionControl.Disconnect()
		{
			if(!player.HasListener)
				throw new InvalidOperationException();

			Session session = Session;
			lock(session)
			{
				if(session.Locked)
					throw new InvalidOperationException();
				session.Locked = true;

				try
				{
					session.RemovePlayer(player);
				}
				catch
				{
					session.Locked = false;
					throw;
				}
				session.Locked = false;
			}
		}

		void IPlayerSessionControl.StartGame()
		{
			if(!player.HasListener)
				throw new InvalidOperationException();
			if(!player.IsCreator)
				throw new MustBeCreatorException();

			Session session = Session;
			lock(session)
			{
				if(session.Locked)
					throw new InvalidOperationException();
				session.Locked = true;

				try
				{
					session.NextGame();
				}
				catch
				{
					session.Locked = false;
					throw;
				}
				session.Locked = false;
			}
		}

		void IPlayerSessionControl.EndSession()
		{
			if(!player.HasListener)
				throw new InvalidOperationException();
			if(!player.IsCreator)
				throw new MustBeCreatorException();

			Session session = Session;
			lock(session)
			{
				if(session.Locked)
					throw new InvalidOperationException();
				session.Locked = true;

				try
				{
					session.End();
				}
				catch
				{
					session.Locked = false;
					throw;
				}
				session.Locked = false;
			}
		}
	}
}

