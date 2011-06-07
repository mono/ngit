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
using NGit.Dircache;
using NGit.Treewalk;
using Sharpen;

namespace NGit.Treewalk
{
	[NUnit.Framework.TestFixture]
	public class PostOrderTreeWalkTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestInitialize_NoPostOrder()
		{
			TreeWalk tw = new TreeWalk(db);
			NUnit.Framework.Assert.IsFalse(tw.PostOrderTraversal);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestInitialize_TogglePostOrder()
		{
			TreeWalk tw = new TreeWalk(db);
			NUnit.Framework.Assert.IsFalse(tw.PostOrderTraversal);
			tw.PostOrderTraversal = true;
			NUnit.Framework.Assert.IsTrue(tw.PostOrderTraversal);
			tw.PostOrderTraversal = false;
			NUnit.Framework.Assert.IsFalse(tw.PostOrderTraversal);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestResetDoesNotAffectPostOrder()
		{
			TreeWalk tw = new TreeWalk(db);
			tw.PostOrderTraversal = true;
			NUnit.Framework.Assert.IsTrue(tw.PostOrderTraversal);
			tw.Reset();
			NUnit.Framework.Assert.IsTrue(tw.PostOrderTraversal);
			tw.PostOrderTraversal = false;
			NUnit.Framework.Assert.IsFalse(tw.PostOrderTraversal);
			tw.Reset();
			NUnit.Framework.Assert.IsFalse(tw.PostOrderTraversal);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNoPostOrder()
		{
			DirCache tree = db.ReadDirCache();
			{
				DirCacheBuilder b = tree.Builder();
				b.Add(MakeFile("a"));
				b.Add(MakeFile("b/c"));
				b.Add(MakeFile("b/d"));
				b.Add(MakeFile("q"));
				b.Finish();
				NUnit.Framework.Assert.AreEqual(4, tree.GetEntryCount());
			}
			TreeWalk tw = new TreeWalk(db);
			tw.PostOrderTraversal = false;
			tw.AddTree(new DirCacheIterator(tree));
			AssertModes("a", FileMode.REGULAR_FILE, tw);
			AssertModes("b", FileMode.TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsSubtree);
			NUnit.Framework.Assert.IsFalse(tw.IsPostChildren);
			tw.EnterSubtree();
			AssertModes("b/c", FileMode.REGULAR_FILE, tw);
			AssertModes("b/d", FileMode.REGULAR_FILE, tw);
			AssertModes("q", FileMode.REGULAR_FILE, tw);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWithPostOrder_EnterSubtree()
		{
			DirCache tree = db.ReadDirCache();
			{
				DirCacheBuilder b = tree.Builder();
				b.Add(MakeFile("a"));
				b.Add(MakeFile("b/c"));
				b.Add(MakeFile("b/d"));
				b.Add(MakeFile("q"));
				b.Finish();
				NUnit.Framework.Assert.AreEqual(4, tree.GetEntryCount());
			}
			TreeWalk tw = new TreeWalk(db);
			tw.PostOrderTraversal = true;
			tw.AddTree(new DirCacheIterator(tree));
			AssertModes("a", FileMode.REGULAR_FILE, tw);
			AssertModes("b", FileMode.TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsSubtree);
			NUnit.Framework.Assert.IsFalse(tw.IsPostChildren);
			tw.EnterSubtree();
			AssertModes("b/c", FileMode.REGULAR_FILE, tw);
			AssertModes("b/d", FileMode.REGULAR_FILE, tw);
			AssertModes("b", FileMode.TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsSubtree);
			NUnit.Framework.Assert.IsTrue(tw.IsPostChildren);
			AssertModes("q", FileMode.REGULAR_FILE, tw);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWithPostOrder_NoEnterSubtree()
		{
			DirCache tree = db.ReadDirCache();
			{
				DirCacheBuilder b = tree.Builder();
				b.Add(MakeFile("a"));
				b.Add(MakeFile("b/c"));
				b.Add(MakeFile("b/d"));
				b.Add(MakeFile("q"));
				b.Finish();
				NUnit.Framework.Assert.AreEqual(4, tree.GetEntryCount());
			}
			TreeWalk tw = new TreeWalk(db);
			tw.PostOrderTraversal = true;
			tw.AddTree(new DirCacheIterator(tree));
			AssertModes("a", FileMode.REGULAR_FILE, tw);
			AssertModes("b", FileMode.TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsSubtree);
			NUnit.Framework.Assert.IsFalse(tw.IsPostChildren);
			AssertModes("q", FileMode.REGULAR_FILE, tw);
		}

		/// <exception cref="System.Exception"></exception>
		private DirCacheEntry MakeFile(string path)
		{
			DirCacheEntry ent = new DirCacheEntry(path);
			ent.FileMode = FileMode.REGULAR_FILE;
			ent.SetObjectId(new ObjectInserter.Formatter().IdFor(Constants.OBJ_BLOB, Constants
				.Encode(path)));
			return ent;
		}

		/// <exception cref="System.Exception"></exception>
		private static void AssertModes(string path, FileMode mode0, TreeWalk tw)
		{
			NUnit.Framework.Assert.IsTrue(tw.Next(), "has " + path);
			NUnit.Framework.Assert.AreEqual(path, tw.PathString);
			NUnit.Framework.Assert.AreEqual(mode0, tw.GetFileMode(0));
		}
	}
}
