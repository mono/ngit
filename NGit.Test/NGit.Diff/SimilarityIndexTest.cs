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

using NGit;
using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	[NUnit.Framework.TestFixture]
	public class SimilarityIndexTest
	{
		/// <exception cref="NGit.Diff.SimilarityIndex.TableFullException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIndexingSmallObject()
		{
			SimilarityIndex si = Hash(string.Empty + "A\n" + "B\n" + "D\n" + "B\n");
			//
			//
			//
			//
			//
			int key_A = KeyFor("A\n");
			int key_B = KeyFor("B\n");
			int key_D = KeyFor("D\n");
			NUnit.Framework.Assert.IsTrue(key_A != key_B && key_A != key_D && key_B != key_D);
			NUnit.Framework.Assert.AreEqual(3, si.Size());
			NUnit.Framework.Assert.AreEqual(2, si.Count(si.FindIndex(key_A)));
			NUnit.Framework.Assert.AreEqual(4, si.Count(si.FindIndex(key_B)));
			NUnit.Framework.Assert.AreEqual(2, si.Count(si.FindIndex(key_D)));
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Diff.SimilarityIndex.TableFullException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIndexingLargeObject()
		{
			byte[] @in = Sharpen.Runtime.GetBytesForString((string.Empty + "A\n" + "B\n" + "B\n"
				 + "B\n"), "UTF-8");
			//
			//
			//
			//
			SimilarityIndex si = new SimilarityIndex();
			si.Hash(new ByteArrayInputStream(@in), @in.Length);
			NUnit.Framework.Assert.AreEqual(2, si.Size());
		}

		/// <exception cref="NGit.Diff.SimilarityIndex.TableFullException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCommonScore_SameFiles()
		{
			string text = string.Empty + "A\n" + "B\n" + "D\n" + "B\n";
			//
			//
			//
			//
			SimilarityIndex src = Hash(text);
			SimilarityIndex dst = Hash(text);
			NUnit.Framework.Assert.AreEqual(8, src.Common(dst));
			NUnit.Framework.Assert.AreEqual(8, dst.Common(src));
			NUnit.Framework.Assert.AreEqual(100, src.Score(dst, 100));
			NUnit.Framework.Assert.AreEqual(100, dst.Score(src, 100));
		}

		/// <exception cref="NGit.Diff.SimilarityIndex.TableFullException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCommonScore_EmptyFiles()
		{
			SimilarityIndex src = Hash(string.Empty);
			SimilarityIndex dst = Hash(string.Empty);
			NUnit.Framework.Assert.AreEqual(0, src.Common(dst));
			NUnit.Framework.Assert.AreEqual(0, dst.Common(src));
		}

		/// <exception cref="NGit.Diff.SimilarityIndex.TableFullException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCommonScore_TotallyDifferentFiles()
		{
			SimilarityIndex src = Hash("A\n");
			SimilarityIndex dst = Hash("D\n");
			NUnit.Framework.Assert.AreEqual(0, src.Common(dst));
			NUnit.Framework.Assert.AreEqual(0, dst.Common(src));
		}

		/// <exception cref="NGit.Diff.SimilarityIndex.TableFullException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCommonScore_SimiliarBy75()
		{
			SimilarityIndex src = Hash("A\nB\nC\nD\n");
			SimilarityIndex dst = Hash("A\nB\nC\nQ\n");
			NUnit.Framework.Assert.AreEqual(6, src.Common(dst));
			NUnit.Framework.Assert.AreEqual(6, dst.Common(src));
			NUnit.Framework.Assert.AreEqual(75, src.Score(dst, 100));
			NUnit.Framework.Assert.AreEqual(75, dst.Score(src, 100));
		}

		/// <exception cref="NGit.Diff.SimilarityIndex.TableFullException"></exception>
		private static SimilarityIndex Hash(string text)
		{
			SimilarityIndex src = new _SimilarityIndex_135();
			byte[] raw = Constants.Encode(text);
			src.SetFileSize(raw.Length);
			src.Hash(raw, 0, raw.Length);
			src.Sort();
			return src;
		}

		private sealed class _SimilarityIndex_135 : SimilarityIndex
		{
			public _SimilarityIndex_135()
			{
			}

			/// <exception cref="NGit.Diff.SimilarityIndex.TableFullException"></exception>
			internal override void Hash(byte[] raw, int ptr, int end)
			{
				while (ptr < end)
				{
					int hash = raw[ptr] & unchecked((int)(0xff));
					int start = ptr;
					do
					{
						int c = raw[ptr++] & unchecked((int)(0xff));
						if (c == '\n')
						{
							break;
						}
					}
					while (ptr < end && ptr - start < 64);
					this.Add(hash, ptr - start);
				}
			}
		}

		/// <exception cref="NGit.Diff.SimilarityIndex.TableFullException"></exception>
		private static int KeyFor(string line)
		{
			SimilarityIndex si = Hash(line);
			NUnit.Framework.Assert.AreEqual(1, si.Size(), "single line scored");
			return si.Key(0);
		}
	}
}
