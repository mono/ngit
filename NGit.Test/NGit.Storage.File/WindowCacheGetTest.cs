using System.Collections.Generic;
using NGit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	public class WindowCacheGetTest : SampleDataRepositoryTestCase
	{
		private IList<WindowCacheGetTest.TestObject> toLoad;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			toLoad = new AList<WindowCacheGetTest.TestObject>();
			BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(
				JGitTestUtil.GetTestResourceFile("all_packed_objects.txt")), Constants.CHARSET));
			try
			{
				string line;
				while ((line = br.ReadLine()) != null)
				{
					string[] parts = line.Split(" {1,}");
					WindowCacheGetTest.TestObject o = new WindowCacheGetTest.TestObject(this);
					o.id = ObjectId.FromString(parts[0]);
					o.SetType(parts[1]);
					// parts[2] is the inflate size
					// parts[3] is the size-in-pack
					// parts[4] is the offset in the pack
					toLoad.AddItem(o);
				}
			}
			finally
			{
				br.Close();
			}
			NUnit.Framework.Assert.AreEqual(96, toLoad.Count);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestCache_Defaults()
		{
			WindowCacheConfig cfg = new WindowCacheConfig();
			WindowCache.Reconfigure(cfg);
			DoCacheTests();
			CheckLimits(cfg);
			WindowCache cache = WindowCache.GetInstance();
			NUnit.Framework.Assert.AreEqual(6, cache.GetOpenFiles());
			NUnit.Framework.Assert.AreEqual(17346, cache.GetOpenBytes());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestCache_TooFewFiles()
		{
			WindowCacheConfig cfg = new WindowCacheConfig();
			cfg.SetPackedGitOpenFiles(2);
			WindowCache.Reconfigure(cfg);
			DoCacheTests();
			CheckLimits(cfg);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestCache_TooSmallLimit()
		{
			WindowCacheConfig cfg = new WindowCacheConfig();
			cfg.SetPackedGitWindowSize(4096);
			cfg.SetPackedGitLimit(4096);
			WindowCache.Reconfigure(cfg);
			DoCacheTests();
			CheckLimits(cfg);
		}

		private void CheckLimits(WindowCacheConfig cfg)
		{
			WindowCache cache = WindowCache.GetInstance();
			NUnit.Framework.Assert.IsTrue(cache.GetOpenFiles() <= cfg.GetPackedGitOpenFiles()
				);
			NUnit.Framework.Assert.IsTrue(cache.GetOpenBytes() <= cfg.GetPackedGitLimit());
			NUnit.Framework.Assert.IsTrue(0 < cache.GetOpenFiles());
			NUnit.Framework.Assert.IsTrue(0 < cache.GetOpenBytes());
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void DoCacheTests()
		{
			foreach (WindowCacheGetTest.TestObject o in toLoad)
			{
				ObjectLoader or = db.Open(o.id, o.type);
				NUnit.Framework.Assert.IsNotNull(or);
				NUnit.Framework.Assert.AreEqual(o.type, or.GetType());
			}
		}

		private class TestObject
		{
			internal ObjectId id;

			internal int type;

			/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
			internal virtual void SetType(string typeStr)
			{
				byte[] typeRaw = Constants.Encode(typeStr + " ");
				MutableInteger ptr = new MutableInteger();
				this.type = Constants.DecodeTypeString(this.id, typeRaw, unchecked((byte)' '), ptr
					);
			}

			internal TestObject(WindowCacheGetTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly WindowCacheGetTest _enclosing;
		}
	}
}
