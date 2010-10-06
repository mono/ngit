using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	internal class FS_POSIX_Java5 : FS
	{
		public override bool SupportsExecute()
		{
			return false;
		}

		public override bool CanExecute(FilePath f)
		{
			return false;
		}

		public override bool SetExecute(FilePath f, bool canExec)
		{
			return false;
		}

		public override bool RetryFailedLockFileCommit()
		{
			return false;
		}
	}
}
