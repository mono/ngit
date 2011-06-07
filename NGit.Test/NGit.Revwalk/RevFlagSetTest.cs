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

using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	[NUnit.Framework.TestFixture]
	public class RevFlagSetTest : RevWalkTestCase
	{
		[NUnit.Framework.Test]
		public virtual void TestEmpty()
		{
			RevFlagSet set = new RevFlagSet();
			NUnit.Framework.Assert.AreEqual(0, set.mask);
			NUnit.Framework.Assert.AreEqual(0, set.Count);
			NUnit.Framework.Assert.IsNotNull(set.Iterator());
			NUnit.Framework.Assert.IsFalse(set.Iterator().HasNext());
		}

		[NUnit.Framework.Test]
		public virtual void TestAddOne()
		{
			string flagName = "flag";
			RevFlag flag = rw.NewFlag(flagName);
			NUnit.Framework.Assert.IsTrue(0 != flag.mask);
			NUnit.Framework.Assert.AreSame(flagName, flag.name);
			RevFlagSet set = new RevFlagSet();
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag));
			NUnit.Framework.Assert.IsFalse(set.AddItem(flag));
			NUnit.Framework.Assert.AreEqual(flag.mask, set.mask);
			NUnit.Framework.Assert.AreEqual(1, set.Count);
			Iterator<RevFlag> i = set.Iterator();
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreSame(flag, i.Next());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
		}

		[NUnit.Framework.Test]
		public virtual void TestAddTwo()
		{
			RevFlag flag1 = rw.NewFlag("flag_1");
			RevFlag flag2 = rw.NewFlag("flag_2");
			NUnit.Framework.Assert.IsTrue((flag1.mask & flag2.mask) == 0);
			RevFlagSet set = new RevFlagSet();
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag1));
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag2));
			NUnit.Framework.Assert.AreEqual(flag1.mask | flag2.mask, set.mask);
			NUnit.Framework.Assert.AreEqual(2, set.Count);
		}

		[NUnit.Framework.Test]
		public virtual void TestContainsAll()
		{
			RevFlag flag1 = rw.NewFlag("flag_1");
			RevFlag flag2 = rw.NewFlag("flag_2");
			RevFlagSet set1 = new RevFlagSet();
			NUnit.Framework.Assert.IsTrue(set1.AddItem(flag1));
			NUnit.Framework.Assert.IsTrue(set1.AddItem(flag2));
			NUnit.Framework.Assert.IsTrue(set1.ContainsAll(set1));
			NUnit.Framework.Assert.IsTrue(set1.ContainsAll(Arrays.AsList(new RevFlag[] { flag1
				, flag2 })));
			RevFlagSet set2 = new RevFlagSet();
			set2.AddItem(rw.NewFlag("flag_3"));
			NUnit.Framework.Assert.IsFalse(set1.ContainsAll(set2));
		}

		[NUnit.Framework.Test]
		public virtual void TestEquals()
		{
			RevFlag flag1 = rw.NewFlag("flag_1");
			RevFlag flag2 = rw.NewFlag("flag_2");
			RevFlagSet set = new RevFlagSet();
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag1));
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag2));
			NUnit.Framework.Assert.IsTrue(new RevFlagSet(set).Equals(set));
			NUnit.Framework.Assert.IsTrue(new RevFlagSet(Arrays.AsList(new RevFlag[] { flag1, 
				flag2 })).Equals(set));
		}

		[NUnit.Framework.Test]
		public virtual void TestRemove()
		{
			RevFlag flag1 = rw.NewFlag("flag_1");
			RevFlag flag2 = rw.NewFlag("flag_2");
			RevFlagSet set = new RevFlagSet();
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag1));
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag2));
			NUnit.Framework.Assert.IsTrue(set.Remove(flag1));
			NUnit.Framework.Assert.IsFalse(set.Remove(flag1));
			NUnit.Framework.Assert.AreEqual(flag2.mask, set.mask);
			NUnit.Framework.Assert.IsFalse(set.Contains(flag1));
		}

		[NUnit.Framework.Test]
		public virtual void TestContains()
		{
			RevFlag flag1 = rw.NewFlag("flag_1");
			RevFlag flag2 = rw.NewFlag("flag_2");
			RevFlagSet set = new RevFlagSet();
			set.AddItem(flag1);
			NUnit.Framework.Assert.IsTrue(set.Contains(flag1));
			NUnit.Framework.Assert.IsFalse(set.Contains(flag2));
			NUnit.Framework.Assert.IsFalse(set.Contains("bob"));
		}
	}
}
