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
using NGit.Revwalk;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using NGit.Util;
using Sharpen;

namespace NGit.Treewalk.Filter
{
	[NUnit.Framework.TestFixture]
	public class IndexDiffFilterTest : RepositoryTestCase
	{
		private static readonly string FILE = "file";

		private static readonly string UNTRACKED_FILE = "untracked_file";

		private static readonly string IGNORED_FILE = "ignored_file";

		private static readonly string FILE_IN_FOLDER = "folder/file";

		private static readonly string UNTRACKED_FILE_IN_FOLDER = "folder/untracked_file";

		private static readonly string IGNORED_FILE_IN_FOLDER = "folder/ignored_file";

		private static readonly string FILE_IN_IGNORED_FOLDER = "ignored_folder/file";

		private static readonly string FOLDER = "folder";

		private static readonly string UNTRACKED_FOLDER = "untracked_folder";

		private static readonly string IGNORED_FOLDER = "ignored_folder";

		private static readonly string GITIGNORE = ".gitignore";

		private static readonly string FILE_CONTENT = "content";

		private static readonly string MODIFIED_FILE_CONTENT = "modified_content";

		private Git git;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			git = new Git(db);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRecursiveTreeWalk()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			DeleteAll();
			WriteFileWithFolderName();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsTrue(treeWalk.Next());
			NUnit.Framework.Assert.AreEqual("folder", treeWalk.PathString);
			NUnit.Framework.Assert.IsTrue(treeWalk.Next());
			NUnit.Framework.Assert.AreEqual("folder/file", treeWalk.PathString);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNonRecursiveTreeWalk()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			DeleteAll();
			WriteFileWithFolderName();
			TreeWalk treeWalk = CreateNonRecursiveTreeWalk(commit);
			NUnit.Framework.Assert.IsTrue(treeWalk.Next());
			NUnit.Framework.Assert.AreEqual("folder", treeWalk.PathString);
			NUnit.Framework.Assert.IsTrue(treeWalk.Next());
			NUnit.Framework.Assert.AreEqual("folder", treeWalk.PathString);
			NUnit.Framework.Assert.IsTrue(treeWalk.IsSubtree);
			treeWalk.EnterSubtree();
			NUnit.Framework.Assert.IsTrue(treeWalk.Next());
			NUnit.Framework.Assert.AreEqual("folder/file", treeWalk.PathString);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileCommitted()
		{
			RevCommit commit = WriteFileAndCommit();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommitted()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyFolderCommitted()
		{
			RevCommit commit = CreateEmptyFolderAndCommit();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileCommittedChangedNotModified()
		{
			RevCommit commit = WriteFileAndCommit();
			WriteFile();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommittedChangedNotModified()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			WriteFileInFolder();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileCommittedModified()
		{
			RevCommit commit = WriteFileAndCommit();
			WriteFileModified();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommittedModified()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			WriteFileInFolderModified();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileCommittedDeleted()
		{
			RevCommit commit = WriteFileAndCommit();
			DeleteFile();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommittedDeleted()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			DeleteFileInFolder();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommittedAllDeleted()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			DeleteAll();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyFolderCommittedDeleted()
		{
			RevCommit commit = CreateEmptyFolderAndCommit();
			DeleteFolder();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileCommittedModifiedCommittedComparedWithInitialCommit()
		{
			RevCommit commit = WriteFileAndCommit();
			WriteFileModifiedAndCommit();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommittedModifiedCommittedComparedWithInitialCommit
			()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			WriteFileInFolderModifiedAndCommit();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileCommittedDeletedCommittedComparedWithInitialCommit()
		{
			RevCommit commit = WriteFileAndCommit();
			DeleteFileAndCommit();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommittedDeletedCommittedComparedWithInitialCommit
			()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			DeleteFileInFolderAndCommit();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommittedAllDeletedCommittedComparedWithInitialCommit
			()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			DeleteAllAndCommit();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyFolderCommittedDeletedCommittedComparedWithInitialCommit
			()
		{
			RevCommit commit = CreateEmptyFolderAndCommit();
			DeleteFolderAndCommit();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileUntracked()
		{
			RevCommit commit = WriteFileAndCommit();
			WriteFileUntracked();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, UNTRACKED_FILE);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderUntracked()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			WriteFileInFolderUntracked();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, UNTRACKED_FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyFolderUntracked()
		{
			RevCommit commit = CreateEmptyFolderAndCommit();
			CreateEmptyFolderUntracked();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileIgnored()
		{
			RevCommit commit = WriteFileAndCommit();
			WriteFileIgnored();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderIgnored()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			WriteFileInFolderIgnored();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderAllIgnored()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			WriteFileInFolderAllIgnored();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyFolderIgnored()
		{
			RevCommit commit = CreateEmptyFolderAndCommit();
			CreateEmptyFolderIgnored();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileIgnoredNotHonored()
		{
			RevCommit commit = WriteFileAndCommit();
			WriteFileIgnored();
			TreeWalk treeWalk = CreateTreeWalkDishonorIgnores(commit);
			AssertPaths(treeWalk, IGNORED_FILE, GITIGNORE);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileCommittedModifiedIgnored()
		{
			RevCommit commit = WriteFileAndCommit();
			WriteFileModifiedIgnored();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommittedModifiedIgnored()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			WriteFileInFolderModifiedIgnored();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommittedModifiedAllIgnored()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			WriteFileInFolderModifiedAllIgnored();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileCommittedDeletedCommittedIgnoredComparedWithInitialCommit
			()
		{
			RevCommit commit = WriteFileAndCommit();
			DeleteFileAndCommit();
			RewriteFileIgnored();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommittedDeletedCommittedIgnoredComparedWithInitialCommit
			()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			DeleteFileInFolderAndCommit();
			RewriteFileInFolderIgnored();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommittedAllDeletedCommittedAllIgnoredComparedWithInitialCommit
			()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			DeleteAllAndCommit();
			RewriteFileInFolderAllIgnored();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyFolderCommittedDeletedCommittedIgnoredComparedWithInitialCommit
			()
		{
			RevCommit commit = CreateEmptyFolderAndCommit();
			DeleteFolderAndCommit();
			RecreateEmptyFolderIgnored();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileInFolderCommittedNonRecursive()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			TreeWalk treeWalk = CreateNonRecursiveTreeWalk(commit);
			AssertPaths(treeWalk, FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFolderChangedToFile()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			DeleteAll();
			WriteFileWithFolderName();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FOLDER, FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFolderChangedToFileCommittedComparedWithInitialCommit()
		{
			RevCommit commit = WriteFileInFolderAndCommit();
			DeleteAll();
			WriteFileWithFolderNameAndCommit();
			TreeWalk treeWalk = CreateTreeWalk(commit);
			AssertPaths(treeWalk, FOLDER, FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFile()
		{
			WriteTrashFile(FILE, FILE_CONTENT);
		}

		/// <exception cref="System.Exception"></exception>
		private RevCommit WriteFileAndCommit()
		{
			WriteFile();
			return CommitAdd();
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileModified()
		{
			WriteTrashFile(FILE, MODIFIED_FILE_CONTENT);
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileModifiedAndCommit()
		{
			WriteFileModified();
			CommitAdd();
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileUntracked()
		{
			WriteTrashFile(UNTRACKED_FILE, FILE_CONTENT);
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileIgnored()
		{
			WriteTrashFile(IGNORED_FILE, FILE_CONTENT);
			WriteTrashFile(GITIGNORE, GITIGNORE + "\n" + IGNORED_FILE);
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileModifiedIgnored()
		{
			WriteFileModified();
			WriteTrashFile(GITIGNORE, GITIGNORE + "\n" + FILE);
		}

		/// <exception cref="System.Exception"></exception>
		private void RewriteFileIgnored()
		{
			WriteFile();
			WriteTrashFile(GITIGNORE, GITIGNORE + "\n" + FILE);
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileWithFolderName()
		{
			WriteTrashFile(FOLDER, FILE_CONTENT);
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileWithFolderNameAndCommit()
		{
			WriteFileWithFolderName();
			CommitAdd();
		}

		/// <exception cref="System.Exception"></exception>
		private void DeleteFile()
		{
			DeleteTrashFile(FILE);
		}

		/// <exception cref="System.Exception"></exception>
		private void DeleteFileAndCommit()
		{
			DeleteFile();
			CommitRm(FILE);
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileInFolder()
		{
			WriteTrashFile(FILE_IN_FOLDER, FILE_CONTENT);
		}

		/// <exception cref="System.Exception"></exception>
		private RevCommit WriteFileInFolderAndCommit()
		{
			WriteFileInFolder();
			return CommitAdd();
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileInFolderModified()
		{
			WriteTrashFile(FILE_IN_FOLDER, MODIFIED_FILE_CONTENT);
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileInFolderModifiedAndCommit()
		{
			WriteFileInFolderModified();
			CommitAdd();
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileInFolderUntracked()
		{
			WriteTrashFile(UNTRACKED_FILE_IN_FOLDER, FILE_CONTENT);
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileInFolderIgnored()
		{
			WriteTrashFile(IGNORED_FILE_IN_FOLDER, FILE_CONTENT);
			WriteTrashFile(GITIGNORE, GITIGNORE + "\n" + IGNORED_FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileInFolderAllIgnored()
		{
			WriteTrashFile(FILE_IN_IGNORED_FOLDER, FILE_CONTENT);
			WriteTrashFile(GITIGNORE, GITIGNORE + "\n" + IGNORED_FOLDER + "/");
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileInFolderModifiedIgnored()
		{
			WriteFileInFolderModified();
			WriteTrashFile(GITIGNORE, GITIGNORE + "\n" + FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		private void RewriteFileInFolderIgnored()
		{
			WriteFileInFolder();
			WriteTrashFile(GITIGNORE, GITIGNORE + "\n" + FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileInFolderModifiedAllIgnored()
		{
			WriteFileInFolderModified();
			WriteTrashFile(GITIGNORE, GITIGNORE + "\n" + FOLDER + "/");
		}

		/// <exception cref="System.Exception"></exception>
		private void RewriteFileInFolderAllIgnored()
		{
			WriteFileInFolder();
			WriteTrashFile(GITIGNORE, GITIGNORE + "\n" + FOLDER + "/");
		}

		/// <exception cref="System.Exception"></exception>
		private void DeleteFileInFolder()
		{
			DeleteTrashFile(FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		private void DeleteFileInFolderAndCommit()
		{
			DeleteFileInFolder();
			CommitRm(FILE_IN_FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		private void CreateEmptyFolder()
		{
			FilePath path = new FilePath(db.WorkTree, FOLDER);
			FileUtils.Mkdir(path);
		}

		/// <exception cref="System.Exception"></exception>
		private RevCommit CreateEmptyFolderAndCommit()
		{
			CreateEmptyFolder();
			return CommitAdd();
		}

		/// <exception cref="System.Exception"></exception>
		private void CreateEmptyFolderUntracked()
		{
			FilePath path = new FilePath(db.WorkTree, UNTRACKED_FOLDER);
			FileUtils.Mkdir(path);
		}

		/// <exception cref="System.Exception"></exception>
		private void CreateEmptyFolderIgnored()
		{
			FilePath path = new FilePath(db.WorkTree, IGNORED_FOLDER);
			FileUtils.Mkdir(path);
			WriteTrashFile(GITIGNORE, GITIGNORE + "\n" + IGNORED_FOLDER + "/");
		}

		/// <exception cref="System.Exception"></exception>
		private void RecreateEmptyFolderIgnored()
		{
			CreateEmptyFolder();
			WriteTrashFile(GITIGNORE, GITIGNORE + "\n" + FOLDER + "/");
		}

		/// <exception cref="System.Exception"></exception>
		private void DeleteFolder()
		{
			DeleteTrashFile(FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		private void DeleteFolderAndCommit()
		{
			DeleteFolder();
			CommitRm(FOLDER);
		}

		/// <exception cref="System.Exception"></exception>
		private void DeleteAll()
		{
			DeleteFileInFolder();
			DeleteFolder();
		}

		/// <exception cref="System.Exception"></exception>
		private void DeleteAllAndCommit()
		{
			DeleteFileInFolderAndCommit();
			DeleteFolderAndCommit();
		}

		/// <exception cref="System.Exception"></exception>
		private RevCommit CommitAdd()
		{
			git.Add().AddFilepattern(".").Call();
			return git.Commit().SetMessage("commit").Call();
		}

		/// <exception cref="System.Exception"></exception>
		private RevCommit CommitRm(string path)
		{
			git.Rm().AddFilepattern(path).Call();
			return git.Commit().SetMessage("commit").Call();
		}

		/// <exception cref="System.Exception"></exception>
		private TreeWalk CreateTreeWalk(RevCommit commit)
		{
			return CreateTreeWalk(commit, true, true);
		}

		/// <exception cref="System.Exception"></exception>
		private TreeWalk CreateTreeWalkDishonorIgnores(RevCommit commit)
		{
			return CreateTreeWalk(commit, true, false);
		}

		/// <exception cref="System.Exception"></exception>
		private TreeWalk CreateNonRecursiveTreeWalk(RevCommit commit)
		{
			return CreateTreeWalk(commit, false, true);
		}

		/// <exception cref="System.Exception"></exception>
		private TreeWalk CreateTreeWalk(RevCommit commit, bool isRecursive, bool honorIgnores
			)
		{
			TreeWalk treeWalk = new TreeWalk(db);
			treeWalk.Recursive = isRecursive;
			treeWalk.AddTree(commit.Tree);
			treeWalk.AddTree(new DirCacheIterator(db.ReadDirCache()));
			treeWalk.AddTree(new FileTreeIterator(db));
			if (!honorIgnores)
			{
				treeWalk.Filter = new IndexDiffFilter(1, 2, honorIgnores);
			}
			else
			{
				treeWalk.Filter = new IndexDiffFilter(1, 2);
			}
			return treeWalk;
		}

		/// <exception cref="System.Exception"></exception>
		private void AssertPaths(TreeWalk treeWalk, params string[] paths)
		{
			for (int i = 0; i < paths.Length; i++)
			{
				NUnit.Framework.Assert.IsTrue(treeWalk.Next());
				AssertPath(treeWalk.PathString, paths);
			}
			NUnit.Framework.Assert.IsFalse(treeWalk.Next());
		}

		private void AssertPath(string path, params string[] paths)
		{
			foreach (string p in paths)
			{
				if (p.Equals(path))
				{
					return;
				}
			}
			NUnit.Framework.Assert.Fail("Expected path '" + path + "' is not returned");
		}
	}
}
