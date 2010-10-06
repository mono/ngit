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

using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	[NUnit.Framework.TestFixture]
	public class EditTest
	{
		[NUnit.Framework.Test]
		public virtual void TestCreate()
		{
			Edit e = new Edit(1, 2, 3, 4);
			NUnit.Framework.Assert.AreEqual(1, e.GetBeginA());
			NUnit.Framework.Assert.AreEqual(2, e.GetEndA());
			NUnit.Framework.Assert.AreEqual(3, e.GetBeginB());
			NUnit.Framework.Assert.AreEqual(4, e.GetEndB());
		}

		[NUnit.Framework.Test]
		public virtual void TestCreateEmpty()
		{
			Edit e = new Edit(1, 3);
			NUnit.Framework.Assert.AreEqual(1, e.GetBeginA());
			NUnit.Framework.Assert.AreEqual(1, e.GetEndA());
			NUnit.Framework.Assert.AreEqual(3, e.GetBeginB());
			NUnit.Framework.Assert.AreEqual(3, e.GetEndB());
			NUnit.Framework.Assert.IsTrue(e.IsEmpty(), "is empty");
			NUnit.Framework.Assert.AreEqual(Edit.Type.EMPTY, e.GetType());
		}

		[NUnit.Framework.Test]
		public virtual void TestSwap()
		{
			Edit e = new Edit(1, 2, 3, 4);
			e.Swap();
			NUnit.Framework.Assert.AreEqual(3, e.GetBeginA());
			NUnit.Framework.Assert.AreEqual(4, e.GetEndA());
			NUnit.Framework.Assert.AreEqual(1, e.GetBeginB());
			NUnit.Framework.Assert.AreEqual(2, e.GetEndB());
		}

		[NUnit.Framework.Test]
		public virtual void TestType_Insert()
		{
			Edit e = new Edit(1, 1, 1, 2);
			NUnit.Framework.Assert.AreEqual(Edit.Type.INSERT, e.GetType());
			NUnit.Framework.Assert.IsFalse(e.IsEmpty(), "not empty");
			NUnit.Framework.Assert.AreEqual(0, e.GetLengthA());
			NUnit.Framework.Assert.AreEqual(1, e.GetLengthB());
		}

		[NUnit.Framework.Test]
		public virtual void TestType_Delete()
		{
			Edit e = new Edit(1, 2, 1, 1);
			NUnit.Framework.Assert.AreEqual(Edit.Type.DELETE, e.GetType());
			NUnit.Framework.Assert.IsFalse(e.IsEmpty(), "not empty");
			NUnit.Framework.Assert.AreEqual(1, e.GetLengthA());
			NUnit.Framework.Assert.AreEqual(0, e.GetLengthB());
		}

		[NUnit.Framework.Test]
		public virtual void TestType_Replace()
		{
			Edit e = new Edit(1, 2, 1, 4);
			NUnit.Framework.Assert.AreEqual(Edit.Type.REPLACE, e.GetType());
			NUnit.Framework.Assert.IsFalse(e.IsEmpty(), "not empty");
			NUnit.Framework.Assert.AreEqual(1, e.GetLengthA());
			NUnit.Framework.Assert.AreEqual(3, e.GetLengthB());
		}

		[NUnit.Framework.Test]
		public virtual void TestType_Empty()
		{
			Edit e = new Edit(1, 1, 2, 2);
			NUnit.Framework.Assert.AreEqual(Edit.Type.EMPTY, e.GetType());
			NUnit.Framework.Assert.AreEqual(Edit.Type.EMPTY, new Edit(1, 2).GetType());
			NUnit.Framework.Assert.IsTrue(e.IsEmpty(), "is empty");
			NUnit.Framework.Assert.AreEqual(0, e.GetLengthA());
			NUnit.Framework.Assert.AreEqual(0, e.GetLengthB());
		}

		[NUnit.Framework.Test]
		public virtual void TestToString()
		{
			Edit e = new Edit(1, 2, 1, 4);
			NUnit.Framework.Assert.AreEqual("REPLACE(1-2,1-4)", e.ToString());
		}

		[NUnit.Framework.Test]
		public virtual void TestEquals1()
		{
			Edit e1 = new Edit(1, 2, 3, 4);
			Edit e2 = new Edit(1, 2, 3, 4);
			NUnit.Framework.Assert.IsTrue(e1.Equals(e1));
			NUnit.Framework.Assert.IsTrue(e1.Equals(e2));
			NUnit.Framework.Assert.IsTrue(e2.Equals(e1));
			NUnit.Framework.Assert.AreEqual(e1.GetHashCode(), e2.GetHashCode());
			NUnit.Framework.Assert.IsFalse(e1.Equals(string.Empty));
		}

		[NUnit.Framework.Test]
		public virtual void TestNotEquals1()
		{
			NUnit.Framework.Assert.IsFalse(new Edit(1, 2, 3, 4).Equals(new Edit(0, 2, 3, 4)));
		}

		[NUnit.Framework.Test]
		public virtual void TestNotEquals2()
		{
			NUnit.Framework.Assert.IsFalse(new Edit(1, 2, 3, 4).Equals(new Edit(1, 0, 3, 4)));
		}

		[NUnit.Framework.Test]
		public virtual void TestNotEquals3()
		{
			NUnit.Framework.Assert.IsFalse(new Edit(1, 2, 3, 4).Equals(new Edit(1, 2, 0, 4)));
		}

		[NUnit.Framework.Test]
		public virtual void TestNotEquals4()
		{
			NUnit.Framework.Assert.IsFalse(new Edit(1, 2, 3, 4).Equals(new Edit(1, 2, 3, 0)));
		}

		[NUnit.Framework.Test]
		public virtual void TestExtendA()
		{
			Edit e = new Edit(1, 2, 1, 1);
			e.ExtendA();
			NUnit.Framework.Assert.AreEqual(new Edit(1, 3, 1, 1), e);
			e.ExtendA();
			NUnit.Framework.Assert.AreEqual(new Edit(1, 4, 1, 1), e);
		}

		[NUnit.Framework.Test]
		public virtual void TestExtendB()
		{
			Edit e = new Edit(1, 2, 1, 1);
			e.ExtendB();
			NUnit.Framework.Assert.AreEqual(new Edit(1, 2, 1, 2), e);
			e.ExtendB();
			NUnit.Framework.Assert.AreEqual(new Edit(1, 2, 1, 3), e);
		}

		[NUnit.Framework.Test]
		public virtual void TestBeforeAfterCuts()
		{
			Edit whole = new Edit(1, 8, 2, 9);
			Edit mid = new Edit(4, 5, 3, 6);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 4, 2, 3), whole.Before(mid));
			NUnit.Framework.Assert.AreEqual(new Edit(5, 8, 6, 9), whole.After(mid));
		}
	}
}
