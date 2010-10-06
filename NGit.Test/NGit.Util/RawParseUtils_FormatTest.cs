using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class RawParseUtils_FormatTest : TestCase
	{
		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		public virtual void TestFormatBase10()
		{
			byte[] b = new byte[64];
			int p;
			p = RawParseUtils.FormatBase10(b, b.Length, 0);
			NUnit.Framework.Assert.AreEqual("0", Sharpen.Extensions.CreateString(b, p, b.Length
				 - p, "UTF-8"));
			p = RawParseUtils.FormatBase10(b, b.Length, 42);
			NUnit.Framework.Assert.AreEqual("42", Sharpen.Extensions.CreateString(b, p, b.Length
				 - p, "UTF-8"));
			p = RawParseUtils.FormatBase10(b, b.Length, 1234);
			NUnit.Framework.Assert.AreEqual("1234", Sharpen.Extensions.CreateString(b, p, b.Length
				 - p, "UTF-8"));
			p = RawParseUtils.FormatBase10(b, b.Length, -9876);
			NUnit.Framework.Assert.AreEqual("-9876", Sharpen.Extensions.CreateString(b, p, b.
				Length - p, "UTF-8"));
			p = RawParseUtils.FormatBase10(b, b.Length, 123456789);
			NUnit.Framework.Assert.AreEqual("123456789", Sharpen.Extensions.CreateString(b, p
				, b.Length - p, "UTF-8"));
		}
	}
}
