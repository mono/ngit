using NGit.Transport;
using NGit.Util;
using NSch;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Creates and destroys SSH connections to a remote system.</summary>
	/// <remarks>
	/// Creates and destroys SSH connections to a remote system.
	/// <p>
	/// Different implementations of the session factory may be used to control
	/// communicating with the end-user as well as reading their personal SSH
	/// configuration settings, such as known hosts and private keys.
	/// <p>
	/// A
	/// <see cref="NSch.Session">NSch.Session</see>
	/// must be returned to the factory that created it. Callers
	/// are encouraged to retain the SshSessionFactory for the duration of the period
	/// they are using the Session.
	/// </remarks>
	public abstract class SshSessionFactory
	{
		private static SshSessionFactory INSTANCE = new DefaultSshSessionFactory();

		/// <summary>Get the currently configured JVM-wide factory.</summary>
		/// <remarks>
		/// Get the currently configured JVM-wide factory.
		/// <p>
		/// A factory is always available. By default the factory will read from the
		/// user's <code>$HOME/.ssh</code> and assume OpenSSH compatibility.
		/// </remarks>
		/// <returns>factory the current factory for this JVM.</returns>
		public static SshSessionFactory GetInstance()
		{
			return INSTANCE;
		}

		/// <summary>Change the JVM-wide factory to a different implementation.</summary>
		/// <remarks>Change the JVM-wide factory to a different implementation.</remarks>
		/// <param name="newFactory">
		/// factory for future sessions to be created through. If null the
		/// default factory will be restored.s
		/// </param>
		public static void SetInstance(SshSessionFactory newFactory)
		{
			if (newFactory != null)
			{
				INSTANCE = newFactory;
			}
			else
			{
				INSTANCE = new DefaultSshSessionFactory();
			}
		}

		/// <summary>Open (or reuse) a session to a host.</summary>
		/// <remarks>
		/// Open (or reuse) a session to a host.
		/// <p>
		/// A reasonable UserInfo that can interact with the end-user (if necessary)
		/// is installed on the returned session by this method.
		/// <p>
		/// The caller must connect the session by invoking <code>connect()</code>
		/// if it has not already been connected.
		/// </remarks>
		/// <param name="user">
		/// username to authenticate as. If null a reasonable default must
		/// be selected by the implementation. This may be
		/// <code>System.getProperty("user.name")</code>.
		/// </param>
		/// <param name="pass">
		/// optional user account password or passphrase. If not null a
		/// UserInfo that supplies this value to the SSH library will be
		/// configured.
		/// </param>
		/// <param name="host">hostname (or IP address) to connect to. Must not be null.</param>
		/// <param name="port">
		/// port number the server is listening for connections on. May be &lt;=
		/// 0 to indicate the IANA registered port of 22 should be used.
		/// </param>
		/// <param name="fs">
		/// the file system abstraction which will be necessary to
		/// perform certain file system operations.
		/// </param>
		/// <returns>a session that can contact the remote host.</returns>
		/// <exception cref="NSch.JSchException">the session could not be created.</exception>
		public abstract Session GetSession(string user, string pass, string host, int port
			, FS fs);

		/// <summary>Close (or recycle) a session to a host.</summary>
		/// <remarks>Close (or recycle) a session to a host.</remarks>
		/// <param name="session">
		/// a session previously obtained from this factory's
		/// <see cref="GetSession(string, string, string, int, NGit.Util.FS)">GetSession(string, string, string, int, NGit.Util.FS)
		/// 	</see>
		/// method.s
		/// </param>
		public virtual void ReleaseSession(Session session)
		{
			if (session.IsConnected())
			{
				session.Disconnect();
			}
		}
	}
}
