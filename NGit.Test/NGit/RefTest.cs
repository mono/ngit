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
using NGit.Junit;
using Sharpen;

namespace NGit
{
	/// <summary>Misc tests for refs.</summary>
	/// <remarks>
	/// Misc tests for refs. A lot of things are tested elsewhere so not having a
	/// test for a ref related method, does not mean it is untested.
	/// </remarks>
	[NUnit.Framework.TestFixture]
	public class RefTest : SampleDataRepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		private void WriteSymref(string src, string dst)
		{
			RefUpdate u = db.UpdateRef(src);
			switch (u.Link(dst))
			{
				case RefUpdate.Result.NEW:
				case RefUpdate.Result.FORCED:
				case RefUpdate.Result.NO_CHANGE:
				{
					break;
				}

				default:
				{
					NUnit.Framework.Assert.Fail("link " + src + " to " + dst);
					break;
				}
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadAllIncludingSymrefs()
		{
			ObjectId masterId = db.Resolve("refs/heads/master");
			RefUpdate updateRef = db.UpdateRef("refs/remotes/origin/master");
			updateRef.SetNewObjectId(masterId);
			updateRef.SetForceUpdate(true);
			updateRef.Update();
			WriteSymref("refs/remotes/origin/HEAD", "refs/remotes/origin/master");
			ObjectId r = db.Resolve("refs/remotes/origin/HEAD");
			NUnit.Framework.Assert.AreEqual(masterId, r);
			IDictionary<string, Ref> allRefs = db.GetAllRefs();
			Ref refHEAD = allRefs.Get("refs/remotes/origin/HEAD");
			NUnit.Framework.Assert.IsNotNull(refHEAD);
			NUnit.Framework.Assert.AreEqual(masterId, refHEAD.GetObjectId());
			NUnit.Framework.Assert.IsFalse(refHEAD.IsPeeled());
			NUnit.Framework.Assert.IsNull(refHEAD.GetPeeledObjectId());
			Ref refmaster = allRefs.Get("refs/remotes/origin/master");
			NUnit.Framework.Assert.AreEqual(masterId, refmaster.GetObjectId());
			NUnit.Framework.Assert.IsFalse(refmaster.IsPeeled());
			NUnit.Framework.Assert.IsNull(refmaster.GetPeeledObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadSymRefToPacked()
		{
			WriteSymref("HEAD", "refs/heads/b");
			Ref @ref = db.GetRef("HEAD");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
			NUnit.Framework.Assert.IsTrue(@ref.IsSymbolic(), "is symref");
			@ref = @ref.GetTarget();
			NUnit.Framework.Assert.AreEqual("refs/heads/b", @ref.GetName());
			NUnit.Framework.Assert.AreEqual(RefStorage.PACKED, @ref.GetStorage());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadSymRefToLoosePacked()
		{
			ObjectId pid = db.Resolve("refs/heads/master^");
			RefUpdate updateRef = db.UpdateRef("refs/heads/master");
			updateRef.SetNewObjectId(pid);
			updateRef.SetForceUpdate(true);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update);
			// internal
			WriteSymref("HEAD", "refs/heads/master");
			Ref @ref = db.GetRef("HEAD");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
			@ref = @ref.GetTarget();
			NUnit.Framework.Assert.AreEqual("refs/heads/master", @ref.GetName());
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadLooseRef()
		{
			RefUpdate updateRef = db.UpdateRef("ref/heads/new");
			updateRef.SetNewObjectId(db.Resolve("refs/heads/master"));
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, update);
			Ref @ref = db.GetRef("ref/heads/new");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
		}

		/// <summary>Let an "outsider" create a loose ref with the same name as a packed one</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestReadLoosePackedRef()
		{
			Ref @ref = db.GetRef("refs/heads/master");
			NUnit.Framework.Assert.AreEqual(RefStorage.PACKED, @ref.GetStorage());
			FileOutputStream os = new FileOutputStream(new FilePath(db.Directory, "refs/heads/master"
				));
			os.Write(Sharpen.Runtime.GetBytesForString(@ref.GetObjectId().Name));
			os.Write('\n');
			os.Close();
			@ref = db.GetRef("refs/heads/master");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
		}

		/// <summary>Modify a packed ref using the API.</summary>
		/// <remarks>
		/// Modify a packed ref using the API. This creates a loose ref too, ie.
		/// LOOSE_PACKED
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestReadSimplePackedRefSameRepo()
		{
			Ref @ref = db.GetRef("refs/heads/master");
			ObjectId pid = db.Resolve("refs/heads/master^");
			NUnit.Framework.Assert.AreEqual(RefStorage.PACKED, @ref.GetStorage());
			RefUpdate updateRef = db.UpdateRef("refs/heads/master");
			updateRef.SetNewObjectId(pid);
			updateRef.SetForceUpdate(true);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update);
			@ref = db.GetRef("refs/heads/master");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestResolvedNamesBranch()
		{
			Ref @ref = db.GetRef("a");
			NUnit.Framework.Assert.AreEqual("refs/heads/a", @ref.GetName());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestResolvedSymRef()
		{
			Ref @ref = db.GetRef(Constants.HEAD);
			NUnit.Framework.Assert.AreEqual(Constants.HEAD, @ref.GetName());
			NUnit.Framework.Assert.IsTrue(@ref.IsSymbolic(), "is symbolic ref");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
			Ref dst = @ref.GetTarget();
			NUnit.Framework.Assert.IsNotNull(dst, "has target");
			NUnit.Framework.Assert.AreEqual("refs/heads/master", dst.GetName());
			NUnit.Framework.Assert.AreSame(dst.GetObjectId(), @ref.GetObjectId());
			NUnit.Framework.Assert.AreSame(dst.GetPeeledObjectId(), @ref.GetPeeledObjectId());
			NUnit.Framework.Assert.AreEqual(dst.IsPeeled(), @ref.IsPeeled());
		}
	}
}
