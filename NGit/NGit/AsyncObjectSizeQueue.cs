using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>Queue to examine object sizes asynchronously.</summary>
	/// <remarks>
	/// Queue to examine object sizes asynchronously.
	/// A queue may perform background lookup of object sizes and supply them
	/// (possibly out-of-order) to the application.
	/// </remarks>
	/// <?></?>
	public interface AsyncObjectSizeQueue<T> : AsyncOperation where T:ObjectId
	{
		/// <summary>Position this queue onto the next available result.</summary>
		/// <remarks>Position this queue onto the next available result.</remarks>
		/// <returns>
		/// true if there is a result available; false if the queue has
		/// finished its input iteration.
		/// </returns>
		/// <exception cref="NGit.Errors.MissingObjectException">
		/// the object does not exist. If the implementation is retaining
		/// the application's objects
		/// <see cref="AsyncObjectSizeQueue{T}.GetCurrent()">AsyncObjectSizeQueue&lt;T&gt;.GetCurrent()
		/// 	</see>
		/// will be the
		/// current object that is missing. There may be more results
		/// still available, so the caller should continue invoking next
		/// to examine another result.
		/// </exception>
		/// <exception cref="System.IO.IOException">the object store cannot be accessed.</exception>
		bool Next();

		/// <returns>
		/// the current object, null if the implementation lost track.
		/// Implementations may for performance reasons discard the caller's
		/// ObjectId and provider their own through
		/// <see cref="AsyncObjectSizeQueue{T}.GetObjectId()">AsyncObjectSizeQueue&lt;T&gt;.GetObjectId()
		/// 	</see>
		/// .
		/// </returns>
		T GetCurrent();

		/// <returns>the ObjectId of the current object. Never null.</returns>
		ObjectId GetObjectId();

		/// <returns>the size of the current object.</returns>
		long GetSize();
	}
}
