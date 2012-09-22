// MessageManager.cs
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
using Mono.Unix;

namespace BangSharp.Client
{
	public static class MessageManager
	{
		public static string GetErrorMessage(Exception e)
		{
			if(e is GameException)
				return GetErrorMessage(e as GameException);
			if(e is System.Runtime.Serialization.SerializationException)
				return Catalog.GetString("Serialization error!");
			if(e is System.Runtime.Remoting.RemotingException)
				return Catalog.GetString("Remoting error!");
			return string.Format(Catalog.GetString("An exception occured:\n{0}"), e.ToString());
		}
		public static string GetErrorMessage(GameException e)
		{
			if(e is BadServerPasswordException)
				return Catalog.GetString("Bad server administration password!");
			if(e is MaxPlayersOutOfRangeException)
				return Catalog.GetString("Maximum player count is out of range!");
			if(e is MinPlayersOutOfRangeException)
				return Catalog.GetString("Minimum player count is out of range!");
			if(e is MaxSpectatorsOutOfRangeException)
				return Catalog.GetString("Maximum spectator count is out of range!");
			if(e is InvalidIdException)
				return Catalog.GetString("Invalid identifier!");
			if(e is BadSessionStateException)
				return Catalog.GetString("Action is not valid for the current state of the session!");
			if(e is BadSessionPasswordException)
				return Catalog.GetString("Bad session password!");
			if(e is BadPlayerPasswordException)
				return Catalog.GetString("Bad player password!");
			if(e is TooManyPlayersException)
				return Catalog.GetString("Too many players!");
			if(e is TooManySpectatorsException)
				return Catalog.GetString("Too many spectators!");
			if(e is CannotReplacePlayerException)
				return Catalog.GetString("You can't replace this player!");
			if(e is MustBeCreatorException)
				return Catalog.GetString("You must be the creator of the session to perform this action!");
			if(e is BadGameStateException)
				return Catalog.GetString("Action is not valid for the current state of the game!");
			if(e is BadUsageException)
				return Catalog.GetString("This response is not valid!");
			if(e is BadPlayerException)
				return Catalog.GetString("Bad player!");
			if(e is BadCardException)
				return Catalog.GetString("Bad card!");
			if(e is BadCharacterTypeException)
				return Catalog.GetString("Bad character type!");
			if(e is BadTargetPlayerException)
				return Catalog.GetString("Bad target player!");
			if(e is BadTargetCardException)
				return Catalog.GetString("Bad target card!");
			if(e is CannotPlayBangException)
				return Catalog.GetString("You can play Bang only once in your turn!");
			if(e is CannotPlayBeerException)
				return Catalog.GetString("You can't play Beer when there are only two players left!");
			return Catalog.GetString("Unknown error!");
		}

		public static string GetRequestHint(RequestType reqType)
		{
			switch(reqType)
			{
			case RequestType.Draw:
				return Catalog.GetString("Choose the way you want to draw cards.\n\tRespondDraw: draw normally\n\tRespondUseAbility: draw using the character's ability (if any)");
			// TODO: Add more request hints!
			default:
				return "Unknown request.";
			}
		}

		public static string GetBooleanString(bool value)
		{
			if(value)
				return Catalog.GetString("Yes");
			else
				return Catalog.GetString("No");
		}

		public static string GetSessionStateString(SessionState state)
		{
			switch(state)
			{
			case SessionState.WaitingForPlayers:
				return Catalog.GetString("Waiting for players");
			case SessionState.Playing:
				return Catalog.GetString("Playing");
			case SessionState.GameFinished:
				return Catalog.GetString("Game finished");
			case SessionState.Ended:
				return Catalog.GetString("Ended");
			default:
				return "";
			}
		}
	}
}

