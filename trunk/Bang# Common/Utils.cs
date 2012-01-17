// Utils.cs
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
/*
#define CONST_SEED
*/
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.TwoWayTcp;
using System.IO;
using System.Reflection;
using System.Net;
namespace Bang
{
	/// <summary>
	/// Contains commonly used methods and constants.
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// The major interface version.
		/// </summary>
		public const int InterfaceVersionMajor = 6;
		/// <summary>
		/// The minor interface version.
		/// </summary>
		public const int InterfaceVersionMinor = 1;

		/// <summary>
		/// Checks the interface version compatibility for the specified server.
		/// </summary>
		/// <param name="server">
		/// The <see cref="IServer"/> to check.
		/// </param>
		/// <returns>
		/// True if the server is compatible, otherwise false.
		/// </returns>
		public static bool IsServerCompatible(IServer server)
		{
			if(server.InterfaceVersionMajor != InterfaceVersionMajor)
				return false;
			if(server.InterfaceVersionMinor < InterfaceVersionMinor)
				return false;
			return true;
		}

		/// <summary>
		/// The path to the folder where Bang# applications should store configuration files.
		/// </summary>
		public static readonly string ConfigFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BangSharp");

#if CONST_SEED
		// For debugging purposes you can use constant seed if you need to reproduce certain situations.
		private const int Seed = 10;
#else
		private static readonly int Seed = Environment.TickCount;
#endif

		/// <summary>
		/// A shared instance of random number generator.
		/// </summary>
		public static readonly Random Random = new Random(Seed);

		private static readonly CardType[] MainCards = new CardType[]
		{
			CardType.Bang,
			CardType.Missed,
			CardType.Beer,
			CardType.Saloon,
			CardType.WellsFargo,
			CardType.Diligenza,
			CardType.GeneralStore,
			CardType.Panic,
			CardType.CatBalou,
			CardType.Indians,
			CardType.Duel,
			CardType.Gatling,
			CardType.Mustang,
			CardType.Appaloosa,
			CardType.Barrel,
			CardType.Dynamite,
			CardType.Jail,
			CardType.Volcanic,
			CardType.Schofield,
			CardType.Remington,
			CardType.Carabine,
			CardType.Winchester,
		};
		private static CardType[] DodgeCityCards = new CardType[]
		{
			CardType.Dodge,
			CardType.Punch,
			CardType.Springfield,
			CardType.Brawl,
			CardType.RagTime,
			CardType.Tequila,
			CardType.Whisky,
			CardType.Hideout,
			CardType.Silver,
			CardType.Sombrero,
			CardType.IronPlate,
			CardType.TenGallonHat,
			CardType.Bible,
			CardType.Canteen,
			CardType.Knife,
			CardType.Derringer,
			CardType.Howitzer,
			CardType.Pepperbox,
			CardType.BuffaloRifle,
			CardType.CanCan,
			CardType.Conestoga,
			CardType.PonyExpress,
		};
		public static List<CardType> GetCardTypes()
		{
			return GetCardTypes(null, false);
		}
		public static List<CardType> GetCardTypes(bool includeDefault)
		{
			return GetCardTypes(null, includeDefault);
		}
		public static List<CardType> GetCardTypes(ISession session)
		{
			return GetCardTypes(session, false);
		}
		public static List<CardType> GetCardTypes(ISession session, bool includeDefault)
		{
			int count = MainCards.Length;
			if(includeDefault)
				count++;

			if(session != null)
			{
				if(session.DodgeCity)
					count += DodgeCityCards.Length;
				//...
			}

			List<CardType> list = new List<CardType>(count);
			list.AddRange(MainCards);
			if(includeDefault)
				list.Add(CardType.Unknown);

			if(session != null)
			{
				if(session.DodgeCity)
					list.AddRange(DodgeCityCards);
				//...
			}
			return list;
		}

		private static readonly Role[] Roles = new Role[]
		{
			Role.Sheriff,
			Role.Deputy,
			Role.Outlaw,
			Role.Renegade,
		};
		public static List<Role> GetRoles()
		{
			return GetRoles(false);
		}
		public static List<Role> GetRoles(bool includeDefault)
		{
			int count = Roles.Length;
			if(includeDefault)
				count++;

			List<Role> list = new List<Role>(count);
			list.AddRange(Roles);

			if(includeDefault)
				list.Add(Role.Unknown);
			return list;
		}

		private static readonly CharacterType[] MainCharacters = new CharacterType[]
		{
			CharacterType.BartCassidy,
			CharacterType.BlackJack,
			CharacterType.CalamityJanet,
			CharacterType.ElGringo,
			CharacterType.JesseJones,
			CharacterType.Jourdonnais,
			CharacterType.KitCarlson,
			CharacterType.LuckyDuke,
			CharacterType.PaulRegret,
			CharacterType.PedroRamirez,
			CharacterType.RoseDoolan,
			CharacterType.SidKetchum,
			CharacterType.SlabTheKiller,
			CharacterType.SuzyLafayette,
			CharacterType.VultureSam,
			CharacterType.WillyTheKid,
		};
		private static readonly CharacterType[] DodgeCityCharacters = new CharacterType[]
		{
			CharacterType.ApacheKid,
			CharacterType.BelleStar,
			CharacterType.BillNoface,
			CharacterType.ChuckWengam,
			CharacterType.DocHolyday,
			CharacterType.ElenaFuente,
			CharacterType.GregDigger,
			CharacterType.HerbHunter,
			CharacterType.JoseDelgado,
			CharacterType.MollyStark,
			CharacterType.PatBrennan,
			CharacterType.PixiePete,
			CharacterType.SeanMallory,
			CharacterType.TequilaJoe,
			CharacterType.VeraCuster,
			//CharacterType.,
		};
		public static List<CharacterType> GetCharacterTypes()
		{
			return GetCharacterTypes(null, false);
		}
		public static List<CharacterType> GetCharacterTypes(bool includeDefault)
		{
			return GetCharacterTypes(null, includeDefault);
		}
		public static List<CharacterType> GetCharacterTypes(ISession session)
		{
			return GetCharacterTypes(session, false);
		}
		public static List<CharacterType> GetCharacterTypes(ISession session, bool includeDefault)
		{
			int count = MainCharacters.Length;
			if(includeDefault)
				++count;

			if(session != null)
			{
				if(session.DodgeCity)
					count += DodgeCityCharacters.Length;
				//...
			}

			List<CharacterType> list = new List<CharacterType>(count);
			list.AddRange(MainCharacters);
			if(includeDefault)
				list.Add(CharacterType.Unknown);

			if(session != null)
			{
				if(session.DodgeCity)
					list.AddRange(DodgeCityCharacters);
				//...
			}
			return list;
		}

		/// <summary>
		/// Shuffles the list.
		/// </summary>
		/// <param name="list">
		/// The <see cref="List<T>"/> to be shuffled.
		/// </param>
		public static void Shuffle<T>(this List<T> list)
		{
			// Wanna more randomity? Just increase the repeat count :-)
			for(int c = 0; c < 10; c++)
			{
				for(int i = 0; i < list.Count; i++)
				{
					int k = Random.Next(list.Count);
					T temp = list[i];
					list[i] = list[k];
					list[k] = temp;
				}
			}
		}
		/// <summary>
		/// Generates a new id for the dictionary.
		/// </summary>
		/// <param name="dict">
		/// The <see cref="Dictionary<int, T>"/> for which to generate a new id.
		/// </param>
		/// <returns>
		/// The generated id.
		/// </returns>
		public static int GenerateID<T>(this Dictionary<int, T> dict)
		{
			int id = 1;
			// id 0 is reserved (like null)
			while(dict.ContainsKey(id))
				if(++id == 0)
					// This should never happen, but... you never know :)
					throw new OverflowException("Out of available IDs!");
			return id;
		}

		/// <summary>
		/// Gets a random element from the specified list.
		/// </summary>
		/// <param name="list">
		/// The <see cref="IList<T>"/> from which to get a random element.
		/// </param>
		/// <returns>
		/// A random element from the list.
		/// </returns>
		public static T GetRandom<T>(this IList<T> list)
		{
			if(list.Count == 0)
				throw new InvalidOperationException();
			return list[Random.Next(list.Count)];
		}

		private static IServerChannelSinkProvider GetServerProvider(IEnumerable<Type> allowedTypes)
		{
			MethodRestrictionServerSinkProvider restrictionProvider = new MethodRestrictionServerSinkProvider();
			restrictionProvider.Filter = m =>
			{
				Type decl = m.DeclaringType;
				if(decl.Equals(typeof(object)))
					return true;
				foreach(Type t in allowedTypes)
					if(t.Equals(decl))
						return true;
					else if(t.IsAssignableFrom(decl) && !decl.IsInterface)
						foreach(MethodInfo mi in decl.GetInterfaceMap(t).TargetMethods)
							if(mi.MethodHandle == m.MethodHandle)
								return true;
				return false;
			};
			BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
			serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
			serverProvider.Next = restrictionProvider;
			return serverProvider;
		}
		private static IClientChannelSinkProvider GetClientProvider()
		{
			return new BinaryClientFormatterSinkProvider();
		}

		public static T Connect<T>(string uri, string address, int port, IEnumerable<Type> allowedTypes)
		{
			ChannelServices.RegisterChannel(new TcpClientChannel("client", GetClientProvider(), GetServerProvider(allowedTypes)), false);
			return (T)RemotingServices.Connect(typeof(T), "tcp://" + address + ":" + port + "/" + uri);
		}
		public static void Serve<T>(string uri)
			where T : MarshalByRefObject, new()
		{
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), uri, WellKnownObjectMode.Singleton);
		}
		public static void OpenChannel(int port, IEnumerable<Type> allowedTypes, IPAddress bindTo)
		{
			Dictionary<string, object> properties = new Dictionary<string, object>();
			properties.Add("name", "server:" + port);
			properties.Add("port", port);
			properties.Add("bindTo", bindTo.ToString());
			TcpServerChannel channel = new TcpServerChannel(properties, GetClientProvider(), GetServerProvider(allowedTypes));
			ChannelServices.RegisterChannel(channel, false);
		}

		public static readonly Type[] ClientSharedTypes = new Type[]
		{
			typeof(IEventListener),
			typeof(IPlayerEventListener),
			typeof(ISpectatorEventListener),
		};
		public static readonly Type[] ServerSharedTypes = new Type[]
		{
			typeof(IServer),
			typeof(ISession),
			typeof(IPlayer),
			typeof(ISpectator),
			typeof(IPlayerSessionControl),
			typeof(ISpectatorSessionControl),
			typeof(IGame),
			typeof(IPublicPlayerView),
			typeof(IPrivatePlayerView),
			typeof(IPlayerControl),
			typeof(ISpectatorControl),
			typeof(ICard),
			typeof(IIdentificable),
		};

		/// <summary>
		/// Connects to the Bang# server with the specified address and port.
		/// </summary>
		/// <param name="address">
		/// The address of the server.
		/// </param>
		/// <param name="port">
		/// The port for the Bang# service.
		/// </param>
		/// <returns>
		/// The <see cref="Bang.IServer"/> object from the server.
		/// </returns>
		public static IServer Connect(string address, int port)
		{
			return Connect<IServer>("BangSharp.rem", address, port, ClientSharedTypes);
		}
		/// <summary>
		/// Starts serving the Bang# service at the specified port.
		/// </summary>
		/// <param name="port">
		/// The port on which to listen.
		/// </param>
		/// <typeparam name="T">
		/// The service type (must implement the <see cref="IServer"/> interface).
		/// </typeparam>
		public static void Serve<T>(int port)
			where T : MarshalByRefObject, IServer, new()
		{
			Serve<T>("BangSharp.rem");
			OpenChannel(port, ServerSharedTypes, IPAddress.Any);
		}
	}
}

