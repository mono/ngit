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
using NGit.Ignore;
using NGit.Treewalk;
using NGit.Util;
using Sharpen;

namespace NGit.Ignore
{
	/// <summary>Tests ignore node behavior on the local filesystem.</summary>
	/// <remarks>Tests ignore node behavior on the local filesystem.</remarks>
	public class IgnoreNodeTest : RepositoryTestCase
	{
		private static readonly FileMode D = FileMode.TREE;

		private static readonly FileMode F = FileMode.REGULAR_FILE;

		private const bool ignored = true;

		private const bool tracked = false;

		private TreeWalk walk;

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRules()
		{
			WriteIgnoreFile(".git/info/exclude", "*~", "/out");
			WriteIgnoreFile(".gitignore", "*.o", "/config");
			WriteTrashFile("config/secret", string.Empty);
			WriteTrashFile("mylib.c", string.Empty);
			WriteTrashFile("mylib.c~", string.Empty);
			WriteTrashFile("mylib.o", string.Empty);
			WriteTrashFile("out/object/foo.exe", string.Empty);
			WriteIgnoreFile("src/config/.gitignore", "lex.out");
			WriteTrashFile("src/config/lex.out", string.Empty);
			WriteTrashFile("src/config/config.c", string.Empty);
			WriteTrashFile("src/config/config.c~", string.Empty);
			WriteTrashFile("src/config/old/lex.out", string.Empty);
			BeginWalk();
			AssertEntry(F, tracked, ".gitignore");
			AssertEntry(D, ignored, "config");
			AssertEntry(F, ignored, "config/secret");
			AssertEntry(F, tracked, "mylib.c");
			AssertEntry(F, ignored, "mylib.c~");
			AssertEntry(F, ignored, "mylib.o");
			AssertEntry(D, ignored, "out");
			AssertEntry(D, ignored, "out/object");
			AssertEntry(F, ignored, "out/object/foo.exe");
			AssertEntry(D, tracked, "src");
			AssertEntry(D, tracked, "src/config");
			AssertEntry(F, tracked, "src/config/.gitignore");
			AssertEntry(F, tracked, "src/config/config.c");
			AssertEntry(F, ignored, "src/config/config.c~");
			AssertEntry(F, ignored, "src/config/lex.out");
			AssertEntry(D, tracked, "src/config/old");
			AssertEntry(F, ignored, "src/config/old/lex.out");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNegation()
		{
			WriteIgnoreFile(".gitignore", "*.o");
			WriteIgnoreFile("src/a/b/.gitignore", "!keep.o");
			WriteTrashFile("src/a/b/keep.o", string.Empty);
			WriteTrashFile("src/a/b/nothere.o", string.Empty);
			BeginWalk();
			AssertEntry(F, tracked, ".gitignore");
			AssertEntry(D, tracked, "src");
			AssertEntry(D, tracked, "src/a");
			AssertEntry(D, tracked, "src/a/b");
			AssertEntry(F, tracked, "src/a/b/.gitignore");
			AssertEntry(F, tracked, "src/a/b/keep.o");
			AssertEntry(F, ignored, "src/a/b/nothere.o");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSlashOnlyMatchesDirectory()
		{
			WriteIgnoreFile(".gitignore", "out/");
			WriteTrashFile("out", string.Empty);
			BeginWalk();
			AssertEntry(F, tracked, ".gitignore");
			AssertEntry(F, tracked, "out");
			FileUtils.Delete(new FilePath(trash, "out"));
			WriteTrashFile("out/foo", string.Empty);
			BeginWalk();
			AssertEntry(F, tracked, ".gitignore");
			AssertEntry(D, ignored, "out");
			AssertEntry(F, ignored, "out/foo");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWithSlashDoesNotMatchInSubDirectory()
		{
			WriteIgnoreFile(".gitignore", "a/b");
			WriteTrashFile("a/a", string.Empty);
			WriteTrashFile("a/b", string.Empty);
			WriteTrashFile("src/a/a", string.Empty);
			WriteTrashFile("src/a/b", string.Empty);
			BeginWalk();
			AssertEntry(F, tracked, ".gitignore");
			AssertEntry(D, tracked, "a");
			AssertEntry(F, tracked, "a/a");
			AssertEntry(F, ignored, "a/b");
			AssertEntry(D, tracked, "src");
			AssertEntry(D, tracked, "src/a");
			AssertEntry(F, tracked, "src/a/a");
			AssertEntry(F, tracked, "src/a/b");
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		private void BeginWalk()
		{
			walk = new TreeWalk(db);
			walk.AddTree(new FileTreeIterator(db));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertEntry(FileMode type, bool entryIgnored, string pathName)
		{
			NUnit.Framework.Assert.IsTrue(walk.Next(), "walk has entry");
			NUnit.Framework.Assert.AreEqual(pathName, walk.PathString);
			NUnit.Framework.Assert.AreEqual(type, walk.GetFileMode(0));
			WorkingTreeIterator itr = walk.GetTree<WorkingTreeIterator>(0);
			NUnit.Framework.Assert.IsNotNull(itr, "has tree");
			NUnit.Framework.Assert.AreEqual(entryIgnored, itr.IsEntryIgnored(), "is ignored");
			if (D.Equals(type))
			{
				walk.EnterSubtree();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WriteIgnoreFile(string name, params string[] rules)
		{
			StringBuilder data = new StringBuilder();
			foreach (string line in rules)
			{
				data.Append(line + "\n");
			}
			WriteTrashFile(name, data.ToString());
		}
	}
}
