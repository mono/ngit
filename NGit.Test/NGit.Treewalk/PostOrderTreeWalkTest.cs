using NGit;
using NGit.Dircache;
using NGit.Treewalk;
using Sharpen;

namespace NGit.Treewalk
{
	public class PostOrderTreeWalkTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestInitialize_NoPostOrder()
		{
			TreeWalk tw = new TreeWalk(db);
			NUnit.Framework.Assert.IsFalse(tw.PostOrderTraversal);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInitialize_TogglePostOrder()
		{
			TreeWalk tw = new TreeWalk(db);
			NUnit.Framework.Assert.IsFalse(tw.PostOrderTraversal);
			tw.PostOrderTraversal = true;
			NUnit.Framework.Assert.IsTrue(tw.PostOrderTraversal);
			tw.PostOrderTraversal = false;
			NUnit.Framework.Assert.IsFalse(tw.PostOrderTraversal);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestResetDoesNotAffectPostOrder()
		{
			TreeWalk tw = new TreeWalk(db);
			tw.PostOrderTraversal = true;
			NUnit.Framework.Assert.IsTrue(tw.PostOrderTraversal);
			tw.Reset();
			NUnit.Framework.Assert.IsTrue(tw.PostOrderTraversal);
			tw.PostOrderTraversal = false;
			NUnit.Framework.Assert.IsFalse(tw.PostOrderTraversal);
			tw.Reset();
			NUnit.Framework.Assert.IsFalse(tw.PostOrderTraversal);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNoPostOrder()
		{
			DirCache tree = db.ReadDirCache();
			{
				DirCacheBuilder b = tree.Builder();
				b.Add(MakeFile("a"));
				b.Add(MakeFile("b/c"));
				b.Add(MakeFile("b/d"));
				b.Add(MakeFile("q"));
				b.Finish();
				NUnit.Framework.Assert.AreEqual(4, tree.GetEntryCount());
			}
			TreeWalk tw = new TreeWalk(db);
			tw.Reset();
			tw.PostOrderTraversal = false;
			tw.AddTree(new DirCacheIterator(tree));
			AssertModes("a", FileMode.REGULAR_FILE, tw);
			AssertModes("b", FileMode.TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsSubtree);
			NUnit.Framework.Assert.IsFalse(tw.IsPostChildren);
			tw.EnterSubtree();
			AssertModes("b/c", FileMode.REGULAR_FILE, tw);
			AssertModes("b/d", FileMode.REGULAR_FILE, tw);
			AssertModes("q", FileMode.REGULAR_FILE, tw);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWithPostOrder_EnterSubtree()
		{
			DirCache tree = db.ReadDirCache();
			{
				DirCacheBuilder b = tree.Builder();
				b.Add(MakeFile("a"));
				b.Add(MakeFile("b/c"));
				b.Add(MakeFile("b/d"));
				b.Add(MakeFile("q"));
				b.Finish();
				NUnit.Framework.Assert.AreEqual(4, tree.GetEntryCount());
			}
			TreeWalk tw = new TreeWalk(db);
			tw.Reset();
			tw.PostOrderTraversal = true;
			tw.AddTree(new DirCacheIterator(tree));
			AssertModes("a", FileMode.REGULAR_FILE, tw);
			AssertModes("b", FileMode.TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsSubtree);
			NUnit.Framework.Assert.IsFalse(tw.IsPostChildren);
			tw.EnterSubtree();
			AssertModes("b/c", FileMode.REGULAR_FILE, tw);
			AssertModes("b/d", FileMode.REGULAR_FILE, tw);
			AssertModes("b", FileMode.TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsSubtree);
			NUnit.Framework.Assert.IsTrue(tw.IsPostChildren);
			AssertModes("q", FileMode.REGULAR_FILE, tw);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWithPostOrder_NoEnterSubtree()
		{
			DirCache tree = db.ReadDirCache();
			{
				DirCacheBuilder b = tree.Builder();
				b.Add(MakeFile("a"));
				b.Add(MakeFile("b/c"));
				b.Add(MakeFile("b/d"));
				b.Add(MakeFile("q"));
				b.Finish();
				NUnit.Framework.Assert.AreEqual(4, tree.GetEntryCount());
			}
			TreeWalk tw = new TreeWalk(db);
			tw.Reset();
			tw.PostOrderTraversal = true;
			tw.AddTree(new DirCacheIterator(tree));
			AssertModes("a", FileMode.REGULAR_FILE, tw);
			AssertModes("b", FileMode.TREE, tw);
			NUnit.Framework.Assert.IsTrue(tw.IsSubtree);
			NUnit.Framework.Assert.IsFalse(tw.IsPostChildren);
			AssertModes("q", FileMode.REGULAR_FILE, tw);
		}

		/// <exception cref="System.Exception"></exception>
		private DirCacheEntry MakeFile(string path)
		{
			DirCacheEntry ent = new DirCacheEntry(path);
			ent.SetFileMode(FileMode.REGULAR_FILE);
			ent.SetObjectId(new ObjectInserter.Formatter().IdFor(Constants.OBJ_BLOB, Constants
				.Encode(path)));
			return ent;
		}

		/// <exception cref="System.Exception"></exception>
		private static void AssertModes(string path, FileMode mode0, TreeWalk tw)
		{
			NUnit.Framework.Assert.IsTrue("has " + path, tw.Next());
			NUnit.Framework.Assert.AreEqual(path, tw.PathString);
			NUnit.Framework.Assert.AreEqual(mode0, tw.GetFileMode(0));
		}
	}
}
