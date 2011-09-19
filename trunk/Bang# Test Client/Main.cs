// Main.cs
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
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using Bang.ConsoleUtils;
namespace Bang.Client
{
	public sealed class TestClient : ImmortalMarshalByRefObject, IPlayerEventListener
	{
		private IPlayerControl gameControl;
		private IPlayerSessionControl sessionControl;
		private IPlayerEventListener aiPlayer;

		private static readonly TestClient Instance = new TestClient();

		public static void Main(string[] cmdArgs)
		{
			ConsoleHelper.PrintLine("Bang# Command-Line Client");
			ConsoleHelper.PrintLine("------------");
			string address;
			string portString;
			if(cmdArgs.Length != 2)
			{
				ConsoleHelper.Print("Server Address: ");
				address = ConsoleHelper.ReadLine();
				ConsoleHelper.Print("Server Port: ");
				portString = ConsoleHelper.ReadLine();
			}
			else
			{
				address = cmdArgs[0];
				portString = cmdArgs[1];
			}
			int port;
			try
			{
				port = int.Parse(portString);
			}
			catch(FormatException)
			{
				ConsoleHelper.ErrorLine("Bad number format!");
				return;
			}

			ConsoleHelper.PrintLine("Connecting to {0} on port {1}...", address, port);
			IServer _server;
			_server = Utils.Connect(address, port);

			ConsoleHelper.PrintLine();

			try
			{
				if(!Utils.IsServerCompatible(_server))
				{
					ConsoleHelper.ErrorLine("Server version {0}.{1} not compatible with client version {2}.{3}!",
						_server.InterfaceVersionMajor, _server.InterfaceVersionMinor,
						Utils.InterfaceVersionMajor, Utils.InterfaceVersionMinor);
					return;
				}
				ConsoleHelper.PrintLine("Server name: {0}", _server.Name);
				ConsoleHelper.PrintLine("Server description: {0}", _server.Description);
				ConsoleHelper.SuccessLine("Connection estabilished!");

				ConsoleHelper.PrintLine();

				NestedCommand rootCmd = new NestedCommand();
				NestedCommand<IServer> serverCmd = new NestedCommand<IServer>(cmd => _server);
				serverCmd.MakeServerCommand();
				NestedCommand<IServer, ISession> sessionCmd = (NestedCommand<IServer, ISession>)serverCmd["session"];
				sessionCmd["join"] = new FinalCommand<ISession>((session, cmd) =>
				{
					CreatePlayerData cpd;
					string playerName;
					Password playerPassword;
					Password password;
					if(Instance.sessionControl != null)
					{
						ConsoleHelper.ErrorLine("Already connected to a session!");
						return;
					}
					
					ConsoleHelper.Print("Session Password: ");
					password = ConsoleHelper.ReadPassword();

					ConsoleHelper.Print("Player Name: ");
					playerName = ConsoleHelper.ReadLine();
					ConsoleHelper.Print("Player Password: ");
					playerPassword = ConsoleHelper.ReadPassword();
					cpd = new CreatePlayerData { Name = playerName, Password = playerPassword };
					
					try
					{
						session.Join(password, cpd, Instance);
						ConsoleHelper.SuccessLine("Joined session!");
					}
					catch(GameException e)
					{
						ConsoleHelper.ErrorLine("Cannot join session: {0}", e.GetType());
					}
				});
				sessionCmd["joinai"] = new FinalCommand<ISession>((session, cmd) =>
				{
					CreatePlayerData cpd;
					Password password;
					if(Instance.sessionControl != null)
					{
						ConsoleHelper.ErrorLine("Already connected to a session!");
						return;
					}

					ConsoleHelper.Print("Session Password: ");
					password = ConsoleHelper.ReadPassword();

					AI.AIPlayer ai = new AI.AIPlayer();
					cpd = ai.CreateData;
					cpd.Name = "TestAI";

					try
					{
						session.Join(password, cpd, Instance);
						Instance.aiPlayer = ai;
						ConsoleHelper.SuccessLine("Joined session!");
					}
					catch(GameException e)
					{
						ConsoleHelper.ErrorLine("Cannot join session: {0}", e.GetType());
					}
				});
				sessionCmd["replace"] = new FinalCommand<ISession>((session, cmd) =>
				{
					CreatePlayerData cpd;
					string playerName;
					Password playerPassword;
					Password password;
					int id;
					try
					{
						id = int.Parse(cmd.Dequeue());
					}
					catch(FormatException)
					{
						ConsoleHelper.ErrorLine("Bad number format!");
						return;
					}

					if(Instance.sessionControl != null)
					{
						ConsoleHelper.ErrorLine("Already connected to a session!");
						return;
					}

					ConsoleHelper.Print("Session Password: ");
					password = new Password(ConsoleHelper.ReadLine());

					ConsoleHelper.Print("Player Name: ");
					playerName = ConsoleHelper.ReadLine();
					ConsoleHelper.Print("Player Password: ");
					playerPassword = ConsoleHelper.ReadPassword();
					cpd = new CreatePlayerData { Name = playerName, Password = playerPassword };

					try
					{
						session.Replace(id, password, cpd, Instance);
						ConsoleHelper.SuccessLine("Joined session!");
					}
					catch(GameException e)
					{
						ConsoleHelper.ErrorLine("Cannot join session: {0}", e.GetType());
					}
				});
				sessionCmd["replaceai"] = new FinalCommand<ISession>((session, cmd) =>
				{
					CreatePlayerData cpd;
					Password playerPassword;
					Password password;
					int id;
					try
					{
						id = int.Parse(cmd.Dequeue());
					}
					catch(FormatException)
					{
						ConsoleHelper.ErrorLine("Bad number format!");
						return;
					}

					if(Instance.sessionControl != null)
					{
						ConsoleHelper.ErrorLine("Already connected to a session!");
						return;
					}

					ConsoleHelper.Print("Session Password: ");
					password = new Password(ConsoleHelper.ReadLine());

					ConsoleHelper.Print("Player Password: ");
					playerPassword = ConsoleHelper.ReadPassword();
					AI.AIPlayer ai = new AI.AIPlayer();
					cpd = ai.CreateData;
					cpd.Name = "TestAI";
					cpd.Password = playerPassword;

					try
					{
						session.Replace(id, password, cpd, Instance);
						Instance.aiPlayer = ai;
						ConsoleHelper.SuccessLine("Joined session!");
					}
					catch(GameException e)
					{
						ConsoleHelper.ErrorLine("Cannot join session: {0}", e.GetType());
					}
				});
				serverCmd["session"] = sessionCmd;
				serverCmd["test"] = new FinalCommand<IServer>((server, cmd) =>
				{
					if(Instance.sessionControl != null)
					{
						ConsoleHelper.ErrorLine("Already connected to a session!");
						return;
					}
					int playerCount = 4;
					if(cmd.Count != 0)
					{
						try
						{
							playerCount = int.Parse(cmd.Dequeue());
						}
						catch(FormatException)
						{
							ConsoleHelper.ErrorLine("Bad number format!");
							return;
						}
					}

					CreateSessionData csd = new CreateSessionData { Name = "Test", Description = "", MinPlayers = playerCount, MaxPlayers = playerCount, MaxSpectators = 0, DodgeCity = true };
					CreatePlayerData cpd = new CreatePlayerData { Name = "Human" };
					try
					{
						server.CreateSession(csd, cpd, Instance);
						ConsoleHelper.SuccessLine("Test session created!");
					}
					catch(GameException e)
					{
						ConsoleHelper.ErrorLine("Cannot create session: {0}", e.GetType());
					}
				});
				serverCmd["testai"] = new FinalCommand<IServer>((server, cmd) =>
				{
					if(Instance.sessionControl != null)
					{
						ConsoleHelper.ErrorLine("Already connected to a session!");
						return;
					}
					int playerCount = 4;
					if(cmd.Count != 0)
					{
						try
						{
							playerCount = int.Parse(cmd.Dequeue());
						}
						catch(FormatException)
						{
							ConsoleHelper.ErrorLine("Bad number format!");
							return;
						}
					}
					
					CreateSessionData csd = new CreateSessionData { Name = "Test", Description = "", MaxPlayers = playerCount, MinPlayers = playerCount, MaxSpectators = 0, DodgeCity = true };
					AI.AIPlayer ai = new AI.AIPlayer();
					CreatePlayerData cpd = ai.CreateData;
					cpd.Name = "TestAI";
					try
					{
						server.CreateSession(csd, cpd, Instance);
						Instance.aiPlayer = ai;
						ConsoleHelper.SuccessLine("Test AI session created!");
					}
					catch(GameException e)
					{
						ConsoleHelper.ErrorLine("Cannot create session: {0}", e.GetType());
					}
				});
				serverCmd["createsession"] = new FinalCommand<IServer>((server, cmd) =>
				{
					if(Instance.sessionControl != null)
					{
						ConsoleHelper.ErrorLine("Already connected to a session!");
						return;
					}
					
					ConsoleHelper.Print("Session Name: ");
					string sessionName = ConsoleHelper.ReadLine();
					ConsoleHelper.Print("Session Description: ");
					string sessionDescription = ConsoleHelper.ReadLine();
					int sessionMinPlayers;
					int sessionMaxPlayers;
					int sessionMaxSpectators;
					try
					{
						ConsoleHelper.Print("Session MinPlayers: ");
						sessionMinPlayers = int.Parse(ConsoleHelper.ReadLine());
						ConsoleHelper.Print("Session MaxPlayers: ");
						sessionMaxPlayers = int.Parse(ConsoleHelper.ReadLine());
						ConsoleHelper.Print("Session MaxSpectators: ");
						sessionMaxSpectators = int.Parse(ConsoleHelper.ReadLine());
					}
					catch(FormatException)
					{
						ConsoleHelper.ErrorLine("Bad number format!");
						return;
					}
					
					ConsoleHelper.Print("Session PlayerPassword: ");
					Password sessionPlayerPassword = ConsoleHelper.ReadPassword();
					ConsoleHelper.Print("Session SpectatorPassword: ");
					Password sessionSpectatorPassword = ConsoleHelper.ReadPassword();
					ConsoleHelper.Print("Session ShufflePlayers: ");
					bool sessionShufflePlayers = ConsoleHelper.ReadLine().ToLower() == "y";
					ConsoleHelper.Print("Session DodgeCity: ");
					bool sessionDodgeCity = ConsoleHelper.ReadLine().ToLower() == "y";
					ConsoleHelper.Print("Session HighNoon: ");
					bool sessionHighNoon = ConsoleHelper.ReadLine().ToLower() == "y";
					ConsoleHelper.Print("Session FistfulOfCards: ");
					bool sessionFistfulOfCards = ConsoleHelper.ReadLine().ToLower() == "y";
					ConsoleHelper.Print("Session WildWestShow: ");
					bool sessionWildWestShow = ConsoleHelper.ReadLine().ToLower() == "y";
					CreateSessionData csd = new CreateSessionData { Name = sessionName, Description = sessionDescription, MinPlayers = sessionMinPlayers, MaxPlayers = sessionMaxPlayers, MaxSpectators = sessionMaxSpectators, PlayerPassword = sessionPlayerPassword, SpectatorPassword = sessionSpectatorPassword, ShufflePlayers = sessionShufflePlayers, DodgeCity = sessionDodgeCity, HighNoon = sessionHighNoon,
					FistfulOfCards = sessionFistfulOfCards, WildWestShow = sessionWildWestShow };
					
					ConsoleHelper.Print("Player Name: ");
					string playerName = ConsoleHelper.ReadLine();
					ConsoleHelper.Print("Player Password: ");
					Password playerPassword = ConsoleHelper.ReadPassword();
					CreatePlayerData cpd = new CreatePlayerData { Name = playerName, Password = playerPassword };
					
					try
					{
						server.CreateSession(csd, cpd, Instance);
						ConsoleHelper.SuccessLine("Session created!");
					}
					catch(GameException e)
					{
						ConsoleHelper.ErrorLine("Cannot create session: {0}", e.GetType());
					}
				});
				rootCmd["server"] = serverCmd;
				NestedCommand<IPlayerSessionControl> sessionControlCommand = new NestedCommand<IPlayerSessionControl>(cmd => Instance.sessionControl);
				sessionControlCommand.MakePlayerSessionControlCommand(() =>
				{
					Instance.sessionControl = null;
					Instance.gameControl = null;
				});
				rootCmd["sessioncontrol"] = sessionControlCommand;
				NestedCommand<IPlayerControl> gameControlCommand = new NestedCommand<IPlayerControl>(cmd => Instance.gameControl);
				gameControlCommand.MakePlayerGameControlCommand();
				rootCmd["gamecontrol"] = gameControlCommand;
				rootCmd["exit"] = new FinalCommand(cmd => Environment.Exit(0));
				while (true) // command-line loop
				{
					try
					{
						rootCmd.ReadAndExecute();
					}
					catch(InvalidOperationException)
					{
						ConsoleHelper.ErrorLine("Too few arguments!");
					}
				}
			}
			catch(RemotingException e)
			{
				ConsoleHelper.ErrorLine("Remoting error!");
#if DEBUG
				ConsoleHelper.DebugLine(e.ToString());
#endif
				return;
			}
			catch(SerializationException e)
			{
				ConsoleHelper.ErrorLine("Serialization error!");
#if DEBUG
				ConsoleHelper.DebugLine(e.ToString());
#endif
				return;
			}
		}

		#region IPlayerEventListener implementation
		void IPlayerEventListener.OnJoinedSession(IPlayerSessionControl control)
		{
			sessionControl = control;
			ConsoleHelper.SessionEvent("Acquired session controller!");
			if(aiPlayer != null)
				aiPlayer.OnJoinedSession(control);
		}
		void IPlayerEventListener.OnJoinedGame (IPlayerControl control)
		{
			this.gameControl = control;
			ConsoleHelper.GameEvent("Acquired game controller!");
			if(aiPlayer != null)
				aiPlayer.OnJoinedGame(control);
		}

		bool IPlayerEventListener.IsAI
		{
			get { return false; }
		}
		#endregion

		#region IEventListener implementation
		void IEventListener.Ping()
		{
		}
		void IEventListener.OnSessionEnded()
		{
			ConsoleHelper.SessionEvent("Session ended!");
			gameControl = null;
			sessionControl = null;
			if(aiPlayer != null)
			{
				aiPlayer.OnSessionEnded();
				aiPlayer = null;
			}
		}
		void IEventListener.OnGameEnded()
		{
			ConsoleHelper.GameEvent("Game ended! You {0}", gameControl.PrivatePlayerView.IsWinner ? "won!" : "lost.");

			if(aiPlayer != null)
				aiPlayer.OnGameEnded();
		}

		void IEventListener.OnPlayerJoinedSession(IPlayer player)
		{
			ConsoleHelper.SessionEvent("Player #{0} '{1}' joined the session.", player.ID, player.Name);
			if(aiPlayer != null)
				aiPlayer.OnPlayerJoinedSession(player);
		}
		void IEventListener.OnSpectatorJoinedSession(ISpectator spectator)
		{
			ConsoleHelper.SessionEvent("Spectator #{0} '{1}' joined the session.", spectator.ID, spectator.Name);
			if(aiPlayer != null)
				aiPlayer.OnSpectatorJoinedSession(spectator);
		}
		void IEventListener.OnPlayerLeftSession(IPlayer player)
		{
			ConsoleHelper.SessionEvent("Player #{0} '{1}' left the session.", player.ID, player.Name);
			if(aiPlayer != null)
				aiPlayer.OnPlayerLeftSession(player);
		}
		void IEventListener.OnSpectatorLeftSession(ISpectator spectator)
		{
			ConsoleHelper.SessionEvent("Spectator #{0} '{1}' left the session.", spectator.ID, spectator.Name);
			if(aiPlayer != null)
				aiPlayer.OnSpectatorLeftSession(spectator);
		}
		void IEventListener.OnPlayerUpdated(IPlayer player)
		{
			ConsoleHelper.SessionEvent("Player #{0} '{1}' has been updated.", player.ID, player.Name);
			if(aiPlayer != null)
				aiPlayer.OnPlayerUpdated(player);
		}

		void IEventListener.OnChatMessage(IPlayer player, string message)
		{
			ConsoleHelper.SessionEvent("Chat: {0}: {1}", player.Name, message);
			if(aiPlayer != null)
				aiPlayer.OnChatMessage(player, message);
		}
		void IEventListener.OnChatMessage (ISpectator spectator, string message)
		{
			ConsoleHelper.SessionEvent("Chat: {0}: {1}", spectator.Name, message);
			if(aiPlayer != null)
				aiPlayer.OnChatMessage(spectator, message);
		}

		void IEventListener.OnNewRequest(RequestType requestType, IPublicPlayerView requestedPlayer, IPublicPlayerView causedBy)
		{
			ConsoleHelper.GameEvent("New request: {0} (player #{1}).", requestType, requestedPlayer == null ? 0 : requestedPlayer.ID);
			if(aiPlayer != null)
				aiPlayer.OnNewRequest(requestType, requestedPlayer, causedBy);
		}
		void IEventListener.OnPlayerDrewFromDeck(IPublicPlayerView player, ReadOnlyCollection<ICard> drawnCards)
		{
			ConsoleHelper.GameEvent("Player #{0} has drawn {1} cards from the deck.", player.ID, drawnCards.Count);
			if(aiPlayer != null)
				aiPlayer.OnPlayerDrewFromDeck(player, drawnCards);
		}
		void IEventListener.OnPlayerDrewFromGraveyard(IPublicPlayerView player, ReadOnlyCollection<ICard> drawnCards)
		{
			ConsoleHelper.GameEvent("Player #{0} has drawn {1} cards from the graveyard.", player.ID, drawnCards.Count);
			if(aiPlayer != null)
				aiPlayer.OnPlayerDrewFromGraveyard(player, drawnCards);
		}
		void IEventListener.OnPlayerDiscardedCard(IPublicPlayerView player, ICard card)
		{
			ConsoleHelper.GameEvent("Player #{0} discarded card {1} #{2}.", player.ID, card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerDiscardedCard(player, card);
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card)
		{
			ConsoleHelper.GameEvent("Player #{0} played card {1} #{2}.", player.ID, card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCard(player, card);
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
			ConsoleHelper.GameEvent("Player #{0} played card {1} #{2} on player #{3}.", player.ID, card.Type, card.ID, targetPlayer.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCard(player, card, targetPlayer);
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			ConsoleHelper.GameEvent("Player #{0} played card {1} #{2} on player #{3}'s card {4} #{5}.", player.ID, card.Type, card.ID, targetPlayer.ID, targetCard.Type, targetCard.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCard(player, card, targetPlayer, targetCard);
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard)
		{
			ConsoleHelper.GameEvent("Player #{0} played card {1} #{2} as card {3}.", player.ID, card.Type, card.ID, asCard);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCard(player, card, asCard);
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer)
		{
			ConsoleHelper.GameEvent("Player #{0} played card {1} #{2} as card {3} on player #{4}.", player.ID, card.Type, card.ID, asCard, targetPlayer.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCard(player, card, asCard, targetPlayer);
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			ConsoleHelper.GameEvent("Player #{0} played card {1} #{2} as card {3} on player #{4}'s card {5} #{6}.", player.ID, card.Type, card.ID, asCard, targetPlayer.ID, targetCard.Type, targetCard.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard);
		}
		void IEventListener.OnPlayerPlayedCardOnTable(IPublicPlayerView player, ICard card)
		{
			ConsoleHelper.GameEvent("Player #{0} played card {1} #{2} on table.", player.ID, card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCardOnTable(player, card);
		}
		void IEventListener.OnPassedTableCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
			ConsoleHelper.GameEvent("Player #{0} passed card {1} #{2} to player #{3}.", player.ID, card.Type, card.ID, targetPlayer.ID);
			if(aiPlayer != null)
				aiPlayer.OnPassedTableCard(player, card, targetPlayer);
		}
		void IEventListener.OnPlayerPassed(IPublicPlayerView player)
		{
			ConsoleHelper.GameEvent("Player #{0} passed.", player.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPassed(player);
		}
		void IEventListener.OnPlayerRespondedWithCard (IPublicPlayerView player, ICard card)
		{
			ConsoleHelper.GameEvent("Player #{0} responded with card {1} #{2}.", player.ID, card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerRespondedWithCard(player, card);
		}
		void IEventListener.OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card, CardType asCard)
		{
			ConsoleHelper.GameEvent("Player #{0} responded with card {1} #{2} as card {3}.", player.ID, card.Type, card.ID, asCard);
			if(aiPlayer != null)
				aiPlayer.OnPlayerRespondedWithCard(player, card, asCard);
		}
		void IEventListener.OnDrawnIntoSelection(ReadOnlyCollection<ICard> drawnCards)
		{
			ConsoleHelper.GameEvent("{0} cards were drawn into the selection.", drawnCards.Count);
			if(aiPlayer != null)
				aiPlayer.OnDrawnIntoSelection(drawnCards);
		}
		void IEventListener.OnPlayerPickedFromSelection (IPublicPlayerView player, ICard card)
		{
			ConsoleHelper.GameEvent("Player #{0} picked card {1} #{2} from selection.", player.ID, card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPickedFromSelection(player, card);
		}
		void IEventListener.OnUndrawnFromSelection(ICard card)
		{
			ConsoleHelper.GameEvent("Card {0} #{1} was undrawn from the selection.", card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnUndrawnFromSelection(card);
		}
		void IEventListener.OnPlayerStoleCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			ConsoleHelper.GameEvent("Player #{0} stole card {1} #{2} from player #{3}.", player.ID, targetCard.Type, targetCard.ID, targetPlayer.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerStoleCard(player, targetPlayer, targetCard);
		}
		void IEventListener.OnPlayerCancelledCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			ConsoleHelper.GameEvent("Player #{0} cancelled card {1} #{2} of player #{3}.", player.ID, targetCard.Type, targetCard.ID, targetPlayer.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerCancelledCard(player, targetPlayer, targetCard);
		}
		void IEventListener.OnDeckChecked(ICard card)
		{
			ConsoleHelper.GameEvent("Card {0} #{1} was checked from the deck.", card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnDeckChecked(card);
		}
		void IEventListener.OnCardCancelled(ICard card)
		{
			ConsoleHelper.GameEvent("Card {0} #{1} was cancelled.", card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnCardCancelled(card);
		}
		void IEventListener.OnPlayerCheckedDeck(IPublicPlayerView player, ICard checkedCard, CardType causedBy, bool result)
		{
			ConsoleHelper.GameEvent("Player #{0} checked card {1} #{2} from deck because of card {3} and {4}.", player.ID, checkedCard.Type, checkedCard.ID, causedBy, result ? "succeeded" : "failed");
			if(aiPlayer != null)
				aiPlayer.OnPlayerCheckedDeck(player, checkedCard, causedBy, result);
		}
		void IEventListener.OnLifePointsChanged(IPublicPlayerView player, int delta, IPublicPlayerView causedBy)
		{
			ConsoleHelper.GameEvent("Player #{0} {1} {2} lives{3}.", player.ID, delta > 0 ? "gained" : "lost", Math.Abs(delta), causedBy == null ? "" : " because of player #" + causedBy.ID);
			if(aiPlayer != null)
				aiPlayer.OnLifePointsChanged(player, delta, causedBy);
		}
		void IEventListener.OnPlayerDied(IPublicPlayerView player, IPublicPlayerView causedBy)
		{
			ConsoleHelper.GameEvent("Player #{0} died{1}, he was {2}.", player.ID, causedBy == null ? "" : " because of player #" + causedBy.ID, player.Role);
			if(aiPlayer != null)
				aiPlayer.OnPlayerDied(player, causedBy);
		}
		void IEventListener.OnPlayerUsedAbility (IPublicPlayerView player)
		{
			ConsoleHelper.GameEvent("Player #{0} used ability.", player.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerUsedAbility(player);
		}
		void IEventListener.OnPlayerGainedAdditionalCharacters(IPublicPlayerView player)
		{
			ConsoleHelper.GameEvent("Player #{0} gained additional abilities.", player.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerGainedAdditionalCharacters(player);
		}
		void IEventListener.OnPlayerLostAdditionalCharacters(IPublicPlayerView player)
		{
			ConsoleHelper.GameEvent("Player #{0} lost his additional abilities.", player.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerLostAdditionalCharacters(player);
		}
		void IEventListener.OnDeckRegenerated ()
		{
			ConsoleHelper.GameEvent("The deck was regenerated.");
			if(aiPlayer != null)
				aiPlayer.OnDeckRegenerated();
		}
		#endregion
	}
}

