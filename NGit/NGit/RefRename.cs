using System.IO;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>A RefUpdate combination for renaming a reference.</summary>
	/// <remarks>
	/// A RefUpdate combination for renaming a reference.
	/// <p>
	/// If the source reference is currently pointed to by
	/// <code>HEAD</code>
	/// , then the
	/// HEAD symbolic reference is updated to point to the new destination.
	/// </remarks>
	public abstract class RefRename
	{
		/// <summary>Update operation to read and delete the source reference.</summary>
		/// <remarks>Update operation to read and delete the source reference.</remarks>
		protected internal readonly RefUpdate source;

		/// <summary>Update operation to create/overwrite the destination reference.</summary>
		/// <remarks>Update operation to create/overwrite the destination reference.</remarks>
		protected internal readonly RefUpdate destination;

		private RefUpdate.Result result = RefUpdate.Result.NOT_ATTEMPTED;

		/// <summary>Initialize a new rename operation.</summary>
		/// <remarks>Initialize a new rename operation.</remarks>
		/// <param name="src">operation to read and delete the source.</param>
		/// <param name="dst">operation to create (or overwrite) the destination.</param>
		protected internal RefRename(RefUpdate src, RefUpdate dst)
		{
			source = src;
			destination = dst;
			Repository repo = destination.GetRepository();
			string cmd = string.Empty;
			if (source.GetName().StartsWith(Constants.R_HEADS) && destination.GetName().StartsWith
				(Constants.R_HEADS))
			{
				cmd = "Branch: ";
			}
			SetRefLogMessage(cmd + "renamed " + repo.ShortenRefName(source.GetName()) + " to "
				 + repo.ShortenRefName(destination.GetName()));
		}

		/// <returns>identity of the user making the change in the reflog.</returns>
		public virtual PersonIdent GetRefLogIdent()
		{
			return destination.GetRefLogIdent();
		}

		/// <summary>Set the identity of the user appearing in the reflog.</summary>
		/// <remarks>
		/// Set the identity of the user appearing in the reflog.
		/// <p>
		/// The timestamp portion of the identity is ignored. A new identity with the
		/// current timestamp will be created automatically when the rename occurs
		/// and the log record is written.
		/// </remarks>
		/// <param name="pi">
		/// identity of the user. If null the identity will be
		/// automatically determined based on the repository
		/// configuration.
		/// </param>
		public virtual void SetRefLogIdent(PersonIdent pi)
		{
			destination.SetRefLogIdent(pi);
		}

		/// <summary>Get the message to include in the reflog.</summary>
		/// <remarks>Get the message to include in the reflog.</remarks>
		/// <returns>
		/// message the caller wants to include in the reflog; null if the
		/// rename should not be logged.
		/// </returns>
		public virtual string GetRefLogMessage()
		{
			return destination.GetRefLogMessage();
		}

		/// <summary>Set the message to include in the reflog.</summary>
		/// <remarks>Set the message to include in the reflog.</remarks>
		/// <param name="msg">the message to describe this change.</param>
		public virtual void SetRefLogMessage(string msg)
		{
			if (msg == null)
			{
				DisableRefLog();
			}
			else
			{
				destination.SetRefLogMessage(msg, false);
			}
		}

		/// <summary>Don't record this rename in the ref's associated reflog.</summary>
		/// <remarks>Don't record this rename in the ref's associated reflog.</remarks>
		public virtual void DisableRefLog()
		{
			destination.SetRefLogMessage(string.Empty, false);
		}

		/// <returns>result of rename operation</returns>
		public virtual RefUpdate.Result GetResult()
		{
			return result;
		}

		/// <returns>the result of the new ref update</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual RefUpdate.Result Rename()
		{
			try
			{
				result = DoRename();
				return result;
			}
			catch (IOException err)
			{
				result = RefUpdate.Result.IO_FAILURE;
				throw;
			}
		}

		/// <returns>the result of the rename operation.</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		protected internal abstract RefUpdate.Result DoRename();

		/// <returns>
		/// true if the
		/// <code>Constants#HEAD</code>
		/// reference needs to be linked
		/// to the new destination name.
		/// </returns>
		/// <exception cref="System.IO.IOException">
		/// the current value of
		/// <code>HEAD</code>
		/// cannot be read.
		/// </exception>
		protected internal virtual bool NeedToUpdateHEAD()
		{
			Ref head = source.GetRefDatabase().GetRef(Constants.HEAD);
			if (head.IsSymbolic())
			{
				head = head.GetTarget();
				return head.GetName().Equals(source.GetName());
			}
			return false;
		}
	}
}
