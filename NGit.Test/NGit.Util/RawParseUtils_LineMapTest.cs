using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class RawParseUtils_LineMapTest : TestCase
	{
		public virtual void TestEmpty()
		{
			IntList map = RawParseUtils.LineMap(new byte[] {  }, 0, 0);
			NUnit.Framework.Assert.IsNotNull(map);
			NUnit.Framework.Assert.AreEqual(2, map.Size());
			NUnit.Framework.Assert.AreEqual(int.MinValue, map.Get(0));
			NUnit.Framework.Assert.AreEqual(0, map.Get(1));
		}

		public virtual void TestOneBlankLine()
		{
			IntList map = RawParseUtils.LineMap(new byte[] { (byte)('\n') }, 0, 1);
			NUnit.Framework.Assert.AreEqual(3, map.Size());
			NUnit.Framework.Assert.AreEqual(int.MinValue, map.Get(0));
			NUnit.Framework.Assert.AreEqual(0, map.Get(1));
			NUnit.Framework.Assert.AreEqual(1, map.Get(2));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		public virtual void TestTwoLineFooBar()
		{
			byte[] buf = Sharpen.Runtime.GetBytesForString("foo\nbar\n", "ISO-8859-1");
			IntList map = RawParseUtils.LineMap(buf, 0, buf.Length);
			NUnit.Framework.Assert.AreEqual(4, map.Size());
			NUnit.Framework.Assert.AreEqual(int.MinValue, map.Get(0));
			NUnit.Framework.Assert.AreEqual(0, map.Get(1));
			NUnit.Framework.Assert.AreEqual(4, map.Get(2));
			NUnit.Framework.Assert.AreEqual(buf.Length, map.Get(3));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		public virtual void TestTwoLineNoLF()
		{
			byte[] buf = Sharpen.Runtime.GetBytesForString("foo\nbar", "ISO-8859-1");
			IntList map = RawParseUtils.LineMap(buf, 0, buf.Length);
			NUnit.Framework.Assert.AreEqual(4, map.Size());
			NUnit.Framework.Assert.AreEqual(int.MinValue, map.Get(0));
			NUnit.Framework.Assert.AreEqual(0, map.Get(1));
			NUnit.Framework.Assert.AreEqual(4, map.Get(2));
			NUnit.Framework.Assert.AreEqual(buf.Length, map.Get(3));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		public virtual void TestFourLineBlanks()
		{
			byte[] buf = Sharpen.Runtime.GetBytesForString("foo\n\n\nbar\n", "ISO-8859-1");
			IntList map = RawParseUtils.LineMap(buf, 0, buf.Length);
			NUnit.Framework.Assert.AreEqual(6, map.Size());
			NUnit.Framework.Assert.AreEqual(int.MinValue, map.Get(0));
			NUnit.Framework.Assert.AreEqual(0, map.Get(1));
			NUnit.Framework.Assert.AreEqual(4, map.Get(2));
			NUnit.Framework.Assert.AreEqual(5, map.Get(3));
			NUnit.Framework.Assert.AreEqual(6, map.Get(4));
			NUnit.Framework.Assert.AreEqual(buf.Length, map.Get(5));
		}
	}
}
