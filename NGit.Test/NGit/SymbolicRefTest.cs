using NGit;
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	public class SymbolicRefTest : TestCase
	{
		private static readonly ObjectId ID_A = ObjectId.FromString("41eb0d88f833b558bddeb269b7ab77399cdf98ed"
			);

		private static readonly ObjectId ID_B = ObjectId.FromString("698dd0b8d0c299f080559a1cffc7fe029479a408"
			);

		private static readonly string targetName = "refs/heads/a.test.ref";

		private static readonly string name = "refs/remotes/origin/HEAD";

		public virtual void TestConstructor()
		{
			Ref t;
			SymbolicRef r;
			t = new ObjectIdRef.Unpeeled(RefStorage.NEW, targetName, null);
			r = new SymbolicRef(name, t);
			NUnit.Framework.Assert.AreSame(RefStorage.LOOSE, r.GetStorage());
			NUnit.Framework.Assert.AreSame(name, r.GetName());
			NUnit.Framework.Assert.IsNull("no id on new ref", r.GetObjectId());
			NUnit.Framework.Assert.IsFalse("not peeled", r.IsPeeled());
			NUnit.Framework.Assert.IsNull("no peel id", r.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame("leaf is t", t, r.GetLeaf());
			NUnit.Framework.Assert.AreSame("target is t", t, r.GetTarget());
			NUnit.Framework.Assert.IsTrue("is symbolic", r.IsSymbolic());
			t = new ObjectIdRef.Unpeeled(RefStorage.PACKED, targetName, ID_A);
			r = new SymbolicRef(name, t);
			NUnit.Framework.Assert.AreSame(RefStorage.LOOSE, r.GetStorage());
			NUnit.Framework.Assert.AreSame(name, r.GetName());
			NUnit.Framework.Assert.AreSame(ID_A, r.GetObjectId());
			NUnit.Framework.Assert.IsFalse("not peeled", r.IsPeeled());
			NUnit.Framework.Assert.IsNull("no peel id", r.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame("leaf is t", t, r.GetLeaf());
			NUnit.Framework.Assert.AreSame("target is t", t, r.GetTarget());
			NUnit.Framework.Assert.IsTrue("is symbolic", r.IsSymbolic());
		}

		public virtual void TestLeaf()
		{
			Ref a;
			SymbolicRef b;
			SymbolicRef c;
			SymbolicRef d;
			a = new ObjectIdRef.PeeledTag(RefStorage.PACKED, targetName, ID_A, ID_B);
			b = new SymbolicRef("B", a);
			c = new SymbolicRef("C", b);
			d = new SymbolicRef("D", c);
			NUnit.Framework.Assert.AreSame(c, d.GetTarget());
			NUnit.Framework.Assert.AreSame(b, c.GetTarget());
			NUnit.Framework.Assert.AreSame(a, b.GetTarget());
			NUnit.Framework.Assert.AreSame(a, d.GetLeaf());
			NUnit.Framework.Assert.AreSame(a, c.GetLeaf());
			NUnit.Framework.Assert.AreSame(a, b.GetLeaf());
			NUnit.Framework.Assert.AreSame(a, a.GetLeaf());
			NUnit.Framework.Assert.AreSame(ID_A, d.GetObjectId());
			NUnit.Framework.Assert.AreSame(ID_A, c.GetObjectId());
			NUnit.Framework.Assert.AreSame(ID_A, b.GetObjectId());
			NUnit.Framework.Assert.IsTrue(d.IsPeeled());
			NUnit.Framework.Assert.IsTrue(c.IsPeeled());
			NUnit.Framework.Assert.IsTrue(b.IsPeeled());
			NUnit.Framework.Assert.AreSame(ID_B, d.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame(ID_B, c.GetPeeledObjectId());
			NUnit.Framework.Assert.AreSame(ID_B, b.GetPeeledObjectId());
		}

		public virtual void TestToString()
		{
			Ref a;
			SymbolicRef b;
			SymbolicRef c;
			SymbolicRef d;
			a = new ObjectIdRef.PeeledTag(RefStorage.PACKED, targetName, ID_A, ID_B);
			b = new SymbolicRef("B", a);
			c = new SymbolicRef("C", b);
			d = new SymbolicRef("D", c);
			NUnit.Framework.Assert.AreEqual("SymbolicRef[D -> C -> B -> " + targetName + "=" 
				+ ID_A.Name + "]", d.ToString());
		}
	}
}
