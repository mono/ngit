/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Threading;
using NGit.Util;
using NGit.Util.IO;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util.IO
{
	[NUnit.Framework.TestFixture]
	public class TimeoutOutputStreamTest
	{
		private const int timeout = 250;

		private PipedOutputStream @out;

		private TimeoutOutputStreamTest.FullPipeInputStream @in;

		private InterruptTimer timer;

		private TimeoutOutputStream os;

		private long start;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			@out = new PipedOutputStream();
			@in = new TimeoutOutputStreamTest.FullPipeInputStream(this, @out);
			timer = new InterruptTimer();
			os = new TimeoutOutputStream(@out, timer);
			os.SetTimeout(timeout);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.TearDown]
		public virtual void TearDown()
		{
			timer.Terminate();
			foreach (Sharpen.Thread t in Active())
			{
				NUnit.Framework.Assert.IsFalse(t is InterruptTimer.AlarmThread);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTimeout_writeByte_Success1()
		{
			@in.Free(1);
			os.Write('a');
			@in.Want(1);
			NUnit.Framework.Assert.AreEqual('a', @in.Read());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
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
			 CollectionAssert.AreEquivalent(exp, act);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
		public virtual void TestTimeout_writeBuffer_Success1()
		{
			byte[] exp = new byte[] { (byte)('a'), (byte)('b'), (byte)('c') };
			byte[] act = new byte[exp.Length];
			@in.Free(exp.Length);
			os.Write(exp);
			@in.Want(exp.Length);
			@in.Read(act);
			 CollectionAssert.AreEquivalent(exp, act);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
		public virtual void TestTimeout_flush_Success()
		{
			bool[] called = new bool[1];
			os = new TimeoutOutputStream(new _OutputStream_153(called), timer);
			os.SetTimeout(timeout);
			os.Flush();
			NUnit.Framework.Assert.IsTrue(called[0]);
		}

		private sealed class _OutputStream_153 : OutputStream
		{
			public _OutputStream_153(bool[] called)
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
		[NUnit.Framework.Test]
		public virtual void TestTimeout_flush_Timeout()
		{
			bool[] called = new bool[1];
			os = new TimeoutOutputStream(new _OutputStream_172(called), timer);
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

		private sealed class _OutputStream_172 : OutputStream
		{
			public _OutputStream_172(bool[] called)
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
		[NUnit.Framework.Test]
		public virtual void TestTimeout_close_Success()
		{
			bool[] called = new bool[1];
			os = new TimeoutOutputStream(new _OutputStream_206(called), timer);
			os.SetTimeout(timeout);
			os.Close();
			NUnit.Framework.Assert.IsTrue(called[0]);
		}

		private sealed class _OutputStream_206 : OutputStream
		{
			public _OutputStream_206(bool[] called)
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
		[NUnit.Framework.Test]
		public virtual void TestTimeout_close_Timeout()
		{
			bool[] called = new bool[1];
			os = new TimeoutOutputStream(new _OutputStream_225(called), timer);
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

		private sealed class _OutputStream_225 : OutputStream
		{
			public _OutputStream_225(bool[] called)
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
			NUnit.Framework.Assert.IsTrue(timeout - wait < 50, "waited only " + wait + " ms");
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
