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

using NGit;
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class ObjectIdTest
	{
		[NUnit.Framework.Test]
		public virtual void Test001_toString()
		{
			string x = "def4c620bc3713bb1bb26b808ec9312548e73946";
			ObjectId oid = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(x, oid.Name);
		}

		[NUnit.Framework.Test]
		public virtual void Test002_toString()
		{
			string x = "ff00eedd003713bb1bb26b808ec9312548e73946";
			ObjectId oid = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(x, oid.Name);
		}

		[NUnit.Framework.Test]
		public virtual void Test003_equals()
		{
			string x = "def4c620bc3713bb1bb26b808ec9312548e73946";
			ObjectId a = ObjectId.FromString(x);
			ObjectId b = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
			NUnit.Framework.Assert.IsTrue(a.Equals(b), "a and b are same");
		}

		[NUnit.Framework.Test]
		public virtual void Test004_isId()
		{
			NUnit.Framework.Assert.IsTrue(ObjectId.IsId("def4c620bc3713bb1bb26b808ec9312548e73946"
				), "valid id");
		}

		[NUnit.Framework.Test]
		public virtual void Test005_notIsId()
		{
			NUnit.Framework.Assert.IsFalse(ObjectId.IsId("bob"), "bob is not an id");
		}

		[NUnit.Framework.Test]
		public virtual void Test006_notIsId()
		{
			NUnit.Framework.Assert.IsFalse(ObjectId.IsId("def4c620bc3713bb1bb26b808ec9312548e7394"
				), "39 digits is not an id");
		}

		[NUnit.Framework.Test]
		public virtual void Test007_isId()
		{
			NUnit.Framework.Assert.IsTrue(ObjectId.IsId("Def4c620bc3713bb1bb26b808ec9312548e73946"
				), "uppercase is accepted");
		}

		[NUnit.Framework.Test]
		public virtual void Test008_notIsId()
		{
			NUnit.Framework.Assert.IsFalse(ObjectId.IsId("gef4c620bc3713bb1bb26b808ec9312548e73946"
				), "g is not a valid hex digit");
		}

		[NUnit.Framework.Test]
		public virtual void Test009_toString()
		{
			string x = "ff00eedd003713bb1bb26b808ec9312548e73946";
			ObjectId oid = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(x, ObjectId.ToString(oid));
		}

		[NUnit.Framework.Test]
		public virtual void Test010_toString()
		{
			string x = "0000000000000000000000000000000000000000";
			NUnit.Framework.Assert.AreEqual(x, ObjectId.ToString(null));
		}

		[NUnit.Framework.Test]
		public virtual void Test011_toString()
		{
			string x = "0123456789ABCDEFabcdef1234567890abcdefAB";
			ObjectId oid = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(x.ToLower(), oid.Name);
		}

		[NUnit.Framework.Test]
		public virtual void TestGetByte()
		{
			byte[] raw = new byte[20];
			for (int i = 0; i < 20; i++)
			{
				raw[i] = unchecked((byte)(unchecked((int)(0xa0)) + i));
			}
			ObjectId id = ObjectId.FromRaw(raw);
			NUnit.Framework.Assert.AreEqual(raw[0] & unchecked((int)(0xff)), id.FirstByte);
			NUnit.Framework.Assert.AreEqual(raw[0] & unchecked((int)(0xff)), id.GetByte(0));
			NUnit.Framework.Assert.AreEqual(raw[1] & unchecked((int)(0xff)), id.GetByte(1));
			for (int i_1 = 2; i_1 < 20; i_1++)
			{
				NUnit.Framework.Assert.AreEqual(raw[i_1] & unchecked((int)(0xff)), id.GetByte(i_1
					), "index " + i_1);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestSetByte()
		{
			byte[] exp = new byte[20];
			for (int i = 0; i < 20; i++)
			{
				exp[i] = unchecked((byte)(unchecked((int)(0xa0)) + i));
			}
			MutableObjectId id = new MutableObjectId();
			id.FromRaw(exp);
			NUnit.Framework.Assert.AreEqual(ObjectId.FromRaw(exp).Name, id.Name);
			id.SetByte(0, unchecked((int)(0x10)));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x10)), id.GetByte(0));
			exp[0] = unchecked((int)(0x10));
			NUnit.Framework.Assert.AreEqual(ObjectId.FromRaw(exp).Name, id.Name);
			for (int p = 1; p < 20; p++)
			{
				id.SetByte(p, unchecked((int)(0x10)) + p);
				NUnit.Framework.Assert.AreEqual(unchecked((int)(0x10)) + p, id.GetByte(p));
				exp[p] = unchecked((byte)(unchecked((int)(0x10)) + p));
				NUnit.Framework.Assert.AreEqual(ObjectId.FromRaw(exp).Name, id.Name);
			}
			for (int p_1 = 0; p_1 < 20; p_1++)
			{
				id.SetByte(p_1, unchecked((int)(0x80)) + p_1);
				NUnit.Framework.Assert.AreEqual(unchecked((int)(0x80)) + p_1, id.GetByte(p_1));
				exp[p_1] = unchecked((byte)(unchecked((int)(0x80)) + p_1));
				NUnit.Framework.Assert.AreEqual(ObjectId.FromRaw(exp).Name, id.Name);
			}
		}
	}
}
