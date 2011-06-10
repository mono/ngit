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
using NGit.Api.Blame;
using NGit.Blame;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Api.Blame
{
	/// <summary>
	/// Unit tests of
	/// <see cref="NGit.Blame.BlameGenerator">NGit.Blame.BlameGenerator</see>
	/// .
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class BlameGeneratorTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBoundLineDelete()
		{
			Git git = new Git(db);
			string[] content1 = new string[] { "first", "second" };
			WriteTrashFile("file.txt", Join(content1));
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit c1 = git.Commit().SetMessage("create file").Call();
			string[] content2 = new string[] { "third", "first", "second" };
			WriteTrashFile("file.txt", Join(content2));
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit c2 = git.Commit().SetMessage("create file").Call();
			BlameGenerator generator = new BlameGenerator(db, "file.txt");
			try
			{
				generator.Push(null, db.Resolve(Constants.HEAD));
				NUnit.Framework.Assert.AreEqual(3, generator.GetResultContents().Size());
				NUnit.Framework.Assert.IsTrue(generator.Next());
				NUnit.Framework.Assert.AreEqual(c2, generator.GetSourceCommit());
				NUnit.Framework.Assert.AreEqual(1, generator.GetRegionLength());
				NUnit.Framework.Assert.AreEqual(0, generator.GetResultStart());
				NUnit.Framework.Assert.AreEqual(1, generator.GetResultEnd());
				NUnit.Framework.Assert.AreEqual(0, generator.GetSourceStart());
				NUnit.Framework.Assert.AreEqual(1, generator.GetSourceEnd());
				NUnit.Framework.Assert.AreEqual("file.txt", generator.GetSourcePath());
				NUnit.Framework.Assert.IsTrue(generator.Next());
				NUnit.Framework.Assert.AreEqual(c1, generator.GetSourceCommit());
				NUnit.Framework.Assert.AreEqual(2, generator.GetRegionLength());
				NUnit.Framework.Assert.AreEqual(1, generator.GetResultStart());
				NUnit.Framework.Assert.AreEqual(3, generator.GetResultEnd());
				NUnit.Framework.Assert.AreEqual(0, generator.GetSourceStart());
				NUnit.Framework.Assert.AreEqual(2, generator.GetSourceEnd());
				NUnit.Framework.Assert.AreEqual("file.txt", generator.GetSourcePath());
				NUnit.Framework.Assert.IsFalse(generator.Next());
			}
			finally
			{
				generator.Release();
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLinesAllDeletedShortenedWalk()
		{
			Git git = new Git(db);
			string[] content1 = new string[] { "first", "second", "third" };
			WriteTrashFile("file.txt", Join(content1));
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("create file").Call();
			string[] content2 = new string[] { string.Empty };
			WriteTrashFile("file.txt", Join(content2));
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("create file").Call();
			WriteTrashFile("file.txt", Join(content1));
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit c3 = git.Commit().SetMessage("create file").Call();
			BlameGenerator generator = new BlameGenerator(db, "file.txt");
			try
			{
				generator.Push(null, db.Resolve(Constants.HEAD));
				NUnit.Framework.Assert.AreEqual(3, generator.GetResultContents().Size());
				NUnit.Framework.Assert.IsTrue(generator.Next());
				NUnit.Framework.Assert.AreEqual(c3, generator.GetSourceCommit());
				NUnit.Framework.Assert.AreEqual(0, generator.GetResultStart());
				NUnit.Framework.Assert.AreEqual(3, generator.GetResultEnd());
				NUnit.Framework.Assert.IsFalse(generator.Next());
			}
			finally
			{
				generator.Release();
			}
		}

		private static string Join(params string[] lines)
		{
			StringBuilder joined = new StringBuilder();
			foreach (string line in lines)
			{
				joined.Append(line).Append('\n');
			}
			return joined.ToString();
		}
	}
}
