using System;
using NSch;
using NSch.Jce;
using Sharpen;

namespace NSch.Jce
{
	public class SHA1 : HASH
	{
		internal MessageDigest md;

		public virtual int GetBlockSize()
		{
			return 20;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Init()
		{
			try
			{
				md = MessageDigest.GetInstance("SHA-1");
			}
			catch (Exception e)
			{
				System.Console.Error.WriteLine(e);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Update(byte[] foo, int start, int len)
		{
			md.Update(foo, start, len);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual byte[] Digest()
		{
			return md.Digest();
		}
	}
}
