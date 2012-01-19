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

namespace Bang
{
	/// <summary>
	/// Represents a text password using a hash.
	/// </summary>
	[Serializable]
	public struct Password
	{
		private static readonly int[] hashBase = new int[] { 0x0ab629fe, 0x3684e89a };
		private int hash0;
		private int hash1;

		private static int[] CreateHash(string password)
		{
			if(string.IsNullOrEmpty(password))
				return new int[] { 0, 0 };

			int[] temp = hashBase.ToArray();
			for(int k = 0; k < password.Length; k++)
				for(int i = 0; i < hashBase.Length; i++)
					temp[i] ^= password[k] << (k + i);
			return temp;
		}

		public int[] Hash
		{
			get { return new int[] { hash0, hash1 }; }
			set
			{
				hash0 = value[0];
				hash1 = value[1];
			}
		}
		
		/// <summary>
		/// Gets a value indicating wheter the password is empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return hash0 == 0 && hash1 == 0; }
		}
		
		/// <summary>
		/// Creates a new password hash from the specified password.
		/// </summary>
		/// <param name="password">
		/// The password.
		/// </param>
		public Password(string password) : this()
		{
			Hash = CreateHash(password);
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
			if(hash.Length != hashBase.Length)
				throw new ArgumentOutOfRangeException("Hash length is incorrect!");
			Hash = hash;
		}

		/// <summary>
		/// Checks if the specified password matches the one prepresented by this hash.
		/// </summary>
		/// <param name="password">
		/// The password to be checked.
		/// </param>
		/// <returns>
		/// <c>true</c>. if the passwords match, otherwise <c>false</c>.
		/// </returns>
		public bool CheckPassword(string password)
		{
			return Hash.SequenceEqual(CreateHash(password));
		}
		/// <summary>
		/// Checks if the specified password hash matches this hash.
		/// </summary>
		/// <param name="password">
		/// The password hash to be checked.
		/// </param>
		/// <returns>
		/// <c>true</c>. if the password hashes match, otherwise <c>false</c>.
		/// </returns>
		public bool CheckPassword(Password password)
		{
			return Hash.SequenceEqual(password.Hash);
		}
	}
}
