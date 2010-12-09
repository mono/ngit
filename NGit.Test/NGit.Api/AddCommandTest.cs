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

using System.IO;
using NGit;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Dircache;
using NGit.Util;
using Sharpen;

namespace NGit.Api
{
	public class AddCommandTest : RepositoryTestCase
	{
		[NUnit.Framework.Test]
		public virtual void TestAddNothing()
		{
			Git git = new Git(db);
			try
			{
				git.Add().Call();
				NUnit.Framework.Assert.Fail("Expected IllegalArgumentException");
			}
			catch (NoFilepatternException)
			{
			}
		}

		// expected
		/// <exception cref="NGit.Api.Errors.NoFilepatternException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddNonExistingSingleFile()
		{
			Git git = new Git(db);
			DirCache dc = git.Add().AddFilepattern("a.txt").Call();
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.NoFilepatternException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddExistingSingleFile()
		{
			FilePath file = new FilePath(db.WorkTree, "a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			Git git = new Git(db);
			git.Add().AddFilepattern("a.txt").Call();
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, content:content]", IndexState
				(CONTENT));
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.NoFilepatternException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddExistingSingleFileInSubDir()
		{
			new FilePath(db.WorkTree, "sub").Mkdir();
			FilePath file = new FilePath(db.WorkTree, "sub/a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			Git git = new Git(db);
			git.Add().AddFilepattern("sub/a.txt").Call();
			NUnit.Framework.Assert.AreEqual("[sub/a.txt, mode:100644, content:content]", IndexState
				(CONTENT));
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.NoFilepatternException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddExistingSingleFileTwice()
		{
			FilePath file = new FilePath(db.WorkTree, "a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			Git git = new Git(db);
			DirCache dc = git.Add().AddFilepattern("a.txt").Call();
			dc.GetEntry(0).GetObjectId();
			writer = new PrintWriter(file);
			writer.Write("other content");
			writer.Close();
			dc = git.Add().AddFilepattern("a.txt").Call();
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, content:other content]", IndexState
				(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddExistingSingleFileTwiceWithCommit()
		{
			FilePath file = new FilePath(db.WorkTree, "a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			Git git = new Git(db);
			DirCache dc = git.Add().AddFilepattern("a.txt").Call();
			dc.GetEntry(0).GetObjectId();
			git.Commit().SetMessage("commit a.txt").Call();
			writer = new PrintWriter(file);
			writer.Write("other content");
			writer.Close();
			dc = git.Add().AddFilepattern("a.txt").Call();
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, content:other content]", IndexState
				(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddRemovedFile()
		{
			FilePath file = new FilePath(db.WorkTree, "a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			Git git = new Git(db);
			DirCache dc = git.Add().AddFilepattern("a.txt").Call();
			dc.GetEntry(0).GetObjectId();
			FileUtils.Delete(file);
			// is supposed to do nothing
			dc = git.Add().AddFilepattern("a.txt").Call();
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, content:content]", IndexState
				(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddRemovedCommittedFile()
		{
			FilePath file = new FilePath(db.WorkTree, "a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			Git git = new Git(db);
			DirCache dc = git.Add().AddFilepattern("a.txt").Call();
			git.Commit().SetMessage("commit a.txt").Call();
			dc.GetEntry(0).GetObjectId();
			FileUtils.Delete(file);
			// is supposed to do nothing
			dc = git.Add().AddFilepattern("a.txt").Call();
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, content:content]", IndexState
				(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddWithConflicts()
		{
			// prepare conflict
			FilePath file = new FilePath(db.WorkTree, "a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			FilePath file2 = new FilePath(db.WorkTree, "b.txt");
			file2.CreateNewFile();
			writer = new PrintWriter(file2);
			writer.Write("content b");
			writer.Close();
			ObjectInserter newObjectInserter = db.NewObjectInserter();
			DirCache dc = db.LockDirCache();
			DirCacheBuilder builder = dc.Builder();
			AddEntryToBuilder("b.txt", file2, newObjectInserter, builder, 0);
			AddEntryToBuilder("a.txt", file, newObjectInserter, builder, 1);
			writer = new PrintWriter(file);
			writer.Write("other content");
			writer.Close();
			AddEntryToBuilder("a.txt", file, newObjectInserter, builder, 3);
			writer = new PrintWriter(file);
			writer.Write("our content");
			writer.Close();
			AddEntryToBuilder("a.txt", file, newObjectInserter, builder, 2).GetObjectId();
			builder.Commit();
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, stage:1, content:content]" 
				+ "[a.txt, mode:100644, stage:2, content:our content]" + "[a.txt, mode:100644, stage:3, content:other content]"
				 + "[b.txt, mode:100644, content:content b]", IndexState(CONTENT));
			// now the test begins
			Git git = new Git(db);
			dc = git.Add().AddFilepattern("a.txt").Call();
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, content:our content]" + "[b.txt, mode:100644, content:content b]"
				, IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddTwoFiles()
		{
			FilePath file = new FilePath(db.WorkTree, "a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			FilePath file2 = new FilePath(db.WorkTree, "b.txt");
			file2.CreateNewFile();
			writer = new PrintWriter(file2);
			writer.Write("content b");
			writer.Close();
			Git git = new Git(db);
			git.Add().AddFilepattern("a.txt").AddFilepattern("b.txt").Call();
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, content:content]" + "[b.txt, mode:100644, content:content b]"
				, IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddFolder()
		{
			new FilePath(db.WorkTree, "sub").Mkdir();
			FilePath file = new FilePath(db.WorkTree, "sub/a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			FilePath file2 = new FilePath(db.WorkTree, "sub/b.txt");
			file2.CreateNewFile();
			writer = new PrintWriter(file2);
			writer.Write("content b");
			writer.Close();
			Git git = new Git(db);
			git.Add().AddFilepattern("sub").Call();
			NUnit.Framework.Assert.AreEqual("[sub/a.txt, mode:100644, content:content]" + "[sub/b.txt, mode:100644, content:content b]"
				, IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddIgnoredFile()
		{
			new FilePath(db.WorkTree, "sub").Mkdir();
			FilePath file = new FilePath(db.WorkTree, "sub/a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			FilePath ignoreFile = new FilePath(db.WorkTree, ".gitignore");
			ignoreFile.CreateNewFile();
			writer = new PrintWriter(ignoreFile);
			writer.Write("sub/b.txt");
			writer.Close();
			FilePath file2 = new FilePath(db.WorkTree, "sub/b.txt");
			file2.CreateNewFile();
			writer = new PrintWriter(file2);
			writer.Write("content b");
			writer.Close();
			Git git = new Git(db);
			git.Add().AddFilepattern("sub").Call();
			NUnit.Framework.Assert.AreEqual("[sub/a.txt, mode:100644, content:content]", IndexState
				(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddWholeRepo()
		{
			new FilePath(db.WorkTree, "sub").Mkdir();
			FilePath file = new FilePath(db.WorkTree, "sub/a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			FilePath file2 = new FilePath(db.WorkTree, "sub/b.txt");
			file2.CreateNewFile();
			writer = new PrintWriter(file2);
			writer.Write("content b");
			writer.Close();
			Git git = new Git(db);
			git.Add().AddFilepattern(".").Call();
			NUnit.Framework.Assert.AreEqual("[sub/a.txt, mode:100644, content:content]" + "[sub/b.txt, mode:100644, content:content b]"
				, IndexState(CONTENT));
		}

		// the same three cases as in testAddWithParameterUpdate
		// file a exists in workdir and in index -> added
		// file b exists not in workdir but in index -> unchanged
		// file c exists in workdir but not in index -> added
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddWithoutParameterUpdate()
		{
			new FilePath(db.WorkTree, "sub").Mkdir();
			FilePath file = new FilePath(db.WorkTree, "sub/a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			FilePath file2 = new FilePath(db.WorkTree, "sub/b.txt");
			file2.CreateNewFile();
			writer = new PrintWriter(file2);
			writer.Write("content b");
			writer.Close();
			Git git = new Git(db);
			git.Add().AddFilepattern("sub").Call();
			NUnit.Framework.Assert.AreEqual("[sub/a.txt, mode:100644, content:content]" + "[sub/b.txt, mode:100644, content:content b]"
				, IndexState(CONTENT));
			git.Commit().SetMessage("commit").Call();
			// new unstaged file sub/c.txt
			FilePath file3 = new FilePath(db.WorkTree, "sub/c.txt");
			file3.CreateNewFile();
			writer = new PrintWriter(file3);
			writer.Write("content c");
			writer.Close();
			// file sub/a.txt is modified
			writer = new PrintWriter(file);
			writer.Write("modified content");
			writer.Close();
			// file sub/b.txt is deleted
			FileUtils.Delete(file2);
			git.Add().AddFilepattern("sub").Call();
			// change in sub/a.txt is staged
			// deletion of sub/b.txt is not staged
			// sub/c.txt is staged
			NUnit.Framework.Assert.AreEqual("[sub/a.txt, mode:100644, content:modified content]"
				 + "[sub/b.txt, mode:100644, content:content b]" + "[sub/c.txt, mode:100644, content:content c]"
				, IndexState(CONTENT));
		}

		// file a exists in workdir and in index -> added
		// file b exists not in workdir but in index -> deleted
		// file c exists in workdir but not in index -> unchanged
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddWithParameterUpdate()
		{
			new FilePath(db.WorkTree, "sub").Mkdir();
			FilePath file = new FilePath(db.WorkTree, "sub/a.txt");
			file.CreateNewFile();
			PrintWriter writer = new PrintWriter(file);
			writer.Write("content");
			writer.Close();
			FilePath file2 = new FilePath(db.WorkTree, "sub/b.txt");
			file2.CreateNewFile();
			writer = new PrintWriter(file2);
			writer.Write("content b");
			writer.Close();
			Git git = new Git(db);
			git.Add().AddFilepattern("sub").Call();
			NUnit.Framework.Assert.AreEqual("[sub/a.txt, mode:100644, content:content]" + "[sub/b.txt, mode:100644, content:content b]"
				, IndexState(CONTENT));
			git.Commit().SetMessage("commit").Call();
			// new unstaged file sub/c.txt
			FilePath file3 = new FilePath(db.WorkTree, "sub/c.txt");
			file3.CreateNewFile();
			writer = new PrintWriter(file3);
			writer.Write("content c");
			writer.Close();
			// file sub/a.txt is modified
			writer = new PrintWriter(file);
			writer.Write("modified content");
			writer.Close();
			FileUtils.Delete(file2);
			// change in sub/a.txt is staged
			// deletion of sub/b.txt is staged
			// sub/c.txt is not staged
			git.Add().AddFilepattern("sub").SetUpdate(true).Call();
			// change in sub/a.txt is staged
			NUnit.Framework.Assert.AreEqual("[sub/a.txt, mode:100644, content:modified content]"
				, IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAssumeUnchanged()
		{
			Git git = new Git(db);
			string path = "a.txt";
			WriteTrashFile(path, "content");
			git.Add().AddFilepattern(path).Call();
			string path2 = "b.txt";
			WriteTrashFile(path2, "content");
			git.Add().AddFilepattern(path2).Call();
			git.Commit().SetMessage("commit").Call();
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, content:" + "content, assume-unchanged:false]"
				 + "[b.txt, mode:100644, content:content, " + "assume-unchanged:false]", IndexState
				(CONTENT | ASSUME_UNCHANGED));
			AssumeUnchanged(path2);
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, content:content, " + "assume-unchanged:false][b.txt, mode:100644, "
				 + "content:content, assume-unchanged:true]", IndexState(CONTENT | ASSUME_UNCHANGED
				));
			WriteTrashFile(path, "more content");
			WriteTrashFile(path2, "more content");
			git.Add().AddFilepattern(".").Call();
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, content:more content," + " assume-unchanged:false][b.txt, mode:100644,"
				 + string.Empty + string.Empty + " content:content, assume-unchanged:true]", IndexState
				(CONTENT | ASSUME_UNCHANGED));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private DirCacheEntry AddEntryToBuilder(string path, FilePath file, ObjectInserter
			 newObjectInserter, DirCacheBuilder builder, int stage)
		{
			FileInputStream inputStream = new FileInputStream(file);
			ObjectId id = newObjectInserter.Insert(Constants.OBJ_BLOB, file.Length(), inputStream
				);
			inputStream.Close();
			DirCacheEntry entry = new DirCacheEntry(path, stage);
			entry.SetObjectId(id);
			entry.FileMode = FileMode.REGULAR_FILE;
			entry.LastModified = file.LastModified();
			entry.SetLength((int)file.Length());
			builder.Add(entry);
			return entry;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssumeUnchanged(string path)
		{
			DirCache dirc = db.LockDirCache();
			DirCacheEntry ent = dirc.GetEntry(path);
			if (ent != null)
			{
				ent.IsAssumeValid = true;
			}
			dirc.Write();
			if (!dirc.Commit())
			{
				throw new IOException("could not commit");
			}
		}
	}
}
