// FinalCommand.cs
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
using System.Linq;

namespace Bang.ConsoleUtils
{
	/// <summary>
	/// The delegate for the final command template.
	/// </summary>
	/// <typeparam name='In'>
	/// The type of the input parameter of the command template.
	/// </typeparam>
	public delegate void FinalCommandDelegate<In>(In param, Queue<string> cmd);
	/// <summary>
	/// Represents a final command template.
	/// </summary>
	/// <typeparam name='In'>
	/// The type of the input parameter of the command template.
	/// </typeparam>
	public class FinalCommand<In> : Command<In>
	{
		private FinalCommandDelegate<In> del;

		public override IEnumerable<string> Subcommands
		{
			get { return Enumerable.Empty<string>(); }
		}

		/// <summary>
		/// Creates a new final command template with the specified delegate.
		/// </summary>
		/// <param name='del'>
		/// The delegate to be invoked when this command executes.
		/// </param>
		public FinalCommand(FinalCommandDelegate<In> del)
		{
			this.del = del;
		}

		public override ICommand GetSubcommand(string text)
		{
			return null;
		}
		public override void Execute(In param, Queue<string> cmd)
		{
			del(param, cmd);
		}
	}

	/// <summary>
	/// The delegate for the root final command template.
	/// </summary>
	public delegate void FinalCommandDelegate(Queue<string> cmd);
	/// <summary>
	/// Represents a root final command template.
	/// </summary>
	public class FinalCommand : FinalCommand<object>
	{
		/// <summary>
		/// Creates a new root final command template with the specified delegate.
		/// </summary>
		/// <param name='del'>
		/// The delegate to be invoked when this command executes.
		/// </param>
		public FinalCommand(FinalCommandDelegate del)
			: base((param, cmd) => del(cmd))
		{
		}
	}
}
