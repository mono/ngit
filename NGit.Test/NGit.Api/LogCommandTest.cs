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
	[NUnit.Framework.TestFixture]
	public class LogCommandTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void LogAllCommits()
		{
			IList<RevCommit> commits = new AList<RevCommit>();
			Git git = Git.Wrap(db);
			WriteTrashFile("Test.txt", "Hello world");
			git.Add().AddFilepattern("Test.txt").Call();
			commits.AddItem(git.Commit().SetMessage("initial commit").Call());
			git.BranchCreate().SetName("branch1").Call();
			Ref checkedOut = git.Checkout().SetName("branch1").Call();
			NUnit.Framework.Assert.AreEqual("refs/heads/branch1", checkedOut.GetName());
			WriteTrashFile("Test1.txt", "Hello world!");
			git.Add().AddFilepattern("Test1.txt").Call();
			commits.AddItem(git.Commit().SetMessage("branch1 commit").Call());
			checkedOut = git.Checkout().SetName("master").Call();
			NUnit.Framework.Assert.AreEqual("refs/heads/master", checkedOut.GetName());
			WriteTrashFile("Test2.txt", "Hello world!!");
			git.Add().AddFilepattern("Test2.txt").Call();
			commits.AddItem(git.Commit().SetMessage("branch1 commit").Call());
			Iterator<RevCommit> log = git.Log().All().Call().Iterator();
			NUnit.Framework.Assert.IsTrue(log.HasNext());
			NUnit.Framework.Assert.IsTrue(commits.Contains(log.Next()));
			NUnit.Framework.Assert.IsTrue(log.HasNext());
			NUnit.Framework.Assert.IsTrue(commits.Contains(log.Next()));
			NUnit.Framework.Assert.IsTrue(log.HasNext());
			NUnit.Framework.Assert.IsTrue(commits.Contains(log.Next()));
			NUnit.Framework.Assert.IsFalse(log.HasNext());
		}

		/// <exception cref="System.Exception"></exception>
		private IList<RevCommit> CreateCommits(Git git)
		{
			IList<RevCommit> commits = new AList<RevCommit>();
			WriteTrashFile("Test.txt", "Hello world");
			git.Add().AddFilepattern("Test.txt").Call();
			commits.AddItem(git.Commit().SetMessage("commit#1").Call());
			WriteTrashFile("Test1.txt", "Hello world!");
			git.Add().AddFilepattern("Test1.txt").Call();
			commits.AddItem(git.Commit().SetMessage("commit#2").Call());
			WriteTrashFile("Test2.txt", "Hello world!!");
			git.Add().AddFilepattern("Test2.txt").Call();
			commits.AddItem(git.Commit().SetMessage("commit#3").Call());
			return commits;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void LogAllCommitsWithMaxCount()
		{
			Git git = Git.Wrap(db);
			IList<RevCommit> commits = CreateCommits(git);
			Iterator<RevCommit> log = git.Log().All().SetMaxCount(2).Call().Iterator();
			NUnit.Framework.Assert.IsTrue(log.HasNext());
			RevCommit commit = log.Next();
			NUnit.Framework.Assert.IsTrue(commits.Contains(commit));
			NUnit.Framework.Assert.AreEqual("commit#3", commit.GetShortMessage());
			NUnit.Framework.Assert.IsTrue(log.HasNext());
			commit = log.Next();
			NUnit.Framework.Assert.IsTrue(commits.Contains(commit));
			NUnit.Framework.Assert.AreEqual("commit#2", commit.GetShortMessage());
			NUnit.Framework.Assert.IsFalse(log.HasNext());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void LogAllCommitsWithSkip()
		{
			Git git = Git.Wrap(db);
			IList<RevCommit> commits = CreateCommits(git);
			Iterator<RevCommit> log = git.Log().All().SetSkip(1).Call().Iterator();
			NUnit.Framework.Assert.IsTrue(log.HasNext());
			RevCommit commit = log.Next();
			NUnit.Framework.Assert.IsTrue(commits.Contains(commit));
			NUnit.Framework.Assert.AreEqual("commit#2", commit.GetShortMessage());
			NUnit.Framework.Assert.IsTrue(log.HasNext());
			commit = log.Next();
			NUnit.Framework.Assert.IsTrue(commits.Contains(commit));
			NUnit.Framework.Assert.AreEqual("commit#1", commit.GetShortMessage());
			NUnit.Framework.Assert.IsFalse(log.HasNext());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void LogAllCommitsWithSkipAndMaxCount()
		{
			Git git = Git.Wrap(db);
			IList<RevCommit> commits = CreateCommits(git);
			Iterator<RevCommit> log = git.Log().All().SetSkip(1).SetMaxCount(1).Call().Iterator
				();
			NUnit.Framework.Assert.IsTrue(log.HasNext());
			RevCommit commit = log.Next();
			NUnit.Framework.Assert.IsTrue(commits.Contains(commit));
			NUnit.Framework.Assert.AreEqual("commit#2", commit.GetShortMessage());
			NUnit.Framework.Assert.IsFalse(log.HasNext());
		}
	}
}
