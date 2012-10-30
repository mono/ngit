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
using NGit.Diff;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using NGit.Util;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Unit tests of
	/// <see cref="StashCreateCommand">StashCreateCommand</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class StashCreateCommandTest : RepositoryTestCase
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

		/// <summary>Core validation to be performed on all stashed commits</summary>
		/// <param name="commit"></param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		private void ValidateStashedCommit(RevCommit commit)
		{
			NUnit.Framework.Assert.IsNotNull(commit);
			Ref stashRef = db.GetRef(Constants.R_STASH);
			NUnit.Framework.Assert.IsNotNull(stashRef);
			NUnit.Framework.Assert.AreEqual(commit, stashRef.GetObjectId());
			NUnit.Framework.Assert.IsNotNull(commit.GetAuthorIdent());
			NUnit.Framework.Assert.AreEqual(commit.GetAuthorIdent(), commit.GetCommitterIdent
				());
			NUnit.Framework.Assert.AreEqual(2, commit.ParentCount);
			// Load parents
			RevWalk walk = new RevWalk(db);
			try
			{
				foreach (RevCommit parent in commit.Parents)
				{
					walk.ParseBody(parent);
				}
			}
			finally
			{
				walk.Release();
			}
			NUnit.Framework.Assert.AreEqual(1, commit.GetParent(1).ParentCount);
			NUnit.Framework.Assert.AreEqual(head, commit.GetParent(1).GetParent(0));
			NUnit.Framework.Assert.IsFalse(commit.Tree.Equals(head.Tree), "Head tree matches stashed commit tree"
				);
			NUnit.Framework.Assert.AreEqual(head, commit.GetParent(0));
			NUnit.Framework.Assert.IsFalse(commit.GetFullMessage().Equals(commit.GetParent(1)
				.GetFullMessage()));
		}

		private TreeWalk CreateTreeWalk()
		{
			TreeWalk walk = new TreeWalk(db);
			walk.Recursive = true;
			walk.Filter = TreeFilter.ANY_DIFF;
			return walk;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private IList<DiffEntry> DiffWorkingAgainstHead(RevCommit commit)
		{
			TreeWalk walk = CreateTreeWalk();
			try
			{
				walk.AddTree(commit.GetParent(0).Tree);
				walk.AddTree(commit.Tree);
				return DiffEntry.Scan(walk);
			}
			finally
			{
				walk.Release();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private IList<DiffEntry> DiffIndexAgainstHead(RevCommit commit)
		{
			TreeWalk walk = CreateTreeWalk();
			try
			{
				walk.AddTree(commit.GetParent(0).Tree);
				walk.AddTree(commit.GetParent(1).Tree);
				return DiffEntry.Scan(walk);
			}
			finally
			{
				walk.Release();
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void NoLocalChanges()
		{
			NUnit.Framework.Assert.IsNull(git.StashCreate().Call());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryDelete()
		{
			DeleteTrashFile("file.txt");
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ValidateStashedCommit(stashed);
			NUnit.Framework.Assert.AreEqual(head.Tree, stashed.GetParent(1).Tree);
			IList<DiffEntry> diffs = DiffWorkingAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, diffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.DELETE, diffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("file.txt", diffs[0].GetOldPath());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void IndexAdd()
		{
			FilePath addedFile = WriteTrashFile("file2.txt", "content2");
			git.Add().AddFilepattern("file2.txt").Call();
			RevCommit stashed = Git.Wrap(db).StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.IsFalse(addedFile.Exists());
			ValidateStashedCommit(stashed);
			NUnit.Framework.Assert.AreEqual(stashed.Tree, stashed.GetParent(1).Tree);
			IList<DiffEntry> diffs = DiffWorkingAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, diffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.ADD, diffs[0].GetChangeType(
				));
			NUnit.Framework.Assert.AreEqual("file2.txt", diffs[0].GetNewPath());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void IndexDelete()
		{
			git.Rm().AddFilepattern("file.txt").Call();
			RevCommit stashed = Git.Wrap(db).StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ValidateStashedCommit(stashed);
			NUnit.Framework.Assert.AreEqual(stashed.Tree, stashed.GetParent(1).Tree);
			IList<DiffEntry> diffs = DiffWorkingAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, diffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.DELETE, diffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("file.txt", diffs[0].GetOldPath());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryModify()
		{
			WriteTrashFile("file.txt", "content2");
			RevCommit stashed = Git.Wrap(db).StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ValidateStashedCommit(stashed);
			NUnit.Framework.Assert.AreEqual(head.Tree, stashed.GetParent(1).Tree);
			IList<DiffEntry> diffs = DiffWorkingAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, diffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, diffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("file.txt", diffs[0].GetNewPath());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryModifyInSubfolder()
		{
			string path = "d1/d2/f.txt";
			FilePath subfolderFile = WriteTrashFile(path, "content");
			git.Add().AddFilepattern(path).Call();
			head = git.Commit().SetMessage("add file").Call();
			WriteTrashFile(path, "content2");
			RevCommit stashed = Git.Wrap(db).StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(subfolderFile));
			ValidateStashedCommit(stashed);
			NUnit.Framework.Assert.AreEqual(head.Tree, stashed.GetParent(1).Tree);
			IList<DiffEntry> diffs = DiffWorkingAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, diffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, diffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual(path, diffs[0].GetNewPath());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryModifyIndexChanged()
		{
			WriteTrashFile("file.txt", "content2");
			git.Add().AddFilepattern("file.txt").Call();
			WriteTrashFile("file.txt", "content3");
			RevCommit stashed = Git.Wrap(db).StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ValidateStashedCommit(stashed);
			NUnit.Framework.Assert.IsFalse(stashed.Tree.Equals(stashed.GetParent(1).Tree));
			IList<DiffEntry> workingDiffs = DiffWorkingAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, workingDiffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, workingDiffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("file.txt", workingDiffs[0].GetNewPath());
			IList<DiffEntry> indexDiffs = DiffIndexAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, indexDiffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, indexDiffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("file.txt", indexDiffs[0].GetNewPath());
			NUnit.Framework.Assert.AreEqual(workingDiffs[0].GetOldId(), indexDiffs[0].GetOldId
				());
			NUnit.Framework.Assert.IsFalse(workingDiffs[0].GetNewId().Equals(indexDiffs[0].GetNewId
				()));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryCleanIndexModify()
		{
			WriteTrashFile("file.txt", "content2");
			git.Add().AddFilepattern("file.txt").Call();
			WriteTrashFile("file.txt", "content");
			RevCommit stashed = Git.Wrap(db).StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ValidateStashedCommit(stashed);
			NUnit.Framework.Assert.AreEqual(stashed.GetParent(1).Tree, stashed.Tree);
			IList<DiffEntry> workingDiffs = DiffWorkingAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, workingDiffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, workingDiffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("file.txt", workingDiffs[0].GetNewPath());
			IList<DiffEntry> indexDiffs = DiffIndexAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, indexDiffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, indexDiffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("file.txt", indexDiffs[0].GetNewPath());
			NUnit.Framework.Assert.AreEqual(workingDiffs[0].GetOldId(), indexDiffs[0].GetOldId
				());
			NUnit.Framework.Assert.IsTrue(workingDiffs[0].GetNewId().Equals(indexDiffs[0].GetNewId
				()));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryDeleteIndexAdd()
		{
			string path = "file2.txt";
			FilePath added = WriteTrashFile(path, "content2");
			NUnit.Framework.Assert.IsTrue(added.Exists());
			git.Add().AddFilepattern(path).Call();
			FileUtils.Delete(added);
			NUnit.Framework.Assert.IsFalse(added.Exists());
			RevCommit stashed = Git.Wrap(db).StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.IsFalse(added.Exists());
			ValidateStashedCommit(stashed);
			NUnit.Framework.Assert.AreEqual(stashed.GetParent(1).Tree, stashed.Tree);
			IList<DiffEntry> workingDiffs = DiffWorkingAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, workingDiffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.ADD, workingDiffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual(path, workingDiffs[0].GetNewPath());
			IList<DiffEntry> indexDiffs = DiffIndexAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, indexDiffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.ADD, indexDiffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual(path, indexDiffs[0].GetNewPath());
			NUnit.Framework.Assert.AreEqual(workingDiffs[0].GetOldId(), indexDiffs[0].GetOldId
				());
			NUnit.Framework.Assert.IsTrue(workingDiffs[0].GetNewId().Equals(indexDiffs[0].GetNewId
				()));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryDeleteIndexEdit()
		{
			FilePath edited = WriteTrashFile("file.txt", "content2");
			git.Add().AddFilepattern("file.txt").Call();
			FileUtils.Delete(edited);
			NUnit.Framework.Assert.IsFalse(edited.Exists());
			RevCommit stashed = Git.Wrap(db).StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ValidateStashedCommit(stashed);
			NUnit.Framework.Assert.IsFalse(stashed.Tree.Equals(stashed.GetParent(1).Tree));
			IList<DiffEntry> workingDiffs = DiffWorkingAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, workingDiffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.DELETE, workingDiffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("file.txt", workingDiffs[0].GetOldPath());
			IList<DiffEntry> indexDiffs = DiffIndexAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(1, indexDiffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, indexDiffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("file.txt", indexDiffs[0].GetNewPath());
			NUnit.Framework.Assert.AreEqual(workingDiffs[0].GetOldId(), indexDiffs[0].GetOldId
				());
			NUnit.Framework.Assert.IsFalse(workingDiffs[0].GetNewId().Equals(indexDiffs[0].GetNewId
				()));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void MultipleEdits()
		{
			git.Rm().AddFilepattern("file.txt").Call();
			FilePath addedFile = WriteTrashFile("file2.txt", "content2");
			git.Add().AddFilepattern("file2.txt").Call();
			RevCommit stashed = Git.Wrap(db).StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.IsFalse(addedFile.Exists());
			ValidateStashedCommit(stashed);
			NUnit.Framework.Assert.AreEqual(stashed.Tree, stashed.GetParent(1).Tree);
			IList<DiffEntry> diffs = DiffWorkingAgainstHead(stashed);
			NUnit.Framework.Assert.AreEqual(2, diffs.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.DELETE, diffs[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("file.txt", diffs[0].GetOldPath());
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.ADD, diffs[1].GetChangeType(
				));
			NUnit.Framework.Assert.AreEqual("file2.txt", diffs[1].GetNewPath());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void RefLogIncludesCommitMessage()
		{
			PersonIdent who = new PersonIdent("user", "user@email.com");
			DeleteTrashFile("file.txt");
			RevCommit stashed = git.StashCreate().SetPerson(who).Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ValidateStashedCommit(stashed);
			ReflogReader reader = new ReflogReader(git.GetRepository(), Constants.R_STASH);
			ReflogEntry entry = reader.GetLastEntry();
			NUnit.Framework.Assert.IsNotNull(entry);
			NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, entry.GetOldId());
			NUnit.Framework.Assert.AreEqual(stashed, entry.GetNewId());
			NUnit.Framework.Assert.AreEqual(who, entry.GetWho());
			NUnit.Framework.Assert.AreEqual(stashed.GetFullMessage(), entry.GetComment());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void UnmergedPathsShouldCauseException()
		{
			CommitFile("file.txt", "master", "base");
			RevCommit side = CommitFile("file.txt", "side", "side");
			CommitFile("file.txt", "master", "master");
			git.Merge().Include(side).Call();
			git.StashCreate().Call();
		}
	}
}
