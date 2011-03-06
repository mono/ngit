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
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class IndexTreeWalkerTest : RepositoryTestCase
	{
		private AList<string> treeOnlyEntriesVisited = new AList<string>();

		private AList<string> bothVisited = new AList<string>();

		private AList<string> indexOnlyEntriesVisited = new AList<string>();

		private class TestIndexTreeVisitor : AbstractIndexTreeVisitor
		{
			public override void VisitEntry(TreeEntry treeEntry, GitIndex.Entry indexEntry, FilePath
				 file)
			{
				if (treeEntry == null)
				{
					this._enclosing.indexOnlyEntriesVisited.AddItem(indexEntry.GetName());
				}
				else
				{
					if (indexEntry == null)
					{
						this._enclosing.treeOnlyEntriesVisited.AddItem(treeEntry.GetFullName());
					}
					else
					{
						this._enclosing.bothVisited.AddItem(indexEntry.GetName());
					}
				}
			}

			internal TestIndexTreeVisitor(IndexTreeWalkerTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly IndexTreeWalkerTest _enclosing;
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTreeOnlyOneLevel()
		{
			treeOnlyEntriesVisited.Clear ();
			GitIndex index = new GitIndex(db);
			Tree tree = new Tree(db);
			tree.AddFile("foo");
			tree.AddFile("bar");
			new IndexTreeWalker(index, tree, trash, new IndexTreeWalkerTest.TestIndexTreeVisitor
				(this)).Walk();
			NUnit.Framework.Assert.IsTrue(treeOnlyEntriesVisited[0].Equals("bar"));
			NUnit.Framework.Assert.IsTrue(treeOnlyEntriesVisited[1].Equals("foo"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIndexOnlyOneLevel()
		{
			indexOnlyEntriesVisited.Clear ();
			GitIndex index = new GitIndex(db);
			Tree tree = new Tree(db);
			index.Add(trash, WriteTrashFile("foo", "foo"));
			index.Add(trash, WriteTrashFile("bar", "bar"));
			new IndexTreeWalker(index, tree, trash, new IndexTreeWalkerTest.TestIndexTreeVisitor
				(this)).Walk();
			NUnit.Framework.Assert.IsTrue(indexOnlyEntriesVisited[0].Equals("bar"));
			NUnit.Framework.Assert.IsTrue(indexOnlyEntriesVisited[1].Equals("foo"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBoth()
		{
			GitIndex index = new GitIndex(db);
			Tree tree = new Tree(db);
			index.Add(trash, WriteTrashFile("a", "a"));
			tree.AddFile("b/b");
			index.Add(trash, WriteTrashFile("c", "c"));
			tree.AddFile("c");
			new IndexTreeWalker(index, tree, trash, new IndexTreeWalkerTest.TestIndexTreeVisitor
				(this)).Walk();
			NUnit.Framework.Assert.IsTrue(indexOnlyEntriesVisited.Contains("a"));
			NUnit.Framework.Assert.IsTrue(treeOnlyEntriesVisited.Contains("b/b"));
			NUnit.Framework.Assert.IsTrue(bothVisited.Contains("c"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIndexOnlySubDirs()
		{
			indexOnlyEntriesVisited.Clear ();
			GitIndex index = new GitIndex(db);
			Tree tree = new Tree(db);
			index.Add(trash, WriteTrashFile("foo/bar/baz", "foobar"));
			index.Add(trash, WriteTrashFile("asdf", "asdf"));
			new IndexTreeWalker(index, tree, trash, new IndexTreeWalkerTest.TestIndexTreeVisitor
				(this)).Walk();
			NUnit.Framework.Assert.AreEqual("asdf", indexOnlyEntriesVisited[0]);
			NUnit.Framework.Assert.AreEqual("foo/bar/baz", indexOnlyEntriesVisited[1]);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLeavingTree()
		{
			GitIndex index = new GitIndex(db);
			index.Add(trash, WriteTrashFile("foo/bar", "foo/bar"));
			index.Add(trash, WriteTrashFile("foobar", "foobar"));
			new IndexTreeWalker(index, db.MapTree(index.WriteTree()), trash, new _AbstractIndexTreeVisitor_144
				()).Walk();
		}

		private sealed class _AbstractIndexTreeVisitor_144 : AbstractIndexTreeVisitor
		{
			public _AbstractIndexTreeVisitor_144()
			{
			}

			public override void VisitEntry(TreeEntry entry, GitIndex.Entry indexEntry, FilePath
				 f)
			{
				if (entry == null || indexEntry == null)
				{
					NUnit.Framework.Assert.Fail();
				}
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void FinishVisitTree(Tree tree, int i, string curDir)
			{
				if (tree.MemberCount() == 0)
				{
					NUnit.Framework.Assert.Fail();
				}
				if (i == 0)
				{
					NUnit.Framework.Assert.Fail();
				}
			}
		}
	}
}
