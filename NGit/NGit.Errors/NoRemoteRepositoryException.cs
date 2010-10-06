using NGit.Errors;
using NGit.Transport;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Indicates a remote repository does not exist.</summary>
	/// <remarks>Indicates a remote repository does not exist.</remarks>
	[System.Serializable]
	public class NoRemoteRepositoryException : TransportException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Constructs an exception indicating a repository does not exist.</summary>
		/// <remarks>Constructs an exception indicating a repository does not exist.</remarks>
		/// <param name="uri">URI used for transport</param>
		/// <param name="s">message</param>
		public NoRemoteRepositoryException(URIish uri, string s) : base(uri, s)
		{
		}
	}
}
