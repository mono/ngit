using NGit;
using NGit.Storage.Pack;
using Sharpen;

namespace NGit.Storage.Pack
{
	/// <summary>
	/// An object representation
	/// <see cref="PackWriter">PackWriter</see>
	/// can consider for packing.
	/// </summary>
	public class StoredObjectRepresentation
	{
		/// <summary>
		/// Special unknown value for
		/// <see cref="GetWeight()">GetWeight()</see>
		/// .
		/// </summary>
		public const int WEIGHT_UNKNOWN = int.MaxValue;

		/// <summary>Stored in pack format, as a delta to another object.</summary>
		/// <remarks>Stored in pack format, as a delta to another object.</remarks>
		public const int PACK_DELTA = 0;

		/// <summary>Stored in pack format, without delta.</summary>
		/// <remarks>Stored in pack format, without delta.</remarks>
		public const int PACK_WHOLE = 1;

		/// <summary>Only available after inflating to canonical format.</summary>
		/// <remarks>Only available after inflating to canonical format.</remarks>
		public const int FORMAT_OTHER = 2;

		/// <returns>
		/// relative size of this object's packed form. The special value
		/// <see cref="WEIGHT_UNKNOWN">WEIGHT_UNKNOWN</see>
		/// can be returned to indicate the
		/// implementation doesn't know, or cannot supply the weight up
		/// front.
		/// </returns>
		public virtual int GetWeight()
		{
			return WEIGHT_UNKNOWN;
		}

		/// <returns>
		/// true if this is a delta against another object and this is stored
		/// in pack delta format.
		/// </returns>
		public virtual int GetFormat()
		{
			return FORMAT_OTHER;
		}

		/// <returns>
		/// identity of the object this delta applies to in order to recover
		/// the original object content. This method should only be called if
		/// <see cref="GetFormat()">GetFormat()</see>
		/// returned
		/// <see cref="PACK_DELTA">PACK_DELTA</see>
		/// .
		/// </returns>
		public virtual ObjectId GetDeltaBase()
		{
			return null;
		}
	}
}
