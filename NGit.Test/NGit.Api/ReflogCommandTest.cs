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
using NGit.Revwalk;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class ReflogCommandTest : RepositoryTestCase
	{
		private Git git;

		private RevCommit commit1;

		private RevCommit commit2;

		private static readonly string FILE = "test.txt";

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			git = new Git(db);
			// commit something
			WriteTrashFile(FILE, "Hello world");
			git.Add().AddFilepattern(FILE).Call();
			commit1 = git.Commit().SetMessage("Initial commit").Call();
			git.Checkout().SetCreateBranch(true).SetName("b1").Call();
			git.Rm().AddFilepattern(FILE).Call();
			commit2 = git.Commit().SetMessage("Removed file").Call();
			git.NotesAdd().SetObjectId(commit1).SetMessage("data").Call();
		}

		/// <summary>Test getting the HEAD reflog</summary>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestHeadReflog()
		{
			ICollection<ReflogEntry> reflog = git.Reflog().Call();
			NUnit.Framework.Assert.IsNotNull(reflog);
			NUnit.Framework.Assert.AreEqual(3, reflog.Count);
			ReflogEntry[] reflogs = Sharpen.Collections.ToArray(reflog, new ReflogEntry[reflog
				.Count]);
			NUnit.Framework.Assert.AreEqual(reflogs[2].GetComment(), "commit: Initial commit"
				);
			NUnit.Framework.Assert.AreEqual(reflogs[2].GetNewId(), commit1.Id);
			NUnit.Framework.Assert.AreEqual(reflogs[2].GetOldId(), ObjectId.ZeroId);
			NUnit.Framework.Assert.AreEqual(reflogs[1].GetNewId(), commit1.Id);
			NUnit.Framework.Assert.AreEqual(reflogs[1].GetOldId(), commit1.Id);
			NUnit.Framework.Assert.AreEqual(reflogs[0].GetComment(), "commit: Removed file");
			NUnit.Framework.Assert.AreEqual(reflogs[0].GetNewId(), commit2.Id);
			NUnit.Framework.Assert.AreEqual(reflogs[0].GetOldId(), commit1.Id);
		}

		/// <summary>Test getting the reflog for an explicit branch</summary>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestBranchReflog()
		{
			ICollection<ReflogEntry> reflog = git.Reflog().SetRef(Constants.R_HEADS + "b1").Call
				();
			NUnit.Framework.Assert.IsNotNull(reflog);
			NUnit.Framework.Assert.AreEqual(2, reflog.Count);
			ReflogEntry[] reflogs = Sharpen.Collections.ToArray(reflog, new ReflogEntry[reflog
				.Count]);
			NUnit.Framework.Assert.AreEqual(reflogs[0].GetComment(), "commit: Removed file");
			NUnit.Framework.Assert.AreEqual(reflogs[0].GetNewId(), commit2.Id);
			NUnit.Framework.Assert.AreEqual(reflogs[0].GetOldId(), commit1.Id);
			NUnit.Framework.Assert.AreEqual(reflogs[1].GetNewId(), commit1.Id);
			NUnit.Framework.Assert.AreEqual(reflogs[1].GetOldId(), ObjectId.ZeroId);
		}
	}
}
