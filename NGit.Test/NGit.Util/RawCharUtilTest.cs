using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class RawCharUtilTest : TestCase
	{
		/// <summary>
		/// Test method for
		/// <see cref="RawCharUtil.IsWhitespace(byte)">RawCharUtil.IsWhitespace(byte)</see>
		/// .
		/// </summary>
		public virtual void TestIsWhitespace()
		{
			for (byte c = unchecked((byte)(-128)); ((sbyte)c) < 127; c++)
			{
				switch (c)
				{
					case unchecked((byte)(byte)('\r')):
					case unchecked((byte)(byte)('\n')):
					case unchecked((byte)(byte)('\t')):
					case unchecked((byte)(byte)(' ')):
					{
						NUnit.Framework.Assert.IsTrue(RawCharUtil.IsWhitespace(c));
						break;
					}

					default:
					{
						NUnit.Framework.Assert.IsFalse(RawCharUtil.IsWhitespace(c));
						break;
					}
				}
			}
		}

		/// <summary>
		/// Test method for
		/// <see cref="RawCharUtil.TrimTrailingWhitespace(byte[], int, int)">RawCharUtil.TrimTrailingWhitespace(byte[], int, int)
		/// 	</see>
		/// .
		/// </summary>
		/// <exception cref="Sharpen.UnsupportedEncodingException">Sharpen.UnsupportedEncodingException
		/// 	</exception>
		public virtual void TestTrimTrailingWhitespace()
		{
			NUnit.Framework.Assert.AreEqual(0, RawCharUtil.TrimTrailingWhitespace(Sharpen.Runtime.GetBytesForString
				(string.Empty, "US-ASCII"), 0, 0));
			NUnit.Framework.Assert.AreEqual(0, RawCharUtil.TrimTrailingWhitespace(Sharpen.Runtime.GetBytesForString
				(" ", "US-ASCII"), 0, 1));
			NUnit.Framework.Assert.AreEqual(1, RawCharUtil.TrimTrailingWhitespace(Sharpen.Runtime.GetBytesForString
				("a ", "US-ASCII"), 0, 2));
			NUnit.Framework.Assert.AreEqual(2, RawCharUtil.TrimTrailingWhitespace(Sharpen.Runtime.GetBytesForString
				(" a ", "US-ASCII"), 0, 3));
			NUnit.Framework.Assert.AreEqual(3, RawCharUtil.TrimTrailingWhitespace(Sharpen.Runtime.GetBytesForString
				("  a", "US-ASCII"), 0, 3));
			NUnit.Framework.Assert.AreEqual(6, RawCharUtil.TrimTrailingWhitespace(Sharpen.Runtime.GetBytesForString
				("  test   ", "US-ASCII"), 2, 9));
		}

		/// <summary>
		/// Test method for
		/// <see cref="RawCharUtil.TrimLeadingWhitespace(byte[], int, int)">RawCharUtil.TrimLeadingWhitespace(byte[], int, int)
		/// 	</see>
		/// .
		/// </summary>
		/// <exception cref="Sharpen.UnsupportedEncodingException">Sharpen.UnsupportedEncodingException
		/// 	</exception>
		public virtual void TestTrimLeadingWhitespace()
		{
			NUnit.Framework.Assert.AreEqual(0, RawCharUtil.TrimLeadingWhitespace(Sharpen.Runtime.GetBytesForString
				(string.Empty, "US-ASCII"), 0, 0));
			NUnit.Framework.Assert.AreEqual(1, RawCharUtil.TrimLeadingWhitespace(Sharpen.Runtime.GetBytesForString
				(" ", "US-ASCII"), 0, 1));
			NUnit.Framework.Assert.AreEqual(0, RawCharUtil.TrimLeadingWhitespace(Sharpen.Runtime.GetBytesForString
				("a ", "US-ASCII"), 0, 2));
			NUnit.Framework.Assert.AreEqual(1, RawCharUtil.TrimLeadingWhitespace(Sharpen.Runtime.GetBytesForString
				(" a ", "US-ASCII"), 0, 3));
			NUnit.Framework.Assert.AreEqual(2, RawCharUtil.TrimLeadingWhitespace(Sharpen.Runtime.GetBytesForString
				("  a", "US-ASCII"), 0, 3));
			NUnit.Framework.Assert.AreEqual(2, RawCharUtil.TrimLeadingWhitespace(Sharpen.Runtime.GetBytesForString
				("  test   ", "US-ASCII"), 2, 9));
		}
	}
}
