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
using NGit.Api;
using NGit.Dircache;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Api
{
	/// <summary>Test cherry-pick command</summary>
	public class CherryPickCommandTest : RepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCherryPick()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "first line\nsec. line\nthird line\n");
			git.Add().AddFilepattern("a").Call();
			RevCommit firstCommit = git.Commit().SetMessage("create a").Call();
			WriteTrashFile("b", "content\n");
			git.Add().AddFilepattern("b").Call();
			git.Commit().SetMessage("create b").Call();
			WriteTrashFile("a", "first line\nsec. line\nthird line\nfourth line\n");
			git.Add().AddFilepattern("a").Call();
			git.Commit().SetMessage("enlarged a").Call();
			WriteTrashFile("a", "first line\nsecond line\nthird line\nfourth line\n");
			git.Add().AddFilepattern("a").Call();
			RevCommit fixingA = git.Commit().SetMessage("fixed a").Call();
			git.BranchCreate().SetName("side").SetStartPoint(firstCommit).Call();
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "first line\nsec. line\nthird line\nfeature++\n");
			git.Add().AddFilepattern("a").Call();
			git.Commit().SetMessage("enhanced a").Call();
			git.CherryPick().Include(fixingA).Call();
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "b").Exists());
			CheckFile(new FilePath(db.WorkTree, "a"), "first line\nsecond line\nthird line\nfeature++\n"
				);
			Iterator<RevCommit> history = git.Log().Call().Iterator();
			NUnit.Framework.Assert.AreEqual("fixed a", history.Next().GetFullMessage());
			NUnit.Framework.Assert.AreEqual("enhanced a", history.Next().GetFullMessage());
			NUnit.Framework.Assert.AreEqual("create a", history.Next().GetFullMessage());
			NUnit.Framework.Assert.IsFalse(history.HasNext());
		}

		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private void CheckoutBranch(string branchName)
		{
			RevWalk walk = new RevWalk(db);
			RevCommit head = walk.ParseCommit(db.Resolve(Constants.HEAD));
			RevCommit branch = walk.ParseCommit(db.Resolve(branchName));
			DirCacheCheckout dco = new DirCacheCheckout(db, head.Tree.Id, db.LockDirCache(), 
				branch.Tree.Id);
			dco.SetFailOnConflict(true);
			dco.Checkout();
			walk.Release();
			// update the HEAD
			RefUpdate refUpdate = db.UpdateRef(Constants.HEAD);
			refUpdate.Link(branchName);
		}
	}
}
