// ServerUtils.cs
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
using System.Linq;
using System.Net;
namespace Bang.Server
{
	public static class ServerUtils
	{
		public const int InterfaceVersionMajor = 1;
		public const int InterfaceVersionMinor = 0;

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

		public static IServerBase ConnectAdmin(string address, int port)
		{
			return Utils.Connect<IServerBase>("BangSharpAdmin.rem", address, port, Utils.ClientSharedTypes.Concat(ClientSharedTypes));
		}
		public static void ServeAdmin<T>(int port, IPAddress bindTo)
			where T : MarshalByRefObject, new()
		{
			Utils.Serve<T>("BangSharpAdmin.rem", port, Utils.ServerSharedTypes.Concat(ServerSharedTypes), bindTo);
		}
	}
}
