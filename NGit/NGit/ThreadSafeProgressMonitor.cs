using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// Wrapper around the general
	/// <see cref="ProgressMonitor">ProgressMonitor</see>
	/// to make it thread safe.
	/// </summary>
	public class ThreadSafeProgressMonitor : ProgressMonitor
	{
		private readonly ProgressMonitor pm;

		private readonly ReentrantLock Lock;

		/// <summary>Wrap a ProgressMonitor to be thread safe.</summary>
		/// <remarks>Wrap a ProgressMonitor to be thread safe.</remarks>
		/// <param name="pm">the underlying monitor to receive events.</param>
		public ThreadSafeProgressMonitor(ProgressMonitor pm)
		{
			this.pm = pm;
			this.Lock = new ReentrantLock();
		}

		public override void Start(int totalTasks)
		{
			Lock.Lock();
			try
			{
				pm.Start(totalTasks);
			}
			finally
			{
				Lock.Unlock();
			}
		}

		public override void BeginTask(string title, int totalWork)
		{
			Lock.Lock();
			try
			{
				pm.BeginTask(title, totalWork);
			}
			finally
			{
				Lock.Unlock();
			}
		}

		public override void Update(int completed)
		{
			Lock.Lock();
			try
			{
				pm.Update(completed);
			}
			finally
			{
				Lock.Unlock();
			}
		}

		public override bool IsCancelled()
		{
			Lock.Lock();
			try
			{
				return pm.IsCancelled();
			}
			finally
			{
				Lock.Unlock();
			}
		}

		public override void EndTask()
		{
			Lock.Lock();
			try
			{
				pm.EndTask();
			}
			finally
			{
				Lock.Unlock();
			}
		}
	}
}
