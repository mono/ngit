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

using System.Collections.Generic;
using NGit;
using NGit.Api;
using NGit.Dircache;
using NGit.Revwalk;
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class DirCacheCheckoutTest : ReadTreeTest
	{
		private DirCacheCheckout dco;

		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override void PrescanTwoTrees(Tree head, Tree merge)
		{
			DirCache dc = db.LockDirCache();
			try
			{
				dco = new DirCacheCheckout(db, head.GetTreeId(), dc, merge.GetTreeId());
				dco.PreScanTwoTrees();
			}
			finally
			{
				dc.Unlock();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Checkout()
		{
			DirCache dc = db.LockDirCache();
			try
			{
				dco = new DirCacheCheckout(db, theHead.GetTreeId(), dc, theMerge.GetTreeId());
				dco.Checkout();
			}
			finally
			{
				dc.Unlock();
			}
		}

		public override IList<string> GetRemoved()
		{
			return dco.GetRemoved();
		}

		public override IDictionary<string, ObjectId> GetUpdated()
		{
			return dco.GetUpdated();
		}

		public override IList<string> GetConflicts()
		{
			return dco.GetConflicts();
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
			NUnit.Framework.Assert.AreEqual("[E/h, mode:100644][G/i, mode:100644][f, mode:100644, stage:1][f, mode:100644, stage:2][f, mode:100644, stage:3]"
				, IndexState(0));
			ResetHard(master);
			AssertIndex(Mkmap("f", "f()\nmaster", "D/g", "g()\ng2()", "E/h", "h()"));
			AssertWorkDir(Mkmap("f", "f()\nmaster", "D/g", "g()\ng2()", "E/h", "h()", "untracked"
				, "untracked"));
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

		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private void CheckoutBranch(string branchName)
		{
			RevWalk walk = new RevWalk(db);
			RevCommit head = walk.ParseCommit(db.Resolve(Constants.HEAD));
			RevCommit branch = walk.ParseCommit(db.Resolve(branchName));
			DirCacheCheckout dco = new DirCacheCheckout(db, head.Tree, db.LockDirCache(), branch
				.Tree);
			dco.SetFailOnConflict(true);
			NUnit.Framework.Assert.IsTrue(dco.Checkout());
			walk.Release();
			// update the HEAD
			RefUpdate refUpdate = db.UpdateRef(Constants.HEAD);
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, refUpdate.Link(branchName
				));
		}
	}
}
