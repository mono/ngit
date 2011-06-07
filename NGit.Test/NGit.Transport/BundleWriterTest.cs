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
using NGit.Errors;
using NGit.Revwalk;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	[NUnit.Framework.TestFixture]
	public class BundleWriterTest : SampleDataRepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWrite0()
		{
			// Create a tiny bundle, (well one of) the first commits only
			byte[] bundle = MakeBundle("refs/heads/firstcommit", "42e4e7c5e507e113ebbb7801b16b52cf867b7ce1"
				, null);
			// Then we clone a new repo from that bundle and do a simple test. This
			// makes sure
			// we could read the bundle we created.
			Repository newRepo = CreateBareRepository();
			FetchResult fetchResult = FetchFromBundle(newRepo, bundle);
			Ref advertisedRef = fetchResult.GetAdvertisedRef("refs/heads/firstcommit");
			// We expect firstcommit to appear by id
			NUnit.Framework.Assert.AreEqual("42e4e7c5e507e113ebbb7801b16b52cf867b7ce1", advertisedRef
				.GetObjectId().Name);
			// ..and by name as the bundle created a new ref
			NUnit.Framework.Assert.AreEqual("42e4e7c5e507e113ebbb7801b16b52cf867b7ce1", newRepo
				.Resolve("refs/heads/firstcommit").Name);
		}

		/// <summary>Incremental bundle test</summary>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestWrite1()
		{
			byte[] bundle;
			// Create a small bundle, an early commit
			bundle = MakeBundle("refs/heads/aa", db.Resolve("a").Name, null);
			// Then we clone a new repo from that bundle and do a simple test. This
			// makes sure
			// we could read the bundle we created.
			Repository newRepo = CreateBareRepository();
			FetchResult fetchResult = FetchFromBundle(newRepo, bundle);
			Ref advertisedRef = fetchResult.GetAdvertisedRef("refs/heads/aa");
			NUnit.Framework.Assert.AreEqual(db.Resolve("a").Name, advertisedRef.GetObjectId()
				.Name);
			NUnit.Framework.Assert.AreEqual(db.Resolve("a").Name, newRepo.Resolve("refs/heads/aa"
				).Name);
			NUnit.Framework.Assert.IsNull(newRepo.Resolve("refs/heads/a"));
			// Next an incremental bundle
			bundle = MakeBundle("refs/heads/cc", db.Resolve("c").Name, new RevWalk(db).ParseCommit
				(db.Resolve("a").ToObjectId()));
			fetchResult = FetchFromBundle(newRepo, bundle);
			advertisedRef = fetchResult.GetAdvertisedRef("refs/heads/cc");
			NUnit.Framework.Assert.AreEqual(db.Resolve("c").Name, advertisedRef.GetObjectId()
				.Name);
			NUnit.Framework.Assert.AreEqual(db.Resolve("c").Name, newRepo.Resolve("refs/heads/cc"
				).Name);
			NUnit.Framework.Assert.IsNull(newRepo.Resolve("refs/heads/c"));
			NUnit.Framework.Assert.IsNull(newRepo.Resolve("refs/heads/a"));
			// still unknown
			try
			{
				// Check that we actually needed the first bundle
				Repository newRepo2 = CreateBareRepository();
				fetchResult = FetchFromBundle(newRepo2, bundle);
				NUnit.Framework.Assert.Fail("We should not be able to fetch from bundle with prerequisites that are not fulfilled"
					);
			}
			catch (MissingBundlePrerequisiteException e)
			{
				NUnit.Framework.Assert.IsTrue(e.Message.IndexOf(db.Resolve("refs/heads/a").Name) 
					>= 0);
			}
		}

		/// <exception cref="Sharpen.URISyntaxException"></exception>
		/// <exception cref="System.NotSupportedException"></exception>
		/// <exception cref="NGit.Errors.TransportException"></exception>
		private FetchResult FetchFromBundle(Repository newRepo, byte[] bundle)
		{
			URIish uri = new URIish("in-memory://");
			ByteArrayInputStream @in = new ByteArrayInputStream(bundle);
			RefSpec rs = new RefSpec("refs/heads/*:refs/heads/*");
			ICollection<RefSpec> refs = Collections.Singleton(rs);
			return new TransportBundleStream(newRepo, uri, @in).Fetch(NullProgressMonitor.INSTANCE
				, refs);
		}

		/// <exception cref="System.IO.FileNotFoundException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private byte[] MakeBundle(string name, string anObjectToInclude, RevCommit assume
			)
		{
			BundleWriter bw;
			bw = new BundleWriter(db);
			bw.Include(name, ObjectId.FromString(anObjectToInclude));
			if (assume != null)
			{
				bw.Assume(assume);
			}
			ByteArrayOutputStream @out = new ByteArrayOutputStream();
			bw.WriteBundle(NullProgressMonitor.INSTANCE, @out);
			return @out.ToByteArray();
		}
	}
}
