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

using System.Text;
using NGit;
using NGit.Junit;
using NGit.Notes;
using NGit.Revwalk;
using NGit.Treewalk;
using NGit.Util;
using Sharpen;

namespace NGit.Notes
{
	[NUnit.Framework.TestFixture]
	public class NoteMapTest : RepositoryTestCase
	{
		private TestRepository<Repository> tr;

		private ObjectReader reader;

		private ObjectInserter inserter;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			tr = new TestRepository<Repository>(db);
			reader = db.NewObjectReader();
			inserter = db.NewObjectInserter();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.TearDown]
		public override void TearDown()
		{
			reader.Release();
			inserter.Release();
			base.TearDown();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadFlatTwoNotes()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			RevCommit r = tr.Commit().Add(a.Name, data1).Add(b.Name, data2).Create();
			//
			//
			//
			tr.ParseBody(r);
			NoteMap map = NoteMap.Read(reader, r);
			NUnit.Framework.Assert.IsNotNull(map, "have map");
			NUnit.Framework.Assert.IsTrue(map.Contains(a), "has note for a");
			NUnit.Framework.Assert.IsTrue(map.Contains(b), "has note for b");
			NUnit.Framework.Assert.AreEqual(data1, map.Get(a));
			NUnit.Framework.Assert.AreEqual(data2, map.Get(b));
			NUnit.Framework.Assert.IsFalse(map.Contains(data1), "no note for data1");
			NUnit.Framework.Assert.IsNull(map.Get(data1), "no note for data1");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadFanout2_38()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			RevCommit r = tr.Commit().Add(Fanout(2, a.Name), data1).Add(Fanout(2, b.Name), data2
				).Create();
			//
			//
			//
			tr.ParseBody(r);
			NoteMap map = NoteMap.Read(reader, r);
			NUnit.Framework.Assert.IsNotNull(map, "have map");
			NUnit.Framework.Assert.IsTrue(map.Contains(a), "has note for a");
			NUnit.Framework.Assert.IsTrue(map.Contains(b), "has note for b");
			NUnit.Framework.Assert.AreEqual(data1, map.Get(a));
			NUnit.Framework.Assert.AreEqual(data2, map.Get(b));
			NUnit.Framework.Assert.IsFalse(map.Contains(data1), "no note for data1");
			NUnit.Framework.Assert.IsNull(map.Get(data1), "no note for data1");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadFanout2_2_36()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			RevCommit r = tr.Commit().Add(Fanout(4, a.Name), data1).Add(Fanout(4, b.Name), data2
				).Create();
			//
			//
			//
			tr.ParseBody(r);
			NoteMap map = NoteMap.Read(reader, r);
			NUnit.Framework.Assert.IsNotNull(map, "have map");
			NUnit.Framework.Assert.IsTrue(map.Contains(a), "has note for a");
			NUnit.Framework.Assert.IsTrue(map.Contains(b), "has note for b");
			NUnit.Framework.Assert.AreEqual(data1, map.Get(a));
			NUnit.Framework.Assert.AreEqual(data2, map.Get(b));
			NUnit.Framework.Assert.IsFalse(map.Contains(data1), "no note for data1");
			NUnit.Framework.Assert.IsNull(map.Get(data1), "no note for data1");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadFullyFannedOut()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			RevCommit r = tr.Commit().Add(Fanout(38, a.Name), data1).Add(Fanout(38, b.Name), 
				data2).Create();
			//
			//
			//
			tr.ParseBody(r);
			NoteMap map = NoteMap.Read(reader, r);
			NUnit.Framework.Assert.IsNotNull(map, "have map");
			NUnit.Framework.Assert.IsTrue(map.Contains(a), "has note for a");
			NUnit.Framework.Assert.IsTrue(map.Contains(b), "has note for b");
			NUnit.Framework.Assert.AreEqual(data1, map.Get(a));
			NUnit.Framework.Assert.AreEqual(data2, map.Get(b));
			NUnit.Framework.Assert.IsFalse(map.Contains(data1), "no note for data1");
			NUnit.Framework.Assert.IsNull(map.Get(data1), "no note for data1");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetCachedBytes()
		{
			string exp = "this is test data";
			RevBlob a = tr.Blob("a");
			RevBlob data = tr.Blob(exp);
			RevCommit r = tr.Commit().Add(a.Name, data).Create();
			//
			//
			tr.ParseBody(r);
			NoteMap map = NoteMap.Read(reader, r);
			byte[] act = map.GetCachedBytes(a, exp.Length * 4);
			NUnit.Framework.Assert.IsNotNull(act, "has data for a");
			NUnit.Framework.Assert.AreEqual(exp, RawParseUtils.Decode(act));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWriteUnchangedFlat()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			RevCommit r = tr.Commit().Add(a.Name, data1).Add(b.Name, data2).Add(".gitignore", 
				string.Empty).Add("zoo-animals.txt", string.Empty).Create();
			//
			//
			//
			//
			//
			tr.ParseBody(r);
			NoteMap map = NoteMap.Read(reader, r);
			NUnit.Framework.Assert.IsTrue(map.Contains(a), "has note for a");
			NUnit.Framework.Assert.IsTrue(map.Contains(b), "has note for b");
			RevCommit n = CommitNoteMap(map);
			NUnit.Framework.Assert.AreNotSame(r, n, "is new commit");
			NUnit.Framework.Assert.AreSame(r.Tree, n.Tree, "same tree");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWriteUnchangedFanout2_38()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			RevCommit r = tr.Commit().Add(Fanout(2, a.Name), data1).Add(Fanout(2, b.Name), data2
				).Add(".gitignore", string.Empty).Add("zoo-animals.txt", string.Empty).Create();
			//
			//
			//
			//
			//
			tr.ParseBody(r);
			NoteMap map = NoteMap.Read(reader, r);
			NUnit.Framework.Assert.IsTrue(map.Contains(a), "has note for a");
			NUnit.Framework.Assert.IsTrue(map.Contains(b), "has note for b");
			// This is a non-lazy map, so we'll be looking at the leaf buckets.
			RevCommit n = CommitNoteMap(map);
			NUnit.Framework.Assert.AreNotSame(r, n, "is new commit");
			NUnit.Framework.Assert.AreSame(r.Tree, n.Tree, "same tree");
			// Use a lazy-map for the next round of the same test.
			map = NoteMap.Read(reader, r);
			n = CommitNoteMap(map);
			NUnit.Framework.Assert.AreNotSame(r, n, "is new commit");
			NUnit.Framework.Assert.AreSame(r.Tree, n.Tree, "same tree");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCreateFromEmpty()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			NoteMap map = NoteMap.NewEmptyMap();
			NUnit.Framework.Assert.IsFalse(map.Contains(a), "no a");
			NUnit.Framework.Assert.IsFalse(map.Contains(b), "no b");
			map.Set(a, data1);
			map.Set(b, data2);
			NUnit.Framework.Assert.AreEqual(data1, map.Get(a));
			NUnit.Framework.Assert.AreEqual(data2, map.Get(b));
			map.Remove(a);
			map.Remove(b);
			NUnit.Framework.Assert.IsFalse(map.Contains(a), "no a");
			NUnit.Framework.Assert.IsFalse(map.Contains(b), "no b");
			map.Set(a, "data1", inserter);
			NUnit.Framework.Assert.AreEqual(data1, map.Get(a));
			map.Set(a, null, inserter);
			NUnit.Framework.Assert.IsFalse(map.Contains(a), "no a");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEditFlat()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			RevCommit r = tr.Commit().Add(a.Name, data1).Add(b.Name, data2).Add(".gitignore", 
				string.Empty).Add("zoo-animals.txt", b).Create();
			//
			//
			//
			//
			//
			tr.ParseBody(r);
			NoteMap map = NoteMap.Read(reader, r);
			map.Set(a, data2);
			map.Set(b, null);
			map.Set(data1, b);
			map.Set(data2, null);
			NUnit.Framework.Assert.AreEqual(data2, map.Get(a));
			NUnit.Framework.Assert.AreEqual(b, map.Get(data1));
			NUnit.Framework.Assert.IsFalse(map.Contains(b), "no b");
			NUnit.Framework.Assert.IsFalse(map.Contains(data2), "no data2");
			MutableObjectId id = new MutableObjectId();
			for (int p = 42; p > 0; p--)
			{
				id.SetByte(1, p);
				map.Set(id, data1);
			}
			for (int p_1 = 42; p_1 > 0; p_1--)
			{
				id.SetByte(1, p_1);
				NUnit.Framework.Assert.IsTrue(map.Contains(id), "contains " + id);
			}
			RevCommit n = CommitNoteMap(map);
			map = NoteMap.Read(reader, n);
			NUnit.Framework.Assert.AreEqual(data2, map.Get(a));
			NUnit.Framework.Assert.AreEqual(b, map.Get(data1));
			NUnit.Framework.Assert.IsFalse(map.Contains(b), "no b");
			NUnit.Framework.Assert.IsFalse(map.Contains(data2), "no data2");
			NUnit.Framework.Assert.AreEqual(b, TreeWalk.ForPath(reader, "zoo-animals.txt", n.
				Tree).GetObjectId(0));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEditFanout2_38()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			RevCommit r = tr.Commit().Add(Fanout(2, a.Name), data1).Add(Fanout(2, b.Name), data2
				).Add(".gitignore", string.Empty).Add("zoo-animals.txt", b).Create();
			//
			//
			//
			//
			//
			tr.ParseBody(r);
			NoteMap map = NoteMap.Read(reader, r);
			map.Set(a, data2);
			map.Set(b, null);
			map.Set(data1, b);
			map.Set(data2, null);
			NUnit.Framework.Assert.AreEqual(data2, map.Get(a));
			NUnit.Framework.Assert.AreEqual(b, map.Get(data1));
			NUnit.Framework.Assert.IsFalse(map.Contains(b), "no b");
			NUnit.Framework.Assert.IsFalse(map.Contains(data2), "no data2");
			RevCommit n = CommitNoteMap(map);
			map.Set(a, null);
			map.Set(data1, null);
			NUnit.Framework.Assert.IsFalse(map.Contains(a), "no a");
			NUnit.Framework.Assert.IsFalse(map.Contains(data1), "no data1");
			map = NoteMap.Read(reader, n);
			NUnit.Framework.Assert.AreEqual(data2, map.Get(a));
			NUnit.Framework.Assert.AreEqual(b, map.Get(data1));
			NUnit.Framework.Assert.IsFalse(map.Contains(b), "no b");
			NUnit.Framework.Assert.IsFalse(map.Contains(data2), "no data2");
			NUnit.Framework.Assert.AreEqual(b, TreeWalk.ForPath(reader, "zoo-animals.txt", n.
				Tree).GetObjectId(0));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLeafSplitsWhenFull()
		{
			RevBlob data1 = tr.Blob("data1");
			MutableObjectId idBuf = new MutableObjectId();
			RevCommit r = tr.Commit().Add(data1.Name, data1).Create();
			//
			//
			tr.ParseBody(r);
			NoteMap map = NoteMap.Read(reader, r);
			for (int i = 0; i < 254; i++)
			{
				idBuf.SetByte(Constants.OBJECT_ID_LENGTH - 1, i);
				map.Set(idBuf, data1);
			}
			RevCommit n = CommitNoteMap(map);
			TreeWalk tw = new TreeWalk(reader);
			tw.Reset(n.Tree);
			while (tw.Next())
			{
				NUnit.Framework.Assert.IsFalse(tw.IsSubtree, "no fan-out subtree");
			}
			for (int i_1 = 254; i_1 < 256; i_1++)
			{
				idBuf.SetByte(Constants.OBJECT_ID_LENGTH - 1, i_1);
				map.Set(idBuf, data1);
			}
			idBuf.SetByte(Constants.OBJECT_ID_LENGTH - 2, 1);
			map.Set(idBuf, data1);
			n = CommitNoteMap(map);
			// The 00 bucket is fully split.
			string path = Fanout(38, idBuf.Name);
			tw = TreeWalk.ForPath(reader, path, n.Tree);
			NUnit.Framework.Assert.IsNotNull(tw, "has " + path);
			// The other bucket is not.
			path = Fanout(2, data1.Name);
			tw = TreeWalk.ForPath(reader, path, n.Tree);
			NUnit.Framework.Assert.IsNotNull(tw, "has " + path);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRemoveDeletesTreeFanout2_38()
		{
			RevBlob a = tr.Blob("a");
			RevBlob data1 = tr.Blob("data1");
			RevTree empty = tr.Tree();
			RevCommit r = tr.Commit().Add(Fanout(2, a.Name), data1).Create();
			//
			//
			tr.ParseBody(r);
			NoteMap map = NoteMap.Read(reader, r);
			map.Set(a, null);
			RevCommit n = CommitNoteMap(map);
			NUnit.Framework.Assert.AreEqual(empty, n.Tree, "empty tree");
		}

		public virtual void TestIteratorEmptyMap()
		{
			Iterator<Note> it = NoteMap.NewEmptyMap().Iterator();
			NUnit.Framework.Assert.IsFalse(it.HasNext());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestIteratorFlatTree()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			RevBlob nonNote = tr.Blob("non note");
			RevCommit r = tr.Commit().Add(a.Name, data1).Add(b.Name, data2).Add("nonNote", nonNote
				).Create();
			//
			//
			//
			//
			tr.ParseBody(r);
			Iterator it = NoteMap.Read(reader, r).Iterator();
			NUnit.Framework.Assert.AreEqual(2, Count(it));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestIteratorFanoutTree2_38()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			RevBlob nonNote = tr.Blob("non note");
			RevCommit r = tr.Commit().Add(Fanout(2, a.Name), data1).Add(Fanout(2, b.Name), data2
				).Add("nonNote", nonNote).Create();
			//
			//
			//
			//
			tr.ParseBody(r);
			Iterator it = NoteMap.Read(reader, r).Iterator();
			NUnit.Framework.Assert.AreEqual(2, Count(it));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestIteratorFanoutTree2_2_36()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			RevBlob nonNote = tr.Blob("non note");
			RevCommit r = tr.Commit().Add(Fanout(4, a.Name), data1).Add(Fanout(4, b.Name), data2
				).Add("nonNote", nonNote).Create();
			//
			//
			//
			//
			tr.ParseBody(r);
			Iterator it = NoteMap.Read(reader, r).Iterator();
			NUnit.Framework.Assert.AreEqual(2, Count(it));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestIteratorFullyFannedOut()
		{
			RevBlob a = tr.Blob("a");
			RevBlob b = tr.Blob("b");
			RevBlob data1 = tr.Blob("data1");
			RevBlob data2 = tr.Blob("data2");
			RevBlob nonNote = tr.Blob("non note");
			RevCommit r = tr.Commit().Add(Fanout(38, a.Name), data1).Add(Fanout(38, b.Name), 
				data2).Add("nonNote", nonNote).Create();
			//
			//
			//
			//
			tr.ParseBody(r);
			Iterator it = NoteMap.Read(reader, r).Iterator();
			NUnit.Framework.Assert.AreEqual(2, Count(it));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private RevCommit CommitNoteMap(NoteMap map)
		{
			tr.Tick(600);
			NGit.CommitBuilder builder = new NGit.CommitBuilder();
			builder.TreeId = map.WriteTree(inserter);
			tr.SetAuthorAndCommitter(builder);
			return tr.GetRevWalk().ParseCommit(inserter.Insert(builder));
		}

		private static string Fanout(int prefix, string name)
		{
			StringBuilder r = new StringBuilder();
			int i = 0;
			for (; i < prefix && i < name.Length; i += 2)
			{
				if (i != 0)
				{
					r.Append('/');
				}
				r.Append(name[i + 0]);
				r.Append(name[i + 1]);
			}
			if (i < name.Length)
			{
				if (i != 0)
				{
					r.Append('/');
				}
				r.Append(Sharpen.Runtime.Substring(name, i));
			}
			return r.ToString();
		}

		private static int Count(Iterator it)
		{
			int c = 0;
			while (it.HasNext())
			{
				c++;
				it.Next();
			}
			return c;
		}
	}
}
