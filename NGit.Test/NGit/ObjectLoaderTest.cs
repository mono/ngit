using NGit;
using NGit.Errors;
using NGit.Junit;
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	public class ObjectLoaderTest : TestCase
	{
		private TestRng rng;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			rng = new TestRng(Sharpen.Extensions.GetTestName(this));
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestSmallObjectLoader()
		{
			byte[] act = rng.NextBytes(512);
			ObjectLoader ldr = new ObjectLoader.SmallObject(Constants.OBJ_BLOB, act);
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_BLOB, ldr.GetType());
			NUnit.Framework.Assert.AreEqual(act.Length, ldr.GetSize());
			NUnit.Framework.Assert.IsFalse("not is large", ldr.IsLarge());
			NUnit.Framework.Assert.AreSame(act, ldr.GetCachedBytes());
			NUnit.Framework.Assert.AreSame(act, ldr.GetCachedBytes(1));
			NUnit.Framework.Assert.AreSame(act, ldr.GetCachedBytes(int.MaxValue));
			byte[] copy = ldr.GetBytes();
			NUnit.Framework.Assert.AreNotSame(act, copy);
			NUnit.Framework.Assert.IsTrue("same content", Arrays.Equals(act, copy));
			copy = ldr.GetBytes(1);
			NUnit.Framework.Assert.AreNotSame(act, copy);
			NUnit.Framework.Assert.IsTrue("same content", Arrays.Equals(act, copy));
			copy = ldr.GetBytes(int.MaxValue);
			NUnit.Framework.Assert.AreNotSame(act, copy);
			NUnit.Framework.Assert.IsTrue("same content", Arrays.Equals(act, copy));
			ObjectStream @in = ldr.OpenStream();
			NUnit.Framework.Assert.IsNotNull("has stream", @in);
			NUnit.Framework.Assert.IsTrue("is small stream", @in is ObjectStream.SmallStream);
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_BLOB, @in.GetType());
			NUnit.Framework.Assert.AreEqual(act.Length, @in.GetSize());
			NUnit.Framework.Assert.AreEqual(act.Length, @in.Available());
			NUnit.Framework.Assert.IsTrue("mark supported", @in.MarkSupported());
			copy = new byte[act.Length];
			NUnit.Framework.Assert.AreEqual(act.Length, @in.Read(copy));
			NUnit.Framework.Assert.AreEqual(0, @in.Available());
			NUnit.Framework.Assert.AreEqual(-1, @in.Read());
			NUnit.Framework.Assert.IsTrue("same content", Arrays.Equals(act, copy));
			ByteArrayOutputStream tmp = new ByteArrayOutputStream();
			ldr.CopyTo(tmp);
			NUnit.Framework.Assert.IsTrue("same content", Arrays.Equals(act, tmp.ToByteArray(
				)));
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestLargeObjectLoader()
		{
			byte[] act = rng.NextBytes(512);
			ObjectLoader ldr = new _ObjectLoader_112(act);
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_BLOB, ldr.GetType());
			NUnit.Framework.Assert.AreEqual(act.Length, ldr.GetSize());
			NUnit.Framework.Assert.IsTrue("is large", ldr.IsLarge());
			try
			{
				ldr.GetCachedBytes();
				NUnit.Framework.Assert.Fail("did not throw on getCachedBytes()");
			}
			catch (LargeObjectException)
			{
			}
			// expected
			try
			{
				ldr.GetBytes();
				NUnit.Framework.Assert.Fail("did not throw on getBytes()");
			}
			catch (LargeObjectException)
			{
			}
			// expected
			try
			{
				ldr.GetCachedBytes(64);
				NUnit.Framework.Assert.Fail("did not throw on getCachedBytes(64)");
			}
			catch (LargeObjectException)
			{
			}
			// expected
			byte[] copy = ldr.GetCachedBytes(1024);
			NUnit.Framework.Assert.AreNotSame(act, copy);
			NUnit.Framework.Assert.IsTrue("same content", Arrays.Equals(act, copy));
			ObjectStream @in = ldr.OpenStream();
			NUnit.Framework.Assert.IsNotNull("has stream", @in);
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_BLOB, @in.GetType());
			NUnit.Framework.Assert.AreEqual(act.Length, @in.GetSize());
			NUnit.Framework.Assert.AreEqual(act.Length, @in.Available());
			NUnit.Framework.Assert.IsTrue("mark supported", @in.MarkSupported());
			copy = new byte[act.Length];
			NUnit.Framework.Assert.AreEqual(act.Length, @in.Read(copy));
			NUnit.Framework.Assert.AreEqual(0, @in.Available());
			NUnit.Framework.Assert.AreEqual(-1, @in.Read());
			NUnit.Framework.Assert.IsTrue("same content", Arrays.Equals(act, copy));
			ByteArrayOutputStream tmp = new ByteArrayOutputStream();
			ldr.CopyTo(tmp);
			NUnit.Framework.Assert.IsTrue("same content", Arrays.Equals(act, tmp.ToByteArray(
				)));
		}

		private sealed class _ObjectLoader_112 : ObjectLoader
		{
			public _ObjectLoader_112(byte[] act)
			{
				this.act = act;
			}

			/// <exception cref="NGit.Errors.LargeObjectException"></exception>
			public override byte[] GetCachedBytes()
			{
				throw new LargeObjectException();
			}

			public override long GetSize()
			{
				return act.Length;
			}

			public override int GetType()
			{
				return Constants.OBJ_BLOB;
			}

			/// <exception cref="NGit.Errors.MissingObjectException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override ObjectStream OpenStream()
			{
				return new ObjectStream.Filter(this.GetType(), act.Length, new ByteArrayInputStream
					(act));
			}

			private readonly byte[] act;
		}

		/// <exception cref="NGit.Errors.LargeObjectException"></exception>
		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestLimitedGetCachedBytes()
		{
			byte[] act = rng.NextBytes(512);
			ObjectLoader ldr = new _SmallObject_185(Constants.OBJ_BLOB, act);
			NUnit.Framework.Assert.IsTrue("is large", ldr.IsLarge());
			try
			{
				ldr.GetCachedBytes(10);
				NUnit.Framework.Assert.Fail("Did not throw LargeObjectException");
			}
			catch (LargeObjectException)
			{
			}
			// Expected result.
			byte[] copy = ldr.GetCachedBytes(512);
			NUnit.Framework.Assert.AreNotSame(act, copy);
			NUnit.Framework.Assert.IsTrue("same content", Arrays.Equals(act, copy));
			copy = ldr.GetCachedBytes(1024);
			NUnit.Framework.Assert.AreNotSame(act, copy);
			NUnit.Framework.Assert.IsTrue("same content", Arrays.Equals(act, copy));
		}

		private sealed class _SmallObject_185 : ObjectLoader.SmallObject
		{
			public _SmallObject_185(int baseArg1, byte[] baseArg2) : base(baseArg1, baseArg2)
			{
			}

			public override bool IsLarge()
			{
				return true;
			}
		}

		/// <exception cref="NGit.Errors.LargeObjectException"></exception>
		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestLimitedGetCachedBytesExceedsJavaLimits()
		{
			ObjectLoader ldr = new _ObjectLoader_211();
			NUnit.Framework.Assert.IsTrue("is large", ldr.IsLarge());
			try
			{
				ldr.GetCachedBytes(10);
				NUnit.Framework.Assert.Fail("Did not throw LargeObjectException");
			}
			catch (LargeObjectException)
			{
			}
			// Expected result.
			try
			{
				ldr.GetCachedBytes(int.MaxValue);
				NUnit.Framework.Assert.Fail("Did not throw LargeObjectException");
			}
			catch (LargeObjectException)
			{
			}
		}

		private sealed class _ObjectLoader_211 : ObjectLoader
		{
			public _ObjectLoader_211()
			{
			}

			public override bool IsLarge()
			{
				return true;
			}

			/// <exception cref="NGit.Errors.LargeObjectException"></exception>
			public override byte[] GetCachedBytes()
			{
				throw new LargeObjectException();
			}

			public override long GetSize()
			{
				return long.MaxValue;
			}

			public override int GetType()
			{
				return Constants.OBJ_BLOB;
			}

			/// <exception cref="NGit.Errors.MissingObjectException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override ObjectStream OpenStream()
			{
				return new _ObjectStream_235();
			}

			private sealed class _ObjectStream_235 : ObjectStream
			{
				public _ObjectStream_235()
				{
				}

				public override long GetSize()
				{
					return long.MaxValue;
				}

				public override int GetType()
				{
					return Constants.OBJ_BLOB;
				}

				/// <exception cref="System.IO.IOException"></exception>
				public override int Read()
				{
					NUnit.Framework.Assert.Fail("never should have reached read");
					return -1;
				}
			}
		}
		// Expected result.
	}
}
