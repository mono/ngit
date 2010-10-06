using System;
using NGit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	public class PackIndexV1Test : PackIndexTestCase
	{
		public override FilePath GetFileForPack34be9032()
		{
			return JGitTestUtil.GetTestResourceFile("pack-34be9032ac282b11fa9babdc2b2a93ca996c9c2f.idx"
				);
		}

		public override FilePath GetFileForPackdf2982f28()
		{
			return JGitTestUtil.GetTestResourceFile("pack-df2982f284bbabb6bdb59ee3fcc6eb0983e20371.idx"
				);
		}

		/// <summary>Verify CRC32 - V1 should not index anything.</summary>
		/// <remarks>Verify CRC32 - V1 should not index anything.</remarks>
		/// <exception cref="NGit.Errors.MissingObjectException">NGit.Errors.MissingObjectException
		/// 	</exception>
		public override void TestCRC32()
		{
			NUnit.Framework.Assert.IsFalse(smallIdx.HasCRC32Support());
			try
			{
				smallIdx.FindCRC32(ObjectId.FromString("4b825dc642cb6eb9a060e54bf8d69288fbee4904"
					));
				NUnit.Framework.Assert.Fail("index V1 shouldn't support CRC");
			}
			catch (NotSupportedException)
			{
			}
		}
		// expected
	}
}
