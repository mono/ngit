using System.IO;
using NGit.Util.IO;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util.IO
{
	public class UnionInputStreamTest : TestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestEmptyStream()
		{
			UnionInputStream u = new UnionInputStream();
			NUnit.Framework.Assert.IsTrue(u.IsEmpty());
			NUnit.Framework.Assert.AreEqual(-1, u.Read());
			NUnit.Framework.Assert.AreEqual(-1, u.Read(new byte[1], 0, 1));
			NUnit.Framework.Assert.AreEqual(0, u.Available());
			NUnit.Framework.Assert.AreEqual(0, u.Skip(1));
			u.Close();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestReadSingleBytes()
		{
			UnionInputStream u = new UnionInputStream();
			NUnit.Framework.Assert.IsTrue(u.IsEmpty());
			u.Add(new ByteArrayInputStream(new byte[] { 1, 0, 2 }));
			u.Add(new ByteArrayInputStream(new byte[] { 3 }));
			u.Add(new ByteArrayInputStream(new byte[] { 4, 5 }));
			NUnit.Framework.Assert.IsFalse(u.IsEmpty());
			NUnit.Framework.Assert.AreEqual(3, u.Available());
			NUnit.Framework.Assert.AreEqual(1, u.Read());
			NUnit.Framework.Assert.AreEqual(0, u.Read());
			NUnit.Framework.Assert.AreEqual(2, u.Read());
			NUnit.Framework.Assert.AreEqual(0, u.Available());
			NUnit.Framework.Assert.AreEqual(3, u.Read());
			NUnit.Framework.Assert.AreEqual(0, u.Available());
			NUnit.Framework.Assert.AreEqual(4, u.Read());
			NUnit.Framework.Assert.AreEqual(1, u.Available());
			NUnit.Framework.Assert.AreEqual(5, u.Read());
			NUnit.Framework.Assert.AreEqual(0, u.Available());
			NUnit.Framework.Assert.AreEqual(-1, u.Read());
			NUnit.Framework.Assert.IsTrue(u.IsEmpty());
			u.Add(new ByteArrayInputStream(new byte[] { unchecked((byte)255) }));
			NUnit.Framework.Assert.AreEqual(255, u.Read());
			NUnit.Framework.Assert.AreEqual(-1, u.Read());
			NUnit.Framework.Assert.IsTrue(u.IsEmpty());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestReadByteBlocks()
		{
			UnionInputStream u = new UnionInputStream();
			u.Add(new ByteArrayInputStream(new byte[] { 1, 0, 2 }));
			u.Add(new ByteArrayInputStream(new byte[] { 3 }));
			u.Add(new ByteArrayInputStream(new byte[] { 4, 5 }));
			byte[] r = new byte[5];
			NUnit.Framework.Assert.AreEqual(5, u.Read(r, 0, 5));
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(new byte[] { 1, 0, 2, 3, 4 }, r));
			NUnit.Framework.Assert.AreEqual(1, u.Read(r, 0, 5));
			NUnit.Framework.Assert.AreEqual(5, r[0]);
			NUnit.Framework.Assert.AreEqual(-1, u.Read(r, 0, 5));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestArrayConstructor()
		{
			UnionInputStream u = new UnionInputStream(new ByteArrayInputStream(new byte[] { 1
				, 0, 2 }), new ByteArrayInputStream(new byte[] { 3 }), new ByteArrayInputStream(
				new byte[] { 4, 5 }));
			byte[] r = new byte[5];
			NUnit.Framework.Assert.AreEqual(5, u.Read(r, 0, 5));
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(new byte[] { 1, 0, 2, 3, 4 }, r));
			NUnit.Framework.Assert.AreEqual(1, u.Read(r, 0, 5));
			NUnit.Framework.Assert.AreEqual(5, r[0]);
			NUnit.Framework.Assert.AreEqual(-1, u.Read(r, 0, 5));
		}

		public virtual void TestMarkSupported()
		{
			UnionInputStream u = new UnionInputStream();
			NUnit.Framework.Assert.IsFalse(u.MarkSupported());
			u.Add(new ByteArrayInputStream(new byte[] { 1, 0, 2 }));
			NUnit.Framework.Assert.IsFalse(u.MarkSupported());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestSkip()
		{
			UnionInputStream u = new UnionInputStream();
			u.Add(new ByteArrayInputStream(new byte[] { 1, 0, 2 }));
			u.Add(new ByteArrayInputStream(new byte[] { 3 }));
			u.Add(new ByteArrayInputStream(new byte[] { 4, 5 }));
			NUnit.Framework.Assert.AreEqual(0, u.Skip(0));
			NUnit.Framework.Assert.AreEqual(4, u.Skip(4));
			NUnit.Framework.Assert.AreEqual(4, u.Read());
			NUnit.Framework.Assert.AreEqual(1, u.Skip(5));
			NUnit.Framework.Assert.AreEqual(0, u.Skip(5));
			NUnit.Framework.Assert.AreEqual(-1, u.Read());
			u.Add(new _ByteArrayInputStream_141(new byte[] { 20, 30 }));
			NUnit.Framework.Assert.AreEqual(2, u.Skip(8));
			NUnit.Framework.Assert.AreEqual(-1, u.Read());
		}

		private sealed class _ByteArrayInputStream_141 : ByteArrayInputStream
		{
			public _ByteArrayInputStream_141(byte[] baseArg1) : base(baseArg1)
			{
			}

			public override long Skip(long n)
			{
				return 0;
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestAutoCloseDuringRead()
		{
			UnionInputStream u = new UnionInputStream();
			bool[] closed = new bool[2];
			u.Add(new _ByteArrayInputStream_153(closed, new byte[] { 1 }));
			u.Add(new _ByteArrayInputStream_158(closed, new byte[] { 2 }));
			NUnit.Framework.Assert.IsFalse(closed[0]);
			NUnit.Framework.Assert.IsFalse(closed[1]);
			NUnit.Framework.Assert.AreEqual(1, u.Read());
			NUnit.Framework.Assert.IsFalse(closed[0]);
			NUnit.Framework.Assert.IsFalse(closed[1]);
			NUnit.Framework.Assert.AreEqual(2, u.Read());
			NUnit.Framework.Assert.IsTrue(closed[0]);
			NUnit.Framework.Assert.IsFalse(closed[1]);
			NUnit.Framework.Assert.AreEqual(-1, u.Read());
			NUnit.Framework.Assert.IsTrue(closed[0]);
			NUnit.Framework.Assert.IsTrue(closed[1]);
		}

		private sealed class _ByteArrayInputStream_153 : ByteArrayInputStream
		{
			public _ByteArrayInputStream_153(bool[] closed, byte[] baseArg1) : base(baseArg1)
			{
				this.closed = closed;
			}

			public override void Close()
			{
				closed[0] = true;
			}

			private readonly bool[] closed;
		}

		private sealed class _ByteArrayInputStream_158 : ByteArrayInputStream
		{
			public _ByteArrayInputStream_158(bool[] closed, byte[] baseArg1) : base(baseArg1)
			{
				this.closed = closed;
			}

			public override void Close()
			{
				closed[1] = true;
			}

			private readonly bool[] closed;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestCloseDuringClose()
		{
			UnionInputStream u = new UnionInputStream();
			bool[] closed = new bool[2];
			u.Add(new _ByteArrayInputStream_183(closed, new byte[] { 1 }));
			u.Add(new _ByteArrayInputStream_188(closed, new byte[] { 2 }));
			NUnit.Framework.Assert.IsFalse(closed[0]);
			NUnit.Framework.Assert.IsFalse(closed[1]);
			u.Close();
			NUnit.Framework.Assert.IsTrue(closed[0]);
			NUnit.Framework.Assert.IsTrue(closed[1]);
		}

		private sealed class _ByteArrayInputStream_183 : ByteArrayInputStream
		{
			public _ByteArrayInputStream_183(bool[] closed, byte[] baseArg1) : base(baseArg1)
			{
				this.closed = closed;
			}

			public override void Close()
			{
				closed[0] = true;
			}

			private readonly bool[] closed;
		}

		private sealed class _ByteArrayInputStream_188 : ByteArrayInputStream
		{
			public _ByteArrayInputStream_188(bool[] closed, byte[] baseArg1) : base(baseArg1)
			{
				this.closed = closed;
			}

			public override void Close()
			{
				closed[1] = true;
			}

			private readonly bool[] closed;
		}

		public virtual void TestExceptionDuringClose()
		{
			UnionInputStream u = new UnionInputStream();
			u.Add(new _ByteArrayInputStream_205(new byte[] { 1 }));
			try
			{
				u.Close();
				NUnit.Framework.Assert.Fail("close ignored inner stream exception");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.AreEqual("I AM A TEST", e.Message);
			}
		}

		private sealed class _ByteArrayInputStream_205 : ByteArrayInputStream
		{
			public _ByteArrayInputStream_205(byte[] baseArg1) : base(baseArg1)
			{
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Close()
			{
				throw new IOException("I AM A TEST");
			}
		}
	}
}
