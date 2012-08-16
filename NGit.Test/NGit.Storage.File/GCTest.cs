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
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	[NUnit.Framework.TestFixture]
	public class GCTest : LocalDiskRepositoryTestCase
	{
		private TestRepository<FileRepository> tr;

		private FileRepository repo;

		private GC gc;

		private GC.RepoStatistics stats;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			repo = CreateWorkRepository();
			tr = new TestRepository<FileRepository>((repo));
			gc = new GC(repo);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.TearDown]
		public override void TearDown()
		{
			base.TearDown();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPackAllObjectsInOnePack()
		{
			tr.Branch("refs/heads/master").Commit().Add("A", "A").Add("B", "B").Create();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(4, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackedObjects);
			gc.Gc();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(4, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(1, stats.numberOfPackFiles);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestKeepFiles()
		{
			BranchBuilder bb = tr.Branch("refs/heads/master");
			bb.Commit().Add("A", "A").Add("B", "B").Create();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(4, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackFiles);
			gc.Gc();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(4, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(1, stats.numberOfPackFiles);
			Iterator<PackFile> packIt = ((ObjectDirectory)repo.ObjectDatabase).GetPacks().Iterator
				();
			PackFile singlePack = packIt.Next();
			NUnit.Framework.Assert.IsFalse(packIt.HasNext());
			FilePath keepFile = new FilePath(singlePack.GetPackFile().GetPath() + ".keep");
			NUnit.Framework.Assert.IsFalse(keepFile.Exists());
			NUnit.Framework.Assert.IsTrue(keepFile.CreateNewFile());
			bb.Commit().Add("A", "A2").Add("B", "B2").Create();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(4, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(4, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(1, stats.numberOfPackFiles);
			gc.Gc();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(2, stats.numberOfPackFiles);
			// check that no object is packed twice
			Iterator<PackFile> packs = ((ObjectDirectory)repo.ObjectDatabase).GetPacks().Iterator
				();
			PackIndex ind1 = packs.Next().GetIndex();
			NUnit.Framework.Assert.AreEqual(4, ind1.GetObjectCount());
			PackIndex ind2 = packs.Next().GetIndex();
			NUnit.Framework.Assert.AreEqual(4, ind2.GetObjectCount());
			foreach (PackIndex.MutableEntry e in ind1)
			{
				if (ind2.HasObject(e.ToObjectId()))
				{
					NUnit.Framework.Assert.IsFalse(ind2.HasObject(e.ToObjectId()), "the following object is in both packfiles: "
						 + e.ToObjectId());
				}
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPackRepoWithNoRefs()
		{
			tr.Commit().Add("A", "A").Add("B", "B").Create();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(4, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackedObjects);
			gc.Gc();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(4, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackFiles);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPack2Commits()
		{
			BranchBuilder bb = tr.Branch("refs/heads/master");
			bb.Commit().Add("A", "A").Add("B", "B").Create();
			bb.Commit().Add("A", "A2").Add("B", "B2").Create();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackedObjects);
			gc.Gc();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(1, stats.numberOfPackFiles);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPackCommitsAndLooseOne()
		{
			BranchBuilder bb = tr.Branch("refs/heads/master");
			RevCommit first = bb.Commit().Add("A", "A").Add("B", "B").Create();
			bb.Commit().Add("A", "A2").Add("B", "B2").Create();
			tr.Update("refs/heads/master", first);
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackedObjects);
			gc.Gc();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(2, stats.numberOfPackFiles);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNotPackTwice()
		{
			BranchBuilder bb = tr.Branch("refs/heads/master");
			RevCommit first = bb.Commit().Message("M").Add("M", "M").Create();
			bb.Commit().Message("B").Add("B", "Q").Create();
			bb.Commit().Message("A").Add("A", "A").Create();
			RevCommit second = tr.Commit().Parent(first).Message("R").Add("R", "Q").Create();
			tr.Update("refs/tags/t1", second);
			ICollection<PackFile> oldPacks = ((ObjectDirectory)tr.GetRepository().ObjectDatabase
				).GetPacks();
			NUnit.Framework.Assert.AreEqual(0, oldPacks.Count);
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(11, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackedObjects);
			gc.SetExpireAgeMillis(0);
			gc.Gc();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfLooseObjects);
			Iterator<PackFile> pIt = ((ObjectDirectory)repo.ObjectDatabase).GetPacks().Iterator
				();
			long c = pIt.Next().GetObjectCount();
			if (c == 9)
			{
				NUnit.Framework.Assert.AreEqual(2, pIt.Next().GetObjectCount());
			}
			else
			{
				NUnit.Framework.Assert.AreEqual(2, c);
				NUnit.Framework.Assert.AreEqual(9, pIt.Next().GetObjectCount());
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPackCommitsAndLooseOneNoReflog()
		{
			BranchBuilder bb = tr.Branch("refs/heads/master");
			RevCommit first = bb.Commit().Add("A", "A").Add("B", "B").Create();
			bb.Commit().Add("A", "A2").Add("B", "B2").Create();
			tr.Update("refs/heads/master", first);
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackedObjects);
			FileUtils.Delete(new FilePath(repo.Directory, "logs/HEAD"), FileUtils.RETRY | FileUtils
				.SKIP_MISSING);
			FileUtils.Delete(new FilePath(repo.Directory, "logs/refs/heads/master"), FileUtils
				.RETRY | FileUtils.SKIP_MISSING);
			gc.Gc();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(4, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(4, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(1, stats.numberOfPackFiles);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPackCommitsAndLooseOneWithPruneNow()
		{
			BranchBuilder bb = tr.Branch("refs/heads/master");
			RevCommit first = bb.Commit().Add("A", "A").Add("B", "B").Create();
			bb.Commit().Add("A", "A2").Add("B", "B2").Create();
			tr.Update("refs/heads/master", first);
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackedObjects);
			gc.SetExpireAgeMillis(0);
			gc.Gc();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(2, stats.numberOfPackFiles);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPackCommitsAndLooseOneWithPruneNowNoReflog()
		{
			BranchBuilder bb = tr.Branch("refs/heads/master");
			RevCommit first = bb.Commit().Add("A", "A").Add("B", "B").Create();
			bb.Commit().Add("A", "A2").Add("B", "B2").Create();
			tr.Update("refs/heads/master", first);
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackedObjects);
			FileUtils.Delete(new FilePath(repo.Directory, "logs/HEAD"), FileUtils.RETRY | FileUtils
				.SKIP_MISSING);
			FileUtils.Delete(new FilePath(repo.Directory, "logs/refs/heads/master"), FileUtils
				.RETRY | FileUtils.SKIP_MISSING);
			gc.SetExpireAgeMillis(0);
			gc.Gc();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(4, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(1, stats.numberOfPackFiles);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIndexSavesObjects()
		{
			BranchBuilder bb = tr.Branch("refs/heads/master");
			bb.Commit().Add("A", "A").Add("B", "B").Create();
			bb.Commit().Add("A", "A2").Add("B", "B2").Create();
			bb.Commit().Add("A", "A3");
			// this new content in index should survive
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(9, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackedObjects);
			gc.Gc();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(1, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(1, stats.numberOfPackFiles);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIndexSavesObjectsWithPruneNow()
		{
			BranchBuilder bb = tr.Branch("refs/heads/master");
			bb.Commit().Add("A", "A").Add("B", "B").Create();
			bb.Commit().Add("A", "A2").Add("B", "B2").Create();
			bb.Commit().Add("A", "A3");
			// this new content in index should survive
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(9, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfPackedObjects);
			gc.SetExpireAgeMillis(0);
			gc.Gc();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(0, stats.numberOfLooseObjects);
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfPackedObjects);
			NUnit.Framework.Assert.AreEqual(1, stats.numberOfPackFiles);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPruneNone()
		{
			BranchBuilder bb = tr.Branch("refs/heads/master");
			bb.Commit().Add("A", "A").Add("B", "B").Create();
			bb.Commit().Add("A", "A2").Add("B", "B2").Create();
			new FilePath(repo.Directory, Constants.LOGS + "/refs/heads/master").Delete();
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfLooseObjects);
			gc.SetExpireAgeMillis(0);
			gc.Prune(Sharpen.Collections.EmptySet<ObjectId>());
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfLooseObjects);
			tr.Blob("x");
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(9, stats.numberOfLooseObjects);
			gc.Prune(Sharpen.Collections.EmptySet<ObjectId>());
			stats = gc.GetStatistics();
			NUnit.Framework.Assert.AreEqual(8, stats.numberOfLooseObjects);
		}
	}
}
