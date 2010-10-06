using System.Text;
using NGit;
using NGit.Dircache;
using Sharpen;

namespace NGit.Dircache
{
	public class DirCacheLargePathTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestPath_4090()
		{
			TestLongPath(4090);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPath_4094()
		{
			TestLongPath(4094);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPath_4095()
		{
			TestLongPath(4095);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPath_4096()
		{
			TestLongPath(4096);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPath_16384()
		{
			TestLongPath(16384);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private void TestLongPath(int len)
		{
			string longPath = MakeLongPath(len);
			string shortPath = "~~~ shorter-path";
			DirCacheEntry longEnt = new DirCacheEntry(longPath);
			DirCacheEntry shortEnt = new DirCacheEntry(shortPath);
			longEnt.SetFileMode(FileMode.REGULAR_FILE);
			shortEnt.SetFileMode(FileMode.REGULAR_FILE);
			NUnit.Framework.Assert.AreEqual(longPath, longEnt.GetPathString());
			NUnit.Framework.Assert.AreEqual(shortPath, shortEnt.GetPathString());
			{
				DirCache dc1 = db.LockDirCache();
				{
					DirCacheBuilder b = dc1.Builder();
					b.Add(longEnt);
					b.Add(shortEnt);
					NUnit.Framework.Assert.IsTrue(b.Commit());
				}
				NUnit.Framework.Assert.AreEqual(2, dc1.GetEntryCount());
				NUnit.Framework.Assert.AreSame(longEnt, dc1.GetEntry(0));
				NUnit.Framework.Assert.AreSame(shortEnt, dc1.GetEntry(1));
			}
			{
				DirCache dc2 = db.ReadDirCache();
				NUnit.Framework.Assert.AreEqual(2, dc2.GetEntryCount());
				NUnit.Framework.Assert.AreNotSame(longEnt, dc2.GetEntry(0));
				NUnit.Framework.Assert.AreEqual(longPath, dc2.GetEntry(0).GetPathString());
				NUnit.Framework.Assert.AreNotSame(shortEnt, dc2.GetEntry(1));
				NUnit.Framework.Assert.AreEqual(shortPath, dc2.GetEntry(1).GetPathString());
			}
		}

		private static string MakeLongPath(int len)
		{
			StringBuilder r = new StringBuilder(len);
			for (int i = 0; i < len; i++)
			{
				r.Append('a' + (i % 26));
			}
			return r.ToString();
		}
	}
}
