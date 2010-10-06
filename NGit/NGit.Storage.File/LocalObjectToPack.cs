using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Storage.Pack;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>
	/// <see cref="NGit.Storage.Pack.ObjectToPack">NGit.Storage.Pack.ObjectToPack</see>
	/// for
	/// <see cref="ObjectDirectory">ObjectDirectory</see>
	/// .
	/// </summary>
	[System.Serializable]
	internal class LocalObjectToPack : ObjectToPack
	{
		/// <summary>Pack to reuse compressed data from, otherwise null.</summary>
		/// <remarks>Pack to reuse compressed data from, otherwise null.</remarks>
		internal PackFile pack;

		/// <summary>
		/// Offset of the object's header in
		/// <see cref="pack">pack</see>
		/// .
		/// </summary>
		internal long offset;

		/// <summary>Length of the data section of the object.</summary>
		/// <remarks>Length of the data section of the object.</remarks>
		internal long length;

		public LocalObjectToPack(RevObject obj) : base(obj)
		{
		}

		protected internal override void ClearReuseAsIs()
		{
			base.ClearReuseAsIs();
			pack = null;
		}

		public override void Select(StoredObjectRepresentation @ref)
		{
			LocalObjectRepresentation ptr = (LocalObjectRepresentation)@ref;
			this.pack = ptr.pack;
			this.offset = ptr.offset;
			this.length = ptr.length;
		}
	}
}
