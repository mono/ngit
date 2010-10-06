using System.IO;
using NGit;
using NGit.Errors;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	internal class LargePackedWholeObject : ObjectLoader
	{
		private readonly int type;

		private readonly long size;

		private readonly long objectOffset;

		private readonly int headerLength;

		private readonly PackFile pack;

		private readonly FileObjectDatabase db;

		internal LargePackedWholeObject(int type, long size, long objectOffset, int headerLength
			, PackFile pack, FileObjectDatabase db)
		{
			this.type = type;
			this.size = size;
			this.objectOffset = objectOffset;
			this.headerLength = headerLength;
			this.pack = pack;
			this.db = db;
		}

		public override int GetType()
		{
			return type;
		}

		public override long GetSize()
		{
			return size;
		}

		public override bool IsLarge()
		{
			return true;
		}

		/// <exception cref="NGit.Errors.LargeObjectException"></exception>
		public override byte[] GetCachedBytes()
		{
			try
			{
				throw new LargeObjectException(GetObjectId());
			}
			catch (IOException cannotObtainId)
			{
				LargeObjectException err = new LargeObjectException();
				Sharpen.Extensions.InitCause(err, cannotObtainId);
				throw err;
			}
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override ObjectStream OpenStream()
		{
			WindowCursor wc = new WindowCursor(db);
			InputStream @in;
			try
			{
				@in = new PackInputStream(pack, objectOffset + headerLength, wc);
			}
			catch (IOException)
			{
				// If the pack file cannot be pinned into the cursor, it
				// probably was repacked recently. Go find the object
				// again and open the stream from that location instead.
				//
				return wc.Open(GetObjectId(), type).OpenStream();
			}
			@in = new BufferedInputStream(new InflaterInputStream(@in, wc.Inflater(), 8192), 
				8192);
			//
			//
			//
			//
			//
			return new ObjectStream.Filter(type, size, @in);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private ObjectId GetObjectId()
		{
			return pack.FindObjectForOffset(objectOffset);
		}
	}
}
