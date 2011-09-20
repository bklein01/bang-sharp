using System;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace Bang.Server
{
	public sealed class SessionSpectator : ImmortalMarshalByRefObject, ISpectator
	{
		private int id;
		private Session session;
		private CreateSpectatorData data;
		private SessionSpectatorControl control;
		private ISpectatorEventListener listener;
		
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
		
		public Session Session
		{
			get { return session; }
		}
		public SessionSpectatorControl Control
		{
			get { return control; }
		}
		public ISpectatorEventListener Listener
		{
			get { return listener; }
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
					session.RemoveSpectator(this);
					return false;
				}
			}
		}
		
		public SessionSpectator(int id, Session session, CreateSpectatorData data)
		{
			this.id = id;
			this.session = session;
			this.data = data;
			control = new SessionSpectatorControl(this);
		}

		public override void Disconnect()
		{
			base.Disconnect();
			control.Disconnect();
		}

		public void RegisterListener (ISpectatorEventListener listener)
		{
			this.listener = listener;
		}
		public void UnregisterListener ()
		{
			listener = null;
		}
	}
}