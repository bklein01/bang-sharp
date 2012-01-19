// ServerUtils.cs
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
using System.Linq;
using System.Net;

namespace Bang.Server
{
	/// <summary>
	/// Conatins commonly used server-administration-related methods and constants.
	/// </summary>
	public static class ServerUtils
	{
		/// <summary>
		/// The major server interface version.
		/// </summary>
		public const int InterfaceVersionMajor = 2;
		/// <summary>
		/// The minor server interface version.
		/// </summary>
		public const int InterfaceVersionMinor = 0;

		/// <summary>
		/// Checks the server interface version compatibility for the specified server.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the server is compatible, otherwise <c>false</c>.
		/// </returns>
		/// <param name='server'>
		/// The <see cref="IServerBase"/> to check.
		/// </param>
		public static bool IsServerCompatible(IServerBase server)
		{
			if(server.ServerInterfaceVersionMajor != InterfaceVersionMajor)
				return false;
			if(server.ServerInterfaceVersionMinor < InterfaceVersionMinor)
				return false;
			return true;
		}

		public static Type[] ClientSharedTypes = new Type[]
		{
		};
		public static Type[] ServerSharedTypes = new Type[]
		{
			typeof(IServerBase),
			typeof(IServerAdmin),
			typeof(ISessionAdmin),
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
		/// The <see cref="Bang.Server.IServerBase"/> object from the server.
		/// </returns>
		public static IServerBase ConnectAdmin(string address, int port)
		{
			return Utils.Connect<IServerBase>("BangSharp.rem", address, port, Utils.ClientSharedTypes.Concat(ClientSharedTypes));
		}
		/// <summary>
		/// Starts serving the Bang# server administration service at the specified port.
		/// </summary>
		/// <param name="port">
		/// The port on which to listen.
		/// </param>
		/// <param name="bindTo">
		/// The adress to bind to (usually <c>System.Net.IPAddress.Any</c> or <c>System.Net.IPAddress.Loopback</c>).
		/// </param>
		/// <remarks>
		/// Note that you must also call <c>Bang.Utils.Serve&lt;T&gt;</c> to start serving both services.
		/// </remarks>
		public static void ServeAdmin(int port, IPAddress bindTo)
		{
			Utils.OpenChannel(port, Utils.ServerSharedTypes.Concat(ServerSharedTypes), bindTo);
		}
	}
}
