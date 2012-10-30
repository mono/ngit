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
using NGit.Errors;
using NGit.Junit;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Storage.Pack;
using NGit.Transport;
using Sharpen;

namespace NGit.Storage.File
{
	[NUnit.Framework.TestFixture]
	public class PackWriterTest : SampleDataRepositoryTestCase
	{
		private static readonly ICollection<ObjectId> EMPTY_SET_OBJECT = Sharpen.Collections
			.EmptySet<ObjectId>();

		private static readonly IList<RevObject> EMPTY_LIST_REVS = Sharpen.Collections.EmptyList
			<RevObject>();

		private PackConfig config;

		private PackWriter writer;

		private ByteArrayOutputStream os;

		private PackFile pack;

		private ObjectInserter inserter;

		private FileRepository dst;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			os = new ByteArrayOutputStream();
			config = new PackConfig(db);
			dst = CreateBareRepository();
			FilePath alt = new FilePath(((ObjectDirectory)dst.ObjectDatabase).GetDirectory(), 
				"info/alternates");
			alt.GetParentFile().Mkdirs();
			Write(alt, ((ObjectDirectory)db.ObjectDatabase).GetDirectory().GetAbsolutePath() 
				+ "\n");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.TearDown]
		public override void TearDown()
		{
			if (writer != null)
			{
				writer.Release();
				writer = null;
			}
			if (inserter != null)
			{
				inserter.Release();
				inserter = null;
			}
			base.TearDown();
		}

		/// <summary>Test constructor for exceptions, default settings, initialization.</summary>
		/// <remarks>Test constructor for exceptions, default settings, initialization.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestContructor()
		{
			writer = new PackWriter(config, db.NewObjectReader());
			NUnit.Framework.Assert.IsFalse(writer.IsDeltaBaseAsOffset());
			NUnit.Framework.Assert.IsTrue(config.IsReuseDeltas());
			NUnit.Framework.Assert.IsTrue(config.IsReuseObjects());
			NUnit.Framework.Assert.AreEqual(0, writer.GetObjectCount());
		}

		/// <summary>Change default settings and verify them.</summary>
		/// <remarks>Change default settings and verify them.</remarks>
		[NUnit.Framework.Test]
		public virtual void TestModifySettings()
		{
			config.SetReuseDeltas(false);
			config.SetReuseObjects(false);
			config.SetDeltaBaseAsOffset(false);
			NUnit.Framework.Assert.IsFalse(config.IsReuseDeltas());
			NUnit.Framework.Assert.IsFalse(config.IsReuseObjects());
			NUnit.Framework.Assert.IsFalse(config.IsDeltaBaseAsOffset());
			writer = new PackWriter(config, db.NewObjectReader());
			writer.SetDeltaBaseAsOffset(true);
			NUnit.Framework.Assert.IsTrue(writer.IsDeltaBaseAsOffset());
			NUnit.Framework.Assert.IsFalse(config.IsDeltaBaseAsOffset());
		}

		/// <summary>
		/// Write empty pack by providing empty sets of interesting/uninteresting
		/// objects and check for correct format.
		/// </summary>
		/// <remarks>
		/// Write empty pack by providing empty sets of interesting/uninteresting
		/// objects and check for correct format.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestWriteEmptyPack1()
		{
			CreateVerifyOpenPack(EMPTY_SET_OBJECT, EMPTY_SET_OBJECT, false, false);
			NUnit.Framework.Assert.AreEqual(0, writer.GetObjectCount());
			NUnit.Framework.Assert.AreEqual(0, pack.GetObjectCount());
			NUnit.Framework.Assert.AreEqual("da39a3ee5e6b4b0d3255bfef95601890afd80709", writer
				.ComputeName().Name);
		}

		/// <summary>
		/// Write empty pack by providing empty iterator of objects to write and
		/// check for correct format.
		/// </summary>
		/// <remarks>
		/// Write empty pack by providing empty iterator of objects to write and
		/// check for correct format.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestWriteEmptyPack2()
		{
			CreateVerifyOpenPack(EMPTY_LIST_REVS);
			NUnit.Framework.Assert.AreEqual(0, writer.GetObjectCount());
			NUnit.Framework.Assert.AreEqual(0, pack.GetObjectCount());
		}

		/// <summary>
		/// Try to pass non-existing object as uninteresting, with non-ignoring
		/// setting.
		/// </summary>
		/// <remarks>
		/// Try to pass non-existing object as uninteresting, with non-ignoring
		/// setting.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestNotIgnoreNonExistingObjects()
		{
			ObjectId nonExisting = ObjectId.FromString("0000000000000000000000000000000000000001"
				);
			try
			{
				CreateVerifyOpenPack(EMPTY_SET_OBJECT, Sharpen.Collections.Singleton(nonExisting)
					, false, false);
				NUnit.Framework.Assert.Fail("Should have thrown MissingObjectException");
			}
			catch (MissingObjectException)
			{
			}
		}

		// expected
		/// <summary>Try to pass non-existing object as uninteresting, with ignoring setting.
		/// 	</summary>
		/// <remarks>Try to pass non-existing object as uninteresting, with ignoring setting.
		/// 	</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestIgnoreNonExistingObjects()
		{
			ObjectId nonExisting = ObjectId.FromString("0000000000000000000000000000000000000001"
				);
			CreateVerifyOpenPack(EMPTY_SET_OBJECT, Sharpen.Collections.Singleton(nonExisting)
				, false, true);
		}

		// shouldn't throw anything
		/// <summary>
		/// Create pack basing on only interesting objects, then precisely verify
		/// content.
		/// </summary>
		/// <remarks>
		/// Create pack basing on only interesting objects, then precisely verify
		/// content. No delta reuse here.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePack1()
		{
			config.SetReuseDeltas(false);
			WriteVerifyPack1();
		}

		/// <summary>Test writing pack without object reuse.</summary>
		/// <remarks>
		/// Test writing pack without object reuse. Pack content/preparation as in
		/// <see cref="TestWritePack1()">TestWritePack1()</see>
		/// .
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePack1NoObjectReuse()
		{
			config.SetReuseDeltas(false);
			config.SetReuseObjects(false);
			WriteVerifyPack1();
		}

		/// <summary>
		/// Create pack basing on both interesting and uninteresting objects, then
		/// precisely verify content.
		/// </summary>
		/// <remarks>
		/// Create pack basing on both interesting and uninteresting objects, then
		/// precisely verify content. No delta reuse here.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePack2()
		{
			WriteVerifyPack2(false);
		}

		/// <summary>Test pack writing with deltas reuse, delta-base first rule.</summary>
		/// <remarks>
		/// Test pack writing with deltas reuse, delta-base first rule. Pack
		/// content/preparation as in
		/// <see cref="TestWritePack2()">TestWritePack2()</see>
		/// .
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePack2DeltasReuseRefs()
		{
			WriteVerifyPack2(true);
		}

		/// <summary>Test pack writing with delta reuse.</summary>
		/// <remarks>
		/// Test pack writing with delta reuse. Delta bases referred as offsets. Pack
		/// configuration as in
		/// <see cref="TestWritePack2DeltasReuseRefs()">TestWritePack2DeltasReuseRefs()</see>
		/// .
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePack2DeltasReuseOffsets()
		{
			config.SetDeltaBaseAsOffset(true);
			WriteVerifyPack2(true);
		}

		/// <summary>Test pack writing with delta reuse.</summary>
		/// <remarks>
		/// Test pack writing with delta reuse. Raw-data copy (reuse) is made on a
		/// pack with CRC32 index. Pack configuration as in
		/// <see cref="TestWritePack2DeltasReuseRefs()">TestWritePack2DeltasReuseRefs()</see>
		/// .
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePack2DeltasCRC32Copy()
		{
			FilePath packDir = new FilePath(((ObjectDirectory)db.ObjectDatabase).GetDirectory
				(), "pack");
			FilePath crc32Pack = new FilePath(packDir, "pack-34be9032ac282b11fa9babdc2b2a93ca996c9c2f.pack"
				);
			FilePath crc32Idx = new FilePath(packDir, "pack-34be9032ac282b11fa9babdc2b2a93ca996c9c2f.idx"
				);
			CopyFile(JGitTestUtil.GetTestResourceFile("pack-34be9032ac282b11fa9babdc2b2a93ca996c9c2f.idxV2"
				), crc32Idx);
			db.OpenPack(crc32Pack, crc32Idx);
			WriteVerifyPack2(true);
		}

		/// <summary>Create pack basing on fixed objects list, then precisely verify content.
		/// 	</summary>
		/// <remarks>
		/// Create pack basing on fixed objects list, then precisely verify content.
		/// No delta reuse here.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		/// <exception cref="NGit.Errors.MissingObjectException">NGit.Errors.MissingObjectException
		/// 	</exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePack3()
		{
			config.SetReuseDeltas(false);
			ObjectId[] forcedOrder = new ObjectId[] { ObjectId.FromString("82c6b885ff600be425b4ea96dee75dca255b69e7"
				), ObjectId.FromString("c59759f143fb1fe21c197981df75a7ee00290799"), ObjectId.FromString
				("aabf2ffaec9b497f0950352b3e582d73035c2035"), ObjectId.FromString("902d5476fa249b7abc9d84c611577a81381f0327"
				), ObjectId.FromString("5b6e7c66c276e7610d4a73c70ec1a1f7c1003259"), ObjectId.FromString
				("6ff87c4664981e4397625791c8ea3bbb5f2279a3") };
			RevWalk parser = new RevWalk(db);
			RevObject[] forcedOrderRevs = new RevObject[forcedOrder.Length];
			for (int i = 0; i < forcedOrder.Length; i++)
			{
				forcedOrderRevs[i] = parser.ParseAny(forcedOrder[i]);
			}
			CreateVerifyOpenPack(Arrays.AsList(forcedOrderRevs));
			NUnit.Framework.Assert.AreEqual(forcedOrder.Length, writer.GetObjectCount());
			VerifyObjectsOrder(forcedOrder);
			NUnit.Framework.Assert.AreEqual("ed3f96b8327c7c66b0f8f70056129f0769323d86", writer
				.ComputeName().Name);
		}

		/// <summary>
		/// Another pack creation: basing on both interesting and uninteresting
		/// objects.
		/// </summary>
		/// <remarks>
		/// Another pack creation: basing on both interesting and uninteresting
		/// objects. No delta reuse possible here, as this is a specific case when we
		/// write only 1 commit, associated with 1 tree, 1 blob.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePack4()
		{
			WriteVerifyPack4(false);
		}

		/// <summary>Test thin pack writing: 1 blob delta base is on objects edge.</summary>
		/// <remarks>
		/// Test thin pack writing: 1 blob delta base is on objects edge. Pack
		/// configuration as in
		/// <see cref="TestWritePack4()">TestWritePack4()</see>
		/// .
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePack4ThinPack()
		{
			WriteVerifyPack4(true);
		}

		/// <summary>
		/// Compare sizes of packs created using
		/// <see cref="TestWritePack2()">TestWritePack2()</see>
		/// and
		/// <see cref="TestWritePack2DeltasReuseRefs()">TestWritePack2DeltasReuseRefs()</see>
		/// . The pack using deltas should
		/// be smaller.
		/// </summary>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePack2SizeDeltasVsNoDeltas()
		{
			TestWritePack2();
			long sizePack2NoDeltas = os.Size();
			TearDown();
			SetUp();
			TestWritePack2DeltasReuseRefs();
			long sizePack2DeltasRefs = os.Size();
			NUnit.Framework.Assert.IsTrue(sizePack2NoDeltas > sizePack2DeltasRefs);
		}

		/// <summary>
		/// Compare sizes of packs created using
		/// <see cref="TestWritePack2DeltasReuseRefs()">TestWritePack2DeltasReuseRefs()</see>
		/// and
		/// <see cref="TestWritePack2DeltasReuseOffsets()">TestWritePack2DeltasReuseOffsets()
		/// 	</see>
		/// . The pack with delta bases
		/// written as offsets should be smaller.
		/// </summary>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePack2SizeOffsetsVsRefs()
		{
			TestWritePack2DeltasReuseRefs();
			long sizePack2DeltasRefs = os.Size();
			TearDown();
			SetUp();
			TestWritePack2DeltasReuseOffsets();
			long sizePack2DeltasOffsets = os.Size();
			NUnit.Framework.Assert.IsTrue(sizePack2DeltasRefs > sizePack2DeltasOffsets);
		}

		/// <summary>
		/// Compare sizes of packs created using
		/// <see cref="TestWritePack4()">TestWritePack4()</see>
		/// and
		/// <see cref="TestWritePack4ThinPack()">TestWritePack4ThinPack()</see>
		/// . Obviously, the thin pack should be
		/// smaller.
		/// </summary>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePack4SizeThinVsNoThin()
		{
			TestWritePack4();
			long sizePack4 = os.Size();
			TearDown();
			SetUp();
			TestWritePack4ThinPack();
			long sizePack4Thin = os.Size();
			NUnit.Framework.Assert.IsTrue(sizePack4 > sizePack4Thin);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWriteIndex()
		{
			config.SetIndexVersion(2);
			WriteVerifyPack4(false);
			FilePath packFile = pack.GetPackFile();
			string name = packFile.GetName();
			string @base = Sharpen.Runtime.Substring(name, 0, name.LastIndexOf('.'));
			FilePath indexFile = new FilePath(packFile.GetParentFile(), @base + ".idx");
			// Validate that IndexPack came up with the right CRC32 value.
			PackIndex idx1 = PackIndex.Open(indexFile);
			NUnit.Framework.Assert.IsTrue(idx1 is PackIndexV2);
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x4743F1E4L)), idx1.FindCRC32(ObjectId
				.FromString("82c6b885ff600be425b4ea96dee75dca255b69e7")));
			// Validate that an index written by PackWriter is the same.
			FilePath idx2File = new FilePath(indexFile.GetAbsolutePath() + ".2");
			FileOutputStream @is = new FileOutputStream(idx2File);
			try
			{
				writer.WriteIndex(@is);
			}
			finally
			{
				@is.Close();
			}
			PackIndex idx2 = PackIndex.Open(idx2File);
			NUnit.Framework.Assert.IsTrue(idx2 is PackIndexV2);
			NUnit.Framework.Assert.AreEqual(idx1.GetObjectCount(), idx2.GetObjectCount());
			NUnit.Framework.Assert.AreEqual(idx1.GetOffset64Count(), idx2.GetOffset64Count());
			for (int i = 0; i < idx1.GetObjectCount(); i++)
			{
				ObjectId id = idx1.GetObjectId(i);
				NUnit.Framework.Assert.AreEqual(id, idx2.GetObjectId(i));
				NUnit.Framework.Assert.AreEqual(idx1.FindOffset(id), idx2.FindOffset(id));
				NUnit.Framework.Assert.AreEqual(idx1.FindCRC32(id), idx2.FindCRC32(id));
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestExclude()
		{
			FileRepository repo = CreateBareRepository();
			TestRepository<FileRepository> testRepo = new TestRepository<FileRepository>(repo
				);
			BranchBuilder bb = testRepo.Branch("refs/heads/master");
			RevBlob contentA = testRepo.Blob("A");
			RevCommit c1 = bb.Commit().Add("f", contentA).Create();
			testRepo.GetRevWalk().ParseHeaders(c1);
			PackIndex pf1 = WritePack(repo, Sharpen.Collections.Singleton(c1), Sharpen.Collections
				.EmptySet<PackIndex>());
			AssertContent(pf1, Arrays.AsList(c1.Id, c1.Tree.Id, contentA.Id));
			RevBlob contentB = testRepo.Blob("B");
			RevCommit c2 = bb.Commit().Add("f", contentB).Create();
			testRepo.GetRevWalk().ParseHeaders(c2);
			PackIndex pf2 = WritePack(repo, Sharpen.Collections.Singleton(c2), Sharpen.Collections
				.Singleton(pf1));
			AssertContent(pf2, Arrays.AsList(c2.Id, c2.Tree.Id, contentB.Id));
		}

		private void AssertContent(PackIndex pi, IList<ObjectId> expected)
		{
			NUnit.Framework.Assert.AreEqual(expected.Count, pi.GetObjectCount(), "Pack index has wrong size."
				);
			for (int i = 0; i < pi.GetObjectCount(); i++)
			{
				NUnit.Framework.Assert.IsTrue(expected.Contains(pi.GetObjectId(i)), "Pack index didn't contain the expected id "
					 + pi.GetObjectId(i));
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private PackIndex WritePack<_T0>(FileRepository repo, ICollection<_T0> want, ICollection
			<PackIndex> excludeObjects) where _T0:ObjectId
		{
			PackWriter pw = new PackWriter(repo);
			pw.SetDeltaBaseAsOffset(true);
			pw.SetReuseDeltaCommits(false);
			foreach (PackIndex idx in excludeObjects)
			{
				pw.ExcludeObjects(idx);
			}
			pw.PreparePack(NullProgressMonitor.INSTANCE, want, Sharpen.Collections.EmptySet<ObjectId
				>());
			string id = pw.ComputeName().GetName();
			FilePath packdir = new FilePath(repo.ObjectsDirectory, "pack");
			FilePath packFile = new FilePath(packdir, "pack-" + id + ".pack");
			FileOutputStream packOS = new FileOutputStream(packFile);
			pw.WritePack(NullProgressMonitor.INSTANCE, NullProgressMonitor.INSTANCE, packOS);
			packOS.Close();
			FilePath idxFile = new FilePath(packdir, "pack-" + id + ".idx");
			FileOutputStream idxOS = new FileOutputStream(idxFile);
			pw.WriteIndex(idxOS);
			idxOS.Close();
			pw.Release();
			return PackIndex.Open(idxFile);
		}

		// TODO: testWritePackDeltasCycle()
		// TODO: testWritePackDeltasDepth()
		/// <exception cref="System.IO.IOException"></exception>
		private void WriteVerifyPack1()
		{
			HashSet<ObjectId> interestings = new HashSet<ObjectId>();
			interestings.AddItem(ObjectId.FromString("82c6b885ff600be425b4ea96dee75dca255b69e7"
				));
			CreateVerifyOpenPack(interestings, EMPTY_SET_OBJECT, false, false);
			ObjectId[] expectedOrder = new ObjectId[] { ObjectId.FromString("82c6b885ff600be425b4ea96dee75dca255b69e7"
				), ObjectId.FromString("c59759f143fb1fe21c197981df75a7ee00290799"), ObjectId.FromString
				("540a36d136cf413e4b064c2b0e0a4db60f77feab"), ObjectId.FromString("aabf2ffaec9b497f0950352b3e582d73035c2035"
				), ObjectId.FromString("902d5476fa249b7abc9d84c611577a81381f0327"), ObjectId.FromString
				("4b825dc642cb6eb9a060e54bf8d69288fbee4904"), ObjectId.FromString("5b6e7c66c276e7610d4a73c70ec1a1f7c1003259"
				), ObjectId.FromString("6ff87c4664981e4397625791c8ea3bbb5f2279a3") };
			NUnit.Framework.Assert.AreEqual(expectedOrder.Length, writer.GetObjectCount());
			VerifyObjectsOrder(expectedOrder);
			NUnit.Framework.Assert.AreEqual("34be9032ac282b11fa9babdc2b2a93ca996c9c2f", writer
				.ComputeName().Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WriteVerifyPack2(bool deltaReuse)
		{
			config.SetReuseDeltas(deltaReuse);
			HashSet<ObjectId> interestings = new HashSet<ObjectId>();
			interestings.AddItem(ObjectId.FromString("82c6b885ff600be425b4ea96dee75dca255b69e7"
				));
			HashSet<ObjectId> uninterestings = new HashSet<ObjectId>();
			uninterestings.AddItem(ObjectId.FromString("540a36d136cf413e4b064c2b0e0a4db60f77feab"
				));
			CreateVerifyOpenPack(interestings, uninterestings, false, false);
			ObjectId[] expectedOrder = new ObjectId[] { ObjectId.FromString("82c6b885ff600be425b4ea96dee75dca255b69e7"
				), ObjectId.FromString("c59759f143fb1fe21c197981df75a7ee00290799"), ObjectId.FromString
				("aabf2ffaec9b497f0950352b3e582d73035c2035"), ObjectId.FromString("902d5476fa249b7abc9d84c611577a81381f0327"
				), ObjectId.FromString("5b6e7c66c276e7610d4a73c70ec1a1f7c1003259"), ObjectId.FromString
				("6ff87c4664981e4397625791c8ea3bbb5f2279a3") };
			if (deltaReuse)
			{
				// objects order influenced (swapped) by delta-base first rule
				ObjectId temp = expectedOrder[4];
				expectedOrder[4] = expectedOrder[5];
				expectedOrder[5] = temp;
			}
			NUnit.Framework.Assert.AreEqual(expectedOrder.Length, writer.GetObjectCount());
			VerifyObjectsOrder(expectedOrder);
			NUnit.Framework.Assert.AreEqual("ed3f96b8327c7c66b0f8f70056129f0769323d86", writer
				.ComputeName().Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WriteVerifyPack4(bool thin)
		{
			HashSet<ObjectId> interestings = new HashSet<ObjectId>();
			interestings.AddItem(ObjectId.FromString("82c6b885ff600be425b4ea96dee75dca255b69e7"
				));
			HashSet<ObjectId> uninterestings = new HashSet<ObjectId>();
			uninterestings.AddItem(ObjectId.FromString("c59759f143fb1fe21c197981df75a7ee00290799"
				));
			CreateVerifyOpenPack(interestings, uninterestings, thin, false);
			ObjectId[] writtenObjects = new ObjectId[] { ObjectId.FromString("82c6b885ff600be425b4ea96dee75dca255b69e7"
				), ObjectId.FromString("aabf2ffaec9b497f0950352b3e582d73035c2035"), ObjectId.FromString
				("5b6e7c66c276e7610d4a73c70ec1a1f7c1003259") };
			NUnit.Framework.Assert.AreEqual(writtenObjects.Length, writer.GetObjectCount());
			ObjectId[] expectedObjects;
			if (thin)
			{
				expectedObjects = new ObjectId[4];
				System.Array.Copy(writtenObjects, 0, expectedObjects, 0, writtenObjects.Length);
				expectedObjects[3] = ObjectId.FromString("6ff87c4664981e4397625791c8ea3bbb5f2279a3"
					);
			}
			else
			{
				expectedObjects = writtenObjects;
			}
			VerifyObjectsOrder(expectedObjects);
			NUnit.Framework.Assert.AreEqual("cded4b74176b4456afa456768b2b5aafb41c44fc", writer
				.ComputeName().Name);
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private void CreateVerifyOpenPack(ICollection<ObjectId> interestings, ICollection
			<ObjectId> uninterestings, bool thin, bool ignoreMissingUninteresting)
		{
			NullProgressMonitor m = NullProgressMonitor.INSTANCE;
			writer = new PackWriter(config, db.NewObjectReader());
			writer.SetThin(thin);
			writer.SetIgnoreMissingUninteresting(ignoreMissingUninteresting);
			writer.PreparePack(m, interestings, uninterestings);
			writer.WritePack(m, m, os);
			writer.Release();
			VerifyOpenPack(thin);
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private void CreateVerifyOpenPack(IList<RevObject> objectSource)
		{
			NullProgressMonitor m = NullProgressMonitor.INSTANCE;
			writer = new PackWriter(config, db.NewObjectReader());
			writer.PreparePack(objectSource.Iterator());
			NUnit.Framework.Assert.AreEqual(objectSource.Count, writer.GetObjectCount());
			writer.WritePack(m, m, os);
			writer.Release();
			VerifyOpenPack(false);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void VerifyOpenPack(bool thin)
		{
			byte[] packData = os.ToByteArray();
			if (thin)
			{
				PackParser p = Index(packData);
				try
				{
					p.Parse(NullProgressMonitor.INSTANCE);
					NUnit.Framework.Assert.Fail("indexer should grumble about missing object");
				}
				catch (IOException)
				{
				}
			}
			// expected
			ObjectDirectoryPackParser p_1 = (ObjectDirectoryPackParser)Index(packData);
			p_1.SetKeepEmpty(true);
			p_1.SetAllowThin(thin);
			p_1.SetIndexVersion(2);
			p_1.Parse(NullProgressMonitor.INSTANCE);
			pack = p_1.GetPackFile();
			NUnit.Framework.Assert.IsNotNull(pack, "have PackFile after parsing");
		}

		/// <exception cref="System.IO.IOException"></exception>
		private PackParser Index(byte[] packData)
		{
			if (inserter == null)
			{
				inserter = dst.NewObjectInserter();
			}
			return inserter.NewPackParser(new ByteArrayInputStream(packData));
		}

		private void VerifyObjectsOrder(ObjectId[] objectsOrder)
		{
			IList<PackIndex.MutableEntry> entries = new AList<PackIndex.MutableEntry>();
			foreach (PackIndex.MutableEntry me in pack)
			{
				entries.AddItem(me.CloneEntry());
			}
			entries.Sort(new _IComparer_660());
			int i = 0;
			foreach (PackIndex.MutableEntry me_1 in entries)
			{
				NUnit.Framework.Assert.AreEqual(objectsOrder[i++].ToObjectId(), me_1.ToObjectId()
					);
			}
		}

		private sealed class _IComparer_660 : IComparer<PackIndex.MutableEntry>
		{
			public _IComparer_660()
			{
			}

			public int Compare(PackIndex.MutableEntry o1, PackIndex.MutableEntry o2)
			{
				return Sharpen.Extensions.Signum(o1.GetOffset() - o2.GetOffset());
			}
		}
	}
}
