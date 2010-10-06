using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Marker interface for an object transport walking transport.</summary>
	/// <remarks>
	/// Marker interface for an object transport walking transport.
	/// <p>
	/// Implementations of WalkTransport transfer individual objects one at a time
	/// from the loose objects directory, or entire packs if the source side does not
	/// have the object as a loose object.
	/// <p>
	/// WalkTransports are not as efficient as
	/// <see cref="PackTransport">PackTransport</see>
	/// instances, but
	/// can be useful in situations where a pack transport is not acceptable.
	/// </remarks>
	/// <seealso cref="WalkFetchConnection">WalkFetchConnection</seealso>
	public interface WalkTransport
	{
		// no methods in marker interface
	}
}
