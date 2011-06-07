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
using NGit.Revwalk;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Unit tests of path-based uses of
	/// <see cref="CheckoutCommand">CheckoutCommand</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class PathCheckoutCommandTest : RepositoryTestCase
	{
		private static readonly string FILE1 = "f/Test.txt";

		private static readonly string FILE2 = "Test2.txt";

		internal Git git;

		internal RevCommit initialCommit;

		internal RevCommit secondCommit;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			git = new Git(db);
			WriteTrashFile(FILE1, "1");
			WriteTrashFile(FILE2, "a");
			git.Add().AddFilepattern(FILE1).AddFilepattern(FILE2).Call();
			initialCommit = git.Commit().SetMessage("Initial commit").Call();
			WriteTrashFile(FILE1, "2");
			WriteTrashFile(FILE2, "b");
			git.Add().AddFilepattern(FILE1).AddFilepattern(FILE2).Call();
			secondCommit = git.Commit().SetMessage("Second commit").Call();
			WriteTrashFile(FILE1, "3");
			WriteTrashFile(FILE2, "c");
			git.Add().AddFilepattern(FILE1).AddFilepattern(FILE2).Call();
			git.Commit().SetMessage("Third commit").Call();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateWorkingDirectory()
		{
			CheckoutCommand co = git.Checkout();
			FilePath written = WriteTrashFile(FILE1, string.Empty);
			NUnit.Framework.Assert.AreEqual(string.Empty, Read(written));
			co.AddPath(FILE1).Call();
			NUnit.Framework.Assert.AreEqual("3", Read(written));
			NUnit.Framework.Assert.AreEqual("c", Read(new FilePath(db.WorkTree, FILE2)));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutFirst()
		{
			CheckoutCommand co = git.Checkout();
			FilePath written = WriteTrashFile(FILE1, string.Empty);
			co.SetStartPoint(initialCommit).AddPath(FILE1).Call();
			NUnit.Framework.Assert.AreEqual("1", Read(written));
			NUnit.Framework.Assert.AreEqual("c", Read(new FilePath(db.WorkTree, FILE2)));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutSecond()
		{
			CheckoutCommand co = git.Checkout();
			FilePath written = WriteTrashFile(FILE1, string.Empty);
			co.SetStartPoint("HEAD~1").AddPath(FILE1).Call();
			NUnit.Framework.Assert.AreEqual("2", Read(written));
			NUnit.Framework.Assert.AreEqual("c", Read(new FilePath(db.WorkTree, FILE2)));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckoutMultiple()
		{
			CheckoutCommand co = git.Checkout();
			FilePath test = WriteTrashFile(FILE1, string.Empty);
			FilePath test2 = WriteTrashFile(FILE2, string.Empty);
			co.SetStartPoint("HEAD~2").AddPath(FILE1).AddPath(FILE2).Call();
			NUnit.Framework.Assert.AreEqual("1", Read(test));
			NUnit.Framework.Assert.AreEqual("a", Read(test2));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateWorkingDirectoryFromIndex()
		{
			CheckoutCommand co = git.Checkout();
			FilePath written = WriteTrashFile(FILE1, "3a");
			git.Add().AddFilepattern(FILE1).Call();
			written = WriteTrashFile(FILE1, string.Empty);
			NUnit.Framework.Assert.AreEqual(string.Empty, Read(written));
			co.AddPath(FILE1).Call();
			NUnit.Framework.Assert.AreEqual("3a", Read(written));
			NUnit.Framework.Assert.AreEqual("c", Read(new FilePath(db.WorkTree, FILE2)));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateWorkingDirectoryFromHeadWithIndexChange()
		{
			CheckoutCommand co = git.Checkout();
			FilePath written = WriteTrashFile(FILE1, "3a");
			git.Add().AddFilepattern(FILE1).Call();
			written = WriteTrashFile(FILE1, string.Empty);
			NUnit.Framework.Assert.AreEqual(string.Empty, Read(written));
			co.AddPath(FILE1).SetStartPoint("HEAD").Call();
			NUnit.Framework.Assert.AreEqual("3", Read(written));
			NUnit.Framework.Assert.AreEqual("c", Read(new FilePath(db.WorkTree, FILE2)));
		}
	}
}
