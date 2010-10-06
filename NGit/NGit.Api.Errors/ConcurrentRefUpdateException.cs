using System;
using NGit;
using NGit.Api.Errors;
using Sharpen;

namespace NGit.Api.Errors
{
	/// <summary>
	/// Exception thrown when a command wants to update a ref but failed because
	/// another process is accessing (or even also updating) the ref.
	/// </summary>
	/// <remarks>
	/// Exception thrown when a command wants to update a ref but failed because
	/// another process is accessing (or even also updating) the ref.
	/// </remarks>
	/// <seealso cref="NGit.RefUpdate.Result.LOCK_FAILURE">NGit.RefUpdate.Result.LOCK_FAILURE
	/// 	</seealso>
	[System.Serializable]
	public class ConcurrentRefUpdateException : GitAPIException
	{
		private const long serialVersionUID = 1L;

		private RefUpdate.Result rc;

		private Ref @ref;

		/// <param name="message"></param>
		/// <param name="ref"></param>
		/// <param name="rc"></param>
		/// <param name="cause"></param>
		public ConcurrentRefUpdateException(string message, Ref @ref, RefUpdate.Result rc
			, Exception cause) : base((rc == null) ? message : message + ". " + MessageFormat
			.Format(JGitText.Get().refUpdateReturnCodeWas, rc), cause)
		{
			this.rc = rc;
			this.@ref = @ref;
		}

		/// <param name="message"></param>
		/// <param name="ref"></param>
		/// <param name="rc"></param>
		public ConcurrentRefUpdateException(string message, Ref @ref, RefUpdate.Result rc
			) : base((rc == null) ? message : message + ". " + MessageFormat.Format(JGitText
			.Get().refUpdateReturnCodeWas, rc))
		{
			this.rc = rc;
			this.@ref = @ref;
		}

		/// <returns>
		/// the
		/// <see cref="NGit.Ref">NGit.Ref</see>
		/// which was tried to by updated
		/// </returns>
		public virtual Ref GetRef()
		{
			return @ref;
		}

		/// <returns>
		/// the result which was returned by
		/// <see cref="NGit.RefUpdate.Update()">NGit.RefUpdate.Update()</see>
		/// and
		/// which caused this error
		/// </returns>
		public virtual RefUpdate.Result GetResult()
		{
			return rc;
		}
	}
}
