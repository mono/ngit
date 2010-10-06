using NSch.Jcraft;
using Sharpen;

namespace NSch.Jcraft
{
	public class HMACMD596 : HMACMD5
	{
		private static readonly string name = "hmac-md5-96";

		private const int BSIZE = 12;

		public override int GetBlockSize()
		{
			return BSIZE;
		}

		private readonly byte[] _buf16 = new byte[16];

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
