using System;
using NGit;
using NGit.Dircache;
using Sharpen;

namespace NGit.Dircache
{
	public class DirCacheBuilderTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestBuildEmpty()
		{
			{
				DirCache dc = db.LockDirCache();
				DirCacheBuilder b = dc.Builder();
				NUnit.Framework.Assert.IsNotNull(b);
				b.Finish();
				dc.Write();
				NUnit.Framework.Assert.IsTrue(dc.Commit());
			}
			{
				DirCache dc = db.ReadDirCache();
				NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBuildRejectsUnsetFileMode()
		{
			DirCache dc = DirCache.NewInCore();
			DirCacheBuilder b = dc.Builder();
			NUnit.Framework.Assert.IsNotNull(b);
			DirCacheEntry e = new DirCacheEntry("a");
			NUnit.Framework.Assert.AreEqual(0, e.GetRawMode());
			try
			{
				b.Add(e);
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("FileMode not set for path a", err.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBuildOneFile_FinishWriteCommit()
		{
			string path = "a-file-path";
			FileMode mode = FileMode.REGULAR_FILE;
			long lastModified = 1218123387057L;
			int length = 1342;
			DirCacheEntry entOrig;
			{
				DirCache dc = db.LockDirCache();
				DirCacheBuilder b = dc.Builder();
				NUnit.Framework.Assert.IsNotNull(b);
				entOrig = new DirCacheEntry(path);
				entOrig.SetFileMode(mode);
				entOrig.SetLastModified(lastModified);
				entOrig.SetLength(length);
				NUnit.Framework.Assert.AreNotSame(path, entOrig.GetPathString());
				NUnit.Framework.Assert.AreEqual(path, entOrig.GetPathString());
				AssertEquals(ObjectId.ZeroId, entOrig.GetObjectId());
				NUnit.Framework.Assert.AreEqual(mode.GetBits(), entOrig.GetRawMode());
				NUnit.Framework.Assert.AreEqual(0, entOrig.GetStage());
				NUnit.Framework.Assert.AreEqual(lastModified, entOrig.GetLastModified());
				NUnit.Framework.Assert.AreEqual(length, entOrig.GetLength());
				NUnit.Framework.Assert.IsFalse(entOrig.IsAssumeValid());
				b.Add(entOrig);
				b.Finish();
				NUnit.Framework.Assert.AreEqual(1, dc.GetEntryCount());
				NUnit.Framework.Assert.AreSame(entOrig, dc.GetEntry(0));
				dc.Write();
				NUnit.Framework.Assert.IsTrue(dc.Commit());
			}
			{
				DirCache dc = db.ReadDirCache();
				NUnit.Framework.Assert.AreEqual(1, dc.GetEntryCount());
				DirCacheEntry entRead = dc.GetEntry(0);
				NUnit.Framework.Assert.AreNotSame(entOrig, entRead);
				NUnit.Framework.Assert.AreEqual(path, entRead.GetPathString());
				AssertEquals(ObjectId.ZeroId, entOrig.GetObjectId());
				NUnit.Framework.Assert.AreEqual(mode.GetBits(), entOrig.GetRawMode());
				NUnit.Framework.Assert.AreEqual(0, entOrig.GetStage());
				NUnit.Framework.Assert.AreEqual(lastModified, entOrig.GetLastModified());
				NUnit.Framework.Assert.AreEqual(length, entOrig.GetLength());
				NUnit.Framework.Assert.IsFalse(entOrig.IsAssumeValid());
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBuildOneFile_Commit()
		{
			string path = "a-file-path";
			FileMode mode = FileMode.REGULAR_FILE;
			long lastModified = 1218123387057L;
			int length = 1342;
			DirCacheEntry entOrig;
			{
				DirCache dc = db.LockDirCache();
				DirCacheBuilder b = dc.Builder();
				NUnit.Framework.Assert.IsNotNull(b);
				entOrig = new DirCacheEntry(path);
				entOrig.SetFileMode(mode);
				entOrig.SetLastModified(lastModified);
				entOrig.SetLength(length);
				NUnit.Framework.Assert.AreNotSame(path, entOrig.GetPathString());
				NUnit.Framework.Assert.AreEqual(path, entOrig.GetPathString());
				AssertEquals(ObjectId.ZeroId, entOrig.GetObjectId());
				NUnit.Framework.Assert.AreEqual(mode.GetBits(), entOrig.GetRawMode());
				NUnit.Framework.Assert.AreEqual(0, entOrig.GetStage());
				NUnit.Framework.Assert.AreEqual(lastModified, entOrig.GetLastModified());
				NUnit.Framework.Assert.AreEqual(length, entOrig.GetLength());
				NUnit.Framework.Assert.IsFalse(entOrig.IsAssumeValid());
				b.Add(entOrig);
				NUnit.Framework.Assert.IsTrue(b.Commit());
				NUnit.Framework.Assert.AreEqual(1, dc.GetEntryCount());
				NUnit.Framework.Assert.AreSame(entOrig, dc.GetEntry(0));
				NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "index.lock").Exists());
			}
			{
				DirCache dc = db.ReadDirCache();
				NUnit.Framework.Assert.AreEqual(1, dc.GetEntryCount());
				DirCacheEntry entRead = dc.GetEntry(0);
				NUnit.Framework.Assert.AreNotSame(entOrig, entRead);
				NUnit.Framework.Assert.AreEqual(path, entRead.GetPathString());
				AssertEquals(ObjectId.ZeroId, entOrig.GetObjectId());
				NUnit.Framework.Assert.AreEqual(mode.GetBits(), entOrig.GetRawMode());
				NUnit.Framework.Assert.AreEqual(0, entOrig.GetStage());
				NUnit.Framework.Assert.AreEqual(lastModified, entOrig.GetLastModified());
				NUnit.Framework.Assert.AreEqual(length, entOrig.GetLength());
				NUnit.Framework.Assert.IsFalse(entOrig.IsAssumeValid());
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFindSingleFile()
		{
			string path = "a-file-path";
			DirCache dc = db.ReadDirCache();
			DirCacheBuilder b = dc.Builder();
			NUnit.Framework.Assert.IsNotNull(b);
			DirCacheEntry entOrig = new DirCacheEntry(path);
			entOrig.SetFileMode(FileMode.REGULAR_FILE);
			NUnit.Framework.Assert.AreNotSame(path, entOrig.GetPathString());
			NUnit.Framework.Assert.AreEqual(path, entOrig.GetPathString());
			b.Add(entOrig);
			b.Finish();
			NUnit.Framework.Assert.AreEqual(1, dc.GetEntryCount());
			NUnit.Framework.Assert.AreSame(entOrig, dc.GetEntry(0));
			NUnit.Framework.Assert.AreEqual(0, dc.FindEntry(path));
			NUnit.Framework.Assert.AreEqual(-1, dc.FindEntry("@@-before"));
			NUnit.Framework.Assert.AreEqual(0, Real(dc.FindEntry("@@-before")));
			NUnit.Framework.Assert.AreEqual(-2, dc.FindEntry("a-zoo"));
			NUnit.Framework.Assert.AreEqual(1, Real(dc.FindEntry("a-zoo")));
			NUnit.Framework.Assert.AreSame(entOrig, dc.GetEntry(path));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestAdd_InGitSortOrder()
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
			NUnit.Framework.Assert.AreEqual(paths.Length, dc.GetEntryCount());
			for (int i_2 = 0; i_2 < paths.Length; i_2++)
			{
				NUnit.Framework.Assert.AreSame(ents[i_2], dc.GetEntry(i_2));
				NUnit.Framework.Assert.AreEqual(paths[i_2], dc.GetEntry(i_2).GetPathString());
				NUnit.Framework.Assert.AreEqual(i_2, dc.FindEntry(paths[i_2]));
				NUnit.Framework.Assert.AreSame(ents[i_2], dc.GetEntry(paths[i_2]));
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestAdd_ReverseGitSortOrder()
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
			for (int i_1 = ents.Length - 1; i_1 >= 0; i_1--)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			NUnit.Framework.Assert.AreEqual(paths.Length, dc.GetEntryCount());
			for (int i_2 = 0; i_2 < paths.Length; i_2++)
			{
				NUnit.Framework.Assert.AreSame(ents[i_2], dc.GetEntry(i_2));
				NUnit.Framework.Assert.AreEqual(paths[i_2], dc.GetEntry(i_2).GetPathString());
				NUnit.Framework.Assert.AreEqual(i_2, dc.FindEntry(paths[i_2]));
				NUnit.Framework.Assert.AreSame(ents[i_2], dc.GetEntry(paths[i_2]));
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBuilderClear()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a.b", "a/b", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(FileMode.REGULAR_FILE);
			}
			{
				DirCacheBuilder b = dc.Builder();
				for (int i_1 = 0; i_1 < ents.Length; i_1++)
				{
					b.Add(ents[i_1]);
				}
				b.Finish();
			}
			NUnit.Framework.Assert.AreEqual(paths.Length, dc.GetEntryCount());
			{
				DirCacheBuilder b = dc.Builder();
				b.Finish();
			}
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
		}

		private static int Real(int eIdx)
		{
			if (eIdx < 0)
			{
				eIdx = -(eIdx + 1);
			}
			return eIdx;
		}
	}
}
