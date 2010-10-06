using System;
using NSch.Jcraft;
using Sharpen;

namespace NSch.Jcraft
{
	public class HMAC
	{
		private const int B = 64;

		private byte[] k_ipad = null;

		private byte[] k_opad = null;

		private MessageDigest md = null;

		private int bsize = 0;

		protected internal virtual void SetH(MessageDigest md)
		{
			this.md = md;
			bsize = md.GetDigestLength();
		}

		public virtual int GetBlockSize()
		{
			return bsize;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Init(byte[] key)
		{
			if (key.Length > bsize)
			{
				byte[] tmp = new byte[bsize];
				System.Array.Copy(key, 0, tmp, 0, bsize);
				key = tmp;
			}
			if (key.Length > B)
			{
				md.Update(key, 0, key.Length);
				key = md.Digest();
			}
			k_ipad = new byte[B];
			System.Array.Copy(key, 0, k_ipad, 0, key.Length);
			k_opad = new byte[B];
			System.Array.Copy(key, 0, k_opad, 0, key.Length);
			for (int i = 0; i < B; i++)
			{
				k_ipad[i] ^= unchecked((byte)unchecked((int)(0x36)));
				k_opad[i] ^= unchecked((byte)unchecked((int)(0x5c)));
			}
			md.Update(k_ipad, 0, B);
		}

		private readonly byte[] tmp = new byte[4];

		public virtual void Update(int i)
		{
			tmp[0] = unchecked((byte)((int)(((uint)i) >> 24)));
			tmp[1] = unchecked((byte)((int)(((uint)i) >> 16)));
			tmp[2] = unchecked((byte)((int)(((uint)i) >> 8)));
			tmp[3] = unchecked((byte)i);
			Update(tmp, 0, 4);
		}

		public virtual void Update(byte[] foo, int s, int l)
		{
			md.Update(foo, s, l);
		}

		public virtual void DoFinal(byte[] buf, int offset)
		{
			byte[] result = md.Digest();
			md.Update(k_opad, 0, B);
			md.Update(result, 0, bsize);
			try
			{
				md.Digest(buf, offset, bsize);
			}
			catch (Exception)
			{
			}
			md.Update(k_ipad, 0, B);
		}
	}
}
