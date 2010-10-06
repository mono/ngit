using NGit;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Description of an object stored in a pack file, including offset.</summary>
	/// <remarks>
	/// Description of an object stored in a pack file, including offset.
	/// <p>
	/// When objects are stored in packs Git needs the ObjectId and the offset
	/// (starting position of the object data) to perform random-access reads of
	/// objects from the pack. This extension of ObjectId includes the offset.
	/// </remarks>
	[System.Serializable]
	public class PackedObjectInfo : ObjectId
	{
		private long offset;

		private int crc;

		internal PackedObjectInfo(long headerOffset, int packedCRC, AnyObjectId id) : base
			(id)
		{
			offset = headerOffset;
			crc = packedCRC;
		}

		/// <summary>Create a new structure to remember information about an object.</summary>
		/// <remarks>Create a new structure to remember information about an object.</remarks>
		/// <param name="id">the identity of the object the new instance tracks.</param>
		protected internal PackedObjectInfo(AnyObjectId id) : base(id)
		{
		}

		/// <returns>
		/// offset in pack when object has been already written, or 0 if it
		/// has not been written yet
		/// </returns>
		public virtual long GetOffset()
		{
			return offset;
		}

		/// <summary>Set the offset in pack when object has been written to.</summary>
		/// <remarks>Set the offset in pack when object has been written to.</remarks>
		/// <param name="offset">offset where written object starts</param>
		public virtual void SetOffset(long offset)
		{
			this.offset = offset;
		}

		/// <returns>the 32 bit CRC checksum for the packed data.</returns>
		public virtual int GetCRC()
		{
			return crc;
		}

		/// <summary>Record the 32 bit CRC checksum for the packed data.</summary>
		/// <remarks>Record the 32 bit CRC checksum for the packed data.</remarks>
		/// <param name="crc">
		/// checksum of all packed data (including object type code,
		/// inflated length and delta base reference) as computed by
		/// <see cref="Sharpen.CRC32">Sharpen.CRC32</see>
		/// .
		/// </param>
		public virtual void SetCRC(int crc)
		{
			this.crc = crc;
		}
	}
}
