using System;
using NSch.Jce;
using Sharpen;

namespace NSch.Jce
{
	public class AES256CTR : NSch.Cipher
	{
		private const int ivsize = 16;

		private const int bsize = 32;

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
			string pad = "NoPadding";
			byte[] tmp;
			if (iv.Length > ivsize)
			{
				tmp = new byte[ivsize];
				System.Array.Copy(iv, 0, tmp, 0, tmp.Length);
				iv = tmp;
			}
			if (key.Length > bsize)
			{
				tmp = new byte[bsize];
				System.Array.Copy(key, 0, tmp, 0, tmp.Length);
				key = tmp;
			}
			try
			{
				SecretKeySpec keyspec = new SecretKeySpec(key, "AES");
				cipher = Sharpen.Cipher.GetInstance("AES/CTR/" + pad);
				cipher.Init((mode == ENCRYPT_MODE ? Sharpen.Cipher.ENCRYPT_MODE : Sharpen.Cipher.
					DECRYPT_MODE), keyspec, new IvParameterSpec(iv));
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
