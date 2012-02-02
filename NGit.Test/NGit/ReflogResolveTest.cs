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
using NGit.Errors;
using NGit.Revwalk;
using Sharpen;

namespace NGit
{
	/// <summary>Unit tests for resolving reflog-based revisions</summary>
	[NUnit.Framework.TestFixture]
	public class ReflogResolveTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ResolveMasterCommits()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit c1 = git.Commit().SetMessage("create file").Call();
			WriteTrashFile("file.txt", "content2");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit c2 = git.Commit().SetMessage("edit file").Call();
			NUnit.Framework.Assert.AreEqual(c2, db.Resolve("master@{0}"));
			NUnit.Framework.Assert.AreEqual(c1, db.Resolve("master@{1}"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ResolveReflogParent()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit c1 = git.Commit().SetMessage("create file").Call();
			WriteTrashFile("file.txt", "content2");
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("edit file").Call();
			NUnit.Framework.Assert.AreEqual(c1, db.Resolve("master@{0}~1"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ResolveNonExistingBranch()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("create file").Call();
			NUnit.Framework.Assert.IsNull(db.Resolve("notabranch@{7}"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ResolveNegativeEntryNumber()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("create file").Call();
			try
			{
				db.Resolve("master@{-12}");
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (RevisionSyntaxException e)
			{
				NUnit.Framework.Assert.IsNotNull(e);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ResolveDate()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			git.Commit().SetMessage("create file").Call();
			try
			{
				db.Resolve("master@{yesterday}");
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (RevisionSyntaxException e)
			{
				NUnit.Framework.Assert.IsNotNull(e);
			}
		}
	}
}
