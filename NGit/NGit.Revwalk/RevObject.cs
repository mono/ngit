using System.Text;
using NGit;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>Base object type accessed during revision walking.</summary>
	/// <remarks>Base object type accessed during revision walking.</remarks>
	[System.Serializable]
	public abstract class RevObject : ObjectId
	{
		internal const int PARSED = 1;

		internal int flags;

		protected internal RevObject(AnyObjectId name) : base(name)
		{
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		internal abstract void ParseHeaders(RevWalk walk);

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		internal abstract void ParseBody(RevWalk walk);

		/// <summary>Get Git object type.</summary>
		/// <remarks>
		/// Get Git object type. See
		/// <see cref="NGit.Constants">NGit.Constants</see>
		/// .
		/// </remarks>
		/// <returns>object type</returns>
		public abstract int Type
		{
			get;
		}

		/// <summary>Get the name of this object.</summary>
		/// <remarks>Get the name of this object.</remarks>
		/// <returns>unique hash of this object.</returns>
		public ObjectId Id
		{
			get
			{
				return this;
			}
		}

		/// <summary>Test to see if the flag has been set on this object.</summary>
		/// <remarks>Test to see if the flag has been set on this object.</remarks>
		/// <param name="flag">the flag to test.</param>
		/// <returns>true if the flag has been added to this object; false if not.</returns>
		public bool Has(RevFlag flag)
		{
			return (flags & flag.mask) != 0;
		}

		/// <summary>Test to see if any flag in the set has been set on this object.</summary>
		/// <remarks>Test to see if any flag in the set has been set on this object.</remarks>
		/// <param name="set">the flags to test.</param>
		/// <returns>
		/// true if any flag in the set has been added to this object; false
		/// if not.
		/// </returns>
		public bool HasAny(RevFlagSet set)
		{
			return (flags & set.mask) != 0;
		}

		/// <summary>Test to see if all flags in the set have been set on this object.</summary>
		/// <remarks>Test to see if all flags in the set have been set on this object.</remarks>
		/// <param name="set">the flags to test.</param>
		/// <returns>
		/// true if all flags of the set have been added to this object;
		/// false if some or none have been added.
		/// </returns>
		public bool HasAll(RevFlagSet set)
		{
			return (flags & set.mask) == set.mask;
		}

		/// <summary>Add a flag to this object.</summary>
		/// <remarks>
		/// Add a flag to this object.
		/// <p>
		/// If the flag is already set on this object then the method has no effect.
		/// </remarks>
		/// <param name="flag">the flag to mark on this object, for later testing.</param>
		public void Add(RevFlag flag)
		{
			flags |= flag.mask;
		}

		/// <summary>Add a set of flags to this object.</summary>
		/// <remarks>Add a set of flags to this object.</remarks>
		/// <param name="set">the set of flags to mark on this object, for later testing.</param>
		public void Add(RevFlagSet set)
		{
			flags |= set.mask;
		}

		/// <summary>Remove a flag from this object.</summary>
		/// <remarks>
		/// Remove a flag from this object.
		/// <p>
		/// If the flag is not set on this object then the method has no effect.
		/// </remarks>
		/// <param name="flag">the flag to remove from this object.</param>
		public void Remove(RevFlag flag)
		{
			flags &= ~flag.mask;
		}

		/// <summary>Remove a set of flags from this object.</summary>
		/// <remarks>Remove a set of flags from this object.</remarks>
		/// <param name="set">the flag to remove from this object.</param>
		public void Remove(RevFlagSet set)
		{
			flags &= ~set.mask;
		}

		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			s.Append(Constants.TypeString(Type));
			s.Append(' ');
			s.Append(Name);
			s.Append(' ');
			AppendCoreFlags(s);
			return s.ToString();
		}

		/// <param name="s">buffer to append a debug description of core RevFlags onto.</param>
		protected internal virtual void AppendCoreFlags(StringBuilder s)
		{
			s.Append((flags & RevWalk.TOPO_DELAY) != 0 ? 'o' : '-');
			s.Append((flags & RevWalk.TEMP_MARK) != 0 ? 't' : '-');
			s.Append((flags & RevWalk.REWRITE) != 0 ? 'r' : '-');
			s.Append((flags & RevWalk.UNINTERESTING) != 0 ? 'u' : '-');
			s.Append((flags & RevWalk.SEEN) != 0 ? 's' : '-');
			s.Append((flags & RevWalk.PARSED) != 0 ? 'p' : '-');
		}
	}
}
