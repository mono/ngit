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
using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	public abstract class AbstractDiffTestCase
	{
		[NUnit.Framework.Test]
		public virtual void TestEmptyInputs()
		{
			EditList r = Diff(T(string.Empty), T(string.Empty));
			NUnit.Framework.Assert.IsTrue(r.IsEmpty(), "is empty");
		}

		[NUnit.Framework.Test]
		public virtual void TestCreateFile()
		{
			EditList r = Diff(T(string.Empty), T("AB"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 0, 0, 2), r[0]);
		}

		[NUnit.Framework.Test]
		public virtual void TestDeleteFile()
		{
			EditList r = Diff(T("AB"), T(string.Empty));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 2, 0, 0), r[0]);
		}

		[NUnit.Framework.Test]
		public virtual void TestDegenerate_InsertMiddle()
		{
			EditList r = Diff(T("ac"), T("aBc"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 1, 1, 2), r[0]);
		}

		[NUnit.Framework.Test]
		public virtual void TestDegenerate_DeleteMiddle()
		{
			EditList r = Diff(T("aBc"), T("ac"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 2, 1, 1), r[0]);
		}

		[NUnit.Framework.Test]
		public virtual void TestDegenerate_ReplaceMiddle()
		{
			EditList r = Diff(T("bCd"), T("bEd"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 2, 1, 2), r[0]);
		}

		[NUnit.Framework.Test]
		public virtual void TestDegenerate_InsertsIntoMidPosition()
		{
			EditList r = Diff(T("aaaa"), T("aaXaa"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(2, 2, 2, 3), r[0]);
		}

		[NUnit.Framework.Test]
		public virtual void TestDegenerate_InsertStart()
		{
			EditList r = Diff(T("bc"), T("Abc"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 0, 0, 1), r[0]);
		}

		[NUnit.Framework.Test]
		public virtual void TestDegenerate_DeleteStart()
		{
			EditList r = Diff(T("Abc"), T("bc"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 1, 0, 0), r[0]);
		}

		[NUnit.Framework.Test]
		public virtual void TestDegenerate_InsertEnd()
		{
			EditList r = Diff(T("bc"), T("bcD"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(2, 2, 2, 3), r[0]);
		}

		[NUnit.Framework.Test]
		public virtual void TestDegenerate_DeleteEnd()
		{
			EditList r = Diff(T("bcD"), T("bc"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(2, 3, 2, 2), r[0]);
		}

		[NUnit.Framework.Test]
		public virtual void TestEdit_ReplaceCommonDelete()
		{
			EditList r = Diff(T("RbC"), T("Sb"));
			NUnit.Framework.Assert.AreEqual(2, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 1, 0, 1), r[0]);
			NUnit.Framework.Assert.AreEqual(new Edit(2, 3, 2, 2), r[1]);
		}

		[NUnit.Framework.Test]
		public virtual void TestEdit_CommonReplaceCommonDeleteCommon()
		{
			EditList r = Diff(T("aRbCd"), T("aSbd"));
			NUnit.Framework.Assert.AreEqual(2, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 2, 1, 2), r[0]);
			NUnit.Framework.Assert.AreEqual(new Edit(3, 4, 3, 3), r[1]);
		}

		[NUnit.Framework.Test]
		public virtual void TestEdit_MoveBlock()
		{
			EditList r = Diff(T("aYYbcdz"), T("abcdYYz"));
			NUnit.Framework.Assert.AreEqual(2, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 3, 1, 1), r[0]);
			NUnit.Framework.Assert.AreEqual(new Edit(6, 6, 4, 6), r[1]);
		}

		[NUnit.Framework.Test]
		public virtual void TestEdit_InvertBlocks()
		{
			EditList r = Diff(T("aYYbcdXXz"), T("aXXbcdYYz"));
			NUnit.Framework.Assert.AreEqual(2, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 3, 1, 3), r[0]);
			NUnit.Framework.Assert.AreEqual(new Edit(6, 8, 6, 8), r[1]);
		}

		[NUnit.Framework.Test]
		public virtual void TestEdit_UniqueCommonLargerThanMatchPoint()
		{
			// We are testing 3 unique common matches, but two of
			// them are consumed as part of the 1st's LCS region.
			EditList r = Diff(T("AbdeZ"), T("PbdeQR"));
			NUnit.Framework.Assert.AreEqual(2, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 1, 0, 1), r[0]);
			NUnit.Framework.Assert.AreEqual(new Edit(4, 5, 4, 6), r[1]);
		}

		[NUnit.Framework.Test]
		public virtual void TestEdit_CommonGrowsPrefixAndSuffix()
		{
			// Here there is only one common unique point, but we can grow it
			// in both directions to find the LCS in the middle.
			EditList r = Diff(T("AaabccZ"), T("PaabccR"));
			NUnit.Framework.Assert.AreEqual(2, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 1, 0, 1), r[0]);
			NUnit.Framework.Assert.AreEqual(new Edit(6, 7, 6, 7), r[1]);
		}

		[NUnit.Framework.Test]
		public virtual void TestEdit_DuplicateAButCommonUniqueInB()
		{
			EditList r = Diff(T("AbbcR"), T("CbcS"));
			NUnit.Framework.Assert.AreEqual(2, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 2, 0, 1), r[0]);
			NUnit.Framework.Assert.AreEqual(new Edit(4, 5, 3, 4), r[1]);
		}

		[NUnit.Framework.Test]
		public virtual void TestEdit_InsertNearCommonTail()
		{
			EditList r = Diff(T("aq}nb"), T("aCq}nD}nb"));
			NUnit.Framework.Assert.AreEqual(new Edit(1, 1, 1, 2), r[0]);
			NUnit.Framework.Assert.AreEqual(new Edit(3, 3, 4, 7), r[1]);
			NUnit.Framework.Assert.AreEqual(2, r.Count);
		}

		public virtual EditList Diff(RawText a, RawText b)
		{
			return Algorithm().Diff(RawTextComparator.DEFAULT, a, b);
		}

		protected internal abstract DiffAlgorithm Algorithm();

		public static RawText T(string text)
		{
			StringBuilder r = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				r.Append(text[i]);
				r.Append('\n');
			}
			try
			{
				return new RawText(Sharpen.Runtime.GetBytesForString(r.ToString(), "UTF-8"));
			}
			catch (UnsupportedEncodingException e)
			{
				throw new RuntimeException(e);
			}
		}
	}
}
