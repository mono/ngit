using System;
using System.Collections.Generic;
using System.Text;
using NGit;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class RefMapTest : TestCase
	{
		private static readonly ObjectId ID_ONE = ObjectId.FromString("41eb0d88f833b558bddeb269b7ab77399cdf98ed"
			);

		private static readonly ObjectId ID_TWO = ObjectId.FromString("698dd0b8d0c299f080559a1cffc7fe029479a408"
			);

		private RefList<Ref> packed;

		private RefList<Ref> loose;

		private RefList<Ref> resolved;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			packed = RefList.EmptyList();
			loose = RefList.EmptyList();
			resolved = RefList.EmptyList();
		}

		public virtual void TestEmpty_NoPrefix1()
		{
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			NUnit.Framework.Assert.IsTrue(map.IsEmpty());
			// before size was computed
			NUnit.Framework.Assert.AreEqual(0, map.Count);
			NUnit.Framework.Assert.IsTrue(map.IsEmpty());
			// after size was computed
			NUnit.Framework.Assert.IsFalse(map.EntrySet().Iterator().HasNext());
			NUnit.Framework.Assert.IsFalse(map.Keys.Iterator().HasNext());
			NUnit.Framework.Assert.IsFalse(map.ContainsKey("a"));
			NUnit.Framework.Assert.IsNull(map.Get("a"));
		}

		public virtual void TestEmpty_NoPrefix2()
		{
			RefMap map = new RefMap();
			NUnit.Framework.Assert.IsTrue(map.IsEmpty());
			// before size was computed
			NUnit.Framework.Assert.AreEqual(0, map.Count);
			NUnit.Framework.Assert.IsTrue(map.IsEmpty());
			// after size was computed
			NUnit.Framework.Assert.IsFalse(map.EntrySet().Iterator().HasNext());
			NUnit.Framework.Assert.IsFalse(map.Keys.Iterator().HasNext());
			NUnit.Framework.Assert.IsFalse(map.ContainsKey("a"));
			NUnit.Framework.Assert.IsNull(map.Get("a"));
		}

		public virtual void TestNotEmpty_NoPrefix()
		{
			Ref master = NewRef("refs/heads/master", ID_ONE);
			packed = ToList(master);
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			NUnit.Framework.Assert.IsFalse(map.IsEmpty());
			// before size was computed
			NUnit.Framework.Assert.AreEqual(1, map.Count);
			NUnit.Framework.Assert.IsFalse(map.IsEmpty());
			// after size was computed
			NUnit.Framework.Assert.AreSame(master, map.Values.Iterator().Next());
		}

		public virtual void TestEmpty_WithPrefix()
		{
			Ref master = NewRef("refs/heads/master", ID_ONE);
			packed = ToList(master);
			RefMap map = new RefMap("refs/tags/", packed, loose, resolved);
			NUnit.Framework.Assert.IsTrue(map.IsEmpty());
			// before size was computed
			NUnit.Framework.Assert.AreEqual(0, map.Count);
			NUnit.Framework.Assert.IsTrue(map.IsEmpty());
			// after size was computed
			NUnit.Framework.Assert.IsFalse(map.EntrySet().Iterator().HasNext());
			NUnit.Framework.Assert.IsFalse(map.Keys.Iterator().HasNext());
		}

		public virtual void TestNotEmpty_WithPrefix()
		{
			Ref master = NewRef("refs/heads/master", ID_ONE);
			packed = ToList(master);
			RefMap map = new RefMap("refs/heads/", packed, loose, resolved);
			NUnit.Framework.Assert.IsFalse(map.IsEmpty());
			// before size was computed
			NUnit.Framework.Assert.AreEqual(1, map.Count);
			NUnit.Framework.Assert.IsFalse(map.IsEmpty());
			// after size was computed
			NUnit.Framework.Assert.AreSame(master, map.Values.Iterator().Next());
		}

		public virtual void TestClear()
		{
			Ref master = NewRef("refs/heads/master", ID_ONE);
			loose = ToList(master);
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			NUnit.Framework.Assert.AreSame(master, map.Get("refs/heads/master"));
			map.Clear();
			NUnit.Framework.Assert.IsNull(map.Get("refs/heads/master"));
			NUnit.Framework.Assert.IsTrue(map.IsEmpty());
			NUnit.Framework.Assert.AreEqual(0, map.Count);
		}

		public virtual void TestIterator_RefusesRemove()
		{
			Ref master = NewRef("refs/heads/master", ID_ONE);
			loose = ToList(master);
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			Iterator<Ref> itr = map.Values.Iterator();
			NUnit.Framework.Assert.IsTrue(itr.HasNext());
			NUnit.Framework.Assert.AreSame(master, itr.Next());
			try
			{
				itr.Remove();
				NUnit.Framework.Assert.Fail("iterator allowed remove");
			}
			catch (NotSupportedException)
			{
			}
		}

		// expected
		public virtual void TestIterator_FailsAtEnd()
		{
			Ref master = NewRef("refs/heads/master", ID_ONE);
			loose = ToList(master);
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			Iterator<Ref> itr = map.Values.Iterator();
			NUnit.Framework.Assert.IsTrue(itr.HasNext());
			NUnit.Framework.Assert.AreSame(master, itr.Next());
			try
			{
				itr.Next();
				NUnit.Framework.Assert.Fail("iterator allowed next");
			}
			catch (NoSuchElementException)
			{
			}
		}

		// expected
		public virtual void TestIterator_MissingUnresolvedSymbolicRefIsBug()
		{
			Ref master = NewRef("refs/heads/master", ID_ONE);
			Ref headR = NewRef("HEAD", master);
			loose = ToList(master);
			// loose should have added newRef("HEAD", "refs/heads/master")
			resolved = ToList(headR);
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			Iterator<Ref> itr = map.Values.Iterator();
			try
			{
				itr.HasNext();
				NUnit.Framework.Assert.Fail("iterator did not catch bad input");
			}
			catch (InvalidOperationException)
			{
			}
		}

		// expected
		public virtual void TestMerge_HeadMaster()
		{
			Ref master = NewRef("refs/heads/master", ID_ONE);
			Ref headU = NewRef("HEAD", "refs/heads/master");
			Ref headR = NewRef("HEAD", master);
			loose = ToList(headU, master);
			resolved = ToList(headR);
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			NUnit.Framework.Assert.AreEqual(2, map.Count);
			NUnit.Framework.Assert.IsFalse(map.IsEmpty());
			NUnit.Framework.Assert.IsTrue(map.ContainsKey("refs/heads/master"));
			NUnit.Framework.Assert.AreSame(master, map.Get("refs/heads/master"));
			// resolved overrides loose given same name
			NUnit.Framework.Assert.AreSame(headR, map.Get("HEAD"));
			Iterator<Ref> itr = map.Values.Iterator();
			NUnit.Framework.Assert.IsTrue(itr.HasNext());
			NUnit.Framework.Assert.AreSame(headR, itr.Next());
			NUnit.Framework.Assert.IsTrue(itr.HasNext());
			NUnit.Framework.Assert.AreSame(master, itr.Next());
			NUnit.Framework.Assert.IsFalse(itr.HasNext());
		}

		public virtual void TestMerge_PackedLooseLoose()
		{
			Ref refA = NewRef("A", ID_ONE);
			Ref refB_ONE = NewRef("B", ID_ONE);
			Ref refB_TWO = NewRef("B", ID_TWO);
			Ref refc = NewRef("c", ID_ONE);
			packed = ToList(refA, refB_ONE);
			loose = ToList(refB_TWO, refc);
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			NUnit.Framework.Assert.AreEqual(3, map.Count);
			NUnit.Framework.Assert.IsFalse(map.IsEmpty());
			NUnit.Framework.Assert.IsTrue(map.ContainsKey(refA.GetName()));
			NUnit.Framework.Assert.AreSame(refA, map.Get(refA.GetName()));
			// loose overrides packed given same name
			NUnit.Framework.Assert.AreSame(refB_TWO, map.Get(refB_ONE.GetName()));
			Iterator<Ref> itr = map.Values.Iterator();
			NUnit.Framework.Assert.IsTrue(itr.HasNext());
			NUnit.Framework.Assert.AreSame(refA, itr.Next());
			NUnit.Framework.Assert.IsTrue(itr.HasNext());
			NUnit.Framework.Assert.AreSame(refB_TWO, itr.Next());
			NUnit.Framework.Assert.IsTrue(itr.HasNext());
			NUnit.Framework.Assert.AreSame(refc, itr.Next());
			NUnit.Framework.Assert.IsFalse(itr.HasNext());
		}

		public virtual void TestMerge_WithPrefix()
		{
			Ref a = NewRef("refs/heads/A", ID_ONE);
			Ref b = NewRef("refs/heads/foo/bar/B", ID_TWO);
			Ref c = NewRef("refs/heads/foo/rab/C", ID_TWO);
			Ref g = NewRef("refs/heads/g", ID_ONE);
			packed = ToList(a, b, c, g);
			RefMap map = new RefMap("refs/heads/foo/", packed, loose, resolved);
			NUnit.Framework.Assert.AreEqual(2, map.Count);
			NUnit.Framework.Assert.AreSame(b, map.Get("bar/B"));
			NUnit.Framework.Assert.AreSame(c, map.Get("rab/C"));
			NUnit.Framework.Assert.IsNull(map.Get("refs/heads/foo/bar/B"));
			NUnit.Framework.Assert.IsNull(map.Get("refs/heads/A"));
			NUnit.Framework.Assert.IsTrue(map.ContainsKey("bar/B"));
			NUnit.Framework.Assert.IsTrue(map.ContainsKey("rab/C"));
			NUnit.Framework.Assert.IsFalse(map.ContainsKey("refs/heads/foo/bar/B"));
			NUnit.Framework.Assert.IsFalse(map.ContainsKey("refs/heads/A"));
			Iterator<KeyValuePair<string, Ref>> itr = map.EntrySet().Iterator();
			KeyValuePair<string, Ref> ent;
			NUnit.Framework.Assert.IsTrue(itr.HasNext());
			ent = itr.Next();
			NUnit.Framework.Assert.AreEqual("bar/B", ent.Key);
			NUnit.Framework.Assert.AreSame(b, ent.Value);
			NUnit.Framework.Assert.IsTrue(itr.HasNext());
			ent = itr.Next();
			NUnit.Framework.Assert.AreEqual("rab/C", ent.Key);
			NUnit.Framework.Assert.AreSame(c, ent.Value);
			NUnit.Framework.Assert.IsFalse(itr.HasNext());
		}

		public virtual void TestPut_KeyMustMatchName_NoPrefix()
		{
			Ref refA = NewRef("refs/heads/A", ID_ONE);
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			try
			{
				map.Put("FOO", refA);
				NUnit.Framework.Assert.Fail("map accepted invalid key/value pair");
			}
			catch (ArgumentException)
			{
			}
		}

		// expected
		public virtual void TestPut_KeyMustMatchName_WithPrefix()
		{
			Ref refA = NewRef("refs/heads/A", ID_ONE);
			RefMap map = new RefMap("refs/heads/", packed, loose, resolved);
			try
			{
				map.Put("FOO", refA);
				NUnit.Framework.Assert.Fail("map accepted invalid key/value pair");
			}
			catch (ArgumentException)
			{
			}
		}

		// expected
		public virtual void TestPut_NoPrefix()
		{
			Ref refA_one = NewRef("refs/heads/A", ID_ONE);
			Ref refA_two = NewRef("refs/heads/A", ID_TWO);
			packed = ToList(refA_one);
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			NUnit.Framework.Assert.AreSame(refA_one, map.Get(refA_one.GetName()));
			NUnit.Framework.Assert.AreSame(refA_one, map.Put(refA_one.GetName(), refA_two));
			// map changed, but packed, loose did not
			NUnit.Framework.Assert.AreSame(refA_two, map.Get(refA_one.GetName()));
			NUnit.Framework.Assert.AreSame(refA_one, packed.Get(0));
			NUnit.Framework.Assert.AreEqual(0, loose.Size());
			NUnit.Framework.Assert.AreSame(refA_two, map.Put(refA_one.GetName(), refA_one));
			NUnit.Framework.Assert.AreSame(refA_one, map.Get(refA_one.GetName()));
		}

		public virtual void TestPut_WithPrefix()
		{
			Ref refA_one = NewRef("refs/heads/A", ID_ONE);
			Ref refA_two = NewRef("refs/heads/A", ID_TWO);
			packed = ToList(refA_one);
			RefMap map = new RefMap("refs/heads/", packed, loose, resolved);
			NUnit.Framework.Assert.AreSame(refA_one, map.Get("A"));
			NUnit.Framework.Assert.AreSame(refA_one, map.Put("A", refA_two));
			// map changed, but packed, loose did not
			NUnit.Framework.Assert.AreSame(refA_two, map.Get("A"));
			NUnit.Framework.Assert.AreSame(refA_one, packed.Get(0));
			NUnit.Framework.Assert.AreEqual(0, loose.Size());
			NUnit.Framework.Assert.AreSame(refA_two, map.Put("A", refA_one));
			NUnit.Framework.Assert.AreSame(refA_one, map.Get("A"));
		}

		public virtual void TestPut_CollapseResolved()
		{
			Ref master = NewRef("refs/heads/master", ID_ONE);
			Ref headU = NewRef("HEAD", "refs/heads/master");
			Ref headR = NewRef("HEAD", master);
			Ref a = NewRef("refs/heads/A", ID_ONE);
			loose = ToList(headU, master);
			resolved = ToList(headR);
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			NUnit.Framework.Assert.IsNull(map.Put(a.GetName(), a));
			NUnit.Framework.Assert.AreSame(a, map.Get(a.GetName()));
			NUnit.Framework.Assert.AreSame(headR, map.Get("HEAD"));
		}

		public virtual void TestRemove()
		{
			Ref master = NewRef("refs/heads/master", ID_ONE);
			Ref headU = NewRef("HEAD", "refs/heads/master");
			Ref headR = NewRef("HEAD", master);
			packed = ToList(master);
			loose = ToList(headU, master);
			resolved = ToList(headR);
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			NUnit.Framework.Assert.IsNull(Sharpen.Collections.Remove(map, "not.a.reference"));
			NUnit.Framework.Assert.AreSame(master, Sharpen.Collections.Remove(map, "refs/heads/master"
				));
			NUnit.Framework.Assert.IsNull(map.Get("refs/heads/master"));
			NUnit.Framework.Assert.AreSame(headR, Sharpen.Collections.Remove(map, "HEAD"));
			NUnit.Framework.Assert.IsNull(map.Get("HEAD"));
			NUnit.Framework.Assert.IsTrue(map.IsEmpty());
		}

		public virtual void TestToString_NoPrefix()
		{
			Ref a = NewRef("refs/heads/A", ID_ONE);
			Ref b = NewRef("refs/heads/B", ID_TWO);
			packed = ToList(a, b);
			StringBuilder exp = new StringBuilder();
			exp.Append("[");
			exp.Append(a.ToString());
			exp.Append(", ");
			exp.Append(b.ToString());
			exp.Append("]");
			RefMap map = new RefMap(string.Empty, packed, loose, resolved);
			NUnit.Framework.Assert.AreEqual(exp.ToString(), map.ToString());
		}

		public virtual void TestToString_WithPrefix()
		{
			Ref a = NewRef("refs/heads/A", ID_ONE);
			Ref b = NewRef("refs/heads/foo/B", ID_TWO);
			Ref c = NewRef("refs/heads/foo/C", ID_TWO);
			Ref g = NewRef("refs/heads/g", ID_ONE);
			packed = ToList(a, b, c, g);
			StringBuilder exp = new StringBuilder();
			exp.Append("[");
			exp.Append(b.ToString());
			exp.Append(", ");
			exp.Append(c.ToString());
			exp.Append("]");
			RefMap map = new RefMap("refs/heads/foo/", packed, loose, resolved);
			NUnit.Framework.Assert.AreEqual(exp.ToString(), map.ToString());
		}

		public virtual void TestEntryType()
		{
			Ref a = NewRef("refs/heads/A", ID_ONE);
			Ref b = NewRef("refs/heads/B", ID_TWO);
			packed = ToList(a, b);
			RefMap map = new RefMap("refs/heads/", packed, loose, resolved);
			Iterator<KeyValuePair<string, Ref>> itr = map.EntrySet().Iterator();
			KeyValuePair<string, Ref> ent_a = itr.Next();
			KeyValuePair<string, Ref> ent_b = itr.Next();
			NUnit.Framework.Assert.AreEqual(ent_a.GetHashCode(), "A".GetHashCode());
			NUnit.Framework.Assert.IsTrue(ent_a.Equals(ent_a));
			NUnit.Framework.Assert.IsFalse(ent_a.Equals(ent_b));
			NUnit.Framework.Assert.AreEqual(a.ToString(), ent_a.ToString());
		}

		public virtual void TestEntryTypeSet()
		{
			Ref refA_one = NewRef("refs/heads/A", ID_ONE);
			Ref refA_two = NewRef("refs/heads/A", ID_TWO);
			packed = ToList(refA_one);
			RefMap map = new RefMap("refs/heads/", packed, loose, resolved);
			NUnit.Framework.Assert.AreSame(refA_one, map.Get("A"));
			KeyValuePair<string, Ref> ent = map.EntrySet().Iterator().Next();
			NUnit.Framework.Assert.AreEqual("A", ent.Key);
			NUnit.Framework.Assert.AreSame(refA_one, ent.Value);
			NUnit.Framework.Assert.AreSame(refA_one, ent.SetValue(refA_two));
			NUnit.Framework.Assert.AreSame(refA_two, ent.Value);
			NUnit.Framework.Assert.AreSame(refA_two, map.Get("A"));
			NUnit.Framework.Assert.AreEqual(1, map.Count);
		}

		private RefList<Ref> ToList(params Ref[] refs)
		{
			RefListBuilder<Ref> b = new RefListBuilder<Ref>(refs.Length);
			b.AddAll(refs, 0, refs.Length);
			return b.ToRefList();
		}

		private static Ref NewRef(string name, string dst)
		{
			return NewRef(name, new ObjectIdRef.Unpeeled(RefStorage.NEW, dst, null));
		}

		private static Ref NewRef(string name, Ref dst)
		{
			return new SymbolicRef(name, dst);
		}

		private static Ref NewRef(string name, ObjectId id)
		{
			return new ObjectIdRef.Unpeeled(RefStorage.LOOSE, name, id);
		}
	}
}
