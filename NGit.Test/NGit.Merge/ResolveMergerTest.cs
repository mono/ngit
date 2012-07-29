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
using NGit.Merge;
using NGit.Revwalk;
using NGit.Treewalk;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Merge
{
	[NUnit.Framework.TestFixture]
	public class ResolveMergerTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void FailingPathsShouldNotResultInOKReturnValue()
		{
			FilePath folder1 = new FilePath(db.WorkTree, "folder1");
			FileUtils.Mkdir(folder1);
			FilePath file = new FilePath(folder1, "file1.txt");
			Write(file, "folder1--file1.txt");
			file = new FilePath(folder1, "file2.txt");
			Write(file, "folder1--file2.txt");
			Git git = new Git(db);
			git.Add().AddFilepattern(folder1.GetName()).Call();
			RevCommit @base = git.Commit().SetMessage("adding folder").Call();
			RecursiveDelete(folder1);
			git.Rm().AddFilepattern("folder1/file1.txt").AddFilepattern("folder1/file2.txt").
				Call();
			RevCommit other = git.Commit().SetMessage("removing folders on 'other'").Call();
			git.Checkout().SetName(@base.Name).Call();
			file = new FilePath(db.WorkTree, "unrelated.txt");
			Write(file, "unrelated");
			git.Add().AddFilepattern("unrelated").Call();
			RevCommit head = git.Commit().SetMessage("Adding another file").Call();
			// Untracked file to cause failing path for delete() of folder1
			file = new FilePath(folder1, "file3.txt");
			Write(file, "folder1--file3.txt");
			ResolveMerger merger = new ResolveMerger(db, false);
			merger.SetCommitNames(new string[] { "BASE", "HEAD", "other" });
			merger.SetWorkingTreeIterator(new FileTreeIterator(db));
			bool ok = merger.Merge(head.Id, other.Id);
			NUnit.Framework.Assert.IsFalse(merger.GetFailingPaths().IsEmpty());
			NUnit.Framework.Assert.IsFalse(ok);
		}

		/// <summary>
		/// Merging two conflicting subtrees when the index does not contain any file
		/// in that subtree should lead to a conflicting state.
		/// </summary>
		/// <remarks>
		/// Merging two conflicting subtrees when the index does not contain any file
		/// in that subtree should lead to a conflicting state.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void CheckMergeConflictingTreesWithoutIndex()
		{
			Git git = Git.Wrap(db);
			WriteTrashFile("d/1", "orig");
			git.Add().AddFilepattern("d/1").Call();
			RevCommit first = git.Commit().SetMessage("added d/1").Call();
			WriteTrashFile("d/1", "master");
			RevCommit masterCommit = git.Commit().SetAll(true).SetMessage("modified d/1 on master"
				).Call();
			git.Checkout().SetCreateBranch(true).SetStartPoint(first).SetName("side").Call();
			WriteTrashFile("d/1", "side");
			git.Commit().SetAll(true).SetMessage("modified d/1 on side").Call();
			git.Rm().AddFilepattern("d/1").Call();
			git.Rm().AddFilepattern("d").Call();
			MergeCommandResult mergeRes = git.Merge().Include(masterCommit).Call();
			NUnit.Framework.Assert.IsTrue(MergeStatus.CONFLICTING.Equals(mergeRes.GetMergeStatus
				()));
			NUnit.Framework.Assert.AreEqual("[d/1, mode:100644, stage:1, content:orig][d/1, mode:100644, stage:2, content:side][d/1, mode:100644, stage:3, content:master]"
				, IndexState(CONTENT));
		}

		/// <summary>
		/// Merging two different but mergeable subtrees when the index does not
		/// contain any file in that subtree should lead to a merged state.
		/// </summary>
		/// <remarks>
		/// Merging two different but mergeable subtrees when the index does not
		/// contain any file in that subtree should lead to a merged state.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void CheckMergeMergeableTreesWithoutIndex()
		{
			Git git = Git.Wrap(db);
			WriteTrashFile("d/1", "1\n2\n3");
			git.Add().AddFilepattern("d/1").Call();
			RevCommit first = git.Commit().SetMessage("added d/1").Call();
			WriteTrashFile("d/1", "1master\n2\n3");
			RevCommit masterCommit = git.Commit().SetAll(true).SetMessage("modified d/1 on master"
				).Call();
			git.Checkout().SetCreateBranch(true).SetStartPoint(first).SetName("side").Call();
			WriteTrashFile("d/1", "1\n2\n3side");
			git.Commit().SetAll(true).SetMessage("modified d/1 on side").Call();
			git.Rm().AddFilepattern("d/1").Call();
			git.Rm().AddFilepattern("d").Call();
			MergeCommandResult mergeRes = git.Merge().Include(masterCommit).Call();
			NUnit.Framework.Assert.IsTrue(MergeStatus.MERGED.Equals(mergeRes.GetMergeStatus()
				));
			NUnit.Framework.Assert.AreEqual("[d/1, mode:100644, content:1master\n2\n3side\n]"
				, IndexState(CONTENT));
		}

		/// <summary>
		/// Merging two equal subtrees when the index does not contain any file in
		/// that subtree should lead to a merged state.
		/// </summary>
		/// <remarks>
		/// Merging two equal subtrees when the index does not contain any file in
		/// that subtree should lead to a merged state.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void CheckMergeEqualTreesWithoutIndex()
		{
			Git git = Git.Wrap(db);
			WriteTrashFile("d/1", "orig");
			git.Add().AddFilepattern("d/1").Call();
			RevCommit first = git.Commit().SetMessage("added d/1").Call();
			WriteTrashFile("d/1", "modified");
			RevCommit masterCommit = git.Commit().SetAll(true).SetMessage("modified d/1 on master"
				).Call();
			git.Checkout().SetCreateBranch(true).SetStartPoint(first).SetName("side").Call();
			WriteTrashFile("d/1", "modified");
			git.Commit().SetAll(true).SetMessage("modified d/1 on side").Call();
			git.Rm().AddFilepattern("d/1").Call();
			git.Rm().AddFilepattern("d").Call();
			MergeCommandResult mergeRes = git.Merge().Include(masterCommit).Call();
			NUnit.Framework.Assert.IsTrue(MergeStatus.MERGED.Equals(mergeRes.GetMergeStatus()
				));
			NUnit.Framework.Assert.AreEqual("[d/1, mode:100644, content:modified]", IndexState
				(CONTENT));
		}

		/// <summary>
		/// Merging two equal subtrees with an incore merger should lead to a merged
		/// state (The 'Gerrit' use case).
		/// </summary>
		/// <remarks>
		/// Merging two equal subtrees with an incore merger should lead to a merged
		/// state (The 'Gerrit' use case).
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void CheckMergeEqualTreesInCore()
		{
			Git git = Git.Wrap(db);
			WriteTrashFile("d/1", "orig");
			git.Add().AddFilepattern("d/1").Call();
			RevCommit first = git.Commit().SetMessage("added d/1").Call();
			WriteTrashFile("d/1", "modified");
			RevCommit masterCommit = git.Commit().SetAll(true).SetMessage("modified d/1 on master"
				).Call();
			git.Checkout().SetCreateBranch(true).SetStartPoint(first).SetName("side").Call();
			WriteTrashFile("d/1", "modified");
			RevCommit sideCommit = git.Commit().SetAll(true).SetMessage("modified d/1 on side"
				).Call();
			git.Rm().AddFilepattern("d/1").Call();
			git.Rm().AddFilepattern("d").Call();
			ThreeWayMerger resolveMerger = ((ThreeWayMerger)MergeStrategy.RESOLVE.NewMerger(db
				, true));
			bool noProblems = resolveMerger.Merge(masterCommit, sideCommit);
			NUnit.Framework.Assert.IsTrue(noProblems);
		}

		/// <summary>
		/// Merging two equal subtrees when the index and HEAD does not contain any
		/// file in that subtree should lead to a merged state.
		/// </summary>
		/// <remarks>
		/// Merging two equal subtrees when the index and HEAD does not contain any
		/// file in that subtree should lead to a merged state.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void CheckMergeEqualNewTrees()
		{
			Git git = Git.Wrap(db);
			WriteTrashFile("2", "orig");
			git.Add().AddFilepattern("2").Call();
			RevCommit first = git.Commit().SetMessage("added 2").Call();
			WriteTrashFile("d/1", "orig");
			git.Add().AddFilepattern("d/1").Call();
			RevCommit masterCommit = git.Commit().SetAll(true).SetMessage("added d/1 on master"
				).Call();
			git.Checkout().SetCreateBranch(true).SetStartPoint(first).SetName("side").Call();
			WriteTrashFile("d/1", "orig");
			git.Add().AddFilepattern("d/1").Call();
			git.Commit().SetAll(true).SetMessage("added d/1 on side").Call();
			git.Rm().AddFilepattern("d/1").Call();
			git.Rm().AddFilepattern("d").Call();
			MergeCommandResult mergeRes = git.Merge().Include(masterCommit).Call();
			NUnit.Framework.Assert.IsTrue(MergeStatus.MERGED.Equals(mergeRes.GetMergeStatus()
				));
			NUnit.Framework.Assert.AreEqual("[2, mode:100644, content:orig][d/1, mode:100644, content:orig]"
				, IndexState(CONTENT));
		}

		/// <summary>
		/// Merging two conflicting subtrees when the index and HEAD does not contain
		/// any file in that subtree should lead to a conflicting state.
		/// </summary>
		/// <remarks>
		/// Merging two conflicting subtrees when the index and HEAD does not contain
		/// any file in that subtree should lead to a conflicting state.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void CheckMergeConflictingNewTrees()
		{
			Git git = Git.Wrap(db);
			WriteTrashFile("2", "orig");
			git.Add().AddFilepattern("2").Call();
			RevCommit first = git.Commit().SetMessage("added 2").Call();
			WriteTrashFile("d/1", "master");
			git.Add().AddFilepattern("d/1").Call();
			RevCommit masterCommit = git.Commit().SetAll(true).SetMessage("added d/1 on master"
				).Call();
			git.Checkout().SetCreateBranch(true).SetStartPoint(first).SetName("side").Call();
			WriteTrashFile("d/1", "side");
			git.Add().AddFilepattern("d/1").Call();
			git.Commit().SetAll(true).SetMessage("added d/1 on side").Call();
			git.Rm().AddFilepattern("d/1").Call();
			git.Rm().AddFilepattern("d").Call();
			MergeCommandResult mergeRes = git.Merge().Include(masterCommit).Call();
			NUnit.Framework.Assert.IsTrue(MergeStatus.CONFLICTING.Equals(mergeRes.GetMergeStatus
				()));
			NUnit.Framework.Assert.AreEqual("[2, mode:100644, content:orig][d/1, mode:100644, stage:2, content:side][d/1, mode:100644, stage:3, content:master]"
				, IndexState(CONTENT));
		}

		/// <summary>
		/// Merging two conflicting files when the index contains a tree for that
		/// path should lead to a failed state.
		/// </summary>
		/// <remarks>
		/// Merging two conflicting files when the index contains a tree for that
		/// path should lead to a failed state.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void CheckMergeConflictingFilesWithTreeInIndex()
		{
			Git git = Git.Wrap(db);
			WriteTrashFile("0", "orig");
			git.Add().AddFilepattern("0").Call();
			RevCommit first = git.Commit().SetMessage("added 0").Call();
			WriteTrashFile("0", "master");
			RevCommit masterCommit = git.Commit().SetAll(true).SetMessage("modified 0 on master"
				).Call();
			git.Checkout().SetCreateBranch(true).SetStartPoint(first).SetName("side").Call();
			WriteTrashFile("0", "side");
			git.Commit().SetAll(true).SetMessage("modified 0 on side").Call();
			git.Rm().AddFilepattern("0").Call();
			WriteTrashFile("0/0", "side");
			git.Add().AddFilepattern("0/0").Call();
			MergeCommandResult mergeRes = git.Merge().Include(masterCommit).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAILED, mergeRes.GetMergeStatus());
		}

		/// <summary>
		/// Merging two equal files when the index contains a tree for that path
		/// should lead to a failed state.
		/// </summary>
		/// <remarks>
		/// Merging two equal files when the index contains a tree for that path
		/// should lead to a failed state.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void CheckMergeMergeableFilesWithTreeInIndex()
		{
			Git git = Git.Wrap(db);
			WriteTrashFile("0", "orig");
			WriteTrashFile("1", "1\n2\n3");
			git.Add().AddFilepattern("0").AddFilepattern("1").Call();
			RevCommit first = git.Commit().SetMessage("added 0, 1").Call();
			WriteTrashFile("1", "1master\n2\n3");
			RevCommit masterCommit = git.Commit().SetAll(true).SetMessage("modified 1 on master"
				).Call();
			git.Checkout().SetCreateBranch(true).SetStartPoint(first).SetName("side").Call();
			WriteTrashFile("1", "1\n2\n3side");
			git.Commit().SetAll(true).SetMessage("modified 1 on side").Call();
			git.Rm().AddFilepattern("0").Call();
			WriteTrashFile("0/0", "modified");
			git.Add().AddFilepattern("0/0").Call();
			try
			{
				git.Merge().Include(masterCommit).Call();
				NUnit.Framework.Assert.Fail("Didn't get the expected exception");
			}
			catch (NGit.Api.Errors.CheckoutConflictException e)
			{
				NUnit.Framework.Assert.AreEqual(1, e.GetConflictingPaths().Count);
				NUnit.Framework.Assert.AreEqual("0/0", e.GetConflictingPaths()[0]);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void CheckLockedFilesToBeDeleted()
		{
			Git git = Git.Wrap(db);
			WriteTrashFile("a.txt", "orig");
			WriteTrashFile("b.txt", "orig");
			git.Add().AddFilepattern("a.txt").AddFilepattern("b.txt").Call();
			RevCommit first = git.Commit().SetMessage("added a.txt, b.txt").Call();
			// modify and delete files on the master branch
			WriteTrashFile("a.txt", "master");
			git.Rm().AddFilepattern("b.txt").Call();
			RevCommit masterCommit = git.Commit().SetMessage("modified a.txt, deleted b.txt")
				.SetAll(true).Call();
			// switch back to a side branch
			git.Checkout().SetCreateBranch(true).SetStartPoint(first).SetName("side").Call();
			WriteTrashFile("c.txt", "side");
			git.Add().AddFilepattern("c.txt").Call();
			git.Commit().SetMessage("added c.txt").Call();
			// Get a handle to the the file so on windows it can't be deleted.
			FileInputStream fis = new FileInputStream(new FilePath(db.WorkTree, "b.txt"));
			MergeCommandResult mergeRes = git.Merge().Include(masterCommit).Call();
			if (mergeRes.GetMergeStatus().Equals(MergeStatus.FAILED))
			{
				// probably windows
				NUnit.Framework.Assert.AreEqual(1, mergeRes.GetFailingPaths().Count);
				NUnit.Framework.Assert.AreEqual(ResolveMerger.MergeFailureReason.COULD_NOT_DELETE
					, mergeRes.GetFailingPaths().Get("b.txt"));
			}
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, content:master]" + "[c.txt, mode:100644, content:side]"
				, IndexState(CONTENT));
			fis.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void CheckForCorrectIndex()
		{
			FilePath f;
			long lastTs4;
			long lastTsIndex;
			Git git = Git.Wrap(db);
			FilePath indexFile = db.GetIndexFile();
			// Create initial content and remember when the last file was written.
			f = WriteTrashFiles(false, "orig", "orig", "1\n2\n3", "orig", "orig");
			lastTs4 = f.LastModified();
			// add all files, commit and check this doesn't update any working tree
			// files and that the index is in a new file system timer tick. Make
			// sure to wait long enough before adding so the index doesn't contain
			// racily clean entries
			FsTick(f);
			git.Add().AddFilepattern(".").Call();
			RevCommit firstCommit = git.Commit().SetMessage("initial commit").Call();
			CheckConsistentLastModified("0", "1", "2", "3", "4");
			CheckModificationTimeStampOrder("1", "2", "3", "4", "<.git/index");
			NUnit.Framework.Assert.AreEqual(lastTs4, new FilePath(db.WorkTree, "4").LastModified
				(), "Commit should not touch working tree file 4");
			lastTsIndex = indexFile.LastModified();
			// Do modifications on the master branch. Then add and commit. This
			// should touch only "0", "2 and "3"
			FsTick(indexFile);
			f = WriteTrashFiles(false, "master", null, "1master\n2\n3", "master", null);
			FsTick(f);
			git.Add().AddFilepattern(".").Call();
			RevCommit masterCommit = git.Commit().SetMessage("master commit").Call();
			CheckConsistentLastModified("0", "1", "2", "3", "4");
			CheckModificationTimeStampOrder("1", "4", "*" + lastTs4, "<*" + lastTsIndex, "<0"
				, "2", "3", "<.git/index");
			lastTsIndex = indexFile.LastModified();
			// Checkout a side branch. This should touch only "0", "2 and "3"
			FsTick(indexFile);
			git.Checkout().SetCreateBranch(true).SetStartPoint(firstCommit).SetName("side").Call
				();
			CheckConsistentLastModified("0", "1", "2", "3", "4");
			CheckModificationTimeStampOrder("1", "4", "*" + lastTs4, "<*" + lastTsIndex, "<0"
				, "2", "3", ".git/index");
			lastTsIndex = indexFile.LastModified();
			// This checkout may have populated worktree and index so fast that we
			// may have smudged entries now. Check that we have the right content
			// and then rewrite the index to get rid of smudged state
			NUnit.Framework.Assert.AreEqual("[0, mode:100644, content:orig]" + "[1, mode:100644, content:orig]"
				 + "[2, mode:100644, content:1\n2\n3]" + "[3, mode:100644, content:orig]" + "[4, mode:100644, content:orig]"
				, IndexState(CONTENT));
			//
			//
			//
			//
			//
			FsTick(indexFile);
			f = WriteTrashFiles(false, "orig", "orig", "1\n2\n3", "orig", "orig");
			lastTs4 = f.LastModified();
			FsTick(f);
			git.Add().AddFilepattern(".").Call();
			CheckConsistentLastModified("0", "1", "2", "3", "4");
			CheckModificationTimeStampOrder("*" + lastTsIndex, "<0", "1", "2", "3", "4", "<.git/index"
				);
			lastTsIndex = indexFile.LastModified();
			// Do modifications on the side branch. Touch only "1", "2 and "3"
			FsTick(indexFile);
			f = WriteTrashFiles(false, null, "side", "1\n2\n3side", "side", null);
			FsTick(f);
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("side commit").Call();
			CheckConsistentLastModified("0", "1", "2", "3", "4");
			CheckModificationTimeStampOrder("0", "4", "*" + lastTs4, "<*" + lastTsIndex, "<1"
				, "2", "3", "<.git/index");
			lastTsIndex = indexFile.LastModified();
			// merge master and side. Should only touch "0," "2" and "3"
			FsTick(indexFile);
			git.Merge().Include(masterCommit).Call();
			CheckConsistentLastModified("0", "1", "2", "4");
			CheckModificationTimeStampOrder("4", "*" + lastTs4, "<1", "<*" + lastTsIndex, "<0"
				, "2", "3", ".git/index");
			NUnit.Framework.Assert.AreEqual("[0, mode:100644, content:master]" + "[1, mode:100644, content:side]"
				 + "[2, mode:100644, content:1master\n2\n3side\n]" + "[3, mode:100644, stage:1, content:orig][3, mode:100644, stage:2, content:side][3, mode:100644, stage:3, content:master]"
				 + "[4, mode:100644, content:orig]", IndexState(CONTENT));
		}

		//
		//
		//
		//
		//
		// Assert that every specified index entry has the same last modification
		// timestamp as the associated file
		/// <exception cref="System.IO.IOException"></exception>
		private void CheckConsistentLastModified(params string[] pathes)
		{
			DirCache dc = db.ReadDirCache();
			FilePath workTree = db.WorkTree;
			foreach (string path in pathes)
			{
				NUnit.Framework.Assert.AreEqual(new FilePath(workTree, path).LastModified(), dc.GetEntry
					(path).LastModified, "IndexEntry with path " + path + " has lastmodified with is different from the worktree file"
					);
			}
		}

		// Assert that modification timestamps of working tree files are as
		// expected. You may specify n files. It is asserted that every file
		// i+1 is not older than file i. If a path of file i+1 is prefixed with "<"
		// then this file must be younger then file i. A path "*<modtime>"
		// represents a file with a modification time of <modtime>
		// E.g. ("a", "b", "<c", "f/a.txt") means: a<=b<c<=f/a.txt
		private void CheckModificationTimeStampOrder(params string[] pathes)
		{
			long lastMod = long.MinValue;
			foreach (string pp in pathes)
			{
				var p = pp;
				bool strong = p.StartsWith("<");
				bool @fixed = p[strong ? 1 : 0] == '*';
				p = Sharpen.Runtime.Substring(p, (strong ? 1 : 0) + (@fixed ? 1 : 0));
				long curMod = @fixed ? long.Parse (p) : new FilePath(db.WorkTree, p
					).LastModified();
				if (strong)
				{
					NUnit.Framework.Assert.IsTrue(curMod > lastMod, "path " + p + " is not younger than predecesssor"
						);
				}
				else
				{
					NUnit.Framework.Assert.IsTrue(curMod >= lastMod, "path " + p + " is older than predecesssor"
						);
				}
			}
		}
	}
}
