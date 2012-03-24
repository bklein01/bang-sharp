// ConsoleHelper.cs
//  
// Author:  WOnder93 <omosnacek@gmail.com>
// 
// Copyright (c) 2012 Ondrej Mosnáček
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
using System.Linq;
using System.Text;

namespace BangSharp.ConsoleUtils
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

		private static readonly object colorLock = new object();
		private static readonly object readLock = new object();

		static ConsoleHelper()
		{
			Console.ForegroundColor = defaultFg;
			Console.BackgroundColor = defaultBg;
			Console.Clear();
		}

		/// <summary>
		/// Prints the main properties of an <see cref="BangSharp.IPublicPlayerView"/> to the console.
		/// </summary>
		/// <param name="player">
		/// The <see cref="BangSharp.IPublicPlayerView"/> to print.
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
		/// Prints the main properties of an <see cref="BangSharp.ICard"/> to the console.
		/// </summary>
		/// <param name="card">
		/// The <see cref="BangSharp.ICard"/> to print.
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
		/// <summary>
		/// Prints the main properties of an <see cref="BangSharp.ISpectator"/> to the console.
		/// </summary>
		/// <param name="spectator">
		/// The <see cref="BangSharp.ISpectator"/> to print.
		/// </param>
		public static void Print(ISpectator spectator)
		{
			if(spectator == null)
				return;
			PrintLine("\tName: {0}", spectator.Name);
		}
		/// <summary>
		/// Prints the main properties of an <see cref="BangSharp.IPlayer"/> to the console.
		/// </summary>
		/// <param name="player">
		/// The <see cref="BangSharp.IPlayer"/> to print.
		/// </param>
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
		/// <summary>
		/// Prints the main properties of an <see cref="BangSharp.ISession"/> to the console.
		/// </summary>
		/// <param name="session">
		/// The <see cref="BangSharp.ISession"/> to print.
		/// </param>
		public static void Print(ISession session)
		{
			if(session == null)
				return;
			PrintLine("\tName: {0}", session.Name);
			PrintLine("\tDescription: {0}", session.Description);
			PrintLine("\tState: {0}", session.State);
		}

		/// <summary>
		/// Reads a line from the console.
		/// </summary>
		/// <returns>
		/// The line.
		/// </returns>
		public static string ReadLine()
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = inputColor;
			}
			string res;
			lock(readLock)
				res = Console.ReadLine();
			lock(colorLock)
				Console.ForegroundColor = backup;
			return res;
		}
		/// <summary>
		/// Reads a password from the console.
		/// </summary>
		/// <returns>
		/// The password.
		/// </returns>
		public static Password ReadPassword()
		{
			StringBuilder line = new StringBuilder();
			ConsoleKeyInfo key;
			lock(readLock)
			{
				while((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
				{
					switch(key.Key)
					{
					case ConsoleKey.Escape:
						line = new StringBuilder();
						break;
					case ConsoleKey.Backspace:
						if(line.Length == 0)
							break;
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
			}
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
		private static void Clear(int length)
		{
			for(int i = 0; i < length; i++)
				Console.Write(' ');
		}
		private static string ReadCommandLine(ICommand command)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = inputColor;
			}
			StringBuilder line = new StringBuilder();
			int pos = 0;
			ConsoleKeyInfo key;
			string currentText = null;
			int historyIndex = -1;
			lock(readLock)
			{
				while((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
				{
					switch(key.Key)
					{
					case ConsoleKey.Escape:
						ConsolePos -= pos;
						Clear(line.Length);
						ConsolePos -= line.Length;
						line = new StringBuilder();
						pos = 0;
						break;
					case ConsoleKey.UpArrow:
						if(history.Count == 0)
							break;
						if(historyIndex < 0)
							historyIndex = history.Count;
						if(historyIndex - 1 < 0)
							break;
						
						historyIndex--;
						currentText = line.ToString();
						ConsolePos -= pos;
						Clear(line.Length);
						ConsolePos -= line.Length;
	
						string h = history[historyIndex];
						Console.Write(h);
						line = new StringBuilder(h);
						pos = h.Length;
						break;
					case ConsoleKey.DownArrow:
						if(historyIndex < 0)
							break;
						
						ConsolePos -= pos;
						Clear(line.Length);
						ConsolePos -= line.Length;
	
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
						ConsolePos += line.Length - pos;
						pos = line.Length;
						break;
					case ConsoleKey.Backspace:
						if(pos == 0)
							break;
						
						ConsolePos--;
						pos--;
						Clear(line.Length - pos);
						ConsolePos -= line.Length - pos;
	
						line = line.Remove(pos, 1);
						Console.Write(line.ToString(pos, line.Length - pos));
						if(pos < line.Length)
							ConsolePos -= line.Length - pos;
						break;
					case ConsoleKey.Delete:
						if(pos == line.Length)
							break;
						
						Clear(line.Length - pos);
						ConsolePos -= line.Length - pos;
	
						line = line.Remove(pos, 1);
						Console.Write(line.ToString(pos, line.Length - pos));
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
							Clear(line.Length - pos);
							if(pos < line.Length)
								ConsolePos -= line.Length - pos;
							line = line.Insert(pos, addition);
							Console.Write(line.ToString(pos, line.Length - pos));
							pos += addition.Length;
							if(pos < line.Length)
								ConsolePos -= line.Length - pos;
						}
						break;
					default:
						if(key.KeyChar == '\0')
							break;
						Clear(line.Length - pos);
						if(pos < line.Length)
							ConsolePos -= line.Length - pos;
						line = line.Insert(pos, key.KeyChar);
						Console.Write(line.ToString(pos, line.Length - pos));
						pos++;
						if(pos < line.Length)
							ConsolePos -= line.Length - pos;
						break;
					}
				}
				Console.WriteLine();
			}
			lock(colorLock)
				Console.ForegroundColor = backup;
			string txt = line.ToString();
			history.Add(txt);
			return txt;
		}
		/// <summary>
		/// Reads and executes a command for the nested command template.
		/// </summary>
		/// <param name='command'>
		/// The command template.
		/// </param>
		/// <typeparam name='Out'>
		/// The output type of the command template.
		/// </typeparam>
		public static void ReadAndExecute<Out>(this NestedCommand<Out> command)
		{
			command.ReadAndExecute<object>(new object());
		}
		/// <summary>
		/// Reads and executes a command for the command template.
		/// </summary>
		/// <param name='command'>
		/// The command template.
		/// </param>
		/// <param name='param'>
		/// The parameter to be passed to the command template.
		/// </param>
		/// <typeparam name='In'>
		/// The output type of the command template.
		/// </typeparam>
		public static void ReadAndExecute<In>(this Command<In> command, In param)
		{
			command.Execute(param, new Queue<string>(ReadCommandLine(command).ToLower().Split(' ').Where(s => s.Length > 0)));
		}

		/// <summary>
		/// Prints the specified object to the console.
		/// </summary>
		/// <param name='value'>
		/// The <see cref="object"/> to print.
		/// </param>
		public static void Print(object value)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = defaultFg;
			}
			Console.Write(value);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints the specified text string to the console.
		/// </summary>
		/// <param name='text'>
		/// The <see cref="string"/> to print.
		/// </param>
		public static void Print(string text)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = defaultFg;
			}
			Console.Write(text);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints the specified format string with arguments to the console.
		/// </summary>
		/// <param name='format'>
		/// The <see cref="string"/> to print.
		/// </param>
		/// <param name='args'>
		/// The arguments for the format string.
		/// </param>
		public static void Print(string format, params object[] args)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = defaultFg;
			}
			Console.Write(format, args);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a line to the console.
		/// </summary>
		public static void PrintLine()
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = defaultFg;
			}
			Console.WriteLine();
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a line with the specified object to the console.
		/// </summary>
		/// <param name='value'>
		/// The <see cref="object"/> to print.
		/// </param>
		public static void PrintLine(object value)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = defaultFg;
			}
			Console.WriteLine(value);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a line with the specified text string to the console.
		/// </summary>
		/// <param name='text'>
		/// The <see cref="string"/> to print.
		/// </param>
		public static void PrintLine(string text)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = defaultFg;
			}
			Console.WriteLine(text);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a line with the specified format string with arguments to the console.
		/// </summary>
		/// <param name='format'>
		/// The <see cref="string"/> to print.
		/// </param>
		/// <param name='args'>
		/// The arguments for the format string.
		/// </param>
		public static void PrintLine(string format, params object[] args)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = defaultFg;
			}
			Console.WriteLine(format, args);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a debug line with the specified object to the standard error stream.
		/// </summary>
		/// <param name='value'>
		/// The <see cref="object"/> to print.
		/// </param>
		public static void DebugLine(object value)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = debugColor;
			}
			Console.Error.Write("Debug: ");
			Console.Error.WriteLine(value);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a debug line with the specified text string to the standard error stream.
		/// </summary>
		/// <param name='text'>
		/// The <see cref="string"/> to print.
		/// </param>
		public static void DebugLine(string text)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = debugColor;
			}
			Console.Error.Write("Debug: ");
			Console.Error.WriteLine(text);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a line with the specified format string with arguments to the console.
		/// </summary>
		/// <param name='format'>
		/// The <see cref="string"/> to print.
		/// </param>
		/// <param name='args'>
		/// The arguments for the format string.
		/// </param>
		public static void DebugLine(string format, params object[] args)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = debugColor;
			}
			Console.Error.Write("Debug: ");
			Console.Error.WriteLine(format, args);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a success line (text 'Done!') to the console.
		/// </summary>
		public static void SuccessLine()
		{
			SuccessLine("Done!");
		}
		/// <summary>
		/// Prints a line with the specified text string to the console.
		/// </summary>
		/// <param name='text'>
		/// The <see cref="string"/> to print.
		/// </param>
		public static void SuccessLine(string text)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = successColor;
			}
			Console.WriteLine(text);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a line with the specified format string with arguments to the console.
		/// </summary>
		/// <param name='format'>
		/// The <see cref="string"/> to print.
		/// </param>
		/// <param name='args'>
		/// The arguments for the format string.
		/// </param>
		public static void SuccessLine(string format, params object[] args)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = successColor;
			}
			Console.WriteLine(format, args);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints an error line with the specified text string to the console.
		/// </summary>
		/// <param name='text'>
		/// The <see cref="string"/> to print.
		/// </param>
		public static void ErrorLine(string text)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = failColor;
			}
			Console.Write("ERROR: ");
			Console.WriteLine(text);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints an error line with the specified format string with arguments to the console.
		/// </summary>
		/// <param name='format'>
		/// The <see cref="string"/> to print.
		/// </param>
		/// <param name='args'>
		/// The arguments for the format string.
		/// </param>
		public static void ErrorLine(string format, params object[] args)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = failColor;
			}
			Console.Write("ERROR: ");
			Console.WriteLine(format, args);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a server event line with the specified text string to the console.
		/// </summary>
		/// <param name='text'>
		/// The <see cref="string"/> to print.
		/// </param>
		public static void ServerEvent(string message)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = eventColor;
			}
			Console.Write("Server event: ");
			Console.WriteLine(message);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a server event line with the specified format string with arguments to the console.
		/// </summary>
		/// <param name='format'>
		/// The <see cref="string"/> to print.
		/// </param>
		/// <param name='args'>
		/// The arguments for the format string.
		/// </param>
		public static void ServerEvent(string format, params object[] args)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = eventColor;
			}
			Console.Write("Server event: ");
			Console.WriteLine(format, args);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a session event line with the specified text string to the console.
		/// </summary>
		/// <param name='text'>
		/// The <see cref="string"/> to print.
		/// </param>
		public static void SessionEvent(string message)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = eventColor;
			}
			Console.Write("Session event: ");
			Console.WriteLine(message);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a session event line with the specified format string with arguments to the console.
		/// </summary>
		/// <param name='format'>
		/// The <see cref="string"/> to print.
		/// </param>
		/// <param name='args'>
		/// The arguments for the format string.
		/// </param>
		public static void SessionEvent(string format, params object[] args)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = eventColor;
			}
			Console.Write("Session event: ");
			Console.WriteLine(format, args);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a game event line with the specified text string to the console.
		/// </summary>
		/// <param name='text'>
		/// The <see cref="string"/> to print.
		/// </param>
		public static void GameEvent(string message)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = eventColor;
			}
			Console.Write("Game event: ");
			Console.WriteLine(message);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}
		/// <summary>
		/// Prints a game event line with the specified text string to the console.
		/// </summary>
		/// <param name='text'>
		/// The <see cref="string"/> to print.
		/// </param>
		public static void GameEvent(string format, params object[] args)
		{
			ConsoleColor backup;
			lock(colorLock)
			{
				backup = Console.ForegroundColor;
				Console.ForegroundColor = eventColor;
			}
			Console.Write("Game event: ");
			Console.WriteLine(format, args);
			lock(colorLock)
				Console.ForegroundColor = backup;
		}

		/// <summary>
		/// Makes the command template a card collection command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a card collection command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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
		/// <summary>
		/// Makes the command template a player collection command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a player collection command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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
		/// <summary>
		/// Makes the command template a public player view command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a public player view command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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
			command["additionalcharacters"] = new FinalCommand<IPublicPlayerView>((player, cmd) =>
			{
				foreach(CharacterType type in player.AdditionalCharacters)
					PrintLine(type);
			});
			command["role"] = new FinalCommand<IPublicPlayerView>((player, cmd) => { PrintLine(player.Role); });
		}
		/// <summary>
		/// Makes the command template a private player view command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a private player view command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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
			command["role"] = new FinalCommand<IPrivatePlayerView>((player, cmd) => { PrintLine(player.Role); });
			command["requesttype"] = new FinalCommand<IPrivatePlayerView>((player, cmd) => { PrintLine(player.RequestType); });
			NestedCommand<IPrivatePlayerView, ReadOnlyCollection<ICard>> selectionCommand = new NestedCommand<IPrivatePlayerView, ReadOnlyCollection<ICard>>((player, cmd) => player.Selection);
			selectionCommand.MakeCardCollectionCommand();
			command["selection"] = selectionCommand;
		}
		/// <summary>
		/// Makes the command template a game command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a game command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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
			command["current"] = new FinalCommand<IGame>((game, cmd) =>
			{
				IPublicPlayerView current = game.CurrentPlayer;
				if(current == null)
					PrintLine("No current player!");
				else
				{
					PrintLine("Current player - #{0}:", current.ID);
					Print(current);
				}
			});
			command["requested"] = new FinalCommand<IGame>((game, cmd) =>
			{
				IPublicPlayerView requested = game.RequestedPlayer;
				if(requested == null)
					PrintLine("No requested player!");
				else
				{
					PrintLine("Requested player - #{0}:", requested.ID);
					Print(requested);
				}
			});
			command["causedby"] = new FinalCommand<IGame>((game, cmd) =>
			{
				IPublicPlayerView causedBy = game.CausedBy;
				if(causedBy == null)
					PrintLine("No caused by player!");
				else
				{
					PrintLine("Caused by player - #{0}:", causedBy.ID);
					Print(causedBy);
				}
			});
		}

		private class RoleVictories
		{
			private Role role;

			public RoleVictories(Role role)
			{
				this.role = role;
			}

			public void Execute(IPlayer player, Queue<string> cmd)
			{
				PrintLine(player.GetVictories(role));
			}
		}
		private class CharacterVictories
		{
			private CharacterType character;

			public CharacterVictories(CharacterType character)
			{
				this.character = character;
			}

			public void Execute(IPlayer player, Queue<string> cmd)
			{
				PrintLine(player.GetVictories(character));
			}
		}
		/// <summary>
		/// Makes the command template a session player command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a session player command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
		public static void MakeSessionPlayerCommand<In>(this NestedCommand<In, IPlayer> command)
		{
			command["name"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.Name); });
			command["haspassword"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.HasPassword); });
			command["iscreator"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.IsCreator); });
			command["isai"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.IsAI); });
			command["haslistener"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.HasListener); });
			command["score"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.Score); });
			command["turnsplayed"] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.TurnsPlayed); });
			NestedCommand<IPlayer, IPlayer> victoriesCommand = new NestedCommand<IPlayer, IPlayer>((player, cmd) => player);
			victoriesCommand[""] = new FinalCommand<IPlayer>((player, cmd) => { PrintLine(player.Victories); });
			foreach(Role role in Utils.GetRoles())
				victoriesCommand["Role" + role.ToString()] = new FinalCommand<IPlayer>(new RoleVictories(role).Execute);
			foreach(CharacterType character in Utils.GetCharacterTypes())
				victoriesCommand["Character" + character.ToString()] = new FinalCommand<IPlayer>(new CharacterVictories(character).Execute);
			command["victories"] = victoriesCommand;
		}
		/// <summary>
		/// Makes the command template a session spectator command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a session spectator command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
		public static void MakeSessionSpectatorCommand<In>(this NestedCommand<In, ISpectator> command)
		{
			command["name"] = new FinalCommand<ISpectator>((spectator, cmd) => { PrintLine(spectator.Name); });
		}
		/// <summary>
		/// Makes the command template a session player collection command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a session player collection command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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
		/// <summary>
		/// Makes the command template a session spectator collection command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a session spectator collection command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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
		/// <summary>
		/// Makes the command template a session command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a session command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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

		/// <summary>
		/// Makes the command template a session collection command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a session collection command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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
		/// <summary>
		/// Makes the command template a server command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a server command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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
			sessionCommand.MakeSessionCommand();
			command["session"] = sessionCommand;
			sessionsCommand.MakeSessionCollectionCommand();
			command["sessions"] = sessionsCommand;
		}

		/// <summary>
		/// Makes the command template a player game control command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a player game control command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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
				}
				catch(GameException e)
				{
					ErrorLine("Response refused: " + e.GetType() + "!");
				}
			});
		}
		/// <summary>
		/// Makes the command template a spectator game control command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a spectator game control command.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
		public static void MakeSpectatorGameControlCommand<In>(this NestedCommand<In, ISpectatorControl> command, OnDisconnected onDisconnected)
		{
			NestedCommand<ISpectatorControl, IGame> gameCommand = new NestedCommand<ISpectatorControl, IGame>((control, cmd) => control.Game);
			gameCommand.MakeGameCommand();
			command["game"] = gameCommand;
		}

		/// <summary>
		/// Represents a delegate to be invoked when a session control is disconnected.
		/// </summary>
		public delegate void OnDisconnected();
		/// <summary>
		/// Makes the command template a player session control command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a player session control command.
		/// </param>
		/// <param name='onDisconnected'>
		/// The delegate to be invoked when the session control is disconnected.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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
		/// <summary>
		/// Makes the command template a spectator session control command.
		/// </summary>
		/// <param name='command'>
		/// The command template to make a spectator session control command.
		/// </param>
		/// <param name='onDisconnected'>
		/// The delegate to be invoked when the session control is disconnected.
		/// </param>
		/// <typeparam name='In'>
		/// The type of the input parameter of the command.
		/// </typeparam>
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
