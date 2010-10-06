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
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	[NUnit.Framework.TestFixture]
	public class IntListTest
	{
		[NUnit.Framework.Test]
		public virtual void TestEmpty_DefaultCapacity()
		{
			IntList i = new IntList();
			NUnit.Framework.Assert.AreEqual(0, i.Size());
			try
			{
				i.Get(0);
				NUnit.Framework.Assert.Fail("Accepted 0 index on empty list");
			}
			catch (IndexOutOfRangeException)
			{
				NUnit.Framework.Assert.IsTrue(true);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestEmpty_SpecificCapacity()
		{
			IntList i = new IntList(5);
			NUnit.Framework.Assert.AreEqual(0, i.Size());
			try
			{
				i.Get(0);
				NUnit.Framework.Assert.Fail("Accepted 0 index on empty list");
			}
			catch (IndexOutOfRangeException)
			{
				NUnit.Framework.Assert.IsTrue(true);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestAdd_SmallGroup()
		{
			IntList i = new IntList();
			int n = 5;
			for (int v = 0; v < n; v++)
			{
				i.Add(10 + v);
			}
			NUnit.Framework.Assert.AreEqual(n, i.Size());
			for (int v_1 = 0; v_1 < n; v_1++)
			{
				NUnit.Framework.Assert.AreEqual(10 + v_1, i.Get(v_1));
			}
			try
			{
				i.Get(n);
				NUnit.Framework.Assert.Fail("Accepted out of bound index on list");
			}
			catch (IndexOutOfRangeException)
			{
				NUnit.Framework.Assert.IsTrue(true);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestAdd_ZeroCapacity()
		{
			IntList i = new IntList(0);
			NUnit.Framework.Assert.AreEqual(0, i.Size());
			i.Add(1);
			NUnit.Framework.Assert.AreEqual(1, i.Get(0));
		}

		[NUnit.Framework.Test]
		public virtual void TestAdd_LargeGroup()
		{
			IntList i = new IntList();
			int n = 500;
			for (int v = 0; v < n; v++)
			{
				i.Add(10 + v);
			}
			NUnit.Framework.Assert.AreEqual(n, i.Size());
			for (int v_1 = 0; v_1 < n; v_1++)
			{
				NUnit.Framework.Assert.AreEqual(10 + v_1, i.Get(v_1));
			}
			try
			{
				i.Get(n);
				NUnit.Framework.Assert.Fail("Accepted out of bound index on list");
			}
			catch (IndexOutOfRangeException)
			{
				NUnit.Framework.Assert.IsTrue(true);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestFillTo0()
		{
			IntList i = new IntList();
			i.FillTo(0, int.MinValue);
			NUnit.Framework.Assert.AreEqual(0, i.Size());
		}

		[NUnit.Framework.Test]
		public virtual void TestFillTo1()
		{
			IntList i = new IntList();
			i.FillTo(1, int.MinValue);
			NUnit.Framework.Assert.AreEqual(1, i.Size());
			i.Add(0);
			NUnit.Framework.Assert.AreEqual(int.MinValue, i.Get(0));
			NUnit.Framework.Assert.AreEqual(0, i.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestFillTo100()
		{
			IntList i = new IntList();
			i.FillTo(100, int.MinValue);
			NUnit.Framework.Assert.AreEqual(100, i.Size());
			i.Add(3);
			NUnit.Framework.Assert.AreEqual(int.MinValue, i.Get(99));
			NUnit.Framework.Assert.AreEqual(3, i.Get(100));
		}

		[NUnit.Framework.Test]
		public virtual void TestClear()
		{
			IntList i = new IntList();
			int n = 5;
			for (int v = 0; v < n; v++)
			{
				i.Add(10 + v);
			}
			NUnit.Framework.Assert.AreEqual(n, i.Size());
			i.Clear();
			NUnit.Framework.Assert.AreEqual(0, i.Size());
			try
			{
				i.Get(0);
				NUnit.Framework.Assert.Fail("Accepted 0 index on empty list");
			}
			catch (IndexOutOfRangeException)
			{
				NUnit.Framework.Assert.IsTrue(true);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestSet()
		{
			IntList i = new IntList();
			i.Add(1);
			NUnit.Framework.Assert.AreEqual(1, i.Size());
			NUnit.Framework.Assert.AreEqual(1, i.Get(0));
			i.Set(0, 5);
			NUnit.Framework.Assert.AreEqual(5, i.Get(0));
			try
			{
				i.Set(5, 5);
				NUnit.Framework.Assert.Fail("accepted set of 5 beyond end of list");
			}
			catch (IndexOutOfRangeException)
			{
				NUnit.Framework.Assert.IsTrue(true);
			}
			i.Set(1, 2);
			NUnit.Framework.Assert.AreEqual(2, i.Size());
			NUnit.Framework.Assert.AreEqual(2, i.Get(1));
		}

		[NUnit.Framework.Test]
		public virtual void TestToString()
		{
			IntList i = new IntList();
			i.Add(1);
			NUnit.Framework.Assert.AreEqual("[1]", i.ToString());
			i.Add(13);
			i.Add(5);
			NUnit.Framework.Assert.AreEqual("[1, 13, 5]", i.ToString());
		}
	}
}
