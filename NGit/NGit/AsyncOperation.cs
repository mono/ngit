using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>Asynchronous operation handle.</summary>
	/// <remarks>
	/// Asynchronous operation handle.
	/// Callers that start an asynchronous operation are supplied with a handle that
	/// may be used to attempt cancellation of the operation if the caller does not
	/// wish to continue.
	/// </remarks>
	public interface AsyncOperation
	{
		/// <summary>Cancels the running task.</summary>
		/// <remarks>
		/// Cancels the running task.
		/// Attempts to cancel execution of this task. This attempt will fail if the
		/// task has already completed, already been cancelled, or could not be
		/// cancelled for some other reason. If successful, and this task has not
		/// started when cancel is called, this task should never run. If the task
		/// has already started, then the mayInterruptIfRunning parameter determines
		/// whether the thread executing this task should be interrupted in an
		/// attempt to stop the task.
		/// </remarks>
		/// <param name="mayInterruptIfRunning">
		/// true if the thread executing this task should be interrupted;
		/// otherwise, in-progress tasks are allowed to complete
		/// </param>
		/// <returns>
		/// false if the task could not be cancelled, typically because it
		/// has already completed normally; true otherwise
		/// </returns>
		bool Cancel(bool mayInterruptIfRunning);

		/// <summary>Release resources used by the operation, including cancellation.</summary>
		/// <remarks>Release resources used by the operation, including cancellation.</remarks>
		void Release();
	}
}
