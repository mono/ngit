using NGit;
using NGit.Dircache;
using Sharpen;

namespace NGit.Dircache
{
	public class DirCacheBasicTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestReadMissing_RealIndex()
		{
			FilePath idx = new FilePath(db.Directory, "index");
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			DirCache dc = db.ReadDirCache();
			NUnit.Framework.Assert.IsNotNull(dc);
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestReadMissing_TempIndex()
		{
			FilePath idx = new FilePath(db.Directory, "tmp_index");
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			DirCache dc = DirCache.Read(idx, db.FileSystem);
			NUnit.Framework.Assert.IsNotNull(dc);
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestLockMissing_RealIndex()
		{
			FilePath idx = new FilePath(db.Directory, "index");
			FilePath lck = new FilePath(db.Directory, "index.lock");
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			NUnit.Framework.Assert.IsFalse(lck.Exists());
			DirCache dc = db.LockDirCache();
			NUnit.Framework.Assert.IsNotNull(dc);
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			NUnit.Framework.Assert.IsTrue(lck.Exists());
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
			dc.Unlock();
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			NUnit.Framework.Assert.IsFalse(lck.Exists());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestLockMissing_TempIndex()
		{
			FilePath idx = new FilePath(db.Directory, "tmp_index");
			FilePath lck = new FilePath(db.Directory, "tmp_index.lock");
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			NUnit.Framework.Assert.IsFalse(lck.Exists());
			DirCache dc = DirCache.Lock(idx, db.FileSystem);
			NUnit.Framework.Assert.IsNotNull(dc);
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			NUnit.Framework.Assert.IsTrue(lck.Exists());
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
			dc.Unlock();
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			NUnit.Framework.Assert.IsFalse(lck.Exists());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWriteEmptyUnlock_RealIndex()
		{
			FilePath idx = new FilePath(db.Directory, "index");
			FilePath lck = new FilePath(db.Directory, "index.lock");
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			NUnit.Framework.Assert.IsFalse(lck.Exists());
			DirCache dc = db.LockDirCache();
			NUnit.Framework.Assert.AreEqual(0, lck.Length());
			dc.Write();
			NUnit.Framework.Assert.AreEqual(12 + 20, lck.Length());
			dc.Unlock();
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			NUnit.Framework.Assert.IsFalse(lck.Exists());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWriteEmptyCommit_RealIndex()
		{
			FilePath idx = new FilePath(db.Directory, "index");
			FilePath lck = new FilePath(db.Directory, "index.lock");
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			NUnit.Framework.Assert.IsFalse(lck.Exists());
			DirCache dc = db.LockDirCache();
			NUnit.Framework.Assert.AreEqual(0, lck.Length());
			dc.Write();
			NUnit.Framework.Assert.AreEqual(12 + 20, lck.Length());
			NUnit.Framework.Assert.IsTrue(dc.Commit());
			NUnit.Framework.Assert.IsTrue(idx.Exists());
			NUnit.Framework.Assert.IsFalse(lck.Exists());
			NUnit.Framework.Assert.AreEqual(12 + 20, idx.Length());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWriteEmptyReadEmpty_RealIndex()
		{
			FilePath idx = new FilePath(db.Directory, "index");
			FilePath lck = new FilePath(db.Directory, "index.lock");
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			NUnit.Framework.Assert.IsFalse(lck.Exists());
			{
				DirCache dc = db.LockDirCache();
				dc.Write();
				NUnit.Framework.Assert.IsTrue(dc.Commit());
				NUnit.Framework.Assert.IsTrue(idx.Exists());
			}
			{
				DirCache dc = db.ReadDirCache();
				NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWriteEmptyLockEmpty_RealIndex()
		{
			FilePath idx = new FilePath(db.Directory, "index");
			FilePath lck = new FilePath(db.Directory, "index.lock");
			NUnit.Framework.Assert.IsFalse(idx.Exists());
			NUnit.Framework.Assert.IsFalse(lck.Exists());
			{
				DirCache dc = db.LockDirCache();
				dc.Write();
				NUnit.Framework.Assert.IsTrue(dc.Commit());
				NUnit.Framework.Assert.IsTrue(idx.Exists());
			}
			{
				DirCache dc = db.LockDirCache();
				NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
				NUnit.Framework.Assert.IsTrue(idx.Exists());
				NUnit.Framework.Assert.IsTrue(lck.Exists());
				dc.Unlock();
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBuildThenClear()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a.b", "a/b", "a0b" };
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
			b.Finish();
			NUnit.Framework.Assert.IsFalse(dc.HasUnmergedPaths());
			NUnit.Framework.Assert.AreEqual(paths.Length, dc.GetEntryCount());
			dc.Clear();
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
			NUnit.Framework.Assert.IsFalse(dc.HasUnmergedPaths());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDetectUnmergedPaths()
		{
			DirCache dc = db.ReadDirCache();
			DirCacheEntry[] ents = new DirCacheEntry[3];
			ents[0] = new DirCacheEntry("a", 1);
			ents[0].SetFileMode(FileMode.REGULAR_FILE);
			ents[1] = new DirCacheEntry("a", 2);
			ents[1].SetFileMode(FileMode.REGULAR_FILE);
			ents[2] = new DirCacheEntry("a", 3);
			ents[2].SetFileMode(FileMode.REGULAR_FILE);
			DirCacheBuilder b = dc.Builder();
			for (int i = 0; i < ents.Length; i++)
			{
				b.Add(ents[i]);
			}
			b.Finish();
			NUnit.Framework.Assert.IsTrue(dc.HasUnmergedPaths());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFindOnEmpty()
		{
			DirCache dc = DirCache.NewInCore();
			byte[] path = Constants.Encode("a");
			NUnit.Framework.Assert.AreEqual(-1, dc.FindEntry(path, path.Length));
		}
	}
}
