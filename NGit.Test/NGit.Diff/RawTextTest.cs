using System.Text;
using NGit;
using NGit.Diff;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Diff
{
	public class RawTextTest : TestCase
	{
		public virtual void TestEmpty()
		{
			RawText r = new RawText(new byte[0]);
			NUnit.Framework.Assert.AreEqual(0, r.Size());
		}

		public virtual void TestEquals()
		{
			RawText a = new RawText(Constants.EncodeASCII("foo-a\nfoo-b\n"));
			RawText b = new RawText(Constants.EncodeASCII("foo-b\nfoo-c\n"));
			RawTextComparator cmp = RawTextComparator.DEFAULT;
			NUnit.Framework.Assert.AreEqual(2, a.Size());
			NUnit.Framework.Assert.AreEqual(2, b.Size());
			// foo-a != foo-b
			NUnit.Framework.Assert.IsFalse(cmp.Equals(a, 0, b, 0));
			NUnit.Framework.Assert.IsFalse(cmp.Equals(b, 0, a, 0));
			// foo-b == foo-b
			NUnit.Framework.Assert.IsTrue(cmp.Equals(a, 1, b, 0));
			NUnit.Framework.Assert.IsTrue(cmp.Equals(b, 0, a, 1));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWriteLine1()
		{
			RawText a = new RawText(Constants.EncodeASCII("foo-a\nfoo-b\n"));
			ByteArrayOutputStream o = new ByteArrayOutputStream();
			a.WriteLine(o, 0);
			byte[] r = o.ToByteArray();
			NUnit.Framework.Assert.AreEqual("foo-a", RawParseUtils.Decode(r));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWriteLine2()
		{
			RawText a = new RawText(Constants.EncodeASCII("foo-a\nfoo-b"));
			ByteArrayOutputStream o = new ByteArrayOutputStream();
			a.WriteLine(o, 1);
			byte[] r = o.ToByteArray();
			NUnit.Framework.Assert.AreEqual("foo-b", RawParseUtils.Decode(r));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWriteLine3()
		{
			RawText a = new RawText(Constants.EncodeASCII("a\n\nb\n"));
			ByteArrayOutputStream o = new ByteArrayOutputStream();
			a.WriteLine(o, 1);
			byte[] r = o.ToByteArray();
			NUnit.Framework.Assert.AreEqual(string.Empty, RawParseUtils.Decode(r));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		public virtual void TestComparatorReduceCommonStartEnd()
		{
			RawTextComparator c = RawTextComparator.DEFAULT;
			Edit e;
			e = c.ReduceCommonStartEnd(T(string.Empty), T(string.Empty), new Edit(0, 0, 0, 0)
				);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 0, 0, 0), e);
			e = c.ReduceCommonStartEnd(T("a"), T("b"), new Edit(0, 1, 0, 1));
			NUnit.Framework.Assert.AreEqual(new Edit(0, 1, 0, 1), e);
			e = c.ReduceCommonStartEnd(T("a"), T("a"), new Edit(0, 1, 0, 1));
			NUnit.Framework.Assert.AreEqual(new Edit(1, 1, 1, 1), e);
			e = c.ReduceCommonStartEnd(T("axB"), T("axC"), new Edit(0, 3, 0, 3));
			NUnit.Framework.Assert.AreEqual(new Edit(2, 3, 2, 3), e);
			e = c.ReduceCommonStartEnd(T("Bxy"), T("Cxy"), new Edit(0, 3, 0, 3));
			NUnit.Framework.Assert.AreEqual(new Edit(0, 1, 0, 1), e);
			e = c.ReduceCommonStartEnd(T("bc"), T("Abc"), new Edit(0, 2, 0, 3));
			NUnit.Framework.Assert.AreEqual(new Edit(0, 0, 0, 1), e);
			e = new Edit(0, 5, 0, 5);
			e = c.ReduceCommonStartEnd(T("abQxy"), T("abRxy"), e);
			NUnit.Framework.Assert.AreEqual(new Edit(2, 3, 2, 3), e);
			RawText a = new RawText(Sharpen.Runtime.GetBytesForString("p\na b\nQ\nc d\n", "UTF-8"
				));
			RawText b = new RawText(Sharpen.Runtime.GetBytesForString("p\na  b \nR\n c  d \n"
				, "UTF-8"));
			e = new Edit(0, 4, 0, 4);
			e = RawTextComparator.WS_IGNORE_ALL.ReduceCommonStartEnd(a, b, e);
			NUnit.Framework.Assert.AreEqual(new Edit(2, 3, 2, 3), e);
		}

		private static RawText T(string text)
		{
			StringBuilder r = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				r.Append(text[i]);
				r.Append('\n');
			}
			try
			{
				return new RawText(Sharpen.Runtime.GetBytesForString(r.ToString(), "UTF-8"));
			}
			catch (UnsupportedEncodingException e)
			{
				throw new RuntimeException(e);
			}
		}
	}
}
