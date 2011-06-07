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
using NGit;
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class ObjectIdOwnerMapTest
	{
		private MutableObjectId idBuf;

		private ObjectIdOwnerMapTest.SubId id_1;

		private ObjectIdOwnerMapTest.SubId id_2;

		private ObjectIdOwnerMapTest.SubId id_3;

		private ObjectIdOwnerMapTest.SubId id_a31;

		private ObjectIdOwnerMapTest.SubId id_b31;

		[SetUp]
		public virtual void Init()
		{
			idBuf = new MutableObjectId();
			id_1 = new ObjectIdOwnerMapTest.SubId(Id(1));
			id_2 = new ObjectIdOwnerMapTest.SubId(Id(2));
			id_3 = new ObjectIdOwnerMapTest.SubId(Id(3));
			id_a31 = new ObjectIdOwnerMapTest.SubId(Id(31));
			id_b31 = new ObjectIdOwnerMapTest.SubId(Id((1 << 8) + 31));
		}

		[NUnit.Framework.Test]
		public virtual void TestEmptyMap()
		{
			ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId> m = new ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId
				>();
			NUnit.Framework.Assert.IsTrue(m.IsEmpty());
			NUnit.Framework.Assert.AreEqual(0, m.Size());
			Iterator<ObjectIdOwnerMapTest.SubId> i = m.Iterator();
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.IsFalse(i.HasNext());
			NUnit.Framework.Assert.IsFalse(m.Contains(Id(1)));
		}

		[NUnit.Framework.Test]
		public virtual void TestAddGetAndContains()
		{
			ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId> m = new ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId
				>();
			m.Add(id_1);
			m.Add(id_2);
			m.Add(id_3);
			m.Add(id_a31);
			m.Add(id_b31);
			NUnit.Framework.Assert.IsFalse(m.IsEmpty());
			NUnit.Framework.Assert.AreEqual(5, m.Size());
			NUnit.Framework.Assert.AreSame(id_1, m.Get(id_1));
			NUnit.Framework.Assert.AreSame(id_1, m.Get(Id(1)));
			NUnit.Framework.Assert.AreSame(id_1, m.Get(Id(1).Copy()));
			NUnit.Framework.Assert.AreSame(id_2, m.Get(Id(2).Copy()));
			NUnit.Framework.Assert.AreSame(id_3, m.Get(Id(3).Copy()));
			NUnit.Framework.Assert.AreSame(id_a31, m.Get(Id(31).Copy()));
			NUnit.Framework.Assert.AreSame(id_b31, m.Get(id_b31.Copy()));
			NUnit.Framework.Assert.IsTrue(m.Contains(id_1));
		}

		[NUnit.Framework.Test]
		public virtual void TestClear()
		{
			ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId> m = new ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId
				>();
			m.Add(id_1);
			NUnit.Framework.Assert.AreSame(id_1, m.Get(id_1));
			m.Clear();
			NUnit.Framework.Assert.IsTrue(m.IsEmpty());
			NUnit.Framework.Assert.AreEqual(0, m.Size());
			Iterator<ObjectIdOwnerMapTest.SubId> i = m.Iterator();
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.IsFalse(i.HasNext());
			NUnit.Framework.Assert.IsFalse(m.Contains(Id(1)));
		}

		[NUnit.Framework.Test]
		public virtual void TestAddIfAbsent()
		{
			ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId> m = new ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId
				>();
			m.Add(id_1);
			NUnit.Framework.Assert.AreSame(id_1, m.AddIfAbsent(new ObjectIdOwnerMapTest.SubId
				(id_1)));
			NUnit.Framework.Assert.AreEqual(1, m.Size());
			NUnit.Framework.Assert.AreSame(id_2, m.AddIfAbsent(id_2));
			NUnit.Framework.Assert.AreEqual(2, m.Size());
			NUnit.Framework.Assert.AreSame(id_a31, m.AddIfAbsent(id_a31));
			NUnit.Framework.Assert.AreSame(id_b31, m.AddIfAbsent(id_b31));
			NUnit.Framework.Assert.AreSame(id_a31, m.AddIfAbsent(new ObjectIdOwnerMapTest.SubId
				(id_a31)));
			NUnit.Framework.Assert.AreSame(id_b31, m.AddIfAbsent(new ObjectIdOwnerMapTest.SubId
				(id_b31)));
			NUnit.Framework.Assert.AreEqual(4, m.Size());
		}

		[NUnit.Framework.Test]
		public virtual void TestAddGrowsWithObjects()
		{
			int n = 16384;
			ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId> m = new ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId
				>();
			m.Add(id_1);
			for (int i = 32; i < n; i++)
			{
				m.Add(new ObjectIdOwnerMapTest.SubId(Id(i)));
			}
			NUnit.Framework.Assert.AreEqual(n - 32 + 1, m.Size());
			NUnit.Framework.Assert.AreSame(id_1, m.Get(id_1.Copy()));
			for (int i_1 = 32; i_1 < n; i_1++)
			{
				NUnit.Framework.Assert.IsTrue(m.Contains(Id(i_1)));
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestAddIfAbsentGrowsWithObjects()
		{
			int n = 16384;
			ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId> m = new ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId
				>();
			m.Add(id_1);
			for (int i = 32; i < n; i++)
			{
				m.AddIfAbsent(new ObjectIdOwnerMapTest.SubId(Id(i)));
			}
			NUnit.Framework.Assert.AreEqual(n - 32 + 1, m.Size());
			NUnit.Framework.Assert.AreSame(id_1, m.Get(id_1.Copy()));
			for (int i_1 = 32; i_1 < n; i_1++)
			{
				NUnit.Framework.Assert.IsTrue(m.Contains(Id(i_1)));
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestIterator()
		{
			ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId> m = new ObjectIdOwnerMap<ObjectIdOwnerMapTest.SubId
				>();
			m.Add(id_1);
			m.Add(id_2);
			m.Add(id_3);
			Iterator<ObjectIdOwnerMapTest.SubId> i = m.Iterator();
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreSame(id_1, i.Next());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreSame(id_2, i.Next());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreSame(id_3, i.Next());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
			try
			{
				i.Next();
				NUnit.Framework.Assert.Fail("did not fail on next with no next");
			}
			catch (NoSuchElementException)
			{
			}
			// OK
			i = m.Iterator();
			NUnit.Framework.Assert.AreSame(id_1, i.Next());
			try
			{
				i.Remove();
				NUnit.Framework.Assert.Fail("did not fail on remove");
			}
			catch (NotSupportedException)
			{
			}
		}

		// OK
		private AnyObjectId Id(int val)
		{
			idBuf.SetByte(0, val & unchecked((int)(0xff)));
			idBuf.SetByte(3, ((int)(((uint)val) >> 8)) & unchecked((int)(0xff)));
			return idBuf;
		}

		[System.Serializable]
		internal class SubId : ObjectIdOwnerMap.Entry
		{
			public SubId(AnyObjectId id) : base(id)
			{
			}
		}
	}
}
