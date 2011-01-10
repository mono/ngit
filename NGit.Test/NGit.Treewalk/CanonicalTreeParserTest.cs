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
using NGit.Treewalk;
using NGit.Util;
using Sharpen;

namespace NGit.Treewalk
{
	[NUnit.Framework.TestFixture]
	public class CanonicalTreeParserTest
	{
		private readonly CanonicalTreeParser ctp = new CanonicalTreeParser();

		private readonly FileMode m644 = FileMode.REGULAR_FILE;

		private readonly FileMode mt = FileMode.TREE;

		private readonly ObjectId hash_a = ObjectId.FromString("6b9c715d21d5486e59083fb6071566aa6ecd4d42"
			);

		private readonly ObjectId hash_foo = ObjectId.FromString("a213e8e25bb2442326e86cbfb9ef56319f482869"
			);

		private readonly ObjectId hash_sometree = ObjectId.FromString("daf4bdb0d7bb24319810fe0e73aa317663448c93"
			);

		private byte[] tree1;

		private byte[] tree2;

		private byte[] tree3;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			tree1 = Mktree(Entry(m644, "a", hash_a));
			tree2 = Mktree(Entry(m644, "a", hash_a), Entry(m644, "foo", hash_foo));
			tree3 = Mktree(Entry(m644, "a", hash_a), Entry(mt, "b_sometree", hash_sometree), 
				Entry(m644, "foo", hash_foo));
		}

		/// <exception cref="System.Exception"></exception>
		private static byte[] Mktree(params byte[][] data)
		{
			ByteArrayOutputStream @out = new ByteArrayOutputStream();
			foreach (byte[] e in data)
			{
				@out.Write(e);
			}
			return @out.ToByteArray();
		}

		/// <exception cref="System.Exception"></exception>
		private static byte[] Entry(FileMode mode, string name, ObjectId id)
		{
			ByteArrayOutputStream @out = new ByteArrayOutputStream();
			mode.CopyTo(@out);
			@out.Write(' ');
			@out.Write(Constants.Encode(name));
			@out.Write(0);
			id.CopyRawTo(@out);
			return @out.ToByteArray();
		}

		private string Path()
		{
			return RawParseUtils.Decode(Constants.CHARSET, ctp.path, ctp.pathOffset, ctp.pathLen
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyTree_AtEOF()
		{
			ctp.Reset(new byte[0]);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOneEntry_Forward()
		{
			ctp.Reset(tree1);
			NUnit.Framework.Assert.IsTrue(ctp.First);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(m644.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual("a", Path());
			NUnit.Framework.Assert.AreEqual(hash_a, ctp.EntryObjectId);
			ctp.Next(1);
			NUnit.Framework.Assert.IsFalse(ctp.First);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoEntries_ForwardOneAtATime()
		{
			ctp.Reset(tree2);
			NUnit.Framework.Assert.IsTrue(ctp.First);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(m644.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual("a", Path());
			NUnit.Framework.Assert.AreEqual(hash_a, ctp.EntryObjectId);
			ctp.Next(1);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(m644.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual("foo", Path());
			NUnit.Framework.Assert.AreEqual(hash_foo, ctp.EntryObjectId);
			ctp.Next(1);
			NUnit.Framework.Assert.IsFalse(ctp.First);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOneEntry_Seek1IsEOF()
		{
			ctp.Reset(tree1);
			ctp.Next(1);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoEntries_Seek2IsEOF()
		{
			ctp.Reset(tree2);
			ctp.Next(2);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestThreeEntries_Seek3IsEOF()
		{
			ctp.Reset(tree3);
			ctp.Next(3);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestThreeEntries_Seek2()
		{
			ctp.Reset(tree3);
			ctp.Next(2);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(m644.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual("foo", Path());
			NUnit.Framework.Assert.AreEqual(hash_foo, ctp.EntryObjectId);
			ctp.Next(1);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOneEntry_Backwards()
		{
			ctp.Reset(tree1);
			ctp.Next(1);
			NUnit.Framework.Assert.IsFalse(ctp.First);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
			ctp.Back(1);
			NUnit.Framework.Assert.IsTrue(ctp.First);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(m644.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual("a", Path());
			NUnit.Framework.Assert.AreEqual(hash_a, ctp.EntryObjectId);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoEntries_BackwardsOneAtATime()
		{
			ctp.Reset(tree2);
			ctp.Next(2);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
			ctp.Back(1);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(m644.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual("foo", Path());
			NUnit.Framework.Assert.AreEqual(hash_foo, ctp.EntryObjectId);
			ctp.Back(1);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(m644.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual("a", Path());
			NUnit.Framework.Assert.AreEqual(hash_a, ctp.EntryObjectId);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoEntries_BackwardsTwo()
		{
			ctp.Reset(tree2);
			ctp.Next(2);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
			ctp.Back(2);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(m644.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual("a", Path());
			NUnit.Framework.Assert.AreEqual(hash_a, ctp.EntryObjectId);
			ctp.Next(1);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(m644.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual("foo", Path());
			NUnit.Framework.Assert.AreEqual(hash_foo, ctp.EntryObjectId);
			ctp.Next(1);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestThreeEntries_BackwardsTwo()
		{
			ctp.Reset(tree3);
			ctp.Next(3);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
			ctp.Back(2);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(mt.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual("b_sometree", Path());
			NUnit.Framework.Assert.AreEqual(hash_sometree, ctp.EntryObjectId);
			ctp.Next(1);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(m644.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual("foo", Path());
			NUnit.Framework.Assert.AreEqual(hash_foo, ctp.EntryObjectId);
			ctp.Next(1);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBackwards_ConfusingPathName()
		{
			string aVeryConfusingName = "confusing 644 entry 755 and others";
			ctp.Reset(Mktree(Entry(m644, "a", hash_a), Entry(mt, aVeryConfusingName, hash_sometree
				), Entry(m644, "foo", hash_foo)));
			ctp.Next(3);
			NUnit.Framework.Assert.IsTrue(ctp.Eof);
			ctp.Back(2);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(mt.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual(aVeryConfusingName, Path());
			NUnit.Framework.Assert.AreEqual(hash_sometree, ctp.EntryObjectId);
			ctp.Back(1);
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(m644.GetBits(), ctp.mode);
			NUnit.Framework.Assert.AreEqual("a", Path());
			NUnit.Framework.Assert.AreEqual(hash_a, ctp.EntryObjectId);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBackwords_Prebuilts1()
		{
			// What is interesting about this test is the ObjectId for the
			// "darwin-x86" path entry ends in an octal digit (37 == '7').
			// Thus when scanning backwards we could over scan and consume
			// part of the SHA-1, and miss the path terminator.
			//
			ObjectId common = ObjectId.FromString("af7bf97cb9bce3f60f1d651a0ef862e9447dd8bc");
			ObjectId darwinx86 = ObjectId.FromString("e927f7398240f78face99e1a738dac54ef738e37"
				);
			ObjectId linuxx86 = ObjectId.FromString("ac08dd97120c7cb7d06e98cd5b152011183baf21"
				);
			ObjectId windows = ObjectId.FromString("6c4c64c221a022bb973165192cca4812033479df"
				);
			ctp.Reset(Mktree(Entry(mt, "common", common), Entry(mt, "darwin-x86", darwinx86), 
				Entry(mt, "linux-x86", linuxx86), Entry(mt, "windows", windows)));
			ctp.Next(3);
			NUnit.Framework.Assert.AreEqual("windows", ctp.EntryPathString);
			NUnit.Framework.Assert.AreSame(mt, ctp.EntryFileMode);
			NUnit.Framework.Assert.AreEqual(windows, ctp.EntryObjectId);
			ctp.Back(1);
			NUnit.Framework.Assert.AreEqual("linux-x86", ctp.EntryPathString);
			NUnit.Framework.Assert.AreSame(mt, ctp.EntryFileMode);
			NUnit.Framework.Assert.AreEqual(linuxx86, ctp.EntryObjectId);
			ctp.Next(1);
			NUnit.Framework.Assert.AreEqual("windows", ctp.EntryPathString);
			NUnit.Framework.Assert.AreSame(mt, ctp.EntryFileMode);
			NUnit.Framework.Assert.AreEqual(windows, ctp.EntryObjectId);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBackwords_Prebuilts2()
		{
			// What is interesting about this test is the ObjectId for the
			// "darwin-x86" path entry ends in an octal digit (37 == '7').
			// Thus when scanning backwards we could over scan and consume
			// part of the SHA-1, and miss the path terminator.
			//
			ObjectId common = ObjectId.FromString("af7bf97cb9bce3f60f1d651a0ef862e9447dd8bc");
			ObjectId darwinx86 = ObjectId.FromString("0000000000000000000000000000000000000037"
				);
			ObjectId linuxx86 = ObjectId.FromString("ac08dd97120c7cb7d06e98cd5b152011183baf21"
				);
			ObjectId windows = ObjectId.FromString("6c4c64c221a022bb973165192cca4812033479df"
				);
			ctp.Reset(Mktree(Entry(mt, "common", common), Entry(mt, "darwin-x86", darwinx86), 
				Entry(mt, "linux-x86", linuxx86), Entry(mt, "windows", windows)));
			ctp.Next(3);
			NUnit.Framework.Assert.AreEqual("windows", ctp.EntryPathString);
			NUnit.Framework.Assert.AreSame(mt, ctp.EntryFileMode);
			NUnit.Framework.Assert.AreEqual(windows, ctp.EntryObjectId);
			ctp.Back(1);
			NUnit.Framework.Assert.AreEqual("linux-x86", ctp.EntryPathString);
			NUnit.Framework.Assert.AreSame(mt, ctp.EntryFileMode);
			NUnit.Framework.Assert.AreEqual(linuxx86, ctp.EntryObjectId);
			ctp.Next(1);
			NUnit.Framework.Assert.AreEqual("windows", ctp.EntryPathString);
			NUnit.Framework.Assert.AreSame(mt, ctp.EntryFileMode);
			NUnit.Framework.Assert.AreEqual(windows, ctp.EntryObjectId);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFreakingHugePathName()
		{
			int n = AbstractTreeIterator.DEFAULT_PATH_SIZE * 4;
			StringBuilder b = new StringBuilder(n);
			for (int i = 0; i < n; i++)
			{
				b.Append('q');
			}
			string name = b.ToString();
			ctp.Reset(Entry(m644, name, hash_a));
			NUnit.Framework.Assert.IsFalse(ctp.Eof);
			NUnit.Framework.Assert.AreEqual(name, RawParseUtils.Decode(Constants.CHARSET, ctp
				.path, ctp.pathOffset, ctp.pathLen));
		}
	}
}
