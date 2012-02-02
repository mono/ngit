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
using System.IO;
using NGit;
using NGit.Events;
using NGit.Junit;
using NGit.Revwalk;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	[NUnit.Framework.TestFixture]
	public class RefDirectoryTest : LocalDiskRepositoryTestCase
	{
		private Repository diskRepo;

		private TestRepository repo;

		private RefDirectory refdir;

		private RevCommit A;

		private RevCommit B;

		private RevTag v1_0;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			diskRepo = CreateBareRepository();
			refdir = (RefDirectory)diskRepo.RefDatabase;
			repo = new TestRepository<Repository>(diskRepo);
			A = repo.Commit().Create();
			B = repo.Commit(repo.GetRevWalk().ParseCommit(A));
			v1_0 = repo.Tag("v1_0", B);
			repo.GetRevWalk().ParseBody(v1_0);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCreate()
		{
			// setUp above created the directory. We just have to test it.
			FilePath d = diskRepo.Directory;
			NUnit.Framework.Assert.AreSame(diskRepo, refdir.GetRepository());
			NUnit.Framework.Assert.IsTrue(new FilePath(d, "refs").IsDirectory());
			NUnit.Framework.Assert.IsTrue(new FilePath(d, "logs").IsDirectory());
			NUnit.Framework.Assert.IsTrue(new FilePath(d, "logs/refs").IsDirectory());
			NUnit.Framework.Assert.IsFalse(new FilePath(d, "packed-refs").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(d, "refs/heads").IsDirectory());
			NUnit.Framework.Assert.IsTrue(new FilePath(d, "refs/tags").IsDirectory());
			NUnit.Framework.Assert.AreEqual(2, new FilePath(d, "refs").List().Length);
			NUnit.Framework.Assert.AreEqual(0, new FilePath(d, "refs/heads").List().Length);
			NUnit.Framework.Assert.AreEqual(0, new FilePath(d, "refs/tags").List().Length);
			NUnit.Framework.Assert.IsTrue(new FilePath(d, "logs/refs/heads").IsDirectory());
			NUnit.Framework.Assert.IsFalse(new FilePath(d, "logs/HEAD").Exists());
			NUnit.Framework.Assert.AreEqual(0, new FilePath(d, "logs/refs/heads").List().Length
				);
			NUnit.Framework.Assert.AreEqual("ref: refs/heads/master\n", Read(new FilePath(d, 
				Constants.HEAD)));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_EmptyDatabase()
		{
			IDictionary<string, Ref> all;
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.IsTrue(all.IsEmpty(), "no references");
			all = refdir.GetRefs(Constants.R_HEADS);
			NUnit.Framework.Assert.IsTrue(all.IsEmpty(), "no references");
			all = refdir.GetRefs(Constants.R_TAGS);
			NUnit.Framework.Assert.IsTrue(all.IsEmpty(), "no references");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_HeadOnOneBranch()
		{
			IDictionary<string, Ref> all;
			Ref head;
			Ref master;
			WriteLooseRef("refs/heads/master", A);
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(2, all.Count);
			NUnit.Framework.Assert.IsTrue(all.ContainsKey(Constants.HEAD), "has HEAD");
			NUnit.Framework.Assert.IsTrue(all.ContainsKey("refs/heads/master"), "has master");
			head = all.Get(Constants.HEAD);
			master = all.Get("refs/heads/master");
			NUnit.Framework.Assert.AreEqual(Constants.HEAD, head.GetName());
			NUnit.Framework.Assert.IsTrue(head.IsSymbolic());
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, head.GetStorage());
			NUnit.Framework.Assert.AreSame(master, head.GetTarget(), "uses same ref as target"
				);
			NUnit.Framework.Assert.AreEqual("refs/heads/master", master.GetName());
			NUnit.Framework.Assert.IsFalse(master.IsSymbolic());
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, master.GetStorage());
			NUnit.Framework.Assert.AreEqual(A, master.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_DeatchedHead1()
		{
			IDictionary<string, Ref> all;
			Ref head;
			WriteLooseRef(Constants.HEAD, A);
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(1, all.Count);
			NUnit.Framework.Assert.IsTrue(all.ContainsKey(Constants.HEAD), "has HEAD");
			head = all.Get(Constants.HEAD);
			NUnit.Framework.Assert.AreEqual(Constants.HEAD, head.GetName());
			NUnit.Framework.Assert.IsFalse(head.IsSymbolic());
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, head.GetStorage());
			NUnit.Framework.Assert.AreEqual(A, head.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_DeatchedHead2()
		{
			IDictionary<string, Ref> all;
			Ref head;
			Ref master;
			WriteLooseRef(Constants.HEAD, A);
			WriteLooseRef("refs/heads/master", B);
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(2, all.Count);
			head = all.Get(Constants.HEAD);
			master = all.Get("refs/heads/master");
			NUnit.Framework.Assert.AreEqual(Constants.HEAD, head.GetName());
			NUnit.Framework.Assert.IsFalse(head.IsSymbolic());
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, head.GetStorage());
			NUnit.Framework.Assert.AreEqual(A, head.GetObjectId());
			NUnit.Framework.Assert.AreEqual("refs/heads/master", master.GetName());
			NUnit.Framework.Assert.IsFalse(master.IsSymbolic());
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, master.GetStorage());
			NUnit.Framework.Assert.AreEqual(B, master.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_DeeplyNestedBranch()
		{
			string name = "refs/heads/a/b/c/d/e/f/g/h/i/j/k";
			IDictionary<string, Ref> all;
			Ref r;
			WriteLooseRef(name, A);
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(1, all.Count);
			r = all.Get(name);
			NUnit.Framework.Assert.AreEqual(name, r.GetName());
			NUnit.Framework.Assert.IsFalse(r.IsSymbolic());
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, r.GetStorage());
			NUnit.Framework.Assert.AreEqual(A, r.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_HeadBranchNotBorn()
		{
			IDictionary<string, Ref> all;
			Ref a;
			Ref b;
			WriteLooseRef("refs/heads/A", A);
			WriteLooseRef("refs/heads/B", B);
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(2, all.Count);
			NUnit.Framework.Assert.IsFalse(all.ContainsKey(Constants.HEAD), "no HEAD");
			a = all.Get("refs/heads/A");
			b = all.Get("refs/heads/B");
			NUnit.Framework.Assert.AreEqual(A, a.GetObjectId());
			NUnit.Framework.Assert.AreEqual(B, b.GetObjectId());
			NUnit.Framework.Assert.AreEqual("refs/heads/A", a.GetName());
			NUnit.Framework.Assert.AreEqual("refs/heads/B", b.GetName());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_LooseOverridesPacked()
		{
			IDictionary<string, Ref> heads;
			Ref a;
			WriteLooseRef("refs/heads/master", B);
			WritePackedRef("refs/heads/master", A);
			heads = refdir.GetRefs(Constants.R_HEADS);
			NUnit.Framework.Assert.AreEqual(1, heads.Count);
			a = heads.Get("master");
			NUnit.Framework.Assert.AreEqual("refs/heads/master", a.GetName());
			NUnit.Framework.Assert.AreEqual(B, a.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_IgnoresGarbageRef1()
		{
			IDictionary<string, Ref> heads;
			Ref a;
			WriteLooseRef("refs/heads/A", A);
			Write(new FilePath(diskRepo.Directory, "refs/heads/bad"), "FAIL\n");
			heads = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(1, heads.Count);
			a = heads.Get("refs/heads/A");
			NUnit.Framework.Assert.AreEqual("refs/heads/A", a.GetName());
			NUnit.Framework.Assert.AreEqual(A, a.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_IgnoresGarbageRef2()
		{
			IDictionary<string, Ref> heads;
			Ref a;
			WriteLooseRef("refs/heads/A", A);
			Write(new FilePath(diskRepo.Directory, "refs/heads/bad"), string.Empty);
			heads = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(1, heads.Count);
			a = heads.Get("refs/heads/A");
			NUnit.Framework.Assert.AreEqual("refs/heads/A", a.GetName());
			NUnit.Framework.Assert.AreEqual(A, a.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_IgnoresGarbageRef3()
		{
			IDictionary<string, Ref> heads;
			Ref a;
			WriteLooseRef("refs/heads/A", A);
			Write(new FilePath(diskRepo.Directory, "refs/heads/bad"), "\n");
			heads = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(1, heads.Count);
			a = heads.Get("refs/heads/A");
			NUnit.Framework.Assert.AreEqual("refs/heads/A", a.GetName());
			NUnit.Framework.Assert.AreEqual(A, a.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_IgnoresGarbageRef4()
		{
			IDictionary<string, Ref> heads;
			Ref a;
			Ref b;
			Ref c;
			WriteLooseRef("refs/heads/A", A);
			WriteLooseRef("refs/heads/B", B);
			WriteLooseRef("refs/heads/C", A);
			heads = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(3, heads.Count);
			NUnit.Framework.Assert.IsTrue(heads.ContainsKey("refs/heads/A"));
			NUnit.Framework.Assert.IsTrue(heads.ContainsKey("refs/heads/B"));
			NUnit.Framework.Assert.IsTrue(heads.ContainsKey("refs/heads/C"));
			WriteLooseRef("refs/heads/B", "FAIL\n");
			heads = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(2, heads.Count);
			a = heads.Get("refs/heads/A");
			b = heads.Get("refs/heads/B");
			c = heads.Get("refs/heads/C");
			NUnit.Framework.Assert.AreEqual("refs/heads/A", a.GetName());
			NUnit.Framework.Assert.AreEqual(A, a.GetObjectId());
			NUnit.Framework.Assert.IsNull(b, "no refs/heads/B");
			NUnit.Framework.Assert.AreEqual("refs/heads/C", c.GetName());
			NUnit.Framework.Assert.AreEqual(A, c.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_InvalidName()
		{
			WriteLooseRef("refs/heads/A", A);
			NUnit.Framework.Assert.IsTrue(refdir.GetRefs("refs/heads").IsEmpty(), "empty refs/heads"
				);
			NUnit.Framework.Assert.IsTrue(refdir.GetRefs("objects").IsEmpty(), "empty objects"
				);
			NUnit.Framework.Assert.IsTrue(refdir.GetRefs("objects/").IsEmpty(), "empty objects/"
				);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_HeadsOnly_AllLoose()
		{
			IDictionary<string, Ref> heads;
			Ref a;
			Ref b;
			WriteLooseRef("refs/heads/A", A);
			WriteLooseRef("refs/heads/B", B);
			WriteLooseRef("refs/tags/v1.0", v1_0);
			heads = refdir.GetRefs(Constants.R_HEADS);
			NUnit.Framework.Assert.AreEqual(2, heads.Count);
			a = heads.Get("A");
			b = heads.Get("B");
			NUnit.Framework.Assert.AreEqual("refs/heads/A", a.GetName());
			NUnit.Framework.Assert.AreEqual("refs/heads/B", b.GetName());
			NUnit.Framework.Assert.AreEqual(A, a.GetObjectId());
			NUnit.Framework.Assert.AreEqual(B, b.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_HeadsOnly_AllPacked1()
		{
			IDictionary<string, Ref> heads;
			Ref a;
			DeleteLooseRef(Constants.HEAD);
			WritePackedRef("refs/heads/A", A);
			heads = refdir.GetRefs(Constants.R_HEADS);
			NUnit.Framework.Assert.AreEqual(1, heads.Count);
			a = heads.Get("A");
			NUnit.Framework.Assert.AreEqual("refs/heads/A", a.GetName());
			NUnit.Framework.Assert.AreEqual(A, a.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_HeadsOnly_SymrefToPacked()
		{
			IDictionary<string, Ref> heads;
			Ref master;
			Ref other;
			WriteLooseRef("refs/heads/other", "ref: refs/heads/master\n");
			WritePackedRef("refs/heads/master", A);
			heads = refdir.GetRefs(Constants.R_HEADS);
			NUnit.Framework.Assert.AreEqual(2, heads.Count);
			master = heads.Get("master");
			other = heads.Get("other");
			NUnit.Framework.Assert.AreEqual("refs/heads/master", master.GetName());
			NUnit.Framework.Assert.AreEqual(A, master.GetObjectId());
			NUnit.Framework.Assert.AreEqual("refs/heads/other", other.GetName());
			NUnit.Framework.Assert.AreEqual(A, other.GetObjectId());
			NUnit.Framework.Assert.AreSame(master, other.GetTarget());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_HeadsOnly_Mixed()
		{
			IDictionary<string, Ref> heads;
			Ref a;
			Ref b;
			WriteLooseRef("refs/heads/A", A);
			WriteLooseRef("refs/heads/B", B);
			WritePackedRef("refs/tags/v1.0", v1_0);
			heads = refdir.GetRefs(Constants.R_HEADS);
			NUnit.Framework.Assert.AreEqual(2, heads.Count);
			a = heads.Get("A");
			b = heads.Get("B");
			NUnit.Framework.Assert.AreEqual("refs/heads/A", a.GetName());
			NUnit.Framework.Assert.AreEqual("refs/heads/B", b.GetName());
			NUnit.Framework.Assert.AreEqual(A, a.GetObjectId());
			NUnit.Framework.Assert.AreEqual(B, b.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_TagsOnly_AllLoose()
		{
			IDictionary<string, Ref> tags;
			Ref a;
			WriteLooseRef("refs/heads/A", A);
			WriteLooseRef("refs/tags/v1.0", v1_0);
			tags = refdir.GetRefs(Constants.R_TAGS);
			NUnit.Framework.Assert.AreEqual(1, tags.Count);
			a = tags.Get("v1.0");
			NUnit.Framework.Assert.AreEqual("refs/tags/v1.0", a.GetName());
			NUnit.Framework.Assert.AreEqual(v1_0, a.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_LooseSortedCorrectly()
		{
			IDictionary<string, Ref> refs;
			WriteLooseRef("refs/heads/project1/A", A);
			WriteLooseRef("refs/heads/project1-B", B);
			refs = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(2, refs.Count);
			NUnit.Framework.Assert.AreEqual(A, refs.Get("refs/heads/project1/A").GetObjectId(
				));
			NUnit.Framework.Assert.AreEqual(B, refs.Get("refs/heads/project1-B").GetObjectId(
				));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_LooseSorting_Bug_348834()
		{
			IDictionary<string, Ref> refs;
			WriteLooseRef("refs/heads/my/a+b", A);
			WriteLooseRef("refs/heads/my/a/b/c", B);
			int[] count = new int[1];
			ListenerHandle listener = Repository.GetGlobalListenerList().AddRefsChangedListener
				(new _RefsChangedListener_481(count));
			refs = refdir.GetRefs(RefDatabase.ALL);
			refs = refdir.GetRefs(RefDatabase.ALL);
			listener.Remove();
			NUnit.Framework.Assert.AreEqual(1, count[0]);
			// Bug 348834 multiple RefsChangedEvents
			NUnit.Framework.Assert.AreEqual(2, refs.Count);
			NUnit.Framework.Assert.AreEqual(A, refs.Get("refs/heads/my/a+b").GetObjectId());
			NUnit.Framework.Assert.AreEqual(B, refs.Get("refs/heads/my/a/b/c").GetObjectId());
		}

		private sealed class _RefsChangedListener_481 : RefsChangedListener
		{
			public _RefsChangedListener_481(int[] count)
			{
				this.count = count;
			}

			public void OnRefsChanged(RefsChangedEvent @event)
			{
				count[0]++;
			}

			private readonly int[] count;
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_TagsOnly_AllPacked()
		{
			IDictionary<string, Ref> tags;
			Ref a;
			DeleteLooseRef(Constants.HEAD);
			WritePackedRef("refs/tags/v1.0", v1_0);
			tags = refdir.GetRefs(Constants.R_TAGS);
			NUnit.Framework.Assert.AreEqual(1, tags.Count);
			a = tags.Get("v1.0");
			NUnit.Framework.Assert.AreEqual("refs/tags/v1.0", a.GetName());
			NUnit.Framework.Assert.AreEqual(v1_0, a.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_DiscoversNewLoose1()
		{
			IDictionary<string, Ref> orig;
			IDictionary<string, Ref> next;
			Ref orig_r;
			Ref next_r;
			WriteLooseRef("refs/heads/master", A);
			orig = refdir.GetRefs(RefDatabase.ALL);
			WriteLooseRef("refs/heads/next", B);
			next = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(2, orig.Count);
			NUnit.Framework.Assert.AreEqual(3, next.Count);
			NUnit.Framework.Assert.IsFalse(orig.ContainsKey("refs/heads/next"));
			NUnit.Framework.Assert.IsTrue(next.ContainsKey("refs/heads/next"));
			orig_r = orig.Get("refs/heads/master");
			next_r = next.Get("refs/heads/master");
			NUnit.Framework.Assert.AreEqual(A, orig_r.GetObjectId());
			NUnit.Framework.Assert.AreSame(orig_r, next_r, "uses cached instance");
			NUnit.Framework.Assert.AreSame(orig_r, orig.Get(Constants.HEAD).GetTarget(), "same HEAD"
				);
			NUnit.Framework.Assert.AreSame(orig_r, next.Get(Constants.HEAD).GetTarget(), "same HEAD"
				);
			next_r = next.Get("refs/heads/next");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, next_r.GetStorage());
			NUnit.Framework.Assert.AreEqual(B, next_r.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_DiscoversNewLoose2()
		{
			IDictionary<string, Ref> orig;
			IDictionary<string, Ref> next;
			IDictionary<string, Ref> news;
			WriteLooseRef("refs/heads/pu", A);
			orig = refdir.GetRefs(RefDatabase.ALL);
			WriteLooseRef("refs/heads/new/B", B);
			news = refdir.GetRefs("refs/heads/new/");
			next = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(1, orig.Count);
			NUnit.Framework.Assert.AreEqual(2, next.Count);
			NUnit.Framework.Assert.AreEqual(1, news.Count);
			NUnit.Framework.Assert.IsTrue(orig.ContainsKey("refs/heads/pu"));
			NUnit.Framework.Assert.IsTrue(next.ContainsKey("refs/heads/pu"));
			NUnit.Framework.Assert.IsFalse(news.ContainsKey("refs/heads/pu"));
			NUnit.Framework.Assert.IsFalse(orig.ContainsKey("refs/heads/new/B"));
			NUnit.Framework.Assert.IsTrue(next.ContainsKey("refs/heads/new/B"));
			NUnit.Framework.Assert.IsTrue(news.ContainsKey("B"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_DiscoversModifiedLoose()
		{
			IDictionary<string, Ref> all;
			WriteLooseRef("refs/heads/master", A);
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(A, all.Get(Constants.HEAD).GetObjectId());
			WriteLooseRef("refs/heads/master", B);
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(B, all.Get(Constants.HEAD).GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRef_DiscoversModifiedLoose()
		{
			IDictionary<string, Ref> all;
			WriteLooseRef("refs/heads/master", A);
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(A, all.Get(Constants.HEAD).GetObjectId());
			WriteLooseRef("refs/heads/master", B);
			Ref master = refdir.GetRef("refs/heads/master");
			NUnit.Framework.Assert.AreEqual(B, master.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_DiscoversDeletedLoose1()
		{
			IDictionary<string, Ref> orig;
			IDictionary<string, Ref> next;
			Ref orig_r;
			Ref next_r;
			WriteLooseRef("refs/heads/B", B);
			WriteLooseRef("refs/heads/master", A);
			orig = refdir.GetRefs(RefDatabase.ALL);
			DeleteLooseRef("refs/heads/B");
			next = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(3, orig.Count);
			NUnit.Framework.Assert.AreEqual(2, next.Count);
			NUnit.Framework.Assert.IsTrue(orig.ContainsKey("refs/heads/B"));
			NUnit.Framework.Assert.IsFalse(next.ContainsKey("refs/heads/B"));
			orig_r = orig.Get("refs/heads/master");
			next_r = next.Get("refs/heads/master");
			NUnit.Framework.Assert.AreEqual(A, orig_r.GetObjectId());
			NUnit.Framework.Assert.AreSame(orig_r, next_r, "uses cached instance");
			NUnit.Framework.Assert.AreSame(orig_r, orig.Get(Constants.HEAD).GetTarget(), "same HEAD"
				);
			NUnit.Framework.Assert.AreSame(orig_r, next.Get(Constants.HEAD).GetTarget(), "same HEAD"
				);
			orig_r = orig.Get("refs/heads/B");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, orig_r.GetStorage());
			NUnit.Framework.Assert.AreEqual(B, orig_r.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRef_DiscoversDeletedLoose()
		{
			IDictionary<string, Ref> all;
			WriteLooseRef("refs/heads/master", A);
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(A, all.Get(Constants.HEAD).GetObjectId());
			DeleteLooseRef("refs/heads/master");
			NUnit.Framework.Assert.IsNull(refdir.GetRef("refs/heads/master"));
			NUnit.Framework.Assert.IsTrue(refdir.GetRefs(RefDatabase.ALL).IsEmpty());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_DiscoversDeletedLoose2()
		{
			IDictionary<string, Ref> orig;
			IDictionary<string, Ref> next;
			WriteLooseRef("refs/heads/master", A);
			WriteLooseRef("refs/heads/pu", B);
			orig = refdir.GetRefs(RefDatabase.ALL);
			DeleteLooseRef("refs/heads/pu");
			next = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(3, orig.Count);
			NUnit.Framework.Assert.AreEqual(2, next.Count);
			NUnit.Framework.Assert.IsTrue(orig.ContainsKey("refs/heads/pu"));
			NUnit.Framework.Assert.IsFalse(next.ContainsKey("refs/heads/pu"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_DiscoversDeletedLoose3()
		{
			IDictionary<string, Ref> orig;
			IDictionary<string, Ref> next;
			WriteLooseRef("refs/heads/master", A);
			WriteLooseRef("refs/heads/next", B);
			WriteLooseRef("refs/heads/pu", B);
			WriteLooseRef("refs/tags/v1.0", v1_0);
			orig = refdir.GetRefs(RefDatabase.ALL);
			DeleteLooseRef("refs/heads/pu");
			DeleteLooseRef("refs/heads/next");
			next = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(5, orig.Count);
			NUnit.Framework.Assert.AreEqual(3, next.Count);
			NUnit.Framework.Assert.IsTrue(orig.ContainsKey("refs/heads/pu"));
			NUnit.Framework.Assert.IsTrue(orig.ContainsKey("refs/heads/next"));
			NUnit.Framework.Assert.IsFalse(next.ContainsKey("refs/heads/pu"));
			NUnit.Framework.Assert.IsFalse(next.ContainsKey("refs/heads/next"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_DiscoversDeletedLoose4()
		{
			IDictionary<string, Ref> orig;
			IDictionary<string, Ref> next;
			Ref orig_r;
			Ref next_r;
			WriteLooseRef("refs/heads/B", B);
			WriteLooseRef("refs/heads/master", A);
			orig = refdir.GetRefs(RefDatabase.ALL);
			DeleteLooseRef("refs/heads/master");
			next = refdir.GetRefs("refs/heads/");
			NUnit.Framework.Assert.AreEqual(3, orig.Count);
			NUnit.Framework.Assert.AreEqual(1, next.Count);
			NUnit.Framework.Assert.IsTrue(orig.ContainsKey("refs/heads/B"));
			NUnit.Framework.Assert.IsTrue(orig.ContainsKey("refs/heads/master"));
			NUnit.Framework.Assert.IsTrue(next.ContainsKey("B"));
			NUnit.Framework.Assert.IsFalse(next.ContainsKey("master"));
			orig_r = orig.Get("refs/heads/B");
			next_r = next.Get("B");
			NUnit.Framework.Assert.AreEqual(B, orig_r.GetObjectId());
			NUnit.Framework.Assert.AreSame(orig_r, next_r, "uses cached instance");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_DiscoversDeletedLoose5()
		{
			IDictionary<string, Ref> orig;
			IDictionary<string, Ref> next;
			WriteLooseRef("refs/heads/master", A);
			WriteLooseRef("refs/heads/pu", B);
			orig = refdir.GetRefs(RefDatabase.ALL);
			DeleteLooseRef("refs/heads/pu");
			WriteLooseRef("refs/tags/v1.0", v1_0);
			next = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(3, orig.Count);
			NUnit.Framework.Assert.AreEqual(3, next.Count);
			NUnit.Framework.Assert.IsTrue(orig.ContainsKey("refs/heads/pu"));
			NUnit.Framework.Assert.IsFalse(orig.ContainsKey("refs/tags/v1.0"));
			NUnit.Framework.Assert.IsFalse(next.ContainsKey("refs/heads/pu"));
			NUnit.Framework.Assert.IsTrue(next.ContainsKey("refs/tags/v1.0"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_SkipsLockFiles()
		{
			IDictionary<string, Ref> all;
			WriteLooseRef("refs/heads/master", A);
			WriteLooseRef("refs/heads/pu.lock", B);
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(2, all.Count);
			NUnit.Framework.Assert.IsTrue(all.ContainsKey(Constants.HEAD));
			NUnit.Framework.Assert.IsTrue(all.ContainsKey("refs/heads/master"));
			NUnit.Framework.Assert.IsFalse(all.ContainsKey("refs/heads/pu.lock"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_CycleInSymbolicRef()
		{
			IDictionary<string, Ref> all;
			Ref r;
			WriteLooseRef("refs/1", "ref: refs/2\n");
			WriteLooseRef("refs/2", "ref: refs/3\n");
			WriteLooseRef("refs/3", "ref: refs/4\n");
			WriteLooseRef("refs/4", "ref: refs/5\n");
			WriteLooseRef("refs/5", "ref: refs/end\n");
			WriteLooseRef("refs/end", A);
			all = refdir.GetRefs(RefDatabase.ALL);
			r = all.Get("refs/1");
			NUnit.Framework.Assert.IsNotNull(r, "has 1");
			NUnit.Framework.Assert.AreEqual("refs/1", r.GetName());
			NUnit.Framework.Assert.AreEqual(A, r.GetObjectId());
			NUnit.Framework.Assert.IsTrue(r.IsSymbolic());
			r = r.GetTarget();
			NUnit.Framework.Assert.AreEqual("refs/2", r.GetName());
			NUnit.Framework.Assert.AreEqual(A, r.GetObjectId());
			NUnit.Framework.Assert.IsTrue(r.IsSymbolic());
			r = r.GetTarget();
			NUnit.Framework.Assert.AreEqual("refs/3", r.GetName());
			NUnit.Framework.Assert.AreEqual(A, r.GetObjectId());
			NUnit.Framework.Assert.IsTrue(r.IsSymbolic());
			r = r.GetTarget();
			NUnit.Framework.Assert.AreEqual("refs/4", r.GetName());
			NUnit.Framework.Assert.AreEqual(A, r.GetObjectId());
			NUnit.Framework.Assert.IsTrue(r.IsSymbolic());
			r = r.GetTarget();
			NUnit.Framework.Assert.AreEqual("refs/5", r.GetName());
			NUnit.Framework.Assert.AreEqual(A, r.GetObjectId());
			NUnit.Framework.Assert.IsTrue(r.IsSymbolic());
			r = r.GetTarget();
			NUnit.Framework.Assert.AreEqual("refs/end", r.GetName());
			NUnit.Framework.Assert.AreEqual(A, r.GetObjectId());
			NUnit.Framework.Assert.IsFalse(r.IsSymbolic());
			WriteLooseRef("refs/5", "ref: refs/6\n");
			WriteLooseRef("refs/6", "ref: refs/end\n");
			all = refdir.GetRefs(RefDatabase.ALL);
			r = all.Get("refs/1");
			NUnit.Framework.Assert.IsNull(r, "mising 1 due to cycle");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_PackedNotPeeled_Sorted()
		{
			IDictionary<string, Ref> all;
			WritePackedRefs(string.Empty + A.Name + " refs/heads/master\n" + B.Name + " refs/heads/other\n"
				 + v1_0.Name + " refs/tags/v1.0\n");
			//
			//
			//
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(4, all.Count);
			Ref head = all.Get(Constants.HEAD);
			Ref master = all.Get("refs/heads/master");
			Ref other = all.Get("refs/heads/other");
			Ref tag = all.Get("refs/tags/v1.0");
			NUnit.Framework.Assert.AreEqual(A, master.GetObjectId());
			NUnit.Framework.Assert.IsFalse(master.IsPeeled());
			NUnit.Framework.Assert.IsNull(master.GetPeeledObjectId());
			NUnit.Framework.Assert.AreEqual(B, other.GetObjectId());
			NUnit.Framework.Assert.IsFalse(other.IsPeeled());
			NUnit.Framework.Assert.IsNull(other.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame(master, head.GetTarget());
			NUnit.Framework.Assert.AreEqual(A, head.GetObjectId());
			NUnit.Framework.Assert.IsFalse(head.IsPeeled());
			NUnit.Framework.Assert.IsNull(head.GetPeeledObjectId());
			NUnit.Framework.Assert.AreEqual(v1_0, tag.GetObjectId());
			NUnit.Framework.Assert.IsFalse(tag.IsPeeled());
			NUnit.Framework.Assert.IsNull(tag.GetPeeledObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRef_PackedNotPeeled_WrongSort()
		{
			WritePackedRefs(string.Empty + v1_0.Name + " refs/tags/v1.0\n" + B.Name + " refs/heads/other\n"
				 + A.Name + " refs/heads/master\n");
			//
			//
			//
			Ref head = refdir.GetRef(Constants.HEAD);
			Ref master = refdir.GetRef("refs/heads/master");
			Ref other = refdir.GetRef("refs/heads/other");
			Ref tag = refdir.GetRef("refs/tags/v1.0");
			NUnit.Framework.Assert.AreEqual(A, master.GetObjectId());
			NUnit.Framework.Assert.IsFalse(master.IsPeeled());
			NUnit.Framework.Assert.IsNull(master.GetPeeledObjectId());
			NUnit.Framework.Assert.AreEqual(B, other.GetObjectId());
			NUnit.Framework.Assert.IsFalse(other.IsPeeled());
			NUnit.Framework.Assert.IsNull(other.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame(master, head.GetTarget());
			NUnit.Framework.Assert.AreEqual(A, head.GetObjectId());
			NUnit.Framework.Assert.IsFalse(head.IsPeeled());
			NUnit.Framework.Assert.IsNull(head.GetPeeledObjectId());
			NUnit.Framework.Assert.AreEqual(v1_0, tag.GetObjectId());
			NUnit.Framework.Assert.IsFalse(tag.IsPeeled());
			NUnit.Framework.Assert.IsNull(tag.GetPeeledObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_PackedWithPeeled()
		{
			IDictionary<string, Ref> all;
			WritePackedRefs("# pack-refs with: peeled \n" + A.Name + " refs/heads/master\n" +
				 B.Name + " refs/heads/other\n" + v1_0.Name + " refs/tags/v1.0\n" + "^" + v1_0.GetObject
				().Name + "\n");
			//
			//
			//
			//
			all = refdir.GetRefs(RefDatabase.ALL);
			NUnit.Framework.Assert.AreEqual(4, all.Count);
			Ref head = all.Get(Constants.HEAD);
			Ref master = all.Get("refs/heads/master");
			Ref other = all.Get("refs/heads/other");
			Ref tag = all.Get("refs/tags/v1.0");
			NUnit.Framework.Assert.AreEqual(A, master.GetObjectId());
			NUnit.Framework.Assert.IsTrue(master.IsPeeled());
			NUnit.Framework.Assert.IsNull(master.GetPeeledObjectId());
			NUnit.Framework.Assert.AreEqual(B, other.GetObjectId());
			NUnit.Framework.Assert.IsTrue(other.IsPeeled());
			NUnit.Framework.Assert.IsNull(other.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame(master, head.GetTarget());
			NUnit.Framework.Assert.AreEqual(A, head.GetObjectId());
			NUnit.Framework.Assert.IsTrue(head.IsPeeled());
			NUnit.Framework.Assert.IsNull(head.GetPeeledObjectId());
			NUnit.Framework.Assert.AreEqual(v1_0, tag.GetObjectId());
			NUnit.Framework.Assert.IsTrue(tag.IsPeeled());
			NUnit.Framework.Assert.AreEqual(v1_0.GetObject(), tag.GetPeeledObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRef_EmptyDatabase()
		{
			Ref r;
			r = refdir.GetRef(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(r.IsSymbolic());
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, r.GetStorage());
			NUnit.Framework.Assert.AreEqual("refs/heads/master", r.GetTarget().GetName());
			NUnit.Framework.Assert.AreEqual(RefStorage.NEW, r.GetTarget().GetStorage());
			NUnit.Framework.Assert.IsNull(r.GetTarget().GetObjectId());
			NUnit.Framework.Assert.IsNull(refdir.GetRef("refs/heads/master"));
			NUnit.Framework.Assert.IsNull(refdir.GetRef("refs/tags/v1.0"));
			NUnit.Framework.Assert.IsNull(refdir.GetRef("FETCH_HEAD"));
			NUnit.Framework.Assert.IsNull(refdir.GetRef("NOT.A.REF.NAME"));
			NUnit.Framework.Assert.IsNull(refdir.GetRef("master"));
			NUnit.Framework.Assert.IsNull(refdir.GetRef("v1.0"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRef_FetchHead()
		{
			// This is an odd special case where we need to make sure we read
			// exactly the first 40 bytes of the file and nothing further on
			// that line, or the remainder of the file.
			Write(new FilePath(diskRepo.Directory, "FETCH_HEAD"), A.Name + "\tnot-for-merge" 
				+ "\tbranch 'master' of git://egit.eclipse.org/jgit\n");
			Ref r = refdir.GetRef("FETCH_HEAD");
			NUnit.Framework.Assert.IsFalse(r.IsSymbolic());
			NUnit.Framework.Assert.AreEqual(A, r.GetObjectId());
			NUnit.Framework.Assert.AreEqual("FETCH_HEAD", r.GetName());
			NUnit.Framework.Assert.IsFalse(r.IsPeeled());
			NUnit.Framework.Assert.IsNull(r.GetPeeledObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRef_AnyHeadWithGarbage()
		{
			Write(new FilePath(diskRepo.Directory, "refs/heads/A"), A.Name + "012345 . this is not a standard reference\n"
				 + "#and even more junk\n");
			Ref r = refdir.GetRef("refs/heads/A");
			NUnit.Framework.Assert.IsFalse(r.IsSymbolic());
			NUnit.Framework.Assert.AreEqual(A, r.GetObjectId());
			NUnit.Framework.Assert.AreEqual("refs/heads/A", r.GetName());
			NUnit.Framework.Assert.IsFalse(r.IsPeeled());
			NUnit.Framework.Assert.IsNull(r.GetPeeledObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_CorruptSymbolicReference()
		{
			string name = "refs/heads/A";
			WriteLooseRef(name, "ref: \n");
			NUnit.Framework.Assert.IsTrue(refdir.GetRefs(RefDatabase.ALL).IsEmpty());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRef_CorruptSymbolicReference()
		{
			string name = "refs/heads/A";
			WriteLooseRef(name, "ref: \n");
			try
			{
				refdir.GetRef(name);
				NUnit.Framework.Assert.Fail("read an invalid reference");
			}
			catch (IOException err)
			{
				string msg = err.Message;
				NUnit.Framework.Assert.AreEqual("Not a ref: " + name + ": ref:", msg);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRefs_CorruptObjectIdReference()
		{
			string name = "refs/heads/A";
			string content = "zoo" + A.Name;
			WriteLooseRef(name, content + "\n");
			NUnit.Framework.Assert.IsTrue(refdir.GetRefs(RefDatabase.ALL).IsEmpty());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetRef_CorruptObjectIdReference()
		{
			string name = "refs/heads/A";
			string content = "zoo" + A.Name;
			WriteLooseRef(name, content + "\n");
			try
			{
				refdir.GetRef(name);
				NUnit.Framework.Assert.Fail("read an invalid reference");
			}
			catch (IOException err)
			{
				string msg = err.Message;
				NUnit.Framework.Assert.AreEqual("Not a ref: " + name + ": " + content, msg);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIsNameConflicting()
		{
			WriteLooseRef("refs/heads/a/b", A);
			WritePackedRef("refs/heads/q", B);
			// new references cannot replace an existing container
			NUnit.Framework.Assert.IsTrue(refdir.IsNameConflicting("refs"));
			NUnit.Framework.Assert.IsTrue(refdir.IsNameConflicting("refs/heads"));
			NUnit.Framework.Assert.IsTrue(refdir.IsNameConflicting("refs/heads/a"));
			// existing reference is not conflicting
			NUnit.Framework.Assert.IsFalse(refdir.IsNameConflicting("refs/heads/a/b"));
			// new references are not conflicting
			NUnit.Framework.Assert.IsFalse(refdir.IsNameConflicting("refs/heads/a/d"));
			NUnit.Framework.Assert.IsFalse(refdir.IsNameConflicting("refs/heads/master"));
			// existing reference must not be used as a container
			NUnit.Framework.Assert.IsTrue(refdir.IsNameConflicting("refs/heads/a/b/c"));
			NUnit.Framework.Assert.IsTrue(refdir.IsNameConflicting("refs/heads/q/master"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPeelLooseTag()
		{
			WriteLooseRef("refs/tags/v1_0", v1_0);
			WriteLooseRef("refs/tags/current", "ref: refs/tags/v1_0\n");
			Ref tag = refdir.GetRef("refs/tags/v1_0");
			Ref cur = refdir.GetRef("refs/tags/current");
			NUnit.Framework.Assert.AreEqual(v1_0, tag.GetObjectId());
			NUnit.Framework.Assert.IsFalse(tag.IsSymbolic());
			NUnit.Framework.Assert.IsFalse(tag.IsPeeled());
			NUnit.Framework.Assert.IsNull(tag.GetPeeledObjectId());
			NUnit.Framework.Assert.AreEqual(v1_0, cur.GetObjectId());
			NUnit.Framework.Assert.IsTrue(cur.IsSymbolic());
			NUnit.Framework.Assert.IsFalse(cur.IsPeeled());
			NUnit.Framework.Assert.IsNull(cur.GetPeeledObjectId());
			Ref tag_p = refdir.Peel(tag);
			Ref cur_p = refdir.Peel(cur);
			NUnit.Framework.Assert.AreNotSame(tag, tag_p);
			NUnit.Framework.Assert.IsFalse(tag_p.IsSymbolic());
			NUnit.Framework.Assert.IsTrue(tag_p.IsPeeled());
			NUnit.Framework.Assert.AreEqual(v1_0, tag_p.GetObjectId());
			NUnit.Framework.Assert.AreEqual(v1_0.GetObject(), tag_p.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame(tag_p, refdir.Peel(tag_p));
			NUnit.Framework.Assert.AreNotSame(cur, cur_p);
			NUnit.Framework.Assert.AreEqual("refs/tags/current", cur_p.GetName());
			NUnit.Framework.Assert.IsTrue(cur_p.IsSymbolic());
			NUnit.Framework.Assert.AreEqual("refs/tags/v1_0", cur_p.GetTarget().GetName());
			NUnit.Framework.Assert.IsTrue(cur_p.IsPeeled());
			NUnit.Framework.Assert.AreEqual(v1_0, cur_p.GetObjectId());
			NUnit.Framework.Assert.AreEqual(v1_0.GetObject(), cur_p.GetPeeledObjectId());
			// reuses cached peeling later, but not immediately due to
			// the implementation so we have to fetch it once.
			Ref tag_p2 = refdir.GetRef("refs/tags/v1_0");
			NUnit.Framework.Assert.IsFalse(tag_p2.IsSymbolic());
			NUnit.Framework.Assert.IsTrue(tag_p2.IsPeeled());
			NUnit.Framework.Assert.AreEqual(v1_0, tag_p2.GetObjectId());
			NUnit.Framework.Assert.AreEqual(v1_0.GetObject(), tag_p2.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame(tag_p2, refdir.GetRef("refs/tags/v1_0"));
			NUnit.Framework.Assert.AreSame(tag_p2, refdir.GetRef("refs/tags/current").GetTarget
				());
			NUnit.Framework.Assert.AreSame(tag_p2, refdir.Peel(tag_p2));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPeelCommit()
		{
			WriteLooseRef("refs/heads/master", A);
			Ref master = refdir.GetRef("refs/heads/master");
			NUnit.Framework.Assert.AreEqual(A, master.GetObjectId());
			NUnit.Framework.Assert.IsFalse(master.IsPeeled());
			NUnit.Framework.Assert.IsNull(master.GetPeeledObjectId());
			Ref master_p = refdir.Peel(master);
			NUnit.Framework.Assert.AreNotSame(master, master_p);
			NUnit.Framework.Assert.AreEqual(A, master_p.GetObjectId());
			NUnit.Framework.Assert.IsTrue(master_p.IsPeeled());
			NUnit.Framework.Assert.IsNull(master_p.GetPeeledObjectId());
			// reuses cached peeling later, but not immediately due to
			// the implementation so we have to fetch it once.
			Ref master_p2 = refdir.GetRef("refs/heads/master");
			NUnit.Framework.Assert.AreNotSame(master, master_p2);
			NUnit.Framework.Assert.AreEqual(A, master_p2.GetObjectId());
			NUnit.Framework.Assert.IsTrue(master_p2.IsPeeled());
			NUnit.Framework.Assert.IsNull(master_p2.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame(master_p2, refdir.Peel(master_p2));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WriteLooseRef(string name, AnyObjectId id)
		{
			WriteLooseRef(name, id.Name + "\n");
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WriteLooseRef(string name, string content)
		{
			Write(new FilePath(diskRepo.Directory, name), content);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WritePackedRef(string name, AnyObjectId id)
		{
			WritePackedRefs(id.Name + " " + name + "\n");
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WritePackedRefs(string content)
		{
			FilePath pr = new FilePath(diskRepo.Directory, "packed-refs");
			Write(pr, content);
			long now = Runtime.CurrentTimeMillis();
			int oneHourAgo = 3600 * 1000;
			pr.SetLastModified(now - oneHourAgo);
		}

		private void DeleteLooseRef(string name)
		{
			FilePath path = new FilePath(diskRepo.Directory, name);
			NUnit.Framework.Assert.IsTrue(path.Delete(), "deleted " + name);
		}
	}
}
