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
using NGit;
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class ThreadSafeProgressMonitorTest
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFailsMethodsOnBackgroundThread()
		{
			ThreadSafeProgressMonitorTest.MockProgressMonitor mock = new ThreadSafeProgressMonitorTest.MockProgressMonitor
				();
			ThreadSafeProgressMonitor pm = new ThreadSafeProgressMonitor(mock);
			RunOnThread(new _Runnable_64(pm));
			// Expected result
			// Expected result
			// Expected result
			// Ensure we didn't alter the mock above when checking threads.
			NUnit.Framework.Assert.IsNull(mock.taskTitle);
			NUnit.Framework.Assert.AreEqual(0, mock.value);
		}

		private sealed class _Runnable_64 : Runnable
		{
			public _Runnable_64(ThreadSafeProgressMonitor pm)
			{
				this.pm = pm;
			}

			public void Run()
			{
				try
				{
					pm.Start(1);
					NUnit.Framework.Assert.Fail("start did not fail on background thread");
				}
				catch (InvalidOperationException)
				{
				}
				try
				{
					pm.BeginTask("title", 1);
					NUnit.Framework.Assert.Fail("beginTask did not fail on background thread");
				}
				catch (InvalidOperationException)
				{
				}
				try
				{
					pm.EndTask();
					NUnit.Framework.Assert.Fail("endTask did not fail on background thread");
				}
				catch (InvalidOperationException)
				{
				}
			}

			private readonly ThreadSafeProgressMonitor pm;
		}

		[NUnit.Framework.Test]
		public virtual void TestMethodsOkOnMainThread()
		{
			ThreadSafeProgressMonitorTest.MockProgressMonitor mock = new ThreadSafeProgressMonitorTest.MockProgressMonitor
				();
			ThreadSafeProgressMonitor pm = new ThreadSafeProgressMonitor(mock);
			pm.Start(1);
			NUnit.Framework.Assert.AreEqual(1, mock.value);
			pm.BeginTask("title", 42);
			NUnit.Framework.Assert.AreEqual("title", mock.taskTitle);
			NUnit.Framework.Assert.AreEqual(42, mock.value);
			pm.Update(1);
			NUnit.Framework.Assert.AreEqual(43, mock.value);
			pm.Update(2);
			NUnit.Framework.Assert.AreEqual(45, mock.value);
			pm.EndTask();
			NUnit.Framework.Assert.IsNull(mock.taskTitle);
			NUnit.Framework.Assert.AreEqual(0, mock.value);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateOnBackgroundThreads()
		{
			ThreadSafeProgressMonitorTest.MockProgressMonitor mock = new ThreadSafeProgressMonitorTest.MockProgressMonitor
				();
			ThreadSafeProgressMonitor pm = new ThreadSafeProgressMonitor(mock);
			pm.StartWorker();
			CountDownLatch doUpdate = new CountDownLatch(1);
			CountDownLatch didUpdate = new CountDownLatch(1);
			CountDownLatch doEndWorker = new CountDownLatch(1);
			Sharpen.Thread bg = new _Thread_128(pm, doUpdate, didUpdate, doEndWorker);
			bg.Start();
			pm.PollForUpdates();
			NUnit.Framework.Assert.AreEqual(0, mock.value);
			doUpdate.CountDown();
			Await(didUpdate);
			pm.PollForUpdates();
			NUnit.Framework.Assert.AreEqual(2, mock.value);
			doEndWorker.CountDown();
			pm.WaitForCompletion();
			NUnit.Framework.Assert.AreEqual(3, mock.value);
		}

		private sealed class _Thread_128 : Sharpen.Thread
		{
			public _Thread_128(ThreadSafeProgressMonitor pm, CountDownLatch doUpdate, CountDownLatch
				 didUpdate, CountDownLatch doEndWorker)
			{
				this.pm = pm;
				this.doUpdate = doUpdate;
				this.didUpdate = didUpdate;
				this.doEndWorker = doEndWorker;
			}

			public override void Run()
			{
				NUnit.Framework.Assert.IsFalse(pm.IsCancelled());
				ThreadSafeProgressMonitorTest.Await(doUpdate);
				pm.Update(2);
				didUpdate.CountDown();
				ThreadSafeProgressMonitorTest.Await(doEndWorker);
				pm.Update(1);
				pm.EndWorker();
			}

			private readonly ThreadSafeProgressMonitor pm;

			private readonly CountDownLatch doUpdate;

			private readonly CountDownLatch didUpdate;

			private readonly CountDownLatch doEndWorker;
		}

		private static void Await(CountDownLatch cdl)
		{
			try
			{
				NUnit.Framework.Assert.IsTrue(cdl.Await(1000, TimeUnit.MILLISECONDS), "latch released"
					);
			}
			catch (Exception)
			{
				NUnit.Framework.Assert.Fail("Did not expect to be interrupted");
			}
		}

		/// <exception cref="System.Exception"></exception>
		private static void RunOnThread(Runnable task)
		{
			Sharpen.Thread t = new Sharpen.Thread(task);
			t.Start();
			t.Join(1000);
			NUnit.Framework.Assert.IsFalse(t.IsAlive(), "thread has stopped");
		}

		private class MockProgressMonitor : ProgressMonitor
		{
			internal string taskTitle;

			internal int value;

			public override void Update(int completed)
			{
				value += completed;
			}

			public override void Start(int totalTasks)
			{
				value = totalTasks;
			}

			public override void BeginTask(string title, int totalWork)
			{
				taskTitle = title;
				value = totalWork;
			}

			public override void EndTask()
			{
				taskTitle = null;
				value = 0;
			}

			public override bool IsCancelled()
			{
				return false;
			}
		}
	}
}
