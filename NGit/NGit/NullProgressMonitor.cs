using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>A NullProgressMonitor does not report progress anywhere.</summary>
	/// <remarks>A NullProgressMonitor does not report progress anywhere.</remarks>
	public class NullProgressMonitor : ProgressMonitor
	{
		/// <summary>Immutable instance of a null progress monitor.</summary>
		/// <remarks>Immutable instance of a null progress monitor.</remarks>
		public static readonly NGit.NullProgressMonitor INSTANCE = new NGit.NullProgressMonitor
			();

		public NullProgressMonitor()
		{
		}

		// Do not let others instantiate
		public override void Start(int totalTasks)
		{
		}

		// Do not report.
		public override void BeginTask(string title, int totalWork)
		{
		}

		// Do not report.
		public override void Update(int completed)
		{
		}

		// Do not report.
		public override bool IsCancelled()
		{
			return false;
		}

		public override void EndTask()
		{
		}
		// Do not report.
	}
}
