using System;
using NGit;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>
	/// An ordered list of
	/// <see cref="RevObject">RevObject</see>
	/// subclasses.
	/// </summary>
	/// <?></?>
	public class RevObjectList<E> : AbstractList<E> where E:RevObject
	{
		internal const int BLOCK_SHIFT = 8;

		internal const int BLOCK_SIZE = 1 << BLOCK_SHIFT;

		/// <summary>Items stored in this list.</summary>
		/// <remarks>
		/// Items stored in this list.
		/// <p>
		/// If
		/// <see cref="RevObjectListBlock.shift">RevObjectListBlock.shift</see>
		/// = 0 this block holds the list elements; otherwise
		/// it holds pointers to other
		/// <see cref="RevObjectListBlock">RevObjectListBlock</see>
		/// instances which use a shift that
		/// is
		/// <see cref="RevObjectList{E}.BLOCK_SHIFT">RevObjectList&lt;E&gt;.BLOCK_SHIFT</see>
		/// smaller.
		/// </remarks>
		internal RevObjectListBlock contents = new RevObjectListBlock(0);

		/// <summary>Current number of elements in the list.</summary>
		/// <remarks>Current number of elements in the list.</remarks>
		protected internal int size = 0;

		/// <summary>Create an empty object list.</summary>
		/// <remarks>Create an empty object list.</remarks>
		public RevObjectList()
		{
		}

		// Initialized above.
		public override void Add(int index, E element)
		{
			if (index != size)
			{
				throw new NotSupportedException(MessageFormat.Format(JGitText.Get().unsupportedOperationNotAddAtEnd
					, index));
			}
			Set(index, element);
			size++;
		}

		public override E Set(int index, E element)
		{
			RevObjectListBlock s = contents;
			while (index >> s.shift >= BLOCK_SIZE)
			{
				s = new RevObjectListBlock(s.shift + BLOCK_SHIFT);
				s.contents[0] = contents;
				contents = s;
			}
			while (s.shift > 0)
			{
				int i = index >> s.shift;
				index -= i << s.shift;
				if (s.contents[i] == null)
				{
					s.contents[i] = new RevObjectListBlock(s.shift - BLOCK_SHIFT);
				}
				s = (RevObjectListBlock)s.contents[i];
			}
			object old = s.contents[index];
			s.contents[index] = element;
			return (E)old;
		}

		public override E Get(int index)
		{
			RevObjectListBlock s = contents;
			if (index >> s.shift >= 1024)
			{
				return null;
			}
			while (s != null && s.shift > 0)
			{
				int i = index >> s.shift;
				index -= i << s.shift;
				s = (RevObjectListBlock)s.contents[i];
			}
			return s != null ? (E)s.contents[index] : null;
		}

		public override int Count
		{
			get
			{
				return size;
			}
		}

		public override void Clear()
		{
			contents = new RevObjectListBlock(0);
			size = 0;
		}
	}

	/// <summary>One level of contents, either an intermediate level or a leaf level.</summary>
	/// <remarks>One level of contents, either an intermediate level or a leaf level.</remarks>
	internal class RevObjectListBlock
	{
		internal readonly object[] contents = new object[RevObjectList<RevObject>.BLOCK_SIZE];

		internal readonly int shift;

		internal RevObjectListBlock(int s)
		{
			shift = s;
		}
	}
}
