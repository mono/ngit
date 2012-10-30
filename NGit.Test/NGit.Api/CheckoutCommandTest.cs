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
using System.IO;
using NGit;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Dircache;
using NGit.Revwalk;
using NGit.Transport;
using NGit.Util;
using Sharpen;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class CheckoutCommandTest : RepositoryTestCase
	{
		private Git git;

		internal RevCommit initialCommit;

		internal RevCommit secondCommit;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			git = new Git(db);
			// commit something
			WriteTrashFile("Test.txt", "Hello world");
			git.Add().AddFilepattern("Test.txt").Call();
			initialCommit = git.Commit().SetMessage("Initial commit").Call();
			// create a master branch and switch to it
			git.BranchCreate().SetName("test").Call();
			RefUpdate rup = db.UpdateRef(Constants.HEAD);
			rup.Link("refs/heads/test");
			// commit something on the test branch
			WriteTrashFile("Test.txt", "Some change");
			git.Add().AddFilepattern("Test.txt").Call();
			secondCommit = git.Commit().SetMessage("Second commit").Call();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSimpleCheckout()
		{
			git.Checkout().SetName("test").Call();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckout()
		{
			git.Checkout().SetName("test").Call();
			NUnit.Framework.Assert.AreEqual("[Test.txt, mode:100644, content:Some change]", IndexState
				(CONTENT));
			Ref result = git.Checkout().SetName("master").Call();
			NUnit.Framework.Assert.AreEqual("[Test.txt, mode:100644, content:Hello world]", IndexState
				(CONTENT));
			NUnit.Framework.Assert.AreEqual("refs/heads/master", result.GetName());
			NUnit.Framework.Assert.AreEqual("refs/heads/master", git.GetRepository().GetFullBranch
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCreateBranchOnCheckout()
		{
			git.Checkout().SetCreateBranch(true).SetName("test2").Call();
			NUnit.Framework.Assert.IsNotNull(db.GetRef("test2"));
		}

		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutToNonExistingBranch()
		{
			try
			{
				git.Checkout().SetName("badbranch").Call();
				NUnit.Framework.Assert.Fail("Should have failed");
			}
			catch (RefNotFoundException)
			{
			}
		}

		// except to hit here
		[NUnit.Framework.Test]
		public virtual void TestCheckoutWithConflict()
		{
			CheckoutCommand co = git.Checkout();
			try
			{
				WriteTrashFile("Test.txt", "Another change");
				NUnit.Framework.Assert.AreEqual(CheckoutResult.Status.NOT_TRIED, co.GetResult().GetStatus
					());
				co.SetName("master").Call();
				NUnit.Framework.Assert.Fail("Should have failed");
			}
			catch (Exception)
			{
				NUnit.Framework.Assert.AreEqual(CheckoutResult.Status.CONFLICTS, co.GetResult().GetStatus
					());
				NUnit.Framework.Assert.IsTrue(co.GetResult().GetConflictList().Contains("Test.txt"
					));
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutWithNonDeletedFiles()
		{
			FilePath testFile = WriteTrashFile("temp", string.Empty);
			FileInputStream fis = new FileInputStream(testFile);
			try
			{
				FileUtils.Delete(testFile);
				return;
			}
			catch (IOException)
			{
			}
			finally
			{
				// the test makes only sense if deletion of
				// a file with open stream fails
				fis.Close();
			}
			FileUtils.Delete(testFile);
			CheckoutCommand co = git.Checkout();
			// delete Test.txt in branch test
			testFile = new FilePath(db.WorkTree, "Test.txt");
			NUnit.Framework.Assert.IsTrue(testFile.Exists());
			FileUtils.Delete(testFile);
			NUnit.Framework.Assert.IsFalse(testFile.Exists());
			git.Add().AddFilepattern("Test.txt");
			git.Commit().SetMessage("Delete Test.txt").SetAll(true).Call();
			git.Checkout().SetName("master").Call();
			NUnit.Framework.Assert.IsTrue(testFile.Exists());
			// lock the file so it can't be deleted (in Windows, that is)
			fis = new FileInputStream(testFile);
			try
			{
				NUnit.Framework.Assert.AreEqual(CheckoutResult.Status.NOT_TRIED, co.GetResult().GetStatus
					());
				co.SetName("test").Call();
				NUnit.Framework.Assert.IsTrue(testFile.Exists());
				NUnit.Framework.Assert.AreEqual(CheckoutResult.Status.NONDELETED, co.GetResult().
					GetStatus());
				NUnit.Framework.Assert.IsTrue(co.GetResult().GetUndeletedList().Contains("Test.txt"
					));
			}
			finally
			{
				fis.Close();
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutCommit()
		{
			Ref result = git.Checkout().SetName(initialCommit.Name).Call();
			NUnit.Framework.Assert.AreEqual("[Test.txt, mode:100644, content:Hello world]", IndexState
				(CONTENT));
			NUnit.Framework.Assert.IsNull(result);
			NUnit.Framework.Assert.AreEqual(initialCommit.Name, git.GetRepository().GetFullBranch
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutRemoteTrackingWithoutLocalBranch()
		{
			// create second repository
			Repository db2 = CreateWorkRepository();
			Git git2 = new Git(db2);
			// setup the second repository to fetch from the first repository
			StoredConfig config = db2.GetConfig();
			RemoteConfig remoteConfig = new RemoteConfig(config, "origin");
			URIish uri = new URIish(db.Directory.ToURI().ToURL());
			remoteConfig.AddURI(uri);
			remoteConfig.Update(config);
			config.Save();
			// fetch from first repository
			RefSpec spec = new RefSpec("+refs/heads/*:refs/remotes/origin/*");
			git2.Fetch().SetRemote("origin").SetRefSpecs(spec).Call();
			// checkout remote tracking branch in second repository
			// (no local branches exist yet in second repository)
			git2.Checkout().SetName("remotes/origin/test").Call();
			NUnit.Framework.Assert.AreEqual("[Test.txt, mode:100644, content:Some change]", IndexState
				(db2, CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutOfFileWithInexistentParentDir()
		{
			FilePath a = WriteTrashFile("dir/a.txt", "A");
			WriteTrashFile("dir/b.txt", "A");
			git.Add().AddFilepattern("dir/a.txt").AddFilepattern("dir/b.txt").Call();
			git.Commit().SetMessage("Added dir").Call();
			FilePath dir = new FilePath(db.WorkTree, "dir");
			FileUtils.Delete(dir, FileUtils.RECURSIVE);
			git.Checkout().AddPath("dir/a.txt").Call();
			NUnit.Framework.Assert.IsTrue(a.Exists());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutOfDirectoryShouldBeRecursive()
		{
			FilePath a = WriteTrashFile("dir/a.txt", "A");
			FilePath b = WriteTrashFile("dir/sub/b.txt", "B");
			git.Add().AddFilepattern("dir").Call();
			git.Commit().SetMessage("Added dir").Call();
			Write(a, "modified");
			Write(b, "modified");
			git.Checkout().AddPath("dir").Call();
			NUnit.Framework.Assert.AreEqual(Read(a), "A");
			NUnit.Framework.Assert.AreEqual(Read(b), "B");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutAllPaths()
		{
			FilePath a = WriteTrashFile("dir/a.txt", "A");
			FilePath b = WriteTrashFile("dir/sub/b.txt", "B");
			git.Add().AddFilepattern("dir").Call();
			git.Commit().SetMessage("Added dir").Call();
			Write(a, "modified");
			Write(b, "modified");
			git.Checkout().SetAllPaths(true).Call();
			NUnit.Framework.Assert.AreEqual(Read(a), "A");
			NUnit.Framework.Assert.AreEqual(Read(b), "B");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutWithStartPoint()
		{
			FilePath a = WriteTrashFile("a.txt", "A");
			git.Add().AddFilepattern("a.txt").Call();
			RevCommit first = git.Commit().SetMessage("Added a").Call();
			Write(a, "other");
			git.Commit().SetAll(true).SetMessage("Other").Call();
			git.Checkout().SetCreateBranch(true).SetName("a").SetStartPoint(first.Id.GetName(
				)).Call();
			NUnit.Framework.Assert.AreEqual(Read(a), "A");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutWithStartPointOnlyCertainFiles()
		{
			FilePath a = WriteTrashFile("a.txt", "A");
			FilePath b = WriteTrashFile("b.txt", "B");
			git.Add().AddFilepattern("a.txt").AddFilepattern("b.txt").Call();
			RevCommit first = git.Commit().SetMessage("First").Call();
			Write(a, "other");
			Write(b, "other");
			git.Commit().SetAll(true).SetMessage("Other").Call();
			git.Checkout().SetCreateBranch(true).SetName("a").SetStartPoint(first.Id.GetName(
				)).AddPath("a.txt").Call();
			NUnit.Framework.Assert.AreEqual(Read(a), "A");
			NUnit.Framework.Assert.AreEqual(Read(b), "other");
		}

		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDetachedHeadOnCheckout()
		{
			CheckoutCommand co = git.Checkout();
			co.SetName("master").Call();
			string commitId = db.GetRef(Constants.MASTER).GetObjectId().Name;
			co = git.Checkout();
			co.SetName(commitId).Call();
			Ref head = db.GetRef(Constants.HEAD);
			NUnit.Framework.Assert.IsFalse(head.IsSymbolic());
			NUnit.Framework.Assert.AreSame(head, head.GetTarget());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateSmudgedEntries()
		{
			git.BranchCreate().SetName("test2").Call();
			RefUpdate rup = db.UpdateRef(Constants.HEAD);
			rup.Link("refs/heads/test2");
			FilePath file = new FilePath(db.WorkTree, "Test.txt");
			long size = file.Length();
			long mTime = file.LastModified() - 5000L;
			NUnit.Framework.Assert.IsTrue(file.SetLastModified(mTime));
			DirCache cache = DirCache.Lock(db.GetIndexFile(), db.FileSystem);
			DirCacheEntry entry = cache.GetEntry("Test.txt");
			NUnit.Framework.Assert.IsNotNull(entry);
			entry.SetLength(0);
			entry.LastModified = 0;
			cache.Write();
			NUnit.Framework.Assert.IsTrue(cache.Commit());
			cache = DirCache.Read(db.GetIndexFile(), db.FileSystem);
			entry = cache.GetEntry("Test.txt");
			NUnit.Framework.Assert.IsNotNull(entry);
			NUnit.Framework.Assert.AreEqual(0, entry.Length);
			NUnit.Framework.Assert.AreEqual(0, entry.LastModified);
			db.GetIndexFile().SetLastModified(db.GetIndexFile().LastModified() - 5000);
			NUnit.Framework.Assert.IsNotNull(git.Checkout().SetName("test").Call());
			cache = DirCache.Read(db.GetIndexFile(), db.FileSystem);
			entry = cache.GetEntry("Test.txt");
			NUnit.Framework.Assert.IsNotNull(entry);
			NUnit.Framework.Assert.AreEqual(size, entry.Length);
			NUnit.Framework.Assert.AreEqual(mTime, entry.LastModified);
		}
	}
}
