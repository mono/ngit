using NGit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	public class T0004_PackReader : SampleDataRepositoryTestCase
	{
		private static readonly string PACK_NAME = "pack-34be9032ac282b11fa9babdc2b2a93ca996c9c2f";

		private static readonly FilePath TEST_PACK = JGitTestUtil.GetTestResourceFile(PACK_NAME
			 + ".pack");

		private static readonly FilePath TEST_IDX = JGitTestUtil.GetTestResourceFile(PACK_NAME
			 + ".idx");

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Test003_lookupCompressedObject()
		{
			PackFile pr;
			ObjectId id;
			ObjectLoader or;
			id = ObjectId.FromString("902d5476fa249b7abc9d84c611577a81381f0327");
			pr = new PackFile(TEST_IDX, TEST_PACK);
			or = pr.Get(new WindowCursor(null), id);
			NUnit.Framework.Assert.IsNotNull(or);
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_TREE, or.GetType());
			NUnit.Framework.Assert.AreEqual(35, or.GetSize());
			pr.Close();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Test004_lookupDeltifiedObject()
		{
			ObjectId id;
			ObjectLoader or;
			id = ObjectId.FromString("5b6e7c66c276e7610d4a73c70ec1a1f7c1003259");
			or = db.Open(id);
			NUnit.Framework.Assert.IsNotNull(or);
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_BLOB, or.GetType());
			NUnit.Framework.Assert.AreEqual(18009, or.GetSize());
		}
	}
}
