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
using NGit.Api.Errors;
using NGit.Internal;
using NGit.Revwalk;
using NGit.Util;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Unit tests of
	/// <see cref="StashApplyCommand">StashApplyCommand</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class StashApplyCommandTest : RepositoryTestCase
	{
		private static readonly string PATH = "file.txt";

		private RevCommit head;

		private Git git;

		private FilePath committedFile;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			git = Git.Wrap(db);
			committedFile = WriteTrashFile(PATH, "content");
			git.Add().AddFilepattern(PATH).Call();
			head = git.Commit().SetMessage("add file").Call();
			NUnit.Framework.Assert.IsNotNull(head);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryDelete()
		{
			DeleteTrashFile(PATH);
			NUnit.Framework.Assert.IsFalse(committedFile.Exists());
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ObjectId unstashed = git.StashApply().Call();
			NUnit.Framework.Assert.AreEqual(stashed, unstashed);
			NUnit.Framework.Assert.IsFalse(committedFile.Exists());
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetAdded().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetConflicting().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetModified().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetUntracked().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetRemoved().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, status.GetMissing().Count);
			NUnit.Framework.Assert.IsTrue(status.GetMissing().Contains(PATH));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void IndexAdd()
		{
			string addedPath = "file2.txt";
			FilePath addedFile = WriteTrashFile(addedPath, "content2");
			git.Add().AddFilepattern(addedPath).Call();
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.IsFalse(addedFile.Exists());
			ObjectId unstashed = git.StashApply().Call();
			NUnit.Framework.Assert.AreEqual(stashed, unstashed);
			NUnit.Framework.Assert.IsTrue(addedFile.Exists());
			NUnit.Framework.Assert.AreEqual("content2", Read(addedFile));
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetConflicting().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetMissing().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetModified().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetUntracked().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, status.GetAdded().Count);
			NUnit.Framework.Assert.IsTrue(status.GetAdded().Contains(addedPath));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void IndexDelete()
		{
			git.Rm().AddFilepattern("file.txt").Call();
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ObjectId unstashed = git.StashApply().Call();
			NUnit.Framework.Assert.AreEqual(stashed, unstashed);
			NUnit.Framework.Assert.IsFalse(committedFile.Exists());
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetAdded().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetConflicting().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetModified().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetMissing().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetUntracked().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, status.GetRemoved().Count);
			NUnit.Framework.Assert.IsTrue(status.GetRemoved().Contains(PATH));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryModify()
		{
			WriteTrashFile("file.txt", "content2");
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ObjectId unstashed = git.StashApply().Call();
			NUnit.Framework.Assert.AreEqual(stashed, unstashed);
			NUnit.Framework.Assert.AreEqual("content2", Read(committedFile));
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetAdded().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetConflicting().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetMissing().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetUntracked().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, status.GetModified().Count);
			NUnit.Framework.Assert.IsTrue(status.GetModified().Contains(PATH));
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
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(subfolderFile));
			ObjectId unstashed = git.StashApply().Call();
			NUnit.Framework.Assert.AreEqual(stashed, unstashed);
			NUnit.Framework.Assert.AreEqual("content2", Read(subfolderFile));
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetAdded().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetConflicting().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetMissing().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetUntracked().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, status.GetModified().Count);
			NUnit.Framework.Assert.IsTrue(status.GetModified().Contains(path));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryModifyIndexChanged()
		{
			WriteTrashFile("file.txt", "content2");
			git.Add().AddFilepattern("file.txt").Call();
			WriteTrashFile("file.txt", "content3");
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ObjectId unstashed = git.StashApply().Call();
			NUnit.Framework.Assert.AreEqual(stashed, unstashed);
			NUnit.Framework.Assert.AreEqual("content3", Read(committedFile));
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetAdded().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetConflicting().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetMissing().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetUntracked().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, status.GetChanged().Count);
			NUnit.Framework.Assert.IsTrue(status.GetChanged().Contains(PATH));
			NUnit.Framework.Assert.AreEqual(1, status.GetModified().Count);
			NUnit.Framework.Assert.IsTrue(status.GetModified().Contains(PATH));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryCleanIndexModify()
		{
			WriteTrashFile("file.txt", "content2");
			git.Add().AddFilepattern("file.txt").Call();
			WriteTrashFile("file.txt", "content");
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ObjectId unstashed = git.StashApply().Call();
			NUnit.Framework.Assert.AreEqual(stashed, unstashed);
			NUnit.Framework.Assert.AreEqual("content2", Read(committedFile));
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetAdded().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetConflicting().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetMissing().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetModified().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetUntracked().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, status.GetChanged().Count);
			NUnit.Framework.Assert.IsTrue(status.GetChanged().Contains(PATH));
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
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.IsFalse(added.Exists());
			ObjectId unstashed = git.StashApply().Call();
			NUnit.Framework.Assert.AreEqual(stashed, unstashed);
			NUnit.Framework.Assert.AreEqual("content2", Read(added));
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetConflicting().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetMissing().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetModified().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetUntracked().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, status.GetAdded().Count);
			NUnit.Framework.Assert.IsTrue(status.GetAdded().Contains(path));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryDeleteIndexEdit()
		{
			WriteTrashFile(PATH, "content2");
			git.Add().AddFilepattern(PATH).Call();
			FileUtils.Delete(committedFile);
			NUnit.Framework.Assert.IsFalse(committedFile.Exists());
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			ObjectId unstashed = git.StashApply().Call();
			NUnit.Framework.Assert.AreEqual(stashed, unstashed);
			NUnit.Framework.Assert.IsFalse(committedFile.Exists());
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetAdded().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetConflicting().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetMissing().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetModified().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetUntracked().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, status.GetRemoved().Count);
			NUnit.Framework.Assert.IsTrue(status.GetRemoved().Contains(PATH));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void MultipleEdits()
		{
			string addedPath = "file2.txt";
			git.Rm().AddFilepattern(PATH).Call();
			FilePath addedFile = WriteTrashFile(addedPath, "content2");
			git.Add().AddFilepattern(addedPath).Call();
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.IsTrue(committedFile.Exists());
			NUnit.Framework.Assert.IsFalse(addedFile.Exists());
			ObjectId unstashed = git.StashApply().Call();
			NUnit.Framework.Assert.AreEqual(stashed, unstashed);
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetConflicting().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetMissing().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetModified().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetUntracked().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, status.GetRemoved().Count);
			NUnit.Framework.Assert.IsTrue(status.GetRemoved().Contains(PATH));
			NUnit.Framework.Assert.AreEqual(1, status.GetAdded().Count);
			NUnit.Framework.Assert.IsTrue(status.GetAdded().Contains(addedPath));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryContentConflict()
		{
			WriteTrashFile(PATH, "content2");
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			NUnit.Framework.Assert.IsTrue(git.Status().Call().IsClean());
			WriteTrashFile(PATH, "content3");
			try
			{
				git.StashApply().Call();
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (JGitInternalException e)
			{
				NUnit.Framework.Assert.IsTrue(e.InnerException is NGit.Errors.CheckoutConflictException
					);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void IndexContentConflict()
		{
			WriteTrashFile(PATH, "content2");
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			NUnit.Framework.Assert.IsTrue(git.Status().Call().IsClean());
			WriteTrashFile(PATH, "content3");
			git.Add().AddFilepattern(PATH).Call();
			WriteTrashFile(PATH, "content2");
			try
			{
				git.StashApply().Call();
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (JGitInternalException e)
			{
				NUnit.Framework.Assert.IsTrue(e.InnerException is NGit.Errors.CheckoutConflictException
					);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void WorkingDirectoryEditPreCommit()
		{
			WriteTrashFile(PATH, "content2");
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.AreEqual("content", Read(committedFile));
			NUnit.Framework.Assert.IsTrue(git.Status().Call().IsClean());
			string path2 = "file2.txt";
			WriteTrashFile(path2, "content3");
			git.Add().AddFilepattern(path2).Call();
			NUnit.Framework.Assert.IsNotNull(git.Commit().SetMessage("adding file").Call());
			ObjectId unstashed = git.StashApply().Call();
			NUnit.Framework.Assert.AreEqual(stashed, unstashed);
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetAdded().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetConflicting().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetMissing().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetUntracked().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, status.GetModified().Count);
			NUnit.Framework.Assert.IsTrue(status.GetModified().Contains(PATH));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void StashChangeInANewSubdirectory()
		{
			string subdir = "subdir";
			string fname = "file2.txt";
			string path = subdir + "/" + fname;
			string otherBranch = "otherbranch";
			WriteTrashFile(subdir, fname, "content2");
			git.Add().AddFilepattern(path).Call();
			RevCommit stashed = git.StashCreate().Call();
			NUnit.Framework.Assert.IsNotNull(stashed);
			NUnit.Framework.Assert.IsTrue(git.Status().Call().IsClean());
			git.BranchCreate().SetName(otherBranch).Call();
			git.Checkout().SetName(otherBranch).Call();
			ObjectId unstashed = git.StashApply().Call();
			NUnit.Framework.Assert.AreEqual(stashed, unstashed);
			Status status = git.Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetChanged().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetConflicting().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetMissing().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetRemoved().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetModified().IsEmpty());
			NUnit.Framework.Assert.IsTrue(status.GetUntracked().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, status.GetAdded().Count);
			NUnit.Framework.Assert.IsTrue(status.GetAdded().Contains(path));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void UnstashNonStashCommit()
		{
			try
			{
				git.StashApply().SetStashRef(head.Name).Call();
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (JGitInternalException e)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().stashCommitMissingTwoParents
					, head.Name), e.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void UnstashNoHead()
		{
			Repository repo = CreateWorkRepository();
			try
			{
				Git.Wrap(repo).StashApply().Call();
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (NoHeadException e)
			{
				NUnit.Framework.Assert.IsNotNull(e.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void NoStashedCommits()
		{
			try
			{
				git.StashApply().Call();
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (InvalidRefNameException e)
			{
				NUnit.Framework.Assert.IsNotNull(e.Message);
			}
		}
	}
}
