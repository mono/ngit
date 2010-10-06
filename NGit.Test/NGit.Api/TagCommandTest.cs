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

using System;
using NGit;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Api
{
	public class TagCommandTest : RepositoryTestCase
	{
		/// <exception cref="NGit.Api.Errors.NoHeadException"></exception>
		/// <exception cref="NGit.Api.Errors.NoMessageException"></exception>
		/// <exception cref="NGit.Errors.UnmergedPathException"></exception>
		/// <exception cref="NGit.Api.Errors.ConcurrentRefUpdateException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.WrongRepositoryStateException"></exception>
		/// <exception cref="NGit.Api.Errors.InvalidTagNameException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTaggingOnHead()
		{
			Git git = new Git(db);
			RevCommit commit = git.Commit().SetMessage("initial commit").Call();
			RevTag tag = git.Tag().SetName("tag").Call();
			AssertEquals(commit.Id, tag.GetObject().Id);
		}

		/// <exception cref="NGit.Api.Errors.NoHeadException"></exception>
		/// <exception cref="NGit.Api.Errors.NoMessageException"></exception>
		/// <exception cref="NGit.Errors.UnmergedPathException"></exception>
		/// <exception cref="NGit.Api.Errors.ConcurrentRefUpdateException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.WrongRepositoryStateException"></exception>
		/// <exception cref="NGit.Api.Errors.InvalidTagNameException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTagging()
		{
			Git git = new Git(db);
			git.Commit().SetMessage("initial commit").Call();
			RevCommit commit = git.Commit().SetMessage("second commit").Call();
			git.Commit().SetMessage("third commit").Call();
			RevTag tag = git.Tag().SetObjectId(commit).SetName("tag").Call();
			AssertEquals(commit.Id, tag.GetObject().Id);
		}

		/// <exception cref="NGit.Api.Errors.NoHeadException"></exception>
		/// <exception cref="NGit.Api.Errors.NoMessageException"></exception>
		/// <exception cref="NGit.Errors.UnmergedPathException"></exception>
		/// <exception cref="NGit.Api.Errors.ConcurrentRefUpdateException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.WrongRepositoryStateException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyTagName()
		{
			Git git = new Git(db);
			git.Commit().SetMessage("initial commit").Call();
			try
			{
				// forget to tag name
				git.Tag().SetMessage("some message").Call();
				NUnit.Framework.Assert.Fail("We should have failed without a tag name");
			}
			catch (InvalidTagNameException)
			{
			}
		}

		// should hit here
		/// <exception cref="NGit.Api.Errors.NoHeadException"></exception>
		/// <exception cref="NGit.Api.Errors.NoMessageException"></exception>
		/// <exception cref="NGit.Errors.UnmergedPathException"></exception>
		/// <exception cref="NGit.Api.Errors.ConcurrentRefUpdateException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.WrongRepositoryStateException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestInvalidTagName()
		{
			Git git = new Git(db);
			git.Commit().SetMessage("initial commit").Call();
			try
			{
				git.Tag().SetName("bad~tag~name").SetMessage("some message").Call();
				NUnit.Framework.Assert.Fail("We should have failed due to a bad tag name");
			}
			catch (InvalidTagNameException)
			{
			}
		}

		// should hit here
		/// <exception cref="NGit.Api.Errors.NoHeadException"></exception>
		/// <exception cref="NGit.Api.Errors.NoMessageException"></exception>
		/// <exception cref="NGit.Errors.UnmergedPathException"></exception>
		/// <exception cref="NGit.Api.Errors.ConcurrentRefUpdateException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.WrongRepositoryStateException"></exception>
		/// <exception cref="NGit.Api.Errors.InvalidTagNameException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFailureOnSignedTags()
		{
			Git git = new Git(db);
			git.Commit().SetMessage("initial commit").Call();
			try
			{
				git.Tag().SetSigned(true).SetName("tag").Call();
				NUnit.Framework.Assert.Fail("We should have failed with an UnsupportedOperationException due to signed tag"
					);
			}
			catch (NotSupportedException)
			{
			}
		}
		// should hit here
	}
}
