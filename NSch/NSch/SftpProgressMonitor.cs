using NSch;
using Sharpen;

namespace NSch
{
	public abstract class SftpProgressMonitor
	{
		public const int PUT = 0;

		public const int GET = 1;

		public abstract void Init(int op, string src, string dest, long max);

		public abstract bool Count(long count);

		public abstract void End();
	}
}
