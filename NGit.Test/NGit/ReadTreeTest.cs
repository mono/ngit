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

using System;
using System.Collections.Generic;
using System.IO;
using NGit;
using NGit.Errors;
using NGit.Treewalk;
using Sharpen;

namespace NGit
{
	public abstract class ReadTreeTest : RepositoryTestCase
	{
		protected internal Tree theHead;

		protected internal Tree theMerge;

		// Each of these rules are from the read-tree manpage
		// go there to see what they mean.
		// Rule 0 is left out for obvious reasons :)
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRules1thru3_NoIndexEntry()
		{
			Tree head = new Tree(db);
			head = BuildTree(Mk("foo"));
			ObjectId objectId = head.FindBlobMember("foo").GetId();
			Tree merge = new Tree(db);
			PrescanTwoTrees(head, merge);
			NUnit.Framework.Assert.IsTrue(GetRemoved().Contains("foo"));
			PrescanTwoTrees(merge, head);
			AssertEquals(objectId, GetUpdated().Get("foo"));
			merge = BuildTree(Mkmap("foo", "a"));
			ObjectId anotherId = merge.FindBlobMember("foo").GetId();
			PrescanTwoTrees(head, merge);
			AssertEquals(anotherId, GetUpdated().Get("foo"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual void SetupCase(Dictionary<string, string> headEntries, Dictionary
			<string, string> mergeEntries, Dictionary<string, string> indexEntries)
		{
			theHead = BuildTree(headEntries);
			theMerge = BuildTree(mergeEntries);
			BuildIndex(indexEntries);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void BuildIndex(Dictionary<string, string> indexEntries)
		{
			GitIndex index = new GitIndex(db);
			if (indexEntries != null)
			{
				foreach (KeyValuePair<string, string> e in indexEntries.EntrySet())
				{
					index.Add(trash, WriteTrashFile(e.Key, e.Value)).ForceRecheck();
				}
			}
			index.Write();
			db.GetIndex().Read();
		}

		/// <exception cref="System.IO.IOException"></exception>
		private Tree BuildTree(Dictionary<string, string> headEntries)
		{
			Tree tree = new Tree(db);
			if (headEntries == null)
			{
				return tree;
			}
			FileTreeEntry fileEntry;
			Tree parent;
			ObjectInserter oi = db.NewObjectInserter();
			try
			{
				foreach (KeyValuePair<string, string> e in headEntries.EntrySet())
				{
					fileEntry = tree.AddFile(e.Key);
					fileEntry.SetId(GenSha1(e.Value));
					parent = fileEntry.GetParent();
					while (parent != null)
					{
						parent.SetId(oi.Insert(Constants.OBJ_TREE, parent.Format()));
						parent = parent.GetParent();
					}
				}
				oi.Flush();
			}
			finally
			{
				oi.Release();
			}
			return tree;
		}

		internal virtual ObjectId GenSha1(string data)
		{
			ObjectInserter w = db.NewObjectInserter();
			try
			{
				ObjectId id = w.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(data
					));
				w.Flush();
				return id;
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.Fail(e.ToString());
			}
			finally
			{
				w.Release();
			}
			return null;
		}

		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		protected internal virtual void Go()
		{
			PrescanTwoTrees(theHead, theMerge);
		}

		// for these rules, they all have clean yes/no options
		// but it doesn't matter if the entry is clean or not
		// so we can just ignore the state in the filesystem entirely
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRules4thru13_IndexEntryNotInHead()
		{
			// rules 4 and 5
			Dictionary<string, string> idxMap;
			idxMap = new Dictionary<string, string>();
			idxMap.Put("foo", "foo");
			SetupCase(null, null, idxMap);
			Go();
			NUnit.Framework.Assert.IsTrue(GetUpdated().IsEmpty());
			NUnit.Framework.Assert.IsTrue(GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(GetConflicts().IsEmpty());
			// rules 6 and 7
			idxMap = new Dictionary<string, string>();
			idxMap.Put("foo", "foo");
			SetupCase(null, idxMap, idxMap);
			Go();
			AssertAllEmpty();
			// rules 8 and 9
			Dictionary<string, string> mergeMap;
			mergeMap = new Dictionary<string, string>();
			mergeMap.Put("foo", "merge");
			SetupCase(null, mergeMap, idxMap);
			Go();
			NUnit.Framework.Assert.IsTrue(GetUpdated().IsEmpty());
			NUnit.Framework.Assert.IsTrue(GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(GetConflicts().Contains("foo"));
			// rule 10
			Dictionary<string, string> headMap = new Dictionary<string, string>();
			headMap.Put("foo", "foo");
			SetupCase(headMap, null, idxMap);
			Go();
			NUnit.Framework.Assert.IsTrue(GetRemoved().Contains("foo"));
			NUnit.Framework.Assert.IsTrue(GetUpdated().IsEmpty());
			NUnit.Framework.Assert.IsTrue(GetConflicts().IsEmpty());
			// rule 11
			SetupCase(headMap, null, idxMap);
			new FilePath(trash, "foo").Delete();
			WriteTrashFile("foo", "bar");
			db.GetIndex().GetMembers()[0].ForceRecheck();
			Go();
			NUnit.Framework.Assert.IsTrue(GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(GetUpdated().IsEmpty());
			NUnit.Framework.Assert.IsTrue(GetConflicts().Contains("foo"));
			// rule 12 & 13
			headMap.Put("foo", "head");
			SetupCase(headMap, null, idxMap);
			Go();
			NUnit.Framework.Assert.IsTrue(GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(GetUpdated().IsEmpty());
			NUnit.Framework.Assert.IsTrue(GetConflicts().Contains("foo"));
			// rules 14 & 15
			SetupCase(headMap, headMap, idxMap);
			Go();
			AssertAllEmpty();
			// rules 16 & 17
			SetupCase(headMap, mergeMap, idxMap);
			Go();
			NUnit.Framework.Assert.IsTrue(GetConflicts().Contains("foo"));
			// rules 18 & 19
			SetupCase(headMap, idxMap, idxMap);
			Go();
			AssertAllEmpty();
			// rule 20
			SetupCase(idxMap, mergeMap, idxMap);
			Go();
			NUnit.Framework.Assert.IsTrue(GetUpdated().ContainsKey("foo"));
			// rules 21
			SetupCase(idxMap, mergeMap, idxMap);
			new FilePath(trash, "foo").Delete();
			WriteTrashFile("foo", "bar");
			db.GetIndex().GetMembers()[0].ForceRecheck();
			Go();
			NUnit.Framework.Assert.IsTrue(GetConflicts().Contains("foo"));
		}

		private void AssertAllEmpty()
		{
			NUnit.Framework.Assert.IsTrue(GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(GetUpdated().IsEmpty());
			NUnit.Framework.Assert.IsTrue(GetConflicts().IsEmpty());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileSimple()
		{
			Tree treeDF = BuildTree(Mkmap("DF", "DF"));
			Tree treeDFDF = BuildTree(Mkmap("DF/DF", "DF/DF"));
			BuildIndex(Mkmap("DF", "DF"));
			PrescanTwoTrees(treeDF, treeDFDF);
			NUnit.Framework.Assert.IsTrue(GetRemoved().Contains("DF"));
			NUnit.Framework.Assert.IsTrue(GetUpdated().ContainsKey("DF/DF"));
			RecursiveDelete(new FilePath(trash, "DF"));
			BuildIndex(Mkmap("DF/DF", "DF/DF"));
			PrescanTwoTrees(treeDFDF, treeDF);
			NUnit.Framework.Assert.IsTrue(GetRemoved().Contains("DF/DF"));
			NUnit.Framework.Assert.IsTrue(GetUpdated().ContainsKey("DF"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_1()
		{
			// 1
			Doit(Mk("DF/DF"), Mk("DF"), Mk("DF/DF"));
			AssertNoConflicts();
			AssertUpdated("DF");
			AssertRemoved("DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_2()
		{
			// 2
			SetupCase(Mk("DF/DF"), Mk("DF"), Mk("DF/DF"));
			WriteTrashFile("DF/DF", "different");
			Go();
			AssertConflict("DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_3()
		{
			// 3 - the first to break!
			Doit(Mk("DF/DF"), Mk("DF/DF"), Mk("DF"));
			AssertUpdated("DF/DF");
			AssertRemoved("DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_4()
		{
			// 4 (basically same as 3, just with H and M different)
			Doit(Mk("DF/DF"), Mkmap("DF/DF", "foo"), Mk("DF"));
			AssertUpdated("DF/DF");
			AssertRemoved("DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_5()
		{
			// 5
			Doit(Mk("DF/DF"), Mk("DF"), Mk("DF"));
			AssertRemoved("DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_6()
		{
			// 6
			SetupCase(Mk("DF/DF"), Mk("DF"), Mk("DF"));
			WriteTrashFile("DF", "different");
			Go();
			AssertRemoved("DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_7()
		{
			// 7
			Doit(Mk("DF"), Mk("DF"), Mk("DF/DF"));
			AssertUpdated("DF");
			AssertRemoved("DF/DF");
			CleanUpDF();
			SetupCase(Mk("DF/DF"), Mk("DF/DF"), Mk("DF/DF/DF/DF/DF"));
			Go();
			AssertRemoved("DF/DF/DF/DF/DF");
			AssertUpdated("DF/DF");
			CleanUpDF();
			SetupCase(Mk("DF/DF"), Mk("DF/DF"), Mk("DF/DF/DF/DF/DF"));
			WriteTrashFile("DF/DF/DF/DF/DF", "diff");
			Go();
			AssertConflict("DF/DF/DF/DF/DF");
		}

		// assertUpdated("DF/DF");
		// Why do we expect an update on DF/DF. H==M,
		// H&M are files and index contains a dir, index
		// is dirty: that case is not in the table but
		// we cannot update DF/DF to a file, this would
		// require that we delete DF/DF/DF/DF/DF in workdir
		// throwing away unsaved contents.
		// This test would fail in DirCacheCheckoutTests.
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_8()
		{
			// 8
			SetupCase(Mk("DF"), Mk("DF"), Mk("DF/DF"));
			RecursiveDelete(new FilePath(db.WorkTree, "DF"));
			WriteTrashFile("DF", "xy");
			Go();
			AssertConflict("DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_9()
		{
			// 9
			Doit(Mk("DF"), Mkmap("DF", "QP"), Mk("DF/DF"));
			AssertRemoved("DF/DF");
			AssertUpdated("DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_10()
		{
			// 10
			CleanUpDF();
			Doit(Mk("DF"), Mk("DF/DF"), Mk("DF/DF"));
			AssertNoConflicts();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_11()
		{
			// 11
			Doit(Mk("DF"), Mk("DF/DF"), Mkmap("DF/DF", "asdf"));
			AssertConflict("DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_12()
		{
			// 12
			CleanUpDF();
			Doit(Mk("DF"), Mk("DF/DF"), Mk("DF"));
			AssertRemoved("DF");
			AssertUpdated("DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_13()
		{
			// 13
			CleanUpDF();
			SetupCase(Mk("DF"), Mk("DF/DF"), Mk("DF"));
			WriteTrashFile("DF", "asdfsdf");
			Go();
			AssertConflict("DF");
			AssertUpdated("DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_14()
		{
			// 14
			CleanUpDF();
			Doit(Mk("DF"), Mk("DF/DF"), Mkmap("DF", "Foo"));
			AssertConflict("DF");
			AssertUpdated("DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_15()
		{
			// 15
			Doit(Mkmap(), Mk("DF/DF"), Mk("DF"));
			// This test would fail in DirCacheCheckoutTests. I think this test is wrong,
			// it should check for conflicts according to rule 15
			// assertRemoved("DF");
			AssertUpdated("DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_15b()
		{
			// 15, take 2, just to check multi-leveled
			Doit(Mkmap(), Mk("DF/DF/DF/DF"), Mk("DF"));
			// I think this test is wrong, it should
			// check for conflicts according to rule 15
			// This test would fail in DirCacheCheckouts
			// assertRemoved("DF");
			AssertUpdated("DF/DF/DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_16()
		{
			// 16
			CleanUpDF();
			Doit(Mkmap(), Mk("DF"), Mk("DF/DF/DF"));
			AssertRemoved("DF/DF/DF");
			AssertUpdated("DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_17()
		{
			// 17
			CleanUpDF();
			SetupCase(Mkmap(), Mk("DF"), Mk("DF/DF/DF"));
			WriteTrashFile("DF/DF/DF", "asdf");
			Go();
			AssertConflict("DF/DF/DF");
		}

		// Why do we expect an update on DF. If we really update
		// DF and update also the working tree we would have to
		// overwrite a dirty file in the work-tree DF/DF/DF
		// This test would fail in DirCacheCheckout
		// assertUpdated("DF");
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_18()
		{
			// 18
			CleanUpDF();
			Doit(Mk("DF/DF"), Mk("DF/DF/DF/DF"), null);
			AssertRemoved("DF/DF");
			AssertUpdated("DF/DF/DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirectoryFileConflicts_19()
		{
			// 19
			CleanUpDF();
			Doit(Mk("DF/DF/DF/DF"), Mk("DF/DF/DF"), null);
			AssertRemoved("DF/DF/DF/DF");
			AssertUpdated("DF/DF/DF");
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual void CleanUpDF()
		{
			TearDown();
			SetUp();
			RecursiveDelete(new FilePath(trash, "DF"));
		}

		protected internal virtual void AssertConflict(string s)
		{
			NUnit.Framework.Assert.IsTrue(GetConflicts().Contains(s));
		}

		protected internal virtual void AssertUpdated(string s)
		{
			NUnit.Framework.Assert.IsTrue(GetUpdated().ContainsKey(s));
		}

		protected internal virtual void AssertRemoved(string s)
		{
			NUnit.Framework.Assert.IsTrue(GetRemoved().Contains(s));
		}

		protected internal virtual void AssertNoConflicts()
		{
			NUnit.Framework.Assert.IsTrue(GetConflicts().IsEmpty());
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal virtual void Doit(Dictionary<string, string> h, Dictionary<string
			, string> m, Dictionary<string, string> i)
		{
			SetupCase(h, m, i);
			Go();
		}

		protected internal static Dictionary<string, string> Mk(string a)
		{
			return Mkmap(a, a);
		}

		protected internal static Dictionary<string, string> Mkmap(params string[] args)
		{
			if ((args.Length % 2) > 0)
			{
				throw new ArgumentException("needs to be pairs");
			}
			Dictionary<string, string> map = new Dictionary<string, string>();
			for (int i = 0; i < args.Length; i += 2)
			{
				map.Put(args[i], args[i + 1]);
			}
			return map;
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUntrackedConflicts()
		{
			SetupCase(null, Mk("foo"), null);
			WriteTrashFile("foo", "foo");
			Go();
			// TODO: Why should we expect conflicts here?
			// H and M are emtpy and according to rule #5 of
			// the carry-over rules a dirty index is no reason
			// for a conflict. (I also feel it should be a
			// conflict because we are going to overwrite
			// unsaved content in the working tree
			// This test would fail in DirCacheCheckoutTest
			// assertConflict("foo");
			RecursiveDelete(new FilePath(trash, "foo"));
			SetupCase(null, Mk("foo"), null);
			WriteTrashFile("foo/bar/baz", string.Empty);
			WriteTrashFile("foo/blahblah", string.Empty);
			Go();
			// TODO: In DirCacheCheckout the following assertion would pass. But
			// old WorkDirCheckout fails on this. For now I leave it out. Find out
			// what's the correct behavior.
			// assertConflict("foo");
			AssertConflict("foo/bar/baz");
			AssertConflict("foo/blahblah");
			RecursiveDelete(new FilePath(trash, "foo"));
			SetupCase(Mkmap("foo/bar", string.Empty, "foo/baz", string.Empty), Mk("foo"), Mkmap
				("foo/bar", string.Empty, "foo/baz", string.Empty));
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "foo/bar").Exists());
			Go();
			AssertNoConflicts();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCloseNameConflictsX0()
		{
			SetupCase(Mkmap("a/a", "a/a-c"), Mkmap("a/a", "a/a", "b.b/b.b", "b.b/b.bs"), Mkmap
				("a/a", "a/a-c"));
			Checkout();
			AssertIndex(Mkmap("a/a", "a/a", "b.b/b.b", "b.b/b.bs"));
			AssertWorkDir(Mkmap("a/a", "a/a", "b.b/b.b", "b.b/b.bs"));
			Go();
			AssertIndex(Mkmap("a/a", "a/a", "b.b/b.b", "b.b/b.bs"));
			AssertWorkDir(Mkmap("a/a", "a/a", "b.b/b.b", "b.b/b.bs"));
			AssertNoConflicts();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCloseNameConflicts1()
		{
			SetupCase(Mkmap("a/a", "a/a-c"), Mkmap("a/a", "a/a", "a.a/a.a", "a.a/a.a"), Mkmap
				("a/a", "a/a-c"));
			Checkout();
			AssertIndex(Mkmap("a/a", "a/a", "a.a/a.a", "a.a/a.a"));
			AssertWorkDir(Mkmap("a/a", "a/a", "a.a/a.a", "a.a/a.a"));
			Go();
			AssertIndex(Mkmap("a/a", "a/a", "a.a/a.a", "a.a/a.a"));
			AssertWorkDir(Mkmap("a/a", "a/a", "a.a/a.a", "a.a/a.a"));
			AssertNoConflicts();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutHierarchy()
		{
			SetupCase(Mkmap("a", "a", "b/c", "b/c", "d", "d", "e/f", "e/f", "e/g", "e/g"), Mkmap
				("a", "a2", "b/c", "b/c", "d", "d", "e/f", "e/f", "e/g", "e/g2"), Mkmap("a", "a"
				, "b/c", "b/c", "d", "d", "e/f", "e/f", "e/g", "e/g3"));
			try
			{
				Checkout();
			}
			catch (CheckoutConflictException)
			{
				AssertWorkDir(Mkmap("a", "a", "b/c", "b/c", "d", "d", "e/f", "e/f", "e/g", "e/g3"
					));
				AssertConflict("e/g");
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutOutChanges()
		{
			SetupCase(Mk("foo"), Mk("foo/bar"), Mk("foo"));
			Checkout();
			AssertIndex(Mk("foo/bar"));
			AssertWorkDir(Mk("foo/bar"));
			NUnit.Framework.Assert.IsFalse(new FilePath(trash, "foo").IsFile());
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "foo/bar").IsFile());
			RecursiveDelete(new FilePath(trash, "foo"));
			AssertWorkDir(Mkmap());
			SetupCase(Mk("foo/bar"), Mk("foo"), Mk("foo/bar"));
			Checkout();
			AssertIndex(Mk("foo"));
			AssertWorkDir(Mk("foo"));
			NUnit.Framework.Assert.IsFalse(new FilePath(trash, "foo/bar").IsFile());
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "foo").IsFile());
			SetupCase(Mk("foo"), Mkmap("foo", "qux"), Mkmap("foo", "bar"));
			AssertIndex(Mkmap("foo", "bar"));
			AssertWorkDir(Mkmap("foo", "bar"));
			try
			{
				Checkout();
				NUnit.Framework.Assert.Fail("did not throw exception");
			}
			catch (CheckoutConflictException)
			{
				AssertIndex(Mkmap("foo", "bar"));
				AssertWorkDir(Mkmap("foo", "bar"));
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutUncachedChanges()
		{
			SetupCase(Mk("foo"), Mk("foo"), Mk("foo"));
			WriteTrashFile("foo", "otherData");
			Checkout();
			AssertIndex(Mk("foo"));
			AssertWorkDir(Mkmap("foo", "otherData"));
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "foo").IsFile());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDontOverwriteDirtyFile()
		{
			SetupCase(Mk("foo"), Mk("other"), Mk("foo"));
			WriteTrashFile("foo", "different");
			try
			{
				Checkout();
				NUnit.Framework.Assert.Fail("Didn't got the expected conflict");
			}
			catch (CheckoutConflictException)
			{
				AssertIndex(Mk("foo"));
				AssertWorkDir(Mkmap("foo", "different"));
				NUnit.Framework.Assert.IsTrue(GetConflicts().Equals(Arrays.AsList("foo")));
				NUnit.Framework.Assert.IsTrue(new FilePath(trash, "foo").IsFile());
			}
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void AssertWorkDir(Dictionary<string, string> i)
		{
			TreeWalk walk = new TreeWalk(db);
			walk.Reset();
			walk.Recursive = true;
			walk.AddTree(new FileTreeIterator(db));
			string expectedValue;
			string path;
			int nrFiles = 0;
			FileTreeIterator ft;
			while (walk.Next())
			{
				ft = walk.GetTree<FileTreeIterator>(0);
				path = ft.GetEntryPathString();
				expectedValue = i.Get(path);
				NUnit.Framework.Assert.IsNotNull(expectedValue, "found unexpected file for path "
					 + path + " in workdir");
				FilePath file = new FilePath(db.WorkTree, path);
				NUnit.Framework.Assert.IsTrue(file.Exists());
				if (file.IsFile())
				{
					FileInputStream @is = new FileInputStream(file);
					byte[] buffer = new byte[(int)file.Length()];
					int offset = 0;
					int numRead = 0;
					while (offset < buffer.Length && (numRead = @is.Read(buffer, offset, buffer.Length
						 - offset)) >= 0)
					{
						offset += numRead;
					}
					@is.Close();
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(buffer, Sharpen.Runtime.GetBytesForString
						(i.Get(path))), "unexpected content for path " + path + " in workDir. Expected: <"
						 + expectedValue + ">");
					nrFiles++;
				}
			}
			NUnit.Framework.Assert.AreEqual(i.Count, nrFiles, "WorkDir has not the right size."
				);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void AssertIndex(Dictionary<string, string> i)
		{
			string expectedValue;
			string path;
			GitIndex theIndex = db.GetIndex();
			// Without an explicit refresh we might miss index updates. If the index
			// is updated multiple times inside a FileSystemTimer tick db.getIndex will
			// not reload the index and return a cached (stale) index.
			theIndex.Read();
			NUnit.Framework.Assert.AreEqual(i.Count, theIndex.GetMembers().Length, "Index has not the right size."
				);
			for (int j = 0; j < theIndex.GetMembers().Length; j++)
			{
				path = theIndex.GetMembers()[j].GetName();
				expectedValue = i.Get(path);
				NUnit.Framework.Assert.IsNotNull(expectedValue, "found unexpected entry for path "
					 + path + " in index");
				NUnit.Framework.Assert.IsTrue(Arrays.Equals(db.Open(theIndex.GetMembers()[j].GetObjectId
					()).GetCachedBytes(), Sharpen.Runtime.GetBytesForString(i.Get(path))), "unexpected content for path "
					 + path + " in index. Expected: <" + expectedValue + ">");
			}
		}

		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public abstract void PrescanTwoTrees(Tree head, Tree merge);

		/// <exception cref="System.IO.IOException"></exception>
		public abstract void Checkout();

		public abstract IList<string> GetRemoved();

		public abstract IDictionary<string, ObjectId> GetUpdated();

		public abstract IList<string> GetConflicts();
	}

	/// <summary>The interface these tests need from a class implementing a checkout</summary>
	internal interface Checkout
	{
		Dictionary<string, ObjectId> Updated();

		AList<string> Conflicts();

		AList<string> Removed();

		/// <exception cref="System.IO.IOException"></exception>
		void PrescanTwoTrees();

		/// <exception cref="System.IO.IOException"></exception>
		void Checkout();
	}
}
