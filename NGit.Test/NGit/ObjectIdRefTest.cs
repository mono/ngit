using NGit;
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	public class ObjectIdRefTest : TestCase
	{
		private static readonly ObjectId ID_A = ObjectId.FromString("41eb0d88f833b558bddeb269b7ab77399cdf98ed"
			);

		private static readonly ObjectId ID_B = ObjectId.FromString("698dd0b8d0c299f080559a1cffc7fe029479a408"
			);

		private static readonly string name = "refs/heads/a.test.ref";

		public virtual void TestConstructor_PeeledStatusNotKnown()
		{
			ObjectIdRef r;
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, name, ID_A);
			NUnit.Framework.Assert.AreSame(RefStorage.LOOSE, r.GetStorage());
			NUnit.Framework.Assert.AreSame(name, r.GetName());
			NUnit.Framework.Assert.AreSame(ID_A, r.GetObjectId());
			NUnit.Framework.Assert.IsFalse("not peeled", r.IsPeeled());
			NUnit.Framework.Assert.IsNull("no peel id", r.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame("leaf is this", r, r.GetLeaf());
			NUnit.Framework.Assert.AreSame("target is this", r, r.GetTarget());
			NUnit.Framework.Assert.IsFalse("not symbolic", r.IsSymbolic());
			r = new ObjectIdRef.Unpeeled(RefStorage.PACKED, name, ID_A);
			NUnit.Framework.Assert.AreSame(RefStorage.PACKED, r.GetStorage());
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE_PACKED, name, ID_A);
			NUnit.Framework.Assert.AreSame(RefStorage.LOOSE_PACKED, r.GetStorage());
			r = new ObjectIdRef.Unpeeled(RefStorage.NEW, name, null);
			NUnit.Framework.Assert.AreSame(RefStorage.NEW, r.GetStorage());
			NUnit.Framework.Assert.AreSame(name, r.GetName());
			NUnit.Framework.Assert.IsNull("no id on new ref", r.GetObjectId());
			NUnit.Framework.Assert.IsFalse("not peeled", r.IsPeeled());
			NUnit.Framework.Assert.IsNull("no peel id", r.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame("leaf is this", r, r.GetLeaf());
			NUnit.Framework.Assert.AreSame("target is this", r, r.GetTarget());
			NUnit.Framework.Assert.IsFalse("not symbolic", r.IsSymbolic());
		}

		public virtual void TestConstructor_Peeled()
		{
			ObjectIdRef r;
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, name, ID_A);
			NUnit.Framework.Assert.AreSame(RefStorage.LOOSE, r.GetStorage());
			NUnit.Framework.Assert.AreSame(name, r.GetName());
			NUnit.Framework.Assert.AreSame(ID_A, r.GetObjectId());
			NUnit.Framework.Assert.IsFalse("not peeled", r.IsPeeled());
			NUnit.Framework.Assert.IsNull("no peel id", r.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame("leaf is this", r, r.GetLeaf());
			NUnit.Framework.Assert.AreSame("target is this", r, r.GetTarget());
			NUnit.Framework.Assert.IsFalse("not symbolic", r.IsSymbolic());
			r = new ObjectIdRef.PeeledNonTag(RefStorage.LOOSE, name, ID_A);
			NUnit.Framework.Assert.IsTrue("is peeled", r.IsPeeled());
			NUnit.Framework.Assert.IsNull("no peel id", r.GetPeeledObjectId());
			r = new ObjectIdRef.PeeledTag(RefStorage.LOOSE, name, ID_A, ID_B);
			NUnit.Framework.Assert.IsTrue("is peeled", r.IsPeeled());
			NUnit.Framework.Assert.AreSame(ID_B, r.GetPeeledObjectId());
		}

		public virtual void TestToString()
		{
			ObjectIdRef r;
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, name, ID_A);
			NUnit.Framework.Assert.AreEqual("Ref[" + name + "=" + ID_A.Name + "]", r.ToString
				());
		}
	}
}
