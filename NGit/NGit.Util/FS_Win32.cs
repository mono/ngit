using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	internal class FS_Win32 : FS
	{
		internal static bool Detect()
		{
			string osDotName = AccessController.DoPrivileged(new _PrivilegedAction_54());
			return osDotName != null && StringUtils.ToLowerCase(osDotName).IndexOf("windows")
				 != -1;
		}

		private sealed class _PrivilegedAction_54 : PrivilegedAction<string>
		{
			public _PrivilegedAction_54()
			{
			}

			public string Run()
			{
				return Runtime.GetProperty("os.name");
			}
		}

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
			return true;
		}
	}
}
