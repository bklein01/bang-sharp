// TcpInputStream.cs
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
using System.IO;
using System.Threading;

namespace System.Runtime.Remoting.Channels.TwoWayTcp
{
	internal class TcpInputStream : Stream
	{
		private object recieveLock;
		private Stream baseStream;
		private int length;
		private int offset;

		public TcpInputStream(Stream baseStream, int length, object recieveLock)
		{
			this.baseStream = baseStream;
			this.length = length;
			this.offset = 0;
			this.recieveLock = recieveLock;
		}

		public override void Flush()
		{
		}

		private void Unlock()
		{
			lock(recieveLock)
				Monitor.Pulse(recieveLock);
		}

		public override int ReadByte()
		{
			if(this.offset >= this.length)
				throw new EndOfStreamException();
			int b = baseStream.ReadByte();
			this.offset++;
			if(this.offset >= this.length)
				Unlock();
			return b;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if(this.offset >= this.length)
				throw new EndOfStreamException();
			count = baseStream.Read(buffer, offset, Math.Min(count, this.length - this.offset));
			this.offset += count;
			if(this.offset >= this.length)
				Unlock();
			return count;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing && this.offset < this.length)
			{
				int remaining = this.length - this.offset;
				baseStream.Read(new byte[remaining], 0, remaining);
				this.offset = this.length;
				Unlock();
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override long Length
		{
			get { return length; }
		}

		public override long Position
		{
			get { return offset; }
			set { throw new NotSupportedException(); }
		}
	}
}

