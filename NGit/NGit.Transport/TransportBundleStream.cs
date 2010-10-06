using System;
using System.IO;
using NGit;
using NGit.Errors;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Single shot fetch from a streamed Git bundle.</summary>
	/// <remarks>
	/// Single shot fetch from a streamed Git bundle.
	/// <p>
	/// The bundle is read from an unbuffered input stream, which limits the
	/// transport to opening at most one FetchConnection before needing to recreate
	/// the transport instance.
	/// </remarks>
	public class TransportBundleStream : NGit.Transport.Transport, TransportBundle
	{
		private InputStream src;

		/// <summary>Create a new transport to fetch objects from a streamed bundle.</summary>
		/// <remarks>
		/// Create a new transport to fetch objects from a streamed bundle.
		/// <p>
		/// The stream can be unbuffered (buffering is automatically provided
		/// internally to smooth out short reads) and unpositionable (the stream is
		/// read from only once, sequentially).
		/// <p>
		/// When the FetchConnection or the this instance is closed the supplied
		/// input stream is also automatically closed. This frees callers from
		/// needing to keep track of the supplied stream.
		/// </remarks>
		/// <param name="db">repository the fetched objects will be loaded into.</param>
		/// <param name="uri">
		/// symbolic name of the source of the stream. The URI can
		/// reference a non-existent resource. It is used only for
		/// exception reporting.
		/// </param>
		/// <param name="in">the stream to read the bundle from.</param>
		public TransportBundleStream(Repository db, URIish uri, InputStream @in) : base(db
			, uri)
		{
			src = @in;
		}

		/// <exception cref="NGit.Errors.TransportException"></exception>
		public override FetchConnection OpenFetch()
		{
			if (src == null)
			{
				throw new TransportException(uri, JGitText.Get().onlyOneFetchSupported);
			}
			try
			{
				return new BundleFetchConnection(this, src);
			}
			finally
			{
				src = null;
			}
		}

		/// <exception cref="System.NotSupportedException"></exception>
		public override PushConnection OpenPush()
		{
			throw new NotSupportedException(JGitText.Get().pushIsNotSupportedForBundleTransport
				);
		}

		public override void Close()
		{
			if (src != null)
			{
				try
				{
					src.Close();
				}
				catch (IOException)
				{
				}
				finally
				{
					// Ignore a close error.
					src = null;
				}
			}
		}
	}
}
