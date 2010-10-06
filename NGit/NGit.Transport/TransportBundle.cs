using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>
	/// Marker interface for transports that supports fetching from a git bundle
	/// (sneaker-net object transport).
	/// </summary>
	/// <remarks>
	/// Marker interface for transports that supports fetching from a git bundle
	/// (sneaker-net object transport).
	/// <p>
	/// Push support for a bundle is complex, as one does not have a peer to
	/// communicate with to decide what the peer already knows. So push is not
	/// supported by the bundle transport.
	/// </remarks>
	public interface TransportBundle : PackTransport
	{
	}

	public abstract class TransportBundleConstants
	{
		/// <summary>Bundle signature</summary>
		public const string V2_BUNDLE_SIGNATURE = "# v2 git bundle";
	}
}
