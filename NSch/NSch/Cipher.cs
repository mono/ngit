using Sharpen;

namespace NSch
{
	public abstract class Cipher
	{
		public const int ENCRYPT_MODE = 0;

		public const int DECRYPT_MODE = 1;

		public abstract int GetIVSize();

		public abstract int GetBlockSize();

		/// <exception cref="System.Exception"></exception>
		public abstract void Init(int mode, byte[] key, byte[] iv);

		/// <exception cref="System.Exception"></exception>
		public abstract void Update(byte[] foo, int s1, int len, byte[] bar, int s2);

		public abstract bool IsCBC();
	}
}
