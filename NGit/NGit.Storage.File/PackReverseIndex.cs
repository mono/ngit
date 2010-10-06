using System;
using NGit;
using NGit.Errors;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>
	/// <p>
	/// Reverse index for forward pack index.
	/// </summary>
	/// <remarks>
	/// <p>
	/// Reverse index for forward pack index. Provides operations based on offset
	/// instead of object id. Such offset-based reverse lookups are performed in
	/// O(log n) time.
	/// </p>
	/// </remarks>
	/// <seealso cref="PackIndex">PackIndex</seealso>
	/// <seealso cref="PackFile">PackFile</seealso>
	internal class PackReverseIndex
	{
		/// <summary>Index we were created from, and that has our ObjectId data.</summary>
		/// <remarks>Index we were created from, and that has our ObjectId data.</remarks>
		private readonly PackIndex index;

		/// <summary>(offset31, truly) Offsets accommodating in 31 bits.</summary>
		/// <remarks>(offset31, truly) Offsets accommodating in 31 bits.</remarks>
		private readonly int[] offsets32;

		/// <summary>Offsets not accommodating in 31 bits.</summary>
		/// <remarks>Offsets not accommodating in 31 bits.</remarks>
		private readonly long[] offsets64;

		/// <summary>
		/// Position of the corresponding
		/// <see cref="offsets32">offsets32</see>
		/// in
		/// <see cref="index">index</see>
		/// .
		/// </summary>
		private readonly int[] nth32;

		/// <summary>
		/// Position of the corresponding
		/// <see cref="offsets64">offsets64</see>
		/// in
		/// <see cref="index">index</see>
		/// .
		/// </summary>
		private readonly int[] nth64;

		/// <summary>
		/// Create reverse index from straight/forward pack index, by indexing all
		/// its entries.
		/// </summary>
		/// <remarks>
		/// Create reverse index from straight/forward pack index, by indexing all
		/// its entries.
		/// </remarks>
		/// <param name="packIndex">forward index - entries to (reverse) index.</param>
		internal PackReverseIndex(PackIndex packIndex)
		{
			index = packIndex;
			long cnt = index.GetObjectCount();
			long n64 = index.GetOffset64Count();
			long n32 = cnt - n64;
			if (n32 > int.MaxValue || n64 > int.MaxValue || cnt > unchecked((long)(0xffffffffL
				)))
			{
				throw new ArgumentException(JGitText.Get().hugeIndexesAreNotSupportedByJgitYet);
			}
			offsets32 = new int[(int)n32];
			offsets64 = new long[(int)n64];
			nth32 = new int[offsets32.Length];
			nth64 = new int[offsets64.Length];
			int i32 = 0;
			int i64 = 0;
			foreach (PackIndex.MutableEntry me in index)
			{
				long o = me.GetOffset();
				if (o < int.MaxValue)
				{
					offsets32[i32++] = (int)o;
				}
				else
				{
					offsets64[i64++] = o;
				}
			}
			Arrays.Sort(offsets32);
			Arrays.Sort(offsets64);
			int nth = 0;
			foreach (PackIndex.MutableEntry me_1 in index)
			{
				long o = me_1.GetOffset();
				if (o < int.MaxValue)
				{
					nth32[System.Array.BinarySearch(offsets32, (int)o)] = nth++;
				}
				else
				{
					nth64[System.Array.BinarySearch(offsets64, o)] = nth++;
				}
			}
		}

		/// <summary>
		/// Search for object id with the specified start offset in this pack
		/// (reverse) index.
		/// </summary>
		/// <remarks>
		/// Search for object id with the specified start offset in this pack
		/// (reverse) index.
		/// </remarks>
		/// <param name="offset">start offset of object to find.</param>
		/// <returns>object id for this offset, or null if no object was found.</returns>
		internal virtual ObjectId FindObject(long offset)
		{
			if (offset <= int.MaxValue)
			{
				int i32 = System.Array.BinarySearch(offsets32, (int)offset);
				if (i32 < 0)
				{
					return null;
				}
				return index.GetObjectId(nth32[i32]);
			}
			else
			{
				int i64 = System.Array.BinarySearch(offsets64, offset);
				if (i64 < 0)
				{
					return null;
				}
				return index.GetObjectId(nth64[i64]);
			}
		}

		/// <summary>
		/// Search for the next offset to the specified offset in this pack (reverse)
		/// index.
		/// </summary>
		/// <remarks>
		/// Search for the next offset to the specified offset in this pack (reverse)
		/// index.
		/// </remarks>
		/// <param name="offset">
		/// start offset of previous object (must be valid-existing
		/// offset).
		/// </param>
		/// <param name="maxOffset">
		/// maximum offset in a pack (returned when there is no next
		/// offset).
		/// </param>
		/// <returns>
		/// offset of the next object in a pack or maxOffset if provided
		/// offset was the last one.
		/// </returns>
		/// <exception cref="NGit.Errors.CorruptObjectException">when there is no object with the provided offset.
		/// 	</exception>
		internal virtual long FindNextOffset(long offset, long maxOffset)
		{
			if (offset <= int.MaxValue)
			{
				int i32 = System.Array.BinarySearch(offsets32, (int)offset);
				if (i32 < 0)
				{
					throw new CorruptObjectException(MessageFormat.Format(JGitText.Get().cantFindObjectInReversePackIndexForTheSpecifiedOffset
						, offset));
				}
				if (i32 + 1 == offsets32.Length)
				{
					if (offsets64.Length > 0)
					{
						return offsets64[0];
					}
					return maxOffset;
				}
				return offsets32[i32 + 1];
			}
			else
			{
				int i64 = System.Array.BinarySearch(offsets64, offset);
				if (i64 < 0)
				{
					throw new CorruptObjectException(MessageFormat.Format(JGitText.Get().cantFindObjectInReversePackIndexForTheSpecifiedOffset
						, offset));
				}
				if (i64 + 1 == offsets64.Length)
				{
					return maxOffset;
				}
				return offsets64[i64 + 1];
			}
		}
	}
}
