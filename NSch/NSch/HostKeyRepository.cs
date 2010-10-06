using NSch;
using Sharpen;

namespace NSch
{
	public abstract class HostKeyRepository
	{
		public const int OK = 0;

		public const int NOT_INCLUDED = 1;

		public const int CHANGED = 2;

		public abstract int Check(string host, byte[] key);

		public abstract void Add(HostKey hostkey, UserInfo ui);

		public abstract void Remove(string host, string type);

		public abstract void Remove(string host, string type, byte[] key);

		public abstract string GetKnownHostsRepositoryID();

		public abstract HostKey[] GetHostKey();

		public abstract HostKey[] GetHostKey(string host, string type);
	}
}
