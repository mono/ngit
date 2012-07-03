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
using NGit.Merge;
using NGit.Revwalk;
using NGit.Util;
using Sharpen;

namespace NGit.Merge
{
	/// <summary>
	/// Test construction of squash message by
	/// <see cref="SquashMessageFormatterTest">SquashMessageFormatterTest</see>
	/// .
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class SquashMessageFormatterTest : SampleDataRepositoryTestCase
	{
		private GitDateFormatter dateFormatter;

		private SquashMessageFormatter msgFormatter;

		private RevCommit revCommit;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			dateFormatter = new GitDateFormatter(GitDateFormatter.Format.DEFAULT);
			msgFormatter = new SquashMessageFormatter();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCommit()
		{
			Git git = new Git(db);
			revCommit = git.Commit().SetMessage("squash_me").Call();
			Ref master = db.GetRef("refs/heads/master");
			string message = msgFormatter.Format(Arrays.AsList(revCommit), master);
			NUnit.Framework.Assert.AreEqual("Squashed commit of the following:\n\ncommit " + 
				revCommit.GetName() + "\nAuthor: " + revCommit.GetAuthorIdent().GetName() + " <"
				 + revCommit.GetAuthorIdent().GetEmailAddress() + ">\nDate:   " + dateFormatter.
				FormatDate(author) + "\n\n\tsquash_me\n", message);
		}
	}
}
