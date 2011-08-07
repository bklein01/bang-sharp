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
using System.Runtime.Remoting.Channels.Tcp;
using System.IO;
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
		public const int InterfaceVersionMajor = 1;
		/// <summary>
		/// The minor interface version.
		/// </summary>
		public const int InterfaceVersionMinor = 0;

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

		private static TcpChannel CreateChannel(int port)
		{
			BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
			BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
			serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
			Dictionary<string, string> props = new Dictionary<string, string>() { { "port", port.ToString() } };
			return new TcpChannel(props, clientProvider, serverProvider);
		}

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
			int clientPort = 1000;
			while(true)
			{
				try
				{
					ChannelServices.RegisterChannel(CreateChannel(clientPort), false);
					break;
				}
				catch(System.Net.Sockets.SocketException)
				{
					if(++clientPort == 0)
						throw;
				}
				catch(RemotingException)
				{
					// channel already registered...
				}
			}
			return (IServer)RemotingServices.Connect(typeof(IServer), "tcp://" + address + ":" + port + "/BangSharp.rem");
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
		{
			ChannelServices.RegisterChannel(CreateChannel(port), false);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), "BangSharp.rem", WellKnownObjectMode.Singleton);
		}
	}
}

