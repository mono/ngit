using System;
using NGit;
using NGit.Errors;
using NGit.Transport;
using NSch;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>The base class for transports that use SSH protocol.</summary>
	/// <remarks>
	/// The base class for transports that use SSH protocol. This class allows
	/// customizing SSH connection settings.
	/// </remarks>
	public abstract class SshTransport : TcpTransport
	{
		private SshSessionFactory sch;

		/// <summary>The open SSH session</summary>
		protected internal Session sock;

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
		protected internal SshTransport(Repository local, URIish uri) : base(local, uri)
		{
			sch = SshSessionFactory.GetInstance();
		}

		/// <summary>
		/// Set SSH session factory instead of the default one for this instance of
		/// the transport.
		/// </summary>
		/// <remarks>
		/// Set SSH session factory instead of the default one for this instance of
		/// the transport.
		/// </remarks>
		/// <param name="factory">a factory to set, must not be null</param>
		/// <exception cref="System.InvalidOperationException">if session has been already created.
		/// 	</exception>
		public virtual void SetSshSessionFactory(SshSessionFactory factory)
		{
			if (factory == null)
			{
				throw new ArgumentNullException(JGitText.Get().theFactoryMustNotBeNull);
			}
			if (sock != null)
			{
				throw new InvalidOperationException(JGitText.Get().anSSHSessionHasBeenAlreadyCreated
					);
			}
			sch = factory;
		}

		/// <returns>the SSH session factory that will be used for creating SSH sessions</returns>
		public virtual SshSessionFactory GetSshSessionFactory()
		{
			return sch;
		}

		/// <summary>Initialize SSH session</summary>
		/// <exception cref="NGit.Errors.TransportException">in case of error with opening SSH session
		/// 	</exception>
		protected internal virtual void InitSession()
		{
			if (sock != null)
			{
				return;
			}
			int tms = GetTimeout() > 0 ? GetTimeout() * 1000 : 0;
			string user = uri.GetUser();
			string pass = uri.GetPass();
			string host = uri.GetHost();
			int port = uri.GetPort();
			try
			{
				sock = sch.GetSession(user, pass, host, port, local.FileSystem);
				if (!sock.IsConnected())
				{
					sock.Connect(tms);
				}
			}
			catch (JSchException je)
			{
				Exception c = je.InnerException;
				if (c is UnknownHostException)
				{
					throw new TransportException(uri, JGitText.Get().unknownHost);
				}
				if (c is ConnectException)
				{
					throw new TransportException(uri, c.Message);
				}
				throw new TransportException(uri, je.Message, je);
			}
		}

		public override void Close()
		{
			if (sock != null)
			{
				try
				{
					sch.ReleaseSession(sock);
				}
				finally
				{
					sock = null;
				}
			}
		}
	}
}
