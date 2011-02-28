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

using System.Text;
using NGit;
using NGit.Diff;
using NGit.Merge;
using Sharpen;

namespace NGit.Merge
{
	[NUnit.Framework.TestFixture]
	public class MergeAlgorithmTest
	{
		internal MergeFormatter fmt = new MergeFormatter();

		/// <summary>
		/// Check for a conflict where the second text was changed similar to the
		/// first one, but the second texts modification covers one more line.
		/// </summary>
		/// <remarks>
		/// Check for a conflict where the second text was changed similar to the
		/// first one, but the second texts modification covers one more line.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoConflictingModifications()
		{
			NUnit.Framework.Assert.AreEqual(T("a<b=Z>Zdefghij"), Merge("abcdefghij", "abZdefghij"
				, "aZZdefghij"));
		}

		/// <summary>Test a case where we have three consecutive chunks.</summary>
		/// <remarks>
		/// Test a case where we have three consecutive chunks. The first text
		/// modifies all three chunks. The second text modifies the first and the
		/// last chunk. This should be reported as one conflicting region.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestOneAgainstTwoConflictingModifications()
		{
			NUnit.Framework.Assert.AreEqual(T("aZ<Z=c>Zefghij"), Merge("abcdefghij", "aZZZefghij"
				, "aZcZefghij"));
		}

		/// <summary>Test a merge where only the second text contains modifications.</summary>
		/// <remarks>
		/// Test a merge where only the second text contains modifications. Expect as
		/// merge result the second text.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestNoAgainstOneModification()
		{
			NUnit.Framework.Assert.AreEqual(T("aZcZefghij"), Merge("abcdefghij", "abcdefghij"
				, "aZcZefghij"));
		}

		/// <summary>Both texts contain modifications but not on the same chunks.</summary>
		/// <remarks>
		/// Both texts contain modifications but not on the same chunks. Expect a
		/// non-conflict merge result.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoNonConflictingModifications()
		{
			NUnit.Framework.Assert.AreEqual(T("YbZdefghij"), Merge("abcdefghij", "abZdefghij"
				, "Ybcdefghij"));
		}

		/// <summary>Merge two complicated modifications.</summary>
		/// <remarks>
		/// Merge two complicated modifications. The merge algorithm has to extend
		/// and combine conflicting regions to get to the expected merge result.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoComplicatedModifications()
		{
			NUnit.Framework.Assert.AreEqual(T("a<ZZZZfZhZj=bYdYYYYiY>"), Merge("abcdefghij", 
				"aZZZZfZhZj", "abYdYYYYiY"));
		}

		/// <summary>Test a conflicting region at the very start of the text.</summary>
		/// <remarks>Test a conflicting region at the very start of the text.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestConflictAtStart()
		{
			NUnit.Framework.Assert.AreEqual(T("<Z=Y>bcdefghij"), Merge("abcdefghij", "Zbcdefghij"
				, "Ybcdefghij"));
		}

		/// <summary>Test a conflicting region at the very end of the text.</summary>
		/// <remarks>Test a conflicting region at the very end of the text.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestConflictAtEnd()
		{
			NUnit.Framework.Assert.AreEqual(T("abcdefghi<Z=Y>"), Merge("abcdefghij", "abcdefghiZ"
				, "abcdefghiY"));
		}

		/// <summary>
		/// Check for a conflict where the second text was changed similar to the
		/// first one, but the second texts modification covers one more line.
		/// </summary>
		/// <remarks>
		/// Check for a conflict where the second text was changed similar to the
		/// first one, but the second texts modification covers one more line.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestSameModification()
		{
			NUnit.Framework.Assert.AreEqual(T("abZdefghij"), Merge("abcdefghij", "abZdefghij"
				, "abZdefghij"));
		}

		/// <summary>Check that a deleted vs.</summary>
		/// <remarks>
		/// Check that a deleted vs. a modified line shows up as conflict (see Bug
		/// 328551)
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteVsModify()
		{
			NUnit.Framework.Assert.AreEqual(T("ab<=Z>defghij"), Merge("abcdefghij", "abdefghij"
				, "abZdefghij"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestInsertVsModify()
		{
			NUnit.Framework.Assert.AreEqual(T("a<bZ=XY>"), Merge("ab", "abZ", "aXY"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAdjacentModifications()
		{
			NUnit.Framework.Assert.AreEqual(T("a<Zc=bY>d"), Merge("abcd", "aZcd", "abYd"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSeperateModifications()
		{
			NUnit.Framework.Assert.AreEqual(T("aZcYe"), Merge("abcde", "aZcde", "abcYe"));
		}

		/// <summary>
		/// Test merging two contents which do one similar modification and one
		/// insertion is only done by one side.
		/// </summary>
		/// <remarks>
		/// Test merging two contents which do one similar modification and one
		/// insertion is only done by one side. Between modification and insertion is
		/// a block which is common between the two contents and the common base
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoSimilarModsAndOneInsert()
		{
			NUnit.Framework.Assert.AreEqual(T("IAAJ"), Merge("iA", "IA", "IAAJ"));
			NUnit.Framework.Assert.AreEqual(T("aBcDde"), Merge("abcde", "aBcde", "aBcDde"));
			NUnit.Framework.Assert.AreEqual(T("IAJ"), Merge("iA", "IA", "IAJ"));
			NUnit.Framework.Assert.AreEqual(T("IAAAJ"), Merge("iA", "IA", "IAAAJ"));
			NUnit.Framework.Assert.AreEqual(T("IAAAJCAB"), Merge("iACAB", "IACAB", "IAAAJCAB"
				));
			NUnit.Framework.Assert.AreEqual(T("HIAAAJCAB"), Merge("HiACAB", "HIACAB", "HIAAAJCAB"
				));
			NUnit.Framework.Assert.AreEqual(T("AGADEFHIAAAJCAB"), Merge("AGADEFHiACAB", "AGADEFHIACAB"
				, "AGADEFHIAAAJCAB"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private string Merge(string commonBase, string ours, string theirs)
		{
			MergeResult<RawText> r = new MergeAlgorithm().Merge(RawTextComparator.DEFAULT, RT(commonBase
				), RT(ours), RT(theirs));
			ByteArrayOutputStream bo = new ByteArrayOutputStream(50);
			fmt.FormatMerge(bo, r, "B", "O", "T", Constants.CHARACTER_ENCODING);
			return Sharpen.Runtime.GetStringForBytes(bo.ToByteArray(), Constants.CHARACTER_ENCODING
				);
		}

		public static string T(string text)
		{
			StringBuilder r = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				switch (c)
				{
					case '<':
					{
						r.Append("<<<<<<< O\n");
						break;
					}

					case '=':
					{
						r.Append("=======\n");
						break;
					}

					case '>':
					{
						r.Append(">>>>>>> T\n");
						break;
					}

					default:
					{
						r.Append(c);
						r.Append('\n');
						break;
					}
				}
			}
			return r.ToString();
		}

		public static RawText RT(string text)
		{
			return new RawText(Constants.Encode(T(text)));
		}
	}
}
