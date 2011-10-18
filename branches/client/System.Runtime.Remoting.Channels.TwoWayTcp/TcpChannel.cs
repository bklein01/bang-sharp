// TcpChannel.cs
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
namespace System.Runtime.Remoting.Channels.TwoWayTcp
{
	internal static class TcpChannel
	{
		public static string ParseChannelUrl(string url, out string objectUri)
		{
			if(url == null)
				throw new ArgumentNullException("url");

			string host, port;
			return ParseTcpUrl(url, out host, out port, out objectUri);
		}
		public static string ParseTcpUrl(string url, out string host, out string port, out string objectUri)
		{
			// format: "tcp://host:port/path/to/object"
			objectUri = null;
			host = null;
			port = null;

			// url needs to be at least "tcp:"
			if (url.Length < 4 || url[3] != ':' ||
			    (url[0] != 'T' && url[0] != 't') ||
			    (url[1] != 'C' && url[1] != 'c') ||
			    (url[2] != 'P' && url[2] != 'p'))
				return null;
			
			// "tcp:" is acceptable
			if (url.Length == 4)
				return url;
			
			// must be of the form "tcp://"
			if (url.Length <= 5 || url[4] != '/' || url[5] != '/')
				return null;
			
			// "tcp://" is acceptable
			if (url.Length == 6)
				return url;
			
			int i;
			for (i = 6; i < url.Length; i++) {
				if (url[i] == ':' || url[i] == '/')
					break;
			}
			
			host = url.Substring (6, i - 6);
			
			if (i + 1 < url.Length && url[i] == ':') {
				int start = i + 1;
				
				for (i++; i < url.Length; i++) {
					if (url[i] == '/')
						break;
				}
				
				if (i > start)
					port = url.Substring (start, i - start);
			}
			
			if (i >= url.Length || url[i] != '/')
				return url;
			
			objectUri = url.Substring (i);
			
			return url.Substring (0, i);
		}
	}
}

