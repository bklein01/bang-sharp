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
using System.Collections.ObjectModel;
namespace Bang.ConsoleUtils
{
	/// <summary>
	/// This class contains utility methods for communication with the user via console.
	/// </summary>
	public static class ConsoleHelper
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

		static ConsoleHelper()
		{
			Console.ForegroundColor = defaultFg;
			Console.BackgroundColor = defaultBg;
			Console.Clear();
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

		public static string ReadLine()
		{
			ConsoleColor backup = Console.ForegroundColor;
			Console.ForegroundColor = inputColor;
			string res = Console.ReadLine();
			Console.ForegroundColor = backup;
			return res;
		}
		public static Password ReadPassword()
		{
			StringBuilder line = new StringBuilder();
			ConsoleKeyInfo key;
			while((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
			{
				switch(key.Key)
				{
				case ConsoleKey.Escape:
					line = new StringBuilder();
					break;
				case ConsoleKey.Backspace:
					line = line.Remove(line.Length - 1, 1);
					break;
				default:
					if(key.KeyChar == '\0')
						break;
					line = line.Append(key.KeyChar);
					break;
				}
			}
			Console.WriteLine();
			return new Password(line.ToString());
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
		private static string ReadCommandLine(ICommand command)
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
					string[] rawLine = line.ToString(0, pos).ToLower().Split(' ');
					string text = rawLine.Length == 0 ? "" : rawLine[rawLine.Length - 1];
					IEnumerable<string> cmdLine = rawLine.Take(rawLine.Length - 1).Where(s => s.Length > 0);
					ICommand cmd = command;
					foreach(string t in cmdLine)
					{
						ICommand child = cmd.GetSubcommand(t);
						if(child != null)
							cmd = child;
					}
					string suggested = "";
					foreach(string t in cmd.Subcommands)
						if(t.StartsWith(text))
						{
							if(suggested.Length == 0)
								suggested = t + ' ';
							else
							{
								suggested = And(suggested, t);
								if(suggested.Length == 0)
									break;
							}
						}

					if(suggested.Length != 0)
					{
						string addition = suggested.Substring(text.Length);
						line = line.Insert(pos, addition);
						len = line.Length;
						ConsolePos += len - pos;
						Backspace(len);
						Console.Write(line.ToString());
						pos += addition.Length;
						ConsolePos -= len - pos;
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
		public static void ReadAndExecute<Out>(this NestedCommand<Out> command)
		{
			command.ReadAndExecute<object>(new object());
		}
		public static void ReadAndExecute<In>(this Command<In> command, In param)
		{
			command.Execute(param, new Queue<string>(ReadCommandLine(command).ToLower().Split(' ').Where(s => s.Length > 0)));
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

		public static void MakeCardCollectionCommand<In>(this NestedCommand<In, ReadOnlyCollection<ICard>> command)
		{
			command[""] = new FinalCommand<ReadOnlyCollection<ICard>>((cards, cmd) =>
			{
				foreach(ICard card in cards)
				{
					PrintLine("Card #{0}:", card.ID);
					Print(card);
				}
			});
			command["count"] = new FinalCommand<ReadOnlyCollection<ICard>>((cards, cmd) => { PrintLine(cards.Count); });
		}
		public static void MakePlayerCollectionCommand<In>(this NestedCommand<In, ReadOnlyCollection<IPublicPlayerView>> command)
		{
			command[""] = new FinalCommand<ReadOnlyCollection<IPublicPlayerView>>((players, cmd) =>
			{
				foreach(IPublicPlayerView player in players)
				{
					PrintLine("Player #{0}:", player.ID);
					Print(player);
				}
			});
			command["count"] = new FinalCommand<ReadOnlyCollection<IPublicPlayerView>>((players, cmd) => { PrintLine(players.Count); });
		}
		public static void MakePublicPlayerViewCommand<In>(this NestedCommand<In, IPublicPlayerView> command)
		{
			command["issheriff"] = new FinalCommand<IPublicPlayerView>((player, cmd) => { PrintLine(player.IsSheriff); });
			command["isalive"] = new FinalCommand<IPublicPlayerView>((player, cmd) => { PrintLine(player.IsAlive); });
			command["iswinner"] = new FinalCommand<IPublicPlayerView>((player, cmd) => { PrintLine(player.IsWinner); });
			command["lifepoints"] = new FinalCommand<IPublicPlayerView>((player, cmd) => { PrintLine(player.LifePoints); });
			command["maxlifepoints"] = new FinalCommand<IPublicPlayerView>((player, cmd) => { PrintLine(player.MaxLifePoints); });
			NestedCommand<IPublicPlayerView, ReadOnlyCollection<ICard>> handCommand = new NestedCommand<IPublicPlayerView, ReadOnlyCollection<ICard>>((player, cmd) => player.Hand);
			handCommand.MakeCardCollectionCommand();
			command["hand"] = handCommand;
			NestedCommand<IPublicPlayerView, ReadOnlyCollection<ICard>> tableCommand = new NestedCommand<IPublicPlayerView, ReadOnlyCollection<ICard>>((player, cmd) => player.Table);
			tableCommand.MakeCardCollectionCommand();
			command["table"] = tableCommand;
			command["character"] = new FinalCommand<IPublicPlayerView>((player, cmd) => { PrintLine(player.CharacterType); });
			command["role"] = new FinalCommand<IPublicPlayerView>((player, cmd) => { PrintLine(player.IsAlive); });
		}
		public static void MakePrivatePlayerViewCommand<In>(this NestedCommand<In, IPrivatePlayerView> command)
		{
			command["issheriff"] = new FinalCommand<IPrivatePlayerView>((player, cmd) => { PrintLine(player.IsSheriff); });
			command["isalive"] = new FinalCommand<IPrivatePlayerView>((player, cmd) => { PrintLine(player.IsAlive); });
			command["iswinner"] = new FinalCommand<IPrivatePlayerView>((player, cmd) => { PrintLine(player.IsWinner); });
			command["lifepoints"] = new FinalCommand<IPrivatePlayerView>((player, cmd) => { PrintLine(player.LifePoints); });
			command["maxlifepoints"] = new FinalCommand<IPrivatePlayerView>((player, cmd) => { PrintLine(player.MaxLifePoints); });
			NestedCommand<IPrivatePlayerView, ReadOnlyCollection<ICard>> handCommand = new NestedCommand<IPrivatePlayerView, ReadOnlyCollection<ICard>>((player, cmd) => player.Hand);
			handCommand.MakeCardCollectionCommand();
			command["hand"] = handCommand;
			NestedCommand<IPrivatePlayerView, ReadOnlyCollection<ICard>> tableCommand = new NestedCommand<IPrivatePlayerView, ReadOnlyCollection<ICard>>((player, cmd) => player.Table);
			tableCommand.MakeCardCollectionCommand();
			command["table"] = tableCommand;
			command["character"] = new FinalCommand<IPrivatePlayerView>((player, cmd) => { PrintLine(player.CharacterType); });
			command["role"] = new FinalCommand<IPrivatePlayerView>((player, cmd) => { PrintLine(player.IsAlive); });
			NestedCommand<IPrivatePlayerView, ReadOnlyCollection<ICard>> selectionCommand = new NestedCommand<IPrivatePlayerView, ReadOnlyCollection<ICard>>((player, cmd) => player.Selection);
			selectionCommand.MakeCardCollectionCommand();
			command["selection"] = selectionCommand;
		}
		public static void MakeGameCommand<In>(this NestedCommand<In, IGame> command)
		{
			NestedCommand<IGame, ReadOnlyCollection<IPublicPlayerView>> playersCommand = new NestedCommand<IGame, ReadOnlyCollection<IPublicPlayerView>>((game, cmd) => game.Players);
			playersCommand.MakePlayerCollectionCommand();
			command["players"] = playersCommand;
			NestedCommand<IGame, IPublicPlayerView> playerCommand = new NestedCommand<IGame, IPublicPlayerView>((game, cmd) =>
			{
				try
				{
					int id = int.Parse(cmd.Dequeue());
					return game.GetPublicPlayerView(id);
				}
				catch(FormatException)
				{
					ErrorLine("Bad number format!");
				}
				catch(InvalidIdException)
				{
					ErrorLine("Invalid player ID!");
				}
				return null;
			});
			playerCommand.MakePublicPlayerViewCommand();
			command["player"] = playerCommand;
			NestedCommand<IGame, ReadOnlyCollection<ICard>> selectionCommand = new NestedCommand<IGame, ReadOnlyCollection<ICard>>((game, cmd) => game.Selection);
			selectionCommand.MakeCardCollectionCommand();
			command["selection"] = selectionCommand;
			command["graveyardtop"] = new FinalCommand<IGame>((game, cmd) => { Print(game.GraveyardTop); });
			command["reqtype"] = new FinalCommand<IGame>((game, cmd) => { Print(game.RequestType); });
			command["current"] = new FinalCommand<IGame>((game, cmd) =>
			{
				PrintLine("Current player - #{0}:", game.CurrentPlayer.ID);
				Print(game.CurrentPlayer);
			});
			command["requested"] = new FinalCommand<IGame>((game, cmd) =>
			{
				PrintLine("Requested player - #{0}:", game.RequestedPlayer.ID);
				Print(game.RequestedPlayer);
			});
			command["causedby"] = new FinalCommand<IGame>((game, cmd) =>
			{
				PrintLine("Caused by player - #{0}:", game.CausedBy.ID);
				Print(game.CausedBy);
			});
		}

		public static void MakeSessionPlayerCommand<In>(this NestedCommand<In, IPlayer> command)
		{
			command["name"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.Name); });
			command["haspassword"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.HasPassword); });
			command["iscreator"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.IsCreator); });
			command["isai"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.IsAI); });
			command["haslistener"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.HasListener); });
			command["score"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.Score); });
			command["turnsplayed"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.TurnsPlayed); });
			command["wins"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.Victories); });
			command["winsassheriff"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.VictoriesAsSheriff); });
			command["winsasdeputy"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.VictoriesAsDeputy); });
			command["winsasoutlaw"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.VictoriesAsOutlaw); });
			command["winsasrenegade"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.VictoriesAsRenegade); });
		}
		public static void MakeSessionSpectatorCommand<In>(this NestedCommand<In, ISpectator> command)
		{
			command["name"] = new FinalCommand<ISpectator>((spectator, cmd) => { PrintLine(spectator.Name); });
		}
		public static void MakeSessionPlayerCollectionCommand<In>(this NestedCommand<In, ReadOnlyCollection<IPlayer>> command)
		{
			command[""] = new FinalCommand<ReadOnlyCollection<IPlayer>>((players, cmd) =>
			{
				foreach(IPlayer player in players)
				{
					PrintLine("Player #{0}:", player.ID);
					Print(player);
				}
			});
			command["count"] = new FinalCommand<ReadOnlyCollection<IPlayer>>((players, cmd) => { PrintLine(players.Count); });
		}
		public static void MakeSessionSpectatorCollectionCommand<In>(this NestedCommand<In, ReadOnlyCollection<ISpectator>> command)
		{
			command[""] = new FinalCommand<ReadOnlyCollection<ISpectator>>((spectators, cmd) =>
			{
				foreach(ISpectator spectator in spectators)
				{
					PrintLine("Player #{0}:", spectator.ID);
					Print(spectator);
				}
			});
			command["count"] = new FinalCommand<ReadOnlyCollection<ISpectator>>((spectators, cmd) => { PrintLine(spectators.Count); });
		}
		public static void MakeSessionCommand<In>(this NestedCommand<In, ISession> command)
		{
			command["name"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.Name); });
			command["description"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.Description); });
			command["state"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.State); });
			command["minplayers"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.MinPlayers); });
			command["maxplayers"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.MaxPlayers); });
			command["maxspectators"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.MaxSpectators); });
			command["hasplayerpassword"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.HasPlayerPassword); });
			command["hasspectatorpassword"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.HasSpectatorPassword); });
			command["creator"] = new FinalCommand<ISession>((session, cmd) =>
			{
				PrintLine("Creator - #{0}:", session.Creator.ID);
				Print(session.Creator);
			});
			command["dodgecity"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.DodgeCity); });
			command["highnoon"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.HighNoon); });
			command["fistfulofcards"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.FistfulOfCards); });
			command["wildwestshow"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.WildWestShow); });
			command["gamesplayed"] = new FinalCommand<ISession>((session, cmd) => { PrintLine(session.GamesPlayed); });
			NestedCommand<ISession, ReadOnlyCollection<IPlayer>> playersCommand = new NestedCommand<ISession, ReadOnlyCollection<IPlayer>>((session, cmd) => session.Players);
			playersCommand.MakeSessionPlayerCollectionCommand();
			command["players"] = playersCommand;
			NestedCommand<ISession, IPlayer> playerCommand = new NestedCommand<ISession, IPlayer>((session, cmd) =>
			{
				try
				{
					int id = int.Parse(cmd.Dequeue());
					return session.GetPlayer(id);
				}
				catch(FormatException)
				{
					ErrorLine("Bad number format!");
				}
				catch(InvalidIdException)
				{
					ErrorLine("Invalid player ID!");
				}
				return null;
			});
			playerCommand.MakeSessionPlayerCommand();
			command["player"] = playerCommand;
			NestedCommand<ISession, ReadOnlyCollection<ISpectator>> spectatorsCommand = new NestedCommand<ISession, ReadOnlyCollection<ISpectator>>((session, cmd) => session.Spectators);
			spectatorsCommand.MakeSessionSpectatorCollectionCommand();
			command["spectators"] = spectatorsCommand;
			NestedCommand<ISession, ISpectator> spectatorCommand = new NestedCommand<ISession, ISpectator>((session, cmd) =>
			{
				try
				{
					int id = int.Parse(cmd.Dequeue());
					return session.GetSpectator(id);
				}
				catch(FormatException)
				{
					ErrorLine("Bad number format!");
				}
				catch(InvalidIdException)
				{
					ErrorLine("Invalid spectator ID!");
				}
				return null;
			});
			spectatorCommand.MakeSessionSpectatorCommand();
			command["spectator"] = spectatorCommand;
		}

		public static void MakeSessionCollectionCommand<In>(this NestedCommand<In, ReadOnlyCollection<ISession>> command)
		{
			command[""] = new FinalCommand<ReadOnlyCollection<ISession>>((sessions, cmd) =>
			{
				foreach(ISession session in sessions)
				{
					PrintLine("Session #{0}:", session.ID);
					Print(session);
				}
			});
			command["count"] = new FinalCommand<ReadOnlyCollection<ISession>>((sessions, cmd) => { PrintLine(sessions.Count); });
		}
		public static void MakeServerCommand<In>(this NestedCommand<In, IServer> command)
		{
			command["name"] = new FinalCommand<IServer>((server, cmd) => { PrintLine(server.Name); });
			command["description"] = new FinalCommand<IServer>((server, cmd) => { PrintLine(server.Description); });
			command["interfaceversion"] = new FinalCommand<IServer>((server, cmd) => { PrintLine("{0}.{1}", server.InterfaceVersionMinor, server.InterfaceVersionMajor); });
			NestedCommand<IServer, ReadOnlyCollection<ISession>> sessionsCommand = new NestedCommand<IServer, ReadOnlyCollection<ISession>>((server, cmd) => server.Sessions);
			sessionsCommand.MakeSessionCollectionCommand();
			command["sessions"] = sessionsCommand;
			NestedCommand<IServer, ISession> sessionCommand = new NestedCommand<IServer, ISession>((server, cmd) =>
			{
				try
				{
					int id = int.Parse(cmd.Dequeue());
					return server.GetSession(id);
				}
				catch(FormatException)
				{
					ErrorLine("Bad number format!");
				}
				catch(InvalidIdException)
				{
					ErrorLine("Invalid session ID!");
				}
				return null;
			});
			command["session"] = sessionCommand;
			sessionsCommand.MakeSessionCollectionCommand();
			command["sessions"] = sessionsCommand;
		}

		public static void MakePlayerGameControlCommand<In>(this NestedCommand<In, IPlayerControl> command)
		{
			NestedCommand<IPlayerControl, IGame> gameCommand = new NestedCommand<IPlayerControl, IGame>((control, cmd) => control.Game);
			gameCommand.MakeGameCommand();
			command["game"] = gameCommand;
			NestedCommand<IPlayerControl, IPrivatePlayerView> playerCommand = new NestedCommand<IPlayerControl, IPrivatePlayerView>((control, cmd) => control.PrivatePlayerView);
			playerCommand.MakePrivatePlayerViewCommand();
			command["player"] = playerCommand;
			command["responddraw"] = new FinalCommand<IPlayerControl>((control, cmd) =>
			{
				try
				{
					control.RespondDraw();
					SuccessLine();
				}
				catch(GameException e)
				{
					ErrorLine("Response refused: " + e.GetType() + "!");
				}
			});
			command["respondcard"] = new FinalCommand<IPlayerControl>((control, cmd) =>
			{
				try
				{
					int id = int.Parse(cmd.Dequeue());
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
			});
			command["respondplayer"] = new FinalCommand<IPlayerControl>((control, cmd) =>
			{
				try
				{
					int id = int.Parse(cmd.Dequeue());
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
			});
			command["respondnoaction"] = new FinalCommand<IPlayerControl>((control, cmd) =>
			{
				try
				{
					control.RespondNoAction();
					SuccessLine();
				}
				catch(GameException e)
				{
					ErrorLine("Response refused: " + e.GetType() + "!");
				}
			});
			command["responduseability"] = new FinalCommand<IPlayerControl>((control, cmd) =>
			{
				try
				{
					control.RespondUseAbility();
					SuccessLine();
				}
				catch(GameException e)
				{
					ErrorLine("Response refused: " + e.GetType() + "!");
				}
			});
		}
		public static void MakeSpectatorGameControlCommand<In>(this NestedCommand<In, ISpectatorControl> command, OnDisconnected onDisconnected)
		{
			NestedCommand<ISpectatorControl, IGame> gameCommand = new NestedCommand<ISpectatorControl, IGame>((control, cmd) => control.Game);
			gameCommand.MakeGameCommand();
			command["game"] = gameCommand;
		}

		public delegate void OnDisconnected();
		public static void MakePlayerSessionControlCommand<In>(this NestedCommand<In, IPlayerSessionControl> command, OnDisconnected onDisconnected)
		{
			NestedCommand<IPlayerSessionControl, ISession> sessionCommand = new NestedCommand<IPlayerSessionControl, ISession>((sessionControl, cmd) => sessionControl.Session);
			sessionCommand.MakeSessionCommand();
			command["session"] = sessionCommand;
			NestedCommand<IPlayerSessionControl, IPlayer> playerCommand = new NestedCommand<IPlayerSessionControl, IPlayer>((sessionControl, cmd) => sessionControl.Player);
			playerCommand.MakeSessionPlayerCommand();
			command["player"] = playerCommand;
			command["chat"] = new FinalCommand<IPlayerSessionControl>((sessionControl, cmd) =>
			{
				Console.Write("Message: ");
				string message = ReadLine();
				sessionControl.SendChatMessage(message);
				SuccessLine("Message sent!");
			});
			command["disconnect"] = new FinalCommand<IPlayerSessionControl>((sessionControl, cmd) =>
			{
				sessionControl.Disconnect();
				onDisconnected();
				SuccessLine("Disconnected from the session!");
			});
			command["startgame"] = new FinalCommand<IPlayerSessionControl>((sessionControl, cmd) =>
			{
				try
				{
					sessionControl.StartGame();
					SuccessLine("Game started!");
				}
				catch(GameException e)
				{
					ErrorLine("Unable to start game: " + e.GetType() + "!");
				}
			});
			command["endsession"] = new FinalCommand<IPlayerSessionControl>((sessionControl, cmd) =>
			{
				try
				{
					sessionControl.EndSession();
					SuccessLine("Session ended!");
				}
				catch(GameException e)
				{
					ErrorLine("Can't end session: " + e.GetType() + "!");
				}
			});
		}
		public static void MakeSpectatorSessionControlCommand<In>(this NestedCommand<In, ISpectatorSessionControl> command, OnDisconnected onDisconnected)
		{
			NestedCommand<ISpectatorSessionControl, ISession> sessionCommand = new NestedCommand<ISpectatorSessionControl, ISession>((sessionControl, cmd) => sessionControl.Session);
			sessionCommand.MakeSessionCommand();
			command["session"] = sessionCommand;
			NestedCommand<ISpectatorSessionControl, ISpectator> spectatorCommand = new NestedCommand<ISpectatorSessionControl, ISpectator>((sessionControl, cmd) => sessionControl.Spectator);
			spectatorCommand.MakeSessionSpectatorCommand();
			command["spectator"] = spectatorCommand;
			command["chat"] = new FinalCommand<ISpectatorSessionControl>((sessionControl, cmd) =>
			{
				Console.Write("Message: ");
				string message = ReadLine();
				sessionControl.SendChatMessage(message);
				SuccessLine("Message sent!");
			});
			command["disconnect"] = new FinalCommand<ISpectatorSessionControl>((sessionControl, cmd) =>
			{
				sessionControl.Disconnect();
				onDisconnected();
				SuccessLine("Disconnected from the session!");
			});
		}
	}
}

