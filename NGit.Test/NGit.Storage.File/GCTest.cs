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
using System.Collections.Generic;
using System.IO;
using NGit;
using NGit.Internal;
using NGit.Junit;
using NGit.Merge;
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

		// GC.packRefs tests
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void PackRefs_looseRefPacked()
		{
			RevBlob a = tr.Blob("a");
			tr.LightweightTag("t", a);
			gc.PackRefs();
			NUnit.Framework.Assert.AreEqual(repo.GetRef("t").GetStorage(), RefStorage.PACKED);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ConcurrentPackRefs_onlyOneWritesPackedRefs()
		{
			RevBlob a = tr.Blob("a");
			tr.LightweightTag("t", a);
			CyclicBarrier syncPoint = new CyclicBarrier(2);
			Callable<int> packRefs = new _Callable_131(this, syncPoint);
			ExecutorService pool = Executors.NewFixedThreadPool(2);
			try
			{
				Future<int> p1 = pool.Submit(packRefs);
				Future<int> p2 = pool.Submit(packRefs);
				NUnit.Framework.Assert.AreEqual(1, p1.Get() + p2.Get());
			}
			finally
			{
				pool.Shutdown();
				pool.AwaitTermination(long.MaxValue, TimeUnit.SECONDS);
			}
		}

		private sealed class _Callable_131 : Callable<int>
		{
			public _Callable_131(GCTest _enclosing, CyclicBarrier syncPoint)
			{
				this._enclosing = _enclosing;
				this.syncPoint = syncPoint;
			}

			/// <returns>0 for success, 1 in case of error when writing pack</returns>
			/// <exception cref="System.Exception"></exception>
			public int Call()
			{
				syncPoint.Await();
				try
				{
					this._enclosing.gc.PackRefs();
					return Sharpen.Extensions.ValueOf(0);
				}
				catch (IOException)
				{
					return Sharpen.Extensions.ValueOf(1);
				}
			}

			private readonly GCTest _enclosing;

			private readonly CyclicBarrier syncPoint;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void PackRefsWhileRefLocked_refNotPackedNoError()
		{
			RevBlob a = tr.Blob("a");
			tr.LightweightTag("t1", a);
			tr.LightweightTag("t2", a);
			LockFile refLock = new LockFile(new FilePath(repo.Directory, "refs/tags/t1"), repo
				.FileSystem);
			try
			{
				refLock.Lock();
				gc.PackRefs();
			}
			finally
			{
				refLock.Unlock();
			}
			NUnit.Framework.Assert.AreEqual(repo.GetRef("refs/tags/t1").GetStorage(), RefStorage
				.LOOSE);
			NUnit.Framework.Assert.AreEqual(repo.GetRef("refs/tags/t2").GetStorage(), RefStorage
				.PACKED);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void PackRefsWhileRefUpdated_refUpdateSucceeds()
		{
			RevBlob a = tr.Blob("a");
			tr.LightweightTag("t", a);
			RevBlob b = tr.Blob("b");
			CyclicBarrier refUpdateLockedRef = new CyclicBarrier(2);
			CyclicBarrier packRefsDone = new CyclicBarrier(2);
			ExecutorService pool = Executors.NewFixedThreadPool(2);
			try
			{
				Future<RefUpdate.Result> result = pool.Submit(new _Callable_185(this, b, refUpdateLockedRef
					, packRefsDone));
				pool.Submit<Void>(new _Callable_210(this, refUpdateLockedRef, packRefsDone));
				NUnit.Framework.Assert.AreEqual(result.Get(), RefUpdate.Result.FORCED);
			}
			finally
			{
				pool.ShutdownNow();
				pool.AwaitTermination(long.MaxValue, TimeUnit.SECONDS);
			}
			NUnit.Framework.Assert.AreEqual(repo.GetRef("refs/tags/t").GetObjectId(), b);
		}

		private sealed class _Callable_185 : Callable<RefUpdate.Result>
		{
			public _Callable_185(GCTest _enclosing, RevBlob b, CyclicBarrier refUpdateLockedRef
				, CyclicBarrier packRefsDone)
			{
				this._enclosing = _enclosing;
				this.b = b;
				this.refUpdateLockedRef = refUpdateLockedRef;
				this.packRefsDone = packRefsDone;
			}

			/// <exception cref="System.Exception"></exception>
			public RefUpdate.Result Call()
			{
				RefUpdate update = new _RefDirectoryUpdate_190(refUpdateLockedRef, packRefsDone, 
					(RefDirectory)this._enclosing.repo.RefDatabase, this._enclosing.repo.GetRef("refs/tags/t"
					));
				update.SetForceUpdate(true);
				update.SetNewObjectId(b);
				return update.Update();
			}

			private sealed class _RefDirectoryUpdate_190 : RefDirectoryUpdate
			{
				public _RefDirectoryUpdate_190(CyclicBarrier refUpdateLockedRef, CyclicBarrier packRefsDone
					, RefDirectory baseArg1, Ref baseArg2) : base(baseArg1, baseArg2)
				{
					this.refUpdateLockedRef = refUpdateLockedRef;
					this.packRefsDone = packRefsDone;
				}

				public override bool IsForceUpdate()
				{
					try
					{
						refUpdateLockedRef.Await();
						packRefsDone.Await();
					}
					catch (Exception)
					{
						Sharpen.Thread.CurrentThread().Interrupt();
					}
					return base.IsForceUpdate();
				}

				private readonly CyclicBarrier refUpdateLockedRef;

				private readonly CyclicBarrier packRefsDone;
			}

			private readonly GCTest _enclosing;

			private readonly RevBlob b;

			private readonly CyclicBarrier refUpdateLockedRef;

			private readonly CyclicBarrier packRefsDone;
		}

		private sealed class _Callable_210 : Callable<Void>
		{
			public _Callable_210(GCTest _enclosing, CyclicBarrier refUpdateLockedRef, CyclicBarrier
				 packRefsDone)
			{
				this._enclosing = _enclosing;
				this.refUpdateLockedRef = refUpdateLockedRef;
				this.packRefsDone = packRefsDone;
			}

			/// <exception cref="System.Exception"></exception>
			public Void Call()
			{
				refUpdateLockedRef.Await();
				this._enclosing.gc.PackRefs();
				packRefsDone.Await();
				return;
			}

			private readonly GCTest _enclosing;

			private readonly CyclicBarrier refUpdateLockedRef;

			private readonly CyclicBarrier packRefsDone;
		}

		// GC.repack tests
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void RepackEmptyRepo_noPackCreated()
		{
			gc.Repack();
			NUnit.Framework.Assert.AreEqual(0, ((ObjectDirectory)repo.ObjectDatabase).GetPacks
				().Count);
		}

		CyclicBarrier syncPoint = new CyclicBarrier(2);
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ConcurrentRepack()
		{
			//
			// leave the syncPoint in broken state so any awaiting
			// threads and any threads that call await in the future get
			// the BrokenBarrierException
			//
			RevBlob a = tr.Blob("a");
			tr.LightweightTag("t", a);
			ExecutorService pool = Executors.NewFixedThreadPool(2);
			try
			{
				_T187790690 repack1 = new _T187790690(this);
				_T187790690 repack2 = new _T187790690(this);
				Future<int> result1 = pool.Submit(repack1);
				Future<int> result2 = pool.Submit(repack2);
				NUnit.Framework.Assert.AreEqual(0, result1.Get() + result2.Get());
			}
			finally
			{
				pool.Shutdown();
				pool.AwaitTermination(long.MaxValue, TimeUnit.SECONDS);
			}
		}

		internal class _T187790690 : EmptyProgressMonitor, Callable<int>
		{
			public override void BeginTask(string title, int totalWork)
			{
				if (title.Equals(JGitText.Get().writingObjects))
				{
					try
					{
						_enclosing.syncPoint.Await();
					}
					catch (Exception)
					{
						Sharpen.Thread.CurrentThread().Interrupt();
					}
				}
			}

			/// <returns>0 for success, 1 in case of error when writing pack</returns>
			/// <exception cref="System.Exception"></exception>
			public virtual int Call()
			{
				try
				{
					this._enclosing.gc.SetProgressMonitor(this);
					this._enclosing.gc.Repack();
					return Sharpen.Extensions.ValueOf(0);
				}
				catch (IOException)
				{
					Sharpen.Thread.CurrentThread().Interrupt();
					try
					{
						_enclosing.syncPoint.Await();
					}
					catch (Exception)
					{
					}
					return Sharpen.Extensions.ValueOf(1);
				}
			}

			internal _T187790690(GCTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly GCTest _enclosing;
		}

		// GC.prune tests
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void NonReferencedNonExpiredObject_notPruned()
		{
			RevBlob a = tr.Blob("a");
			gc.SetExpire(Sharpen.Extensions.CreateDate(LastModified(a)));
			gc.Prune(Collections.EmptySet<ObjectId>());
			NUnit.Framework.Assert.IsTrue(repo.HasObject(a));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void NonReferencedExpiredObject_pruned()
		{
			RevBlob a = tr.Blob("a");
			gc.SetExpireAgeMillis(0);
			gc.Prune(Collections.EmptySet<ObjectId>());
			NUnit.Framework.Assert.IsFalse(repo.HasObject(a));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void NonReferencedExpiredObjectTree_pruned()
		{
			RevBlob a = tr.Blob("a");
			RevTree t = tr.Tree(tr.File("a", a));
			gc.SetExpireAgeMillis(0);
			gc.Prune(Collections.EmptySet<ObjectId>());
			NUnit.Framework.Assert.IsFalse(repo.HasObject(t));
			NUnit.Framework.Assert.IsFalse(repo.HasObject(a));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void NonReferencedObjects_onlyExpiredPruned()
		{
			RevBlob a = tr.Blob("a");
			gc.SetExpire(Sharpen.Extensions.CreateDate(LastModified(a) + 1));
			FsTick();
			RevBlob b = tr.Blob("b");
			gc.Prune(Collections.EmptySet<ObjectId>());
			NUnit.Framework.Assert.IsFalse(repo.HasObject(a));
			NUnit.Framework.Assert.IsTrue(repo.HasObject(b));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void LightweightTag_objectNotPruned()
		{
			RevBlob a = tr.Blob("a");
			tr.LightweightTag("t", a);
			gc.SetExpireAgeMillis(0);
			gc.Prune(Collections.EmptySet<ObjectId>());
			NUnit.Framework.Assert.IsTrue(repo.HasObject(a));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void AnnotatedTag_objectNotPruned()
		{
			RevBlob a = tr.Blob("a");
			RevTag t = tr.Tag("t", a);
			// this doesn't create the refs/tags/t ref
			tr.LightweightTag("t", t);
			gc.SetExpireAgeMillis(0);
			gc.Prune(Collections.EmptySet<ObjectId>());
			NUnit.Framework.Assert.IsTrue(repo.HasObject(t));
			NUnit.Framework.Assert.IsTrue(repo.HasObject(a));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void Branch_historyNotPruned()
		{
			RevCommit tip = CommitChain(10);
			tr.Branch("b").Update(tip);
			gc.SetExpireAgeMillis(0);
			gc.Prune(Collections.EmptySet<ObjectId>());
			do
			{
				NUnit.Framework.Assert.IsTrue(repo.HasObject(tip));
				tr.ParseBody(tip);
				RevTree t = tip.Tree;
				NUnit.Framework.Assert.IsTrue(repo.HasObject(t));
				NUnit.Framework.Assert.IsTrue(repo.HasObject(tr.Get(t, "a")));
				tip = tip.ParentCount > 0 ? tip.GetParent(0) : null;
			}
			while (tip != null);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void DeleteBranch_historyPruned()
		{
			RevCommit tip = CommitChain(10);
			tr.Branch("b").Update(tip);
			RefUpdate update = repo.UpdateRef("refs/heads/b");
			update.SetForceUpdate(true);
			update.Delete();
			gc.SetExpireAgeMillis(0);
			gc.Prune(Collections.EmptySet<ObjectId>());
			NUnit.Framework.Assert.IsTrue(gc.GetStatistics().numberOfLooseObjects == 0);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void DeleteMergedBranch_historyNotPruned()
		{
			RevCommit parent = tr.Commit().Create();
			RevCommit b1Tip = tr.Branch("b1").Commit().Parent(parent).Add("x", "x").Create();
			RevCommit b2Tip = tr.Branch("b2").Commit().Parent(parent).Add("y", "y").Create();
			// merge b1Tip and b2Tip and update refs/heads/b1 to the merge commit
			Merger merger = ((ThreeWayMerger)MergeStrategy.SIMPLE_TWO_WAY_IN_CORE.NewMerger(repo
				));
			merger.Merge(b1Tip, b2Tip);
			NGit.Junit.CommitBuilder cb = tr.Commit();
			cb.Parent(b1Tip).Parent(b2Tip);
			cb.SetTopLevelTree(merger.GetResultTreeId());
			RevCommit mergeCommit = cb.Create();
			RefUpdate u = repo.UpdateRef("refs/heads/b1");
			u.SetNewObjectId(mergeCommit);
			u.Update();
			RefUpdate update = repo.UpdateRef("refs/heads/b2");
			update.SetForceUpdate(true);
			update.Delete();
			gc.SetExpireAgeMillis(0);
			gc.Prune(Collections.EmptySet<ObjectId>());
			NUnit.Framework.Assert.IsTrue(repo.HasObject(b2Tip));
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

		/// <summary>Create a chain of commits of given depth.</summary>
		/// <remarks>
		/// Create a chain of commits of given depth.
		/// <p>
		/// Each commit contains one file named "a" containing the index of the
		/// commit in the chain as its content. The created commit chain is
		/// referenced from any ref.
		/// <p>
		/// A chain of depth = N will create 3*N objects in Gits object database. For
		/// each depth level three objects are created: the commit object, the
		/// top-level tree object and a blob for the content of the file "a".
		/// </remarks>
		/// <param name="depth">the depth of the commit chain.</param>
		/// <returns>the commit that is the tip of the commit chain</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		private RevCommit CommitChain(int depth)
		{
			if (depth <= 0)
			{
				throw new ArgumentException("Chain depth must be > 0");
			}
			NGit.Junit.CommitBuilder cb = tr.Commit();
			RevCommit tip;
			do
			{
				--depth;
				tip = cb.Add("a", string.Empty + depth).Message(string.Empty + depth).Create();
				cb = cb.Child();
			}
			while (depth > 0);
			return tip;
		}

		private long LastModified(AnyObjectId objectId)
		{
			return ((ObjectDirectory)repo.ObjectDatabase).FileFor(objectId).LastModified();
		}

		/// <exception cref="System.Exception"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private static void FsTick()
		{
			RepositoryTestCase.FsTick(null);
		}
	}
}
