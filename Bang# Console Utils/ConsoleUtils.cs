// cs
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
using System.Linq;
using System.Text;
namespace Bang
{
	/// <summary>
	/// This class contains utility methods for communication with the user via console.
	/// </summary>
	public static class ConsoleUtils
	{
		// 'Black on White' theme:
		private const ConsoleColor defaultFg = ConsoleColor.Black;
		private const ConsoleColor defaultBg = ConsoleColor.White;
		private const ConsoleColor debugColor = ConsoleColor.DarkMagenta;
		private const ConsoleColor successColor = ConsoleColor.DarkGreen;
		private const ConsoleColor failColor = ConsoleColor.DarkRed;
		private const ConsoleColor inputColor = ConsoleColor.DarkBlue;
		private const ConsoleColor eventColor = ConsoleColor.DarkYellow;

		// 'White on Black' theme:
		//private const ConsoleColor defaultFg = ConsoleColor.White;
		//private const ConsoleColor defaultBg = ConsoleColor.Black;
		//private const ConsoleColor debugColor = ConsoleColor.Magenta;
		//private const ConsoleColor successColor = ConsoleColor.Green;
		//private const ConsoleColor failColor = ConsoleColor.Red;
		//private const ConsoleColor inputColor = ConsoleColor.Blue;
		//private const ConsoleColor eventColor = ConsoleColor.Yellow;

		/// <summary>
		/// Gets or sets the array of commands (or sub-commands) that are used for the autocompletion.
		/// </summary>
		public static string[] Commands
		{
			get;
			set;
		}

		static ConsoleUtils()
		{
			Console.ForegroundColor = defaultFg;
			Console.BackgroundColor = defaultBg;
			Console.Clear();
			Commands = new string[] {};
		}

		/// <summary>
		/// Prints the main properties of an <see cref="IPublicPlayerView"/> to the console.
		/// </summary>
		/// <param name="player">
		/// The <see cref="IPublicPlayerView"/> to print.
		/// </param>
		public static void Print(IPublicPlayerView player)
		{
			if(player == null)
				return;
			PrintLine("\tIsAlive: {0}", player.IsAlive);
			PrintLine("\tIsWinner: {0}", player.IsWinner);
			PrintLine("\tLifePoints: {0}/{1}", player.LifePoints, player.MaxLifePoints);
			PrintLine("\tRole: {0}", player.Role);
			PrintLine("\tCharacter: {0}", player.CharacterType);
		}
		/// <summary>
		/// Prints the main properties of an <see cref="ICard"/> to the console.
		/// </summary>
		/// <param name="card">
		/// The <see cref="ICard"/> to print.
		/// </param>
		public static void Print(ICard card)
		{
			if(card == null)
				return;
			PrintLine("\tType: {0}", card.Type);
			PrintLine("\tColor: {0}", card.Color);
			PrintLine("\tRank: {0}", card.Rank);
			PrintLine("\tSuit: {0}", card.Suit);
		}
		public static void Print(ISpectator spectator)
		{
			if(spectator == null)
				return;
			PrintLine("\tName: {0}", spectator.Name);
		}
		public static void Print(IPlayer player)
		{
			if(player == null)
				return;
			PrintLine("\tName: {0}", player.Name);
			PrintLine("\tIsAI: {0}", player.IsAI);
			PrintLine("\tIsCreator: {0}", player.IsCreator);
			PrintLine("\tHasListener: {0}", player.HasListener);
			PrintLine("\tHasPassword: {0}", player.HasPassword);
		}
		public static void Print(ISession session)
		{
			if(session == null)
				return;
			PrintLine("\tName: {0}", session.Name);
			PrintLine("\tDescription: {0}", session.Description);
			PrintLine("\tState: {0}", session.State);
		}

		private static string And(string first, string second)
		{
			int minLen = Math.Min(first.Length, second.Length);
			for(int i = 0; i < minLen; i++)
			{
				if(first[i] != second[i])
					return first.Substring(0, i);
			}
			return first.Substring(0, minLen);
		}
		private static List<string> history = new List<string>();
		private static int ConsolePos
		{
			get { return Console.CursorLeft + Console.CursorTop * Console.BufferWidth; }
			set
			{
				Console.CursorLeft = value % Console.BufferWidth;
				Console.CursorTop = value / Console.BufferWidth;
			}
		}
		private static void Backspace(int len)
		{
			for(int i = 0; i < len; i++)
			{
				// Also write space because of the stupid Windows console...
				Console.Write('\b');
				Console.Write(' ');
				Console.Write('\b');
			}
		}
		public static string ReadLine()
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = inputColor;
			StringBuilder line = new StringBuilder();
			int pos = 0;
			ConsoleKeyInfo key;
			string currentText = null;
			int historyIndex = -1;
			int len;
			while((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
			{
				switch(key.Key)
				{
				case ConsoleKey.Escape:
					len = line.Length;
					ConsolePos += len - pos;
					line = new StringBuilder();
					Backspace(len);
					pos = 0;
					break;
				case ConsoleKey.UpArrow:
					if(historyIndex < 0)
						historyIndex = history.Count;
					if(historyIndex - 1 < 0)
						break;
					
					historyIndex--;
					currentText = line.ToString();
					len = line.Length;
					ConsolePos += len - pos;
					Backspace(len);

					string h = history[historyIndex];
					Console.Write(h);
					line = new StringBuilder(h);
					pos = h.Length;
					break;
				case ConsoleKey.DownArrow:
					if(historyIndex < 0)
						break;
					
					len = line.Length;
					ConsolePos += len - pos;
					Backspace(len);

					string newText;
					if(++historyIndex >= history.Count)
					{
						historyIndex = -1;
						newText = currentText;
					}
					else
						newText = history[historyIndex];
					
					Console.Write(newText);
					line = new StringBuilder(newText);
					pos = newText.Length;
					break;
				case ConsoleKey.RightArrow:
					if(pos == line.Length)
						break;
					ConsolePos++;
					pos++;
					break;
				case ConsoleKey.LeftArrow:
					if(pos == 0)
						break;
					ConsolePos--;
					pos--;
					break;
				case ConsoleKey.Home:
					if(pos == 0)
						break;
					ConsolePos -= pos;
					pos = 0;
					break;
				case ConsoleKey.End:
					if(pos == line.Length)
						break;
					len = line.Length;
					ConsolePos += len - pos;
					pos = len;
					break;
				case ConsoleKey.Backspace:
					if(pos == 0)
						break;
					
					len = line.Length;
					ConsolePos += len - pos;
					Backspace(len);

					line = line.Remove(pos - 1, 1);
					Console.Write(line.ToString());
					pos--;
					ConsolePos -= line.Length - pos;
					break;
				case ConsoleKey.Delete:
					if(pos == line.Length)
						break;
					
					len = line.Length;
					ConsolePos += len - pos;
					Backspace(len);

					line = line.Remove(pos, 1);
					Console.Write(line.ToString());
					ConsolePos -= line.Length - pos;
					break;
				case ConsoleKey.Tab:
					string text = line.ToString().ToLower().Split(' ').LastOrDefault();
					if(string.IsNullOrEmpty(text))
						break;
					string suggested = "";
					foreach(string cmd in Commands)
						if(cmd.StartsWith(text))
						{
							if(suggested.Length == 0)
								suggested = cmd + ' ';
							else
							{
								suggested = And(suggested, cmd);
								if(suggested.Length == 0)
									break;
							}
						}

					if(suggested.Length != 0)
					{
						ConsolePos += line.Length - pos;
						string addition = suggested.Substring(text.Length);
						Console.Write(addition);
						line = line.Append(addition);
						pos = line.Length;
					}

					break;
				default:
					if(key.KeyChar == '\0')
						break;
					line = line.Insert(pos, key.KeyChar);
					len = line.Length;
					ConsolePos += len - pos;
					Backspace(len);
					Console.Write(line.ToString());
					pos++;
					ConsolePos -= len - pos;
					break;
				}
			}
			Console.WriteLine();
			Console.ForegroundColor = backup;
			string txt = line.ToString();
			history.Add(txt);
			return txt;
		}
		public static Queue<string> ReadCommand()
		{
			Queue<string> args = new Queue<string>(ReadLine().ToLower().Split(' ').Where(s => s.Length > 0));
			return args;
		}

		public static void Print(object value)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = defaultFg;
			Console.Write(value);
			Console.ForegroundColor = backup;
		}
		public static void Print(string text)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = defaultFg;
			Console.Write(text);
			Console.ForegroundColor = backup;
		}
		public static void Print(string format, params object[] args)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = defaultFg;
			Console.Write(format, args);
			Console.ForegroundColor = backup;
		}
		public static void PrintLine()
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = defaultFg;
			Console.WriteLine();
			Console.ForegroundColor = backup;
		}
		public static void PrintLine(object value)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = defaultFg;
			Console.WriteLine(value);
			Console.ForegroundColor = backup;
		}
		public static void PrintLine(string message)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = defaultFg;
			Console.WriteLine(message);
			Console.ForegroundColor = backup;
		}
		public static void PrintLine(string format, params object[] args)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = defaultFg;
			Console.WriteLine(format, args);
			Console.ForegroundColor = backup;
		}
		public static void DebugLine(object value)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = debugColor;
			Console.Write("Debug: ");
			Console.WriteLine(value);
			Console.ForegroundColor = backup;
		}
		public static void DebugLine(string message)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = debugColor;
			Console.Write("Debug: ");
			Console.WriteLine(message);
			Console.ForegroundColor = backup;
		}
		public static void DebugLine(string format, params object[] args)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = debugColor;
			Console.Write("Debug: ");
			Console.WriteLine(format, args);
			Console.ForegroundColor = backup;
		}
		public static void SuccessLine()
		{
			SuccessLine("Done!");
		}
		public static void SuccessLine(string message)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = successColor;
			Console.WriteLine(message);
			Console.ForegroundColor = backup;
		}
		public static void SuccessLine(string format, params object[] args)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = successColor;
			Console.WriteLine(format, args);
			Console.ForegroundColor = backup;
		}
		public static void ErrorLine(string error)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = failColor;
			Console.Write("ERROR: ");
			Console.WriteLine(error);
			Console.ForegroundColor = backup;
		}
		public static void ErrorLine(string format, params object[] args)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = failColor;
			Console.Write("ERROR: ");
			Console.WriteLine(format, args);
			Console.ForegroundColor = backup;
		}
		public static void SessionEvent(string message)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = eventColor;
			Console.Write("Session event: ");
			Console.WriteLine(message);
			Console.ForegroundColor = backup;
		}
		public static void SessionEvent(string format, params object[] args)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = eventColor;
			Console.Write("Session event: ");
			Console.WriteLine(format, args);
			Console.ForegroundColor = backup;
		}
		public static void GameEvent(string message)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = eventColor;
			Console.Write("Game event: ");
			Console.WriteLine(message);
			Console.ForegroundColor = backup;
		}
		public static void GameEvent(string format, params object[] args)
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = eventColor;
			Console.Write("Game event: ");
			Console.WriteLine(format, args);
			Console.ForegroundColor = backup;
		}

		public static void PlayerCommand(string arg, Queue<string> args, IPublicPlayerView player)
		{
			switch(arg)
			{
			case "issheriff":
				PrintLine(player.IsSheriff);
				break;
			case "isalive":
				PrintLine(player.IsAlive);
				break;
			case "iswinner":
				PrintLine(player.IsWinner);
				break;
			case "lifepoints":
				PrintLine(player.LifePoints);
				break;
			case "maxlifepoints":
				PrintLine(player.MaxLifePoints);
				break;
			case "hand":
				if(args.Count == 0)
				{
					foreach(ICard card in player.Hand)
					{
						PrintLine("Card #{0}:", card.ID);
						Print(card);
					}
					break;
				}

				switch(arg = args.Dequeue())
				{
				case "count":
					PrintLine(player.Hand.Count);
					break;
				default:
					ErrorLine("Unknown command!");
					break;
				}

				break;
			case "table":
				if(args.Count == 0)
				{
					foreach(ICard card in player.Table)
					{
						PrintLine("Card #{0}:", card.ID);
						Print(card);
					}
					break;
				}

				switch(arg = args.Dequeue())
				{
				case "count":
					PrintLine(player.Table.Count);
					break;
				default:
					ErrorLine("Unknown command!");
					break;
				}

				break;
			case "character":
				PrintLine(player.CharacterType);
				break;
			case "role":
				PrintLine(player.Role);
				break;
			default:
				ErrorLine("Unknown command!");
				break;
			}
		}
		public static void GameCommand(string arg, Queue<string> args, IGame game)
		{
			switch(arg)
			{
			case "players":
				if(args.Count == 0)
				{
					foreach(IPublicPlayerView player in game.Players)
					{
						PrintLine("Player #{0}:", player.ID);
						Print(player);
					}
					break;
				}
				switch(arg = args.Dequeue())
				{
				case "count":
					PrintLine(game.Players.Count);
					break;
				default:
					ErrorLine("Unknown command!");
					break;
				}
				break;
			case "player":
				IPublicPlayerView player;
				try
				{
					int id = int.Parse(arg = args.Dequeue());
					player = game.GetPublicPlayerView(id);
				}
				catch(FormatException)
				{
					ErrorLine("Bad number format!");
					break;
				}
				catch(InvalidIdException)
				{
					ErrorLine("Invalid player ID!");
					break;
				}

				if(args.Count == 0)
				{
					Print(player);
					break;
				}
				PlayerCommand(args.Dequeue(), args, player);
				break;
			case "selection":
				if(args.Count == 0)
				{
					foreach(ICard card in game.Selection)
					{
						PrintLine("Card #{0}:", card.ID);
						Print(card);
					}
					break;
				}
				
				switch(arg = args.Dequeue())
				{
				case "count":
					PrintLine(game.Selection.Count);
					break;
				default:
					ErrorLine("Unknown command!");
					break;
				}
				break;
			case "graveyardtop":
				Print(game.GraveyardTop);
				break;
			case "reqtype":
				PrintLine(game.RequestType);
				break;
			case "current":
				PrintLine("Current player - #{0}:", game.CurrentPlayer.ID);
				Print(game.CurrentPlayer);
				break;
			case "requested":
				PrintLine("Requested player - #{0}:", game.RequestedPlayer.ID);
				break;
			case "causedby":
				PrintLine("Caused by player #{0}:", game.CausedBy.ID);
				break;
			default:
				ErrorLine("Unknown command!");
				break;
			}
		}
		public static void SessionPlayerCommand(string arg, Queue<string> args, IPlayer player)
		{
			switch(arg)
			{
			case "name":
				PrintLine(player.Name);
				break;
			case "haspassword":
				PrintLine(player.HasPassword);
				break;
			case "iscreator":
				PrintLine(player.IsCreator);
				break;
			case "isai":
				PrintLine(player.IsAI);
				break;
			case "haslistener":
				PrintLine(player.HasListener);
				break;
			case "score":
				PrintLine(player.Score);
				break;
			case "turnsplayed":
				PrintLine(player.TurnsPlayed);
				break;
			case "wins":
				PrintLine(player.Victories);
				break;
			case "winsassheriff":
				PrintLine(player.VictoriesAsSheriff);
				break;
			case "winsasdeputy":
				PrintLine(player.VictoriesAsDeputy);
				break;
			case "winsasoutlaw":
				PrintLine(player.VictoriesAsOutlaw);
				break;
			case "winsasrenegade":
				PrintLine(player.VictoriesAsRenegade);
				break;
			default:
				ErrorLine("Unknown command!");
				break;
			}
		}
		public static void SessionSpectatorCommand(string arg, Queue<string> args, ISpectator spectator)
		{
			switch(arg)
			{
			case "name":
				PrintLine(spectator.Name);
				break;
			default:
				ErrorLine("Unknown command!");
				break;
			}
		}
		public static void SessionCommand(string arg, Queue<string> args, ISession session)
		{
			switch(arg)
			{
			case "name":
				PrintLine(session.Name);
				break;
			case "description":
				PrintLine(session.Description);
				break;
			case "state":
				PrintLine(session.State);
				break;
			case "minplayers":
				PrintLine(session.MinPlayers);
				break;
			case "maxplayers":
				PrintLine(session.MaxPlayers);
				break;
			case "maxspectators":
				PrintLine(session.MaxSpectators);
				break;
			case "hasplayerpassword":
				PrintLine(session.HasPlayerPassword);
				break;
			case "hasspectatorpassword":
				PrintLine(session.HasSpectatorPassword);
				break;
			case "creator":
				PrintLine("Creator - #{0}:", session.Creator.ID);
				Print(session.Creator);
				break;
			case "dodgecity":
				PrintLine(session.DodgeCity);
				break;
			case "highnoon":
				PrintLine(session.HighNoon);
				break;
			case "fistfulofcards":
				PrintLine(session.FistfulOfCards);
				break;
			case "wildwestshow":
				PrintLine(session.WildWestShow);
				break;
			case "gamesplayed":
				PrintLine(session.GamesPlayed);
				break;
			case "spectators":
				if(args.Count == 0)
				{
					foreach(ISpectator spectator in session.Spectators)
					{
						PrintLine("Player #{0}:", spectator.ID);
						Print(spectator);
					}
					break;
				}

				switch(arg = args.Dequeue())
				{
				case "count":
					PrintLine(session.Spectators.Count);
					break;
				default:
					ErrorLine("Unknown command!");
					break;
				}
				break;
			case "spectator":
				ISpectator spectator;
				try
				{
					int id = int.Parse(arg = args.Dequeue());
					spectator = session.GetSpectator(id);
				}
				catch(FormatException)
				{
					ErrorLine("Expected an ID!");
					break;
				}
				catch(InvalidIdException)
				{
					ErrorLine("Invalid ID!");
					break;
				}

				if(args.Count == 0)
				{
					Print(spectator);
					break;
				}

				SessionSpectatorCommand(args.Dequeue(), args, spectator);
				break;
			case "players":
				if(args.Count == 0)
				{
					foreach(IPlayer player in session.Players)
					{
						PrintLine("Player #{0}:", player.ID);
						Print(player);
					}
					break;
				}

				switch(arg = args.Dequeue())
				{
				case "count":
					PrintLine(session.Players.Count);
					break;
				default:
					ErrorLine("Unknown command!");
					break;
				}

				break;
			case "player":
				IPlayer player;
				try
				{
					int id = int.Parse(arg = args.Dequeue());
					player = session.GetPlayer(id);
				}
				catch(FormatException)
				{
					ErrorLine("Expected an ID!");
					break;
				}
				catch(InvalidIdException)
				{
					ErrorLine("Invalid ID!");
					break;
				}

				if(args.Count == 0)
				{
					Print(player);
					break;
				}
				SessionPlayerCommand(args.Dequeue(), args, player);
				break;
			default:
				ErrorLine("Unknown command!");
				break;
			}
		}
		public static void PlayerSessionControlCommand(string arg, Queue<string> args, ref IPlayerSessionControl sessionControl, ref IPlayerControl gameControl)
		{
			switch(arg)
			{
			case "session":
				ISession session = sessionControl.Session;
				SessionCommand(args.Dequeue(), args, session);
				break;
			case "player":
				IPlayer player = sessionControl.Player;
				SessionPlayerCommand(args.Dequeue(), args, player);
				break;
			case "chat":
				Console.Write("Message: ");
				string message = ReadLine();
				sessionControl.SendChatMessage(message);
				SuccessLine("Message sent!");
				break;
			case "disconnect":
				sessionControl.Disconnect();
				sessionControl = null;
				gameControl = null;
				SuccessLine("Disconnected from the session!");
				break;
			case "startgame":
				try
				{
					sessionControl.StartGame();
					SuccessLine("Game started!");
				}
				catch(GameException e)
				{
					ErrorLine("Unable to start game: " + e.GetType() + "!");
				}
				break;
			case "endsession":
				try
				{
					sessionControl.EndSession();
					SuccessLine("Session ended!");
				}
				catch(GameException e)
				{
					ErrorLine("Can't end session: " + e.GetType() + "!");
				}
				break;
			default:
				ErrorLine("Unknown command!");
				break;
			}
		}
		public static void SpectatorSessionControlCommand(string arg, Queue<string> args, ref ISpectatorSessionControl sessionControl, ref ISpectatorControl gameControl)
		{
			switch(arg)
			{
			case "session":
				ISession session = sessionControl.Session;
				SessionCommand(args.Dequeue(), args, session);
				break;
			case "spectator":
				ISpectator spectator = sessionControl.Spectator;
				SessionSpectatorCommand(args.Dequeue(), args, spectator);
				break;
			case "chat":
				Console.Write("Message: ");
				string message = ReadLine();
				sessionControl.SendChatMessage(message);
				SuccessLine("Message sent!");
				break;
			case "disconnect":
				sessionControl.Disconnect();
				sessionControl = null;
				gameControl = null;
				SuccessLine("Disconnected from the session!");
				break;
			default:
				ErrorLine("Unknown command!");
				break;
			}
		}
		public static void PlayerGameControlCommand(string arg, Queue<string> args, IPlayerControl control)
		{
			switch(arg)
			{
			case "game":
				IGame game = control.Game;
				GameCommand(args.Dequeue(), args, game);
				break;
			case "player":
				IPrivatePlayerView player = control.PrivatePlayerView;
				switch(arg = args.Dequeue())
				{
				case "hand":
					if(args.Count == 0)
					{
						foreach(ICard card in player.Hand)
						{
							PrintLine("Card #{0}:", card.ID);
							Print(card);
						}
						break;
					}

					switch(arg = args.Dequeue())
					{
					case "count":
						PrintLine(player.Hand.Count);
						break;
					default:
						ErrorLine("Unknown command!");
						break;
					}
					break;
				case "role":
					PrintLine(player.Role);
					break;
				case "selection":
					if(args.Count == 0)
					{
						foreach(ICard card in player.Selection)
						{
							PrintLine("Card #{0}:", card.ID);
							Print(card);
						}
						break;
					}

					switch(arg = args.Dequeue())
					{
					case "count":
						PrintLine(player.Selection.Count);
						break;
					default:
						ErrorLine("Unknown command!");
						break;
					}
					break;
				default:
					PlayerCommand(arg, args, player);
					break;
				}
				break;
			case "responddraw":
				try
				{
					control.RespondDraw();
					SuccessLine();
				}
				catch(GameException e)
				{
					ErrorLine("Response refused: " + e.GetType() + "!");
				}
				break;
			case "respondcard":
				try
				{
					int id = int.Parse(arg = args.Dequeue());
					control.RespondCard(id);
					SuccessLine();
				}
				catch(FormatException)
				{
					ErrorLine("Bad number format!");
				}
				catch(GameException e)
				{
					ErrorLine("Response refused: " + e.GetType() + "!");
				}
				break;
			case "respondplayer":
				try
				{
					int id = int.Parse(arg = args.Dequeue());
					control.RespondPlayer(id);
					SuccessLine();
				}
				catch(FormatException)
				{
					ErrorLine("Bad number format!");
				}
				catch(GameException e)
				{
					ErrorLine("Response refused: " + e.GetType() + "!");
				}
				break;
			case "respondnoaction":
				try
				{
					control.RespondNoAction();
					SuccessLine();
				}
				catch(GameException e)
				{
					ErrorLine("Response refused: " + e.GetType() + "!");
				}
				break;
			case "responduseability":
				try
				{
					control.RespondUseAbility();
					SuccessLine();
				}
				catch(GameException e)
				{
					ErrorLine("Response refused: " + e.GetType() + "!");
				}
				break;
			default:
				ErrorLine("Unknown command!");
				break;
			}
		}
		public static void SpectatorGameControlCommand(string arg, Queue<string> args, ISpectatorControl control)
		{
			switch(arg)
			{
			case "game":
				IGame game = control.Game;
				GameCommand(args.Dequeue(), args, game);
				break;
			default:
				ErrorLine("Unknown command!");
				break;
			}
		}
	}
}

