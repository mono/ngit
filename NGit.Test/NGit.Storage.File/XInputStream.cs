/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	internal class XInputStream : BufferedInputStream
	{
		private readonly byte[] intbuf = new byte[8];

		protected XInputStream(InputStream s) : base(s)
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual byte[] ReadFully(int len)
		{
			lock (this)
			{
				byte[] b = new byte[len];
				ReadFully(b, 0, len);
				return b;
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual void ReadFully(byte[] b, int o, int len)
		{
			lock (this)
			{
				int r;
				while (len > 0 && (r = Read(b, o, len)) > 0)
				{
					o += r;
					len -= r;
				}
				if (len > 0)
				{
					throw new EOFException();
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual int ReadUInt8()
		{
			int r = Read();
			if (r < 0)
			{
				throw new EOFException();
			}
			return r;
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual long ReadUInt32()
		{
			ReadFully(intbuf, 0, 4);
			return NB.DecodeUInt32(intbuf, 0);
		}
	}
}
