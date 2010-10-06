using NGit;
using NGit.Dircache;
using Sharpen;

namespace NGit.Dircache
{
	public class DirCacheFindTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestEntriesWithin()
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
			NUnit.Framework.Assert.AreEqual(paths.Length, dc.GetEntryCount());
			for (int i_2 = 0; i_2 < ents.Length; i_2++)
			{
				NUnit.Framework.Assert.AreSame(ents[i_2], dc.GetEntry(i_2));
			}
			{
				DirCacheEntry[] aContents = dc.GetEntriesWithin("a");
				NUnit.Framework.Assert.IsNotNull(aContents);
				NUnit.Framework.Assert.AreEqual(aLast - aFirst + 1, aContents.Length);
				for (int i_3 = aFirst; i_3 <= aLast; i_3++, j++)
				{
					NUnit.Framework.Assert.AreSame(ents[i_3], aContents[j]);
				}
			}
			{
				DirCacheEntry[] aContents = dc.GetEntriesWithin("a/");
				NUnit.Framework.Assert.IsNotNull(aContents);
				NUnit.Framework.Assert.AreEqual(aLast - aFirst + 1, aContents.Length);
				for (int i_3 = aFirst; i_3 <= aLast; i_3++, j++)
				{
					NUnit.Framework.Assert.AreSame(ents[i_3], aContents[j]);
				}
			}
			NUnit.Framework.Assert.IsNotNull(dc.GetEntriesWithin("a."));
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntriesWithin("a.").Length);
			NUnit.Framework.Assert.IsNotNull(dc.GetEntriesWithin("a0b"));
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntriesWithin("a0b.").Length);
			NUnit.Framework.Assert.IsNotNull(dc.GetEntriesWithin("zoo"));
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntriesWithin("zoo.").Length);
		}
	}
}
