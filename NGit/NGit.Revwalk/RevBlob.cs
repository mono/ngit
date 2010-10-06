using NGit;
using NGit.Errors;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>A binary file, or a symbolic link.</summary>
	/// <remarks>A binary file, or a symbolic link.</remarks>
	[System.Serializable]
	public class RevBlob : RevObject
	{
		/// <summary>Create a new blob reference.</summary>
		/// <remarks>Create a new blob reference.</remarks>
		/// <param name="id">object name for the blob.</param>
		protected internal RevBlob(AnyObjectId id) : base(id)
		{
		}

		public sealed override int Type
		{
			get
			{
				return Constants.OBJ_BLOB;
			}
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		internal override void ParseHeaders(RevWalk walk)
		{
			if (walk.reader.Has(this))
			{
				flags |= PARSED;
			}
			else
			{
				throw new MissingObjectException(this, Type);
			}
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		internal override void ParseBody(RevWalk walk)
		{
			if ((flags & PARSED) == 0)
			{
				ParseHeaders(walk);
			}
		}
	}
}
