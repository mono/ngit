using NSch.Jcraft;
using Sharpen;

namespace NSch.Jcraft
{
	public class HMACSHA196 : HMACSHA1
	{
		private static readonly string name = "hmac-sha1-96";

		private const int BSIZE = 12;

		public override int GetBlockSize()
		{
			return BSIZE;
		}

		private readonly byte[] _buf16 = new byte[20];

		public override void DoFinal(byte[] buf, int offset)
		{
			base.DoFinal(_buf16, 0);
			System.Array.Copy(_buf16, 0, buf, offset, BSIZE);
		}

		public override string GetName()
		{
			return name;
		}
	}
}
