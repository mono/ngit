using NGit;
using NGit.Dircache;
using Sharpen;

namespace NGit.Dircache
{
	public class DirCacheTreeTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestEmptyCache_NoCacheTree()
		{
			DirCache dc = db.ReadDirCache();
			NUnit.Framework.Assert.IsNull(dc.GetCacheTree(false));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestEmptyCache_CreateEmptyCacheTree()
		{
			DirCache dc = db.ReadDirCache();
			DirCacheTree tree = dc.GetCacheTree(true);
			NUnit.Framework.Assert.IsNotNull(tree);
			NUnit.Framework.Assert.AreSame(tree, dc.GetCacheTree(false));
			NUnit.Framework.Assert.AreSame(tree, dc.GetCacheTree(true));
			NUnit.Framework.Assert.AreEqual(string.Empty, tree.GetNameString());
			NUnit.Framework.Assert.AreEqual(string.Empty, tree.GetPathString());
			NUnit.Framework.Assert.AreEqual(0, tree.GetChildCount());
			NUnit.Framework.Assert.AreEqual(0, tree.GetEntrySpan());
			NUnit.Framework.Assert.IsFalse(tree.IsValid());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestEmptyCache_Clear_NoCacheTree()
		{
			DirCache dc = db.ReadDirCache();
			DirCacheTree tree = dc.GetCacheTree(true);
			NUnit.Framework.Assert.IsNotNull(tree);
			dc.Clear();
			NUnit.Framework.Assert.IsNull(dc.GetCacheTree(false));
			NUnit.Framework.Assert.AreNotSame(tree, dc.GetCacheTree(true));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSingleSubtree()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a/b", "a/c", "a/d", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(FileMode.REGULAR_FILE);
			}
			int aFirst = 1;
			int aLast = 3;
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			NUnit.Framework.Assert.IsNull(dc.GetCacheTree(false));
			DirCacheTree root = dc.GetCacheTree(true);
			NUnit.Framework.Assert.IsNotNull(root);
			NUnit.Framework.Assert.AreSame(root, dc.GetCacheTree(true));
			NUnit.Framework.Assert.AreEqual(string.Empty, root.GetNameString());
			NUnit.Framework.Assert.AreEqual(string.Empty, root.GetPathString());
			NUnit.Framework.Assert.AreEqual(1, root.GetChildCount());
			NUnit.Framework.Assert.AreEqual(dc.GetEntryCount(), root.GetEntrySpan());
			NUnit.Framework.Assert.IsFalse(root.IsValid());
			DirCacheTree aTree = root.GetChild(0);
			NUnit.Framework.Assert.IsNotNull(aTree);
			NUnit.Framework.Assert.AreSame(aTree, root.GetChild(0));
			NUnit.Framework.Assert.AreEqual("a", aTree.GetNameString());
			NUnit.Framework.Assert.AreEqual("a/", aTree.GetPathString());
			NUnit.Framework.Assert.AreEqual(0, aTree.GetChildCount());
			NUnit.Framework.Assert.AreEqual(aLast - aFirst + 1, aTree.GetEntrySpan());
			NUnit.Framework.Assert.IsFalse(aTree.IsValid());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoLevelSubtree()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a/b", "a/c/e", "a/c/f", "a/d", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(FileMode.REGULAR_FILE);
			}
			int aFirst = 1;
			int aLast = 4;
			int acFirst = 2;
			int acLast = 3;
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			NUnit.Framework.Assert.IsNull(dc.GetCacheTree(false));
			DirCacheTree root = dc.GetCacheTree(true);
			NUnit.Framework.Assert.IsNotNull(root);
			NUnit.Framework.Assert.AreSame(root, dc.GetCacheTree(true));
			NUnit.Framework.Assert.AreEqual(string.Empty, root.GetNameString());
			NUnit.Framework.Assert.AreEqual(string.Empty, root.GetPathString());
			NUnit.Framework.Assert.AreEqual(1, root.GetChildCount());
			NUnit.Framework.Assert.AreEqual(dc.GetEntryCount(), root.GetEntrySpan());
			NUnit.Framework.Assert.IsFalse(root.IsValid());
			DirCacheTree aTree = root.GetChild(0);
			NUnit.Framework.Assert.IsNotNull(aTree);
			NUnit.Framework.Assert.AreSame(aTree, root.GetChild(0));
			NUnit.Framework.Assert.AreEqual("a", aTree.GetNameString());
			NUnit.Framework.Assert.AreEqual("a/", aTree.GetPathString());
			NUnit.Framework.Assert.AreEqual(1, aTree.GetChildCount());
			NUnit.Framework.Assert.AreEqual(aLast - aFirst + 1, aTree.GetEntrySpan());
			NUnit.Framework.Assert.IsFalse(aTree.IsValid());
			DirCacheTree acTree = aTree.GetChild(0);
			NUnit.Framework.Assert.IsNotNull(acTree);
			NUnit.Framework.Assert.AreSame(acTree, aTree.GetChild(0));
			NUnit.Framework.Assert.AreEqual("c", acTree.GetNameString());
			NUnit.Framework.Assert.AreEqual("a/c/", acTree.GetPathString());
			NUnit.Framework.Assert.AreEqual(0, acTree.GetChildCount());
			NUnit.Framework.Assert.AreEqual(acLast - acFirst + 1, acTree.GetEntrySpan());
			NUnit.Framework.Assert.IsFalse(acTree.IsValid());
		}

		/// <summary>We had bugs related to buffer size in the DirCache.</summary>
		/// <remarks>
		/// We had bugs related to buffer size in the DirCache. This test creates an
		/// index larger than the default BufferedInputStream buffer size. This made
		/// the DirCache unable to read the extensions when index size exceeded the
		/// buffer size (in some cases at least).
		/// </remarks>
		/// <exception cref="NGit.Errors.CorruptObjectException">NGit.Errors.CorruptObjectException
		/// 	</exception>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestWriteReadTree()
		{
			DirCache dc = db.LockDirCache();
			string A = string.Format("a%2000s", "a");
			string B = string.Format("b%2000s", "b");
			string[] paths = new string[] { A + ".", A + "." + B, A + "/" + B, A + "0" + B };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(FileMode.REGULAR_FILE);
			}
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Commit();
			DirCache read = db.ReadDirCache();
			NUnit.Framework.Assert.AreEqual(paths.Length, read.GetEntryCount());
			NUnit.Framework.Assert.AreEqual(1, read.GetCacheTree(true).GetChildCount());
		}
	}
}
