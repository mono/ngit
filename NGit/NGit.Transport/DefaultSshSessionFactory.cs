using NGit.Transport;
using NSch;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Loads known hosts and private keys from <code>$HOME/.ssh</code>.</summary>
	/// <remarks>
	/// Loads known hosts and private keys from <code>$HOME/.ssh</code>.
	/// <p>
	/// This is the default implementation used by JGit and provides most of the
	/// compatibility necessary to match OpenSSH, a popular implementation of SSH
	/// used by C Git.
	/// <p>
	/// If user interactivity is required by SSH (e.g. to obtain a password), the
	/// connection will immediately fail.
	/// </remarks>
	internal class DefaultSshSessionFactory : SshConfigSessionFactory
	{
		protected internal override void Configure(OpenSshConfig.Host hc, Session session
			)
		{
		}
		// No additional configuration required.
	}
}
