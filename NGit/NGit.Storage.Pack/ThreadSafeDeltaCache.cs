using NGit.Storage.Pack;
using Sharpen;

namespace NGit.Storage.Pack
{
	internal class ThreadSafeDeltaCache : DeltaCache
	{
		private readonly ReentrantLock Lock;

		internal ThreadSafeDeltaCache(PackConfig pc) : base(pc)
		{
			Lock = new ReentrantLock();
		}

		internal override bool CanCache(int length, ObjectToPack src, ObjectToPack res)
		{
			Lock.Lock();
			try
			{
				return base.CanCache(length, src, res);
			}
			finally
			{
				Lock.Unlock();
			}
		}

		internal override void Credit(int reservedSize)
		{
			Lock.Lock();
			try
			{
				base.Credit(reservedSize);
			}
			finally
			{
				Lock.Unlock();
			}
		}

		internal override DeltaCache.Ref Cache(byte[] data, int actLen, int reservedSize)
		{
			data = Resize(data, actLen);
			Lock.Lock();
			try
			{
				return base.Cache(data, actLen, reservedSize);
			}
			finally
			{
				Lock.Unlock();
			}
		}
	}
}
