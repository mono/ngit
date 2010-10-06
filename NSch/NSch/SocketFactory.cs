using System.Net.Sockets;
using NSch;
using Sharpen;

namespace NSch
{
	public interface SocketFactory
	{
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Sharpen.UnknownHostException"></exception>
		Socket CreateSocket(string host, int port);

		/// <exception cref="System.IO.IOException"></exception>
		InputStream GetInputStream(Socket socket);

		/// <exception cref="System.IO.IOException"></exception>
		OutputStream GetOutputStream(Socket socket);
	}
}
