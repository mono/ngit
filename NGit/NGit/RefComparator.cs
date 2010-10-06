using System.Collections.Generic;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>Util for sorting (or comparing) Ref instances by name.</summary>
	/// <remarks>
	/// Util for sorting (or comparing) Ref instances by name.
	/// <p>
	/// Useful for command line tools or writing out refs to file.
	/// </remarks>
	public class RefComparator : IComparer<Ref>
	{
		/// <summary>Singleton instance of RefComparator</summary>
		public static readonly RefComparator INSTANCE = new RefComparator();

		public virtual int Compare(Ref o1, Ref o2)
		{
			return CompareTo(o1, o2);
		}

		/// <summary>Sorts the collection of refs, returning a new collection.</summary>
		/// <remarks>Sorts the collection of refs, returning a new collection.</remarks>
		/// <param name="refs">collection to be sorted</param>
		/// <returns>sorted collection of refs</returns>
		public static ICollection<Ref> Sort(ICollection<Ref> refs)
		{
			IList<Ref> r = new AList<Ref>(refs);
			r.Sort(INSTANCE);
			return r;
		}

		/// <summary>Compare a reference to a name.</summary>
		/// <remarks>Compare a reference to a name.</remarks>
		/// <param name="o1">the reference instance.</param>
		/// <param name="o2">the name to compare to.</param>
		/// <returns>standard Comparator result of &lt; 0, 0, &gt; 0.</returns>
		public static int CompareTo(Ref o1, string o2)
		{
			return o1.GetName().CompareTo(o2);
		}

		/// <summary>Compare two references by name.</summary>
		/// <remarks>Compare two references by name.</remarks>
		/// <param name="o1">the reference instance.</param>
		/// <param name="o2">the other reference instance.</param>
		/// <returns>standard Comparator result of &lt; 0, 0, &gt; 0.</returns>
		public static int CompareTo(Ref o1, Ref o2)
		{
			return o1.GetName().CompareTo(o2.GetName());
		}
	}
}
