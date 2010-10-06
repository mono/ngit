using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Marker interface an object transport using Git pack transfers.</summary>
	/// <remarks>
	/// Marker interface an object transport using Git pack transfers.
	/// <p>
	/// Implementations of PackTransport setup connections and move objects back and
	/// forth by creating pack files on the source side and indexing them on the
	/// receiving side.
	/// </remarks>
	/// <seealso cref="BasePackFetchConnection">BasePackFetchConnection</seealso>
	/// <seealso cref="BasePackPushConnection">BasePackPushConnection</seealso>
	public interface PackTransport
	{
		// no methods in marker interface
	}
}
