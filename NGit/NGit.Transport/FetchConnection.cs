using System.Collections.Generic;
using NGit;
using NGit.Storage.File;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Lists known refs from the remote and copies objects of selected refs.</summary>
	/// <remarks>
	/// Lists known refs from the remote and copies objects of selected refs.
	/// <p>
	/// A fetch connection typically connects to the <code>git-upload-pack</code>
	/// service running where the remote repository is stored. This provides a
	/// one-way object transfer service to copy objects from the remote repository
	/// into this local repository.
	/// <p>
	/// Instances of a FetchConnection must be created by a
	/// <see cref="Transport">Transport</see>
	/// that
	/// implements a specific object transfer protocol that both sides of the
	/// connection understand.
	/// <p>
	/// FetchConnection instances are not thread safe and may be accessed by only one
	/// thread at a time.
	/// </remarks>
	/// <seealso cref="Transport">Transport</seealso>
	public interface FetchConnection : Connection
	{
		/// <summary>Fetch objects we don't have but that are reachable from advertised refs.
		/// 	</summary>
		/// <remarks>
		/// Fetch objects we don't have but that are reachable from advertised refs.
		/// <p>
		/// Only one call per connection is allowed. Subsequent calls will result in
		/// <see cref="NGit.Errors.TransportException">NGit.Errors.TransportException</see>
		/// .
		/// </p>
		/// <p>
		/// Implementations are free to use network connections as necessary to
		/// efficiently (for both client and server) transfer objects from the remote
		/// repository into this repository. When possible implementations should
		/// avoid replacing/overwriting/duplicating an object already available in
		/// the local destination repository. Locally available objects and packs
		/// should always be preferred over remotely available objects and packs.
		/// <see cref="Transport.IsFetchThin()">Transport.IsFetchThin()</see>
		/// should be honored if applicable.
		/// </p>
		/// </remarks>
		/// <param name="monitor">
		/// progress monitor to inform the end-user about the amount of
		/// work completed, or to indicate cancellation. Implementations
		/// should poll the monitor at regular intervals to look for
		/// cancellation requests from the user.
		/// </param>
		/// <param name="want">
		/// one or more refs advertised by this connection that the caller
		/// wants to store locally.
		/// </param>
		/// <param name="have">
		/// additional objects known to exist in the destination
		/// repository, especially if they aren't yet reachable by the ref
		/// database. Connections should take this set as an addition to
		/// what is reachable through all Refs, not in replace of it.
		/// </param>
		/// <exception cref="NGit.Errors.TransportException">
		/// objects could not be copied due to a network failure,
		/// protocol error, or error on remote side, or connection was
		/// already used for fetch.
		/// </exception>
		void Fetch(ProgressMonitor monitor, ICollection<Ref> want, ICollection<ObjectId> 
			have);

		/// <summary>
		/// Did the last
		/// <see cref="Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E}, System.Collections.Generic.ICollection{E})
		/// 	">Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;, System.Collections.Generic.ICollection&lt;E&gt;)
		/// 	</see>
		/// get tags?
		/// <p>
		/// Some Git aware transports are able to implicitly grab an annotated tag if
		/// <see cref="TagOpt.AUTO_FOLLOW">TagOpt.AUTO_FOLLOW</see>
		/// or
		/// <see cref="TagOpt.FETCH_TAGS">TagOpt.FETCH_TAGS</see>
		/// was selected and
		/// the object the tag peels to (references) was transferred as part of the
		/// last
		/// <see cref="Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E}, System.Collections.Generic.ICollection{E})
		/// 	">Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;, System.Collections.Generic.ICollection&lt;E&gt;)
		/// 	</see>
		/// call. If it is
		/// possible for such tags to have been included in the transfer this method
		/// returns true, allowing the caller to attempt tag discovery.
		/// <p>
		/// By returning only true/false (and not the actual list of tags obtained)
		/// the transport itself does not need to be aware of whether or not tags
		/// were included in the transfer.
		/// </summary>
		/// <returns>
		/// true if the last fetch call implicitly included tag objects;
		/// false if tags were not implicitly obtained.
		/// </returns>
		bool DidFetchIncludeTags();

		/// <summary>
		/// Did the last
		/// <see cref="Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E}, System.Collections.Generic.ICollection{E})
		/// 	">Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;, System.Collections.Generic.ICollection&lt;E&gt;)
		/// 	</see>
		/// validate
		/// graph?
		/// <p>
		/// Some transports walk the object graph on the client side, with the client
		/// looking for what objects it is missing and requesting them individually
		/// from the remote peer. By virtue of completing the fetch call the client
		/// implicitly tested the object connectivity, as every object in the graph
		/// was either already local or was requested successfully from the peer. In
		/// such transports this method returns true.
		/// <p>
		/// Some transports assume the remote peer knows the Git object graph and is
		/// able to supply a fully connected graph to the client (although it may
		/// only be transferring the parts the client does not yet have). Its faster
		/// to assume such remote peers are well behaved and send the correct
		/// response to the client. In such transports this method returns false.
		/// </summary>
		/// <returns>
		/// true if the last fetch had to perform a connectivity check on the
		/// client side in order to succeed; false if the last fetch assumed
		/// the remote peer supplied a complete graph.
		/// </returns>
		bool DidFetchTestConnectivity();

		/// <summary>Set the lock message used when holding a pack out of garbage collection.
		/// 	</summary>
		/// <remarks>
		/// Set the lock message used when holding a pack out of garbage collection.
		/// <p>
		/// Callers that set a lock message <b>must</b> ensure they call
		/// <see cref="GetPackLocks()">GetPackLocks()</see>
		/// after
		/// <see cref="Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E}, System.Collections.Generic.ICollection{E})
		/// 	">Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;, System.Collections.Generic.ICollection&lt;E&gt;)
		/// 	</see>
		/// , even if an exception
		/// was thrown, and release the locks that are held.
		/// </remarks>
		/// <param name="message">message to use when holding a pack in place.</param>
		void SetPackLockMessage(string message);

		/// <summary>
		/// All locks created by the last
		/// <see cref="Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E}, System.Collections.Generic.ICollection{E})
		/// 	">Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;, System.Collections.Generic.ICollection&lt;E&gt;)
		/// 	</see>
		/// call.
		/// </summary>
		/// <returns>
		/// collection (possibly empty) of locks created by the last call to
		/// fetch. The caller must release these after refs are updated in
		/// order to safely permit garbage collection.
		/// </returns>
		ICollection<PackLock> GetPackLocks();
	}
}
