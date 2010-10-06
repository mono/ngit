using NSch;
using Sharpen;

namespace NSch
{
	public class CipherNone : NSch.Cipher
	{
		private const int ivsize = 8;

		private const int bsize = 16;

		public override int GetIVSize()
		{
			return ivsize;
		}

		public override int GetBlockSize()
		{
			return bsize;
		}

		/// <exception cref="System.Exception"></exception>
		public override void Init(int mode, byte[] key, byte[] iv)
		{
		}

		/// <exception cref="System.Exception"></exception>
		public override void Update(byte[] foo, int s1, int len, byte[] bar, int s2)
		{
		}

		public override bool IsCBC()
		{
			return false;
		}
	}
}
