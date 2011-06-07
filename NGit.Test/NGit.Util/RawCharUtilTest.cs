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

namespace NGit.Util
{
	[NUnit.Framework.TestFixture]
	public class RawCharUtilTest
	{
		/// <summary>
		/// Test method for
		/// <see cref="RawCharUtil.IsWhitespace(byte)">RawCharUtil.IsWhitespace(byte)</see>
		/// .
		/// </summary>
		[NUnit.Framework.Test]
		public virtual void TestIsWhitespace()
		{
			for (byte c = unchecked((byte)(-128)); ((sbyte)c) < 127; c++)
			{
				switch (c)
				{
					case unchecked((byte)(byte)('\r')):
					case unchecked((byte)(byte)('\n')):
					case unchecked((byte)(byte)('\t')):
					case unchecked((byte)(byte)(' ')):
					{
						NUnit.Framework.Assert.IsTrue(RawCharUtil.IsWhitespace(c));
						break;
					}

					default:
					{
						NUnit.Framework.Assert.IsFalse(RawCharUtil.IsWhitespace(c));
						break;
					}
				}
			}
		}

		/// <summary>
		/// Test method for
		/// <see cref="RawCharUtil.TrimTrailingWhitespace(byte[], int, int)">RawCharUtil.TrimTrailingWhitespace(byte[], int, int)
		/// 	</see>
		/// .
		/// </summary>
		/// <exception cref="Sharpen.UnsupportedEncodingException">Sharpen.UnsupportedEncodingException
		/// 	</exception>
		[NUnit.Framework.Test]
		public virtual void TestTrimTrailingWhitespace()
		{
			NUnit.Framework.Assert.AreEqual(0, RawCharUtil.TrimTrailingWhitespace(Sharpen.Runtime.GetBytesForString
				(string.Empty, "US-ASCII"), 0, 0));
			NUnit.Framework.Assert.AreEqual(0, RawCharUtil.TrimTrailingWhitespace(Sharpen.Runtime.GetBytesForString
				(" ", "US-ASCII"), 0, 1));
			NUnit.Framework.Assert.AreEqual(1, RawCharUtil.TrimTrailingWhitespace(Sharpen.Runtime.GetBytesForString
				("a ", "US-ASCII"), 0, 2));
			NUnit.Framework.Assert.AreEqual(2, RawCharUtil.TrimTrailingWhitespace(Sharpen.Runtime.GetBytesForString
				(" a ", "US-ASCII"), 0, 3));
			NUnit.Framework.Assert.AreEqual(3, RawCharUtil.TrimTrailingWhitespace(Sharpen.Runtime.GetBytesForString
				("  a", "US-ASCII"), 0, 3));
			NUnit.Framework.Assert.AreEqual(6, RawCharUtil.TrimTrailingWhitespace(Sharpen.Runtime.GetBytesForString
				("  test   ", "US-ASCII"), 2, 9));
		}

		/// <summary>
		/// Test method for
		/// <see cref="RawCharUtil.TrimLeadingWhitespace(byte[], int, int)">RawCharUtil.TrimLeadingWhitespace(byte[], int, int)
		/// 	</see>
		/// .
		/// </summary>
		/// <exception cref="Sharpen.UnsupportedEncodingException">Sharpen.UnsupportedEncodingException
		/// 	</exception>
		[NUnit.Framework.Test]
		public virtual void TestTrimLeadingWhitespace()
		{
			NUnit.Framework.Assert.AreEqual(0, RawCharUtil.TrimLeadingWhitespace(Sharpen.Runtime.GetBytesForString
				(string.Empty, "US-ASCII"), 0, 0));
			NUnit.Framework.Assert.AreEqual(1, RawCharUtil.TrimLeadingWhitespace(Sharpen.Runtime.GetBytesForString
				(" ", "US-ASCII"), 0, 1));
			NUnit.Framework.Assert.AreEqual(0, RawCharUtil.TrimLeadingWhitespace(Sharpen.Runtime.GetBytesForString
				("a ", "US-ASCII"), 0, 2));
			NUnit.Framework.Assert.AreEqual(1, RawCharUtil.TrimLeadingWhitespace(Sharpen.Runtime.GetBytesForString
				(" a ", "US-ASCII"), 0, 3));
			NUnit.Framework.Assert.AreEqual(2, RawCharUtil.TrimLeadingWhitespace(Sharpen.Runtime.GetBytesForString
				("  a", "US-ASCII"), 0, 3));
			NUnit.Framework.Assert.AreEqual(2, RawCharUtil.TrimLeadingWhitespace(Sharpen.Runtime.GetBytesForString
				("  test   ", "US-ASCII"), 2, 9));
		}
	}
}
