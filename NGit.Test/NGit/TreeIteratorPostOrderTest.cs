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
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class TreeIteratorPostOrderTest : RepositoryTestCase
	{
		/// <summary>Empty tree</summary>
		[NUnit.Framework.Test]
		public virtual void TestEmpty()
		{
			Tree tree = new Tree(db);
			TreeIterator i = MakeIterator(tree);
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual(string.Empty, i.Next().GetFullName());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
		}

		/// <summary>one file</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestSimpleF1()
		{
			Tree tree = new Tree(db);
			tree.AddFile("x");
			TreeIterator i = MakeIterator(tree);
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("x", i.Next().GetName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual(string.Empty, i.Next().GetFullName());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
		}

		/// <summary>two files</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestSimpleF2()
		{
			Tree tree = new Tree(db);
			tree.AddFile("a");
			tree.AddFile("x");
			TreeIterator i = MakeIterator(tree);
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a", i.Next().GetName());
			NUnit.Framework.Assert.AreEqual("x", i.Next().GetName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual(string.Empty, i.Next().GetFullName());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
		}

		/// <summary>Empty tree</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestSimpleT()
		{
			Tree tree = new Tree(db);
			tree.AddTree("a");
			TreeIterator i = MakeIterator(tree);
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual(string.Empty, i.Next().GetFullName());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTricky()
		{
			Tree tree = new Tree(db);
			tree.AddFile("a.b");
			tree.AddFile("a.c");
			tree.AddFile("a/b.b/b");
			tree.AddFile("a/b");
			tree.AddFile("a/c");
			tree.AddFile("a=c");
			tree.AddFile("a=d");
			TreeIterator i = MakeIterator(tree);
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a.b", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a.c", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a/b", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a/b.b/b", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a/b.b", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a/c", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a=c", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a=d", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual(string.Empty, i.Next().GetFullName());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
		}

		private TreeIterator MakeIterator(Tree tree)
		{
			return new TreeIterator(tree, TreeIterator.Order.POSTORDER);
		}
	}
}
