using System.Collections.Generic;
using System.Threading;
using NGit.Util;
using NGit.Util.IO;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util.IO
{
	public class TimeoutInputStreamTest : TestCase
	{
		private const int timeout = 250;

		private PipedOutputStream @out;

		private PipedInputStream @in;

		private InterruptTimer timer;

		private TimeoutInputStream @is;

		private long start;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			@out = new PipedOutputStream();
			@in = new PipedInputStream(@out);
			timer = new InterruptTimer();
			@is = new TimeoutInputStream(@in, timer);
			@is.SetTimeout(timeout);
		}

		/// <exception cref="System.Exception"></exception>
		protected override void TearDown()
		{
			timer.Terminate();
			foreach (Sharpen.Thread t in Active())
			{
				NUnit.Framework.Assert.IsFalse(t is InterruptTimer.AlarmThread);
			}
			base.TearDown();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_readByte_Success1()
		{
			@out.Write('a');
			NUnit.Framework.Assert.AreEqual('a', @is.Read());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_readByte_Success2()
		{
			byte[] exp = new byte[] { (byte)('a'), (byte)('b'), (byte)('c') };
			@out.Write(exp);
			NUnit.Framework.Assert.AreEqual(exp[0], @is.Read());
			NUnit.Framework.Assert.AreEqual(exp[1], @is.Read());
			NUnit.Framework.Assert.AreEqual(exp[2], @is.Read());
			@out.Close();
			NUnit.Framework.Assert.AreEqual(-1, @is.Read());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_readByte_Timeout()
		{
			BeginRead();
			try
			{
				@is.Read();
				NUnit.Framework.Assert.Fail("incorrectly read a byte");
			}
			catch (ThreadInterruptedException)
			{
			}
			// expected
			AssertTimeout();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_readBuffer_Success1()
		{
			byte[] exp = new byte[] { (byte)('a'), (byte)('b'), (byte)('c') };
			byte[] act = new byte[exp.Length];
			@out.Write(exp);
			IOUtil.ReadFully(@is, act, 0, act.Length);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(exp, act));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_readBuffer_Success2()
		{
			byte[] exp = new byte[] { (byte)('a'), (byte)('b'), (byte)('c') };
			byte[] act = new byte[exp.Length];
			@out.Write(exp);
			IOUtil.ReadFully(@is, act, 0, 1);
			IOUtil.ReadFully(@is, act, 1, 1);
			IOUtil.ReadFully(@is, act, 2, 1);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(exp, act));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_readBuffer_Timeout()
		{
			BeginRead();
			try
			{
				IOUtil.ReadFully(@is, new byte[512], 0, 512);
				NUnit.Framework.Assert.Fail("incorrectly read bytes");
			}
			catch (ThreadInterruptedException)
			{
			}
			// expected
			AssertTimeout();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_skip_Success()
		{
			byte[] exp = new byte[] { (byte)('a'), (byte)('b'), (byte)('c') };
			@out.Write(exp);
			NUnit.Framework.Assert.AreEqual(2, @is.Skip(2));
			NUnit.Framework.Assert.AreEqual('c', @is.Read());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_skip_Timeout()
		{
			BeginRead();
			try
			{
				@is.Skip(1024);
				NUnit.Framework.Assert.Fail("incorrectly skipped bytes");
			}
			catch (ThreadInterruptedException)
			{
			}
			// expected
			AssertTimeout();
		}

		private void BeginRead()
		{
			start = Now();
		}

		private void AssertTimeout()
		{
			// Our timeout was supposed to be ~250 ms. Since this is a timing
			// test we can't assume we spent *exactly* the timeout period, as
			// there may be other activity going on in the system. Instead we
			// look for the delta between the start and end times to be within
			// 50 ms of the expected timeout.
			//
			long wait = Now() - start;
			NUnit.Framework.Assert.IsTrue("waited only " + wait + " ms", timeout - wait < 50);
		}

		private static IList<Sharpen.Thread> Active()
		{
			Sharpen.Thread[] all = new Sharpen.Thread[16];
			int n = Sharpen.Thread.CurrentThread().GetThreadGroup().Enumerate(all);
			while (n == all.Length)
			{
				all = new Sharpen.Thread[all.Length * 2];
				n = Sharpen.Thread.CurrentThread().GetThreadGroup().Enumerate(all);
			}
			return Arrays.AsList(all).SubList(0, n);
		}

		private static long Now()
		{
			return Runtime.CurrentTimeMillis();
		}
	}
}
