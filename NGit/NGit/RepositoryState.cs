using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// Important state of the repository that affects what can and cannot bed
	/// done.
	/// </summary>
	/// <remarks>
	/// Important state of the repository that affects what can and cannot bed
	/// done. This is things like unhandled conflicted merges and unfinished rebase.
	/// The granularity and set of states are somewhat arbitrary. The methods
	/// on the state are the only supported means of deciding what to do.
	/// </remarks>
	public abstract class RepositoryState
	{
		/// <summary>Has no work tree and cannot be used for normal editing.</summary>
		/// <remarks>Has no work tree and cannot be used for normal editing.</remarks>
		public static RepositoryState BARE = new RepositoryState.BARE_Class();

		internal class BARE_Class : RepositoryState
		{
			public override bool CanCheckout()
			{
				return false;
			}

			public override bool CanResetHead()
			{
				return false;
			}

			public override bool CanCommit()
			{
				return false;
			}

			public override string GetDescription()
			{
				return "Bare";
			}

			public override string Name()
			{
				return "BARE";
			}
		}

		/// <summary>A safe state for working normally</summary>
		public static RepositoryState SAFE = new RepositoryState.SAFE_Class();

		internal class SAFE_Class : RepositoryState
		{
			public override bool CanCheckout()
			{
				return true;
			}

			public override bool CanResetHead()
			{
				return true;
			}

			public override bool CanCommit()
			{
				return true;
			}

			public override string GetDescription()
			{
				return JGitText.Get().repositoryState_normal;
			}

			public override string Name()
			{
				return "SAFE";
			}
		}

		/// <summary>An unfinished merge.</summary>
		/// <remarks>An unfinished merge. Must resolve or reset before continuing normally</remarks>
		public static RepositoryState MERGING = new RepositoryState.MERGING_Class();

		internal class MERGING_Class : RepositoryState
		{
			public override bool CanCheckout()
			{
				return false;
			}

			public override bool CanResetHead()
			{
				return true;
			}

			public override bool CanCommit()
			{
				return false;
			}

			public override string GetDescription()
			{
				return JGitText.Get().repositoryState_conflicts;
			}

			public override string Name()
			{
				return "MERGING";
			}
		}

		/// <summary>An merge where all conflicts have been resolved.</summary>
		/// <remarks>
		/// An merge where all conflicts have been resolved. The index does not
		/// contain any unmerged paths.
		/// </remarks>
		public static RepositoryState MERGING_RESOLVED = new RepositoryState.MERGING_RESOLVED_Class
			();

		internal class MERGING_RESOLVED_Class : RepositoryState
		{
			public override bool CanCheckout()
			{
				return true;
			}

			public override bool CanResetHead()
			{
				return true;
			}

			public override bool CanCommit()
			{
				return true;
			}

			public override string GetDescription()
			{
				return JGitText.Get().repositoryState_merged;
			}

			public override string Name()
			{
				return "MERGING_RESOLVED";
			}
		}

		/// <summary>An unfinished rebase or am.</summary>
		/// <remarks>An unfinished rebase or am. Must resolve, skip or abort before normal work can take place
		/// 	</remarks>
		public static RepositoryState REBASING = new RepositoryState.REBASING_Class();

		internal class REBASING_Class : RepositoryState
		{
			public override bool CanCheckout()
			{
				return false;
			}

			public override bool CanResetHead()
			{
				return false;
			}

			public override bool CanCommit()
			{
				return true;
			}

			public override string GetDescription()
			{
				return JGitText.Get().repositoryState_rebaseOrApplyMailbox;
			}

			public override string Name()
			{
				return "REBASING";
			}
		}

		/// <summary>An unfinished rebase.</summary>
		/// <remarks>An unfinished rebase. Must resolve, skip or abort before normal work can take place
		/// 	</remarks>
		public static RepositoryState REBASING_REBASING = new RepositoryState.REBASING_REBASING_Class
			();

		internal class REBASING_REBASING_Class : RepositoryState
		{
			public override bool CanCheckout()
			{
				return false;
			}

			public override bool CanResetHead()
			{
				return false;
			}

			public override bool CanCommit()
			{
				return true;
			}

			public override string GetDescription()
			{
				return JGitText.Get().repositoryState_rebase;
			}

			public override string Name()
			{
				return "REBASING_REBASING";
			}
		}

		/// <summary>An unfinished apply.</summary>
		/// <remarks>An unfinished apply. Must resolve, skip or abort before normal work can take place
		/// 	</remarks>
		public static RepositoryState APPLY = new RepositoryState.APPLY_Class();

		internal class APPLY_Class : RepositoryState
		{
			public override bool CanCheckout()
			{
				return false;
			}

			public override bool CanResetHead()
			{
				return false;
			}

			public override bool CanCommit()
			{
				return true;
			}

			public override string GetDescription()
			{
				return JGitText.Get().repositoryState_applyMailbox;
			}

			public override string Name()
			{
				return "APPLY";
			}
		}

		/// <summary>An unfinished rebase with merge.</summary>
		/// <remarks>An unfinished rebase with merge. Must resolve, skip or abort before normal work can take place
		/// 	</remarks>
		public static RepositoryState REBASING_MERGE = new RepositoryState.REBASING_MERGE_Class
			();

		internal class REBASING_MERGE_Class : RepositoryState
		{
			public override bool CanCheckout()
			{
				return false;
			}

			public override bool CanResetHead()
			{
				return false;
			}

			public override bool CanCommit()
			{
				return true;
			}

			public override string GetDescription()
			{
				return JGitText.Get().repositoryState_rebaseWithMerge;
			}

			public override string Name()
			{
				return "REBASING_MERGE";
			}
		}

		/// <summary>An unfinished interactive rebase.</summary>
		/// <remarks>An unfinished interactive rebase. Must resolve, skip or abort before normal work can take place
		/// 	</remarks>
		public static RepositoryState REBASING_INTERACTIVE = new RepositoryState.REBASING_INTERACTIVE_Class
			();

		internal class REBASING_INTERACTIVE_Class : RepositoryState
		{
			public override bool CanCheckout()
			{
				return false;
			}

			public override bool CanResetHead()
			{
				return false;
			}

			public override bool CanCommit()
			{
				return true;
			}

			public override string GetDescription()
			{
				return JGitText.Get().repositoryState_rebaseInteractive;
			}

			public override string Name()
			{
				return "REBASING_INTERACTIVE";
			}
		}

		/// <summary>Bisecting being done.</summary>
		/// <remarks>Bisecting being done. Normal work may continue but is discouraged</remarks>
		public static RepositoryState BISECTING = new RepositoryState.BISECTING_Class();

		internal class BISECTING_Class : RepositoryState
		{
			public override bool CanCheckout()
			{
				return true;
			}

			public override bool CanResetHead()
			{
				return false;
			}

			public override bool CanCommit()
			{
				return true;
			}

			public override string GetDescription()
			{
				return JGitText.Get().repositoryState_bisecting;
			}

			public override string Name()
			{
				return "BISECTING";
			}
		}

		/// <returns>true if changing HEAD is sane.</returns>
		public abstract bool CanCheckout();

		/// <returns>true if we can commit</returns>
		public abstract bool CanCommit();

		/// <returns>true if reset to another HEAD is considered SAFE</returns>
		public abstract bool CanResetHead();

		/// <returns>a human readable description of the state.</returns>
		public abstract string GetDescription();

		public abstract string Name();
	}
}
