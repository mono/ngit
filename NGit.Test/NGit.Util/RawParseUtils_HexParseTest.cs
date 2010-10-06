using System;
using NGit;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class RawParseUtils_HexParseTest : TestCase
	{
		public virtual void TestInt4_1()
		{
			NUnit.Framework.Assert.AreEqual(0, RawParseUtils.ParseHexInt4(unchecked((byte)'0'
				)));
			NUnit.Framework.Assert.AreEqual(1, RawParseUtils.ParseHexInt4(unchecked((byte)'1'
				)));
			NUnit.Framework.Assert.AreEqual(2, RawParseUtils.ParseHexInt4(unchecked((byte)'2'
				)));
			NUnit.Framework.Assert.AreEqual(3, RawParseUtils.ParseHexInt4(unchecked((byte)'3'
				)));
			NUnit.Framework.Assert.AreEqual(4, RawParseUtils.ParseHexInt4(unchecked((byte)'4'
				)));
			NUnit.Framework.Assert.AreEqual(5, RawParseUtils.ParseHexInt4(unchecked((byte)'5'
				)));
			NUnit.Framework.Assert.AreEqual(6, RawParseUtils.ParseHexInt4(unchecked((byte)'6'
				)));
			NUnit.Framework.Assert.AreEqual(7, RawParseUtils.ParseHexInt4(unchecked((byte)'7'
				)));
			NUnit.Framework.Assert.AreEqual(8, RawParseUtils.ParseHexInt4(unchecked((byte)'8'
				)));
			NUnit.Framework.Assert.AreEqual(9, RawParseUtils.ParseHexInt4(unchecked((byte)'9'
				)));
			NUnit.Framework.Assert.AreEqual(10, RawParseUtils.ParseHexInt4(unchecked((byte)'a'
				)));
			NUnit.Framework.Assert.AreEqual(11, RawParseUtils.ParseHexInt4(unchecked((byte)'b'
				)));
			NUnit.Framework.Assert.AreEqual(12, RawParseUtils.ParseHexInt4(unchecked((byte)'c'
				)));
			NUnit.Framework.Assert.AreEqual(13, RawParseUtils.ParseHexInt4(unchecked((byte)'d'
				)));
			NUnit.Framework.Assert.AreEqual(14, RawParseUtils.ParseHexInt4(unchecked((byte)'e'
				)));
			NUnit.Framework.Assert.AreEqual(15, RawParseUtils.ParseHexInt4(unchecked((byte)'f'
				)));
			NUnit.Framework.Assert.AreEqual(10, RawParseUtils.ParseHexInt4(unchecked((byte)'A'
				)));
			NUnit.Framework.Assert.AreEqual(11, RawParseUtils.ParseHexInt4(unchecked((byte)'B'
				)));
			NUnit.Framework.Assert.AreEqual(12, RawParseUtils.ParseHexInt4(unchecked((byte)'C'
				)));
			NUnit.Framework.Assert.AreEqual(13, RawParseUtils.ParseHexInt4(unchecked((byte)'D'
				)));
			NUnit.Framework.Assert.AreEqual(14, RawParseUtils.ParseHexInt4(unchecked((byte)'E'
				)));
			NUnit.Framework.Assert.AreEqual(15, RawParseUtils.ParseHexInt4(unchecked((byte)'F'
				)));
			AssertNotHex('q');
			AssertNotHex(' ');
			AssertNotHex('.');
		}

		private static void AssertNotHex(char c)
		{
			try
			{
				RawParseUtils.ParseHexInt4(unchecked((byte)c));
				NUnit.Framework.Assert.Fail("Incorrectly acccepted " + c);
			}
			catch (IndexOutOfRangeException)
			{
			}
		}

		// pass
		public virtual void TestInt16()
		{
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x0000)), Parse16("0000"));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x0001)), Parse16("0001"));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x1234)), Parse16("1234"));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xdead)), Parse16("dead"));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xBEEF)), Parse16("BEEF"));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x4321)), Parse16("4321"));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xffff)), Parse16("ffff"));
			try
			{
				Parse16("noth");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"noth\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
			// pass
			try
			{
				Parse16("01");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"01\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
			// pass
			try
			{
				Parse16("000.");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"000.\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
		}

		// pass
		private static int Parse16(string str)
		{
			return RawParseUtils.ParseHexInt16(Constants.EncodeASCII(str), 0);
		}

		public virtual void TestInt32()
		{
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x00000000)), Parse32("00000000")
				);
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x00000001)), Parse32("00000001")
				);
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xc0ffEE42)), Parse32("c0ffEE42")
				);
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xffffffff)), Parse32("ffffffff")
				);
			NUnit.Framework.Assert.AreEqual(-1, Parse32("ffffffff"));
			try
			{
				Parse32("noth");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"noth\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
			// pass
			try
			{
				Parse32("notahexs");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"notahexs\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
			// pass
			try
			{
				Parse32("01");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"01\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
			// pass
			try
			{
				Parse32("0000000.");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"0000000.\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
		}

		// pass
		private static int Parse32(string str)
		{
			return RawParseUtils.ParseHexInt32(Constants.EncodeASCII(str), 0);
		}
	}
}
