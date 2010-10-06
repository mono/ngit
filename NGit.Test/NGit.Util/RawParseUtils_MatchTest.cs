using NGit;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class RawParseUtils_MatchTest : TestCase
	{
		public virtual void TestMatch_Equal()
		{
			byte[] src = Constants.EncodeASCII(" differ\n");
			byte[] dst = Constants.EncodeASCII("foo differ\n");
			NUnit.Framework.Assert.IsTrue(RawParseUtils.Match(dst, 3, src) == 3 + src.Length);
		}

		public virtual void TestMatch_NotEqual()
		{
			byte[] src = Constants.EncodeASCII(" differ\n");
			byte[] dst = Constants.EncodeASCII("a differ\n");
			NUnit.Framework.Assert.IsTrue(RawParseUtils.Match(dst, 2, src) < 0);
		}

		public virtual void TestMatch_Prefix()
		{
			byte[] src = Constants.EncodeASCII("author ");
			byte[] dst = Constants.EncodeASCII("author A. U. Thor");
			NUnit.Framework.Assert.IsTrue(RawParseUtils.Match(dst, 0, src) == src.Length);
			NUnit.Framework.Assert.IsTrue(RawParseUtils.Match(dst, 1, src) < 0);
		}

		public virtual void TestMatch_TooSmall()
		{
			byte[] src = Constants.EncodeASCII("author ");
			byte[] dst = Constants.EncodeASCII("author autho");
			NUnit.Framework.Assert.IsTrue(RawParseUtils.Match(dst, src.Length + 1, src) < 0);
		}
	}
}
