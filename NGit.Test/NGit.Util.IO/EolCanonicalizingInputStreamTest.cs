using System;
using NGit.Util.IO;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util.IO
{
	public class EolCanonicalizingInputStreamTest : TestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestLF()
		{
			byte[] bytes = AsBytes("1\n2\n3");
			Test(bytes, bytes);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestCR()
		{
			byte[] bytes = AsBytes("1\r2\r3");
			Test(bytes, bytes);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestCRLF()
		{
			Test(AsBytes("1\r\n2\r\n3"), AsBytes("1\n2\n3"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestLFCR()
		{
			byte[] bytes = AsBytes("1\n\r2\n\r3");
			Test(bytes, bytes);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Test(byte[] input, byte[] expected)
		{
			InputStream bis1 = new ByteArrayInputStream(input);
			InputStream cis1 = new EolCanonicalizingInputStream(bis1);
			int index1 = 0;
			for (int b = cis1.Read(); b != -1; b = cis1.Read())
			{
				NUnit.Framework.Assert.AreEqual(expected[index1], unchecked((byte)b));
				index1++;
			}
			NUnit.Framework.Assert.AreEqual(expected.Length, index1);
			for (int bufferSize = 1; bufferSize < 10; bufferSize++)
			{
				byte[] buffer = new byte[bufferSize];
				InputStream bis2 = new ByteArrayInputStream(input);
				InputStream cis2 = new EolCanonicalizingInputStream(bis2);
				int read = 0;
				for (int readNow = cis2.Read(buffer, 0, buffer.Length); readNow != -1 && read < expected
					.Length; readNow = cis2.Read(buffer, 0, buffer.Length))
				{
					for (int index2 = 0; index2 < readNow; index2++)
					{
						NUnit.Framework.Assert.AreEqual(expected[read + index2], buffer[index2]);
					}
					read += readNow;
				}
				NUnit.Framework.Assert.AreEqual(expected.Length, read);
			}
		}

		private static byte[] AsBytes(string @in)
		{
			try
			{
				return Sharpen.Runtime.GetBytesForString(@in, "UTF-8");
			}
			catch (UnsupportedEncodingException)
			{
				throw new Exception();
			}
		}
	}
}
