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
using NGit.Merge;
using Sharpen;

namespace NGit.Merge
{
	/// <summary>
	/// Test construction of merge message by
	/// <see cref="MergeMessageFormatter">MergeMessageFormatter</see>
	/// .
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class MergeMessageFormatterTest : SampleDataRepositoryTestCase
	{
		private MergeMessageFormatter formatter;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			RefUpdate createRemoteRefA = db.UpdateRef("refs/remotes/origin/remote-a");
			createRemoteRefA.SetNewObjectId(db.Resolve("refs/heads/a"));
			createRemoteRefA.Update();
			RefUpdate createRemoteRefB = db.UpdateRef("refs/remotes/origin/remote-b");
			createRemoteRefB.SetNewObjectId(db.Resolve("refs/heads/b"));
			createRemoteRefB.Update();
			formatter = new MergeMessageFormatter();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOneBranch()
		{
			Ref a = db.GetRef("refs/heads/a");
			Ref master = db.GetRef("refs/heads/master");
			string message = formatter.Format(Arrays.AsList(a), master);
			NUnit.Framework.Assert.AreEqual("Merge branch 'a'", message);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoBranches()
		{
			Ref a = db.GetRef("refs/heads/a");
			Ref b = db.GetRef("refs/heads/b");
			Ref master = db.GetRef("refs/heads/master");
			string message = formatter.Format(Arrays.AsList(a, b), master);
			NUnit.Framework.Assert.AreEqual("Merge branches 'a' and 'b'", message);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestThreeBranches()
		{
			Ref c = db.GetRef("refs/heads/c");
			Ref b = db.GetRef("refs/heads/b");
			Ref a = db.GetRef("refs/heads/a");
			Ref master = db.GetRef("refs/heads/master");
			string message = formatter.Format(Arrays.AsList(c, b, a), master);
			NUnit.Framework.Assert.AreEqual("Merge branches 'c', 'b' and 'a'", message);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRemoteBranch()
		{
			Ref remoteA = db.GetRef("refs/remotes/origin/remote-a");
			Ref master = db.GetRef("refs/heads/master");
			string message = formatter.Format(Arrays.AsList(remoteA), master);
			NUnit.Framework.Assert.AreEqual("Merge remote branch 'origin/remote-a'", message);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMixed()
		{
			Ref c = db.GetRef("refs/heads/c");
			Ref remoteA = db.GetRef("refs/remotes/origin/remote-a");
			Ref master = db.GetRef("refs/heads/master");
			string message = formatter.Format(Arrays.AsList(c, remoteA), master);
			NUnit.Framework.Assert.AreEqual("Merge branch 'c', remote branch 'origin/remote-a'"
				, message);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTag()
		{
			Ref tagA = db.GetRef("refs/tags/A");
			Ref master = db.GetRef("refs/heads/master");
			string message = formatter.Format(Arrays.AsList(tagA), master);
			NUnit.Framework.Assert.AreEqual("Merge tag 'A'", message);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCommit()
		{
			ObjectId objectId = ObjectId.FromString("6db9c2ebf75590eef973081736730a9ea169a0c4"
				);
			Ref commit = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, objectId.GetName(), objectId
				);
			Ref master = db.GetRef("refs/heads/master");
			string message = formatter.Format(Arrays.AsList(commit), master);
			NUnit.Framework.Assert.AreEqual("Merge commit '6db9c2ebf75590eef973081736730a9ea169a0c4'"
				, message);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPullWithUri()
		{
			string name = "branch 'test' of http://egit.eclipse.org/jgit.git";
			ObjectId objectId = ObjectId.FromString("6db9c2ebf75590eef973081736730a9ea169a0c4"
				);
			Ref remoteBranch = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, name, objectId);
			Ref master = db.GetRef("refs/heads/master");
			string message = formatter.Format(Arrays.AsList(remoteBranch), master);
			NUnit.Framework.Assert.AreEqual("Merge branch 'test' of http://egit.eclipse.org/jgit.git"
				, message);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIntoOtherThanMaster()
		{
			Ref a = db.GetRef("refs/heads/a");
			Ref b = db.GetRef("refs/heads/b");
			string message = formatter.Format(Arrays.AsList(a), b);
			NUnit.Framework.Assert.AreEqual("Merge branch 'a' into b", message);
		}
	}
}
