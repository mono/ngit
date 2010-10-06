using System;
using NGit.Errors;
using NGit.Transport;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Indicates a protocol error has occurred while fetching/pushing objects.</summary>
	/// <remarks>Indicates a protocol error has occurred while fetching/pushing objects.</remarks>
	[System.Serializable]
	public class PackProtocolException : TransportException
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Constructs an PackProtocolException with the specified detail message
		/// prefixed with provided URI.
		/// </summary>
		/// <remarks>
		/// Constructs an PackProtocolException with the specified detail message
		/// prefixed with provided URI.
		/// </remarks>
		/// <param name="uri">URI used for transport</param>
		/// <param name="s">message</param>
		public PackProtocolException(URIish uri, string s) : base(uri + ": " + s)
		{
		}

		/// <summary>
		/// Constructs an PackProtocolException with the specified detail message
		/// prefixed with provided URI.
		/// </summary>
		/// <remarks>
		/// Constructs an PackProtocolException with the specified detail message
		/// prefixed with provided URI.
		/// </remarks>
		/// <param name="uri">URI used for transport</param>
		/// <param name="s">message</param>
		/// <param name="cause">root cause exception</param>
		public PackProtocolException(URIish uri, string s, Exception cause) : this(uri + 
			": " + s, cause)
		{
		}

		/// <summary>Constructs an PackProtocolException with the specified detail message.</summary>
		/// <remarks>Constructs an PackProtocolException with the specified detail message.</remarks>
		/// <param name="s">message</param>
		public PackProtocolException(string s) : base(s)
		{
		}

		/// <summary>Constructs an PackProtocolException with the specified detail message.</summary>
		/// <remarks>Constructs an PackProtocolException with the specified detail message.</remarks>
		/// <param name="s">message</param>
		/// <param name="cause">root cause exception</param>
		public PackProtocolException(string s, Exception cause) : base(s)
		{
			Sharpen.Extensions.InitCause(this, cause);
		}
	}
}
