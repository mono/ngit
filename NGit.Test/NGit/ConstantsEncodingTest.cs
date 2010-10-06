using System;
using NGit;
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	public class ConstantsEncodingTest : TestCase
	{
		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		public virtual void TestEncodeASCII_SimpleASCII()
		{
			string src = "abc";
			byte[] exp = new byte[] { (byte)('a'), (byte)('b'), (byte)('c') };
			byte[] res = Constants.EncodeASCII(src);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(exp, res));
			NUnit.Framework.Assert.AreEqual(src, Sharpen.Extensions.CreateString(res, 0, res.
				Length, "UTF-8"));
		}

		public virtual void TestEncodeASCII_FailOnNonASCII()
		{
			string src = "Ūnĭcōde̽";
			try
			{
				Constants.EncodeASCII(src);
				NUnit.Framework.Assert.Fail("Incorrectly accepted a Unicode character");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Not ASCII string: " + src, err.Message);
			}
		}

		public virtual void TestEncodeASCII_Number13()
		{
			long src = 13;
			byte[] exp = new byte[] { (byte)('1'), (byte)('3') };
			byte[] res = Constants.EncodeASCII(src);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(exp, res));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		public virtual void TestEncode_SimpleASCII()
		{
			string src = "abc";
			byte[] exp = new byte[] { (byte)('a'), (byte)('b'), (byte)('c') };
			byte[] res = Constants.Encode(src);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(exp, res));
			NUnit.Framework.Assert.AreEqual(src, Sharpen.Extensions.CreateString(res, 0, res.
				Length, "UTF-8"));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		public virtual void TestEncode_Unicode()
		{
			string src = "Ūnĭcōde̽";
			byte[] exp = new byte[] { unchecked((byte)unchecked((int)(0xC5))), unchecked((byte
				)unchecked((int)(0xAA))), unchecked((int)(0x6E)), unchecked((byte)unchecked((int
				)(0xC4))), unchecked((byte)unchecked((int)(0xAD))), unchecked((int)(0x63)), unchecked(
				(byte)unchecked((int)(0xC5))), unchecked((byte)unchecked((int)(0x8D))), unchecked(
				(int)(0x64)), unchecked((int)(0x65)), unchecked((byte)unchecked((int)(0xCC))), unchecked(
				(byte)unchecked((int)(0xBD))) };
			byte[] res = Constants.Encode(src);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(exp, res));
			NUnit.Framework.Assert.AreEqual(src, Sharpen.Extensions.CreateString(res, 0, res.
				Length, "UTF-8"));
		}
	}
}
