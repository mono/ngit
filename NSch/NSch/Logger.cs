using NSch;
using Sharpen;

namespace NSch
{
	public abstract class Logger
	{
		public const int DEBUG = 0;

		public const int INFO = 1;

		public const int WARN = 2;

		public const int ERROR = 3;

		public const int FATAL = 4;

		public abstract bool IsEnabled(int level);

		public abstract void Log(int level, string message);
	}
}
