using NGit;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>The base class for transports based on TCP sockets.</summary>
	/// <remarks>
	/// The base class for transports based on TCP sockets. This class
	/// holds settings common for all TCP based transports.
	/// </remarks>
	public abstract class TcpTransport : NGit.Transport.Transport
	{
		/// <summary>Create a new transport instance.</summary>
		/// <remarks>Create a new transport instance.</remarks>
		/// <param name="local">
		/// the repository this instance will fetch into, or push out of.
		/// This must be the repository passed to
		/// <see cref="Transport.Open(NGit.Repository, URIish)">Transport.Open(NGit.Repository, URIish)
		/// 	</see>
		/// .
		/// </param>
		/// <param name="uri">
		/// the URI used to access the remote repository. This must be the
		/// URI passed to
		/// <see cref="Transport.Open(NGit.Repository, URIish)">Transport.Open(NGit.Repository, URIish)
		/// 	</see>
		/// .
		/// </param>
		protected internal TcpTransport(Repository local, URIish uri) : base(local, uri)
		{
		}
	}
}
