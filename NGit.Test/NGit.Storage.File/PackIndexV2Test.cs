using NGit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	public class PackIndexV2Test : PackIndexTestCase
	{
		public override FilePath GetFileForPack34be9032()
		{
			return JGitTestUtil.GetTestResourceFile("pack-34be9032ac282b11fa9babdc2b2a93ca996c9c2f.idxV2"
				);
		}

		public override FilePath GetFileForPackdf2982f28()
		{
			return JGitTestUtil.GetTestResourceFile("pack-df2982f284bbabb6bdb59ee3fcc6eb0983e20371.idxV2"
				);
		}

		/// <summary>Verify CRC32 indexing.</summary>
		/// <remarks>Verify CRC32 indexing.</remarks>
		/// <exception cref="System.NotSupportedException">System.NotSupportedException</exception>
		/// <exception cref="NGit.Errors.MissingObjectException">NGit.Errors.MissingObjectException
		/// 	</exception>
		public override void TestCRC32()
		{
			NUnit.Framework.Assert.IsTrue(smallIdx.HasCRC32Support());
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x00000000C2B64258l)), smallIdx.
				FindCRC32(ObjectId.FromString("4b825dc642cb6eb9a060e54bf8d69288fbee4904")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x0000000072AD57C2l)), smallIdx.
				FindCRC32(ObjectId.FromString("540a36d136cf413e4b064c2b0e0a4db60f77feab")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x00000000FF10A479l)), smallIdx.
				FindCRC32(ObjectId.FromString("5b6e7c66c276e7610d4a73c70ec1a1f7c1003259")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x0000000034B27DDCl)), smallIdx.
				FindCRC32(ObjectId.FromString("6ff87c4664981e4397625791c8ea3bbb5f2279a3")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x000000004743F1E4l)), smallIdx.
				FindCRC32(ObjectId.FromString("82c6b885ff600be425b4ea96dee75dca255b69e7")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x00000000640B358Bl)), smallIdx.
				FindCRC32(ObjectId.FromString("902d5476fa249b7abc9d84c611577a81381f0327")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x000000002A17CB5El)), smallIdx.
				FindCRC32(ObjectId.FromString("aabf2ffaec9b497f0950352b3e582d73035c2035")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x000000000B3B5BA6l)), smallIdx.
				FindCRC32(ObjectId.FromString("c59759f143fb1fe21c197981df75a7ee00290799")));
		}
	}
}
