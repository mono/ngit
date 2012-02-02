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
using NGit.Dircache;
using NGit.Revwalk;
using NGit.Treewalk;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Diff
{
	[NUnit.Framework.TestFixture]
	public class DiffEntryTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldListAddedFileInInitialCommit()
		{
			// given
			WriteTrashFile("a.txt", "content");
			Git git = new Git(db);
			git.Add().AddFilepattern("a.txt").Call();
			RevCommit c = git.Commit().SetMessage("initial commit").Call();
			// when
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(new EmptyTreeIterator());
			walk.AddTree(c.Tree);
			IList<DiffEntry> result = DiffEntry.Scan(walk);
			// then
			Assert.IsNotNull (result);
			Assert.AreEqual(1, result.Count);
			DiffEntry entry = result[0];
			Assert.AreEqual(entry.GetChangeType(), DiffEntry.ChangeType.ADD);
			Assert.AreEqual(entry.GetNewPath(), "a.txt");
			Assert.AreEqual(entry.GetOldPath(), DiffEntry.DEV_NULL);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldListAddedFileBetweenTwoCommits()
		{
			// given
			Git git = new Git(db);
			RevCommit c1 = git.Commit().SetMessage("initial commit").Call();
			WriteTrashFile("a.txt", "content");
			git.Add().AddFilepattern("a.txt").Call();
			RevCommit c2 = git.Commit().SetMessage("second commit").Call();
			// when
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(c1.Tree);
			walk.AddTree(c2.Tree);
			IList<DiffEntry> result = DiffEntry.Scan(walk);
			// then
			Assert.IsNotNull(result);
			Assert.AreEqual (1, result.Count);
			DiffEntry entry = result[0];
			Assert.AreEqual(entry.GetChangeType(), DiffEntry.ChangeType.ADD);
			Assert.AreEqual(entry.GetNewPath(), "a.txt");
			Assert.AreEqual(entry.GetOldPath(), DiffEntry.DEV_NULL);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldListModificationBetweenTwoCommits()
		{
			// given
			Git git = new Git(db);
			FilePath file = WriteTrashFile("a.txt", "content");
			git.Add().AddFilepattern("a.txt").Call();
			RevCommit c1 = git.Commit().SetMessage("initial commit").Call();
			Write(file, "new content");
			RevCommit c2 = git.Commit().SetAll(true).SetMessage("second commit").Call();
			// when
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(c1.Tree);
			walk.AddTree(c2.Tree);
			IList<DiffEntry> result = DiffEntry.Scan(walk);
			// then
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			DiffEntry entry = result[0];
			Assert.AreEqual(entry.GetChangeType(), DiffEntry.ChangeType.MODIFY);
			Assert.AreEqual(entry.GetNewPath(), "a.txt");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldListDeletionBetweenTwoCommits()
		{
			// given
			Git git = new Git(db);
			FilePath file = WriteTrashFile("a.txt", "content");
			git.Add().AddFilepattern("a.txt").Call();
			RevCommit c1 = git.Commit().SetMessage("initial commit").Call();
			FileUtils.Delete(file);
			RevCommit c2 = git.Commit().SetAll(true).SetMessage("delete a.txt").Call();
			// when
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(c1.Tree);
			walk.AddTree(c2.Tree);
			IList<DiffEntry> result = DiffEntry.Scan(walk);
			// then
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			DiffEntry entry = result[0];
			Assert.AreEqual(entry.GetOldPath(), "a.txt");
			Assert.AreEqual(entry.GetNewPath(), DiffEntry.DEV_NULL);
			Assert.AreEqual(entry.GetChangeType(), DiffEntry.ChangeType.DELETE);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldListModificationInDirWithoutModifiedTrees()
		{
			// given
			Git git = new Git(db);
			FilePath tree = new FilePath(new FilePath(db.WorkTree, "a"), "b");
			FileUtils.Mkdirs(tree);
			FilePath file = new FilePath(tree, "c.txt");
			FileUtils.CreateNewFile(file);
			Write(file, "content");
			git.Add().AddFilepattern("a").Call();
			RevCommit c1 = git.Commit().SetMessage("initial commit").Call();
			Write(file, "new line");
			RevCommit c2 = git.Commit().SetAll(true).SetMessage("second commit").Call();
			// when
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(c1.Tree);
			walk.AddTree(c2.Tree);
			walk.Recursive = true;
			IList<DiffEntry> result = DiffEntry.Scan(walk);
			// then
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			DiffEntry entry = result[0];
			Assert.AreEqual(entry.GetChangeType(), DiffEntry.ChangeType.MODIFY);
			Assert.AreEqual(entry.GetNewPath(), "a/b/c.txt");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldListModificationInDirWithModifiedTrees()
		{
			// given
			Git git = new Git(db);
			FilePath tree = new FilePath(new FilePath(db.WorkTree, "a"), "b");
			FileUtils.Mkdirs(tree);
			FilePath file = new FilePath(tree, "c.txt");
			FileUtils.CreateNewFile(file);
			Write(file, "content");
			git.Add().AddFilepattern("a").Call();
			RevCommit c1 = git.Commit().SetMessage("initial commit").Call();
			Write(file, "new line");
			RevCommit c2 = git.Commit().SetAll(true).SetMessage("second commit").Call();
			// when
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(c1.Tree);
			walk.AddTree(c2.Tree);
			IList<DiffEntry> result = DiffEntry.Scan(walk, true);
			// then
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count);;
			DiffEntry entry = result[0];
			Assert.AreEqual(entry.GetChangeType(), DiffEntry.ChangeType.MODIFY);
			Assert.AreEqual(entry.GetNewPath(), "a");
			entry = result[1];
			Assert.AreEqual(entry.GetChangeType(), DiffEntry.ChangeType.MODIFY);
			Assert.AreEqual(entry.GetNewPath(), "a/b");
			entry = result[2];
			Assert.AreEqual(entry.GetChangeType(), DiffEntry.ChangeType.MODIFY);
			Assert.AreEqual(entry.GetNewPath(), "a/b/c.txt");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldListChangesInWorkingTree()
		{
			// given
			WriteTrashFile("a.txt", "content");
			Git git = new Git(db);
			git.Add().AddFilepattern("a.txt").Call();
			RevCommit c = git.Commit().SetMessage("initial commit").Call();
			WriteTrashFile("b.txt", "new line");
			// when
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(c.Tree);
			walk.AddTree(new FileTreeIterator(db));
			IList<DiffEntry> result = DiffEntry.Scan(walk, true);
			// then
			
			Assert.AreEqual (1, result.Count);
			DiffEntry entry = result[0];
			Assert.AreEqual(entry.GetChangeType(), DiffEntry.ChangeType.ADD);
			Assert.AreEqual(entry.GetNewPath(), "b.txt");
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void ShouldThrowIAEWhenTreeWalkHasLessThanTwoTrees()
		{
			// given - we don't need anything here
			// when
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(new EmptyTreeIterator());
			DiffEntry.Scan(walk);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void ShouldThrowIAEWhenTreeWalkHasMoreThanTwoTrees()
		{
			// given - we don't need anything here
			// when
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(new EmptyTreeIterator());
			walk.AddTree(new EmptyTreeIterator());
			walk.AddTree(new EmptyTreeIterator());
			DiffEntry.Scan(walk);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void ShouldThrowIAEWhenScanShouldIncludeTreesAndWalkIsRecursive()
		{
			// given - we don't need anything here
			// when
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(new EmptyTreeIterator());
			walk.AddTree(new EmptyTreeIterator());
			walk.Recursive = true;
			DiffEntry.Scan(walk, true);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldReportFileModeChange()
		{
			WriteTrashFile("a.txt", "content");
			Git git = new Git(db);
			git.Add().AddFilepattern("a.txt").Call();
			RevCommit c1 = git.Commit().SetMessage("initial commit").Call();
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(c1.Tree);
			walk.Recursive = true;
			NUnit.Framework.Assert.IsTrue(walk.Next());
			editor.Add(new _PathEdit_318(walk, "a.txt"));
			NUnit.Framework.Assert.IsTrue(editor.Commit());
			RevCommit c2 = git.Commit().SetMessage("second commit").Call();
			walk.Reset();
			walk.AddTree(c1.Tree);
			walk.AddTree(c2.Tree);
			IList<DiffEntry> diffs = DiffEntry.Scan(walk, false);
			NUnit.Framework.Assert.AreEqual(1, diffs.Count);
			DiffEntry diff = diffs[0];
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, diff.GetChangeType()
				);
			NUnit.Framework.Assert.AreEqual(diff.GetOldId(), diff.GetNewId());
			NUnit.Framework.Assert.AreEqual("a.txt", diff.GetOldPath());
			NUnit.Framework.Assert.AreEqual(diff.GetOldPath(), diff.GetNewPath());
			NUnit.Framework.Assert.AreEqual(FileMode.EXECUTABLE_FILE, diff.GetNewMode());
			NUnit.Framework.Assert.AreEqual(FileMode.REGULAR_FILE, diff.GetOldMode());
		}

		private sealed class _PathEdit_318 : DirCacheEditor.PathEdit
		{
			public _PathEdit_318(TreeWalk walk, string baseArg1) : base(baseArg1)
			{
				this.walk = walk;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.EXECUTABLE_FILE;
				ent.SetObjectId(walk.GetObjectId(0));
			}

			private readonly TreeWalk walk;
		}
	}
}
