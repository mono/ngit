using System;
using NGit;
using NGit.Dircache;
using NUnit.Framework;
using Sharpen;

namespace NGit.Dircache
{
	public class DirCacheEntryTest : TestCase
	{
		public virtual void TestIsValidPath()
		{
			NUnit.Framework.Assert.IsTrue(IsValidPath("a"));
			NUnit.Framework.Assert.IsTrue(IsValidPath("a/b"));
			NUnit.Framework.Assert.IsTrue(IsValidPath("ab/cd/ef"));
			NUnit.Framework.Assert.IsFalse(IsValidPath(string.Empty));
			NUnit.Framework.Assert.IsFalse(IsValidPath("/a"));
			NUnit.Framework.Assert.IsFalse(IsValidPath("a//b"));
			NUnit.Framework.Assert.IsFalse(IsValidPath("ab/cd//ef"));
			NUnit.Framework.Assert.IsFalse(IsValidPath("a/"));
			NUnit.Framework.Assert.IsFalse(IsValidPath("ab/cd/ef/"));
			NUnit.Framework.Assert.IsFalse(IsValidPath("a\u0000b"));
		}

		private static bool IsValidPath(string path)
		{
			return DirCacheEntry.IsValidPath(Constants.Encode(path));
		}

		public virtual void TestCreate_ByStringPath()
		{
			NUnit.Framework.Assert.AreEqual("a", new DirCacheEntry("a").GetPathString());
			NUnit.Framework.Assert.AreEqual("a/b", new DirCacheEntry("a/b").GetPathString());
			try
			{
				new DirCacheEntry("/a");
				NUnit.Framework.Assert.Fail("Incorrectly created DirCacheEntry");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Invalid path: /a", err.Message);
			}
		}

		public virtual void TestCreate_ByStringPathAndStage()
		{
			DirCacheEntry e;
			e = new DirCacheEntry("a", 0);
			NUnit.Framework.Assert.AreEqual("a", e.GetPathString());
			NUnit.Framework.Assert.AreEqual(0, e.GetStage());
			e = new DirCacheEntry("a/b", 1);
			NUnit.Framework.Assert.AreEqual("a/b", e.GetPathString());
			NUnit.Framework.Assert.AreEqual(1, e.GetStage());
			e = new DirCacheEntry("a/c", 2);
			NUnit.Framework.Assert.AreEqual("a/c", e.GetPathString());
			NUnit.Framework.Assert.AreEqual(2, e.GetStage());
			e = new DirCacheEntry("a/d", 3);
			NUnit.Framework.Assert.AreEqual("a/d", e.GetPathString());
			NUnit.Framework.Assert.AreEqual(3, e.GetStage());
			try
			{
				new DirCacheEntry("/a", 1);
				NUnit.Framework.Assert.Fail("Incorrectly created DirCacheEntry");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Invalid path: /a", err.Message);
			}
			try
			{
				new DirCacheEntry("a", -11);
				NUnit.Framework.Assert.Fail("Incorrectly created DirCacheEntry");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Invalid stage -11 for path a", err.Message);
			}
			try
			{
				new DirCacheEntry("a", 4);
				NUnit.Framework.Assert.Fail("Incorrectly created DirCacheEntry");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Invalid stage 4 for path a", err.Message);
			}
		}

		public virtual void TestSetFileMode()
		{
			DirCacheEntry e = new DirCacheEntry("a");
			NUnit.Framework.Assert.AreEqual(0, e.GetRawMode());
			e.SetFileMode(FileMode.REGULAR_FILE);
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, e.GetFileMode());
			NUnit.Framework.Assert.AreEqual(FileMode.REGULAR_FILE.GetBits(), e.GetRawMode());
			e.SetFileMode(FileMode.EXECUTABLE_FILE);
			NUnit.Framework.Assert.AreSame(FileMode.EXECUTABLE_FILE, e.GetFileMode());
			NUnit.Framework.Assert.AreEqual(FileMode.EXECUTABLE_FILE.GetBits(), e.GetRawMode(
				));
			e.SetFileMode(FileMode.SYMLINK);
			NUnit.Framework.Assert.AreSame(FileMode.SYMLINK, e.GetFileMode());
			NUnit.Framework.Assert.AreEqual(FileMode.SYMLINK.GetBits(), e.GetRawMode());
			e.SetFileMode(FileMode.GITLINK);
			NUnit.Framework.Assert.AreSame(FileMode.GITLINK, e.GetFileMode());
			NUnit.Framework.Assert.AreEqual(FileMode.GITLINK.GetBits(), e.GetRawMode());
			try
			{
				e.SetFileMode(FileMode.MISSING);
				NUnit.Framework.Assert.Fail("incorrectly accepted FileMode.MISSING");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Invalid mode 0 for path a", err.Message);
			}
			try
			{
				e.SetFileMode(FileMode.TREE);
				NUnit.Framework.Assert.Fail("incorrectly accepted FileMode.TREE");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Invalid mode 40000 for path a", err.Message);
			}
		}
	}
}
