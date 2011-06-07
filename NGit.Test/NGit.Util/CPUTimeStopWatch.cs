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

using NGit.Util;
using Sharpen;
using Sharpen.Management;

namespace NGit.Util
{
	/// <summary>A simple stopwatch which measures elapsed CPU time of the current thread.
	/// 	</summary>
	/// <remarks>
	/// A simple stopwatch which measures elapsed CPU time of the current thread. CPU
	/// time is the time spent on executing your own code plus the time spent on
	/// executing operating system calls triggered by your application.
	/// <p>
	/// This stopwatch needs a VM which supports getting CPU Time information for the
	/// current thread. The static method createInstance() will take care to return
	/// only a new instance of this class if the VM is capable of returning CPU time.
	/// </remarks>
	public class CPUTimeStopWatch
	{
		private long start;

		private static ThreadMXBean mxBean = ManagementFactory.GetThreadMXBean();

		/// <summary>
		/// use this method instead of the constructor to be sure that the underlying
		/// VM provides all features needed by this class.
		/// </summary>
		/// <remarks>
		/// use this method instead of the constructor to be sure that the underlying
		/// VM provides all features needed by this class.
		/// </remarks>
		/// <returns>
		/// a new instance of
		/// <see cref="CPUTimeStopWatch()">CPUTimeStopWatch()</see>
		/// or
		/// <code>null</code> if the VM does not support getting CPU time
		/// information
		/// </returns>
		public static CPUTimeStopWatch CreateInstance()
		{
			return mxBean.IsCurrentThreadCpuTimeSupported() ? new CPUTimeStopWatch() : null;
		}

		/// <summary>Starts the stopwatch.</summary>
		/// <remarks>
		/// Starts the stopwatch. If the stopwatch is already started this will
		/// restart the stopwatch.
		/// </remarks>
		public virtual void Start()
		{
			start = mxBean.GetCurrentThreadCpuTime();
		}

		/// <summary>Stops the stopwatch and return the elapsed CPU time in nanoseconds.</summary>
		/// <remarks>
		/// Stops the stopwatch and return the elapsed CPU time in nanoseconds.
		/// Should be called only on started stopwatches.
		/// </remarks>
		/// <returns>
		/// the elapsed CPU time in nanoseconds. When called on non-started
		/// stopwatches (either because
		/// <see cref="Start()">Start()</see>
		/// was never called or
		/// <see cref="Stop()">Stop()</see>
		/// was called after the last call to
		/// <see cref="Start()">Start()</see>
		/// ) this method will return 0.
		/// </returns>
		public virtual long Stop()
		{
			long cpuTime = Readout();
			start = 0;
			return cpuTime;
		}

		/// <summary>Return the elapsed CPU time in nanoseconds.</summary>
		/// <remarks>
		/// Return the elapsed CPU time in nanoseconds. In contrast to
		/// <see cref="Stop()">Stop()</see>
		/// the stopwatch will continue to run after this call.
		/// </remarks>
		/// <returns>
		/// the elapsed CPU time in nanoseconds. When called on non-started
		/// stopwatches (either because
		/// <see cref="Start()">Start()</see>
		/// was never called or
		/// <see cref="Stop()">Stop()</see>
		/// was called after the last call to
		/// <see cref="Start()">Start()</see>
		/// ) this method will return 0.
		/// </returns>
		public virtual long Readout()
		{
			return (start == 0) ? 0 : mxBean.GetCurrentThreadCpuTime() - start;
		}
	}
}
