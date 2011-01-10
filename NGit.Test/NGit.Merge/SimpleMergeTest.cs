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
using NGit.Merge;
using NGit.Treewalk;
using Sharpen;

namespace NGit.Merge
{
	[NUnit.Framework.TestFixture]
	public class SimpleMergeTest : SampleDataRepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOurs()
		{
			Merger ourMerger = MergeStrategy.OURS.NewMerger(db);
			bool merge = ourMerger.Merge(new ObjectId[] { db.Resolve("a"), db.Resolve("c") });
			NUnit.Framework.Assert.IsTrue(merge);
			NUnit.Framework.Assert.AreEqual(db.MapTree("a").GetId(), ourMerger.GetResultTreeId
				());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTheirs()
		{
			Merger ourMerger = MergeStrategy.THEIRS.NewMerger(db);
			bool merge = ourMerger.Merge(new ObjectId[] { db.Resolve("a"), db.Resolve("c") });
			NUnit.Framework.Assert.IsTrue(merge);
			NUnit.Framework.Assert.AreEqual(db.MapTree("c").GetId(), ourMerger.GetResultTreeId
				());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTrivialTwoWay()
		{
			Merger ourMerger = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger
				(db));
			bool merge = ourMerger.Merge(new ObjectId[] { db.Resolve("a"), db.Resolve("c") });
			NUnit.Framework.Assert.IsTrue(merge);
			NUnit.Framework.Assert.AreEqual("02ba32d3649e510002c21651936b7077aa75ffa9", ourMerger
				.GetResultTreeId().Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTrivialTwoWay_disjointhistories()
		{
			Merger ourMerger = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger
				(db));
			bool merge = ourMerger.Merge(new ObjectId[] { db.Resolve("a"), db.Resolve("c~4") }
				);
			NUnit.Framework.Assert.IsTrue(merge);
			NUnit.Framework.Assert.AreEqual("86265c33b19b2be71bdd7b8cb95823f2743d03a8", ourMerger
				.GetResultTreeId().Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTrivialTwoWay_ok()
		{
			Merger ourMerger = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger
				(db));
			bool merge = ourMerger.Merge(new ObjectId[] { db.Resolve("a^0^0^0"), db.Resolve("a^0^0^1"
				) });
			NUnit.Framework.Assert.IsTrue(merge);
			NUnit.Framework.Assert.AreEqual(db.MapTree("a^0^0").GetId(), ourMerger.GetResultTreeId
				());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTrivialTwoWay_conflict()
		{
			Merger ourMerger = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger
				(db));
			bool merge = ourMerger.Merge(new ObjectId[] { db.Resolve("f"), db.Resolve("g") });
			NUnit.Framework.Assert.IsFalse(merge);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTrivialTwoWay_validSubtreeSort()
		{
			DirCache treeB = db.ReadDirCache();
			DirCache treeO = db.ReadDirCache();
			DirCache treeT = db.ReadDirCache();
			{
				DirCacheBuilder b = treeB.Builder();
				DirCacheBuilder o = treeO.Builder();
				DirCacheBuilder t = treeT.Builder();
				b.Add(MakeEntry("libelf-po/a", FileMode.REGULAR_FILE));
				b.Add(MakeEntry("libelf/c", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("Makefile", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("libelf-po/a", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("libelf/c", FileMode.REGULAR_FILE));
				t.Add(MakeEntry("libelf-po/a", FileMode.REGULAR_FILE));
				t.Add(MakeEntry("libelf/c", FileMode.REGULAR_FILE, "blah"));
				b.Finish();
				o.Finish();
				t.Finish();
			}
			ObjectInserter ow = db.NewObjectInserter();
			ObjectId b_1 = Commit(ow, treeB, new ObjectId[] {  });
			ObjectId o_1 = Commit(ow, treeO, new ObjectId[] { b_1 });
			ObjectId t_1 = Commit(ow, treeT, new ObjectId[] { b_1 });
			Merger ourMerger = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger
				(db));
			bool merge = ourMerger.Merge(new ObjectId[] { o_1, t_1 });
			NUnit.Framework.Assert.IsTrue(merge);
			TreeWalk tw = new TreeWalk(db);
			tw.Recursive = true;
			tw.Reset(ourMerger.GetResultTreeId());
			NUnit.Framework.Assert.IsTrue(tw.Next());
			NUnit.Framework.Assert.AreEqual("Makefile", tw.PathString);
			AssertCorrectId(treeO, tw);
			NUnit.Framework.Assert.IsTrue(tw.Next());
			NUnit.Framework.Assert.AreEqual("libelf-po/a", tw.PathString);
			AssertCorrectId(treeO, tw);
			NUnit.Framework.Assert.IsTrue(tw.Next());
			NUnit.Framework.Assert.AreEqual("libelf/c", tw.PathString);
			AssertCorrectId(treeT, tw);
			NUnit.Framework.Assert.IsFalse(tw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTrivialTwoWay_concurrentSubtreeChange()
		{
			DirCache treeB = db.ReadDirCache();
			DirCache treeO = db.ReadDirCache();
			DirCache treeT = db.ReadDirCache();
			{
				DirCacheBuilder b = treeB.Builder();
				DirCacheBuilder o = treeO.Builder();
				DirCacheBuilder t = treeT.Builder();
				b.Add(MakeEntry("d/o", FileMode.REGULAR_FILE));
				b.Add(MakeEntry("d/t", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("d/o", FileMode.REGULAR_FILE, "o !"));
				o.Add(MakeEntry("d/t", FileMode.REGULAR_FILE));
				t.Add(MakeEntry("d/o", FileMode.REGULAR_FILE));
				t.Add(MakeEntry("d/t", FileMode.REGULAR_FILE, "t !"));
				b.Finish();
				o.Finish();
				t.Finish();
			}
			ObjectInserter ow = db.NewObjectInserter();
			ObjectId b_1 = Commit(ow, treeB, new ObjectId[] {  });
			ObjectId o_1 = Commit(ow, treeO, new ObjectId[] { b_1 });
			ObjectId t_1 = Commit(ow, treeT, new ObjectId[] { b_1 });
			Merger ourMerger = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger
				(db));
			bool merge = ourMerger.Merge(new ObjectId[] { o_1, t_1 });
			NUnit.Framework.Assert.IsTrue(merge);
			TreeWalk tw = new TreeWalk(db);
			tw.Recursive = true;
			tw.Reset(ourMerger.GetResultTreeId());
			NUnit.Framework.Assert.IsTrue(tw.Next());
			NUnit.Framework.Assert.AreEqual("d/o", tw.PathString);
			AssertCorrectId(treeO, tw);
			NUnit.Framework.Assert.IsTrue(tw.Next());
			NUnit.Framework.Assert.AreEqual("d/t", tw.PathString);
			AssertCorrectId(treeT, tw);
			NUnit.Framework.Assert.IsFalse(tw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTrivialTwoWay_conflictSubtreeChange()
		{
			DirCache treeB = db.ReadDirCache();
			DirCache treeO = db.ReadDirCache();
			DirCache treeT = db.ReadDirCache();
			{
				DirCacheBuilder b = treeB.Builder();
				DirCacheBuilder o = treeO.Builder();
				DirCacheBuilder t = treeT.Builder();
				b.Add(MakeEntry("d/o", FileMode.REGULAR_FILE));
				b.Add(MakeEntry("d/t", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("d/o", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("d/t", FileMode.REGULAR_FILE, "o !"));
				t.Add(MakeEntry("d/o", FileMode.REGULAR_FILE, "t !"));
				t.Add(MakeEntry("d/t", FileMode.REGULAR_FILE, "t !"));
				b.Finish();
				o.Finish();
				t.Finish();
			}
			ObjectInserter ow = db.NewObjectInserter();
			ObjectId b_1 = Commit(ow, treeB, new ObjectId[] {  });
			ObjectId o_1 = Commit(ow, treeO, new ObjectId[] { b_1 });
			ObjectId t_1 = Commit(ow, treeT, new ObjectId[] { b_1 });
			Merger ourMerger = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger
				(db));
			bool merge = ourMerger.Merge(new ObjectId[] { o_1, t_1 });
			NUnit.Framework.Assert.IsFalse(merge);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTrivialTwoWay_leftDFconflict1()
		{
			DirCache treeB = db.ReadDirCache();
			DirCache treeO = db.ReadDirCache();
			DirCache treeT = db.ReadDirCache();
			{
				DirCacheBuilder b = treeB.Builder();
				DirCacheBuilder o = treeO.Builder();
				DirCacheBuilder t = treeT.Builder();
				b.Add(MakeEntry("d/o", FileMode.REGULAR_FILE));
				b.Add(MakeEntry("d/t", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("d", FileMode.REGULAR_FILE));
				t.Add(MakeEntry("d/o", FileMode.REGULAR_FILE));
				t.Add(MakeEntry("d/t", FileMode.REGULAR_FILE, "t !"));
				b.Finish();
				o.Finish();
				t.Finish();
			}
			ObjectInserter ow = db.NewObjectInserter();
			ObjectId b_1 = Commit(ow, treeB, new ObjectId[] {  });
			ObjectId o_1 = Commit(ow, treeO, new ObjectId[] { b_1 });
			ObjectId t_1 = Commit(ow, treeT, new ObjectId[] { b_1 });
			Merger ourMerger = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger
				(db));
			bool merge = ourMerger.Merge(new ObjectId[] { o_1, t_1 });
			NUnit.Framework.Assert.IsFalse(merge);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTrivialTwoWay_rightDFconflict1()
		{
			DirCache treeB = db.ReadDirCache();
			DirCache treeO = db.ReadDirCache();
			DirCache treeT = db.ReadDirCache();
			{
				DirCacheBuilder b = treeB.Builder();
				DirCacheBuilder o = treeO.Builder();
				DirCacheBuilder t = treeT.Builder();
				b.Add(MakeEntry("d/o", FileMode.REGULAR_FILE));
				b.Add(MakeEntry("d/t", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("d/o", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("d/t", FileMode.REGULAR_FILE, "o !"));
				t.Add(MakeEntry("d", FileMode.REGULAR_FILE));
				b.Finish();
				o.Finish();
				t.Finish();
			}
			ObjectInserter ow = db.NewObjectInserter();
			ObjectId b_1 = Commit(ow, treeB, new ObjectId[] {  });
			ObjectId o_1 = Commit(ow, treeO, new ObjectId[] { b_1 });
			ObjectId t_1 = Commit(ow, treeT, new ObjectId[] { b_1 });
			Merger ourMerger = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger
				(db));
			bool merge = ourMerger.Merge(new ObjectId[] { o_1, t_1 });
			NUnit.Framework.Assert.IsFalse(merge);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTrivialTwoWay_leftDFconflict2()
		{
			DirCache treeB = db.ReadDirCache();
			DirCache treeO = db.ReadDirCache();
			DirCache treeT = db.ReadDirCache();
			{
				DirCacheBuilder b = treeB.Builder();
				DirCacheBuilder o = treeO.Builder();
				DirCacheBuilder t = treeT.Builder();
				b.Add(MakeEntry("d", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("d", FileMode.REGULAR_FILE, "o !"));
				t.Add(MakeEntry("d/o", FileMode.REGULAR_FILE));
				b.Finish();
				o.Finish();
				t.Finish();
			}
			ObjectInserter ow = db.NewObjectInserter();
			ObjectId b_1 = Commit(ow, treeB, new ObjectId[] {  });
			ObjectId o_1 = Commit(ow, treeO, new ObjectId[] { b_1 });
			ObjectId t_1 = Commit(ow, treeT, new ObjectId[] { b_1 });
			Merger ourMerger = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger
				(db));
			bool merge = ourMerger.Merge(new ObjectId[] { o_1, t_1 });
			NUnit.Framework.Assert.IsFalse(merge);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTrivialTwoWay_rightDFconflict2()
		{
			DirCache treeB = db.ReadDirCache();
			DirCache treeO = db.ReadDirCache();
			DirCache treeT = db.ReadDirCache();
			{
				DirCacheBuilder b = treeB.Builder();
				DirCacheBuilder o = treeO.Builder();
				DirCacheBuilder t = treeT.Builder();
				b.Add(MakeEntry("d", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("d/o", FileMode.REGULAR_FILE));
				t.Add(MakeEntry("d", FileMode.REGULAR_FILE, "t !"));
				b.Finish();
				o.Finish();
				t.Finish();
			}
			ObjectInserter ow = db.NewObjectInserter();
			ObjectId b_1 = Commit(ow, treeB, new ObjectId[] {  });
			ObjectId o_1 = Commit(ow, treeO, new ObjectId[] { b_1 });
			ObjectId t_1 = Commit(ow, treeT, new ObjectId[] { b_1 });
			Merger ourMerger = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger
				(db));
			bool merge = ourMerger.Merge(new ObjectId[] { o_1, t_1 });
			NUnit.Framework.Assert.IsFalse(merge);
		}

		private void AssertCorrectId(DirCache treeT, TreeWalk tw)
		{
			NUnit.Framework.Assert.AreEqual(treeT.GetEntry(tw.PathString).GetObjectId(), tw.GetObjectId
				(0));
		}

		/// <exception cref="System.Exception"></exception>
		private ObjectId Commit(ObjectInserter odi, DirCache treeB, ObjectId[] parentIds)
		{
			NGit.CommitBuilder c = new NGit.CommitBuilder();
			c.TreeId = treeB.WriteTree(odi);
			c.Author = new PersonIdent("A U Thor", "a.u.thor", 1L, 0);
			c.Committer = c.Author;
			c.SetParentIds(parentIds);
			c.Message = "Tree " + c.TreeId.Name;
			ObjectId id = odi.Insert(c);
			odi.Flush();
			return id;
		}

		/// <exception cref="System.Exception"></exception>
		private DirCacheEntry MakeEntry(string path, FileMode mode)
		{
			return MakeEntry(path, mode, path);
		}

		/// <exception cref="System.Exception"></exception>
		private DirCacheEntry MakeEntry(string path, FileMode mode, string content)
		{
			DirCacheEntry ent = new DirCacheEntry(path);
			ent.FileMode = mode;
			ent.SetObjectId(new ObjectInserter.Formatter().IdFor(Constants.OBJ_BLOB, Constants
				.Encode(content)));
			return ent;
		}
	}
}
