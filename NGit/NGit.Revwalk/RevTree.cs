using NGit;
using NGit.Errors;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>A reference to a tree of subtrees/files.</summary>
	/// <remarks>A reference to a tree of subtrees/files.</remarks>
	[System.Serializable]
	public class RevTree : RevObject
	{
		/// <summary>Create a new tree reference.</summary>
		/// <remarks>Create a new tree reference.</remarks>
		/// <param name="id">object name for the tree.</param>
		protected internal RevTree(AnyObjectId id) : base(id)
		{
		}

		public sealed override int Type
		{
			get
			{
				return Constants.OBJ_TREE;
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
