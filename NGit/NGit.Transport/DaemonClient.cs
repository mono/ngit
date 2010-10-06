using System.Net;
using System.Net.Sockets;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>
	/// Active network client of
	/// <see cref="Daemon">Daemon</see>
	/// .
	/// </summary>
	public class DaemonClient
	{
		private readonly Daemon daemon;

		private IPAddress peer;

		private InputStream rawIn;

		private OutputStream rawOut;

		internal DaemonClient(Daemon d)
		{
			daemon = d;
		}

		internal virtual void SetRemoteAddress(IPAddress ia)
		{
			peer = ia;
		}

		/// <returns>the daemon which spawned this client.</returns>
		public virtual Daemon GetDaemon()
		{
			return daemon;
		}

		/// <returns>Internet address of the remote client.</returns>
		public virtual IPAddress GetRemoteAddress()
		{
			return peer;
		}

		/// <returns>input stream to read from the connected client.</returns>
		public virtual InputStream GetInputStream()
		{
			return rawIn;
		}

		/// <returns>output stream to send data to the connected client.</returns>
		public virtual OutputStream GetOutputStream()
		{
			return rawOut;
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual void Execute(Socket sock)
		{
			rawIn = new BufferedInputStream(sock.GetInputStream());
			rawOut = new BufferedOutputStream(sock.GetOutputStream());
			if (0 < daemon.GetTimeout())
			{
				sock.ReceiveTimeout = daemon.GetTimeout() * 1000;
			}
			string cmd = new PacketLineIn(rawIn).ReadStringRaw();
			int nul = cmd.IndexOf('\0');
			if (nul >= 0)
			{
				// Newer clients hide a "host" header behind this byte.
				// Currently we don't use it for anything, so we ignore
				// this portion of the command.
				//
				cmd = Sharpen.Runtime.Substring(cmd, 0, nul);
			}
			DaemonService srv = GetDaemon().MatchService(cmd);
			if (srv == null)
			{
				return;
			}
			sock.ReceiveTimeout = 0;
			srv.Execute(this, cmd);
		}
	}
}
