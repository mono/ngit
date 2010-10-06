using System.Net.Sockets;
using NSch;
using Sharpen;

namespace NSch
{
	public interface Proxy
	{
		/// <exception cref="System.Exception"></exception>
		void Connect(SocketFactory socket_factory, string host, int port, int timeout);

		InputStream GetInputStream();

		OutputStream GetOutputStream();

		Socket GetSocket();

		void Close();
	}
}
