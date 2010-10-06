using System;
using NSch;
using NSch.Jcraft;
using Sharpen;

namespace NSch.Jcraft
{
	public class HMACSHA1 : HMAC, MAC
	{
		private static readonly string name = "hmac-sha1";

		public HMACSHA1() : base()
		{
			MessageDigest md = null;
			try
			{
				md = MessageDigest.GetInstance("SHA-1");
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
