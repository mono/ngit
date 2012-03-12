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
using NGit.Api;
using NGit.Dircache;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Treewalk;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class DirCacheCheckoutTest : RepositoryTestCase
	{
		private DirCacheCheckout dco;

		protected internal ObjectId theHead;

		protected internal ObjectId theMerge;

		private DirCache dirCache;

		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private void PrescanTwoTrees(ObjectId head, ObjectId merge)
		{
			DirCache dc = db.LockDirCache();
			try
			{
				dco = new DirCacheCheckout(db, head, dc, merge);
				dco.PreScanTwoTrees();
			}
			finally
			{
				dc.Unlock();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Checkout()
		{
			DirCache dc = db.LockDirCache();
			try
			{
				dco = new DirCacheCheckout(db, theHead, dc, theMerge);
				dco.Checkout();
			}
			finally
			{
				dc.Unlock();
			}
		}

		private IList<string> GetRemoved()
		{
			return dco.GetRemoved();
		}

		private IDictionary<string, ObjectId> GetUpdated()
		{
			return dco.GetUpdated();
		}

		private IList<string> GetConflicts()
		{
			return dco.GetConflicts();
		}

		private static Dictionary<string, string> Mk(string a)
		{
			return Mkmap(a, a);
		}

		private static Dictionary<string, string> Mkmap(params string[] args)
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
		/// <exception cref="NGit.Api.Errors.NoFilepatternException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestResetHard()
		{
			Git git = new Git(db);
			WriteTrashFile("f", "f()");
			WriteTrashFile("D/g", "g()");
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("inital").Call();
			AssertIndex(Mkmap("f", "f()", "D/g", "g()"));
			git.BranchCreate().SetName("topic").Call();
			WriteTrashFile("f", "f()\nmaster");
			WriteTrashFile("D/g", "g()\ng2()");
			WriteTrashFile("E/h", "h()");
			git.Add().AddFilepattern(".").Call();
			RevCommit master = git.Commit().SetMessage("master-1").Call();
			AssertIndex(Mkmap("f", "f()\nmaster", "D/g", "g()\ng2()", "E/h", "h()"));
			CheckoutBranch("refs/heads/topic");
			AssertIndex(Mkmap("f", "f()", "D/g", "g()"));
			WriteTrashFile("f", "f()\nside");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "D/g").Delete());
			WriteTrashFile("G/i", "i()");
			git.Add().AddFilepattern(".").Call();
			git.Add().AddFilepattern(".").SetUpdate(true).Call();
			RevCommit topic = git.Commit().SetMessage("topic-1").Call();
			AssertIndex(Mkmap("f", "f()\nside", "G/i", "i()"));
			WriteTrashFile("untracked", "untracked");
			ResetHard(master);
			AssertIndex(Mkmap("f", "f()\nmaster", "D/g", "g()\ng2()", "E/h", "h()"));
			ResetHard(topic);
			AssertIndex(Mkmap("f", "f()\nside", "G/i", "i()"));
			AssertWorkDir(Mkmap("f", "f()\nside", "G/i", "i()", "untracked", "untracked"));
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, git.Merge().Include(master
				).Call().GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("[D/g, mode:100644, stage:1][D/g, mode:100644, stage:3][E/h, mode:100644][G/i, mode:100644][f, mode:100644, stage:1][f, mode:100644, stage:2][f, mode:100644, stage:3]"
				, IndexState(0));
			ResetHard(master);
			AssertIndex(Mkmap("f", "f()\nmaster", "D/g", "g()\ng2()", "E/h", "h()"));
			AssertWorkDir(Mkmap("f", "f()\nmaster", "D/g", "g()\ng2()", "E/h", "h()", "untracked"
				, "untracked"));
		}

		/// <summary>Reset hard from unclean condition.</summary>
		/// <remarks>
		/// Reset hard from unclean condition.
		/// <p>
		/// WorkDir: Empty <br/>
		/// Index: f/g <br/>
		/// Merge: x
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestResetHardFromIndexEntryWithoutFileToTreeWithoutFile()
		{
			Git git = new Git(db);
			WriteTrashFile("x", "x");
			git.Add().AddFilepattern("x").Call();
			RevCommit id1 = git.Commit().SetMessage("c1").Call();
			WriteTrashFile("f/g", "f/g");
			git.Rm().AddFilepattern("x").Call();
			git.Add().AddFilepattern("f/g").Call();
			git.Commit().SetMessage("c2").Call();
			DeleteTrashFile("f/g");
			DeleteTrashFile("f");
			// The actual test
			git.Reset().SetMode(ResetCommand.ResetType.HARD).SetRef(id1.GetName()).Call();
			AssertIndex(Mkmap("x", "x"));
		}

		/// <exception cref="NGit.Errors.NoWorkTreeException"></exception>
		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private DirCacheCheckout ResetHard(RevCommit commit)
		{
			DirCacheCheckout dc;
			dc = new DirCacheCheckout(db, null, db.LockDirCache(), commit.Tree);
			dc.SetFailOnConflict(true);
			NUnit.Framework.Assert.IsTrue(dc.Checkout());
			return dc;
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private void AssertIndex(Dictionary<string, string> i)
		{
			string expectedValue;
			string path;
			DirCache read = DirCache.Read(db.GetIndexFile(), db.FileSystem);
			NUnit.Framework.Assert.AreEqual(i.Count, read.GetEntryCount(), "Index has not the right size."
				);
			for (int j = 0; j < read.GetEntryCount(); j++)
			{
				path = read.GetEntry(j).PathString;
				expectedValue = i.Get(path);
				NUnit.Framework.Assert.IsNotNull(expectedValue, "found unexpected entry for path "
					 + path + " in index");
				NUnit.Framework.Assert.IsTrue(Arrays.Equals(db.Open(read.GetEntry(j).GetObjectId(
					)).GetCachedBytes(), Sharpen.Runtime.GetBytesForString(i.Get(path))), "unexpected content for path "
					 + path + " in index. Expected: <" + expectedValue + ">");
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRules1thru3_NoIndexEntry()
		{
			ObjectId head = BuildTree(Mk("foo"));
			TreeWalk tw = TreeWalk.ForPath(db, "foo", head);
			ObjectId objectId = tw.GetObjectId(0);
			ObjectId merge = db.NewObjectInserter().Insert(Constants.OBJ_TREE, new byte[0]);
			PrescanTwoTrees(head, merge);
			NUnit.Framework.Assert.IsTrue(GetRemoved().Contains("foo"));
			PrescanTwoTrees(merge, head);
			NUnit.Framework.Assert.AreEqual(objectId, GetUpdated().Get("foo"));
			merge = BuildTree(Mkmap("foo", "a"));
			tw = TreeWalk.ForPath(db, "foo", merge);
			ObjectId anotherId = tw.GetObjectId(0);
			PrescanTwoTrees(head, merge);
			NUnit.Framework.Assert.AreEqual(anotherId, GetUpdated().Get("foo"));
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
			dirCache = new DirCache(db.GetIndexFile(), db.FileSystem);
			if (indexEntries != null)
			{
				NUnit.Framework.Assert.IsTrue(dirCache.Lock());
				DirCacheEditor editor = dirCache.Editor();
				foreach (KeyValuePair<string, string> e in indexEntries.EntrySet())
				{
					WriteTrashFile(e.Key, e.Value);
					ObjectInserter inserter = db.NewObjectInserter();
					ObjectId id = inserter.Insert(Constants.OBJ_BLOB, Constants.Encode(e.Value));
					editor.Add(new DirCacheEditor.DeletePath(e.Key));
					editor.Add(new _PathEdit_288(id, e.Key));
				}
				NUnit.Framework.Assert.IsTrue(editor.Commit());
			}
		}

		private sealed class _PathEdit_288 : DirCacheEditor.PathEdit
		{
			public _PathEdit_288(ObjectId id, string baseArg1) : base(baseArg1)
			{
				this.id = id;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.REGULAR_FILE;
				ent.SetObjectId(id);
				ent.IsUpdateNeeded = false;
			}

			private readonly ObjectId id;
		}

		internal sealed class AddEdit : DirCacheEditor.PathEdit
		{
			private readonly ObjectId data;

			private readonly long length;

			public AddEdit(string entryPath, ObjectId data, long length) : base(entryPath)
			{
				this.data = data;
				this.length = length;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.REGULAR_FILE;
				ent.SetLength(length);
				ent.SetObjectId(data);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private ObjectId BuildTree(Dictionary<string, string> headEntries)
		{
			DirCache lockDirCache = DirCache.NewInCore();
			// assertTrue(lockDirCache.lock());
			DirCacheEditor editor = lockDirCache.Editor();
			if (headEntries != null)
			{
				foreach (KeyValuePair<string, string> e in headEntries.EntrySet())
				{
					DirCacheCheckoutTest.AddEdit addEdit = new DirCacheCheckoutTest.AddEdit(e.Key, GenSha1
						(e.Value), e.Value.Length);
					editor.Add(addEdit);
				}
			}
			editor.Finish();
			return lockDirCache.WriteTree(db.NewObjectInserter());
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
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "foo").Delete());
			WriteTrashFile("foo", "bar");
			db.ReadDirCache().GetEntry(0).IsUpdateNeeded = true;
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
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "foo").Delete());
			WriteTrashFile("foo", "bar");
			db.ReadDirCache().GetEntry(0).IsUpdateNeeded = true;
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
			ObjectId treeDF = BuildTree(Mkmap("DF", "DF"));
			ObjectId treeDFDF = BuildTree(Mkmap("DF/DF", "DF/DF"));
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

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUntrackedConflicts()
		{
			SetupCase(null, Mk("foo"), null);
			WriteTrashFile("foo", "foo");
			Go();
			// test that we don't overwrite untracked files when there is a HEAD
			RecursiveDelete(new FilePath(trash, "foo"));
			SetupCase(Mk("other"), Mkmap("other", "other", "foo", "foo"), Mk("other"));
			WriteTrashFile("foo", "bar");
			try
			{
				Checkout();
				NUnit.Framework.Assert.Fail("didn't get the expected exception");
			}
			catch (NGit.Errors.CheckoutConflictException)
			{
				AssertConflict("foo");
				AssertWorkDir(Mkmap("foo", "bar", "other", "other"));
				AssertIndex(Mk("other"));
			}
			// test that we don't overwrite untracked files when there is no HEAD
			RecursiveDelete(new FilePath(trash, "other"));
			RecursiveDelete(new FilePath(trash, "foo"));
			SetupCase(null, Mk("foo"), null);
			WriteTrashFile("foo", "bar");
			try
			{
				Checkout();
				NUnit.Framework.Assert.Fail("didn't get the expected exception");
			}
			catch (NGit.Errors.CheckoutConflictException)
			{
				AssertConflict("foo");
				AssertWorkDir(Mkmap("foo", "bar"));
				AssertIndex(Mkmap("other", "other"));
			}
			// TODO: Why should we expect conflicts here?
			// H and M are empty and according to rule #5 of
			// the carry-over rules a dirty index is no reason
			// for a conflict. (I also feel it should be a
			// conflict because we are going to overwrite
			// unsaved content in the working tree
			// This test would fail in DirCacheCheckoutTest
			// assertConflict("foo");
			RecursiveDelete(new FilePath(trash, "foo"));
			RecursiveDelete(new FilePath(trash, "other"));
			SetupCase(null, Mk("foo"), null);
			WriteTrashFile("foo/bar/baz", string.Empty);
			WriteTrashFile("foo/blahblah", string.Empty);
			Go();
			AssertConflict("foo");
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
			catch (NGit.Errors.CheckoutConflictException)
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
			catch (NGit.Errors.CheckoutConflictException)
			{
				AssertIndex(Mkmap("foo", "bar"));
				AssertWorkDir(Mkmap("foo", "bar"));
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutOutChangesAutoCRLFfalse()
		{
			SetupCase(Mk("foo"), Mkmap("foo/bar", "foo\nbar"), Mk("foo"));
			Checkout();
			AssertIndex(Mkmap("foo/bar", "foo\nbar"));
			AssertWorkDir(Mkmap("foo/bar", "foo\nbar"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutOutChangesAutoCRLFInput()
		{
			SetupCase(Mk("foo"), Mkmap("foo/bar", "foo\nbar"), Mk("foo"));
			((FileBasedConfig)db.GetConfig()).SetString("core", null, "autocrlf", "input");
			Checkout();
			AssertIndex(Mkmap("foo/bar", "foo\nbar"));
			AssertWorkDir(Mkmap("foo/bar", "foo\nbar"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutOutChangesAutoCRLFtrue()
		{
			SetupCase(Mk("foo"), Mkmap("foo/bar", "foo\nbar"), Mk("foo"));
			((FileBasedConfig)db.GetConfig()).SetString("core", null, "autocrlf", "true");
			Checkout();
			AssertIndex(Mkmap("foo/bar", "foo\nbar"));
			AssertWorkDir(Mkmap("foo/bar", "foo\r\nbar"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutOutChangesAutoCRLFtrueBinary()
		{
			SetupCase(Mk("foo"), Mkmap("foo/bar", "foo\nb\u0000ar"), Mk("foo"));
			((FileBasedConfig)db.GetConfig()).SetString("core", null, "autocrlf", "true");
			Checkout();
			AssertIndex(Mkmap("foo/bar", "foo\nb\u0000ar"));
			AssertWorkDir(Mkmap("foo/bar", "foo\nb\u0000ar"));
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
			catch (NGit.Errors.CheckoutConflictException)
			{
				AssertIndex(Mk("foo"));
				AssertWorkDir(Mkmap("foo", "different"));
				NUnit.Framework.Assert.IsTrue(GetConflicts().Equals(Arrays.AsList("foo")));
				NUnit.Framework.Assert.IsTrue(new FilePath(trash, "foo").IsFile());
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileModeChangeWithNoContentChangeUpdate()
		{
			if (!FS.DETECTED.SupportsExecute())
			{
				return;
			}
			Git git = Git.Wrap(db);
			// Add non-executable file
			FilePath file = WriteTrashFile("file.txt", "a");
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("commit1").Call();
			NUnit.Framework.Assert.IsFalse(db.FileSystem.CanExecute(file));
			// Create branch
			git.BranchCreate().SetName("b1").Call();
			// Make file executable
			db.FileSystem.SetExecute(file, true);
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("commit2").Call();
			// Verify executable and working directory is clean
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetModified().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsTrue(db.FileSystem.CanExecute(file));
			// Switch branches
			git.Checkout().SetName("b1").Call();
			// Verify not executable and working directory is clean
			status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetModified().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsFalse(db.FileSystem.CanExecute(file));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileModeChangeAndContentChangeConflict()
		{
			if (!FS.DETECTED.SupportsExecute())
			{
				return;
			}
			Git git = Git.Wrap(db);
			// Add non-executable file
			FilePath file = WriteTrashFile("file.txt", "a");
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("commit1").Call();
			NUnit.Framework.Assert.IsFalse(db.FileSystem.CanExecute(file));
			// Create branch
			git.BranchCreate().SetName("b1").Call();
			// Make file executable
			db.FileSystem.SetExecute(file, true);
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("commit2").Call();
			// Verify executable and working directory is clean
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetModified().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsTrue(db.FileSystem.CanExecute(file));
			WriteTrashFile("file.txt", "b");
			// Switch branches
			CheckoutCommand checkout = git.Checkout().SetName("b1");
			try
			{
				checkout.Call();
				NUnit.Framework.Assert.Fail("Checkout exception not thrown");
			}
			catch (NGit.Api.Errors.CheckoutConflictException)
			{
				CheckoutResult result = checkout.GetResult();
				NUnit.Framework.Assert.IsNotNull(result);
				NUnit.Framework.Assert.IsNotNull(result.GetConflictList());
				NUnit.Framework.Assert.AreEqual(1, result.GetConflictList().Count);
				NUnit.Framework.Assert.IsTrue(result.GetConflictList().Contains("file.txt"));
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirtyFileModeEqualHeadMerge()
		{
			if (!FS.DETECTED.SupportsExecute())
			{
				return;
			}
			Git git = Git.Wrap(db);
			// Add non-executable file
			FilePath file = WriteTrashFile("file.txt", "a");
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("commit1").Call();
			NUnit.Framework.Assert.IsFalse(db.FileSystem.CanExecute(file));
			// Create branch
			git.BranchCreate().SetName("b1").Call();
			// Create second commit and don't touch file
			WriteTrashFile("file2.txt", string.Empty);
			git.Add().AddFilepattern("file2.txt").Call();
			git.Commit().SetMessage("commit2").Call();
			// stage a mode change
			WriteTrashFile("file.txt", "a");
			db.FileSystem.SetExecute(file, true);
			git.Add().AddFilepattern("file.txt").Call();
			// dirty the file
			WriteTrashFile("file.txt", "b");
			NUnit.Framework.Assert.AreEqual("[file.txt, mode:100755, content:a][file2.txt, mode:100644, content:]"
				, IndexState(CONTENT));
			AssertWorkDir(Mkmap("file.txt", "b", "file2.txt", string.Empty));
			// Switch branches and check that the dirty file survived in worktree
			// and index
			git.Checkout().SetName("b1").Call();
			NUnit.Framework.Assert.AreEqual("[file.txt, mode:100755, content:a]", IndexState(
				CONTENT));
			AssertWorkDir(Mkmap("file.txt", "b"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirtyFileModeEqualIndexMerge()
		{
			if (!FS.DETECTED.SupportsExecute())
			{
				return;
			}
			Git git = Git.Wrap(db);
			// Add non-executable file
			FilePath file = WriteTrashFile("file.txt", "a");
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("commit1").Call();
			NUnit.Framework.Assert.IsFalse(db.FileSystem.CanExecute(file));
			// Create branch
			git.BranchCreate().SetName("b1").Call();
			// Create second commit with executable file
			file = WriteTrashFile("file.txt", "b");
			db.FileSystem.SetExecute(file, true);
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("commit2").Call();
			// stage the same content as in the branch we want to switch to
			WriteTrashFile("file.txt", "a");
			db.FileSystem.SetExecute(file, false);
			git.Add().AddFilepattern("file.txt").Call();
			// dirty the file
			WriteTrashFile("file.txt", "c");
			db.FileSystem.SetExecute(file, true);
			NUnit.Framework.Assert.AreEqual("[file.txt, mode:100644, content:a]", IndexState(
				CONTENT));
			AssertWorkDir(Mkmap("file.txt", "c"));
			// Switch branches and check that the dirty file survived in worktree
			// and index
			git.Checkout().SetName("b1").Call();
			NUnit.Framework.Assert.AreEqual("[file.txt, mode:100644, content:a]", IndexState(
				CONTENT));
			AssertWorkDir(Mkmap("file.txt", "c"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileModeChangeAndContentChangeNoConflict()
		{
			if (!FS.DETECTED.SupportsExecute())
			{
				return;
			}
			Git git = Git.Wrap(db);
			// Add first file
			FilePath file1 = WriteTrashFile("file1.txt", "a");
			git.Add().AddFilepattern("file1.txt").Call();
			git.Commit().SetMessage("commit1").Call();
			NUnit.Framework.Assert.IsFalse(db.FileSystem.CanExecute(file1));
			// Add second file
			FilePath file2 = WriteTrashFile("file2.txt", "b");
			git.Add().AddFilepattern("file2.txt").Call();
			git.Commit().SetMessage("commit2").Call();
			NUnit.Framework.Assert.IsFalse(db.FileSystem.CanExecute(file2));
			// Create branch from first commit
			NUnit.Framework.Assert.IsNotNull(git.Checkout().SetCreateBranch(true).SetName("b1"
				).SetStartPoint(Constants.HEAD + "~1").Call());
			// Change content and file mode in working directory and index
			file1 = WriteTrashFile("file1.txt", "c");
			db.FileSystem.SetExecute(file1, true);
			git.Add().AddFilepattern("file1.txt").Call();
			// Switch back to 'master'
			NUnit.Framework.Assert.IsNotNull(git.Checkout().SetName(Constants.MASTER).Call());
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void AssertWorkDir(Dictionary<string, string> i)
		{
			TreeWalk walk = new TreeWalk(db);
			walk.Recursive = true;
			walk.AddTree(new FileTreeIterator(db));
			string expectedValue;
			string path;
			int nrFiles = 0;
			FileTreeIterator ft;
			while (walk.Next())
			{
				ft = walk.GetTree<FileTreeIterator>(0);
				path = ft.EntryPathString;
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
					
					CollectionAssert.AreEqual (buffer, Sharpen.Runtime.GetBytesForString(i.Get(path)), 
						"unexpected content for path " + path + " in workDir. ");
					nrFiles++;
				}
			}
			NUnit.Framework.Assert.AreEqual(i.Count, nrFiles, "WorkDir has not the right size."
				);
		}
	}
}
