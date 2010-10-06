using System.Net;
using System.Net.Sockets;
using NSch;
using Sharpen;

namespace NSch
{
	public interface ServerSocketFactory
	{
		/// <exception cref="System.IO.IOException"></exception>
		Socket CreateServerSocket(int port, int backlog, IPAddress bindAddr);
	}
}
