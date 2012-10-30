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

using System.IO;
using NGit.Util;
using Sharpen;

namespace NGit.Util.IO
{
	/// <summary>An alternative PrintWriter that doesn't catch exceptions.</summary>
	/// <remarks>An alternative PrintWriter that doesn't catch exceptions.</remarks>
	public class ThrowingPrintWriter : TextWriter
	{
		private readonly TextWriter @out;

		private readonly string LF;

		/// <summary>Construct a JGitPrintWriter</summary>
		/// <param name="out">
		/// the underlying
		/// <see cref="System.IO.TextWriter">System.IO.TextWriter</see>
		/// </param>
		public ThrowingPrintWriter(TextWriter @out)
		{
			this.@out = @out;
			LF = AccessController.DoPrivileged(new _PrivilegedAction_69());
		}

		private sealed class _PrivilegedAction_69 : PrivilegedAction<string>
		{
			public _PrivilegedAction_69()
			{
			}

			public string Run()
			{
				return SystemReader.GetInstance().GetProperty("line.separator");
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Write(char[] cbuf, int off, int len)
		{
			@out.Write(cbuf, off, len);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Flush()
		{
			@out.Flush();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Close()
		{
			@out.Close();
		}

		/// <summary>Print a string and terminate with a line feed.</summary>
		/// <remarks>Print a string and terminate with a line feed.</remarks>
		/// <param name="s"></param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void Println(string s)
		{
			Print(s + LF);
		}

		/// <summary>Print a platform dependent new line</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void Println()
		{
			Print(LF);
		}

		/// <summary>Print a char</summary>
		/// <param name="value"></param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void Print(char value)
		{
			Print(value.ToString());
		}

		/// <summary>Print an int as string</summary>
		/// <param name="value"></param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void Print(int value)
		{
			Print(value.ToString());
		}

		/// <summary>Print a long as string</summary>
		/// <param name="value"></param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void Print(long value)
		{
			Print(value.ToString());
		}

		/// <summary>Print a short as string</summary>
		/// <param name="value"></param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void Print(short value)
		{
			Print(value.ToString());
		}

		/// <summary>
		/// Print a formatted message according to
		/// <see cref="string.Format(string, object[])">string.Format(string, object[])</see>
		/// .
		/// </summary>
		/// <param name="fmt"></param>
		/// <param name="args"></param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void Format(string fmt, params object[] args)
		{
			Print(string.Format(fmt, args));
		}

		/// <summary>Print an object's toString representations</summary>
		/// <param name="any"></param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void Print(object any)
		{
			@out.Write(any.ToString());
		}
	}
}
