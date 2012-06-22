// BufferedOutputStream.cs
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
using System.IO;

namespace System.Runtime.Remoting.Channels.TwoWayTcp
{
	internal class BufferedOutputStream : Stream
	{
		private Stream stream;
		private byte[] buffer;
		private int bufferPos;

		public BufferedOutputStream(Stream stream, int bufferSize)
		{
			this.stream = stream;
			buffer = new byte[bufferSize];
			bufferPos = 0;
		}

		public override void Flush()
		{
			if(bufferPos != 0)
			{
				stream.Write(buffer, 0, bufferPos);
				bufferPos = 0;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
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
			while(count > 0)
			{
				if(bufferPos > 0 || count < this.buffer.Length - bufferPos)
				{
					int toCopy = Math.Min(this.buffer.Length - bufferPos, count);
					Buffer.BlockCopy(buffer, offset, this.buffer, bufferPos, toCopy);
					count -= toCopy;
					bufferPos += toCopy;
					offset += toCopy;
					if(bufferPos >= this.buffer.Length)
					{
						stream.Write(this.buffer, 0, this.buffer.Length);
						bufferPos = 0;
					}
				}
				else
				{
					int toCopy = this.buffer.Length;
					stream.Write(buffer, offset, toCopy);
					count -= toCopy;
					offset += toCopy;
				}
			}
		}

		public override bool CanRead
		{
			get { return false; }
		}
		public override bool CanSeek
		{
			get { return false; }
		}
		public override bool CanWrite
		{
			get { return true; }
		}

		public override long Length
		{
			get { throw new NotSupportedException(); }
		}
		public override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}
	}
}

