using System;
using System.IO;
using NGit;
using NGit.Errors;
using NGit.Transport;
using NGit.Util;
using Sharpen;

namespace NGit.Transport
{
	internal class TransportBundleFile : NGit.Transport.Transport, TransportBundle
	{
		internal static bool CanHandle(URIish uri, FS fs)
		{
			if (uri.GetHost() != null || uri.GetPort() > 0 || uri.GetUser() != null || uri.GetPass
				() != null || uri.GetPath() == null)
			{
				return false;
			}
			if ("file".Equals(uri.GetScheme()) || uri.GetScheme() == null)
			{
				FilePath f = fs.Resolve(new FilePath("."), uri.GetPath());
				return f.IsFile() || f.GetName().EndsWith(".bundle");
			}
			return false;
		}

		private readonly FilePath bundle;

		protected internal TransportBundleFile(Repository local, URIish uri) : base(local
			, uri)
		{
			bundle = local.FileSystem.Resolve(new FilePath("."), uri.GetPath()).GetAbsoluteFile
				();
		}

		/// <exception cref="System.NotSupportedException"></exception>
		/// <exception cref="NGit.Errors.TransportException"></exception>
		public override FetchConnection OpenFetch()
		{
			InputStream src;
			try
			{
				src = new FileInputStream(bundle);
			}
			catch (FileNotFoundException)
			{
				throw new TransportException(uri, JGitText.Get().notFound);
			}
			return new BundleFetchConnection(this, src);
		}

		/// <exception cref="System.NotSupportedException"></exception>
		public override PushConnection OpenPush()
		{
			throw new NotSupportedException(JGitText.Get().pushIsNotSupportedForBundleTransport
				);
		}

		public override void Close()
		{
		}
		// Resources must be established per-connection.
	}
}
