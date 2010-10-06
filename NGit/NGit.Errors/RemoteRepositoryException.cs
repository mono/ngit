using NGit.Errors;
using NGit.Transport;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Contains a message from the remote repository indicating a problem.</summary>
	/// <remarks>
	/// Contains a message from the remote repository indicating a problem.
	/// <p>
	/// Some remote repositories may send customized error messages describing why
	/// they cannot be accessed. These messages are wrapped up in this exception and
	/// thrown to the caller of the transport operation.
	/// </remarks>
	[System.Serializable]
	public class RemoteRepositoryException : TransportException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Constructs a RemoteRepositoryException for a message.</summary>
		/// <remarks>Constructs a RemoteRepositoryException for a message.</remarks>
		/// <param name="uri">URI used for transport</param>
		/// <param name="message">
		/// message, exactly as supplied by the remote repository. May
		/// contain LFs (newlines) if the remote formatted it that way.
		/// </param>
		public RemoteRepositoryException(URIish uri, string message) : base(uri, message)
		{
		}
	}
}
