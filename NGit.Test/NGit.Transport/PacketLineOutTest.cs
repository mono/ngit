using NGit;
using NGit.Transport;
using NUnit.Framework;
using Sharpen;

namespace NGit.Transport
{
	public class PacketLineOutTest : TestCase
	{
		private ByteArrayOutputStream rawOut;

		private PacketLineOut @out;

		// Note, test vectors created with:
		//
		// perl -e 'printf "%4.4x%s\n", 4+length($ARGV[0]),$ARGV[0]'
		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			rawOut = new ByteArrayOutputStream();
			@out = new PacketLineOut(rawOut);
		}

		// writeString
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWriteString1()
		{
			@out.WriteString("a");
			@out.WriteString("bc");
			AssertBuffer("0005a0006bc");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWriteString2()
		{
			@out.WriteString("a\n");
			@out.WriteString("bc\n");
			AssertBuffer("0006a\n0007bc\n");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWriteString3()
		{
			@out.WriteString(string.Empty);
			AssertBuffer("0004");
		}

		// end
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWriteEnd()
		{
			int[] flushCnt = new int[1];
			OutputStream mockout = new _OutputStream_92(this, flushCnt);
			new PacketLineOut(mockout).End();
			AssertBuffer("0000");
			NUnit.Framework.Assert.AreEqual(1, flushCnt[0]);
		}

		private sealed class _OutputStream_92 : OutputStream
		{
			public _OutputStream_92(PacketLineOutTest _enclosing, int[] flushCnt)
			{
				this._enclosing = _enclosing;
				this.flushCnt = flushCnt;
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Write(int arg0)
			{
				this._enclosing.rawOut.Write(arg0);
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Flush()
			{
				flushCnt[0]++;
			}

			private readonly PacketLineOutTest _enclosing;

			private readonly int[] flushCnt;
		}

		// writePacket
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWritePacket1()
		{
			@out.WritePacket(new byte[] { (byte)('a') });
			AssertBuffer("0005a");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWritePacket2()
		{
			@out.WritePacket(new byte[] { (byte)('a'), (byte)('b'), (byte)('c'), (byte)('d') }
				);
			AssertBuffer("0008abcd");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWritePacket3()
		{
			int buflen = SideBandOutputStream.MAX_BUF - 5;
			byte[] buf = new byte[buflen];
			for (int i = 0; i < buf.Length; i++)
			{
				buf[i] = unchecked((byte)i);
			}
			@out.WritePacket(buf);
			@out.Flush();
			byte[] act = rawOut.ToByteArray();
			string explen = Sharpen.Extensions.ToString(buf.Length + 4, 16);
			NUnit.Framework.Assert.AreEqual(4 + buf.Length, act.Length);
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.CreateString(act, 0, 4, "UTF-8"
				), explen);
			for (int i_1 = 0; i_1 < buf.Length; i_1++, j++)
			{
				NUnit.Framework.Assert.AreEqual(buf[i_1], act[j]);
			}
		}

		// flush
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestFlush()
		{
			int[] flushCnt = new int[1];
			OutputStream mockout = new _OutputStream_143(flushCnt);
			new PacketLineOut(mockout).Flush();
			NUnit.Framework.Assert.AreEqual(1, flushCnt[0]);
		}

		private sealed class _OutputStream_143 : OutputStream
		{
			public _OutputStream_143(int[] flushCnt)
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

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertBuffer(string exp)
		{
			NUnit.Framework.Assert.AreEqual(exp, Sharpen.Extensions.CreateString(rawOut.ToByteArray
				(), Constants.CHARACTER_ENCODING));
		}
	}
}
