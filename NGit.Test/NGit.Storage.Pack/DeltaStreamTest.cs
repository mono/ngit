using NGit;
using NGit.Errors;
using NGit.Junit;
using NGit.Storage.Pack;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Storage.Pack
{
	public class DeltaStreamTest : TestCase
	{
		private TestRng rng;

		private ByteArrayOutputStream deltaBuf;

		private DeltaEncoder deltaEnc;

		private byte[] @base;

		private byte[] data;

		private int dataPtr;

		private byte[] delta;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			rng = new TestRng(Sharpen.Extensions.GetTestName(this));
			deltaBuf = new ByteArrayOutputStream();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestCopy_SingleOp()
		{
			Init((1 << 16) + 1, (1 << 8) + 1);
			Copy(0, data.Length);
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestCopy_MaxSize()
		{
			int max = (unchecked((int)(0xff)) << 16) + (unchecked((int)(0xff)) << 8) + unchecked(
				(int)(0xff));
			Init(1 + max, max);
			Copy(1, max);
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestCopy_64k()
		{
			Init(unchecked((int)(0x10000)) + 2, unchecked((int)(0x10000)) + 1);
			Copy(1, unchecked((int)(0x10000)));
			Copy(unchecked((int)(0x10001)), 1);
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestCopy_Gap()
		{
			Init(256, 8);
			Copy(4, 4);
			Copy(128, 4);
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestCopy_OutOfOrder()
		{
			Init((1 << 16) + 1, (1 << 16) + 1);
			Copy(1 << 8, 1 << 8);
			Copy(0, data.Length - dataPtr);
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestInsert_SingleOp()
		{
			Init((1 << 16) + 1, 2);
			Insert("hi");
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestInsertAndCopy()
		{
			Init(8, 512);
			Insert(new byte[127]);
			Insert(new byte[127]);
			Insert(new byte[127]);
			Insert(new byte[125]);
			Copy(2, 6);
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestSkip()
		{
			Init(32, 15);
			Copy(2, 2);
			Insert("ab");
			Insert("cd");
			Copy(4, 4);
			Copy(0, 2);
			Insert("efg");
			AssertValidState();
			for (int p = 0; p < data.Length; p++)
			{
				byte[] act = new byte[data.Length];
				System.Array.Copy(data, 0, act, 0, p);
				DeltaStream @in = Open();
				IOUtil.SkipFully(@in, p);
				NUnit.Framework.Assert.AreEqual(data.Length - p, @in.Read(act, p, data.Length - p
					));
				NUnit.Framework.Assert.AreEqual(-1, @in.Read());
				NUnit.Framework.Assert.IsTrue("skipping " + p, Arrays.Equals(data, act));
			}
			// Skip all the way to the end should still recognize EOF.
			DeltaStream in_1 = Open();
			IOUtil.SkipFully(in_1, data.Length);
			NUnit.Framework.Assert.AreEqual(-1, in_1.Read());
			NUnit.Framework.Assert.AreEqual(0, in_1.Skip(1));
			// Skip should not open the base as we move past it, but it
			// will open when we need to start copying data from it.
			bool[] opened = new bool[1];
			in_1 = new _DeltaStream_160(this, opened, new ByteArrayInputStream(delta));
			IOUtil.SkipFully(in_1, 7);
			NUnit.Framework.Assert.IsFalse("not yet open", opened[0]);
			NUnit.Framework.Assert.AreEqual(data[7], in_1.Read());
			NUnit.Framework.Assert.IsTrue("now open", opened[0]);
		}

		private sealed class _DeltaStream_160 : DeltaStream
		{
			public _DeltaStream_160(DeltaStreamTest _enclosing, bool[] opened, InputStream baseArg1
				) : base(baseArg1)
			{
				this._enclosing = _enclosing;
				this.opened = opened;
			}

			/// <exception cref="System.IO.IOException"></exception>
			protected override long GetBaseSize()
			{
				return this._enclosing.@base.Length;
			}

			/// <exception cref="System.IO.IOException"></exception>
			protected override InputStream OpenBase()
			{
				opened[0] = true;
				return new ByteArrayInputStream(this._enclosing.@base);
			}

			private readonly DeltaStreamTest _enclosing;

			private readonly bool[] opened;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestIncorrectBaseSize()
		{
			Init(4, 4);
			Copy(0, 4);
			AssertValidState();
			DeltaStream @in = new _DeltaStream_183(this, new ByteArrayInputStream(delta));
			try
			{
				@in.Read(new byte[4]);
				NUnit.Framework.Assert.Fail("did not throw an exception");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual(JGitText.Get().baseLengthIncorrect, e.Message);
			}
			@in = new _DeltaStream_201(new ByteArrayInputStream(delta));
			try
			{
				@in.Read(new byte[4]);
				NUnit.Framework.Assert.Fail("did not throw an exception");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual(JGitText.Get().baseLengthIncorrect, e.Message);
			}
		}

		private sealed class _DeltaStream_183 : DeltaStream
		{
			public _DeltaStream_183(DeltaStreamTest _enclosing, InputStream baseArg1) : base(
				baseArg1)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.IO.IOException"></exception>
			protected override long GetBaseSize()
			{
				return 128;
			}

			/// <exception cref="System.IO.IOException"></exception>
			protected override InputStream OpenBase()
			{
				return new ByteArrayInputStream(this._enclosing.@base);
			}

			private readonly DeltaStreamTest _enclosing;
		}

		private sealed class _DeltaStream_201 : DeltaStream
		{
			public _DeltaStream_201(InputStream baseArg1) : base(baseArg1)
			{
			}

			/// <exception cref="System.IO.IOException"></exception>
			protected override long GetBaseSize()
			{
				return 4;
			}

			/// <exception cref="System.IO.IOException"></exception>
			protected override InputStream OpenBase()
			{
				return new ByteArrayInputStream(new byte[0]);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Init(int baseSize, int dataSize)
		{
			@base = rng.NextBytes(baseSize);
			data = new byte[dataSize];
			deltaEnc = new DeltaEncoder(deltaBuf, baseSize, dataSize);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Copy(int offset, int len)
		{
			System.Array.Copy(@base, offset, data, dataPtr, len);
			deltaEnc.Copy(offset, len);
			NUnit.Framework.Assert.AreEqual(deltaBuf.Size(), deltaEnc.GetSize());
			dataPtr += len;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Insert(string text)
		{
			Insert(Constants.Encode(text));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Insert(byte[] text)
		{
			System.Array.Copy(text, 0, data, dataPtr, text.Length);
			deltaEnc.Insert(text);
			NUnit.Framework.Assert.AreEqual(deltaBuf.Size(), deltaEnc.GetSize());
			dataPtr += text.Length;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertValidState()
		{
			NUnit.Framework.Assert.AreEqual("test filled example result", data.Length, dataPtr
				);
			delta = deltaBuf.ToByteArray();
			NUnit.Framework.Assert.AreEqual(@base.Length, BinaryDelta.GetBaseSize(delta));
			NUnit.Framework.Assert.AreEqual(data.Length, BinaryDelta.GetResultSize(delta));
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(data, BinaryDelta.Apply(@base, delta)
				));
			// Assert that a single bulk read produces the correct result.
			//
			byte[] act = new byte[data.Length];
			DeltaStream @in = Open();
			NUnit.Framework.Assert.AreEqual(data.Length, @in.GetSize());
			NUnit.Framework.Assert.AreEqual(data.Length, @in.Read(act));
			NUnit.Framework.Assert.AreEqual(-1, @in.Read());
			NUnit.Framework.Assert.IsTrue("bulk read has same content", Arrays.Equals(data, act
				));
			// Assert that smaller tiny reads have the same result too.
			//
			act = new byte[data.Length];
			@in = Open();
			int read = 0;
			while (read < data.Length)
			{
				int n = @in.Read(act, read, 128);
				if (n <= 0)
				{
					break;
				}
				read += n;
			}
			NUnit.Framework.Assert.AreEqual(data.Length, read);
			NUnit.Framework.Assert.AreEqual(-1, @in.Read());
			NUnit.Framework.Assert.IsTrue("small reads have same content", Arrays.Equals(data
				, act));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private DeltaStream Open()
		{
			return new _DeltaStream_278(this, new ByteArrayInputStream(delta));
		}

		private sealed class _DeltaStream_278 : DeltaStream
		{
			public _DeltaStream_278(DeltaStreamTest _enclosing, InputStream baseArg1) : base(
				baseArg1)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.IO.IOException"></exception>
			protected override long GetBaseSize()
			{
				return this._enclosing.@base.Length;
			}

			/// <exception cref="System.IO.IOException"></exception>
			protected override InputStream OpenBase()
			{
				return new ByteArrayInputStream(this._enclosing.@base);
			}

			private readonly DeltaStreamTest _enclosing;
		}
	}
}
