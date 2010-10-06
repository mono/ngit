using System;
using System.IO;
using NGit.Transport;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Indicates a protocol error has occurred while fetching/pushing objects.</summary>
	/// <remarks>Indicates a protocol error has occurred while fetching/pushing objects.</remarks>
	[System.Serializable]
	public class TransportException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Constructs an TransportException with the specified detail message
		/// prefixed with provided URI.
		/// </summary>
		/// <remarks>
		/// Constructs an TransportException with the specified detail message
		/// prefixed with provided URI.
		/// </remarks>
		/// <param name="uri">URI used for transport</param>
		/// <param name="s">message</param>
		public TransportException(URIish uri, string s) : base(uri.SetPass(null) + ": " +
			 s)
		{
		}

		/// <summary>
		/// Constructs an TransportException with the specified detail message
		/// prefixed with provided URI.
		/// </summary>
		/// <remarks>
		/// Constructs an TransportException with the specified detail message
		/// prefixed with provided URI.
		/// </remarks>
		/// <param name="uri">URI used for transport</param>
		/// <param name="s">message</param>
		/// <param name="cause">root cause exception</param>
		public TransportException(URIish uri, string s, Exception cause) : this(uri.SetPass
			(null) + ": " + s, cause)
		{
		}

		/// <summary>Constructs an TransportException with the specified detail message.</summary>
		/// <remarks>Constructs an TransportException with the specified detail message.</remarks>
		/// <param name="s">message</param>
		public TransportException(string s) : base(s)
		{
		}

		/// <summary>Constructs an TransportException with the specified detail message.</summary>
		/// <remarks>Constructs an TransportException with the specified detail message.</remarks>
		/// <param name="s">message</param>
		/// <param name="cause">root cause exception</param>
		public TransportException(string s, Exception cause) : base(s)
		{
			Sharpen.Extensions.InitCause(this, cause);
		}
	}
}
