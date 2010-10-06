using System;
using System.Collections.Generic;
using System.Threading;
using NGit.Util;
using NGit.Util.IO;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util.IO
{
	public class TimeoutOutputStreamTest : TestCase
	{
		private const int timeout = 250;

		private PipedOutputStream @out;

		private TimeoutOutputStreamTest.FullPipeInputStream @in;

		private InterruptTimer timer;

		private TimeoutOutputStream os;

		private long start;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			@out = new PipedOutputStream();
			@in = new TimeoutOutputStreamTest.FullPipeInputStream(this, @out);
			timer = new InterruptTimer();
			os = new TimeoutOutputStream(@out, timer);
			os.SetTimeout(timeout);
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
		public virtual void TestTimeout_writeByte_Success1()
		{
			@in.Free(1);
			os.Write('a');
			@in.Want(1);
			NUnit.Framework.Assert.AreEqual('a', @in.Read());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_writeByte_Success2()
		{
			byte[] exp = new byte[] { (byte)('a'), (byte)('b'), (byte)('c') };
			byte[] act = new byte[exp.Length];
			@in.Free(exp.Length);
			os.Write(exp[0]);
			os.Write(exp[1]);
			os.Write(exp[2]);
			@in.Want(exp.Length);
			@in.Read(act);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(exp, act));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_writeByte_Timeout()
		{
			BeginWrite();
			try
			{
				os.Write('\n');
				NUnit.Framework.Assert.Fail("incorrectly write a byte");
			}
			catch (ThreadInterruptedException)
			{
			}
			// expected
			AssertTimeout();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_writeBuffer_Success1()
		{
			byte[] exp = new byte[] { (byte)('a'), (byte)('b'), (byte)('c') };
			byte[] act = new byte[exp.Length];
			@in.Free(exp.Length);
			os.Write(exp);
			@in.Want(exp.Length);
			@in.Read(act);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(exp, act));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_writeBuffer_Timeout()
		{
			BeginWrite();
			try
			{
				os.Write(new byte[512]);
				NUnit.Framework.Assert.Fail("incorrectly wrote bytes");
			}
			catch (ThreadInterruptedException)
			{
			}
			// expected
			AssertTimeout();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_flush_Success()
		{
			bool[] called = new bool[1];
			os = new TimeoutOutputStream(new _OutputStream_142(called), timer);
			os.SetTimeout(timeout);
			os.Flush();
			NUnit.Framework.Assert.IsTrue(called[0]);
		}

		private sealed class _OutputStream_142 : OutputStream
		{
			public _OutputStream_142(bool[] called)
			{
				this.called = called;
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Write(int b)
			{
				NUnit.Framework.Assert.Fail("should not have written");
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Flush()
			{
				called[0] = true;
			}

			private readonly bool[] called;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_flush_Timeout()
		{
			bool[] called = new bool[1];
			os = new TimeoutOutputStream(new _OutputStream_160(called), timer);
			os.SetTimeout(timeout);
			BeginWrite();
			try
			{
				os.Flush();
				NUnit.Framework.Assert.Fail("incorrectly flushed");
			}
			catch (ThreadInterruptedException)
			{
			}
			// expected
			AssertTimeout();
			NUnit.Framework.Assert.IsTrue(called[0]);
		}

		private sealed class _OutputStream_160 : OutputStream
		{
			public _OutputStream_160(bool[] called)
			{
				this.called = called;
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Write(int b)
			{
				NUnit.Framework.Assert.Fail("should not have written");
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Flush()
			{
				called[0] = true;
				for (; ; )
				{
					try
					{
						Sharpen.Thread.Sleep(1000);
					}
					catch (Exception)
					{
						throw new ThreadInterruptedException();
					}
				}
			}

			private readonly bool[] called;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_close_Success()
		{
			bool[] called = new bool[1];
			os = new TimeoutOutputStream(new _OutputStream_193(called), timer);
			os.SetTimeout(timeout);
			os.Close();
			NUnit.Framework.Assert.IsTrue(called[0]);
		}

		private sealed class _OutputStream_193 : OutputStream
		{
			public _OutputStream_193(bool[] called)
			{
				this.called = called;
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Write(int b)
			{
				NUnit.Framework.Assert.Fail("should not have written");
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Close()
			{
				called[0] = true;
			}

			private readonly bool[] called;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTimeout_close_Timeout()
		{
			bool[] called = new bool[1];
			os = new TimeoutOutputStream(new _OutputStream_211(called), timer);
			os.SetTimeout(timeout);
			BeginWrite();
			try
			{
				os.Close();
				NUnit.Framework.Assert.Fail("incorrectly closed");
			}
			catch (ThreadInterruptedException)
			{
			}
			// expected
			AssertTimeout();
			NUnit.Framework.Assert.IsTrue(called[0]);
		}

		private sealed class _OutputStream_211 : OutputStream
		{
			public _OutputStream_211(bool[] called)
			{
				this.called = called;
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Write(int b)
			{
				NUnit.Framework.Assert.Fail("should not have written");
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Close()
			{
				called[0] = true;
				for (; ; )
				{
					try
					{
						Sharpen.Thread.Sleep(1000);
					}
					catch (Exception)
					{
						throw new ThreadInterruptedException();
					}
				}
			}

			private readonly bool[] called;
		}

		private void BeginWrite()
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

		private sealed class FullPipeInputStream : PipedInputStream
		{
			/// <exception cref="System.IO.IOException"></exception>
			public FullPipeInputStream(TimeoutOutputStreamTest _enclosing, PipedOutputStream 
				src) : base(src)
			{
				this._enclosing = _enclosing;
				src.Write(new byte[PipedInputStream.PIPE_SIZE]);
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal void Want(int cnt)
			{
				IOUtil.SkipFully(this, PipedInputStream.PIPE_SIZE - cnt);
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal void Free(int cnt)
			{
				IOUtil.SkipFully(this, cnt);
			}

			private readonly TimeoutOutputStreamTest _enclosing;
		}
	}
}
