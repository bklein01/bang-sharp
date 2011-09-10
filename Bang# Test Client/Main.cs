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
			ConsoleUtils.Commands = new string[]
			{
				"server",
				"name",
				"description",
				"sessions",
				"count",
				"session",
				"join",
				"joinai",
				"replace",
				"replaceai",
				"test",
				"testai",
				"createsession",
				"sessioncontrol",
				"chat",
				"disconnect",
				"startgame",
				"endsession",
				"gamecontrol",
				"game",
				"players",
				"player",
				"graveyardtop",
				"reqtype",
				"current",
				"requested",
				"causedby",
				"hand",
				"role",
				"selection",
				"responddraw",
				"respondcard",
				"respondplayer",
				"respondnoaction",
				"responduseability",
				"exit",
				"issheriff",
				"isalive",
				"iswinner",
				"lifepoints",
				"maxlifepoints",
				"table",
				"character",
				"state",
				"minplayers",
				"maxplayers",
				"maxspectators",
				"hasplayerpassword",
				"hasspectatorpassword",
				"creator",
				"dodgecity",
				"highnoon",
				"fistfulofcards",
				"wildwestshow",
				"gamesplayed",
				"spectators",
				"spectator",
				"haspassword",
				"iscreator",
				"isai",
				"haslistener",
				"score",
				"turnsplayed",
				"wins",
				"winsassheriff",
				"winsasdeputy",
				"winsasoutlaw",
				"winsasrenegade",
			};
			ConsoleUtils.PrintLine("Bang# Command-Line Client");
			ConsoleUtils.PrintLine("------------");
			string address;
			string portString;
			if(cmdArgs.Length != 2)
			{
				ConsoleUtils.Print("Server Address: ");
				address = ConsoleUtils.ReadLine();
				ConsoleUtils.Print("Server Port: ");
				portString = ConsoleUtils.ReadLine();
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
				ConsoleUtils.ErrorLine("Bad number format!");
				return;
			}

			ConsoleUtils.PrintLine("Connecting to {0} on port {1}...", address, port);
			IServer server = Utils.Connect(address, port);

			ConsoleUtils.PrintLine();

			try
			{
				if(!Utils.IsServerCompatible(server))
				{
					ConsoleUtils.ErrorLine("Server version {0}.{1} not compatible with client version {2}.{3}!",
						server.InterfaceVersionMajor, server.InterfaceVersionMinor,
						Utils.InterfaceVersionMajor, Utils.InterfaceVersionMinor);
					return;
				}
				ConsoleUtils.PrintLine("Server name: {0}", server.Name);
				ConsoleUtils.PrintLine("Server description: {0}", server.Description);
				ConsoleUtils.SuccessLine("Connection estabilished!");

				ConsoleUtils.PrintLine();
	
				while (true) // command-line loop
				{
					Queue<string> args = ConsoleUtils.ReadCommand();
					string arg;
					try
					{
						switch (arg = args.Dequeue())
						{
						case "server":
							ServerCommand(args.Dequeue(), args, server);
							break;
						case "sessioncontrol":
							IPlayerSessionControl sessionControl = Instance.sessionControl;
							if(sessionControl == null)
							{
								ConsoleUtils.ErrorLine("Not connected to any session!");
								break;
							}
							ConsoleUtils.PlayerSessionControlCommand(args.Dequeue(), args, ref Instance.sessionControl, ref Instance.gameControl);
							break;
						case "gamecontrol":
							IPlayerControl control = Instance.gameControl;
							if(control == null)
							{
								ConsoleUtils.ErrorLine("Not connected to any game!");
								break;
							}
							ConsoleUtils.PlayerGameControlCommand(args.Dequeue(), args, control);
							break;
						case "exit":
							return;
						default:
							ConsoleUtils.ErrorLine("Unknown command!");
							break;
						}
					}
					catch(InvalidOperationException)
					{
						ConsoleUtils.ErrorLine("Too few arguments!");
					}
				}
			}
			catch(RemotingException e)
			{
				ConsoleUtils.ErrorLine("Remoting error!");
#if DEBUG
				ConsoleUtils.DebugLine(e.ToString());
#endif
				return;
			}
			catch(SerializationException e)
			{
				ConsoleUtils.ErrorLine("Serialization error!");
#if DEBUG
				ConsoleUtils.DebugLine(e.ToString());
#endif
				return;
			}
		}
		private static void ServerCommand(string arg, Queue<string> args, IServer server)
		{
			CreateSessionData csd;
			switch(arg)
			{
			case "name":
				ConsoleUtils.PrintLine(server.Name);
				break;
			case "description":
				ConsoleUtils.PrintLine(server.Description);
				break;
			case "sessions":
				if(args.Count == 0)
				{
					foreach(ISession s in server.Sessions)
					{
						ConsoleUtils.PrintLine("Session #{0}:", s.ID);
						ConsoleUtils.Print(s);
					}
					break;
				}

				switch(arg = args.Dequeue())
				{
				case "count":
					ConsoleUtils.PrintLine(server.Sessions.Count);
					break;
				default:
					ConsoleUtils.ErrorLine("Unknown command!");
					break;
				}
				break;
			case "session":
				ISession session;
				try
				{
					int id = int.Parse(arg = args.Dequeue());
					session = server.GetSession(id);
				}
				catch(FormatException)
				{
					ConsoleUtils.ErrorLine("Expected an ID!");
					break;
				}
				catch(InvalidIdException)
				{
					ConsoleUtils.ErrorLine("Invalid ID!");
					break;
				}

				if(args.Count == 0)
				{
					ConsoleUtils.Print(session);
					break;
				}

				CreatePlayerData cpd;
				string playerName;
				Password playerPassword;
				Password password;
				switch(arg = args.Dequeue())
				{
				case "join":
					if(Instance.sessionControl != null)
					{
						ConsoleUtils.ErrorLine("Already connected to a session!");
						break;
					}

					ConsoleUtils.Print("Session Password: ");
					password = ConsoleUtils.ReadPassword();

					ConsoleUtils.Print("Player Name: ");
					playerName = ConsoleUtils.ReadLine();
					ConsoleUtils.Print("Player Password: ");
					playerPassword = ConsoleUtils.ReadPassword();
					cpd = new CreatePlayerData { Name = playerName, Password = playerPassword };

					try
					{
						session.Join(password, cpd, Instance);
						ConsoleUtils.SuccessLine("Joined session!");
					}
					catch(GameException e)
					{
						ConsoleUtils.ErrorLine("Cannot join session: {0}", e.GetType());
					}
					break;
				case "joinai":
					if(Instance.sessionControl != null)
					{
						ConsoleUtils.ErrorLine("Already connected to a session!");
						break;
					}

					ConsoleUtils.Print("Session Password: ");
					password = ConsoleUtils.ReadPassword();

					AI.AIPlayer ai = new AI.AIPlayer();
					cpd = ai.CreateData;
					cpd.Name = "TestAI";

					try
					{
						session.Join(password, cpd, Instance);
						Instance.aiPlayer = ai;
						ConsoleUtils.SuccessLine("Joined session!");
					}
					catch(GameException e)
					{
						ConsoleUtils.ErrorLine("Cannot join session: {0}", e.GetType());
					}
					break;
				case "replace":
					int id;
					try
					{
						id = int.Parse(arg = args.Dequeue());
					}
					catch(FormatException)
					{
						ConsoleUtils.ErrorLine("Bad number format!");
						break;
					}

					if(Instance.sessionControl != null)
					{
						ConsoleUtils.ErrorLine("Already connected to a session!");
						break;
					}

					ConsoleUtils.Print("Session Password: ");
					password = new Password(ConsoleUtils.ReadLine());

					ConsoleUtils.Print("Player Name: ");
					playerName = ConsoleUtils.ReadLine();
					ConsoleUtils.Print("Player Password: ");
					playerPassword = ConsoleUtils.ReadPassword();
					cpd = new CreatePlayerData { Name = playerName, Password = playerPassword };

					try
					{
						session.Replace(id, password, cpd, Instance);
						ConsoleUtils.SuccessLine("Joined session!");
					}
					catch(GameException e)
					{
						ConsoleUtils.ErrorLine("Cannot join session: {0}", e.GetType());
					}
					break;
				case "replaceai":
					try
					{
						id = int.Parse(arg = args.Dequeue());
					}
					catch(FormatException)
					{
						ConsoleUtils.ErrorLine("Bad number format!");
						break;
					}

					if(Instance.sessionControl != null)
					{
						ConsoleUtils.ErrorLine("Already connected to a session!");
						break;
					}

					ConsoleUtils.Print("Session Password: ");
					password = new Password(ConsoleUtils.ReadLine());

					ConsoleUtils.Print("Player Password: ");
					playerPassword = ConsoleUtils.ReadPassword();
					ai = new AI.AIPlayer();
					cpd = ai.CreateData;
					cpd.Name = "TestAI";
					cpd.Password = playerPassword;

					try
					{
						session.Replace(id, password, cpd, Instance);
						Instance.aiPlayer = ai;
						ConsoleUtils.SuccessLine("Joined session!");
					}
					catch(GameException e)
					{
						ConsoleUtils.ErrorLine("Cannot join session: {0}", e.GetType());
					}
					break;
				default:
					ConsoleUtils.SessionCommand(arg, args, session);
					break;
				}
				break;
			case "test":
				if(Instance.sessionControl != null)
				{
					ConsoleUtils.ErrorLine("Already connected to a session!");
					break;
				}
				int playerCount = 4;
				if(args.Count != 0)
				{
					try
					{
						playerCount = int.Parse(arg = args.Dequeue());
					}
					catch(FormatException)
					{
						ConsoleUtils.ErrorLine("Bad number format!");
						break;
					}
				}

				csd = new CreateSessionData { Name = "Test", Description = "", MinPlayers = playerCount, MaxPlayers = playerCount, MaxSpectators = 0, DodgeCity = true };
				cpd = new CreatePlayerData { Name = "Human" };
				try
				{
					server.CreateSession(csd, cpd, Instance);
					ConsoleUtils.SuccessLine("Test session created!");
				}
				catch(GameException e)
				{
					ConsoleUtils.ErrorLine("Cannot create session: {0}", e.GetType());
				}
				break;
			case "testai":
				if(Instance.sessionControl != null)
				{
					ConsoleUtils.ErrorLine("Already connected to a session!");
					break;
				}
				playerCount = 4;
				if(args.Count != 0)
				{
					try
					{
						playerCount = int.Parse(arg = args.Dequeue());
					}
					catch(FormatException)
					{
						ConsoleUtils.ErrorLine("Bad number format!");
						break;
					}
				}

				csd = new CreateSessionData { Name = "Test", Description = "", MaxPlayers = playerCount, MinPlayers = playerCount, MaxSpectators = 0, DodgeCity = true };
				AI.AIPlayer ai = new AI.AIPlayer();
				cpd = ai.CreateData;
				cpd.Name = "TestAI";
				try
				{
					server.CreateSession(csd, cpd, Instance);
					Instance.aiPlayer = ai;
					ConsoleUtils.SuccessLine("Test AI session created!");
				}
				catch(GameException e)
				{
					ConsoleUtils.ErrorLine("Cannot create session: {0}", e.GetType());
				}
				break;
			case "createsession":
				if(Instance.sessionControl != null)
				{
					ConsoleUtils.ErrorLine("Already connected to a session!");
					break;
				}

				ConsoleUtils.Print("Session Name: ");
				string sessionName = ConsoleUtils.ReadLine();
				ConsoleUtils.Print("Session Description: ");
				string sessionDescription = ConsoleUtils.ReadLine();
				int sessionMinPlayers;
				int sessionMaxPlayers;
				int sessionMaxSpectators;
				try
				{
					ConsoleUtils.Print("Session MinPlayers: ");
					sessionMinPlayers = int.Parse(ConsoleUtils.ReadLine());
					ConsoleUtils.Print("Session MaxPlayers: ");
					sessionMaxPlayers = int.Parse(ConsoleUtils.ReadLine());
					ConsoleUtils.Print("Session MaxSpectators: ");
					sessionMaxSpectators = int.Parse(ConsoleUtils.ReadLine());
				}
				catch(FormatException)
				{
					ConsoleUtils.ErrorLine("Bad number format!");
					break;
				}

				ConsoleUtils.Print("Session PlayerPassword: ");
				Password sessionPlayerPassword = ConsoleUtils.ReadPassword();
				ConsoleUtils.Print("Session SpectatorPassword: ");
				Password sessionSpectatorPassword = ConsoleUtils.ReadPassword();
				ConsoleUtils.Print("Session ShufflePlayers: ");
				bool sessionShufflePlayers = ConsoleUtils.ReadLine().ToLower() == "y";
				ConsoleUtils.Print("Session DodgeCity: ");
				bool sessionDodgeCity = ConsoleUtils.ReadLine().ToLower() == "y";
				ConsoleUtils.Print("Session HighNoon: ");
				bool sessionHighNoon = ConsoleUtils.ReadLine().ToLower() == "y";
				ConsoleUtils.Print("Session FistfulOfCards: ");
				bool sessionFistfulOfCards = ConsoleUtils.ReadLine().ToLower() == "y";
				ConsoleUtils.Print("Session WildWestShow: ");
				bool sessionWildWestShow = ConsoleUtils.ReadLine().ToLower() == "y";
				csd = new CreateSessionData { Name = sessionName, Description = sessionDescription,
					MinPlayers = sessionMinPlayers, MaxPlayers = sessionMaxPlayers, MaxSpectators = sessionMaxSpectators,
					PlayerPassword = sessionPlayerPassword, SpectatorPassword = sessionSpectatorPassword,
					ShufflePlayers = sessionShufflePlayers, DodgeCity = sessionDodgeCity, HighNoon = sessionHighNoon,
				FistfulOfCards = sessionFistfulOfCards, WildWestShow = sessionWildWestShow };

				ConsoleUtils.Print("Player Name: ");
				playerName = ConsoleUtils.ReadLine();
				ConsoleUtils.Print("Player Password: ");
				playerPassword = ConsoleUtils.ReadPassword();
				cpd = new CreatePlayerData { Name = playerName, Password = playerPassword };

				try
				{
					server.CreateSession(csd, cpd, Instance);
					ConsoleUtils.SuccessLine("Session created!");
				}
				catch(GameException e)
				{
					ConsoleUtils.ErrorLine("Cannot create session: {0}", e.GetType());
				}
				break;
			default:
				ConsoleUtils.ErrorLine("Unknown command!");
				break;
			}
		}

		#region IPlayerEventListener implementation
		void IPlayerEventListener.OnJoinedSession(IPlayerSessionControl control)
		{
			sessionControl = control;
			ConsoleUtils.SessionEvent("Acquired session controller!");
			if(aiPlayer != null)
				aiPlayer.OnJoinedSession(control);
		}
		void IPlayerEventListener.OnJoinedGame (IPlayerControl control)
		{
			this.gameControl = control;
			ConsoleUtils.GameEvent("Acquired game controller!");
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
			ConsoleUtils.SessionEvent("Session ended!");
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
			ConsoleUtils.GameEvent("Game ended! You {0}", gameControl.PrivatePlayerView.IsWinner ? "won!" : "lost.");

			if(aiPlayer != null)
				aiPlayer.OnGameEnded();
		}

		void IEventListener.OnPlayerJoinedSession(IPlayer player)
		{
			ConsoleUtils.SessionEvent("Player #{0} '{1}' joined the session.", player.ID, player.Name);
			if(aiPlayer != null)
				aiPlayer.OnPlayerJoinedSession(player);
		}
		void IEventListener.OnSpectatorJoinedSession(ISpectator spectator)
		{
			ConsoleUtils.SessionEvent("Spectator #{0} '{1}' joined the session.", spectator.ID, spectator.Name);
			if(aiPlayer != null)
				aiPlayer.OnSpectatorJoinedSession(spectator);
		}
		void IEventListener.OnPlayerLeftSession(IPlayer player)
		{
			ConsoleUtils.SessionEvent("Player #{0} '{1}' left the session.", player.ID, player.Name);
			if(aiPlayer != null)
				aiPlayer.OnPlayerLeftSession(player);
		}
		void IEventListener.OnSpectatorLeftSession(ISpectator spectator)
		{
			ConsoleUtils.SessionEvent("Spectator #{0} '{1}' left the session.", spectator.ID, spectator.Name);
			if(aiPlayer != null)
				aiPlayer.OnSpectatorLeftSession(spectator);
		}
		void IEventListener.OnPlayerUpdated(IPlayer player)
		{
			ConsoleUtils.SessionEvent("Player #{0} '{1}' has been updated.", player.ID, player.Name);
			if(aiPlayer != null)
				aiPlayer.OnPlayerUpdated(player);
		}

		void IEventListener.OnChatMessage(IPlayer player, string message)
		{
			ConsoleUtils.SessionEvent("Chat: {0}: {1}", player.Name, message);
			if(aiPlayer != null)
				aiPlayer.OnChatMessage(player, message);
		}
		void IEventListener.OnChatMessage (ISpectator spectator, string message)
		{
			ConsoleUtils.SessionEvent("Chat: {0}: {1}", spectator.Name, message);
			if(aiPlayer != null)
				aiPlayer.OnChatMessage(spectator, message);
		}

		void IEventListener.OnNewRequest(RequestType requestType, IPublicPlayerView requestedPlayer, IPublicPlayerView causedBy)
		{
			ConsoleUtils.GameEvent("New request: {0} (player #{1}).", requestType, requestedPlayer == null ? 0 : requestedPlayer.ID);
			if(aiPlayer != null)
				aiPlayer.OnNewRequest(requestType, requestedPlayer, causedBy);
		}
		void IEventListener.OnPlayerDrewFromDeck(IPublicPlayerView player, ReadOnlyCollection<ICard> drawnCards)
		{
			ConsoleUtils.GameEvent("Player #{0} has drawn {1} cards from the deck.", player.ID, drawnCards.Count);
			if(aiPlayer != null)
				aiPlayer.OnPlayerDrewFromDeck(player, drawnCards);
		}
		void IEventListener.OnPlayerDrewFromGraveyard(IPublicPlayerView player, ReadOnlyCollection<ICard> drawnCards)
		{
			ConsoleUtils.GameEvent("Player #{0} has drawn {1} cards from the graveyard.", player.ID, drawnCards.Count);
			if(aiPlayer != null)
				aiPlayer.OnPlayerDrewFromGraveyard(player, drawnCards);
		}
		void IEventListener.OnPlayerDiscardedCard(IPublicPlayerView player, ICard card)
		{
			ConsoleUtils.GameEvent("Player #{0} discarded card {1} #{2}.", player.ID, card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerDiscardedCard(player, card);
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card)
		{
			ConsoleUtils.GameEvent("Player #{0} played card {1} #{2}.", player.ID, card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCard(player, card);
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
			ConsoleUtils.GameEvent("Player #{0} played card {1} #{2} on player #{3}.", player.ID, card.Type, card.ID, targetPlayer.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCard(player, card, targetPlayer);
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			ConsoleUtils.GameEvent("Player #{0} played card {1} #{2} on player #{3}'s card {4} #{5}.", player.ID, card.Type, card.ID, targetPlayer.ID, targetCard.Type, targetCard.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCard(player, card, targetPlayer, targetCard);
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard)
		{
			ConsoleUtils.GameEvent("Player #{0} played card {1} #{2} as card {3}.", player.ID, card.Type, card.ID, asCard);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCard(player, card, asCard);
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer)
		{
			ConsoleUtils.GameEvent("Player #{0} played card {1} #{2} as card {3} on player #{4}.", player.ID, card.Type, card.ID, asCard, targetPlayer.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCard(player, card, asCard, targetPlayer);
		}
		void IEventListener.OnPlayerPlayedCard(IPublicPlayerView player, ICard card, CardType asCard, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			ConsoleUtils.GameEvent("Player #{0} played card {1} #{2} as card {3} on player #{4}'s card {5} #{6}.", player.ID, card.Type, card.ID, asCard, targetPlayer.ID, targetCard.Type, targetCard.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCard(player, card, asCard, targetPlayer, targetCard);
		}
		void IEventListener.OnPlayerPlayedCardOnTable(IPublicPlayerView player, ICard card)
		{
			ConsoleUtils.GameEvent("Player #{0} played card {1} #{2} on table.", player.ID, card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPlayedCardOnTable(player, card);
		}
		void IEventListener.OnPassedTableCard(IPublicPlayerView player, ICard card, IPublicPlayerView targetPlayer)
		{
			ConsoleUtils.GameEvent("Player #{0} passed card {1} #{2} to player #{3}.", player.ID, card.Type, card.ID, targetPlayer.ID);
			if(aiPlayer != null)
				aiPlayer.OnPassedTableCard(player, card, targetPlayer);
		}
		void IEventListener.OnPlayerPassed(IPublicPlayerView player)
		{
			ConsoleUtils.GameEvent("Player #{0} passed.", player.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPassed(player);
		}
		void IEventListener.OnPlayerRespondedWithCard (IPublicPlayerView player, ICard card)
		{
			ConsoleUtils.GameEvent("Player #{0} responded with card {1} #{2}.", player.ID, card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerRespondedWithCard(player, card);
		}
		void IEventListener.OnPlayerRespondedWithCard(IPublicPlayerView player, ICard card, CardType asCard)
		{
			ConsoleUtils.GameEvent("Player #{0} responded with card {1} #{2} as card {3}.", player.ID, card.Type, card.ID, asCard);
			if(aiPlayer != null)
				aiPlayer.OnPlayerRespondedWithCard(player, card, asCard);
		}
		void IEventListener.OnDrawnIntoSelection(ReadOnlyCollection<ICard> drawnCards)
		{
			ConsoleUtils.GameEvent("{0} cards were drawn into the selection.", drawnCards.Count);
			if(aiPlayer != null)
				aiPlayer.OnDrawnIntoSelection(drawnCards);
		}
		void IEventListener.OnPlayerPickedFromSelection (IPublicPlayerView player, ICard card)
		{
			ConsoleUtils.GameEvent("Player #{0} picked card {1} #{2} from selection.", player.ID, card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerPickedFromSelection(player, card);
		}
		void IEventListener.OnUndrawnFromSelection(ICard card)
		{
			ConsoleUtils.GameEvent("Card {0} #{1} was undrawn from the selection.", card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnUndrawnFromSelection(card);
		}
		void IEventListener.OnPlayerStoleCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			ConsoleUtils.GameEvent("Player #{0} stole card {1} #{2} from player #{3}.", player.ID, targetCard.Type, targetCard.ID, targetPlayer.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerStoleCard(player, targetPlayer, targetCard);
		}
		void IEventListener.OnPlayerCancelledCard(IPublicPlayerView player, IPublicPlayerView targetPlayer, ICard targetCard)
		{
			ConsoleUtils.GameEvent("Player #{0} cancelled card {1} #{2} of player #{3}.", player.ID, targetCard.Type, targetCard.ID, targetPlayer.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerCancelledCard(player, targetPlayer, targetCard);
		}
		void IEventListener.OnDeckChecked(ICard card)
		{
			ConsoleUtils.GameEvent("Card {0} #{1} was checked from the deck.", card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnDeckChecked(card);
		}
		void IEventListener.OnCardCancelled(ICard card)
		{
			ConsoleUtils.GameEvent("Card {0} #{1} was cancelled.", card.Type, card.ID);
			if(aiPlayer != null)
				aiPlayer.OnCardCancelled(card);
		}
		void IEventListener.OnPlayerCheckedDeck(IPublicPlayerView player, ICard checkedCard, CardType causedBy, bool result)
		{
			ConsoleUtils.GameEvent("Player #{0} checked card {1} #{2} from deck because of card {3} and {4}.", player.ID, checkedCard.Type, checkedCard.ID, causedBy, result ? "succeeded" : "failed");
			if(aiPlayer != null)
				aiPlayer.OnPlayerCheckedDeck(player, checkedCard, causedBy, result);
		}
		void IEventListener.OnLifePointsChanged(IPublicPlayerView player, int delta, IPublicPlayerView causedBy)
		{
			ConsoleUtils.GameEvent("Player #{0} {1} {2} lives{3}.", player.ID, delta > 0 ? "gained" : "lost", Math.Abs(delta), causedBy == null ? "" : " because of player #" + causedBy.ID);
			if(aiPlayer != null)
				aiPlayer.OnLifePointsChanged(player, delta, causedBy);
		}
		void IEventListener.OnPlayerDied(IPublicPlayerView player, IPublicPlayerView causedBy)
		{
			ConsoleUtils.GameEvent("Player #{0} died{1}, he was {2}.", player.ID, causedBy == null ? "" : " because of player #" + causedBy.ID, player.Role);
			if(aiPlayer != null)
				aiPlayer.OnPlayerDied(player, causedBy);
		}
		void IEventListener.OnPlayerUsedAbility (IPublicPlayerView player)
		{
			ConsoleUtils.GameEvent("Player #{0} used ability.", player.ID);
			if(aiPlayer != null)
				aiPlayer.OnPlayerUsedAbility(player);
		}
		void IEventListener.OnDeckRegenerated ()
		{
			ConsoleUtils.GameEvent("The deck was regenerated.");
			if(aiPlayer != null)
				aiPlayer.OnDeckRegenerated();
		}
		#endregion
	}
}

