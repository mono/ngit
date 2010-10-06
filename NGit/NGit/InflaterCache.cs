using System;
using ICSharpCode.SharpZipLib.Zip.Compression;
using Sharpen;

namespace NGit
{
	/// <summary>Creates zlib based inflaters as necessary for object decompression.</summary>
	/// <remarks>Creates zlib based inflaters as necessary for object decompression.</remarks>
	public class InflaterCache
	{
		private const int SZ = 4;

		private static readonly Inflater[] inflaterCache;

		private static int openInflaterCount;

		static InflaterCache()
		{
			inflaterCache = new Inflater[SZ];
		}

		/// <summary>Obtain an Inflater for decompression.</summary>
		/// <remarks>
		/// Obtain an Inflater for decompression.
		/// <p>
		/// Inflaters obtained through this cache should be returned (if possible) by
		/// <see cref="Release(ICSharpCode.SharpZipLib.Zip.Compression.Inflater)">Release(ICSharpCode.SharpZipLib.Zip.Compression.Inflater)
		/// 	</see>
		/// to avoid garbage collection and reallocation.
		/// </remarks>
		/// <returns>an available inflater. Never null.</returns>
		public static Inflater Get()
		{
			Inflater r = GetImpl();
			return r != null ? r : new Inflater(false);
		}

		private static Inflater GetImpl()
		{
			lock (typeof(InflaterCache))
			{
				if (openInflaterCount > 0)
				{
					Inflater r = inflaterCache[--openInflaterCount];
					inflaterCache[openInflaterCount] = null;
					return r;
				}
				return null;
			}
		}

		/// <summary>Release an inflater previously obtained from this cache.</summary>
		/// <remarks>Release an inflater previously obtained from this cache.</remarks>
		/// <param name="i">
		/// the inflater to return. May be null, in which case this method
		/// does nothing.
		/// </param>
		public static void Release(Inflater i)
		{
			if (i != null)
			{
				i.Reset();
				if (ReleaseImpl(i))
				{
					i.Finish();
				}
			}
		}

		private static bool ReleaseImpl(Inflater i)
		{
			lock (typeof(InflaterCache))
			{
				if (openInflaterCount < SZ)
				{
					inflaterCache[openInflaterCount++] = i;
					return false;
				}
				return true;
			}
		}

		public InflaterCache()
		{
			throw new NotSupportedException();
		}
	}
}
