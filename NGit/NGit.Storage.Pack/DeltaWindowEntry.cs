using NGit.Storage.Pack;
using Sharpen;

namespace NGit.Storage.Pack
{
	internal class DeltaWindowEntry
	{
		internal ObjectToPack @object;

		/// <summary>Complete contents of this object.</summary>
		/// <remarks>Complete contents of this object. Lazily loaded.</remarks>
		internal byte[] buffer;

		/// <summary>Index of this object's content, to encode other deltas.</summary>
		/// <remarks>Index of this object's content, to encode other deltas. Lazily loaded.</remarks>
		internal DeltaIndex index;

		internal virtual void Set(ObjectToPack @object)
		{
			this.@object = @object;
			this.index = null;
			this.buffer = null;
		}

		/// <returns>current delta chain depth of this object.</returns>
		internal virtual int Depth()
		{
			return @object.GetDeltaDepth();
		}

		/// <returns>type of the object in this window entry.</returns>
		internal virtual int Type()
		{
			return @object.GetType();
		}

		/// <returns>estimated unpacked size of the object, in bytes .</returns>
		internal virtual int Size()
		{
			return @object.GetWeight();
		}

		/// <returns>true if there is no object stored in this entry.</returns>
		internal virtual bool Empty()
		{
			return @object == null;
		}
	}
}
