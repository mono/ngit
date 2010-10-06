using System;
using System.Collections.Generic;
using NGit;
using NGit.Diff;
using NGit.Junit;
using Sharpen;

namespace NGit.Diff
{
	public class RenameDetectorTest : RepositoryTestCase
	{
		private static readonly string PATH_A = "src/A";

		private static readonly string PATH_B = "src/B";

		private static readonly string PATH_H = "src/H";

		private static readonly string PATH_Q = "src/Q";

		private RenameDetector rd;

		private TestRepository testDb;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			testDb = new TestRepository(db);
			rd = new RenameDetector(db);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestExactRename_OneRename()
		{
			ObjectId foo = Blob("foo");
			DiffEntry a = DiffEntry.Add(PATH_A, foo);
			DiffEntry b = DiffEntry.Delete(PATH_Q, foo);
			rd.Add(a);
			rd.Add(b);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(1, entries.Count);
			AssertRename(b, a, 100, entries[0]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestExactRename_DifferentObjects()
		{
			ObjectId foo = Blob("foo");
			ObjectId bar = Blob("bar");
			DiffEntry a = DiffEntry.Add(PATH_A, foo);
			DiffEntry h = DiffEntry.Add(PATH_H, foo);
			DiffEntry q = DiffEntry.Delete(PATH_Q, bar);
			rd.Add(a);
			rd.Add(h);
			rd.Add(q);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(3, entries.Count);
			NUnit.Framework.Assert.AreSame(a, entries[0]);
			NUnit.Framework.Assert.AreSame(h, entries[1]);
			NUnit.Framework.Assert.AreSame(q, entries[2]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestExactRename_OneRenameOneModify()
		{
			ObjectId foo = Blob("foo");
			ObjectId bar = Blob("bar");
			DiffEntry a = DiffEntry.Add(PATH_A, foo);
			DiffEntry b = DiffEntry.Delete(PATH_Q, foo);
			DiffEntry c = DiffEntry.Modify(PATH_H);
			c.newId = c.oldId = AbbreviatedObjectId.FromObjectId(bar);
			rd.Add(a);
			rd.Add(b);
			rd.Add(c);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			AssertRename(b, a, 100, entries[0]);
			NUnit.Framework.Assert.AreSame(c, entries[1]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestExactRename_ManyRenames()
		{
			ObjectId foo = Blob("foo");
			ObjectId bar = Blob("bar");
			DiffEntry a = DiffEntry.Add(PATH_A, foo);
			DiffEntry b = DiffEntry.Delete(PATH_Q, foo);
			DiffEntry c = DiffEntry.Add(PATH_H, bar);
			DiffEntry d = DiffEntry.Delete(PATH_B, bar);
			rd.Add(a);
			rd.Add(b);
			rd.Add(c);
			rd.Add(d);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			AssertRename(b, a, 100, entries[0]);
			AssertRename(d, c, 100, entries[1]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestExactRename_MultipleIdenticalDeletes()
		{
			ObjectId foo = Blob("foo");
			DiffEntry a = DiffEntry.Delete(PATH_A, foo);
			DiffEntry b = DiffEntry.Delete(PATH_B, foo);
			DiffEntry c = DiffEntry.Delete(PATH_H, foo);
			DiffEntry d = DiffEntry.Add(PATH_Q, foo);
			rd.Add(a);
			rd.Add(b);
			rd.Add(c);
			rd.Add(d);
			// Pairs the add with the first delete added
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(3, entries.Count);
			NUnit.Framework.Assert.AreEqual(b, entries[0]);
			NUnit.Framework.Assert.AreEqual(c, entries[1]);
			AssertRename(a, d, 100, entries[2]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestExactRename_PathBreaksTie()
		{
			ObjectId foo = Blob("foo");
			DiffEntry a = DiffEntry.Add("src/com/foo/a.java", foo);
			DiffEntry b = DiffEntry.Delete("src/com/foo/b.java", foo);
			DiffEntry c = DiffEntry.Add("c.txt", foo);
			DiffEntry d = DiffEntry.Delete("d.txt", foo);
			DiffEntry e = DiffEntry.Add("the_e_file.txt", foo);
			// Add out of order to avoid first-match succeeding
			rd.Add(a);
			rd.Add(d);
			rd.Add(e);
			rd.Add(b);
			rd.Add(c);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(3, entries.Count);
			AssertRename(d, c, 100, entries[0]);
			AssertRename(b, a, 100, entries[1]);
			AssertCopy(d, e, 100, entries[2]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestExactRename_OneDeleteManyAdds()
		{
			ObjectId foo = Blob("foo");
			DiffEntry a = DiffEntry.Add("src/com/foo/a.java", foo);
			DiffEntry b = DiffEntry.Add("src/com/foo/b.java", foo);
			DiffEntry c = DiffEntry.Add("c.txt", foo);
			DiffEntry d = DiffEntry.Delete("d.txt", foo);
			rd.Add(a);
			rd.Add(b);
			rd.Add(c);
			rd.Add(d);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(3, entries.Count);
			AssertRename(d, c, 100, entries[0]);
			AssertCopy(d, a, 100, entries[1]);
			AssertCopy(d, b, 100, entries[2]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInexactRename_OnePair()
		{
			ObjectId aId = Blob("foo\nbar\nbaz\nblarg\n");
			ObjectId bId = Blob("foo\nbar\nbaz\nblah\n");
			DiffEntry a = DiffEntry.Add(PATH_A, aId);
			DiffEntry b = DiffEntry.Delete(PATH_Q, bId);
			rd.Add(a);
			rd.Add(b);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(1, entries.Count);
			AssertRename(b, a, 66, entries[0]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInexactRename_OneRenameTwoUnrelatedFiles()
		{
			ObjectId aId = Blob("foo\nbar\nbaz\nblarg\n");
			ObjectId bId = Blob("foo\nbar\nbaz\nblah\n");
			DiffEntry a = DiffEntry.Add(PATH_A, aId);
			DiffEntry b = DiffEntry.Delete(PATH_Q, bId);
			ObjectId cId = Blob("some\nsort\nof\ntext\n");
			ObjectId dId = Blob("completely\nunrelated\ntext\n");
			DiffEntry c = DiffEntry.Add(PATH_B, cId);
			DiffEntry d = DiffEntry.Delete(PATH_H, dId);
			rd.Add(a);
			rd.Add(b);
			rd.Add(c);
			rd.Add(d);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(3, entries.Count);
			AssertRename(b, a, 66, entries[0]);
			NUnit.Framework.Assert.AreSame(c, entries[1]);
			NUnit.Framework.Assert.AreSame(d, entries[2]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInexactRename_LastByteDifferent()
		{
			ObjectId aId = Blob("foo\nbar\na");
			ObjectId bId = Blob("foo\nbar\nb");
			DiffEntry a = DiffEntry.Add(PATH_A, aId);
			DiffEntry b = DiffEntry.Delete(PATH_Q, bId);
			rd.Add(a);
			rd.Add(b);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(1, entries.Count);
			AssertRename(b, a, 88, entries[0]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInexactRename_NewlinesOnly()
		{
			ObjectId aId = Blob("\n\n\n");
			ObjectId bId = Blob("\n\n\n\n");
			DiffEntry a = DiffEntry.Add(PATH_A, aId);
			DiffEntry b = DiffEntry.Delete(PATH_Q, bId);
			rd.Add(a);
			rd.Add(b);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(1, entries.Count);
			AssertRename(b, a, 74, entries[0]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInexactRename_SameContentMultipleTimes()
		{
			ObjectId aId = Blob("a\na\na\na\n");
			ObjectId bId = Blob("a\na\na\n");
			DiffEntry a = DiffEntry.Add(PATH_A, aId);
			DiffEntry b = DiffEntry.Delete(PATH_Q, bId);
			rd.Add(a);
			rd.Add(b);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(1, entries.Count);
			AssertRename(b, a, 74, entries[0]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInexactRenames_OnePair2()
		{
			ObjectId aId = Blob("ab\nab\nab\nac\nad\nae\n");
			ObjectId bId = Blob("ac\nab\nab\nab\naa\na0\na1\n");
			DiffEntry a = DiffEntry.Add(PATH_A, aId);
			DiffEntry b = DiffEntry.Delete(PATH_Q, bId);
			rd.Add(a);
			rd.Add(b);
			rd.SetRenameScore(50);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(1, entries.Count);
			AssertRename(b, a, 57, entries[0]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNoRenames_SingleByteFiles()
		{
			ObjectId aId = Blob("a");
			ObjectId bId = Blob("b");
			DiffEntry a = DiffEntry.Add(PATH_A, aId);
			DiffEntry b = DiffEntry.Delete(PATH_Q, bId);
			rd.Add(a);
			rd.Add(b);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			NUnit.Framework.Assert.AreSame(a, entries[0]);
			NUnit.Framework.Assert.AreSame(b, entries[1]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNoRenames_EmptyFile1()
		{
			ObjectId aId = Blob(string.Empty);
			DiffEntry a = DiffEntry.Add(PATH_A, aId);
			rd.Add(a);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(1, entries.Count);
			NUnit.Framework.Assert.AreSame(a, entries[0]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNoRenames_EmptyFile2()
		{
			ObjectId aId = Blob(string.Empty);
			ObjectId bId = Blob("blah");
			DiffEntry a = DiffEntry.Add(PATH_A, aId);
			DiffEntry b = DiffEntry.Delete(PATH_Q, bId);
			rd.Add(a);
			rd.Add(b);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			NUnit.Framework.Assert.AreSame(a, entries[0]);
			NUnit.Framework.Assert.AreSame(b, entries[1]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNoRenames_SymlinkAndFile()
		{
			ObjectId aId = Blob("src/dest");
			DiffEntry a = DiffEntry.Add(PATH_A, aId);
			DiffEntry b = DiffEntry.Delete(PATH_Q, aId);
			b.oldMode = FileMode.SYMLINK;
			rd.Add(a);
			rd.Add(b);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			NUnit.Framework.Assert.AreSame(a, entries[0]);
			NUnit.Framework.Assert.AreSame(b, entries[1]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNoRenames_GitlinkAndFile()
		{
			ObjectId aId = Blob("src/dest");
			DiffEntry a = DiffEntry.Add(PATH_A, aId);
			DiffEntry b = DiffEntry.Delete(PATH_Q, aId);
			b.oldMode = FileMode.GITLINK;
			rd.Add(a);
			rd.Add(b);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			NUnit.Framework.Assert.AreSame(a, entries[0]);
			NUnit.Framework.Assert.AreSame(b, entries[1]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNoRenames_SymlinkAndFileSamePath()
		{
			ObjectId aId = Blob("src/dest");
			DiffEntry a = DiffEntry.Delete(PATH_A, aId);
			DiffEntry b = DiffEntry.Add(PATH_A, aId);
			a.oldMode = FileMode.SYMLINK;
			rd.Add(a);
			rd.Add(b);
			// Deletes should be first
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			NUnit.Framework.Assert.AreSame(a, entries[0]);
			NUnit.Framework.Assert.AreSame(b, entries[1]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBreakModify_BreakAll()
		{
			ObjectId aId = Blob("foo");
			ObjectId bId = Blob("bar");
			DiffEntry m = DiffEntry.Modify(PATH_A);
			m.oldId = AbbreviatedObjectId.FromObjectId(aId);
			m.newId = AbbreviatedObjectId.FromObjectId(bId);
			DiffEntry a = DiffEntry.Add(PATH_B, aId);
			rd.Add(a);
			rd.Add(m);
			rd.SetBreakScore(101);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			AssertAdd(PATH_A, bId, FileMode.REGULAR_FILE, entries[0]);
			AssertRename(DiffEntry.BreakModify(m)[0], a, 100, entries[1]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBreakModify_BreakNone()
		{
			ObjectId aId = Blob("foo");
			ObjectId bId = Blob("bar");
			DiffEntry m = DiffEntry.Modify(PATH_A);
			m.oldId = AbbreviatedObjectId.FromObjectId(aId);
			m.newId = AbbreviatedObjectId.FromObjectId(bId);
			DiffEntry a = DiffEntry.Add(PATH_B, aId);
			rd.Add(a);
			rd.Add(m);
			rd.SetBreakScore(-1);
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			NUnit.Framework.Assert.AreSame(m, entries[0]);
			NUnit.Framework.Assert.AreSame(a, entries[1]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBreakModify_BreakBelowScore()
		{
			ObjectId aId = Blob("foo");
			ObjectId bId = Blob("bar");
			DiffEntry m = DiffEntry.Modify(PATH_A);
			m.oldId = AbbreviatedObjectId.FromObjectId(aId);
			m.newId = AbbreviatedObjectId.FromObjectId(bId);
			DiffEntry a = DiffEntry.Add(PATH_B, aId);
			rd.Add(a);
			rd.Add(m);
			rd.SetBreakScore(20);
			// Should break the modify
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			AssertAdd(PATH_A, bId, FileMode.REGULAR_FILE, entries[0]);
			AssertRename(DiffEntry.BreakModify(m)[0], a, 100, entries[1]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBreakModify_DontBreakAboveScore()
		{
			ObjectId aId = Blob("blah\nblah\nfoo");
			ObjectId bId = Blob("blah\nblah\nbar");
			DiffEntry m = DiffEntry.Modify(PATH_A);
			m.oldId = AbbreviatedObjectId.FromObjectId(aId);
			m.newId = AbbreviatedObjectId.FromObjectId(bId);
			DiffEntry a = DiffEntry.Add(PATH_B, aId);
			rd.Add(a);
			rd.Add(m);
			rd.SetBreakScore(20);
			// Should not break the modify
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(2, entries.Count);
			NUnit.Framework.Assert.AreSame(m, entries[0]);
			NUnit.Framework.Assert.AreSame(a, entries[1]);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBreakModify_RejoinIfUnpaired()
		{
			ObjectId aId = Blob("foo");
			ObjectId bId = Blob("bar");
			DiffEntry m = DiffEntry.Modify(PATH_A);
			m.oldId = AbbreviatedObjectId.FromObjectId(aId);
			m.newId = AbbreviatedObjectId.FromObjectId(bId);
			rd.Add(m);
			rd.SetBreakScore(101);
			// Ensure m is broken apart
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(1, entries.Count);
			DiffEntry modify = entries[0];
			NUnit.Framework.Assert.AreEqual(m.oldPath, modify.oldPath);
			NUnit.Framework.Assert.AreEqual(m.oldId, modify.oldId);
			NUnit.Framework.Assert.AreEqual(m.oldMode, modify.oldMode);
			NUnit.Framework.Assert.AreEqual(m.newPath, modify.newPath);
			NUnit.Framework.Assert.AreEqual(m.newId, modify.newId);
			NUnit.Framework.Assert.AreEqual(m.newMode, modify.newMode);
			NUnit.Framework.Assert.AreEqual(m.changeType, modify.changeType);
			NUnit.Framework.Assert.AreEqual(0, modify.score);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSetRenameScore_IllegalArgs()
		{
			try
			{
				rd.SetRenameScore(-1);
				NUnit.Framework.Assert.Fail();
			}
			catch (ArgumentException)
			{
			}
			// pass
			try
			{
				rd.SetRenameScore(101);
				NUnit.Framework.Assert.Fail();
			}
			catch (ArgumentException)
			{
			}
		}

		// pass
		/// <exception cref="System.Exception"></exception>
		public virtual void TestRenameLimit()
		{
			ObjectId aId = Blob("foo\nbar\nbaz\nblarg\n");
			ObjectId bId = Blob("foo\nbar\nbaz\nblah\n");
			DiffEntry a = DiffEntry.Add(PATH_A, aId);
			DiffEntry b = DiffEntry.Delete(PATH_B, bId);
			ObjectId cId = Blob("a\nb\nc\nd\n");
			ObjectId dId = Blob("a\nb\nc\n");
			DiffEntry c = DiffEntry.Add(PATH_H, cId);
			DiffEntry d = DiffEntry.Delete(PATH_Q, dId);
			rd.Add(a);
			rd.Add(b);
			rd.Add(c);
			rd.Add(d);
			rd.SetRenameLimit(1);
			NUnit.Framework.Assert.IsTrue(rd.IsOverRenameLimit());
			IList<DiffEntry> entries = rd.Compute();
			NUnit.Framework.Assert.AreEqual(4, entries.Count);
			NUnit.Framework.Assert.AreSame(a, entries[0]);
			NUnit.Framework.Assert.AreSame(b, entries[1]);
			NUnit.Framework.Assert.AreSame(c, entries[2]);
			NUnit.Framework.Assert.AreSame(d, entries[3]);
		}

		/// <exception cref="System.Exception"></exception>
		private ObjectId Blob(string content)
		{
			return testDb.Blob(content).Copy();
		}

		private static void AssertRename(DiffEntry o, DiffEntry n, int score, DiffEntry rename
			)
		{
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.RENAME, rename.GetChangeType
				());
			NUnit.Framework.Assert.AreEqual(o.GetOldPath(), rename.GetOldPath());
			NUnit.Framework.Assert.AreEqual(n.GetNewPath(), rename.GetNewPath());
			NUnit.Framework.Assert.AreEqual(o.GetOldMode(), rename.GetOldMode());
			NUnit.Framework.Assert.AreEqual(n.GetNewMode(), rename.GetNewMode());
			NUnit.Framework.Assert.AreEqual(o.GetOldId(), rename.GetOldId());
			NUnit.Framework.Assert.AreEqual(n.GetNewId(), rename.GetNewId());
			NUnit.Framework.Assert.AreEqual(score, rename.GetScore());
		}

		private static void AssertCopy(DiffEntry o, DiffEntry n, int score, DiffEntry copy
			)
		{
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.COPY, copy.GetChangeType());
			NUnit.Framework.Assert.AreEqual(o.GetOldPath(), copy.GetOldPath());
			NUnit.Framework.Assert.AreEqual(n.GetNewPath(), copy.GetNewPath());
			NUnit.Framework.Assert.AreEqual(o.GetOldMode(), copy.GetOldMode());
			NUnit.Framework.Assert.AreEqual(n.GetNewMode(), copy.GetNewMode());
			NUnit.Framework.Assert.AreEqual(o.GetOldId(), copy.GetOldId());
			NUnit.Framework.Assert.AreEqual(n.GetNewId(), copy.GetNewId());
			NUnit.Framework.Assert.AreEqual(score, copy.GetScore());
		}

		private static void AssertAdd(string newName, ObjectId newId, FileMode newMode, DiffEntry
			 add)
		{
			NUnit.Framework.Assert.AreEqual(DiffEntry.DEV_NULL, add.oldPath);
			NUnit.Framework.Assert.AreEqual(DiffEntry.A_ZERO, add.oldId);
			NUnit.Framework.Assert.AreEqual(FileMode.MISSING, add.oldMode);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.ADD, add.changeType);
			NUnit.Framework.Assert.AreEqual(newName, add.newPath);
			NUnit.Framework.Assert.AreEqual(AbbreviatedObjectId.FromObjectId(newId), add.newId
				);
			NUnit.Framework.Assert.AreEqual(newMode, add.newMode);
		}
	}
}
