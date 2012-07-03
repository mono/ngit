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
using NGit.Dircache;
using NGit.Revwalk;
using NGit.Treewalk;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class ResetCommandTest : RepositoryTestCase
	{
		private Git git;

		private RevCommit initialCommit;

		private RevCommit secondCommit;

		private FilePath indexFile;

		private FilePath untrackedFile;

		private DirCacheEntry prestage;

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		public virtual void SetupRepository()
		{
			// create initial commit
			git = new Git(db);
			initialCommit = git.Commit().SetMessage("initial commit").Call();
			// create nested file
			FilePath dir = new FilePath(db.WorkTree, "dir");
			FileUtils.Mkdir(dir);
			FilePath nestedFile = new FilePath(dir, "b.txt");
			FileUtils.CreateNewFile(nestedFile);
			PrintWriter nesterFileWriter = new PrintWriter(nestedFile);
			nesterFileWriter.Write("content");
			nesterFileWriter.Flush();
			// create file
			indexFile = new FilePath(db.WorkTree, "a.txt");
			FileUtils.CreateNewFile(indexFile);
			PrintWriter writer = new PrintWriter(indexFile);
			writer.Write("content");
			writer.Flush();
			// add file and commit it
			git.Add().AddFilepattern("dir").AddFilepattern("a.txt").Call();
			secondCommit = git.Commit().SetMessage("adding a.txt and dir/b.txt").Call();
			prestage = DirCache.Read(db.GetIndexFile(), db.FileSystem).GetEntry(indexFile.GetName
				());
			// modify file and add to index
			writer.Write("new content");
			writer.Close();
			nesterFileWriter.Write("new content");
			nesterFileWriter.Close();
			git.Add().AddFilepattern("a.txt").AddFilepattern("dir").Call();
			// create a file not added to the index
			untrackedFile = new FilePath(db.WorkTree, "notAddedToIndex.txt");
			FileUtils.CreateNewFile(untrackedFile);
			PrintWriter writer2 = new PrintWriter(untrackedFile);
			writer2.Write("content");
			writer2.Close();
		}

		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Errors.AmbiguousObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestHardReset()
		{
			SetupRepository();
			ObjectId prevHead = db.Resolve(Constants.HEAD);
			git.Reset().SetMode(ResetCommand.ResetType.HARD).SetRef(initialCommit.GetName()).
				Call();
			// check if HEAD points to initial commit now
			ObjectId head = db.Resolve(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(head.Equals(initialCommit));
			// check if files were removed
			NUnit.Framework.Assert.IsFalse(indexFile.Exists());
			NUnit.Framework.Assert.IsTrue(untrackedFile.Exists());
			// fileInIndex must no longer be in HEAD and in the index
			string fileInIndexPath = indexFile.GetAbsolutePath();
			NUnit.Framework.Assert.IsFalse(InHead(fileInIndexPath));
			NUnit.Framework.Assert.IsFalse(InIndex(indexFile.GetName()));
			AssertReflog(prevHead, head);
			NUnit.Framework.Assert.AreEqual(prevHead, db.ReadOrigHead());
		}

		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Errors.AmbiguousObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestResetToNonexistingHEAD()
		{
			// create a file in the working tree of a fresh repo
			git = new Git(db);
			WriteTrashFile("f", "content");
			try
			{
				git.Reset().SetRef(Constants.HEAD).Call();
				NUnit.Framework.Assert.Fail("Expected JGitInternalException didn't occur");
			}
			catch (JGitInternalException)
			{
			}
		}

		// got the expected exception
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Errors.AmbiguousObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSoftReset()
		{
			SetupRepository();
			ObjectId prevHead = db.Resolve(Constants.HEAD);
			git.Reset().SetMode(ResetCommand.ResetType.SOFT).SetRef(initialCommit.GetName()).
				Call();
			// check if HEAD points to initial commit now
			ObjectId head = db.Resolve(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(head.Equals(initialCommit));
			// check if files still exist
			NUnit.Framework.Assert.IsTrue(untrackedFile.Exists());
			NUnit.Framework.Assert.IsTrue(indexFile.Exists());
			// fileInIndex must no longer be in HEAD but has to be in the index
			string fileInIndexPath = indexFile.GetAbsolutePath();
			NUnit.Framework.Assert.IsFalse(InHead(fileInIndexPath));
			NUnit.Framework.Assert.IsTrue(InIndex(indexFile.GetName()));
			AssertReflog(prevHead, head);
			NUnit.Framework.Assert.AreEqual(prevHead, db.ReadOrigHead());
		}

		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Errors.AmbiguousObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMixedReset()
		{
			SetupRepository();
			ObjectId prevHead = db.Resolve(Constants.HEAD);
			git.Reset().SetMode(ResetCommand.ResetType.MIXED).SetRef(initialCommit.GetName())
				.Call();
			// check if HEAD points to initial commit now
			ObjectId head = db.Resolve(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(head.Equals(initialCommit));
			// check if files still exist
			NUnit.Framework.Assert.IsTrue(untrackedFile.Exists());
			NUnit.Framework.Assert.IsTrue(indexFile.Exists());
			// fileInIndex must no longer be in HEAD and in the index
			string fileInIndexPath = indexFile.GetAbsolutePath();
			NUnit.Framework.Assert.IsFalse(InHead(fileInIndexPath));
			NUnit.Framework.Assert.IsFalse(InIndex(indexFile.GetName()));
			AssertReflog(prevHead, head);
			NUnit.Framework.Assert.AreEqual(prevHead, db.ReadOrigHead());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMixedResetRetainsSizeAndModifiedTime()
		{
			git = new Git(db);
			WriteTrashFile("a.txt", "a").SetLastModified(Runtime.CurrentTimeMillis() - 60 * 1000
				);
			NUnit.Framework.Assert.IsNotNull(git.Add().AddFilepattern("a.txt").Call());
			NUnit.Framework.Assert.IsNotNull(git.Commit().SetMessage("a commit").Call());
			WriteTrashFile("b.txt", "b").SetLastModified(Runtime.CurrentTimeMillis() - 60 * 1000
				);
			NUnit.Framework.Assert.IsNotNull(git.Add().AddFilepattern("b.txt").Call());
			RevCommit commit2 = git.Commit().SetMessage("b commit").Call();
			NUnit.Framework.Assert.IsNotNull(commit2);
			DirCache cache = db.ReadDirCache();
			DirCacheEntry aEntry = cache.GetEntry("a.txt");
			NUnit.Framework.Assert.IsNotNull(aEntry);
			NUnit.Framework.Assert.IsTrue(aEntry.Length > 0);
			NUnit.Framework.Assert.IsTrue(aEntry.LastModified > 0);
			DirCacheEntry bEntry = cache.GetEntry("b.txt");
			NUnit.Framework.Assert.IsNotNull(bEntry);
			NUnit.Framework.Assert.IsTrue(bEntry.Length > 0);
			NUnit.Framework.Assert.IsTrue(bEntry.LastModified > 0);
			git.Reset().SetMode(ResetCommand.ResetType.MIXED).SetRef(commit2.GetName()).Call(
				);
			cache = db.ReadDirCache();
			DirCacheEntry mixedAEntry = cache.GetEntry("a.txt");
			NUnit.Framework.Assert.IsNotNull(mixedAEntry);
			NUnit.Framework.Assert.AreEqual(aEntry.LastModified, mixedAEntry.LastModified);
			NUnit.Framework.Assert.AreEqual(aEntry.LastModified, mixedAEntry.LastModified);
			DirCacheEntry mixedBEntry = cache.GetEntry("b.txt");
			NUnit.Framework.Assert.IsNotNull(mixedBEntry);
			NUnit.Framework.Assert.AreEqual(bEntry.LastModified, mixedBEntry.LastModified);
			NUnit.Framework.Assert.AreEqual(bEntry.LastModified, mixedBEntry.LastModified);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPathsReset()
		{
			SetupRepository();
			DirCacheEntry preReset = DirCache.Read(db.GetIndexFile(), db.FileSystem).GetEntry
				(indexFile.GetName());
			NUnit.Framework.Assert.IsNotNull(preReset);
			git.Add().AddFilepattern(untrackedFile.GetName()).Call();
			// 'a.txt' has already been modified in setupRepository
			// 'notAddedToIndex.txt' has been added to repository
			git.Reset().AddPath(indexFile.GetName()).AddPath(untrackedFile.GetName()).Call();
			DirCacheEntry postReset = DirCache.Read(db.GetIndexFile(), db.FileSystem).GetEntry
				(indexFile.GetName());
			NUnit.Framework.Assert.IsNotNull(postReset);
			NUnit.Framework.Assert.AreNotSame(preReset.GetObjectId(), postReset.GetObjectId()
				);
			NUnit.Framework.Assert.AreEqual(prestage.GetObjectId(), postReset.GetObjectId());
			// check that HEAD hasn't moved
			ObjectId head = db.Resolve(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(head.Equals(secondCommit));
			// check if files still exist
			NUnit.Framework.Assert.IsTrue(untrackedFile.Exists());
			NUnit.Framework.Assert.IsTrue(indexFile.Exists());
			NUnit.Framework.Assert.IsTrue(InHead(indexFile.GetName()));
			NUnit.Framework.Assert.IsTrue(InIndex(indexFile.GetName()));
			NUnit.Framework.Assert.IsFalse(InIndex(untrackedFile.GetName()));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPathsResetOnDirs()
		{
			SetupRepository();
			DirCacheEntry preReset = DirCache.Read(db.GetIndexFile(), db.FileSystem).GetEntry
				("dir/b.txt");
			NUnit.Framework.Assert.IsNotNull(preReset);
			git.Add().AddFilepattern(untrackedFile.GetName()).Call();
			// 'dir/b.txt' has already been modified in setupRepository
			git.Reset().AddPath("dir").Call();
			DirCacheEntry postReset = DirCache.Read(db.GetIndexFile(), db.FileSystem).GetEntry
				("dir/b.txt");
			NUnit.Framework.Assert.IsNotNull(postReset);
			NUnit.Framework.Assert.AreNotSame(preReset.GetObjectId(), postReset.GetObjectId()
				);
			// check that HEAD hasn't moved
			ObjectId head = db.Resolve(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(head.Equals(secondCommit));
			// check if files still exist
			NUnit.Framework.Assert.IsTrue(untrackedFile.Exists());
			NUnit.Framework.Assert.IsTrue(InHead("dir/b.txt"));
			NUnit.Framework.Assert.IsTrue(InIndex("dir/b.txt"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPathsResetWithRef()
		{
			SetupRepository();
			DirCacheEntry preReset = DirCache.Read(db.GetIndexFile(), db.FileSystem).GetEntry
				(indexFile.GetName());
			NUnit.Framework.Assert.IsNotNull(preReset);
			git.Add().AddFilepattern(untrackedFile.GetName()).Call();
			// 'a.txt' has already been modified in setupRepository
			// 'notAddedToIndex.txt' has been added to repository
			// reset to the inital commit
			git.Reset().SetRef(initialCommit.GetName()).AddPath(indexFile.GetName()).AddPath(
				untrackedFile.GetName()).Call();
			// check that HEAD hasn't moved
			ObjectId head = db.Resolve(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(head.Equals(secondCommit));
			// check if files still exist
			NUnit.Framework.Assert.IsTrue(untrackedFile.Exists());
			NUnit.Framework.Assert.IsTrue(indexFile.Exists());
			NUnit.Framework.Assert.IsTrue(InHead(indexFile.GetName()));
			NUnit.Framework.Assert.IsFalse(InIndex(indexFile.GetName()));
			NUnit.Framework.Assert.IsFalse(InIndex(untrackedFile.GetName()));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestHardResetOnTag()
		{
			SetupRepository();
			string tagName = "initialtag";
			git.Tag().SetName(tagName).SetObjectId(secondCommit).SetMessage("message").Call();
			DirCacheEntry preReset = DirCache.Read(db.GetIndexFile(), db.FileSystem).GetEntry
				(indexFile.GetName());
			NUnit.Framework.Assert.IsNotNull(preReset);
			git.Add().AddFilepattern(untrackedFile.GetName()).Call();
			git.Reset().SetRef(tagName).SetMode(ResetCommand.ResetType.HARD).Call();
			ObjectId head = db.Resolve(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(head.Equals(secondCommit));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestHardResetAfterSquashMerge()
		{
			Git g = new Git(db);
			WriteTrashFile("file1", "file1");
			g.Add().AddFilepattern("file1").Call();
			RevCommit first = g.Commit().SetMessage("initial commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			CreateBranch(first, "refs/heads/branch1");
			CheckoutBranch("refs/heads/branch1");
			WriteTrashFile("file2", "file2");
			g.Add().AddFilepattern("file2").Call();
			g.Commit().SetMessage("second commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			CheckoutBranch("refs/heads/master");
			MergeCommandResult result = g.Merge().Include(db.GetRef("branch1")).SetSquash(true
				).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAST_FORWARD_SQUASHED, result.GetMergeStatus
				());
			NUnit.Framework.Assert.IsNotNull(db.ReadSquashCommitMsg());
			g.Reset().SetMode(ResetCommand.ResetType.HARD).SetRef(first.GetName()).Call();
			NUnit.Framework.Assert.IsNull(db.ReadSquashCommitMsg());
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertReflog(ObjectId prevHead, ObjectId head)
		{
			// Check the reflog for HEAD
			string actualHeadMessage = db.GetReflogReader(Constants.HEAD).GetLastEntry().GetComment
				();
			string expectedHeadMessage = head.GetName() + ": updating HEAD";
			NUnit.Framework.Assert.AreEqual(expectedHeadMessage, actualHeadMessage);
			NUnit.Framework.Assert.AreEqual(head.GetName(), db.GetReflogReader(Constants.HEAD
				).GetLastEntry().GetNewId().GetName());
			NUnit.Framework.Assert.AreEqual(prevHead.GetName(), db.GetReflogReader(Constants.
				HEAD).GetLastEntry().GetOldId().GetName());
			// The reflog for master contains the same as the one for HEAD
			string actualMasterMessage = db.GetReflogReader("refs/heads/master").GetLastEntry
				().GetComment();
			string expectedMasterMessage = head.GetName() + ": updating HEAD";
			// yes!
			NUnit.Framework.Assert.AreEqual(expectedMasterMessage, actualMasterMessage);
			NUnit.Framework.Assert.AreEqual(head.GetName(), db.GetReflogReader(Constants.HEAD
				).GetLastEntry().GetNewId().GetName());
			NUnit.Framework.Assert.AreEqual(prevHead.GetName(), db.GetReflogReader("refs/heads/master"
				).GetLastEntry().GetOldId().GetName());
		}

		/// <summary>Checks if a file with the given path exists in the HEAD tree</summary>
		/// <param name="path"></param>
		/// <returns>true if the file exists</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		private bool InHead(string path)
		{
			ObjectId headId = db.Resolve(Constants.HEAD);
			RevWalk rw = new RevWalk(db);
			TreeWalk tw = null;
			try
			{
				tw = TreeWalk.ForPath(db, path, rw.ParseTree(headId));
				return tw != null;
			}
			finally
			{
				rw.Release();
				rw.Dispose();
				if (tw != null)
				{
					tw.Release();
				}
			}
		}

		/// <summary>Checks if a file with the given path exists in the index</summary>
		/// <param name="path"></param>
		/// <returns>true if the file exists</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		private bool InIndex(string path)
		{
			DirCache dc = DirCache.Read(db.GetIndexFile(), db.FileSystem);
			return dc.GetEntry(path) != null;
		}
	}
}
