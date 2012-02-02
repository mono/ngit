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
using NGit.Api.Errors;
using NGit.Errors;
using NGit.Revwalk;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>
	/// Unit tests of
	/// <see cref="LockFile">LockFile</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class LockFileTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void LockFailedExceptionRecovery()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit1 = git.Commit().SetMessage("create file").Call();
			NUnit.Framework.Assert.IsNotNull(commit1);
			WriteTrashFile("file.txt", "content2");
			git.Add().AddFilepattern("file.txt").Call();
			NUnit.Framework.Assert.IsNotNull(git.Commit().SetMessage("edit file").Call());
			NUnit.Framework.Assert.IsTrue(new LockFile(db.GetIndexFile(), db.FileSystem).Lock
				());
			try
			{
				git.Checkout().SetName(commit1.Name).Call();
				NUnit.Framework.Assert.Fail("JGitInternalException not thrown");
			}
			catch (JGitInternalException e)
			{
				NUnit.Framework.Assert.IsTrue(e.InnerException is LockFailedException);
				LockFile.Unlock(((LockFailedException)e.InnerException).GetFile());
				git.Checkout().SetName(commit1.Name).Call();
			}
		}
	}
}
