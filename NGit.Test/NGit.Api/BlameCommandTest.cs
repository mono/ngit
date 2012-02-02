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

using System.Text;
using NGit;
using NGit.Api;
using NGit.Blame;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Unit tests of
	/// <see cref="BlameCommand">BlameCommand</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class BlameCommandTest : RepositoryTestCase
	{
		private string Join(params string[] lines)
		{
			StringBuilder joined = new StringBuilder();
			foreach (string line in lines)
			{
				joined.Append(line).Append('\n');
			}
			return joined.ToString();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSingleRevision()
		{
			Git git = new Git(db);
			string[] content = new string[] { "first", "second", "third" };
			WriteTrashFile("file.txt", Join(content));
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit = git.Commit().SetMessage("create file").Call();
			BlameCommand command = new BlameCommand(db);
			command.SetFilePath("file.txt");
			BlameResult lines = command.Call();
			NUnit.Framework.Assert.IsNotNull(lines);
			NUnit.Framework.Assert.AreEqual(3, lines.GetResultContents().Size());
			for (int i = 0; i < 3; i++)
			{
				NUnit.Framework.Assert.AreEqual(commit, lines.GetSourceCommit(i));
				NUnit.Framework.Assert.AreEqual(i, lines.GetSourceLine(i));
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoRevisions()
		{
			Git git = new Git(db);
			string[] content1 = new string[] { "first", "second" };
			WriteTrashFile("file.txt", Join(content1));
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit1 = git.Commit().SetMessage("create file").Call();
			string[] content2 = new string[] { "first", "second", "third" };
			WriteTrashFile("file.txt", Join(content2));
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit2 = git.Commit().SetMessage("create file").Call();
			BlameCommand command = new BlameCommand(db);
			command.SetFilePath("file.txt");
			BlameResult lines = command.Call();
			NUnit.Framework.Assert.AreEqual(3, lines.GetResultContents().Size());
			NUnit.Framework.Assert.AreEqual(commit1, lines.GetSourceCommit(0));
			NUnit.Framework.Assert.AreEqual(0, lines.GetSourceLine(0));
			NUnit.Framework.Assert.AreEqual(commit1, lines.GetSourceCommit(1));
			NUnit.Framework.Assert.AreEqual(1, lines.GetSourceLine(1));
			NUnit.Framework.Assert.AreEqual(commit2, lines.GetSourceCommit(2));
			NUnit.Framework.Assert.AreEqual(2, lines.GetSourceLine(2));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRename()
		{
			TestRename("file1.txt", "file2.txt");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameInSubDir()
		{
			TestRename("subdir/file1.txt", "subdir/file2.txt");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMoveToOtherDir()
		{
			TestRename("subdir/file1.txt", "otherdir/file1.txt");
		}

		/// <exception cref="System.Exception"></exception>
		private void TestRename(string sourcePath, string destPath)
		{
			Git git = new Git(db);
			string[] content1 = new string[] { "a", "b", "c" };
			WriteTrashFile(sourcePath, Join(content1));
			git.Add().AddFilepattern(sourcePath).Call();
			RevCommit commit1 = git.Commit().SetMessage("create file").Call();
			WriteTrashFile(destPath, Join(content1));
			git.Add().AddFilepattern(destPath).Call();
			git.Rm().AddFilepattern(sourcePath).Call();
			git.Commit().SetMessage("moving file").Call();
			string[] content2 = new string[] { "a", "b", "c2" };
			WriteTrashFile(destPath, Join(content2));
			git.Add().AddFilepattern(destPath).Call();
			RevCommit commit3 = git.Commit().SetMessage("editing file").Call();
			BlameCommand command = new BlameCommand(db);
			command.SetFollowFileRenames(true);
			command.SetFilePath(destPath);
			BlameResult lines = command.Call();
			NUnit.Framework.Assert.AreEqual(commit1, lines.GetSourceCommit(0));
			NUnit.Framework.Assert.AreEqual(0, lines.GetSourceLine(0));
			NUnit.Framework.Assert.AreEqual(sourcePath, lines.GetSourcePath(0));
			NUnit.Framework.Assert.AreEqual(commit1, lines.GetSourceCommit(1));
			NUnit.Framework.Assert.AreEqual(1, lines.GetSourceLine(1));
			NUnit.Framework.Assert.AreEqual(sourcePath, lines.GetSourcePath(1));
			NUnit.Framework.Assert.AreEqual(commit3, lines.GetSourceCommit(2));
			NUnit.Framework.Assert.AreEqual(2, lines.GetSourceLine(2));
			NUnit.Framework.Assert.AreEqual(destPath, lines.GetSourcePath(2));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoRenames()
		{
			Git git = new Git(db);
			// Commit 1: Add file.txt
			string[] content1 = new string[] { "a" };
			WriteTrashFile("file.txt", Join(content1));
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit1 = git.Commit().SetMessage("create file").Call();
			// Commit 2: Rename to file1.txt
			WriteTrashFile("file1.txt", Join(content1));
			git.Add().AddFilepattern("file1.txt").Call();
			git.Rm().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("moving file").Call();
			// Commit 3: Edit file1.txt
			string[] content2 = new string[] { "a", "b" };
			WriteTrashFile("file1.txt", Join(content2));
			git.Add().AddFilepattern("file1.txt").Call();
			RevCommit commit3 = git.Commit().SetMessage("editing file").Call();
			// Commit 4: Rename to file2.txt
			WriteTrashFile("file2.txt", Join(content2));
			git.Add().AddFilepattern("file2.txt").Call();
			git.Rm().AddFilepattern("file1.txt").Call();
			git.Commit().SetMessage("moving file again").Call();
			BlameCommand command = new BlameCommand(db);
			command.SetFollowFileRenames(true);
			command.SetFilePath("file2.txt");
			BlameResult lines = command.Call();
			NUnit.Framework.Assert.AreEqual(commit1, lines.GetSourceCommit(0));
			NUnit.Framework.Assert.AreEqual(0, lines.GetSourceLine(0));
			NUnit.Framework.Assert.AreEqual("file.txt", lines.GetSourcePath(0));
			NUnit.Framework.Assert.AreEqual(commit3, lines.GetSourceCommit(1));
			NUnit.Framework.Assert.AreEqual(1, lines.GetSourceLine(1));
			NUnit.Framework.Assert.AreEqual("file1.txt", lines.GetSourcePath(1));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteTrailingLines()
		{
			Git git = new Git(db);
			string[] content1 = new string[] { "a", "b", "c", "d" };
			string[] content2 = new string[] { "a", "b" };
			WriteTrashFile("file.txt", Join(content2));
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit1 = git.Commit().SetMessage("create file").Call();
			WriteTrashFile("file.txt", Join(content1));
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("edit file").Call();
			WriteTrashFile("file.txt", Join(content2));
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("edit file").Call();
			BlameCommand command = new BlameCommand(db);
			command.SetFilePath("file.txt");
			BlameResult lines = command.Call();
			NUnit.Framework.Assert.AreEqual(content2.Length, lines.GetResultContents().Size()
				);
			NUnit.Framework.Assert.AreEqual(commit1, lines.GetSourceCommit(0));
			NUnit.Framework.Assert.AreEqual(commit1, lines.GetSourceCommit(1));
			NUnit.Framework.Assert.AreEqual(0, lines.GetSourceLine(0));
			NUnit.Framework.Assert.AreEqual(1, lines.GetSourceLine(1));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteMiddleLines()
		{
			Git git = new Git(db);
			string[] content1 = new string[] { "a", "b", "c", "d", "e" };
			string[] content2 = new string[] { "a", "c", "e" };
			WriteTrashFile("file.txt", Join(content2));
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit1 = git.Commit().SetMessage("edit file").Call();
			WriteTrashFile("file.txt", Join(content1));
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("edit file").Call();
			WriteTrashFile("file.txt", Join(content2));
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("edit file").Call();
			BlameCommand command = new BlameCommand(db);
			command.SetFilePath("file.txt");
			BlameResult lines = command.Call();
			NUnit.Framework.Assert.AreEqual(content2.Length, lines.GetResultContents().Size()
				);
			NUnit.Framework.Assert.AreEqual(commit1, lines.GetSourceCommit(0));
			NUnit.Framework.Assert.AreEqual(0, lines.GetSourceLine(0));
			NUnit.Framework.Assert.AreEqual(commit1, lines.GetSourceCommit(1));
			NUnit.Framework.Assert.AreEqual(1, lines.GetSourceLine(1));
			NUnit.Framework.Assert.AreEqual(commit1, lines.GetSourceCommit(2));
			NUnit.Framework.Assert.AreEqual(2, lines.GetSourceLine(2));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEditAllLines()
		{
			Git git = new Git(db);
			string[] content1 = new string[] { "a", "1" };
			string[] content2 = new string[] { "b", "2" };
			WriteTrashFile("file.txt", Join(content1));
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("edit file").Call();
			WriteTrashFile("file.txt", Join(content2));
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit2 = git.Commit().SetMessage("create file").Call();
			BlameCommand command = new BlameCommand(db);
			command.SetFilePath("file.txt");
			BlameResult lines = command.Call();
			NUnit.Framework.Assert.AreEqual(content2.Length, lines.GetResultContents().Size()
				);
			NUnit.Framework.Assert.AreEqual(commit2, lines.GetSourceCommit(0));
			NUnit.Framework.Assert.AreEqual(commit2, lines.GetSourceCommit(1));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMiddleClearAllLines()
		{
			Git git = new Git(db);
			string[] content1 = new string[] { "a", "b", "c" };
			WriteTrashFile("file.txt", Join(content1));
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("edit file").Call();
			WriteTrashFile("file.txt", string.Empty);
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("create file").Call();
			WriteTrashFile("file.txt", Join(content1));
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit3 = git.Commit().SetMessage("edit file").Call();
			BlameCommand command = new BlameCommand(db);
			command.SetFilePath("file.txt");
			BlameResult lines = command.Call();
			NUnit.Framework.Assert.AreEqual(content1.Length, lines.GetResultContents().Size()
				);
			NUnit.Framework.Assert.AreEqual(commit3, lines.GetSourceCommit(0));
			NUnit.Framework.Assert.AreEqual(commit3, lines.GetSourceCommit(1));
			NUnit.Framework.Assert.AreEqual(commit3, lines.GetSourceCommit(2));
		}
	}
}
