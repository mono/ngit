using System;
using NGit;
using NGit.Transport;
using NUnit.Framework;
using Sharpen;

namespace NGit.Transport
{
	public class SideBandOutputStreamTest : TestCase
	{
		private ByteArrayOutputStream rawOut;

		// Note, test vectors created with:
		//
		// perl -e 'printf "%4.4x%s\n", 4+length($ARGV[0]),$ARGV[0]'
		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			rawOut = new ByteArrayOutputStream();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWrite_CH_DATA()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_DATA, SideBandOutputStream
				.SMALL_BUF, rawOut);
			@out.Write(new byte[] { (byte)('a'), (byte)('b'), (byte)('c') });
			@out.Flush();
			AssertBuffer("0008\x1abc");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWrite_CH_PROGRESS()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_PROGRESS, SideBandOutputStream
				.SMALL_BUF, rawOut);
			@out.Write(new byte[] { (byte)('a'), (byte)('b'), (byte)('c') });
			@out.Flush();
			AssertBuffer("0008\x2abc");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWrite_CH_ERROR()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_ERROR, SideBandOutputStream
				.SMALL_BUF, rawOut);
			@out.Write(new byte[] { (byte)('a'), (byte)('b'), (byte)('c') });
			@out.Flush();
			AssertBuffer("0008\x3abc");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWrite_Small()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_DATA, SideBandOutputStream
				.SMALL_BUF, rawOut);
			@out.Write('a');
			@out.Write('b');
			@out.Write('c');
			@out.Flush();
			AssertBuffer("0008\x1abc");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWrite_SmallBlocks1()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_DATA, 6, rawOut);
			@out.Write('a');
			@out.Write('b');
			@out.Write('c');
			@out.Flush();
			AssertBuffer("0006\x1a0006\x1b0006\x1c");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWrite_SmallBlocks2()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_DATA, 6, rawOut);
			@out.Write(new byte[] { (byte)('a'), (byte)('b'), (byte)('c') });
			@out.Flush();
			AssertBuffer("0006\x1a0006\x1b0006\x1c");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWrite_SmallBlocks3()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_DATA, 7, rawOut);
			@out.Write('a');
			@out.Write(new byte[] { (byte)('b'), (byte)('c') });
			@out.Flush();
			AssertBuffer("0007\x1ab0006\x1c");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWrite_Large()
		{
			int buflen = SideBandOutputStream.MAX_BUF - SideBandOutputStream.HDR_SIZE;
			byte[] buf = new byte[buflen];
			for (int i = 0; i < buf.Length; i++)
			{
				buf[i] = unchecked((byte)i);
			}
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_DATA, SideBandOutputStream
				.MAX_BUF, rawOut);
			@out.Write(buf);
			@out.Flush();
			byte[] act = rawOut.ToByteArray();
			string explen = Sharpen.Extensions.ToString(buf.Length + SideBandOutputStream.HDR_SIZE
				, 16);
			NUnit.Framework.Assert.AreEqual(SideBandOutputStream.HDR_SIZE + buf.Length, act.Length
				);
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.CreateString(act, 0, 4, "UTF-8"
				), explen);
			NUnit.Framework.Assert.AreEqual(1, act[4]);
			for (int i_1 = 0; i_1 < buf.Length; i_1++, j++)
			{
				NUnit.Framework.Assert.AreEqual(buf[i_1], act[j]);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestFlush()
		{
			int[] flushCnt = new int[1];
			OutputStream mockout = new _OutputStream_160(flushCnt);
			new SideBandOutputStream(SideBandOutputStream.CH_DATA, SideBandOutputStream.SMALL_BUF
				, mockout).Flush();
			NUnit.Framework.Assert.AreEqual(1, flushCnt[0]);
		}

		private sealed class _OutputStream_160 : OutputStream
		{
			public _OutputStream_160(int[] flushCnt)
			{
				this.flushCnt = flushCnt;
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Write(int arg0)
			{
				NUnit.Framework.Assert.Fail("should not write");
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Flush()
			{
				flushCnt[0]++;
			}

			private readonly int[] flushCnt;
		}

		public virtual void TestConstructor_RejectsBadChannel()
		{
			try
			{
				new SideBandOutputStream(-1, SideBandOutputStream.MAX_BUF, rawOut);
				NUnit.Framework.Assert.Fail("Accepted -1 channel number");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("channel -1 must be in range [0, 255]", e.Message
					);
			}
			try
			{
				new SideBandOutputStream(0, SideBandOutputStream.MAX_BUF, rawOut);
				NUnit.Framework.Assert.Fail("Accepted 0 channel number");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("channel 0 must be in range [0, 255]", e.Message);
			}
			try
			{
				new SideBandOutputStream(256, SideBandOutputStream.MAX_BUF, rawOut);
				NUnit.Framework.Assert.Fail("Accepted 256 channel number");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("channel 256 must be in range [0, 255]", e.Message
					);
			}
		}

		public virtual void TestConstructor_RejectsBadBufferSize()
		{
			try
			{
				new SideBandOutputStream(SideBandOutputStream.CH_DATA, -1, rawOut);
				NUnit.Framework.Assert.Fail("Accepted -1 for buffer size");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("packet size -1 must be >= 5", e.Message);
			}
			try
			{
				new SideBandOutputStream(SideBandOutputStream.CH_DATA, 0, rawOut);
				NUnit.Framework.Assert.Fail("Accepted 0 for buffer size");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("packet size 0 must be >= 5", e.Message);
			}
			try
			{
				new SideBandOutputStream(SideBandOutputStream.CH_DATA, 1, rawOut);
				NUnit.Framework.Assert.Fail("Accepted 1 for buffer size");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("packet size 1 must be >= 5", e.Message);
			}
			try
			{
				new SideBandOutputStream(SideBandOutputStream.CH_DATA, int.MaxValue, rawOut);
				NUnit.Framework.Assert.Fail("Accepted " + int.MaxValue + " for buffer size");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().packetSizeMustBeAtMost
					, int.MaxValue, 65520), e.Message);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertBuffer(string exp)
		{
			NUnit.Framework.Assert.AreEqual(exp, Sharpen.Extensions.CreateString(rawOut.ToByteArray
				(), Constants.CHARACTER_ENCODING));
		}
	}
}
