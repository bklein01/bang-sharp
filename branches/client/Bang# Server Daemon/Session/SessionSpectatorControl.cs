using System;
namespace Bang.Server
{
	public sealed class SessionSpectatorControl : ImmortalMarshalByRefObject, ISpectatorSessionControl
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
					throw new MethodAccessException();
				session.Locked = true;

				session.EventManager.SendChatMessage(spectator, message);

				session.Locked = false;
			}
		}
		
		void ISpectatorSessionControl.Disconnect()
		{
			if(!spectator.HasListener)
				throw new InvalidOperationException();
			Session.RemoveSpectator(spectator);
		}
	}
}

