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
	public class AbbreviatedObjectIdTest
	{
		[NUnit.Framework.Test]
		public virtual void TestEmpty_FromByteArray()
		{
			AbbreviatedObjectId i;
			i = AbbreviatedObjectId.FromString(new byte[] {  }, 0, 0);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(0, i.Length);
			NUnit.Framework.Assert.IsFalse(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(string.Empty, i.Name);
		}

		[NUnit.Framework.Test]
		public virtual void TestEmpty_FromString()
		{
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(string.Empty);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(0, i.Length);
			NUnit.Framework.Assert.IsFalse(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(string.Empty, i.Name);
		}

		[NUnit.Framework.Test]
		public virtual void TestFull_FromByteArray()
		{
			string s = "7b6e8067ec96acef9a4184b43210d583b6d2f99a";
			byte[] b = Constants.EncodeASCII(s);
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(b, 0, b.Length);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(s.Length, i.Length);
			NUnit.Framework.Assert.IsTrue(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(s, i.Name);
			ObjectId f = i.ToObjectId();
			NUnit.Framework.Assert.IsNotNull(f);
			NUnit.Framework.Assert.AreEqual(ObjectId.FromString(s), f);
			NUnit.Framework.Assert.AreEqual(f.GetHashCode(), i.GetHashCode());
		}

		[NUnit.Framework.Test]
		public virtual void TestFull_FromString()
		{
			string s = "7b6e8067ec96acef9a4184b43210d583b6d2f99a";
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(s.Length, i.Length);
			NUnit.Framework.Assert.IsTrue(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(s, i.Name);
			ObjectId f = i.ToObjectId();
			NUnit.Framework.Assert.IsNotNull(f);
			NUnit.Framework.Assert.AreEqual(ObjectId.FromString(s), f);
			NUnit.Framework.Assert.AreEqual(f.GetHashCode(), i.GetHashCode());
		}

		[NUnit.Framework.Test]
		public virtual void Test1_FromString()
		{
			string s = "7";
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(s.Length, i.Length);
			NUnit.Framework.Assert.IsFalse(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(s, i.Name);
			NUnit.Framework.Assert.IsNull(i.ToObjectId());
		}

		[NUnit.Framework.Test]
		public virtual void Test2_FromString()
		{
			string s = "7b";
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(s.Length, i.Length);
			NUnit.Framework.Assert.IsFalse(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(s, i.Name);
			NUnit.Framework.Assert.IsNull(i.ToObjectId());
		}

		[NUnit.Framework.Test]
		public virtual void Test3_FromString()
		{
			string s = "7b6";
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(s.Length, i.Length);
			NUnit.Framework.Assert.IsFalse(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(s, i.Name);
			NUnit.Framework.Assert.IsNull(i.ToObjectId());
		}

		[NUnit.Framework.Test]
		public virtual void Test4_FromString()
		{
			string s = "7b6e";
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(s.Length, i.Length);
			NUnit.Framework.Assert.IsFalse(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(s, i.Name);
			NUnit.Framework.Assert.IsNull(i.ToObjectId());
		}

		[NUnit.Framework.Test]
		public virtual void Test5_FromString()
		{
			string s = "7b6e8";
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(s.Length, i.Length);
			NUnit.Framework.Assert.IsFalse(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(s, i.Name);
			NUnit.Framework.Assert.IsNull(i.ToObjectId());
		}

		[NUnit.Framework.Test]
		public virtual void Test6_FromString()
		{
			string s = "7b6e80";
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(s.Length, i.Length);
			NUnit.Framework.Assert.IsFalse(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(s, i.Name);
			NUnit.Framework.Assert.IsNull(i.ToObjectId());
		}

		[NUnit.Framework.Test]
		public virtual void Test7_FromString()
		{
			string s = "7b6e806";
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(s.Length, i.Length);
			NUnit.Framework.Assert.IsFalse(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(s, i.Name);
			NUnit.Framework.Assert.IsNull(i.ToObjectId());
		}

		[NUnit.Framework.Test]
		public virtual void Test8_FromString()
		{
			string s = "7b6e8067";
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(s.Length, i.Length);
			NUnit.Framework.Assert.IsFalse(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(s, i.Name);
			NUnit.Framework.Assert.IsNull(i.ToObjectId());
		}

		[NUnit.Framework.Test]
		public virtual void Test9_FromString()
		{
			string s = "7b6e8067e";
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(s.Length, i.Length);
			NUnit.Framework.Assert.IsFalse(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(s, i.Name);
			NUnit.Framework.Assert.IsNull(i.ToObjectId());
		}

		[NUnit.Framework.Test]
		public virtual void Test17_FromString()
		{
			string s = "7b6e8067ec96acef9";
			AbbreviatedObjectId i = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual(s.Length, i.Length);
			NUnit.Framework.Assert.IsFalse(i.IsComplete);
			NUnit.Framework.Assert.AreEqual(s, i.Name);
			NUnit.Framework.Assert.IsNull(i.ToObjectId());
		}

		[NUnit.Framework.Test]
		public virtual void TestEquals_Short()
		{
			string s = "7b6e8067";
			AbbreviatedObjectId a = AbbreviatedObjectId.FromString(s);
			AbbreviatedObjectId b = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.AreNotSame(a, b);
			NUnit.Framework.Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
			NUnit.Framework.Assert.AreEqual(b, a);
			NUnit.Framework.Assert.AreEqual(a, b);
		}

		[NUnit.Framework.Test]
		public virtual void TestEquals_Full()
		{
			string s = "7b6e8067ec96acef9a4184b43210d583b6d2f99a";
			AbbreviatedObjectId a = AbbreviatedObjectId.FromString(s);
			AbbreviatedObjectId b = AbbreviatedObjectId.FromString(s);
			NUnit.Framework.Assert.AreNotSame(a, b);
			NUnit.Framework.Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
			NUnit.Framework.Assert.AreEqual(b, a);
			NUnit.Framework.Assert.AreEqual(a, b);
		}

		[NUnit.Framework.Test]
		public virtual void TestNotEquals_SameLength()
		{
			string sa = "7b6e8067";
			string sb = "7b6e806e";
			AbbreviatedObjectId a = AbbreviatedObjectId.FromString(sa);
			AbbreviatedObjectId b = AbbreviatedObjectId.FromString(sb);
			NUnit.Framework.Assert.IsFalse(a.Equals(b));
			NUnit.Framework.Assert.IsFalse(b.Equals(a));
		}

		[NUnit.Framework.Test]
		public virtual void TestNotEquals_DiffLength()
		{
			string sa = "7b6e8067abcd";
			string sb = "7b6e8067";
			AbbreviatedObjectId a = AbbreviatedObjectId.FromString(sa);
			AbbreviatedObjectId b = AbbreviatedObjectId.FromString(sb);
			NUnit.Framework.Assert.IsFalse(a.Equals(b));
			NUnit.Framework.Assert.IsFalse(b.Equals(a));
		}

		[NUnit.Framework.Test]
		public virtual void TestPrefixCompare_Full()
		{
			string s1 = "7b6e8067ec96acef9a4184b43210d583b6d2f99a";
			AbbreviatedObjectId a = AbbreviatedObjectId.FromString(s1);
			ObjectId i1 = ObjectId.FromString(s1);
			NUnit.Framework.Assert.AreEqual(0, a.PrefixCompare(i1));
			NUnit.Framework.Assert.IsTrue(i1.StartsWith(a));
			string s2 = "7b6e8067ec96acef9a4184b43210d583b6d2f99b";
			ObjectId i2 = ObjectId.FromString(s2);
			NUnit.Framework.Assert.IsTrue(a.PrefixCompare(i2) < 0);
			NUnit.Framework.Assert.IsFalse(i2.StartsWith(a));
			string s3 = "7b6e8067ec96acef9a4184b43210d583b6d2f999";
			ObjectId i3 = ObjectId.FromString(s3);
			NUnit.Framework.Assert.IsTrue(a.PrefixCompare(i3) > 0);
			NUnit.Framework.Assert.IsFalse(i3.StartsWith(a));
		}

		[NUnit.Framework.Test]
		public virtual void TestPrefixCompare_1()
		{
			string sa = "7";
			AbbreviatedObjectId a = AbbreviatedObjectId.FromString(sa);
			string s1 = "7b6e8067ec96acef9a4184b43210d583b6d2f99a";
			ObjectId i1 = ObjectId.FromString(s1);
			NUnit.Framework.Assert.AreEqual(0, a.PrefixCompare(i1));
			NUnit.Framework.Assert.IsTrue(i1.StartsWith(a));
			string s2 = "8b6e8067ec96acef9a4184b43210d583b6d2f99a";
			ObjectId i2 = ObjectId.FromString(s2);
			NUnit.Framework.Assert.IsTrue(a.PrefixCompare(i2) < 0);
			NUnit.Framework.Assert.IsFalse(i2.StartsWith(a));
			string s3 = "6b6e8067ec96acef9a4184b43210d583b6d2f99a";
			ObjectId i3 = ObjectId.FromString(s3);
			NUnit.Framework.Assert.IsTrue(a.PrefixCompare(i3) > 0);
			NUnit.Framework.Assert.IsFalse(i3.StartsWith(a));
		}

		[NUnit.Framework.Test]
		public virtual void TestPrefixCompare_7()
		{
			string sa = "7b6e806";
			AbbreviatedObjectId a = AbbreviatedObjectId.FromString(sa);
			string s1 = "7b6e8067ec96acef9a4184b43210d583b6d2f99a";
			ObjectId i1 = ObjectId.FromString(s1);
			NUnit.Framework.Assert.AreEqual(0, a.PrefixCompare(i1));
			NUnit.Framework.Assert.IsTrue(i1.StartsWith(a));
			string s2 = "7b6e8167ec86acef9a4184b43210d583b6d2f99a";
			ObjectId i2 = ObjectId.FromString(s2);
			NUnit.Framework.Assert.IsTrue(a.PrefixCompare(i2) < 0);
			NUnit.Framework.Assert.IsFalse(i2.StartsWith(a));
			string s3 = "7b6e8057eca6acef9a4184b43210d583b6d2f99a";
			ObjectId i3 = ObjectId.FromString(s3);
			NUnit.Framework.Assert.IsTrue(a.PrefixCompare(i3) > 0);
			NUnit.Framework.Assert.IsFalse(i3.StartsWith(a));
		}

		[NUnit.Framework.Test]
		public virtual void TestPrefixCompare_8()
		{
			string sa = "7b6e8067";
			AbbreviatedObjectId a = AbbreviatedObjectId.FromString(sa);
			string s1 = "7b6e8067ec96acef9a4184b43210d583b6d2f99a";
			ObjectId i1 = ObjectId.FromString(s1);
			NUnit.Framework.Assert.AreEqual(0, a.PrefixCompare(i1));
			NUnit.Framework.Assert.IsTrue(i1.StartsWith(a));
			string s2 = "7b6e8167ec86acef9a4184b43210d583b6d2f99a";
			ObjectId i2 = ObjectId.FromString(s2);
			NUnit.Framework.Assert.IsTrue(a.PrefixCompare(i2) < 0);
			NUnit.Framework.Assert.IsFalse(i2.StartsWith(a));
			string s3 = "7b6e8057eca6acef9a4184b43210d583b6d2f99a";
			ObjectId i3 = ObjectId.FromString(s3);
			NUnit.Framework.Assert.IsTrue(a.PrefixCompare(i3) > 0);
			NUnit.Framework.Assert.IsFalse(i3.StartsWith(a));
		}

		[NUnit.Framework.Test]
		public virtual void TestPrefixCompare_9()
		{
			string sa = "7b6e8067e";
			AbbreviatedObjectId a = AbbreviatedObjectId.FromString(sa);
			string s1 = "7b6e8067ec96acef9a4184b43210d583b6d2f99a";
			ObjectId i1 = ObjectId.FromString(s1);
			NUnit.Framework.Assert.AreEqual(0, a.PrefixCompare(i1));
			NUnit.Framework.Assert.IsTrue(i1.StartsWith(a));
			string s2 = "7b6e8167ec86acef9a4184b43210d583b6d2f99a";
			ObjectId i2 = ObjectId.FromString(s2);
			NUnit.Framework.Assert.IsTrue(a.PrefixCompare(i2) < 0);
			NUnit.Framework.Assert.IsFalse(i2.StartsWith(a));
			string s3 = "7b6e8057eca6acef9a4184b43210d583b6d2f99a";
			ObjectId i3 = ObjectId.FromString(s3);
			NUnit.Framework.Assert.IsTrue(a.PrefixCompare(i3) > 0);
			NUnit.Framework.Assert.IsFalse(i3.StartsWith(a));
		}

		[NUnit.Framework.Test]
		public virtual void TestPrefixCompare_17()
		{
			string sa = "7b6e8067ec96acef9";
			AbbreviatedObjectId a = AbbreviatedObjectId.FromString(sa);
			string s1 = "7b6e8067ec96acef9a4184b43210d583b6d2f99a";
			ObjectId i1 = ObjectId.FromString(s1);
			NUnit.Framework.Assert.AreEqual(0, a.PrefixCompare(i1));
			NUnit.Framework.Assert.IsTrue(i1.StartsWith(a));
			string s2 = "7b6e8067eca6acef9a4184b43210d583b6d2f99a";
			ObjectId i2 = ObjectId.FromString(s2);
			NUnit.Framework.Assert.IsTrue(a.PrefixCompare(i2) < 0);
			NUnit.Framework.Assert.IsFalse(i2.StartsWith(a));
			string s3 = "7b6e8067ec86acef9a4184b43210d583b6d2f99a";
			ObjectId i3 = ObjectId.FromString(s3);
			NUnit.Framework.Assert.IsTrue(a.PrefixCompare(i3) > 0);
			NUnit.Framework.Assert.IsFalse(i3.StartsWith(a));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsId()
		{
			// These are all too short.
			NUnit.Framework.Assert.IsFalse(AbbreviatedObjectId.IsId(string.Empty));
			NUnit.Framework.Assert.IsFalse(AbbreviatedObjectId.IsId("a"));
			// These are too long.
			NUnit.Framework.Assert.IsFalse(AbbreviatedObjectId.IsId(ObjectId.FromString("7b6e8067ec86acef9a4184b43210d583b6d2f99a"
				).Name + "0"));
			NUnit.Framework.Assert.IsFalse(AbbreviatedObjectId.IsId(ObjectId.FromString("7b6e8067ec86acef9a4184b43210d583b6d2f99a"
				).Name + "c0ffee"));
			// These contain non-hex characters.
			NUnit.Framework.Assert.IsFalse(AbbreviatedObjectId.IsId("01notahexstring"));
			// These should all work.
			NUnit.Framework.Assert.IsTrue(AbbreviatedObjectId.IsId("ab"));
			NUnit.Framework.Assert.IsTrue(AbbreviatedObjectId.IsId("abc"));
			NUnit.Framework.Assert.IsTrue(AbbreviatedObjectId.IsId("abcd"));
			NUnit.Framework.Assert.IsTrue(AbbreviatedObjectId.IsId("abcd0"));
			NUnit.Framework.Assert.IsTrue(AbbreviatedObjectId.IsId("abcd09"));
			NUnit.Framework.Assert.IsTrue(AbbreviatedObjectId.IsId(ObjectId.FromString("7b6e8067ec86acef9a4184b43210d583b6d2f99a"
				).Name));
		}
	}
}
