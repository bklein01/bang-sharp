using System;
namespace Bang.Server
{
	public sealed class SessionSpectatorControl : MarshalByRefObject, ISpectatorSessionControl
	{
		private SessionSpectator spectator;

		public SessionSpectator Spectator
		{
			get { return spectator; }
		}
		public Session Session
		{
			get { return spectator.Session; }
		}
		ISession ISpectatorSessionControl.Session
		{
			get { return spectator.Session; }
		}
		ISpectator ISpectatorSessionControl.Spectator
		{
			get { return spectator; }
		}

		public SessionSpectatorControl(SessionSpectator spectator)
		{
			this.spectator = spectator;
		}

		void ISpectatorSessionControl.SendChatMessage(string message)
		{
			if(!spectator.HasListener)
				throw new InvalidOperationException();

			Session session = Session;
			lock(session)
			{
				if(session.Locked)
					throw new InvalidOperationException();
				session.Locked = true;

				try
				{
					session.EventManager.SendChatMessage(spectator, message);
				}
				catch
				{
					session.Locked = false;
					throw;
				}
				session.Locked = false;
			}
		}
		
		void ISpectatorSessionControl.Disconnect()
		{
			if(!spectator.HasListener)
				throw new InvalidOperationException();
			Session session = Session;
			lock(session)
			{
				if(session.Locked)
					throw new InvalidOperationException();
				session.Locked = true;

				try
				{
					session.RemoveSpectator(spectator);
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

