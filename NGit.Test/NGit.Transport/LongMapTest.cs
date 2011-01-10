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

using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	[NUnit.Framework.TestFixture]
	public class LongMapTest
	{
		private LongMap<long> map;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			map = new LongMap<long>();
		}

		[NUnit.Framework.Test]
		public virtual void TestEmptyMap()
		{
			NUnit.Framework.Assert.IsFalse(map.ContainsKey(0));
			NUnit.Framework.Assert.IsFalse(map.ContainsKey(1));
			NUnit.Framework.Assert.IsNull(map.Get(0));
			NUnit.Framework.Assert.IsNull(map.Get(1));
			NUnit.Framework.Assert.IsNull(map.Remove(0));
			NUnit.Framework.Assert.IsNull(map.Remove(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestInsertMinValue()
		{
			long min = Sharpen.Extensions.ValueOf(long.MinValue);
			NUnit.Framework.Assert.IsNull(map.Put(long.MinValue, min));
			NUnit.Framework.Assert.IsTrue(map.ContainsKey(long.MinValue));
			NUnit.Framework.Assert.AreSame(min, map.Get(long.MinValue));
			NUnit.Framework.Assert.IsFalse(map.ContainsKey(int.MinValue));
		}

		[NUnit.Framework.Test]
		public virtual void TestReplaceMaxValue()
		{
			long min = Sharpen.Extensions.ValueOf(long.MaxValue);
			long one = Sharpen.Extensions.ValueOf(1);
			NUnit.Framework.Assert.IsNull(map.Put(long.MaxValue, min));
			NUnit.Framework.Assert.AreSame(min, map.Get(long.MaxValue));
			NUnit.Framework.Assert.AreSame(min, map.Put(long.MaxValue, one));
			NUnit.Framework.Assert.AreSame(one, map.Get(long.MaxValue));
		}

		[NUnit.Framework.Test]
		public virtual void TestRemoveOne()
		{
			long start = 1;
			NUnit.Framework.Assert.IsNull(map.Put(start, Sharpen.Extensions.ValueOf(start)));
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(start), map.Remove(start
				));
			NUnit.Framework.Assert.IsFalse(map.ContainsKey(start));
		}

		[NUnit.Framework.Test]
		public virtual void TestRemoveCollision1()
		{
			// This test relies upon the fact that we always >>> 1 the value
			// to derive an unsigned hash code. Thus, 0 and 1 fall into the
			// same hash bucket. Further it relies on the fact that we add
			// the 2nd put at the top of the chain, so removing the 1st will
			// cause a different code path.
			//
			NUnit.Framework.Assert.IsNull(map.Put(0, Sharpen.Extensions.ValueOf(0)));
			NUnit.Framework.Assert.IsNull(map.Put(1, Sharpen.Extensions.ValueOf(1)));
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(0), map.Remove(0));
			NUnit.Framework.Assert.IsFalse(map.ContainsKey(0));
			NUnit.Framework.Assert.IsTrue(map.ContainsKey(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestRemoveCollision2()
		{
			// This test relies upon the fact that we always >>> 1 the value
			// to derive an unsigned hash code. Thus, 0 and 1 fall into the
			// same hash bucket. Further it relies on the fact that we add
			// the 2nd put at the top of the chain, so removing the 2nd will
			// cause a different code path.
			//
			NUnit.Framework.Assert.IsNull(map.Put(0, Sharpen.Extensions.ValueOf(0)));
			NUnit.Framework.Assert.IsNull(map.Put(1, Sharpen.Extensions.ValueOf(1)));
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(1), map.Remove(1));
			NUnit.Framework.Assert.IsTrue(map.ContainsKey(0));
			NUnit.Framework.Assert.IsFalse(map.ContainsKey(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestSmallMap()
		{
			long start = 12;
			long n = 8;
			for (long i = start; i < start + n; i++)
			{
				NUnit.Framework.Assert.IsNull(map.Put(i, Sharpen.Extensions.ValueOf(i)));
			}
			for (long i_1 = start; i_1 < start + n; i_1++)
			{
				NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(i_1), map.Get(i_1));
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestLargeMap()
		{
			long start = int.MaxValue;
			long n = 100000;
			for (long i = start; i < start + n; i++)
			{
				NUnit.Framework.Assert.IsNull(map.Put(i, Sharpen.Extensions.ValueOf(i)));
			}
			for (long i_1 = start; i_1 < start + n; i_1++)
			{
				NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(i_1), map.Get(i_1));
			}
		}
	}
}
