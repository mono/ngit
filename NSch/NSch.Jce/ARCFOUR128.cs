using System;
using NSch.Jce;
using Sharpen;

namespace NSch.Jce
{
	public class ARCFOUR128 : NSch.Cipher
	{
		private const int ivsize = 8;

		private const int bsize = 16;

		private const int skip = 1536;

		private Sharpen.Cipher cipher;

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
			byte[] tmp;
			if (key.Length > bsize)
			{
				tmp = new byte[bsize];
				System.Array.Copy(key, 0, tmp, 0, tmp.Length);
				key = tmp;
			}
			try
			{
				cipher = Sharpen.Cipher.GetInstance("RC4");
				SecretKeySpec _key = new SecretKeySpec(key, "RC4");
				cipher.Init((mode == ENCRYPT_MODE ? Sharpen.Cipher.ENCRYPT_MODE : Sharpen.Cipher.
					DECRYPT_MODE), _key);
				byte[] foo = new byte[1];
				for (int i = 0; i < skip; i++)
				{
					cipher.Update(foo, 0, 1, foo, 0);
				}
			}
			catch (Exception e)
			{
				cipher = null;
				throw;
			}
		}

		/// <exception cref="System.Exception"></exception>
		public override void Update(byte[] foo, int s1, int len, byte[] bar, int s2)
		{
			cipher.Update(foo, s1, len, bar, s2);
		}

		public override bool IsCBC()
		{
			return false;
		}
	}
}
