using NGit;
using Sharpen;

namespace NGit
{
	public class MergeHeadMsgTest : RepositoryTestCase
	{
		private static readonly string mergeMsg = "merge a and b";

		private static readonly string sampleId = "1c6db447abdbb291b25f07be38ea0b1bf94947c5";

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestReadWriteMergeHeads()
		{
			NUnit.Framework.Assert.AreEqual(db.ReadMergeHeads(), null);
			db.WriteMergeHeads(Arrays.AsList(ObjectId.ZeroId, ObjectId.FromString(sampleId)));
			NUnit.Framework.Assert.AreEqual(Read(new FilePath(db.Directory, "MERGE_HEAD")), "0000000000000000000000000000000000000000\n1c6db447abdbb291b25f07be38ea0b1bf94947c5\n"
				);
			NUnit.Framework.Assert.AreEqual(db.ReadMergeHeads().Count, 2);
			AssertEquals(db.ReadMergeHeads()[0], ObjectId.ZeroId);
			AssertEquals(db.ReadMergeHeads()[1], ObjectId.FromString(sampleId));
			// same test again, this time with lower-level io
			FileOutputStream fos = new FileOutputStream(new FilePath(db.Directory, "MERGE_HEAD"
				));
			try
			{
				fos.Write(Sharpen.Runtime.GetBytesForString("0000000000000000000000000000000000000000\n1c6db447abdbb291b25f07be38ea0b1bf94947c5\n"
					, Constants.CHARACTER_ENCODING));
			}
			finally
			{
				fos.Close();
			}
			NUnit.Framework.Assert.AreEqual(db.ReadMergeHeads().Count, 2);
			AssertEquals(db.ReadMergeHeads()[0], ObjectId.ZeroId);
			AssertEquals(db.ReadMergeHeads()[1], ObjectId.FromString(sampleId));
			db.WriteMergeHeads(Collections.EMPTY_LIST);
			NUnit.Framework.Assert.AreEqual(Read(new FilePath(db.Directory, "MERGE_HEAD")), string.Empty
				);
			NUnit.Framework.Assert.AreEqual(db.ReadMergeHeads(), null);
			fos = new FileOutputStream(new FilePath(db.Directory, "MERGE_HEAD"));
			try
			{
				fos.Write(Sharpen.Runtime.GetBytesForString(sampleId, Constants.CHARACTER_ENCODING
					));
			}
			finally
			{
				fos.Close();
			}
			NUnit.Framework.Assert.AreEqual(db.ReadMergeHeads().Count, 1);
			AssertEquals(db.ReadMergeHeads()[0], ObjectId.FromString(sampleId));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestReadWriteMergeMsg()
		{
			NUnit.Framework.Assert.AreEqual(db.ReadMergeCommitMsg(), null);
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "MERGE_MSG").Exists());
			db.WriteMergeCommitMsg(mergeMsg);
			NUnit.Framework.Assert.AreEqual(db.ReadMergeCommitMsg(), mergeMsg);
			NUnit.Framework.Assert.AreEqual(Read(new FilePath(db.Directory, "MERGE_MSG")), mergeMsg
				);
			db.WriteMergeCommitMsg(null);
			NUnit.Framework.Assert.AreEqual(db.ReadMergeCommitMsg(), null);
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "MERGE_MSG").Exists());
			FileOutputStream fos = new FileOutputStream(new FilePath(db.Directory, Constants.
				MERGE_MSG));
			try
			{
				fos.Write(Sharpen.Runtime.GetBytesForString(mergeMsg, Constants.CHARACTER_ENCODING
					));
			}
			finally
			{
				fos.Close();
			}
			NUnit.Framework.Assert.AreEqual(db.ReadMergeCommitMsg(), mergeMsg);
		}
	}
}
