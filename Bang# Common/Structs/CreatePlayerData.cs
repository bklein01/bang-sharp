// CreatePlayerData.cs
//  
// Author:  WOnder93 <omosnacek@gmail.com>
// 
// Copyright (c) 2011 Ondrej Mosnáček
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
namespace Bang
{
	/// <summary>
	/// A struct used to pass the information needed for creating a player from a client to a server.
	/// </summary>
	[Serializable]
	public struct CreatePlayerData
	{
		/// <summary>
		/// The name of the player.
		/// </summary>
		public string Name
		{
			get;
			set;
		}
		/// <summary>
		/// The avatar of the player.
		/// </summary>
		/// <remarks>
		/// Should contain image data encoded in the PNG format.
		/// Other popular formats (JPG, TIFF, ...) can also be used,
		/// but only PNG is guaranteed to be supported by clients.
		/// </remarks>
		public byte[] Image
		{
			get;
			set;
		}
		/// <summary>
		/// The password of the player.
		/// </summary>
		public Password Password
		{
			get;
			set;
		}
		
		/// <summary>
		/// Creates a new instance with the specified data.
		/// </summary>
		/// <param name="name">
		/// The name of the player.
		/// </param>
		/// <param name="image">
		/// The avatar of the player.
		/// </param>
		/// <param name="password">
		/// The password of the player.
		/// </param>
		public CreatePlayerData (string name, byte[] image, string password)
		{
			Name = name;
			Image = image;
			Password = new Password(password);
		}
	}
}

