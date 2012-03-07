// Password.cs
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
using System.Security.Cryptography;
using System.Text;

namespace BangSharp
{
	/// <summary>
	/// Represents a text password using a hash.
	/// </summary>
	[Serializable]
	public struct Password
	{
		private static readonly SHA256 sha = SHA256.Create();
		private byte[] hash;

		private static byte[] CreateHash(string password)
		{
			if(string.IsNullOrEmpty(password))
				return null;

			byte[] key = Encoding.UTF8.GetBytes(password);
			return sha.ComputeHash(key);
		}

		public int[] LongHash
		{
			get
			{
				if(hash == null)
					return new int[0];
				int[] res = new int[8];
				for(int i = 0, k = 0; i < 8; i++, k += 4)
				{
					res[i] |= hash[k];
					res[i] |= (int)hash[k + 1] << 8;
					res[i] |= (int)hash[k + 2] << 16;
					res[i] |= (int)hash[k + 3] << 24;
				}
				return res;
			}
			private set
			{
				if(value.Length == 0)
				{
					hash = null;
					return;
				}
				hash = new byte[32];
				for(int i = 0, k = 0; i < 8; i++, k += 4)
				{
					int n = value[i];
					hash[k] = (byte)(n & 0xff);
					n >>= 8;
					hash[k + 1] = (byte)(n & 0xff);
					n >>= 8;
					hash[k + 2] = (byte)(n & 0xff);
					n >>= 8;
					hash[k + 3] = (byte)(n & 0xff);
				}
			}
		}
		
		/// <summary>
		/// Gets a value indicating wheter the password is empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return hash == null; }
		}
		
		/// <summary>
		/// Creates a new password hash from the specified password.
		/// </summary>
		/// <param name="password">
		/// The password.
		/// </param>
		public Password(string password) : this()
		{
			hash = CreateHash(password);
		}
		/// <summary>
		/// Copies the password hash from an existing array.
		/// </summary>
		/// <param name="hash">
		/// The hash array.
		/// </param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// The length of the hash is incorrect.
		/// </exception>
		public Password(int[] hash) : this()
		{
			if(hash.Length != 8 && hash.Length != 0)
				throw new ArgumentOutOfRangeException("Hash length is incorrect!");
			LongHash = hash;
		}

		/// <summary>
		/// Checks if the specified password matches the one prepresented by this hash.
		/// </summary>
		/// <param name="password">
		/// The password to be checked.
		/// </param>
		/// <returns>
		/// <c>true</c> if the passwords match, otherwise <c>false</c>.
		/// </returns>
		public bool CheckPassword(string password)
		{
			if(IsEmpty)
				return string.IsNullOrEmpty(password);
			if(string.IsNullOrEmpty(password))
				return false;
			return hash.SequenceEqual(CreateHash(password));
		}
		/// <summary>
		/// Checks if the specified password hash matches this hash.
		/// </summary>
		/// <param name="password">
		/// The password hash to be checked.
		/// </param>
		/// <returns>
		/// <c>true</c> if the password hashes match, otherwise <c>false</c>.
		/// </returns>
		public bool CheckPassword(Password password)
		{
			if(IsEmpty)
				return password.IsEmpty;
			if(password.IsEmpty)
				return false;
			return hash.SequenceEqual(password.hash);
		}
	}
}
