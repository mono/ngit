using System.Collections.Generic;
using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	/// <summary>
	/// Specialized list of
	/// <see cref="Edit">Edit</see>
	/// s in a document.
	/// </summary>
	public class EditList : AbstractList<Edit>
	{
		/// <summary>Construct an edit list containing a single edit.</summary>
		/// <remarks>Construct an edit list containing a single edit.</remarks>
		/// <param name="edit">the edit to return in the list.</param>
		/// <returns>
		/// list containing only
		/// <code>edit</code>
		/// .
		/// </returns>
		public static NGit.Diff.EditList Singleton(Edit edit)
		{
			NGit.Diff.EditList res = new NGit.Diff.EditList(1);
			res.AddItem(edit);
			return res;
		}

		private readonly AList<Edit> container;

		/// <summary>Create a new, empty edit list.</summary>
		/// <remarks>Create a new, empty edit list.</remarks>
		public EditList()
		{
			container = new AList<Edit>();
		}

		/// <summary>Create an empty edit list with the specified capacity.</summary>
		/// <remarks>Create an empty edit list with the specified capacity.</remarks>
		/// <param name="capacity">
		/// the initial capacity of the edit list. If additional edits are
		/// added to the list, it will be grown to support them.
		/// </param>
		public EditList(int capacity)
		{
			container = new AList<Edit>(capacity);
		}

		public override int Count
		{
			get
			{
				return container.Count;
			}
		}

		public override Edit Get(int index)
		{
			return container[index];
		}

		public override Edit Set(int index, Edit element)
		{
			return container.Set(index, element);
		}

		public override void Add(int index, Edit element)
		{
			container.Add(index, element);
		}

		public override bool AddAll<_T0>(ICollection<_T0> c)
		{
			return Sharpen.Collections.AddAll(container, c);
		}

		public override Edit Remove(int index)
		{
			return container.Remove(index);
		}

		public override int GetHashCode()
		{
			return container.GetHashCode();
		}

		public override bool Equals(object o)
		{
			if (o is NGit.Diff.EditList)
			{
				return container.Equals(((NGit.Diff.EditList)o).container);
			}
			return false;
		}

		public override string ToString()
		{
			return "EditList" + container.ToString();
		}
	}
}
