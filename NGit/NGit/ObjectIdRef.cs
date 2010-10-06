using System.Text;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// A
	/// <see cref="Ref">Ref</see>
	/// that points directly at an
	/// <see cref="ObjectId">ObjectId</see>
	/// .
	/// </summary>
	public abstract class ObjectIdRef : Ref
	{
		/// <summary>Any reference whose peeled value is not yet known.</summary>
		/// <remarks>Any reference whose peeled value is not yet known.</remarks>
		public class Unpeeled : ObjectIdRef
		{
			/// <summary>Create a new ref pairing.</summary>
			/// <remarks>Create a new ref pairing.</remarks>
			/// <param name="st">method used to store this ref.</param>
			/// <param name="name">name of this ref.</param>
			/// <param name="id">
			/// current value of the ref. May be null to indicate a ref
			/// that does not exist yet.
			/// </param>
			protected internal Unpeeled(RefStorage st, string name, ObjectId id) : base(st, name
				, id)
			{
			}

			public override ObjectId GetPeeledObjectId()
			{
				return null;
			}

			public override bool IsPeeled()
			{
				return false;
			}
		}

		/// <summary>An annotated tag whose peeled object has been cached.</summary>
		/// <remarks>An annotated tag whose peeled object has been cached.</remarks>
		public class PeeledTag : ObjectIdRef
		{
			private readonly ObjectId peeledObjectId;

			/// <summary>Create a new ref pairing.</summary>
			/// <remarks>Create a new ref pairing.</remarks>
			/// <param name="st">method used to store this ref.</param>
			/// <param name="name">name of this ref.</param>
			/// <param name="id">current value of the ref.</param>
			/// <param name="p">
			/// the first non-tag object that tag
			/// <code>id</code>
			/// points to.
			/// </param>
			public PeeledTag(RefStorage st, string name, ObjectId id, ObjectId p) : base(st, 
				name, id)
			{
				peeledObjectId = p;
			}

			public override ObjectId GetPeeledObjectId()
			{
				return peeledObjectId;
			}

			public override bool IsPeeled()
			{
				return true;
			}
		}

		/// <summary>A reference to a non-tag object coming from a cached source.</summary>
		/// <remarks>A reference to a non-tag object coming from a cached source.</remarks>
		public class PeeledNonTag : ObjectIdRef
		{
			/// <summary>Create a new ref pairing.</summary>
			/// <remarks>Create a new ref pairing.</remarks>
			/// <param name="st">method used to store this ref.</param>
			/// <param name="name">name of this ref.</param>
			/// <param name="id">
			/// current value of the ref. May be null to indicate a ref
			/// that does not exist yet.
			/// </param>
			protected internal PeeledNonTag(RefStorage st, string name, ObjectId id) : base(st
				, name, id)
			{
			}

			public override ObjectId GetPeeledObjectId()
			{
				return null;
			}

			public override bool IsPeeled()
			{
				return true;
			}
		}

		private readonly string name;

		private readonly RefStorage storage;

		private readonly ObjectId objectId;

		/// <summary>Create a new ref pairing.</summary>
		/// <remarks>Create a new ref pairing.</remarks>
		/// <param name="st">method used to store this ref.</param>
		/// <param name="name">name of this ref.</param>
		/// <param name="id">
		/// current value of the ref. May be null to indicate a ref that
		/// does not exist yet.
		/// </param>
		protected internal ObjectIdRef(RefStorage st, string name, ObjectId id)
		{
			this.name = name;
			this.storage = st;
			this.objectId = id;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual bool IsSymbolic()
		{
			return false;
		}

		public virtual Ref GetLeaf()
		{
			return this;
		}

		public virtual Ref GetTarget()
		{
			return this;
		}

		public virtual ObjectId GetObjectId()
		{
			return objectId;
		}

		public virtual RefStorage GetStorage()
		{
			return storage;
		}

		public override string ToString()
		{
			StringBuilder r = new StringBuilder();
			r.Append("Ref[");
			r.Append(GetName());
			r.Append('=');
			r.Append(ObjectId.ToString(GetObjectId()));
			r.Append(']');
			return r.ToString();
		}

		public abstract ObjectId GetPeeledObjectId();

		public abstract bool IsPeeled();
	}
}
