using NSch;
using Sharpen;

namespace NSch
{
	public interface HASH
	{
		/// <exception cref="System.Exception"></exception>
		void Init();

		int GetBlockSize();

		/// <exception cref="System.Exception"></exception>
		void Update(byte[] foo, int start, int len);

		/// <exception cref="System.Exception"></exception>
		byte[] Digest();
	}
}
