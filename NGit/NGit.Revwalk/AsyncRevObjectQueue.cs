using NGit;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>Queue to lookup and parse objects asynchronously.</summary>
	/// <remarks>
	/// Queue to lookup and parse objects asynchronously.
	/// A queue may perform background lookup of objects and supply them (possibly
	/// out-of-order) to the application.
	/// </remarks>
	public interface AsyncRevObjectQueue : AsyncOperation
	{
		/// <summary>Obtain the next object.</summary>
		/// <remarks>Obtain the next object.</remarks>
		/// <returns>the object; null if there are no more objects remaining.</returns>
		/// <exception cref="NGit.Errors.MissingObjectException">
		/// the object does not exist. There may be more objects
		/// remaining in the iteration, the application should call
		/// <see cref="Next()">Next()</see>
		/// again.
		/// </exception>
		/// <exception cref="System.IO.IOException">the object store cannot be accessed.</exception>
		RevObject Next();
	}
}
