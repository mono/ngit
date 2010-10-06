using System.Collections.Generic;
using NGit;
using NGit.Storage.File;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Base helper class for fetch connection implementations.</summary>
	/// <remarks>
	/// Base helper class for fetch connection implementations. Provides some common
	/// typical structures and methods used during fetch connection.
	/// <p>
	/// Implementors of fetch over pack-based protocols should consider using
	/// <see cref="BasePackFetchConnection">BasePackFetchConnection</see>
	/// instead.
	/// </p>
	/// </remarks>
	internal abstract class BaseFetchConnection : BaseConnection, FetchConnection
	{
		/// <exception cref="NGit.Errors.TransportException"></exception>
		public void Fetch(ProgressMonitor monitor, ICollection<Ref> want, ICollection<ObjectId
			> have)
		{
			MarkStartedOperation();
			DoFetch(monitor, want, have);
		}

		/// <summary>
		/// Default implementation of
		/// <see cref="FetchConnection.DidFetchIncludeTags()">FetchConnection.DidFetchIncludeTags()
		/// 	</see>
		/// -
		/// returning false.
		/// </summary>
		public virtual bool DidFetchIncludeTags()
		{
			return false;
		}

		/// <summary>
		/// Implementation of
		/// <see cref="Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E}, System.Collections.Generic.ICollection{E})
		/// 	">Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;, System.Collections.Generic.ICollection&lt;E&gt;)
		/// 	</see>
		/// without checking for multiple fetch.
		/// </summary>
		/// <param name="monitor">
		/// as in
		/// <see cref="Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E}, System.Collections.Generic.ICollection{E})
		/// 	">Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;, System.Collections.Generic.ICollection&lt;E&gt;)
		/// 	</see>
		/// </param>
		/// <param name="want">
		/// as in
		/// <see cref="Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E}, System.Collections.Generic.ICollection{E})
		/// 	">Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;, System.Collections.Generic.ICollection&lt;E&gt;)
		/// 	</see>
		/// </param>
		/// <param name="have">
		/// as in
		/// <see cref="Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E}, System.Collections.Generic.ICollection{E})
		/// 	">Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;, System.Collections.Generic.ICollection&lt;E&gt;)
		/// 	</see>
		/// </param>
		/// <exception cref="NGit.Errors.TransportException">
		/// as in
		/// <see cref="Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E}, System.Collections.Generic.ICollection{E})
		/// 	">Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;, System.Collections.Generic.ICollection&lt;E&gt;)
		/// 	</see>
		/// , but
		/// implementation doesn't have to care about multiple
		/// <see cref="Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E}, System.Collections.Generic.ICollection{E})
		/// 	">Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;, System.Collections.Generic.ICollection&lt;E&gt;)
		/// 	</see>
		/// calls, as it
		/// is checked in this class.
		/// </exception>
		protected internal abstract void DoFetch(ProgressMonitor monitor, ICollection<Ref
			> want, ICollection<ObjectId> have);

		public abstract bool DidFetchTestConnectivity();

		public abstract ICollection<PackLock> GetPackLocks();

		public abstract void SetPackLockMessage(string arg1);
	}
}
