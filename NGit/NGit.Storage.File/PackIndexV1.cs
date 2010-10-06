using System;
using System.Collections.Generic;
using NGit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	internal class PackIndexV1 : PackIndex
	{
		private const int IDX_HDR_LEN = 256 * 4;

		private readonly long[] idxHeader;

		private byte[][] idxdata;

		private long objectCnt;

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		internal PackIndexV1(InputStream fd, byte[] hdr)
		{
			byte[] fanoutTable = new byte[IDX_HDR_LEN];
			System.Array.Copy(hdr, 0, fanoutTable, 0, hdr.Length);
			IOUtil.ReadFully(fd, fanoutTable, hdr.Length, IDX_HDR_LEN - hdr.Length);
			idxHeader = new long[256];
			// really unsigned 32-bit...
			for (int k = 0; k < idxHeader.Length; k++)
			{
				idxHeader[k] = NB.DecodeUInt32(fanoutTable, k * 4);
			}
			idxdata = new byte[idxHeader.Length][];
			for (int k_1 = 0; k_1 < idxHeader.Length; k_1++)
			{
				int n;
				if (k_1 == 0)
				{
					n = (int)(idxHeader[k_1]);
				}
				else
				{
					n = (int)(idxHeader[k_1] - idxHeader[k_1 - 1]);
				}
				if (n > 0)
				{
					idxdata[k_1] = new byte[n * (Constants.OBJECT_ID_LENGTH + 4)];
					IOUtil.ReadFully(fd, idxdata[k_1], 0, idxdata[k_1].Length);
				}
			}
			objectCnt = idxHeader[255];
			packChecksum = new byte[20];
			IOUtil.ReadFully(fd, packChecksum, 0, packChecksum.Length);
		}

		internal override long GetObjectCount()
		{
			return objectCnt;
		}

		internal override long GetOffset64Count()
		{
			long n64 = 0;
			foreach (PackIndex.MutableEntry e in this)
			{
				if (e.GetOffset() >= int.MaxValue)
				{
					n64++;
				}
			}
			return n64;
		}

		internal override ObjectId GetObjectId(long nthPosition)
		{
			int levelOne = System.Array.BinarySearch(idxHeader, nthPosition + 1);
			long @base;
			if (levelOne >= 0)
			{
				// If we hit the bucket exactly the item is in the bucket, or
				// any bucket before it which has the same object count.
				//
				@base = idxHeader[levelOne];
				while (levelOne > 0 && @base == idxHeader[levelOne - 1])
				{
					levelOne--;
				}
			}
			else
			{
				// The item is in the bucket we would insert it into.
				//
				levelOne = -(levelOne + 1);
			}
			@base = levelOne > 0 ? idxHeader[levelOne - 1] : 0;
			int p = (int)(nthPosition - @base);
			int dataIdx = IdOffset(p);
			return ObjectId.FromRaw(idxdata[levelOne], dataIdx);
		}

		internal override long FindOffset(AnyObjectId objId)
		{
			int levelOne = objId.FirstByte;
			byte[] data = idxdata[levelOne];
			if (data == null)
			{
				return -1;
			}
			int high = data.Length / (4 + Constants.OBJECT_ID_LENGTH);
			int low = 0;
			do
			{
				int mid = (int)(((uint)(low + high)) >> 1);
				int pos = IdOffset(mid);
				int cmp = objId.CompareTo(data, pos);
				if (cmp < 0)
				{
					high = mid;
				}
				else
				{
					if (cmp == 0)
					{
						int b0 = data[pos - 4] & unchecked((int)(0xff));
						int b1 = data[pos - 3] & unchecked((int)(0xff));
						int b2 = data[pos - 2] & unchecked((int)(0xff));
						int b3 = data[pos - 1] & unchecked((int)(0xff));
						return (((long)b0) << 24) | (b1 << 16) | (b2 << 8) | (b3);
					}
					else
					{
						low = mid + 1;
					}
				}
			}
			while (low < high);
			return -1;
		}

		internal override long FindCRC32(AnyObjectId objId)
		{
			throw new NotSupportedException();
		}

		internal override bool HasCRC32Support()
		{
			return false;
		}

		public override Sharpen.Iterator<PackIndex.MutableEntry> Iterator()
		{
			return new PackIndexV1.IndexV1Iterator(this);
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal override void Resolve(ICollection<ObjectId> matches, AbbreviatedObjectId
			 id, int matchLimit)
		{
			byte[] data = idxdata[id.FirstByte];
			if (data == null)
			{
				return;
			}
			int max = data.Length / (4 + Constants.OBJECT_ID_LENGTH);
			int high = max;
			int low = 0;
			do
			{
				int p = (int)(((uint)(low + high)) >> 1);
				int cmp = id.PrefixCompare(data, IdOffset(p));
				if (cmp < 0)
				{
					high = p;
				}
				else
				{
					if (cmp == 0)
					{
						// We may have landed in the middle of the matches.  Move
						// backwards to the start of matches, then walk forwards.
						//
						while (0 < p && id.PrefixCompare(data, IdOffset(p - 1)) == 0)
						{
							p--;
						}
						for (; p < max && id.PrefixCompare(data, IdOffset(p)) == 0; p++)
						{
							matches.AddItem(ObjectId.FromRaw(data, IdOffset(p)));
							if (matches.Count > matchLimit)
							{
								break;
							}
						}
						return;
					}
					else
					{
						low = p + 1;
					}
				}
			}
			while (low < high);
		}

		private static int IdOffset(int mid)
		{
			return ((4 + Constants.OBJECT_ID_LENGTH) * mid) + 4;
		}

		private class IndexV1Iterator : PackIndex.EntriesIterator
		{
			private int levelOne;

			private int levelTwo;

			protected internal override PackIndex.MutableEntry InitEntry()
			{
				return new _MutableEntry_219(this);
			}

			private sealed class _MutableEntry_219 : PackIndex.MutableEntry
			{
				public _MutableEntry_219(IndexV1Iterator _enclosing)
				{
					this._enclosing = _enclosing;
				}

				internal override void EnsureId()
				{
					this.idBuffer.FromRaw(this._enclosing._enclosing.idxdata[this._enclosing.levelOne
						], this._enclosing.levelTwo - Constants.OBJECT_ID_LENGTH);
				}

				private readonly IndexV1Iterator _enclosing;
			}

			public override PackIndex.MutableEntry Next()
			{
				for (; this.levelOne < this._enclosing.idxdata.Length; this.levelOne++)
				{
					if (this._enclosing.idxdata[this.levelOne] == null)
					{
						continue;
					}
					if (this.levelTwo < this._enclosing.idxdata[this.levelOne].Length)
					{
						this.entry.offset = NB.DecodeUInt32(this._enclosing.idxdata[this.levelOne], this.
							levelTwo);
						this.levelTwo += Constants.OBJECT_ID_LENGTH + 4;
						this.returnedNumber++;
						return this.entry;
					}
					this.levelTwo = 0;
				}
				throw new NoSuchElementException();
			}

			internal IndexV1Iterator(PackIndexV1 _enclosing) : base(_enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly PackIndexV1 _enclosing;
		}
	}
}
