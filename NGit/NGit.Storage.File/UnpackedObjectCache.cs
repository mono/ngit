using System;
using NGit;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>Remembers objects that are currently unpacked.</summary>
	/// <remarks>Remembers objects that are currently unpacked.</remarks>
	internal class UnpackedObjectCache
	{
		private const int INITIAL_BITS = 5;

		private const int MAX_BITS = 11;

		private volatile UnpackedObjectCache.Table table;

		public UnpackedObjectCache()
		{
			// size = 32
			// size = 2048
			table = new UnpackedObjectCache.Table(INITIAL_BITS);
		}

		internal virtual bool IsUnpacked(AnyObjectId objectId)
		{
			return table.Contains(objectId);
		}

		internal virtual void Add(AnyObjectId objectId)
		{
			UnpackedObjectCache.Table t = table;
			if (t.Add(objectId))
			{
			}
			else
			{
				// The object either already exists in the table, or was
				// successfully added. Either way leave the table alone.
				//
				// The object won't fit into the table. Implement a crude
				// cache removal by just dropping the table away, but double
				// it in size for the next incarnation.
				//
				UnpackedObjectCache.Table n = new UnpackedObjectCache.Table(Math.Min(t.bits + 1, 
					MAX_BITS));
				n.Add(objectId);
				table = n;
			}
		}

		internal virtual void Remove(AnyObjectId objectId)
		{
			if (IsUnpacked(objectId))
			{
				Clear();
			}
		}

		internal virtual void Clear()
		{
			table = new UnpackedObjectCache.Table(INITIAL_BITS);
		}

		private class Table
		{
			private const int MAX_CHAIN = 8;

			private readonly AtomicReferenceArray<ObjectId> ids;

			private readonly int shift;

			internal readonly int bits;

			internal Table(int bits)
			{
				this.ids = new AtomicReferenceArray<ObjectId>(1 << bits);
				this.shift = 32 - bits;
				this.bits = bits;
			}

			internal virtual bool Contains(AnyObjectId toFind)
			{
				int i = Index(toFind);
				for (int n = 0; n < MAX_CHAIN; n++)
				{
					ObjectId obj = ids.Get(i);
					if (obj == null)
					{
						break;
					}
					if (AnyObjectId.Equals(obj, toFind))
					{
						return true;
					}
					if (++i == ids.Length())
					{
						i = 0;
					}
				}
				return false;
			}

			internal virtual bool Add(AnyObjectId toAdd)
			{
				int i = Index(toAdd);
				for (int n = 0; n < MAX_CHAIN; )
				{
					ObjectId obj = ids.Get(i);
					if (obj == null)
					{
						if (ids.CompareAndSet(i, null, toAdd.Copy()))
						{
							return true;
						}
						else
						{
							continue;
						}
					}
					if (AnyObjectId.Equals(obj, toAdd))
					{
						return true;
					}
					if (++i == ids.Length())
					{
						i = 0;
					}
					n++;
				}
				return false;
			}

			private int Index(AnyObjectId id)
			{
				return (int)(((uint)id.GetHashCode()) >> shift);
			}
		}
	}
}
