// RemotingUtils.cs
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
using System.Net;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.TwoWayTcp;

namespace BangSharp
{
	public static class RemotingUtils
	{
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

		public static void OpenClientChannel(IEnumerable<Type> allowedTypes)
		{
			ChannelServices.RegisterChannel(new TcpClientChannel("client", GetClientProvider(), GetServerProvider(allowedTypes)), false);
		}
		public static T Connect<T>(string uri, string address, int port)
		{
			return (T)RemotingServices.Connect(typeof(T), "tcp://" + address + ":" + port + "/" + uri);
		}
		public static void Serve<T>(string uri)
			where T : MarshalByRefObject, new()
		{
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), uri, WellKnownObjectMode.Singleton);
		}
		public static void OpenServerChannel(int port, IEnumerable<Type> allowedTypes, IPAddress bindTo)
		{
			Dictionary<string, object> properties = new Dictionary<string, object>();
			properties.Add("name", "server:" + port);
			properties.Add("port", port);
			properties.Add("bindTo", bindTo.ToString());
			TcpServerChannel channel = new TcpServerChannel(properties, GetClientProvider(), GetServerProvider(allowedTypes));
			ChannelServices.RegisterChannel(channel, false);
		}
	}
}

