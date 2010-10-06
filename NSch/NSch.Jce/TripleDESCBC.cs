using System;
using NSch.Jce;
using Sharpen;

namespace NSch.Jce
{
	public class TripleDESCBC : NSch.Cipher
	{
		private const int ivsize = 8;

		private const int bsize = 24;

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
			//if(padding) pad="PKCS5Padding";
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
				cipher = Sharpen.Cipher.GetInstance("DESede/CBC/" + pad);
				DESedeKeySpec keyspec = new DESedeKeySpec(key);
				SecretKeyFactory keyfactory = SecretKeyFactory.GetInstance("DESede");
				SecretKey _key = keyfactory.GenerateSecret(keyspec);
				cipher.Init((mode == ENCRYPT_MODE ? Sharpen.Cipher.ENCRYPT_MODE : Sharpen.Cipher.
					DECRYPT_MODE), _key, new IvParameterSpec(iv));
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
			return true;
		}
	}
}
