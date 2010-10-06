using NSch;
using Sharpen;

namespace NSch
{
	public abstract class Compression
	{
		public const int INFLATER = 0;

		public const int DEFLATER = 1;

		public abstract void Init(int type, int level);

		public abstract int Compress(byte[] buf, int start, int len);

		public abstract byte[] Uncompress(byte[] buf, int start, int[] len);
	}
}
