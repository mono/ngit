using System;
using NGit;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	public class WindowCacheReconfigureTest : RepositoryTestCase
	{
		public virtual void TestConfigureCache_PackedGitLimit_0()
		{
			try
			{
				WindowCacheConfig cfg = new WindowCacheConfig();
				cfg.SetPackedGitLimit(0);
				WindowCache.Reconfigure(cfg);
				NUnit.Framework.Assert.Fail("incorrectly permitted PackedGitLimit = 0");
			}
			catch (ArgumentException)
			{
			}
		}

		//
		public virtual void TestConfigureCache_PackedGitWindowSize_0()
		{
			try
			{
				WindowCacheConfig cfg = new WindowCacheConfig();
				cfg.SetPackedGitWindowSize(0);
				WindowCache.Reconfigure(cfg);
				NUnit.Framework.Assert.Fail("incorrectly permitted PackedGitWindowSize = 0");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("Invalid window size", e.Message);
			}
		}

		public virtual void TestConfigureCache_PackedGitWindowSize_512()
		{
			try
			{
				WindowCacheConfig cfg = new WindowCacheConfig();
				cfg.SetPackedGitWindowSize(512);
				WindowCache.Reconfigure(cfg);
				NUnit.Framework.Assert.Fail("incorrectly permitted PackedGitWindowSize = 512");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("Invalid window size", e.Message);
			}
		}

		public virtual void TestConfigureCache_PackedGitWindowSize_4097()
		{
			try
			{
				WindowCacheConfig cfg = new WindowCacheConfig();
				cfg.SetPackedGitWindowSize(4097);
				WindowCache.Reconfigure(cfg);
				NUnit.Framework.Assert.Fail("incorrectly permitted PackedGitWindowSize = 4097");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("Window size must be power of 2", e.Message);
			}
		}

		public virtual void TestConfigureCache_PackedGitOpenFiles_0()
		{
			try
			{
				WindowCacheConfig cfg = new WindowCacheConfig();
				cfg.SetPackedGitOpenFiles(0);
				WindowCache.Reconfigure(cfg);
				NUnit.Framework.Assert.Fail("incorrectly permitted PackedGitOpenFiles = 0");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("Open files must be >= 1", e.Message);
			}
		}

		public virtual void TestConfigureCache_PackedGitWindowSizeAbovePackedGitLimit()
		{
			try
			{
				WindowCacheConfig cfg = new WindowCacheConfig();
				cfg.SetPackedGitLimit(1024);
				cfg.SetPackedGitWindowSize(8192);
				WindowCache.Reconfigure(cfg);
				NUnit.Framework.Assert.Fail("incorrectly permitted PackedGitWindowSize > PackedGitLimit"
					);
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("Window size must be < limit", e.Message);
			}
		}

		public virtual void TestConfigureCache_Limits1()
		{
			// This test is just to force coverage over some lower bounds for
			// the table. We don't want the table to wind up with too small
			// of a size. This is highly dependent upon the table allocation
			// algorithm actually implemented in WindowCache.
			//
			WindowCacheConfig cfg = new WindowCacheConfig();
			cfg.SetPackedGitLimit(6 * 4096 / 5);
			cfg.SetPackedGitWindowSize(4096);
			WindowCache.Reconfigure(cfg);
		}
	}
}
