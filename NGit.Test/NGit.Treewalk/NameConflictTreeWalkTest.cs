using NGit;
using NGit.Dircache;
using NGit.Treewalk;
using Sharpen;

namespace NGit.Treewalk
{
	public class NameConflictTreeWalkTest : RepositoryTestCase
	{
		private static readonly FileMode TREE = FileMode.TREE;

		private static readonly FileMode SYMLINK = FileMode.SYMLINK;

		private static readonly FileMode MISSING = FileMode.MISSING;

		private static readonly FileMode REGULAR_FILE = FileMode.REGULAR_FILE;

		private static readonly FileMode EXECUTABLE_FILE = FileMode.EXECUTABLE_FILE;

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNoDF_NoGap()
		{
			DirCache tree0 = db.ReadDirCache();
			DirCache tree1 = db.ReadDirCache();
			{
				DirCacheBuilder b0 = tree0.Builder();
				DirCacheBuilder b1 = tree1.Builder();
				b0.Add(MakeEntry("a", REGULAR_FILE));
				b0.Add(MakeEntry("a.b", EXECUTABLE_FILE));
				b1.Add(MakeEntry("a/b", REGULAR_FILE));
				b0.Add(MakeEntry("a0b", SYMLINK));
				b0.Finish();
				b1.Finish();
				NUnit.Framework.Assert.AreEqual(3, tree0.GetEntryCount());
				NUnit.Framework.Assert.AreEqual(1, tree1.GetEntryCount());
			}
			TreeWalk tw = new TreeWalk(db);
			tw.Reset();
			tw.AddTree(new DirCacheIterator(tree0));
			tw.AddTree(new DirCacheIterator(tree1));
			AssertModes("a", REGULAR_FILE, MISSING, tw);
			AssertModes("a.b", EXECUTABLE_FILE, MISSING, tw);
			AssertModes("a", MISSING, TREE, tw);
			tw.EnterSubtree();
			AssertModes("a/b", MISSING, REGULAR_FILE, tw);
			AssertModes("a0b", SYMLINK, MISSING, tw);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDF_NoGap()
		{
			DirCache tree0 = db.ReadDirCache();
			DirCache tree1 = db.ReadDirCache();
			{
				DirCacheBuilder b0 = tree0.Builder();
				DirCacheBuilder b1 = tree1.Builder();
				b0.Add(MakeEntry("a", REGULAR_FILE));
				b0.Add(MakeEntry("a.b", EXECUTABLE_FILE));
				b1.Add(MakeEntry("a/b", REGULAR_FILE));
				b0.Add(MakeEntry("a0b", SYMLINK));
				b0.Finish();
				b1.Finish();
				NUnit.Framework.Assert.AreEqual(3, tree0.GetEntryCount());
				NUnit.Framework.Assert.AreEqual(1, tree1.GetEntryCount());
			}
			NameConflictTreeWalk tw = new NameConflictTreeWalk(db);
			tw.Reset();
			tw.AddTree(new DirCacheIterator(tree0));
			tw.AddTree(new DirCacheIterator(tree1));
			AssertModes("a", REGULAR_FILE, TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsDirectoryFileConflict());
			NUnit.Framework.Assert.IsTrue(tw.IsSubtree);
			tw.EnterSubtree();
			AssertModes("a/b", MISSING, REGULAR_FILE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsDirectoryFileConflict());
			AssertModes("a.b", EXECUTABLE_FILE, MISSING, tw);
			NUnit.Framework.Assert.IsFalse(tw.IsDirectoryFileConflict());
			AssertModes("a0b", SYMLINK, MISSING, tw);
			NUnit.Framework.Assert.IsFalse(tw.IsDirectoryFileConflict());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDF_GapByOne()
		{
			DirCache tree0 = db.ReadDirCache();
			DirCache tree1 = db.ReadDirCache();
			{
				DirCacheBuilder b0 = tree0.Builder();
				DirCacheBuilder b1 = tree1.Builder();
				b0.Add(MakeEntry("a", REGULAR_FILE));
				b0.Add(MakeEntry("a.b", EXECUTABLE_FILE));
				b1.Add(MakeEntry("a.b", EXECUTABLE_FILE));
				b1.Add(MakeEntry("a/b", REGULAR_FILE));
				b0.Add(MakeEntry("a0b", SYMLINK));
				b0.Finish();
				b1.Finish();
				NUnit.Framework.Assert.AreEqual(3, tree0.GetEntryCount());
				NUnit.Framework.Assert.AreEqual(2, tree1.GetEntryCount());
			}
			NameConflictTreeWalk tw = new NameConflictTreeWalk(db);
			tw.Reset();
			tw.AddTree(new DirCacheIterator(tree0));
			tw.AddTree(new DirCacheIterator(tree1));
			AssertModes("a", REGULAR_FILE, TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsSubtree);
			NUnit.Framework.Assert.IsTrue(tw.IsDirectoryFileConflict());
			tw.EnterSubtree();
			AssertModes("a/b", MISSING, REGULAR_FILE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsDirectoryFileConflict());
			AssertModes("a.b", EXECUTABLE_FILE, EXECUTABLE_FILE, tw);
			NUnit.Framework.Assert.IsFalse(tw.IsDirectoryFileConflict());
			AssertModes("a0b", SYMLINK, MISSING, tw);
			NUnit.Framework.Assert.IsFalse(tw.IsDirectoryFileConflict());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDF_SkipsSeenSubtree()
		{
			DirCache tree0 = db.ReadDirCache();
			DirCache tree1 = db.ReadDirCache();
			{
				DirCacheBuilder b0 = tree0.Builder();
				DirCacheBuilder b1 = tree1.Builder();
				b0.Add(MakeEntry("a", REGULAR_FILE));
				b1.Add(MakeEntry("a.b", EXECUTABLE_FILE));
				b1.Add(MakeEntry("a/b", REGULAR_FILE));
				b0.Add(MakeEntry("a0b", SYMLINK));
				b1.Add(MakeEntry("a0b", SYMLINK));
				b0.Finish();
				b1.Finish();
				NUnit.Framework.Assert.AreEqual(2, tree0.GetEntryCount());
				NUnit.Framework.Assert.AreEqual(3, tree1.GetEntryCount());
			}
			NameConflictTreeWalk tw = new NameConflictTreeWalk(db);
			tw.Reset();
			tw.AddTree(new DirCacheIterator(tree0));
			tw.AddTree(new DirCacheIterator(tree1));
			AssertModes("a", REGULAR_FILE, TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsSubtree);
			NUnit.Framework.Assert.IsTrue(tw.IsDirectoryFileConflict());
			tw.EnterSubtree();
			AssertModes("a/b", MISSING, REGULAR_FILE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsDirectoryFileConflict());
			AssertModes("a.b", MISSING, EXECUTABLE_FILE, tw);
			NUnit.Framework.Assert.IsFalse(tw.IsDirectoryFileConflict());
			AssertModes("a0b", SYMLINK, SYMLINK, tw);
			NUnit.Framework.Assert.IsFalse(tw.IsDirectoryFileConflict());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDF_DetectConflict()
		{
			DirCache tree0 = db.ReadDirCache();
			DirCache tree1 = db.ReadDirCache();
			{
				DirCacheBuilder b0 = tree0.Builder();
				DirCacheBuilder b1 = tree1.Builder();
				b0.Add(MakeEntry("0", REGULAR_FILE));
				b0.Add(MakeEntry("a", REGULAR_FILE));
				b1.Add(MakeEntry("0", REGULAR_FILE));
				b1.Add(MakeEntry("a.b", REGULAR_FILE));
				b1.Add(MakeEntry("a/b", REGULAR_FILE));
				b1.Add(MakeEntry("a/c/e", REGULAR_FILE));
				b0.Finish();
				b1.Finish();
				NUnit.Framework.Assert.AreEqual(2, tree0.GetEntryCount());
				NUnit.Framework.Assert.AreEqual(4, tree1.GetEntryCount());
			}
			NameConflictTreeWalk tw = new NameConflictTreeWalk(db);
			tw.Reset();
			tw.AddTree(new DirCacheIterator(tree0));
			tw.AddTree(new DirCacheIterator(tree1));
			AssertModes("0", REGULAR_FILE, REGULAR_FILE, tw);
			NUnit.Framework.Assert.IsFalse(tw.IsDirectoryFileConflict());
			AssertModes("a", REGULAR_FILE, TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsSubtree);
			NUnit.Framework.Assert.IsTrue(tw.IsDirectoryFileConflict());
			tw.EnterSubtree();
			AssertModes("a/b", MISSING, REGULAR_FILE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsDirectoryFileConflict());
			AssertModes("a/c", MISSING, TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsDirectoryFileConflict());
			tw.EnterSubtree();
			AssertModes("a/c/e", MISSING, REGULAR_FILE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsDirectoryFileConflict());
			AssertModes("a.b", MISSING, REGULAR_FILE, tw);
			NUnit.Framework.Assert.IsFalse(tw.IsDirectoryFileConflict());
		}

		/// <exception cref="System.Exception"></exception>
		private DirCacheEntry MakeEntry(string path, FileMode mode)
		{
			DirCacheEntry ent = new DirCacheEntry(path);
			ent.SetFileMode(mode);
			ent.SetObjectId(new ObjectInserter.Formatter().IdFor(Constants.OBJ_BLOB, Constants
				.Encode(path)));
			return ent;
		}

		/// <exception cref="System.Exception"></exception>
		private static void AssertModes(string path, FileMode mode0, FileMode mode1, TreeWalk
			 tw)
		{
			NUnit.Framework.Assert.IsTrue("has " + path, tw.Next());
			NUnit.Framework.Assert.AreEqual(path, tw.PathString);
			NUnit.Framework.Assert.AreEqual(mode0, tw.GetFileMode(0));
			NUnit.Framework.Assert.AreEqual(mode1, tw.GetFileMode(1));
		}
	}
}
