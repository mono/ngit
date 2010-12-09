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
	public class CherryPickTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPick()
		{
			// B---O
			// \----P---T
			//
			// Cherry-pick "T" onto "O". This shouldn't introduce "p-fail", which
			// was created by "P", nor should it modify "a", which was done by "P".
			//
			DirCache treeB = db.ReadDirCache();
			DirCache treeO = db.ReadDirCache();
			DirCache treeP = db.ReadDirCache();
			DirCache treeT = db.ReadDirCache();
			{
				DirCacheBuilder b = treeB.Builder();
				DirCacheBuilder o = treeO.Builder();
				DirCacheBuilder p = treeP.Builder();
				DirCacheBuilder t = treeT.Builder();
				b.Add(MakeEntry("a", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("a", FileMode.REGULAR_FILE));
				o.Add(MakeEntry("o", FileMode.REGULAR_FILE));
				p.Add(MakeEntry("a", FileMode.REGULAR_FILE, "q"));
				p.Add(MakeEntry("p-fail", FileMode.REGULAR_FILE));
				t.Add(MakeEntry("a", FileMode.REGULAR_FILE));
				t.Add(MakeEntry("t", FileMode.REGULAR_FILE));
				b.Finish();
				o.Finish();
				p.Finish();
				t.Finish();
			}
			ObjectInserter ow = db.NewObjectInserter();
			ObjectId B = Commit(ow, treeB, new ObjectId[] {  });
			ObjectId O = Commit(ow, treeO, new ObjectId[] { B });
			ObjectId P = Commit(ow, treeP, new ObjectId[] { B });
			ObjectId T = Commit(ow, treeT, new ObjectId[] { P });
			ThreeWayMerger twm = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger
				(db));
			twm.SetBase(P);
			bool merge = twm.Merge(new ObjectId[] { O, T });
			NUnit.Framework.Assert.IsTrue(merge);
			TreeWalk tw = new TreeWalk(db);
			tw.Recursive = true;
			tw.Reset(twm.GetResultTreeId());
			NUnit.Framework.Assert.IsTrue(tw.Next());
			NUnit.Framework.Assert.AreEqual("a", tw.PathString);
			AssertCorrectId(treeO, tw);
			NUnit.Framework.Assert.IsTrue(tw.Next());
			NUnit.Framework.Assert.AreEqual("o", tw.PathString);
			AssertCorrectId(treeO, tw);
			NUnit.Framework.Assert.IsTrue(tw.Next());
			NUnit.Framework.Assert.AreEqual("t", tw.PathString);
			AssertCorrectId(treeT, tw);
			NUnit.Framework.Assert.IsFalse(tw.Next());
		}

		private void AssertCorrectId(DirCache treeT, TreeWalk tw)
		{
			AssertEquals(treeT.GetEntry(tw.PathString).GetObjectId(), tw.GetObjectId(0));
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
