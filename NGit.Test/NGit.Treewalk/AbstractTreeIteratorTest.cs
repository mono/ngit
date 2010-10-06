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
using NGit.Treewalk;
using Sharpen;

namespace NGit.Treewalk
{
	[NUnit.Framework.TestFixture]
	public class AbstractTreeIteratorTest
	{
		private static string Prefix(string path)
		{
			int s = path.LastIndexOf('/');
			return s > 0 ? Sharpen.Runtime.Substring(path, 0, s) : string.Empty;
		}

		public class FakeTreeIterator : WorkingTreeIterator
		{
			public FakeTreeIterator(AbstractTreeIteratorTest _enclosing, string pathName, FileMode
				 fileMode) : base(AbstractTreeIteratorTest.Prefix(pathName), new WorkingTreeOptions
				(CoreConfig.AutoCRLF.FALSE))
			{
				this._enclosing = _enclosing;
				this.mode = fileMode.GetBits();
				int s = pathName.LastIndexOf('/');
				byte[] name = Constants.Encode(Sharpen.Runtime.Substring(pathName, s + 1));
				this.EnsurePathCapacity(this.pathOffset + name.Length, this.pathOffset);
				System.Array.Copy(name, 0, this.path, this.pathOffset, name.Length);
				this.pathLen = this.pathOffset + name.Length;
			}

			/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override AbstractTreeIterator CreateSubtreeIterator(ObjectReader reader)
			{
				return null;
			}

			private readonly AbstractTreeIteratorTest _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPathCompare()
		{
			NUnit.Framework.Assert.IsTrue(new AbstractTreeIteratorTest.FakeTreeIterator(this, 
				"a", FileMode.REGULAR_FILE).PathCompare(new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "a", FileMode.TREE)) < 0);
			NUnit.Framework.Assert.IsTrue(new AbstractTreeIteratorTest.FakeTreeIterator(this, 
				"a", FileMode.TREE).PathCompare(new AbstractTreeIteratorTest.FakeTreeIterator(this
				, "a", FileMode.REGULAR_FILE)) > 0);
			NUnit.Framework.Assert.IsTrue(new AbstractTreeIteratorTest.FakeTreeIterator(this, 
				"a", FileMode.REGULAR_FILE).PathCompare(new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "a", FileMode.REGULAR_FILE)) == 0);
			NUnit.Framework.Assert.IsTrue(new AbstractTreeIteratorTest.FakeTreeIterator(this, 
				"a", FileMode.TREE).PathCompare(new AbstractTreeIteratorTest.FakeTreeIterator(this
				, "a", FileMode.TREE)) == 0);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGrowPath()
		{
			AbstractTreeIteratorTest.FakeTreeIterator i = new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "ab", FileMode.TREE);
			byte[] origpath = i.path;
			NUnit.Framework.Assert.AreEqual(i.path[0], 'a');
			NUnit.Framework.Assert.AreEqual(i.path[1], 'b');
			i.GrowPath(2);
			NUnit.Framework.Assert.AreNotSame(origpath, i.path);
			NUnit.Framework.Assert.AreEqual(origpath.Length * 2, i.path.Length);
			NUnit.Framework.Assert.AreEqual(i.path[0], 'a');
			NUnit.Framework.Assert.AreEqual(i.path[1], 'b');
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEnsurePathCapacityFastCase()
		{
			AbstractTreeIteratorTest.FakeTreeIterator i = new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "ab", FileMode.TREE);
			int want = 50;
			byte[] origpath = i.path;
			NUnit.Framework.Assert.AreEqual(i.path[0], 'a');
			NUnit.Framework.Assert.AreEqual(i.path[1], 'b');
			NUnit.Framework.Assert.IsTrue(want < i.path.Length);
			i.EnsurePathCapacity(want, 2);
			NUnit.Framework.Assert.AreSame(origpath, i.path);
			NUnit.Framework.Assert.AreEqual(i.path[0], 'a');
			NUnit.Framework.Assert.AreEqual(i.path[1], 'b');
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEnsurePathCapacityGrows()
		{
			AbstractTreeIteratorTest.FakeTreeIterator i = new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "ab", FileMode.TREE);
			int want = 384;
			byte[] origpath = i.path;
			NUnit.Framework.Assert.AreEqual(i.path[0], 'a');
			NUnit.Framework.Assert.AreEqual(i.path[1], 'b');
			NUnit.Framework.Assert.IsTrue(i.path.Length < want);
			i.EnsurePathCapacity(want, 2);
			NUnit.Framework.Assert.AreNotSame(origpath, i.path);
			NUnit.Framework.Assert.AreEqual(512, i.path.Length);
			NUnit.Framework.Assert.AreEqual(i.path[0], 'a');
			NUnit.Framework.Assert.AreEqual(i.path[1], 'b');
		}

		[NUnit.Framework.Test]
		public virtual void TestEntryFileMode()
		{
			foreach (FileMode m in new FileMode[] { FileMode.TREE, FileMode.REGULAR_FILE, FileMode
				.EXECUTABLE_FILE, FileMode.GITLINK, FileMode.SYMLINK })
			{
				AbstractTreeIteratorTest.FakeTreeIterator i = new AbstractTreeIteratorTest.FakeTreeIterator
					(this, "a", m);
				NUnit.Framework.Assert.AreEqual(m.GetBits(), i.GetEntryRawMode());
				NUnit.Framework.Assert.AreSame(m, i.GetEntryFileMode());
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestEntryPath()
		{
			AbstractTreeIteratorTest.FakeTreeIterator i = new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "a/b/cd", FileMode.TREE);
			NUnit.Framework.Assert.AreEqual("a/b/cd", i.GetEntryPathString());
			NUnit.Framework.Assert.AreEqual(2, i.GetNameLength());
			byte[] b = new byte[3];
			b[0] = unchecked((int)(0x0a));
			i.GetName(b, 1);
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x0a)), b[0]);
			NUnit.Framework.Assert.AreEqual('c', b[1]);
			NUnit.Framework.Assert.AreEqual('d', b[2]);
		}

		[NUnit.Framework.Test]
		public virtual void TestCreateEmptyTreeIterator()
		{
			AbstractTreeIteratorTest.FakeTreeIterator i = new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "a/b/cd", FileMode.TREE);
			EmptyTreeIterator e = i.CreateEmptyTreeIterator();
			NUnit.Framework.Assert.IsNotNull(e);
			NUnit.Framework.Assert.AreEqual(i.GetEntryPathString() + "/", e.GetEntryPathString
				());
		}
	}
}
