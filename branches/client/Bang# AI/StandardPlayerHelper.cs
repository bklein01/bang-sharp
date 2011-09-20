// StandardPlayerHelper.cs
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
using System.Linq;
namespace Bang.AI
{
	internal sealed class StandardPlayerHelper : PlayerHelper
	{
		private class PlayerEntry
		{
			private StandardPlayerHelper parent;
			private int id;

			private Dictionary<int, int> attacksIn;
			private Dictionary<int, int> attacksOut;
			private Dictionary<int, int> helpsIn;
			private Dictionary<int, int> helpsOut;

			private Dictionary<Role, int> points;

			public StandardPlayerHelper Parent
			{
				get { return parent; }
			}
			public IPublicPlayerView Player
			{
				get { return parent.Control.Game.GetPublicPlayerView(id); }
			}
			public int ID
			{
				get { return id; }
			}
			public Role EstimatedRole
			{
				get;
				set;
			}

			public PlayerEntry(StandardPlayerHelper parent, IPublicPlayerView player)
			{
				this.parent = parent;
				this.id = player.ID;
				int playerCount = parent.Game.Players.Count;
				attacksIn = new Dictionary<int, int>(playerCount);
				attacksOut = new Dictionary<int, int>(playerCount);
				helpsIn = new Dictionary<int, int>(playerCount);
				helpsOut = new Dictionary<int, int>(playerCount);
				foreach(IPublicPlayerView p in parent.Game.Players)
				{
					int id = p.ID;
					attacksIn.Add(id, 0);
					attacksOut.Add(id, 0);
					helpsIn.Add(id, 0);
					helpsOut.Add(id, 0);
				}
				points = new Dictionary<Role, int>(3);
				points[Role.Deputy] = 0;
				points[Role.Outlaw] = 0;
				points[Role.Renegade] = 0;
				EstimatedRole = player.Role;
			}

			public int GetPoints(Role role)
			{
				return points[role];
			}

			public void UpdatePoints()
			{
				points = new Dictionary<Role, int>(3);
				points[Role.Deputy] = 0;
				points[Role.Outlaw] = 0;
				points[Role.Renegade] = 0;
				
				foreach(PlayerEntry e in parent.entries.Values)
				{
					int aOut = attacksOut[e.id];
					int hOut = helpsOut[e.id];
					int aIn = attacksIn[e.id];
					int hIn = helpsIn[e.id];
					switch(e.EstimatedRole)
					{
					case Role.Sheriff:
						points[Role.Deputy] -= 2 * aOut;
						points[Role.Deputy] += 2 * hOut;
						points[Role.Deputy] -= aIn;
						points[Role.Deputy] += hIn;
						points[Role.Outlaw] += 2 * aOut;
						points[Role.Outlaw] -= 2 * hOut;
						points[Role.Outlaw] += aIn;
						points[Role.Outlaw] -= hIn;
						points[Role.Renegade] -= aOut;
						points[Role.Renegade] += hOut;
						points[Role.Renegade] += aIn;
						points[Role.Renegade] -= hIn;
						break;
					case Role.Deputy:
						points[Role.Deputy] -= aOut;
						points[Role.Deputy] += hOut;
						points[Role.Deputy] -= aIn;
						points[Role.Deputy] += hIn;
						points[Role.Outlaw] += aOut;
						points[Role.Outlaw] -= hOut;
						points[Role.Outlaw] += aIn;
						points[Role.Outlaw] -= hIn;
						points[Role.Renegade] += aOut;
						points[Role.Renegade] -= hOut;
						points[Role.Renegade] += aIn;
						points[Role.Renegade] -= hIn;
						break;
					case Role.Outlaw:
						points[Role.Deputy] += aOut;
						points[Role.Deputy] -= hOut;
						points[Role.Deputy] += aIn;
						points[Role.Deputy] -= hIn;
						points[Role.Outlaw] -= aOut;
						points[Role.Outlaw] += hOut;
						points[Role.Outlaw] -= aIn;
						points[Role.Outlaw] += hIn;
						points[Role.Renegade] += aOut;
						points[Role.Renegade] -= hOut;
						points[Role.Renegade] += aIn;
						points[Role.Renegade] -= hIn;
						break;
					case Role.Renegade:
						points[Role.Deputy] += aOut;
						points[Role.Deputy] -= hOut;
						points[Role.Deputy] += aIn;
						points[Role.Deputy] -= hIn;
						points[Role.Outlaw] += aOut;
						points[Role.Outlaw] -= hOut;
						points[Role.Outlaw] += aIn;
						points[Role.Outlaw] -= hIn;
						points[Role.Renegade] += aOut;
						points[Role.Renegade] -= hOut;
						points[Role.Renegade] += aIn;
						points[Role.Renegade] -= hIn;
						break;
					}
				}
			}

			public void AttackIn(IPublicPlayerView attacker)
			{
				attacksIn[attacker.ID]++;
			}
			public void AttackOut(IPublicPlayerView target)
			{
				attacksOut[target.ID]++;
			}
			public void HelpIn(IPublicPlayerView helper)
			{
				helpsIn[helper.ID]++;
			}
			public void HelpOut(IPublicPlayerView target)
			{
				helpsOut[target.ID]++;
			}
		}
		private Dictionary<int, PlayerEntry> entries;
		private List<Role> unknownRoles;

		public StandardPlayerHelper(IPlayerControl control) : base(control)
		{
			int playerCount = Game.Players.Count;
			switch(playerCount)
			{
			case 4:
				unknownRoles = new List<Role> { Role.Outlaw, Role.Outlaw, Role.Renegade };
				break;
			case 5:
				unknownRoles = new List<Role> { Role.Deputy, Role.Outlaw, Role.Outlaw, Role.Renegade };
				break;
			case 6:
				unknownRoles = new List<Role> { Role.Deputy, Role.Outlaw, Role.Outlaw, Role.Outlaw, Role.Renegade };
				break;
			case 7:
				unknownRoles = new List<Role> { Role.Deputy, Role.Deputy, Role.Outlaw, Role.Outlaw, Role.Outlaw, Role.Renegade };
				break;
			case 8:
				unknownRoles = new List<Role> { Role.Deputy, Role.Deputy, Role.Outlaw, Role.Outlaw, Role.Outlaw, Role.Renegade, Role.Renegade };
				break;
			default:
				throw new InvalidOperationException();
			}
			
			entries = new Dictionary<int, PlayerEntry>(playerCount);
			foreach(IPublicPlayerView p in Game.Players)
				entries.Add(p.ID, new PlayerEntry(this, p));
			IPrivatePlayerView thisPlayer = ThisPlayer;
			entries[thisPlayer.ID].EstimatedRole = thisPlayer.Role;
		}

		public void EstimateRoles()
		{
			List<PlayerEntry> incognitoPlayers = new List<PlayerEntry>(entries.Values.Where(entry => entry.Player.Role == Role.Unknown && entry.ID != ThisPlayer.ID));

			Dictionary<int, Dictionary<Role, int>> stats = new Dictionary<int, Dictionary<Role, int>>();
			foreach(PlayerEntry e in incognitoPlayers)
			{
				Dictionary<Role, int> iterations = new Dictionary<Role, int>();
				iterations[Role.Unknown] = 0;
				iterations[Role.Deputy] = 0;
				iterations[Role.Outlaw] = 0;
				iterations[Role.Renegade] = 0;
				stats[e.ID] = iterations;
			}
			Dictionary<int, Role> roleDistribution = new Dictionary<int, Role>();
			bool changed;
			int iteration = 0;
			//ConsoleUtils.DebugLine("AI: P#{0}: Starting estimation...", ThisPlayer.ID);
			do
			{
				//ConsoleUtils.DebugLine("AI: P#{0}: Iteration #{1}...", ThisPlayer.ID, iteration);
				foreach(PlayerEntry e in incognitoPlayers)
				{
					roleDistribution[e.ID] = Role.Unknown;
					e.UpdatePoints();
				}

				List<Role> remainingRoles = new List<Role>(unknownRoles);
				foreach(Role role in unknownRoles)
				{
					int maxPoints = 0;
					PlayerEntry best = null;
					foreach(PlayerEntry e in incognitoPlayers)
					{
						if(roleDistribution[e.ID] == role)
							continue;
						int points = e.GetPoints(role);
						if(points > maxPoints)
						{
							maxPoints = points;
							best = e;
						}
					}
					if(best != null)
					{
						roleDistribution[best.ID] = role;
						remainingRoles.Remove(role);
					}
				}
				if(remainingRoles.Count > 0)
				{
					Role first = remainingRoles.First();
					if(remainingRoles.All(r => r == first))
						foreach(PlayerEntry e in incognitoPlayers)
							if(e.EstimatedRole == Role.Unknown)
								roleDistribution[e.ID] = first;
				}

				changed = false;
				foreach(PlayerEntry e in incognitoPlayers)
				{
					Role newRole = roleDistribution[e.ID];
					stats[e.ID][newRole]++;
					//ConsoleUtils.DebugLine("AI: P#{0} - {1} => {2}", e.ID, e.EstimatedRole, newRole);
					if(e.EstimatedRole != newRole)
					{
						e.EstimatedRole = newRole;
						changed = true;
					}
				}
				if(iteration++ >= 15)
					break;
			}
			while(changed);
			if(changed)
				foreach(PlayerEntry e in incognitoPlayers)
				{
					//ConsoleUtils.DebugLine("Stats P#{0}:", e.ID);
					Dictionary<Role, int> iterations = stats[e.ID];
					//foreach(KeyValuePair<Role, int> it in iterations)
						//ConsoleUtils.DebugLine("{0}: {1}", it.Key, it.Value);
					int max = iterations.Values.Max();
					if(iterations.Values.Count(i => i == max) > 1)
						e.EstimatedRole = Role.Unknown;
				}
		}

		public override IEnumerable<IPublicPlayerView> Allies
		{
			get
			{
				int thisPlayerId = ThisPlayer.ID;
				List<IPublicPlayerView> allies = new List<IPublicPlayerView>(Game.Players.Count);
				switch(ThisPlayer.Role)
				{
				case Role.Sheriff:
					foreach(PlayerEntry entry in entries.Values)
					{
						IPublicPlayerView player = entry.Player;
						if(entry.EstimatedRole == Role.Deputy && player.IsAlive && entry.ID != thisPlayerId)
							allies.Add(player);
					}
					break;
				case Role.Deputy:
					foreach(PlayerEntry entry in entries.Values)
					{
						IPublicPlayerView player = entry.Player;
						if((entry.EstimatedRole == Role.Sheriff || entry.EstimatedRole == Role.Deputy) && player.IsAlive && entry.ID != thisPlayerId)
							allies.Add(player);
					}
					break;
				case Role.Outlaw:
					foreach(PlayerEntry entry in entries.Values)
					{
						IPublicPlayerView player = entry.Player;
						if(entry.EstimatedRole == Role.Outlaw && player.IsAlive && entry.ID != thisPlayerId)
							allies.Add(player);
					}
					break;
				case Role.Renegade:
					IPublicPlayerView sheriff = Game.Players.First(p => p.IsSheriff);
					if(sheriff.IsAlive)
					{
						foreach(PlayerEntry entry in entries.Values)
						{
							IPublicPlayerView player = entry.Player;
							if(entry.EstimatedRole == Role.Renegade && player.IsAlive && entry.ID != thisPlayerId)
								allies.Add(player);
						}
						if(Game.Players.Any(p => p.Role != Role.Sheriff && p.Role != Role.Renegade && p.IsAlive))
							allies.Add(sheriff);
					}
					break;
				}
				return new ReadOnlyCollection<IPublicPlayerView>(allies);
			}
		}
		public override IEnumerable<IPublicPlayerView> Enemies
		{
			get
			{
				int thisPlayerId = ThisPlayer.ID;
				List<IPublicPlayerView> enemies = new List<IPublicPlayerView>(Game.Players.Count);
				switch(ThisPlayer.Role)
				{
				case Role.Sheriff:
					foreach(PlayerEntry entry in entries.Values)
					{
						IPublicPlayerView player = entry.Player;
						if(entry.EstimatedRole == Role.Outlaw && player.IsAlive)
							enemies.Add(player);
					}
					if(enemies.Count == 0)
						foreach(PlayerEntry entry in entries.Values)
						{
							IPublicPlayerView player = entry.Player;
							if(entry.EstimatedRole != Role.Sheriff && player.IsAlive)
								enemies.Add(player);
						}
					break;
				case Role.Deputy:
					foreach(PlayerEntry entry in entries.Values)
					{
						IPublicPlayerView player = entry.Player;
						if(entry.EstimatedRole == Role.Outlaw && player.IsAlive)
							enemies.Add(player);
					}
					if(enemies.Count == 0)
						foreach(PlayerEntry entry in entries.Values)
						{
							IPublicPlayerView player = entry.Player;
							if(entry.EstimatedRole != Role.Sheriff && entry.EstimatedRole != Role.Deputy && player.IsAlive)
								enemies.Add(player);
						}
					break;
				case Role.Outlaw:
					foreach(PlayerEntry entry in entries.Values)
					{
						IPublicPlayerView player = entry.Player;
						if(entry.EstimatedRole == Role.Sheriff || entry.EstimatedRole == Role.Deputy && player.IsAlive)
							enemies.Add(player);
					}
					break;
				case Role.Renegade:
					IPublicPlayerView sheriff = Game.Players.First(p => p.IsSheriff);
					if(sheriff.IsAlive)
					{
						foreach(PlayerEntry entry in entries.Values)
						{
							IPublicPlayerView player = entry.Player;
							if(entry.EstimatedRole != Role.Sheriff && entry.EstimatedRole != Role.Renegade && player.IsAlive)
								enemies.Add(player);
						}
						if(enemies.Count == 0)
							enemies.Add(sheriff);
					}
					else
						foreach(PlayerEntry entry in entries.Values)
						{
							IPublicPlayerView player = entry.Player;
							if(entry.ID != thisPlayerId && player.IsAlive)
								enemies.Add(player);
						}
					break;
				}
				return new ReadOnlyCollection<IPublicPlayerView>(enemies);
			}
		}

		public override void RegisterAttack(IPublicPlayerView target, IPublicPlayerView attacker)
		{
			PlayerEntry targetEntry = entries[target.ID];
			targetEntry.AttackIn(attacker);
			PlayerEntry attackerEntry = entries[attacker.ID];
			attackerEntry.AttackOut(target);
			EstimateRoles();
		}
		public override void RegisterHelp(IPublicPlayerView target, IPublicPlayerView helper)
		{
			PlayerEntry targetEntry = entries[target.ID];
			targetEntry.HelpIn(helper);
			PlayerEntry helperEntry = entries[helper.ID];
			helperEntry.HelpOut(target);
			EstimateRoles();
		}

		public override void OnRoleRevealed(IPublicPlayerView player)
		{
			PlayerEntry entry = entries[player.ID];
			unknownRoles.Remove(entry.EstimatedRole = player.Role);
			EstimateRoles();
		}
	}
}

