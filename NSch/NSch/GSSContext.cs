using Sharpen;

namespace NSch
{
	public interface GSSContext
	{
		/// <exception cref="NSch.JSchException"></exception>
		void Create(string user, string host);

		bool IsEstablished();

		/// <exception cref="NSch.JSchException"></exception>
		byte[] Init(byte[] token, int s, int l);

		byte[] GetMIC(byte[] message, int s, int l);

		void Dispose();
	}
}
