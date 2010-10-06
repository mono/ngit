using NGit.Storage.Pack;
using Sharpen;

namespace NGit.Storage.Pack
{
	internal class DeltaCache
	{
		private readonly long size;

		private readonly int entryLimit;

		private readonly ReferenceQueue<byte[]> queue;

		private long used;

		internal DeltaCache(PackConfig pc)
		{
			size = pc.GetDeltaCacheSize();
			entryLimit = pc.GetDeltaCacheLimit();
			queue = new ReferenceQueue<byte[]>();
		}

		internal virtual bool CanCache(int length, ObjectToPack src, ObjectToPack res)
		{
			// If the cache would overflow, don't store.
			//
			if (0 < size && size < used + length)
			{
				CheckForGarbageCollectedObjects();
				if (0 < size && size < used + length)
				{
					return false;
				}
			}
			if (length < entryLimit)
			{
				used += length;
				return true;
			}
			// If the combined source files are multiple megabytes but the delta
			// is on the order of a kilobyte or two, this was likely costly to
			// construct. Cache it anyway, even though its over the limit.
			//
			if (length >> 10 < (src.GetWeight() >> 20) + (res.GetWeight() >> 21))
			{
				used += length;
				return true;
			}
			return false;
		}

		internal virtual void Credit(int reservedSize)
		{
			used -= reservedSize;
		}

		internal virtual DeltaCache.Ref Cache(byte[] data, int actLen, int reservedSize)
		{
			// The caller may have had to allocate more space than is
			// required. If we are about to waste anything, shrink it.
			//
			data = Resize(data, actLen);
			// When we reserved space for this item we did it for the
			// inflated size of the delta, but we were just given the
			// compressed version. Adjust the cache cost to match.
			//
			if (reservedSize != data.Length)
			{
				used -= reservedSize;
				used += data.Length;
			}
			return new DeltaCache.Ref(data, queue);
		}

		internal virtual byte[] Resize(byte[] data, int actLen)
		{
			if (data.Length != actLen)
			{
				byte[] nbuf = new byte[actLen];
				System.Array.Copy(data, 0, nbuf, 0, actLen);
				data = nbuf;
			}
			return data;
		}

		private void CheckForGarbageCollectedObjects()
		{
			DeltaCache.Ref r;
			while ((r = (DeltaCache.Ref)queue.Poll()) != null)
			{
				used -= r.cost;
			}
		}

		internal class Ref : SoftReference<byte[]>
		{
			internal readonly int cost;

			internal Ref(byte[] array, ReferenceQueue<byte[]> queue) : base(array, queue)
			{
				cost = array.Length;
			}
		}
	}
}
