using System;
using System.IO;
using NGit.Junit;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class TemporaryBufferTest : TestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestEmpty()
		{
			TemporaryBuffer b = new TemporaryBuffer.LocalFile();
			try
			{
				b.Close();
				NUnit.Framework.Assert.AreEqual(0, b.Length());
				byte[] r = b.ToByteArray();
				NUnit.Framework.Assert.IsNotNull(r);
				NUnit.Framework.Assert.AreEqual(0, r.Length);
			}
			finally
			{
				b.Destroy();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestOneByte()
		{
			TemporaryBuffer b = new TemporaryBuffer.LocalFile();
			byte test = unchecked((byte)new TestRng(Sharpen.Extensions.GetTestName(this)).NextInt
				());
			try
			{
				b.Write(test);
				b.Close();
				NUnit.Framework.Assert.AreEqual(1, b.Length());
				{
					byte[] r = b.ToByteArray();
					NUnit.Framework.Assert.IsNotNull(r);
					NUnit.Framework.Assert.AreEqual(1, r.Length);
					NUnit.Framework.Assert.AreEqual(test, r[0]);
				}
				{
					ByteArrayOutputStream o = new ByteArrayOutputStream();
					b.WriteTo(o, null);
					o.Close();
					byte[] r = o.ToByteArray();
					NUnit.Framework.Assert.AreEqual(1, r.Length);
					NUnit.Framework.Assert.AreEqual(test, r[0]);
				}
			}
			finally
			{
				b.Destroy();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestOneBlock_BulkWrite()
		{
			TemporaryBuffer b = new TemporaryBuffer.LocalFile();
			byte[] test = new TestRng(Sharpen.Extensions.GetTestName(this)).NextBytes(TemporaryBuffer.Block
				.SZ);
			try
			{
				b.Write(test, 0, 2);
				b.Write(test, 2, 4);
				b.Write(test, 6, test.Length - 6 - 2);
				b.Write(test, test.Length - 2, 2);
				b.Close();
				NUnit.Framework.Assert.AreEqual(test.Length, b.Length());
				{
					byte[] r = b.ToByteArray();
					NUnit.Framework.Assert.IsNotNull(r);
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
				{
					ByteArrayOutputStream o = new ByteArrayOutputStream();
					b.WriteTo(o, null);
					o.Close();
					byte[] r = o.ToByteArray();
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
			}
			finally
			{
				b.Destroy();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestOneBlockAndHalf_BulkWrite()
		{
			TemporaryBuffer b = new TemporaryBuffer.LocalFile();
			byte[] test = new TestRng(Sharpen.Extensions.GetTestName(this)).NextBytes(TemporaryBuffer.Block
				.SZ * 3 / 2);
			try
			{
				b.Write(test, 0, 2);
				b.Write(test, 2, 4);
				b.Write(test, 6, test.Length - 6 - 2);
				b.Write(test, test.Length - 2, 2);
				b.Close();
				NUnit.Framework.Assert.AreEqual(test.Length, b.Length());
				{
					byte[] r = b.ToByteArray();
					NUnit.Framework.Assert.IsNotNull(r);
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
				{
					ByteArrayOutputStream o = new ByteArrayOutputStream();
					b.WriteTo(o, null);
					o.Close();
					byte[] r = o.ToByteArray();
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
			}
			finally
			{
				b.Destroy();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestOneBlockAndHalf_SingleWrite()
		{
			TemporaryBuffer b = new TemporaryBuffer.LocalFile();
			byte[] test = new TestRng(Sharpen.Extensions.GetTestName(this)).NextBytes(TemporaryBuffer.Block
				.SZ * 3 / 2);
			try
			{
				for (int i = 0; i < test.Length; i++)
				{
					b.Write(test[i]);
				}
				b.Close();
				NUnit.Framework.Assert.AreEqual(test.Length, b.Length());
				{
					byte[] r = b.ToByteArray();
					NUnit.Framework.Assert.IsNotNull(r);
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
				{
					ByteArrayOutputStream o = new ByteArrayOutputStream();
					b.WriteTo(o, null);
					o.Close();
					byte[] r = o.ToByteArray();
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
			}
			finally
			{
				b.Destroy();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestOneBlockAndHalf_Copy()
		{
			TemporaryBuffer b = new TemporaryBuffer.LocalFile();
			byte[] test = new TestRng(Sharpen.Extensions.GetTestName(this)).NextBytes(TemporaryBuffer.Block
				.SZ * 3 / 2);
			try
			{
				ByteArrayInputStream @in = new ByteArrayInputStream(test);
				b.Write(@in.Read());
				b.Copy(@in);
				b.Close();
				NUnit.Framework.Assert.AreEqual(test.Length, b.Length());
				{
					byte[] r = b.ToByteArray();
					NUnit.Framework.Assert.IsNotNull(r);
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
				{
					ByteArrayOutputStream o = new ByteArrayOutputStream();
					b.WriteTo(o, null);
					o.Close();
					byte[] r = o.ToByteArray();
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
			}
			finally
			{
				b.Destroy();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestLarge_SingleWrite()
		{
			TemporaryBuffer b = new TemporaryBuffer.LocalFile();
			byte[] test = new TestRng(Sharpen.Extensions.GetTestName(this)).NextBytes(TemporaryBuffer
				.DEFAULT_IN_CORE_LIMIT * 3);
			try
			{
				b.Write(test);
				b.Close();
				NUnit.Framework.Assert.AreEqual(test.Length, b.Length());
				{
					byte[] r = b.ToByteArray();
					NUnit.Framework.Assert.IsNotNull(r);
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
				{
					ByteArrayOutputStream o = new ByteArrayOutputStream();
					b.WriteTo(o, null);
					o.Close();
					byte[] r = o.ToByteArray();
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
			}
			finally
			{
				b.Destroy();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestInCoreLimit_SwitchOnAppendByte()
		{
			TemporaryBuffer b = new TemporaryBuffer.LocalFile();
			byte[] test = new TestRng(Sharpen.Extensions.GetTestName(this)).NextBytes(TemporaryBuffer
				.DEFAULT_IN_CORE_LIMIT + 1);
			try
			{
				b.Write(test, 0, test.Length - 1);
				b.Write(test[test.Length - 1]);
				b.Close();
				NUnit.Framework.Assert.AreEqual(test.Length, b.Length());
				{
					byte[] r = b.ToByteArray();
					NUnit.Framework.Assert.IsNotNull(r);
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
				{
					ByteArrayOutputStream o = new ByteArrayOutputStream();
					b.WriteTo(o, null);
					o.Close();
					byte[] r = o.ToByteArray();
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
			}
			finally
			{
				b.Destroy();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestInCoreLimit_SwitchBeforeAppendByte()
		{
			TemporaryBuffer b = new TemporaryBuffer.LocalFile();
			byte[] test = new TestRng(Sharpen.Extensions.GetTestName(this)).NextBytes(TemporaryBuffer
				.DEFAULT_IN_CORE_LIMIT * 3);
			try
			{
				b.Write(test, 0, test.Length - 1);
				b.Write(test[test.Length - 1]);
				b.Close();
				NUnit.Framework.Assert.AreEqual(test.Length, b.Length());
				{
					byte[] r = b.ToByteArray();
					NUnit.Framework.Assert.IsNotNull(r);
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
				{
					ByteArrayOutputStream o = new ByteArrayOutputStream();
					b.WriteTo(o, null);
					o.Close();
					byte[] r = o.ToByteArray();
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
			}
			finally
			{
				b.Destroy();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestInCoreLimit_SwitchOnCopy()
		{
			TemporaryBuffer b = new TemporaryBuffer.LocalFile();
			byte[] test = new TestRng(Sharpen.Extensions.GetTestName(this)).NextBytes(TemporaryBuffer
				.DEFAULT_IN_CORE_LIMIT * 2);
			try
			{
				ByteArrayInputStream @in = new ByteArrayInputStream(test, TemporaryBuffer.DEFAULT_IN_CORE_LIMIT
					, test.Length - TemporaryBuffer.DEFAULT_IN_CORE_LIMIT);
				b.Write(test, 0, TemporaryBuffer.DEFAULT_IN_CORE_LIMIT);
				b.Copy(@in);
				b.Close();
				NUnit.Framework.Assert.AreEqual(test.Length, b.Length());
				{
					byte[] r = b.ToByteArray();
					NUnit.Framework.Assert.IsNotNull(r);
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
				{
					ByteArrayOutputStream o = new ByteArrayOutputStream();
					b.WriteTo(o, null);
					o.Close();
					byte[] r = o.ToByteArray();
					NUnit.Framework.Assert.AreEqual(test.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(test, r));
				}
			}
			finally
			{
				b.Destroy();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestDestroyWhileOpen()
		{
			TemporaryBuffer b = new TemporaryBuffer.LocalFile();
			try
			{
				b.Write(new TestRng(Sharpen.Extensions.GetTestName(this)).NextBytes(TemporaryBuffer
					.DEFAULT_IN_CORE_LIMIT * 2));
			}
			finally
			{
				b.Destroy();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestRandomWrites()
		{
			TemporaryBuffer b = new TemporaryBuffer.LocalFile();
			TestRng rng = new TestRng(Sharpen.Extensions.GetTestName(this));
			int max = TemporaryBuffer.DEFAULT_IN_CORE_LIMIT * 2;
			byte[] expect = new byte[max];
			try
			{
				int written = 0;
				bool onebyte = true;
				while (written < max)
				{
					if (onebyte)
					{
						byte v = unchecked((byte)rng.NextInt());
						b.Write(v);
						expect[written++] = v;
					}
					else
					{
						int len = Math.Min(rng.NextInt() & 127, max - written);
						byte[] tmp = rng.NextBytes(len);
						b.Write(tmp, 0, len);
						System.Array.Copy(tmp, 0, expect, written, len);
						written += len;
					}
					onebyte = !onebyte;
				}
				NUnit.Framework.Assert.AreEqual(expect.Length, written);
				b.Close();
				NUnit.Framework.Assert.AreEqual(expect.Length, b.Length());
				{
					byte[] r = b.ToByteArray();
					NUnit.Framework.Assert.IsNotNull(r);
					NUnit.Framework.Assert.AreEqual(expect.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(expect, r));
				}
				{
					ByteArrayOutputStream o = new ByteArrayOutputStream();
					b.WriteTo(o, null);
					o.Close();
					byte[] r = o.ToByteArray();
					NUnit.Framework.Assert.AreEqual(expect.Length, r.Length);
					NUnit.Framework.Assert.IsTrue(Arrays.Equals(expect, r));
				}
			}
			finally
			{
				b.Destroy();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestHeap()
		{
			TemporaryBuffer b = new TemporaryBuffer.Heap(2 * 8 * 1024);
			byte[] r = new byte[8 * 1024];
			b.Write(r);
			b.Write(r);
			try
			{
				b.Write(1);
				NUnit.Framework.Assert.Fail("accepted too many bytes of data");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.AreEqual("In-memory buffer limit exceeded", e.Message);
			}
		}
	}
}
