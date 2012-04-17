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
using NGit.Api.Errors;
using NGit.Revwalk;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Unit tests of
	/// <see cref="StashCreateCommand">StashCreateCommand</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class StashDropCommandTest : RepositoryTestCase
	{
		private RevCommit head;

		private Git git;

		private FilePath committedFile;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			git = Git.Wrap(db);
			committedFile = WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			head = git.Commit().SetMessage("add file").Call();
			NUnit.Framework.Assert.IsNotNull(head);
		}

		public virtual void DropNegativeRef()
		{
			git.StashDrop().SetStashRef(-1);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void DropWithNoStashedCommits()
		{
			NUnit.Framework.Assert.IsNull(git.StashDrop().Call());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void DropWithInvalidLogIndex()
		{
			Write(committedFile, "content2");
			Ref stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNull(stashRef);
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.AreEqual(stashed, git.GetRepository().GetRef(Constants.R_STASH
				).GetObjectId());
			try
			{
				NUnit.Framework.Assert.IsNull(git.StashDrop().SetStashRef(100).Call());
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (JGitInternalException e)
			{
				NUnit.Framework.Assert.IsNotNull(e.Message);
				NUnit.Framework.Assert.IsTrue(e.Message.Length > 0);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void DropSingleStashedCommit()
		{
			Write(committedFile, "content2");
			Ref stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNull(stashRef);
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.AreEqual(stashed, git.GetRepository().GetRef(Constants.R_STASH
				).GetObjectId());
			NUnit.Framework.Assert.IsNull(git.StashDrop().Call());
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNull(stashRef);
			ReflogReader reader = new ReflogReader(git.GetRepository(), Constants.R_STASH);
			NUnit.Framework.Assert.IsTrue(reader.GetReverseEntries().IsEmpty());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void DropAll()
		{
			Write(committedFile, "content2");
			Ref stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNull(stashRef);
			RevCommit firstStash = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(firstStash);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(firstStash, git.GetRepository().GetRef(Constants.
				R_STASH).GetObjectId());
			Write(committedFile, "content3");
			RevCommit secondStash = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(secondStash);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(secondStash, git.GetRepository().GetRef(Constants
				.R_STASH).GetObjectId());
			NUnit.Framework.Assert.IsNull(git.StashDrop().SetAll(true).Call());
			NUnit.Framework.Assert.IsNull(git.GetRepository().GetRef(Constants.R_STASH));
			ReflogReader reader = new ReflogReader(git.GetRepository(), Constants.R_STASH);
			NUnit.Framework.Assert.IsTrue(reader.GetReverseEntries().IsEmpty());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void DropFirstStashedCommit()
		{
			Write(committedFile, "content2");
			Ref stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNull(stashRef);
			RevCommit firstStash = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(firstStash);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(firstStash, git.GetRepository().GetRef(Constants.
				R_STASH).GetObjectId());
			Write(committedFile, "content3");
			RevCommit secondStash = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(secondStash);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(secondStash, git.GetRepository().GetRef(Constants
				.R_STASH).GetObjectId());
			NUnit.Framework.Assert.AreEqual(firstStash, git.StashDrop().Call());
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(firstStash, stashRef.GetObjectId());
			ReflogReader reader = new ReflogReader(git.GetRepository(), Constants.R_STASH);
			IList<ReflogEntry> entries = reader.GetReverseEntries();
			NUnit.Framework.Assert.AreEqual(1, entries.Count);
			NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, entries[0].GetOldId());
			NUnit.Framework.Assert.AreEqual(firstStash, entries[0].GetNewId());
			NUnit.Framework.Assert.IsTrue(entries[0].GetComment().Length > 0);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void DropMiddleStashCommit()
		{
			Write(committedFile, "content2");
			Ref stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNull(stashRef);
			RevCommit firstStash = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(firstStash);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(firstStash, git.GetRepository().GetRef(Constants.
				R_STASH).GetObjectId());
			Write(committedFile, "content3");
			RevCommit secondStash = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(secondStash);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(secondStash, git.GetRepository().GetRef(Constants
				.R_STASH).GetObjectId());
			Write(committedFile, "content4");
			RevCommit thirdStash = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(thirdStash);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(thirdStash, git.GetRepository().GetRef(Constants.
				R_STASH).GetObjectId());
			NUnit.Framework.Assert.AreEqual(thirdStash, git.StashDrop().SetStashRef(1).Call()
				);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(thirdStash, stashRef.GetObjectId());
			ReflogReader reader = new ReflogReader(git.GetRepository(), Constants.R_STASH);
			IList<ReflogEntry> entries = reader.GetReverseEntries();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, entries[1].GetOldId());
			NUnit.Framework.Assert.AreEqual(firstStash, entries[1].GetNewId());
			NUnit.Framework.Assert.IsTrue(entries[1].GetComment().Length > 0);
			NUnit.Framework.Assert.AreEqual(entries[0].GetOldId(), entries[1].GetNewId());
			NUnit.Framework.Assert.AreEqual(thirdStash, entries[0].GetNewId());
			NUnit.Framework.Assert.IsTrue(entries[0].GetComment().Length > 0);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void DropBoundaryStashedCommits()
		{
			Write(committedFile, "content2");
			Ref stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNull(stashRef);
			RevCommit firstStash = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(firstStash);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(firstStash, git.GetRepository().GetRef(Constants.
				R_STASH).GetObjectId());
			Write(committedFile, "content3");
			RevCommit secondStash = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(secondStash);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(secondStash, git.GetRepository().GetRef(Constants
				.R_STASH).GetObjectId());
			Write(committedFile, "content4");
			RevCommit thirdStash = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(thirdStash);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(thirdStash, git.GetRepository().GetRef(Constants.
				R_STASH).GetObjectId());
			Write(committedFile, "content5");
			RevCommit fourthStash = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(fourthStash);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(fourthStash, git.GetRepository().GetRef(Constants
				.R_STASH).GetObjectId());
			NUnit.Framework.Assert.AreEqual(thirdStash, git.StashDrop().Call());
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(thirdStash, stashRef.GetObjectId());
			NUnit.Framework.Assert.AreEqual(thirdStash, git.StashDrop().SetStashRef(2).Call()
				);
			stashRef = git.GetRepository().GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(thirdStash, stashRef.GetObjectId());
			ReflogReader reader = new ReflogReader(git.GetRepository(), Constants.R_STASH);
			IList<ReflogEntry> entries = reader.GetReverseEntries();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, entries[1].GetOldId());
			NUnit.Framework.Assert.AreEqual(secondStash, entries[1].GetNewId());
			NUnit.Framework.Assert.IsTrue(entries[1].GetComment().Length > 0);
			NUnit.Framework.Assert.AreEqual(entries[0].GetOldId(), entries[1].GetNewId());
			NUnit.Framework.Assert.AreEqual(thirdStash, entries[0].GetNewId());
			NUnit.Framework.Assert.IsTrue(entries[0].GetComment().Length > 0);
		}
	}
}
