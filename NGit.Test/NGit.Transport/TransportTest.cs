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
using NGit.Storage.File;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	public class TransportTest : SampleDataRepositoryTestCase
	{
		private NGit.Transport.Transport transport;

		private RemoteConfig remoteConfig;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			Config config = ((FileBasedConfig)db.GetConfig());
			remoteConfig = new RemoteConfig(config, "test");
			remoteConfig.AddURI(new URIish("http://everyones.loves.git/u/2"));
			transport = null;
		}

		/// <exception cref="System.Exception"></exception>
		protected override void TearDown()
		{
			if (transport != null)
			{
				transport.Close();
				transport = null;
			}
			base.TearDown();
		}

		/// <summary>
		/// Test RefSpec to RemoteRefUpdate conversion with simple RefSpec - no
		/// wildcard, no tracking ref in repo configuration.
		/// </summary>
		/// <remarks>
		/// Test RefSpec to RemoteRefUpdate conversion with simple RefSpec - no
		/// wildcard, no tracking ref in repo configuration.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestFindRemoteRefUpdatesNoWildcardNoTracking()
		{
			transport = NGit.Transport.Transport.Open(db, remoteConfig);
			ICollection<RemoteRefUpdate> result = transport.FindRemoteRefUpdatesFor(Collections
				.NCopies(1, new RefSpec("refs/heads/master:refs/heads/x")));
			NUnit.Framework.Assert.AreEqual(1, result.Count);
			RemoteRefUpdate rru = result.Iterator().Next();
			NUnit.Framework.Assert.IsNull(rru.GetExpectedOldObjectId());
			NUnit.Framework.Assert.IsFalse(rru.IsForceUpdate());
			NUnit.Framework.Assert.AreEqual("refs/heads/master", rru.GetSrcRef());
			AssertEquals(db.Resolve("refs/heads/master"), rru.GetNewObjectId());
			NUnit.Framework.Assert.AreEqual("refs/heads/x", rru.GetRemoteName());
		}

		/// <summary>
		/// Test RefSpec to RemoteRefUpdate conversion with no-destination RefSpec
		/// (destination should be set up for the same name as source).
		/// </summary>
		/// <remarks>
		/// Test RefSpec to RemoteRefUpdate conversion with no-destination RefSpec
		/// (destination should be set up for the same name as source).
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestFindRemoteRefUpdatesNoWildcardNoDestination()
		{
			transport = NGit.Transport.Transport.Open(db, remoteConfig);
			ICollection<RemoteRefUpdate> result = transport.FindRemoteRefUpdatesFor(Sharpen.Collections
				.NCopies(1, new RefSpec("+refs/heads/master")));
			NUnit.Framework.Assert.AreEqual(1, result.Count);
			RemoteRefUpdate rru = result.Iterator().Next();
			NUnit.Framework.Assert.IsNull(rru.GetExpectedOldObjectId());
			NUnit.Framework.Assert.IsTrue(rru.IsForceUpdate());
			NUnit.Framework.Assert.AreEqual("refs/heads/master", rru.GetSrcRef());
			AssertEquals(db.Resolve("refs/heads/master"), rru.GetNewObjectId());
			NUnit.Framework.Assert.AreEqual("refs/heads/master", rru.GetRemoteName());
		}

		/// <summary>Test RefSpec to RemoteRefUpdate conversion with wildcard RefSpec.</summary>
		/// <remarks>Test RefSpec to RemoteRefUpdate conversion with wildcard RefSpec.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestFindRemoteRefUpdatesWildcardNoTracking()
		{
			transport = NGit.Transport.Transport.Open(db, remoteConfig);
			ICollection<RemoteRefUpdate> result = transport.FindRemoteRefUpdatesFor(Sharpen.Collections
				.NCopies(1, new RefSpec("+refs/heads/*:refs/heads/test/*")));
			NUnit.Framework.Assert.AreEqual(12, result.Count);
			bool foundA = false;
			bool foundB = false;
			foreach (RemoteRefUpdate rru in result)
			{
				if ("refs/heads/a".Equals(rru.GetSrcRef()) && "refs/heads/test/a".Equals(rru.GetRemoteName
					()))
				{
					foundA = true;
				}
				if ("refs/heads/b".Equals(rru.GetSrcRef()) && "refs/heads/test/b".Equals(rru.GetRemoteName
					()))
				{
					foundB = true;
				}
			}
			NUnit.Framework.Assert.IsTrue(foundA);
			NUnit.Framework.Assert.IsTrue(foundB);
		}

		/// <summary>
		/// Test RefSpec to RemoteRefUpdate conversion for more than one RefSpecs
		/// handling.
		/// </summary>
		/// <remarks>
		/// Test RefSpec to RemoteRefUpdate conversion for more than one RefSpecs
		/// handling.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestFindRemoteRefUpdatesTwoRefSpecs()
		{
			transport = NGit.Transport.Transport.Open(db, remoteConfig);
			RefSpec specA = new RefSpec("+refs/heads/a:refs/heads/b");
			RefSpec specC = new RefSpec("+refs/heads/c:refs/heads/d");
			ICollection<RefSpec> specs = Arrays.AsList(specA, specC);
			ICollection<RemoteRefUpdate> result = transport.FindRemoteRefUpdatesFor(specs);
			NUnit.Framework.Assert.AreEqual(2, result.Count);
			bool foundA = false;
			bool foundC = false;
			foreach (RemoteRefUpdate rru in result)
			{
				if ("refs/heads/a".Equals(rru.GetSrcRef()) && "refs/heads/b".Equals(rru.GetRemoteName
					()))
				{
					foundA = true;
				}
				if ("refs/heads/c".Equals(rru.GetSrcRef()) && "refs/heads/d".Equals(rru.GetRemoteName
					()))
				{
					foundC = true;
				}
			}
			NUnit.Framework.Assert.IsTrue(foundA);
			NUnit.Framework.Assert.IsTrue(foundC);
		}

		/// <summary>Test RefSpec to RemoteRefUpdate conversion for tracking ref search.</summary>
		/// <remarks>Test RefSpec to RemoteRefUpdate conversion for tracking ref search.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestFindRemoteRefUpdatesTrackingRef()
		{
			remoteConfig.AddFetchRefSpec(new RefSpec("refs/heads/*:refs/remotes/test/*"));
			transport = NGit.Transport.Transport.Open(db, remoteConfig);
			ICollection<RemoteRefUpdate> result = transport.FindRemoteRefUpdatesFor(Sharpen.Collections
				.NCopies(1, new RefSpec("+refs/heads/a:refs/heads/a")));
			NUnit.Framework.Assert.AreEqual(1, result.Count);
			TrackingRefUpdate tru = result.Iterator().Next().GetTrackingRefUpdate();
			NUnit.Framework.Assert.AreEqual("refs/remotes/test/a", tru.GetLocalName());
			NUnit.Framework.Assert.AreEqual("refs/heads/a", tru.GetRemoteName());
			AssertEquals(db.Resolve("refs/heads/a"), tru.GetNewObjectId());
			NUnit.Framework.Assert.IsNull(tru.GetOldObjectId());
		}
	}
}
