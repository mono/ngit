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
using NGit.Revwalk;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Unit tests of
	/// <see cref="StashListCommand">StashListCommand</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class StashListCommandTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void NoStashRef()
		{
			StashListCommand command = Git.Wrap(db).StashList();
			ICollection<RevCommit> stashed = command.Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.IsTrue(stashed.IsEmpty());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void EmptyStashReflog()
		{
			Git git = Git.Wrap(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit = git.Commit().SetMessage("create file").Call();
			RefUpdate update = db.UpdateRef(Constants.R_STASH);
			update.SetNewObjectId(commit);
			update.DisableRefLog();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, update.Update());
			StashListCommand command = Git.Wrap(db).StashList();
			ICollection<RevCommit> stashed = command.Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.IsTrue(stashed.IsEmpty());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void SingleStashedCommit()
		{
			Git git = Git.Wrap(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit = git.Commit().SetMessage("create file").Call();
			RefUpdate update = db.UpdateRef(Constants.R_STASH);
			update.SetNewObjectId(commit);
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, update.Update());
			StashListCommand command = git.StashList();
			ICollection<RevCommit> stashed = command.Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual(1, stashed.Count);
			NUnit.Framework.Assert.AreEqual(commit, stashed.Iterator().Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void MultipleStashedCommits()
		{
			Git git = Git.Wrap(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit1 = git.Commit().SetMessage("create file").Call();
			WriteTrashFile("file.txt", "content2");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit2 = git.Commit().SetMessage("edit file").Call();
			RefUpdate create = db.UpdateRef(Constants.R_STASH);
			create.SetNewObjectId(commit1);
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, create.Update());
			RefUpdate update = db.UpdateRef(Constants.R_STASH);
			update.SetNewObjectId(commit2);
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FAST_FORWARD, update.Update());
			StashListCommand command = git.StashList();
			ICollection<RevCommit> stashed = command.Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual(2, stashed.Count);
			Iterator<RevCommit> iter = stashed.Iterator();
			NUnit.Framework.Assert.AreEqual(commit2, iter.Next());
			NUnit.Framework.Assert.AreEqual(commit1, iter.Next());
		}
	}
}
