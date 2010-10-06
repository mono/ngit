using NGit;
using NGit.Treewalk;
using Sharpen;

namespace NGit.Treewalk
{
	/// <summary>Iterator over an empty tree (a directory with no files).</summary>
	/// <remarks>Iterator over an empty tree (a directory with no files).</remarks>
	public class EmptyTreeIterator : AbstractTreeIterator
	{
		/// <summary>Create a new iterator with no parent.</summary>
		/// <remarks>Create a new iterator with no parent.</remarks>
		public EmptyTreeIterator()
		{
		}

		protected internal EmptyTreeIterator(AbstractTreeIterator p) : base(p)
		{
			// Create a root empty tree.
			pathLen = pathOffset;
		}

		/// <summary>Create an iterator for a subtree of an existing iterator.</summary>
		/// <remarks>
		/// Create an iterator for a subtree of an existing iterator.
		/// <p>
		/// The caller is responsible for setting up the path of the child iterator.
		/// </remarks>
		/// <param name="p">parent tree iterator.</param>
		/// <param name="childPath">
		/// path array to be used by the child iterator. This path must
		/// contain the path from the top of the walk to the first child
		/// and must end with a '/'.
		/// </param>
		/// <param name="childPathOffset">
		/// position within <code>childPath</code> where the child can
		/// insert its data. The value at
		/// <code>childPath[childPathOffset-1]</code> must be '/'.
		/// </param>
		protected internal EmptyTreeIterator(AbstractTreeIterator p, byte[] childPath, int
			 childPathOffset) : base(p, childPath, childPathOffset)
		{
			pathLen = childPathOffset - 1;
		}

		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override AbstractTreeIterator CreateSubtreeIterator(ObjectReader reader)
		{
			return new NGit.Treewalk.EmptyTreeIterator(this);
		}

		public override bool HasId()
		{
			return false;
		}

		public override ObjectId GetEntryObjectId()
		{
			return ObjectId.ZeroId;
		}

		public override byte[] IdBuffer()
		{
			return zeroid;
		}

		public override int IdOffset()
		{
			return 0;
		}

		public override void Reset()
		{
		}

		// Do nothing.
		public override bool First()
		{
			return true;
		}

		public override bool Eof()
		{
			return true;
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		public override void Next(int delta)
		{
		}

		// Do nothing.
		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		public override void Back(int delta)
		{
		}

		// Do nothing.
		public override void StopWalk()
		{
			if (parent != null)
			{
				parent.StopWalk();
			}
		}
	}
}
