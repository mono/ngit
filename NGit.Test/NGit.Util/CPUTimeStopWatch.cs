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
