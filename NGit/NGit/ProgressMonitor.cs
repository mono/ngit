using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>A progress reporting interface.</summary>
	/// <remarks>A progress reporting interface.</remarks>
	public abstract class ProgressMonitor
	{
		/// <summary>Constant indicating the total work units cannot be predicted.</summary>
		/// <remarks>Constant indicating the total work units cannot be predicted.</remarks>
		public const int UNKNOWN = 0;

		/// <summary>Advise the monitor of the total number of subtasks.</summary>
		/// <remarks>
		/// Advise the monitor of the total number of subtasks.
		/// <p>
		/// This should be invoked at most once per progress monitor interface.
		/// </remarks>
		/// <param name="totalTasks">
		/// the total number of tasks the caller will need to complete
		/// their processing.
		/// </param>
		public abstract void Start(int totalTasks);

		/// <summary>Begin processing a single task.</summary>
		/// <remarks>Begin processing a single task.</remarks>
		/// <param name="title">
		/// title to describe the task. Callers should publish these as
		/// stable string constants that implementations could match
		/// against for translation support.
		/// </param>
		/// <param name="totalWork">
		/// total number of work units the application will perform;
		/// <see cref="UNKNOWN">UNKNOWN</see>
		/// if it cannot be predicted in advance.
		/// </param>
		public abstract void BeginTask(string title, int totalWork);

		/// <summary>Denote that some work units have been completed.</summary>
		/// <remarks>
		/// Denote that some work units have been completed.
		/// <p>
		/// This is an incremental update; if invoked once per work unit the correct
		/// value for our argument is <code>1</code>, to indicate a single unit of
		/// work has been finished by the caller.
		/// </remarks>
		/// <param name="completed">the number of work units completed since the last call.</param>
		public abstract void Update(int completed);

		/// <summary>Finish the current task, so the next can begin.</summary>
		/// <remarks>Finish the current task, so the next can begin.</remarks>
		public abstract void EndTask();

		/// <summary>Check for user task cancellation.</summary>
		/// <remarks>Check for user task cancellation.</remarks>
		/// <returns>true if the user asked the process to stop working.</returns>
		public abstract bool IsCancelled();
	}
}
