using System;
using System.IO;
using NGit;
using NGit.Storage.File;
using NGit.Storage.Pack;
using Sharpen;

namespace NGit.Storage.File
{
	internal class LocalObjectRepresentation : StoredObjectRepresentation
	{
		internal static LocalObjectRepresentation NewWhole(PackFile f, long p, long length
			)
		{
			LocalObjectRepresentation r = new _LocalObjectRepresentation_53();
			r.pack = f;
			r.offset = p;
			r.length = length;
			return r;
		}

		private sealed class _LocalObjectRepresentation_53 : LocalObjectRepresentation
		{
			public _LocalObjectRepresentation_53()
			{
			}

			public override int GetFormat()
			{
				return StoredObjectRepresentation.PACK_WHOLE;
			}
		}

		internal static LocalObjectRepresentation NewDelta(PackFile f, long p, long n, ObjectId
			 @base)
		{
			LocalObjectRepresentation r = new LocalObjectRepresentation.Delta();
			r.pack = f;
			r.offset = p;
			r.length = n;
			r.baseId = @base;
			return r;
		}

		internal static LocalObjectRepresentation NewDelta(PackFile f, long p, long n, long
			 @base)
		{
			LocalObjectRepresentation r = new LocalObjectRepresentation.Delta();
			r.pack = f;
			r.offset = p;
			r.length = n;
			r.baseOffset = @base;
			return r;
		}

		internal PackFile pack;

		internal long offset;

		internal long length;

		private long baseOffset;

		private ObjectId baseId;

		public override int GetWeight()
		{
			return (int)Math.Min(length, int.MaxValue);
		}

		public override ObjectId GetDeltaBase()
		{
			if (baseId == null && GetFormat() == PACK_DELTA)
			{
				try
				{
					baseId = pack.FindObjectForOffset(baseOffset);
				}
				catch (IOException)
				{
					return null;
				}
			}
			return baseId;
		}

		private sealed class Delta : LocalObjectRepresentation
		{
			public override int GetFormat()
			{
				return PACK_DELTA;
			}
		}
	}
}
