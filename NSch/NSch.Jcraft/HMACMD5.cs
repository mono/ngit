using System;
using NSch;
using NSch.Jcraft;
using Sharpen;

namespace NSch.Jcraft
{
	public class HMACMD5 : HMAC, MAC
	{
		private static readonly string name = "hmac-md5";

		public HMACMD5() : base()
		{
			MessageDigest md = null;
			try
			{
				md = MessageDigest.GetInstance("MD5");
			}
			catch (Exception e)
			{
				System.Console.Error.WriteLine(e);
			}
			SetH(md);
		}

		public virtual string GetName()
		{
			return name;
		}
	}
}
