using System.Collections.Generic;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>
	/// Multiple application level mark bits for
	/// <see cref="RevObject">RevObject</see>
	/// s.
	/// </summary>
	/// <seealso cref="RevFlag">RevFlag</seealso>
	public class RevFlagSet : AbstractSet<RevFlag>
	{
		internal int mask;

		private readonly IList<RevFlag> active;

		/// <summary>Create an empty set of flags.</summary>
		/// <remarks>Create an empty set of flags.</remarks>
		public RevFlagSet()
		{
			active = new AList<RevFlag>();
		}

		/// <summary>Create a set of flags, copied from an existing set.</summary>
		/// <remarks>Create a set of flags, copied from an existing set.</remarks>
		/// <param name="s">the set to copy flags from.</param>
		public RevFlagSet(NGit.Revwalk.RevFlagSet s)
		{
			mask = s.mask;
			active = new AList<RevFlag>(s.active);
		}

		/// <summary>Create a set of flags, copied from an existing collection.</summary>
		/// <remarks>Create a set of flags, copied from an existing collection.</remarks>
		/// <param name="s">the collection to copy flags from.</param>
		public RevFlagSet(ICollection<RevFlag> s) : this()
		{
			Sharpen.Collections.AddAll(this, s);
		}

		public override bool Contains(object o)
		{
			if (o is RevFlag)
			{
				return (mask & ((RevFlag)o).mask) != 0;
			}
			return false;
		}

		public override bool ContainsAll (ICollection<object> c)
		{
			if (c is NGit.Revwalk.RevFlagSet)
			{
				int cMask = ((NGit.Revwalk.RevFlagSet)c).mask;
				return (mask & cMask) == cMask;
			}
			return base.ContainsAll(c);
		}

		public override bool AddItem(RevFlag flag)
		{
			if ((mask & flag.mask) != 0)
			{
				return false;
			}
			mask |= flag.mask;
			int p = 0;
			while (p < active.Count && active[p].mask < flag.mask)
			{
				p++;
			}
			active.Add(p, flag);
			return true;
		}

		public override bool Remove(object o)
		{
			RevFlag flag = (RevFlag)o;
			if ((mask & flag.mask) == 0)
			{
				return false;
			}
			mask &= ~flag.mask;
			for (int i = 0; i < active.Count; i++)
			{
				if (active[i].mask == flag.mask)
				{
					active.Remove(i);
				}
			}
			return true;
		}

		public override Sharpen.Iterator<RevFlag> Iterator()
		{
			Sharpen.Iterator<RevFlag> i = active.Iterator();
			return new _Iterator_132(this, i);
		}

		private sealed class _Iterator_132 : Sharpen.Iterator<RevFlag>
		{
			public _Iterator_132(RevFlagSet _enclosing, Sharpen.Iterator<RevFlag> i)
			{
				this._enclosing = _enclosing;
				this.i = i;
			}

			private RevFlag current;

			public override bool HasNext()
			{
				return i.HasNext();
			}

			public override RevFlag Next()
			{
				return this.current = i.Next();
			}

			public override void Remove()
			{
				this._enclosing.mask &= ~this.current.mask;
				i.Remove();
			}

			private readonly RevFlagSet _enclosing;

			private readonly Sharpen.Iterator<RevFlag> i;
		}

		public override int Count
		{
			get
			{
				return active.Count;
			}
		}
	}
}
