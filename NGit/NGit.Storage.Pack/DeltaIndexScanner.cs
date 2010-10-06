using NGit.Storage.Pack;
using Sharpen;

namespace NGit.Storage.Pack
{
	/// <summary>
	/// Supports
	/// <see cref="DeltaIndex">DeltaIndex</see>
	/// by performing a partial scan of the content.
	/// </summary>
	internal class DeltaIndexScanner
	{
		internal readonly int[] table;

		internal readonly long[] entries;

		internal readonly int[] next;

		internal readonly int tableMask;

		private int entryCnt;

		internal DeltaIndexScanner(byte[] raw, int len)
		{
			// To save memory the buckets for hash chains are stored in correlated
			// arrays. This permits us to get 3 values per entry, without paying
			// the penalty for an object header on each entry.
			// Clip the length so it falls on a block boundary. We won't
			// bother to scan the final partial block.
			//
			len -= (len % DeltaIndex.BLKSZ);
			int worstCaseBlockCnt = len / DeltaIndex.BLKSZ;
			if (worstCaseBlockCnt < 1)
			{
				table = new int[] {  };
				tableMask = 0;
				entries = new long[] {  };
				next = new int[] {  };
			}
			else
			{
				table = new int[TableSize(worstCaseBlockCnt)];
				tableMask = table.Length - 1;
				// As we insert blocks we preincrement so that 0 is never a
				// valid entry. Therefore we have to allocate one extra space.
				//
				entries = new long[1 + worstCaseBlockCnt];
				next = new int[entries.Length];
				Scan(raw, len);
			}
		}

		private void Scan(byte[] raw, int end)
		{
			// We scan the input backwards, and always insert onto the
			// front of the chain. This ensures that chains will have lower
			// offsets at the front of the chain, allowing us to prefer the
			// earlier match rather than the later match.
			//
			int lastHash = 0;
			int ptr = end - DeltaIndex.BLKSZ;
			do
			{
				int key = DeltaIndex.HashBlock(raw, ptr);
				int tIdx = key & tableMask;
				int head = table[tIdx];
				if (head != 0 && lastHash == key)
				{
					// Two consecutive blocks have the same content hash,
					// prefer the earlier block because we want to use the
					// longest sequence we can during encoding.
					//
					entries[head] = (((long)key) << 32) | ptr;
				}
				else
				{
					int eIdx = ++entryCnt;
					entries[eIdx] = (((long)key) << 32) | ptr;
					next[eIdx] = head;
					table[tIdx] = eIdx;
				}
				lastHash = key;
				ptr -= DeltaIndex.BLKSZ;
			}
			while (0 <= ptr);
		}

		private static int TableSize(int worstCaseBlockCnt)
		{
			int shift = 32 - Sharpen.Extensions.NumberOfLeadingZeros(worstCaseBlockCnt);
			int sz = 1 << (shift - 1);
			if (sz < worstCaseBlockCnt)
			{
				sz <<= 1;
			}
			return sz;
		}
	}
}
