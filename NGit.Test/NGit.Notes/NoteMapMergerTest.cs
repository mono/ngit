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
using NGit.Junit;
using NGit.Merge;
using NGit.Notes;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Notes
{
	[NUnit.Framework.TestFixture]
	public class NoteMapMergerTest : RepositoryTestCase
	{
		private TestRepository<Repository> tr;

		private ObjectReader reader;

		private ObjectInserter inserter;

		private NoteMap noRoot;

		private NoteMap empty;

		private NoteMap map_a;

		private NoteMap map_a_b;

		private RevBlob noteAId;

		private string noteAContent;

		private RevBlob noteABlob;

		private RevBlob noteBId;

		private string noteBContent;

		private RevBlob noteBBlob;

		private RevCommit sampleTree_a;

		private RevCommit sampleTree_a_b;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			tr = new TestRepository<Repository>(db);
			reader = db.NewObjectReader();
			inserter = db.NewObjectInserter();
			noRoot = NoteMap.NewMap(null, reader);
			empty = NoteMap.NewEmptyMap();
			noteAId = tr.Blob("a");
			noteAContent = "noteAContent";
			noteABlob = tr.Blob(noteAContent);
			sampleTree_a = tr.Commit().Add(noteAId.Name, noteABlob).Create();
			tr.ParseBody(sampleTree_a);
			map_a = NoteMap.Read(reader, sampleTree_a);
			noteBId = tr.Blob("b");
			noteBContent = "noteBContent";
			noteBBlob = tr.Blob(noteBContent);
			sampleTree_a_b = tr.Commit().Add(noteAId.Name, noteABlob).Add(noteBId.Name, noteBBlob
				).Create();
			tr.ParseBody(sampleTree_a_b);
			map_a_b = NoteMap.Read(reader, sampleTree_a_b);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.TearDown]
		public override void TearDown()
		{
			reader.Release();
			inserter.Release();
			base.TearDown();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNoChange()
		{
			NoteMapMerger merger = new NoteMapMerger(db, null, null);
			NoteMap result;
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(noRoot, noRoot, noRoot
				)));
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(empty, empty, empty)));
			result = merger.Merge(map_a, map_a, map_a);
			NUnit.Framework.Assert.AreEqual(1, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOursEqualsTheirs()
		{
			NoteMapMerger merger = new NoteMapMerger(db, null, null);
			NoteMap result;
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(empty, noRoot, noRoot)
				));
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(map_a, noRoot, noRoot)
				));
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(noRoot, empty, empty))
				);
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(map_a, empty, empty)));
			result = merger.Merge(noRoot, map_a, map_a);
			NUnit.Framework.Assert.AreEqual(1, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			result = merger.Merge(empty, map_a, map_a);
			NUnit.Framework.Assert.AreEqual(1, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			result = merger.Merge(map_a_b, map_a, map_a);
			NUnit.Framework.Assert.AreEqual(1, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			result = merger.Merge(map_a, map_a_b, map_a_b);
			NUnit.Framework.Assert.AreEqual(2, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			NUnit.Framework.Assert.AreEqual(noteBBlob, result.Get(noteBId));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBaseEqualsOurs()
		{
			NoteMapMerger merger = new NoteMapMerger(db, null, null);
			NoteMap result;
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(noRoot, noRoot, empty)
				));
			result = merger.Merge(noRoot, noRoot, map_a);
			NUnit.Framework.Assert.AreEqual(1, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(empty, empty, noRoot))
				);
			result = merger.Merge(empty, empty, map_a);
			NUnit.Framework.Assert.AreEqual(1, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(map_a, map_a, noRoot))
				);
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(map_a, map_a, empty)));
			result = merger.Merge(map_a, map_a, map_a_b);
			NUnit.Framework.Assert.AreEqual(2, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			NUnit.Framework.Assert.AreEqual(noteBBlob, result.Get(noteBId));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBaseEqualsTheirs()
		{
			NoteMapMerger merger = new NoteMapMerger(db, null, null);
			NoteMap result;
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(noRoot, empty, noRoot)
				));
			result = merger.Merge(noRoot, map_a, noRoot);
			NUnit.Framework.Assert.AreEqual(1, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(empty, noRoot, empty))
				);
			result = merger.Merge(empty, map_a, empty);
			NUnit.Framework.Assert.AreEqual(1, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(map_a, noRoot, map_a))
				);
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(map_a, empty, map_a)));
			result = merger.Merge(map_a, map_a_b, map_a);
			NUnit.Framework.Assert.AreEqual(2, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			NUnit.Framework.Assert.AreEqual(noteBBlob, result.Get(noteBId));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddDifferentNotes()
		{
			NoteMapMerger merger = new NoteMapMerger(db, null, null);
			NoteMap result;
			NoteMap map_a_c = NoteMap.Read(reader, sampleTree_a);
			RevBlob noteCId = tr.Blob("c");
			RevBlob noteCBlob = tr.Blob("noteCContent");
			map_a_c.Set(noteCId, noteCBlob);
			map_a_c.WriteTree(inserter);
			result = merger.Merge(map_a, map_a_b, map_a_c);
			NUnit.Framework.Assert.AreEqual(3, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			NUnit.Framework.Assert.AreEqual(noteBBlob, result.Get(noteBId));
			NUnit.Framework.Assert.AreEqual(noteCBlob, result.Get(noteCId));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddSameNoteDifferentContent()
		{
			NoteMapMerger merger = new NoteMapMerger(db, new DefaultNoteMerger(), null);
			NoteMap result;
			NoteMap map_a_b1 = NoteMap.Read(reader, sampleTree_a);
			string noteBContent1 = noteBContent + "change";
			RevBlob noteBBlob1 = tr.Blob(noteBContent1);
			map_a_b1.Set(noteBId, noteBBlob1);
			map_a_b1.WriteTree(inserter);
			result = merger.Merge(map_a, map_a_b, map_a_b1);
			NUnit.Framework.Assert.AreEqual(2, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			NUnit.Framework.Assert.AreEqual(tr.Blob(noteBContent + noteBContent1), result.Get
				(noteBId));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEditSameNoteDifferentContent()
		{
			NoteMapMerger merger = new NoteMapMerger(db, new DefaultNoteMerger(), null);
			NoteMap result;
			NoteMap map_a1 = NoteMap.Read(reader, sampleTree_a);
			string noteAContent1 = noteAContent + "change1";
			RevBlob noteABlob1 = tr.Blob(noteAContent1);
			map_a1.Set(noteAId, noteABlob1);
			map_a1.WriteTree(inserter);
			NoteMap map_a2 = NoteMap.Read(reader, sampleTree_a);
			string noteAContent2 = noteAContent + "change2";
			RevBlob noteABlob2 = tr.Blob(noteAContent2);
			map_a2.Set(noteAId, noteABlob2);
			map_a2.WriteTree(inserter);
			result = merger.Merge(map_a, map_a1, map_a2);
			NUnit.Framework.Assert.AreEqual(1, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(tr.Blob(noteAContent1 + noteAContent2), result.Get
				(noteAId));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEditDifferentNotes()
		{
			NoteMapMerger merger = new NoteMapMerger(db, null, null);
			NoteMap result;
			NoteMap map_a1_b = NoteMap.Read(reader, sampleTree_a_b);
			string noteAContent1 = noteAContent + "change";
			RevBlob noteABlob1 = tr.Blob(noteAContent1);
			map_a1_b.Set(noteAId, noteABlob1);
			map_a1_b.WriteTree(inserter);
			NoteMap map_a_b1 = NoteMap.Read(reader, sampleTree_a_b);
			string noteBContent1 = noteBContent + "change";
			RevBlob noteBBlob1 = tr.Blob(noteBContent1);
			map_a_b1.Set(noteBId, noteBBlob1);
			map_a_b1.WriteTree(inserter);
			result = merger.Merge(map_a_b, map_a1_b, map_a_b1);
			NUnit.Framework.Assert.AreEqual(2, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob1, result.Get(noteAId));
			NUnit.Framework.Assert.AreEqual(noteBBlob1, result.Get(noteBId));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteDifferentNotes()
		{
			NoteMapMerger merger = new NoteMapMerger(db, null, null);
			NoteMap map_b = NoteMap.Read(reader, sampleTree_a_b);
			map_b.Set(noteAId, null);
			// delete note a
			map_b.WriteTree(inserter);
			NUnit.Framework.Assert.AreEqual(0, CountNotes(merger.Merge(map_a_b, map_a, map_b)
				));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEditDeleteConflict()
		{
			NoteMapMerger merger = new NoteMapMerger(db, new DefaultNoteMerger(), null);
			NoteMap result;
			NoteMap map_a_b1 = NoteMap.Read(reader, sampleTree_a_b);
			string noteBContent1 = noteBContent + "change";
			RevBlob noteBBlob1 = tr.Blob(noteBContent1);
			map_a_b1.Set(noteBId, noteBBlob1);
			map_a_b1.WriteTree(inserter);
			result = merger.Merge(map_a_b, map_a_b1, map_a);
			NUnit.Framework.Assert.AreEqual(2, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(noteABlob, result.Get(noteAId));
			NUnit.Framework.Assert.AreEqual(noteBBlob1, result.Get(noteBId));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLargeTreesWithoutConflict()
		{
			NoteMapMerger merger = new NoteMapMerger(db, null, null);
			NoteMap map1 = CreateLargeNoteMap("note_1_", "content_1_", 300, 0);
			NoteMap map2 = CreateLargeNoteMap("note_2_", "content_2_", 300, 0);
			NoteMap result = merger.Merge(empty, map1, map2);
			NUnit.Framework.Assert.AreEqual(600, CountNotes(result));
			// check a few random notes
			NUnit.Framework.Assert.AreEqual(tr.Blob("content_1_59"), result.Get(tr.Blob("note_1_59"
				)));
			NUnit.Framework.Assert.AreEqual(tr.Blob("content_2_10"), result.Get(tr.Blob("note_2_10"
				)));
			NUnit.Framework.Assert.AreEqual(tr.Blob("content_2_99"), result.Get(tr.Blob("note_2_99"
				)));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLargeTreesWithConflict()
		{
			NoteMapMerger merger = new NoteMapMerger(db, new DefaultNoteMerger(), null);
			NoteMap largeTree1 = CreateLargeNoteMap("note_1_", "content_1_", 300, 0);
			NoteMap largeTree2 = CreateLargeNoteMap("note_1_", "content_2_", 300, 0);
			NoteMap result = merger.Merge(empty, largeTree1, largeTree2);
			NUnit.Framework.Assert.AreEqual(300, CountNotes(result));
			// check a few random notes
			NUnit.Framework.Assert.AreEqual(tr.Blob("content_1_59content_2_59"), result.Get(tr
				.Blob("note_1_59")));
			NUnit.Framework.Assert.AreEqual(tr.Blob("content_1_10content_2_10"), result.Get(tr
				.Blob("note_1_10")));
			NUnit.Framework.Assert.AreEqual(tr.Blob("content_1_99content_2_99"), result.Get(tr
				.Blob("note_1_99")));
		}

		/// <exception cref="System.Exception"></exception>
		private NoteMap CreateLargeNoteMap(string noteNamePrefix, string noteContentPrefix
			, int notesCount, int firstIndex)
		{
			NoteMap result = NoteMap.NewEmptyMap();
			for (int i = 0; i < notesCount; i++)
			{
				result.Set(tr.Blob(noteNamePrefix + (firstIndex + i)), tr.Blob(noteContentPrefix 
					+ (firstIndex + i)));
			}
			result.WriteTree(inserter);
			return result;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFanoutAndLeafWithoutConflict()
		{
			NoteMapMerger merger = new NoteMapMerger(db, null, null);
			NoteMap largeTree = CreateLargeNoteMap("note_1_", "content_1_", 300, 0);
			NoteMap result = merger.Merge(map_a, map_a_b, largeTree);
			NUnit.Framework.Assert.AreEqual(301, CountNotes(result));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFanoutAndLeafWitConflict()
		{
			NoteMapMerger merger = new NoteMapMerger(db, new DefaultNoteMerger(), null);
			NoteMap largeTree_b1 = CreateLargeNoteMap("note_1_", "content_1_", 300, 0);
			string noteBContent1 = noteBContent + "change";
			largeTree_b1.Set(noteBId, tr.Blob(noteBContent1));
			largeTree_b1.WriteTree(inserter);
			NoteMap result = merger.Merge(map_a, map_a_b, largeTree_b1);
			NUnit.Framework.Assert.AreEqual(301, CountNotes(result));
			NUnit.Framework.Assert.AreEqual(tr.Blob(noteBContent + noteBContent1), result.Get
				(noteBId));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCollapseFanoutAfterMerge()
		{
			NoteMapMerger merger = new NoteMapMerger(db, null, null);
			NoteMap largeTree = CreateLargeNoteMap("note_", "content_", 257, 0);
			NUnit.Framework.Assert.IsTrue(largeTree.GetRoot() is FanoutBucket);
			NoteMap deleteFirstHundredNotes = CreateLargeNoteMap("note_", "content_", 157, 100
				);
			NoteMap deleteLastHundredNotes = CreateLargeNoteMap("note_", "content_", 157, 0);
			NoteMap result = merger.Merge(largeTree, deleteFirstHundredNotes, deleteLastHundredNotes
				);
			NUnit.Framework.Assert.AreEqual(57, CountNotes(result));
			NUnit.Framework.Assert.IsTrue(result.GetRoot() is LeafBucket);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNonNotesWithoutNonNoteConflict()
		{
			NoteMapMerger merger = new NoteMapMerger(db, null, MergeStrategy.RESOLVE);
			RevCommit treeWithNonNotes = tr.Commit().Add(noteAId.Name, noteABlob).Add("a.txt"
				, tr.Blob("content of a.txt")).Create();
			// this is a note
			// this is a non-note
			tr.ParseBody(treeWithNonNotes);
			NoteMap @base = NoteMap.Read(reader, treeWithNonNotes);
			treeWithNonNotes = tr.Commit().Add(noteAId.Name, noteABlob).Add("a.txt", tr.Blob(
				"content of a.txt")).Add("b.txt", tr.Blob("content of b.txt")).Create();
			tr.ParseBody(treeWithNonNotes);
			NoteMap ours = NoteMap.Read(reader, treeWithNonNotes);
			treeWithNonNotes = tr.Commit().Add(noteAId.Name, noteABlob).Add("a.txt", tr.Blob(
				"content of a.txt")).Add("c.txt", tr.Blob("content of c.txt")).Create();
			tr.ParseBody(treeWithNonNotes);
			NoteMap theirs = NoteMap.Read(reader, treeWithNonNotes);
			NoteMap result = merger.Merge(@base, ours, theirs);
			NUnit.Framework.Assert.AreEqual(3, CountNonNotes(result));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNonNotesWithNonNoteConflict()
		{
			NoteMapMerger merger = new NoteMapMerger(db, null, MergeStrategy.RESOLVE);
			RevCommit treeWithNonNotes = tr.Commit().Add(noteAId.Name, noteABlob).Add("a.txt"
				, tr.Blob("content of a.txt")).Create();
			// this is a note
			// this is a non-note
			tr.ParseBody(treeWithNonNotes);
			NoteMap @base = NoteMap.Read(reader, treeWithNonNotes);
			treeWithNonNotes = tr.Commit().Add(noteAId.Name, noteABlob).Add("a.txt", tr.Blob(
				"change 1")).Create();
			tr.ParseBody(treeWithNonNotes);
			NoteMap ours = NoteMap.Read(reader, treeWithNonNotes);
			treeWithNonNotes = tr.Commit().Add(noteAId.Name, noteABlob).Add("a.txt", tr.Blob(
				"change 2")).Create();
			tr.ParseBody(treeWithNonNotes);
			NoteMap theirs = NoteMap.Read(reader, treeWithNonNotes);
			try
			{
				merger.Merge(@base, ours, theirs);
				NUnit.Framework.Assert.Fail("NotesMergeConflictException was expected");
			}
			catch (NotesMergeConflictException)
			{
			}
		}

		// expected
		private static int CountNotes(NoteMap map)
		{
			int c = 0;
			Iterator<Note> it = map.Iterator();
			while (it.HasNext())
			{
				it.Next();
				c++;
			}
			return c;
		}

		private static int CountNonNotes(NoteMap map)
		{
			int c = 0;
			NonNoteEntry nonNotes = map.GetRoot().nonNotes;
			while (nonNotes != null)
			{
				c++;
				nonNotes = nonNotes.next;
			}
			return c;
		}
	}
}
