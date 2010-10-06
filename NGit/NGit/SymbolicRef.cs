using System.Text;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// A reference that indirectly points at another
	/// <see cref="Ref">Ref</see>
	/// .
	/// <p>
	/// A symbolic reference always derives its current value from the target
	/// reference.
	/// </summary>
	public class SymbolicRef : Ref
	{
		private readonly string name;

		private readonly Ref target;

		/// <summary>Create a new ref pairing.</summary>
		/// <remarks>Create a new ref pairing.</remarks>
		/// <param name="refName">name of this ref.</param>
		/// <param name="target">the ref we reference and derive our value from.</param>
		public SymbolicRef(string refName, Ref target)
		{
			this.name = refName;
			this.target = target;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual bool IsSymbolic()
		{
			return true;
		}

		public virtual Ref GetLeaf()
		{
			Ref dst = GetTarget();
			while (dst.IsSymbolic())
			{
				dst = dst.GetTarget();
			}
			return dst;
		}

		public virtual Ref GetTarget()
		{
			return target;
		}

		public virtual ObjectId GetObjectId()
		{
			return GetLeaf().GetObjectId();
		}

		public virtual RefStorage GetStorage()
		{
			return RefStorage.LOOSE;
		}

		public virtual ObjectId GetPeeledObjectId()
		{
			return GetLeaf().GetPeeledObjectId();
		}

		public virtual bool IsPeeled()
		{
			return GetLeaf().IsPeeled();
		}

		public override string ToString()
		{
			StringBuilder r = new StringBuilder();
			r.Append("SymbolicRef[");
			Ref cur = this;
			while (cur.IsSymbolic())
			{
				r.Append(cur.GetName());
				r.Append(" -> ");
				cur = cur.GetTarget();
			}
			r.Append(cur.GetName());
			r.Append('=');
			r.Append(ObjectId.ToString(cur.GetObjectId()));
			r.Append("]");
			return r.ToString();
		}
	}
}
