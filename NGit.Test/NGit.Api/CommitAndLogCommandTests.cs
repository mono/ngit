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
using NGit;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Treewalk;
using NGit.Util;
using Sharpen;

namespace NGit.Api
{
	/// <summary>Testing the git commit and log commands</summary>
	[NUnit.Framework.TestFixture]
	public class CommitAndLogCommandTests : RepositoryTestCase
	{
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSomeCommits()
		{
			// do 4 commits
			Git git = new Git(db);
			git.Commit().SetMessage("initial commit").Call();
			git.Commit().SetMessage("second commit").SetCommitter(committer).Call();
			git.Commit().SetMessage("third commit").SetAuthor(author).Call();
			git.Commit().SetMessage("fourth commit").SetAuthor(author).SetCommitter(committer
				).Call();
			Iterable<RevCommit> commits = git.Log().Call();
			// check that all commits came in correctly
			PersonIdent defaultCommitter = new PersonIdent(db);
			PersonIdent[] expectedAuthors = new PersonIdent[] { defaultCommitter, committer, 
				author, author };
			PersonIdent[] expectedCommitters = new PersonIdent[] { defaultCommitter, committer
				, defaultCommitter, committer };
			string[] expectedMessages = new string[] { "initial commit", "second commit", "third commit"
				, "fourth commit" };
			int l = expectedAuthors.Length - 1;
			foreach (RevCommit c in commits)
			{
				NUnit.Framework.Assert.AreEqual(expectedAuthors[l].GetName(), c.GetAuthorIdent().
					GetName());
				NUnit.Framework.Assert.AreEqual(expectedCommitters[l].GetName(), c.GetCommitterIdent
					().GetName());
				NUnit.Framework.Assert.AreEqual(c.GetFullMessage(), expectedMessages[l]);
				l--;
			}
			NUnit.Framework.Assert.AreEqual(l, -1);
			ReflogReader reader = db.GetReflogReader(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(reader.GetLastEntry().GetComment().StartsWith("commit:"
				));
			reader = db.GetReflogReader(db.GetBranch());
			NUnit.Framework.Assert.IsTrue(reader.GetLastEntry().GetComment().StartsWith("commit:"
				));
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLogWithFilter()
		{
			Git git = new Git(db);
			// create first file
			FilePath file = new FilePath(db.WorkTree, "a.txt");
			FileUtils.CreateNewFile(file);
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content1");
			writer.Close();
			// First commit - a.txt file
			git.Add().AddFilepattern("a.txt").Call();
			git.Commit().SetMessage("commit1").SetCommitter(committer).Call();
			// create second file
			file = new FilePath(db.WorkTree, "b.txt");
			FileUtils.CreateNewFile(file);
			writer = new PrintWriter(file);
			writer.Write("content2");
			writer.Close();
			// Second commit - b.txt file
			git.Add().AddFilepattern("b.txt").Call();
			git.Commit().SetMessage("commit2").SetCommitter(committer).Call();
			// First log - a.txt filter
			int count = 0;
			foreach (RevCommit c in git.Log().AddPath("a.txt").Call())
			{
				NUnit.Framework.Assert.AreEqual("commit1", c.GetFullMessage());
				count++;
			}
			NUnit.Framework.Assert.AreEqual(1, count);
			// Second log - b.txt filter
			count = 0;
			foreach (RevCommit c_1 in git.Log().AddPath("b.txt").Call())
			{
				NUnit.Framework.Assert.AreEqual("commit2", c_1.GetFullMessage());
				count++;
			}
			NUnit.Framework.Assert.AreEqual(1, count);
			// Third log - without filter
			count = 0;
			foreach (RevCommit c_2 in git.Log().Call())
			{
				NUnit.Framework.Assert.AreEqual(committer, c_2.GetCommitterIdent());
				count++;
			}
			NUnit.Framework.Assert.AreEqual(2, count);
		}

		// try to do a commit without specifying a message. Should fail!
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWrongParams()
		{
			Git git = new Git(db);
			try
			{
				git.Commit().SetAuthor(author).Call();
				NUnit.Framework.Assert.Fail("Didn't get the expected exception");
			}
			catch (NoMessageException)
			{
			}
		}

		// expected
		// try to work with Commands after command has been invoked. Should throw
		// exceptions
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMultipleInvocations()
		{
			Git git = new Git(db);
			CommitCommand commitCmd = git.Commit();
			commitCmd.SetMessage("initial commit").Call();
			try
			{
				// check that setters can't be called after invocation
				commitCmd.SetAuthor(author);
				NUnit.Framework.Assert.Fail("didn't catch the expected exception");
			}
			catch (InvalidOperationException)
			{
			}
			// expected
			LogCommand logCmd = git.Log();
			logCmd.Call();
			try
			{
				// check that call can't be called twice
				logCmd.Call();
				NUnit.Framework.Assert.Fail("didn't catch the expected exception");
			}
			catch (InvalidOperationException)
			{
			}
		}

		// expected
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMergeEmptyBranches()
		{
			if (!FS.DETECTED.SupportsExecute())
				return;

			Git git = new Git(db);
			git.Commit().SetMessage("initial commit").Call();
			RefUpdate r = db.UpdateRef("refs/heads/side");
			r.SetNewObjectId(db.Resolve(Constants.HEAD));
			NUnit.Framework.Assert.AreEqual(r.ForceUpdate(), RefUpdate.Result.NEW);
			RevCommit second = git.Commit().SetMessage("second commit").SetCommitter(committer
				).Call();
			db.UpdateRef(Constants.HEAD).Link("refs/heads/side");
			RevCommit firstSide = git.Commit().SetMessage("first side commit").SetAuthor(author
				).Call();
			Write(new FilePath(db.Directory, Constants.MERGE_HEAD), ObjectId.ToString(db.Resolve
				("refs/heads/master")));
			Write(new FilePath(db.Directory, Constants.MERGE_MSG), "merging");
			RevCommit commit = git.Commit().Call();
			RevCommit[] parents = commit.Parents;
			NUnit.Framework.Assert.AreEqual(parents[0], firstSide);
			NUnit.Framework.Assert.AreEqual(parents[1], second);
			NUnit.Framework.Assert.AreEqual(2, parents.Length);
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddUnstagedChanges()
		{
			FilePath file = new FilePath(db.WorkTree, "a.txt");
			FileUtils.CreateNewFile(file);
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			Git git = new Git(db);
			git.Add().AddFilepattern("a.txt").Call();
			RevCommit commit = git.Commit().SetMessage("initial commit").Call();
			TreeWalk tw = TreeWalk.ForPath(db, "a.txt", commit.Tree);
			NUnit.Framework.Assert.AreEqual("6b584e8ece562ebffc15d38808cd6b98fc3d97ea", tw.GetObjectId
				(0).GetName());
			writer = new PrintWriter(file);
			writer.Write("content2");
			writer.Close();
			commit = git.Commit().SetMessage("second commit").Call();
			tw = TreeWalk.ForPath(db, "a.txt", commit.Tree);
			NUnit.Framework.Assert.AreEqual("6b584e8ece562ebffc15d38808cd6b98fc3d97ea", tw.GetObjectId
				(0).GetName());
			commit = git.Commit().SetAll(true).SetMessage("third commit").SetAll(true).Call();
			tw = TreeWalk.ForPath(db, "a.txt", commit.Tree);
			NUnit.Framework.Assert.AreEqual("db00fd65b218578127ea51f3dffac701f12f486a", tw.GetObjectId
				(0).GetName());
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestModeChange()
		{
			if (Runtime.GetProperty("os.name").StartsWith("Windows"))
			{
				return;
			}
			// SKIP
			Git git = new Git(db);
			// create file
			FilePath file = new FilePath(db.WorkTree, "a.txt");
			FileUtils.CreateNewFile(file);
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content1");
			writer.Close();
			// First commit - a.txt file
			git.Add().AddFilepattern("a.txt").Call();
			git.Commit().SetMessage("commit1").SetCommitter(committer).Call();
			// pure mode change should be committable
			FS fs = db.FileSystem;
			fs.SetExecute(file, true);
			git.Add().AddFilepattern("a.txt").Call();
			git.Commit().SetMessage("mode change").SetCommitter(committer).Call();
			// pure mode change should be committable with -o option
			fs.SetExecute(file, false);
			git.Add().AddFilepattern("a.txt").Call();
			git.Commit().SetMessage("mode change").SetCommitter(committer).SetOnly("a.txt").Call
				();
		}

		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCommitRange()
		{
			// do 4 commits and set the range to the second and fourth one
			Git git = new Git(db);
			git.Commit().SetMessage("first commit").Call();
			RevCommit second = git.Commit().SetMessage("second commit").SetCommitter(committer
				).Call();
			git.Commit().SetMessage("third commit").SetAuthor(author).Call();
			RevCommit last = git.Commit().SetMessage("fourth commit").SetAuthor(author).SetCommitter
				(committer).Call();
			Iterable<RevCommit> commits = git.Log().AddRange(second.Id, last.Id).Call();
			// check that we have the third and fourth commit
			PersonIdent defaultCommitter = new PersonIdent(db);
			PersonIdent[] expectedAuthors = new PersonIdent[] { author, author };
			PersonIdent[] expectedCommitters = new PersonIdent[] { defaultCommitter, committer
				 };
			string[] expectedMessages = new string[] { "third commit", "fourth commit" };
			int l = expectedAuthors.Length - 1;
			foreach (RevCommit c in commits)
			{
				NUnit.Framework.Assert.AreEqual(expectedAuthors[l].GetName(), c.GetAuthorIdent().
					GetName());
				NUnit.Framework.Assert.AreEqual(expectedCommitters[l].GetName(), c.GetCommitterIdent
					().GetName());
				NUnit.Framework.Assert.AreEqual(c.GetFullMessage(), expectedMessages[l]);
				l--;
			}
			NUnit.Framework.Assert.AreEqual(l, -1);
		}

		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCommitAmend()
		{
			Git git = new Git(db);
			git.Commit().SetMessage("first comit").Call();
			// typo
			git.Commit().SetAmend(true).SetMessage("first commit").Call();
			Iterable<RevCommit> commits = git.Log().Call();
			int c = 0;
			foreach (RevCommit commit in commits)
			{
				NUnit.Framework.Assert.AreEqual("first commit", commit.GetFullMessage());
				c++;
			}
			NUnit.Framework.Assert.AreEqual(1, c);
			ReflogReader reader = db.GetReflogReader(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(reader.GetLastEntry().GetComment().StartsWith("commit (amend):"
				));
			reader = db.GetReflogReader(db.GetBranch());
			NUnit.Framework.Assert.IsTrue(reader.GetLastEntry().GetComment().StartsWith("commit (amend):"
				));
		}

		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestInsertChangeId()
		{
			Git git = new Git(db);
			string messageHeader = "Some header line\n\nSome detail explanation\n";
			string changeIdTemplate = "\nChange-Id: I" + ObjectId.ZeroId.GetName() + "\n";
			string messageFooter = "Some foooter lines\nAnother footer line\n";
			RevCommit commit = git.Commit().SetMessage(messageHeader + messageFooter).SetInsertChangeId
				(true).Call();
			// we should find a real change id (at the end of the file)
			byte[] chars = Sharpen.Runtime.GetBytesForString(commit.GetFullMessage());
			int lastLineBegin = RawParseUtils.PrevLF(chars, chars.Length - 2);
			string lastLine = RawParseUtils.Decode(chars, lastLineBegin + 1, chars.Length);
			NUnit.Framework.Assert.IsTrue(lastLine.Contains("Change-Id:"));
			NUnit.Framework.Assert.IsFalse(lastLine.Contains("Change-Id: I" + ObjectId.ZeroId
				.GetName()));
			commit = git.Commit().SetMessage(messageHeader + changeIdTemplate + messageFooter
				).SetInsertChangeId(true).Call();
			// we should find a real change id (in the line as dictated by the
			// template)
			chars = Sharpen.Runtime.GetBytesForString(commit.GetFullMessage());
			int lineStart = 0;
			int lineEnd = 0;
			for (int i = 0; i < 4; i++)
			{
				lineStart = RawParseUtils.NextLF(chars, lineStart);
			}
			lineEnd = RawParseUtils.NextLF(chars, lineStart);
			string line = RawParseUtils.Decode(chars, lineStart, lineEnd);
			NUnit.Framework.Assert.IsTrue(line.Contains("Change-Id:"));
			NUnit.Framework.Assert.IsFalse(line.Contains("Change-Id: I" + ObjectId.ZeroId.GetName
				()));
			commit = git.Commit().SetMessage(messageHeader + changeIdTemplate + messageFooter
				).SetInsertChangeId(false).Call();
			// we should find the untouched template
			chars = Sharpen.Runtime.GetBytesForString(commit.GetFullMessage());
			lineStart = 0;
			lineEnd = 0;
			for (int i_1 = 0; i_1 < 4; i_1++)
			{
				lineStart = RawParseUtils.NextLF(chars, lineStart);
			}
			lineEnd = RawParseUtils.NextLF(chars, lineStart);
			line = RawParseUtils.Decode(chars, lineStart, lineEnd);
			NUnit.Framework.Assert.IsTrue(commit.GetFullMessage().Contains("Change-Id: I" + ObjectId
				.ZeroId.GetName()));
		}
	}
}
