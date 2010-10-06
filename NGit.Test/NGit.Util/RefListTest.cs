/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Text;
using NGit;
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	[NUnit.Framework.TestFixture]
	public class RefListTest
	{
		private static readonly ObjectId ID = ObjectId.FromString("41eb0d88f833b558bddeb269b7ab77399cdf98ed"
			);

		private static readonly Ref REF_A = NewRef("A");

		private static readonly Ref REF_B = NewRef("B");

		private static readonly Ref REF_c = NewRef("c");

		[NUnit.Framework.Test]
		public virtual void TestEmpty()
		{
			RefList<Ref> list = RefList.EmptyList();
			NUnit.Framework.Assert.AreEqual(0, list.Size());
			NUnit.Framework.Assert.IsTrue(list.IsEmpty());
			NUnit.Framework.Assert.IsFalse(list.Iterator().HasNext());
			NUnit.Framework.Assert.AreEqual(-1, list.Find("a"));
			NUnit.Framework.Assert.AreEqual(-1, list.Find("z"));
			NUnit.Framework.Assert.IsFalse(list.Contains("a"));
			NUnit.Framework.Assert.IsNull(list.Get("a"));
			try
			{
				list.Get(0);
				NUnit.Framework.Assert.Fail("RefList.emptyList should have 0 element array");
			}
			catch (IndexOutOfRangeException)
			{
			}
		}

		// expected
		[NUnit.Framework.Test]
		public virtual void TestEmptyBuilder()
		{
			RefList<Ref> list = new RefListBuilder<Ref>().ToRefList();
			NUnit.Framework.Assert.AreEqual(0, list.Size());
			NUnit.Framework.Assert.IsFalse(list.Iterator().HasNext());
			NUnit.Framework.Assert.AreEqual(-1, list.Find("a"));
			NUnit.Framework.Assert.AreEqual(-1, list.Find("z"));
			NUnit.Framework.Assert.IsFalse(list.Contains("a"));
			NUnit.Framework.Assert.IsNull(list.Get("a"));
			NUnit.Framework.Assert.IsTrue(list.AsList().IsEmpty());
			NUnit.Framework.Assert.AreEqual("[]", list.ToString());
			// default array capacity should be 16, with no bounds checking.
			NUnit.Framework.Assert.IsNull(list.Get(16 - 1));
			try
			{
				list.Get(16);
				NUnit.Framework.Assert.Fail("default RefList should have 16 element array");
			}
			catch (IndexOutOfRangeException)
			{
			}
		}

		// expected
		[NUnit.Framework.Test]
		public virtual void TestBuilder_AddThenSort()
		{
			RefListBuilder<Ref> builder = new RefListBuilder<Ref>(1);
			builder.Add(REF_B);
			builder.Add(REF_A);
			RefList<Ref> list = builder.ToRefList();
			NUnit.Framework.Assert.AreEqual(2, list.Size());
			NUnit.Framework.Assert.AreSame(REF_B, list.Get(0));
			NUnit.Framework.Assert.AreSame(REF_A, list.Get(1));
			builder.Sort();
			list = builder.ToRefList();
			NUnit.Framework.Assert.AreEqual(2, list.Size());
			NUnit.Framework.Assert.AreSame(REF_A, list.Get(0));
			NUnit.Framework.Assert.AreSame(REF_B, list.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestBuilder_AddAll()
		{
			RefListBuilder<Ref> builder = new RefListBuilder<Ref>(1);
			Ref[] src = new Ref[] { REF_A, REF_B, REF_c, REF_A };
			builder.AddAll(src, 1, 2);
			RefList<Ref> list = builder.ToRefList();
			NUnit.Framework.Assert.AreEqual(2, list.Size());
			NUnit.Framework.Assert.AreSame(REF_B, list.Get(0));
			NUnit.Framework.Assert.AreSame(REF_c, list.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestBuilder_Set()
		{
			RefListBuilder<Ref> builder = new RefListBuilder<Ref>();
			builder.Add(REF_A);
			builder.Add(REF_A);
			NUnit.Framework.Assert.AreEqual(2, builder.Size());
			NUnit.Framework.Assert.AreSame(REF_A, builder.Get(0));
			NUnit.Framework.Assert.AreSame(REF_A, builder.Get(1));
			RefList<Ref> list = builder.ToRefList();
			NUnit.Framework.Assert.AreEqual(2, list.Size());
			NUnit.Framework.Assert.AreSame(REF_A, list.Get(0));
			NUnit.Framework.Assert.AreSame(REF_A, list.Get(1));
			builder.Set(1, REF_B);
			list = builder.ToRefList();
			NUnit.Framework.Assert.AreEqual(2, list.Size());
			NUnit.Framework.Assert.AreSame(REF_A, list.Get(0));
			NUnit.Framework.Assert.AreSame(REF_B, list.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestBuilder_Remove()
		{
			RefListBuilder<Ref> builder = new RefListBuilder<Ref>();
			builder.Add(REF_A);
			builder.Add(REF_B);
			builder.Remove(0);
			NUnit.Framework.Assert.AreEqual(1, builder.Size());
			NUnit.Framework.Assert.AreSame(REF_B, builder.Get(0));
		}

		[NUnit.Framework.Test]
		public virtual void TestSet()
		{
			RefList<Ref> one = ToList(REF_A, REF_A);
			RefList<Ref> two = one.Set(1, REF_B);
			NUnit.Framework.Assert.AreNotSame(one, two);
			// one is not modified
			NUnit.Framework.Assert.AreEqual(2, one.Size());
			NUnit.Framework.Assert.AreSame(REF_A, one.Get(0));
			NUnit.Framework.Assert.AreSame(REF_A, one.Get(1));
			// but two is
			NUnit.Framework.Assert.AreEqual(2, two.Size());
			NUnit.Framework.Assert.AreSame(REF_A, one.Get(0));
			NUnit.Framework.Assert.AreSame(REF_B, two.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestAddToEmptyList()
		{
			RefList<Ref> one = ToList();
			RefList<Ref> two = one.Add(0, REF_B);
			NUnit.Framework.Assert.AreNotSame(one, two);
			// one is not modified, but two is
			NUnit.Framework.Assert.AreEqual(0, one.Size());
			NUnit.Framework.Assert.AreEqual(1, two.Size());
			NUnit.Framework.Assert.IsFalse(two.IsEmpty());
			NUnit.Framework.Assert.AreSame(REF_B, two.Get(0));
		}

		[NUnit.Framework.Test]
		public virtual void TestAddToFrontOfList()
		{
			RefList<Ref> one = ToList(REF_A);
			RefList<Ref> two = one.Add(0, REF_B);
			NUnit.Framework.Assert.AreNotSame(one, two);
			// one is not modified, but two is
			NUnit.Framework.Assert.AreEqual(1, one.Size());
			NUnit.Framework.Assert.AreSame(REF_A, one.Get(0));
			NUnit.Framework.Assert.AreEqual(2, two.Size());
			NUnit.Framework.Assert.AreSame(REF_B, two.Get(0));
			NUnit.Framework.Assert.AreSame(REF_A, two.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestAddToEndOfList()
		{
			RefList<Ref> one = ToList(REF_A);
			RefList<Ref> two = one.Add(1, REF_B);
			NUnit.Framework.Assert.AreNotSame(one, two);
			// one is not modified, but two is
			NUnit.Framework.Assert.AreEqual(1, one.Size());
			NUnit.Framework.Assert.AreSame(REF_A, one.Get(0));
			NUnit.Framework.Assert.AreEqual(2, two.Size());
			NUnit.Framework.Assert.AreSame(REF_A, two.Get(0));
			NUnit.Framework.Assert.AreSame(REF_B, two.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestAddToMiddleOfListByInsertionPosition()
		{
			RefList<Ref> one = ToList(REF_A, REF_c);
			NUnit.Framework.Assert.AreEqual(-2, one.Find(REF_B.GetName()));
			RefList<Ref> two = one.Add(one.Find(REF_B.GetName()), REF_B);
			NUnit.Framework.Assert.AreNotSame(one, two);
			// one is not modified, but two is
			NUnit.Framework.Assert.AreEqual(2, one.Size());
			NUnit.Framework.Assert.AreSame(REF_A, one.Get(0));
			NUnit.Framework.Assert.AreSame(REF_c, one.Get(1));
			NUnit.Framework.Assert.AreEqual(3, two.Size());
			NUnit.Framework.Assert.AreSame(REF_A, two.Get(0));
			NUnit.Framework.Assert.AreSame(REF_B, two.Get(1));
			NUnit.Framework.Assert.AreSame(REF_c, two.Get(2));
		}

		[NUnit.Framework.Test]
		public virtual void TestPutNewEntry()
		{
			RefList<Ref> one = ToList(REF_A, REF_c);
			RefList<Ref> two = one.Put(REF_B);
			NUnit.Framework.Assert.AreNotSame(one, two);
			// one is not modified, but two is
			NUnit.Framework.Assert.AreEqual(2, one.Size());
			NUnit.Framework.Assert.AreSame(REF_A, one.Get(0));
			NUnit.Framework.Assert.AreSame(REF_c, one.Get(1));
			NUnit.Framework.Assert.AreEqual(3, two.Size());
			NUnit.Framework.Assert.AreSame(REF_A, two.Get(0));
			NUnit.Framework.Assert.AreSame(REF_B, two.Get(1));
			NUnit.Framework.Assert.AreSame(REF_c, two.Get(2));
		}

		[NUnit.Framework.Test]
		public virtual void TestPutReplaceEntry()
		{
			Ref otherc = NewRef(REF_c.GetName());
			NUnit.Framework.Assert.AreNotSame(REF_c, otherc);
			RefList<Ref> one = ToList(REF_A, REF_c);
			RefList<Ref> two = one.Put(otherc);
			NUnit.Framework.Assert.AreNotSame(one, two);
			// one is not modified, but two is
			NUnit.Framework.Assert.AreEqual(2, one.Size());
			NUnit.Framework.Assert.AreSame(REF_A, one.Get(0));
			NUnit.Framework.Assert.AreSame(REF_c, one.Get(1));
			NUnit.Framework.Assert.AreEqual(2, two.Size());
			NUnit.Framework.Assert.AreSame(REF_A, two.Get(0));
			NUnit.Framework.Assert.AreSame(otherc, two.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestRemoveFrontOfList()
		{
			RefList<Ref> one = ToList(REF_A, REF_B, REF_c);
			RefList<Ref> two = one.Remove(0);
			NUnit.Framework.Assert.AreNotSame(one, two);
			NUnit.Framework.Assert.AreEqual(3, one.Size());
			NUnit.Framework.Assert.AreSame(REF_A, one.Get(0));
			NUnit.Framework.Assert.AreSame(REF_B, one.Get(1));
			NUnit.Framework.Assert.AreSame(REF_c, one.Get(2));
			NUnit.Framework.Assert.AreEqual(2, two.Size());
			NUnit.Framework.Assert.AreSame(REF_B, two.Get(0));
			NUnit.Framework.Assert.AreSame(REF_c, two.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestRemoveMiddleOfList()
		{
			RefList<Ref> one = ToList(REF_A, REF_B, REF_c);
			RefList<Ref> two = one.Remove(1);
			NUnit.Framework.Assert.AreNotSame(one, two);
			NUnit.Framework.Assert.AreEqual(3, one.Size());
			NUnit.Framework.Assert.AreSame(REF_A, one.Get(0));
			NUnit.Framework.Assert.AreSame(REF_B, one.Get(1));
			NUnit.Framework.Assert.AreSame(REF_c, one.Get(2));
			NUnit.Framework.Assert.AreEqual(2, two.Size());
			NUnit.Framework.Assert.AreSame(REF_A, two.Get(0));
			NUnit.Framework.Assert.AreSame(REF_c, two.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestRemoveEndOfList()
		{
			RefList<Ref> one = ToList(REF_A, REF_B, REF_c);
			RefList<Ref> two = one.Remove(2);
			NUnit.Framework.Assert.AreNotSame(one, two);
			NUnit.Framework.Assert.AreEqual(3, one.Size());
			NUnit.Framework.Assert.AreSame(REF_A, one.Get(0));
			NUnit.Framework.Assert.AreSame(REF_B, one.Get(1));
			NUnit.Framework.Assert.AreSame(REF_c, one.Get(2));
			NUnit.Framework.Assert.AreEqual(2, two.Size());
			NUnit.Framework.Assert.AreSame(REF_A, two.Get(0));
			NUnit.Framework.Assert.AreSame(REF_B, two.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestRemoveMakesEmpty()
		{
			RefList<Ref> one = ToList(REF_A);
			RefList<Ref> two = one.Remove(1);
			NUnit.Framework.Assert.AreNotSame(one, two);
			NUnit.Framework.Assert.AreSame(two, RefList.EmptyList<Ref>());
		}

		[NUnit.Framework.Test]
		public virtual void TestToString()
		{
			StringBuilder exp = new StringBuilder();
			exp.Append("[");
			exp.Append(REF_A);
			exp.Append(", ");
			exp.Append(REF_B);
			exp.Append("]");
			RefList<Ref> list = ToList(REF_A, REF_B);
			NUnit.Framework.Assert.AreEqual(exp.ToString(), list.ToString());
		}

		[NUnit.Framework.Test]
		public virtual void TestBuilder_ToString()
		{
			StringBuilder exp = new StringBuilder();
			exp.Append("[");
			exp.Append(REF_A);
			exp.Append(", ");
			exp.Append(REF_B);
			exp.Append("]");
			RefListBuilder<Ref> list = new RefListBuilder<Ref>();
			list.Add(REF_A);
			list.Add(REF_B);
			NUnit.Framework.Assert.AreEqual(exp.ToString(), list.ToString());
		}

		[NUnit.Framework.Test]
		public virtual void TestFindContainsGet()
		{
			RefList<Ref> list = ToList(REF_A, REF_B, REF_c);
			NUnit.Framework.Assert.AreEqual(0, list.Find("A"));
			NUnit.Framework.Assert.AreEqual(1, list.Find("B"));
			NUnit.Framework.Assert.AreEqual(2, list.Find("c"));
			NUnit.Framework.Assert.AreEqual(-1, list.Find("0"));
			NUnit.Framework.Assert.AreEqual(-2, list.Find("AB"));
			NUnit.Framework.Assert.AreEqual(-3, list.Find("a"));
			NUnit.Framework.Assert.AreEqual(-4, list.Find("z"));
			NUnit.Framework.Assert.AreSame(REF_A, list.Get("A"));
			NUnit.Framework.Assert.AreSame(REF_B, list.Get("B"));
			NUnit.Framework.Assert.AreSame(REF_c, list.Get("c"));
			NUnit.Framework.Assert.IsNull(list.Get("AB"));
			NUnit.Framework.Assert.IsNull(list.Get("z"));
			NUnit.Framework.Assert.IsTrue(list.Contains("A"));
			NUnit.Framework.Assert.IsTrue(list.Contains("B"));
			NUnit.Framework.Assert.IsTrue(list.Contains("c"));
			NUnit.Framework.Assert.IsFalse(list.Contains("AB"));
			NUnit.Framework.Assert.IsFalse(list.Contains("z"));
		}

		[NUnit.Framework.Test]
		public virtual void TestIterable()
		{
			RefList<Ref> list = ToList(REF_A, REF_B, REF_c);
			int idx = 0;
			foreach (Ref @ref in list)
			{
				NUnit.Framework.Assert.AreSame(list.Get(idx++), @ref);
			}
			NUnit.Framework.Assert.AreEqual(3, idx);
			Iterator<Ref> i = RefList.EmptyList().Iterator();
			try
			{
				i.Next();
				NUnit.Framework.Assert.Fail("did not throw NoSuchElementException");
			}
			catch (NoSuchElementException)
			{
			}
			// expected
			i = list.Iterator();
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreSame(REF_A, i.Next());
			try
			{
				i.Remove();
				NUnit.Framework.Assert.Fail("did not throw UnsupportedOperationException");
			}
			catch (NotSupportedException)
			{
			}
		}

		// expected
		[NUnit.Framework.Test]
		public virtual void TestCopyLeadingPrefix()
		{
			RefList<Ref> one = ToList(REF_A, REF_B, REF_c);
			RefList<Ref> two = one.Copy(2).ToRefList();
			NUnit.Framework.Assert.AreNotSame(one, two);
			NUnit.Framework.Assert.AreEqual(3, one.Size());
			NUnit.Framework.Assert.AreSame(REF_A, one.Get(0));
			NUnit.Framework.Assert.AreSame(REF_B, one.Get(1));
			NUnit.Framework.Assert.AreSame(REF_c, one.Get(2));
			NUnit.Framework.Assert.AreEqual(2, two.Size());
			NUnit.Framework.Assert.AreSame(REF_A, two.Get(0));
			NUnit.Framework.Assert.AreSame(REF_B, two.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestCopyConstructorReusesArray()
		{
			RefListBuilder<Ref> one = new RefListBuilder<Ref>();
			one.Add(REF_A);
			RefList<Ref> two = new RefList<Ref>(one.ToRefList());
			one.Set(0, REF_B);
			NUnit.Framework.Assert.AreSame(REF_B, two.Get(0));
		}

		private RefList<Ref> ToList(params Ref[] refs)
		{
			RefListBuilder<Ref> b = new RefListBuilder<Ref>(refs.Length);
			b.AddAll(refs, 0, refs.Length);
			return b.ToRefList();
		}

		private static Ref NewRef(string name)
		{
			return new ObjectIdRef.Unpeeled(RefStorage.LOOSE, name, ID);
		}
	}
}
