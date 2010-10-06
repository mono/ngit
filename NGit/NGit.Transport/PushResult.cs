using System.Collections.Generic;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Result of push operation to the remote repository.</summary>
	/// <remarks>
	/// Result of push operation to the remote repository. Holding information of
	/// <see cref="OperationResult">OperationResult</see>
	/// and remote refs updates status.
	/// </remarks>
	/// <seealso cref="Transport.Push(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E})
	/// 	">Transport.Push(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;)
	/// 	</seealso>
	public class PushResult : OperationResult
	{
		private IDictionary<string, RemoteRefUpdate> remoteUpdates = Sharpen.Collections.
			EmptyMap<string, RemoteRefUpdate>();

		/// <summary>Get status of remote refs updates.</summary>
		/// <remarks>
		/// Get status of remote refs updates. Together with
		/// <see cref="OperationResult.GetAdvertisedRefs()">OperationResult.GetAdvertisedRefs()
		/// 	</see>
		/// it provides full description/status of each
		/// ref update.
		/// <p>
		/// Returned collection is not sorted in any order.
		/// </p>
		/// </remarks>
		/// <returns>collection of remote refs updates</returns>
		public virtual ICollection<RemoteRefUpdate> GetRemoteUpdates()
		{
			return Sharpen.Collections.UnmodifiableCollection(remoteUpdates.Values);
		}

		/// <summary>Get status of specific remote ref update by remote ref name.</summary>
		/// <remarks>
		/// Get status of specific remote ref update by remote ref name. Together
		/// with
		/// <see cref="OperationResult.GetAdvertisedRef(string)">OperationResult.GetAdvertisedRef(string)
		/// 	</see>
		/// it provide full description/status
		/// of this ref update.
		/// </remarks>
		/// <param name="refName">remote ref name</param>
		/// <returns>status of remote ref update</returns>
		public virtual RemoteRefUpdate GetRemoteUpdate(string refName)
		{
			return remoteUpdates.Get(refName);
		}

		internal virtual void SetRemoteUpdates(IDictionary<string, RemoteRefUpdate> remoteUpdates
			)
		{
			this.remoteUpdates = remoteUpdates;
		}
	}
}
