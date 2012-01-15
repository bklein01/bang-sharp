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
			lock(session.Lock)
			{
				if(session.Locked)
					throw new MethodAccessException();

				session.EventManager.SendChatMessage(player, message);
			}
		}
		
		void IPlayerSessionControl.Disconnect()
		{
			if(!player.HasListener)
				throw new InvalidOperationException();

			Session.RemovePlayer(player);
		}

		void IPlayerSessionControl.StartGame()
		{
			if(!player.HasListener)
				throw new InvalidOperationException();
			if(!player.IsCreator)
				throw new MustBeCreatorException();

			Session.NextGame();
		}

		void IPlayerSessionControl.EndSession()
		{
			if(!player.HasListener)
				throw new InvalidOperationException();
			if(!player.IsCreator)
				throw new MustBeCreatorException();

			Session.End();
		}
	}
}

