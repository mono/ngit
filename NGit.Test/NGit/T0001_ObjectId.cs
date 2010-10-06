using NGit;
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	public class T0001_ObjectId : TestCase
	{
		public virtual void Test001_toString()
		{
			string x = "def4c620bc3713bb1bb26b808ec9312548e73946";
			ObjectId oid = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(x, oid.Name);
		}

		public virtual void Test002_toString()
		{
			string x = "ff00eedd003713bb1bb26b808ec9312548e73946";
			ObjectId oid = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(x, oid.Name);
		}

		public virtual void Test003_equals()
		{
			string x = "def4c620bc3713bb1bb26b808ec9312548e73946";
			ObjectId a = ObjectId.FromString(x);
			ObjectId b = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
			NUnit.Framework.Assert.IsTrue("a and b are same", a.Equals(b));
		}

		public virtual void Test004_isId()
		{
			NUnit.Framework.Assert.IsTrue("valid id", ObjectId.IsId("def4c620bc3713bb1bb26b808ec9312548e73946"
				));
		}

		public virtual void Test005_notIsId()
		{
			NUnit.Framework.Assert.IsFalse("bob is not an id", ObjectId.IsId("bob"));
		}

		public virtual void Test006_notIsId()
		{
			NUnit.Framework.Assert.IsFalse("39 digits is not an id", ObjectId.IsId("def4c620bc3713bb1bb26b808ec9312548e7394"
				));
		}

		public virtual void Test007_isId()
		{
			NUnit.Framework.Assert.IsTrue("uppercase is accepted", ObjectId.IsId("Def4c620bc3713bb1bb26b808ec9312548e73946"
				));
		}

		public virtual void Test008_notIsId()
		{
			NUnit.Framework.Assert.IsFalse("g is not a valid hex digit", ObjectId.IsId("gef4c620bc3713bb1bb26b808ec9312548e73946"
				));
		}

		public virtual void Test009_toString()
		{
			string x = "ff00eedd003713bb1bb26b808ec9312548e73946";
			ObjectId oid = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(x, ObjectId.ToString(oid));
		}

		public virtual void Test010_toString()
		{
			string x = "0000000000000000000000000000000000000000";
			NUnit.Framework.Assert.AreEqual(x, ObjectId.ToString(null));
		}

		public virtual void Test011_toString()
		{
			string x = "0123456789ABCDEFabcdef1234567890abcdefAB";
			ObjectId oid = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(x.ToLower(), oid.Name);
		}
	}
}
