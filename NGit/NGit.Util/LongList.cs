using System.Text;
using Sharpen;

namespace NGit.Util
{
	/// <summary>A more efficient List<Long> using a primitive long array.</summary>
	/// <remarks>A more efficient List<Long> using a primitive long array.</remarks>
	public class LongList
	{
		private long[] entries;

		private int count;

		/// <summary>Create an empty list with a default capacity.</summary>
		/// <remarks>Create an empty list with a default capacity.</remarks>
		public LongList() : this(10)
		{
		}

		/// <summary>Create an empty list with the specified capacity.</summary>
		/// <remarks>Create an empty list with the specified capacity.</remarks>
		/// <param name="capacity">number of entries the list can initially hold.</param>
		public LongList(int capacity)
		{
			entries = new long[capacity];
		}

		/// <returns>number of entries in this list</returns>
		public virtual int Size()
		{
			return count;
		}

		/// <param name="i">
		/// index to read, must be in the range [0,
		/// <see cref="Size()">Size()</see>
		/// ).
		/// </param>
		/// <returns>the number at the specified index</returns>
		/// <exception cref="System.IndexOutOfRangeException">the index outside the valid range
		/// 	</exception>
		public virtual long Get(int i)
		{
			if (count <= i)
			{
				throw Sharpen.Extensions.CreateIndexOutOfRangeException(i);
			}
			return entries[i];
		}

		/// <summary>Determine if an entry appears in this collection.</summary>
		/// <remarks>Determine if an entry appears in this collection.</remarks>
		/// <param name="value">the value to search for.</param>
		/// <returns>
		/// true of
		/// <code>value</code>
		/// appears in this list.
		/// </returns>
		public virtual bool Contains(long value)
		{
			for (int i = 0; i < count; i++)
			{
				if (entries[i] == value)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Empty this list</summary>
		public virtual void Clear()
		{
			count = 0;
		}

		/// <summary>Add an entry to the end of the list.</summary>
		/// <remarks>Add an entry to the end of the list.</remarks>
		/// <param name="n">the number to add.</param>
		public virtual void Add(long n)
		{
			if (count == entries.Length)
			{
				Grow();
			}
			entries[count++] = n;
		}

		/// <summary>Assign an entry in the list.</summary>
		/// <remarks>Assign an entry in the list.</remarks>
		/// <param name="index">
		/// index to set, must be in the range [0,
		/// <see cref="Size()">Size()</see>
		/// ).
		/// </param>
		/// <param name="n">value to store at the position.</param>
		public virtual void Set(int index, long n)
		{
			if (count < index)
			{
				throw Sharpen.Extensions.CreateIndexOutOfRangeException(index);
			}
			else
			{
				if (count == index)
				{
					Add(n);
				}
				else
				{
					entries[index] = n;
				}
			}
		}

		/// <summary>Pad the list with entries.</summary>
		/// <remarks>Pad the list with entries.</remarks>
		/// <param name="toIndex">
		/// index position to stop filling at. 0 inserts no filler. 1
		/// ensures the list has a size of 1, adding <code>val</code> if
		/// the list is currently empty.
		/// </param>
		/// <param name="val">value to insert into padded positions.</param>
		public virtual void FillTo(int toIndex, long val)
		{
			while (count < toIndex)
			{
				Add(val);
			}
		}

		/// <summary>Sort the list of longs according to their natural ordering.</summary>
		/// <remarks>Sort the list of longs according to their natural ordering.</remarks>
		public virtual void Sort()
		{
			Arrays.Sort(entries, 0, count);
		}

		private void Grow()
		{
			long[] n = new long[(entries.Length + 16) * 3 / 2];
			System.Array.Copy(entries, 0, n, 0, count);
			entries = n;
		}

		public override string ToString()
		{
			StringBuilder r = new StringBuilder();
			r.Append('[');
			for (int i = 0; i < count; i++)
			{
				if (i > 0)
				{
					r.Append(", ");
				}
				r.Append(entries[i]);
			}
			r.Append(']');
			return r.ToString();
		}
	}
}
