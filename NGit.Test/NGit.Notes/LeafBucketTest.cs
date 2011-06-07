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
using NGit.Notes;
using Sharpen;

namespace NGit.Notes
{
	[NUnit.Framework.TestFixture]
	public class LeafBucketTest
	{
		[NUnit.Framework.Test]
		public virtual void TestEmpty()
		{
			LeafBucket b = new LeafBucket(0);
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x00))), null));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x01))), null));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0xfe))), null));
		}

		[NUnit.Framework.Test]
		public virtual void TestParseFive()
		{
			LeafBucket b = new LeafBucket(0);
			b.ParseOneEntry(Id(unchecked((int)(0x11))), Id(unchecked((int)(0x81))));
			b.ParseOneEntry(Id(unchecked((int)(0x22))), Id(unchecked((int)(0x82))));
			b.ParseOneEntry(Id(unchecked((int)(0x33))), Id(unchecked((int)(0x83))));
			b.ParseOneEntry(Id(unchecked((int)(0x44))), Id(unchecked((int)(0x84))));
			b.ParseOneEntry(Id(unchecked((int)(0x55))), Id(unchecked((int)(0x85))));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x01))), null));
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x81))), b.GetNote(Id(unchecked(
				(int)(0x11))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x82))), b.GetNote(Id(unchecked(
				(int)(0x22))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x83))), b.GetNote(Id(unchecked(
				(int)(0x33))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x84))), b.GetNote(Id(unchecked(
				(int)(0x44))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x85))), b.GetNote(Id(unchecked(
				(int)(0x55))), null).GetData());
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x66))), null));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSetFive_InOrder()
		{
			LeafBucket b = new LeafBucket(0);
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x11))), Id(unchecked(
				(int)(0x81))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x22))), Id(unchecked(
				(int)(0x82))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x33))), Id(unchecked(
				(int)(0x83))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x44))), Id(unchecked(
				(int)(0x84))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x55))), Id(unchecked(
				(int)(0x85))), null));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x01))), null));
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x81))), b.GetNote(Id(unchecked(
				(int)(0x11))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x82))), b.GetNote(Id(unchecked(
				(int)(0x22))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x83))), b.GetNote(Id(unchecked(
				(int)(0x33))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x84))), b.GetNote(Id(unchecked(
				(int)(0x44))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x85))), b.GetNote(Id(unchecked(
				(int)(0x55))), null).GetData());
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x66))), null));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSetFive_ReverseOrder()
		{
			LeafBucket b = new LeafBucket(0);
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x55))), Id(unchecked(
				(int)(0x85))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x44))), Id(unchecked(
				(int)(0x84))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x33))), Id(unchecked(
				(int)(0x83))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x22))), Id(unchecked(
				(int)(0x82))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x11))), Id(unchecked(
				(int)(0x81))), null));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x01))), null));
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x81))), b.GetNote(Id(unchecked(
				(int)(0x11))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x82))), b.GetNote(Id(unchecked(
				(int)(0x22))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x83))), b.GetNote(Id(unchecked(
				(int)(0x33))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x84))), b.GetNote(Id(unchecked(
				(int)(0x44))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x85))), b.GetNote(Id(unchecked(
				(int)(0x55))), null).GetData());
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x66))), null));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSetFive_MixedOrder()
		{
			LeafBucket b = new LeafBucket(0);
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x11))), Id(unchecked(
				(int)(0x81))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x33))), Id(unchecked(
				(int)(0x83))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x55))), Id(unchecked(
				(int)(0x85))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x22))), Id(unchecked(
				(int)(0x82))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x44))), Id(unchecked(
				(int)(0x84))), null));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x01))), null));
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x81))), b.GetNote(Id(unchecked(
				(int)(0x11))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x82))), b.GetNote(Id(unchecked(
				(int)(0x22))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x83))), b.GetNote(Id(unchecked(
				(int)(0x33))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x84))), b.GetNote(Id(unchecked(
				(int)(0x44))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x85))), b.GetNote(Id(unchecked(
				(int)(0x55))), null).GetData());
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x66))), null));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSet_Replace()
		{
			LeafBucket b = new LeafBucket(0);
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x11))), Id(unchecked(
				(int)(0x81))), null));
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x81))), b.GetNote(Id(unchecked(
				(int)(0x11))), null).GetData());
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x11))), Id(unchecked(
				(int)(0x01))), null));
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x01))), b.GetNote(Id(unchecked(
				(int)(0x11))), null).GetData());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRemoveMissingNote()
		{
			LeafBucket b = new LeafBucket(0);
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x11))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x11))), null, null));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x11))), null));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRemoveFirst()
		{
			LeafBucket b = new LeafBucket(0);
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x11))), Id(unchecked(
				(int)(0x81))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x22))), Id(unchecked(
				(int)(0x82))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x33))), Id(unchecked(
				(int)(0x83))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x44))), Id(unchecked(
				(int)(0x84))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x55))), Id(unchecked(
				(int)(0x85))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x11))), null, null));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x01))), null));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x11))), null));
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x82))), b.GetNote(Id(unchecked(
				(int)(0x22))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x83))), b.GetNote(Id(unchecked(
				(int)(0x33))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x84))), b.GetNote(Id(unchecked(
				(int)(0x44))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x85))), b.GetNote(Id(unchecked(
				(int)(0x55))), null).GetData());
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x66))), null));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRemoveMiddle()
		{
			LeafBucket b = new LeafBucket(0);
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x11))), Id(unchecked(
				(int)(0x81))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x22))), Id(unchecked(
				(int)(0x82))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x33))), Id(unchecked(
				(int)(0x83))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x44))), Id(unchecked(
				(int)(0x84))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x55))), Id(unchecked(
				(int)(0x85))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x33))), null, null));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x01))), null));
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x81))), b.GetNote(Id(unchecked(
				(int)(0x11))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x82))), b.GetNote(Id(unchecked(
				(int)(0x22))), null).GetData());
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x33))), null));
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x84))), b.GetNote(Id(unchecked(
				(int)(0x44))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x85))), b.GetNote(Id(unchecked(
				(int)(0x55))), null).GetData());
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x66))), null));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRemoveLast()
		{
			LeafBucket b = new LeafBucket(0);
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x11))), Id(unchecked(
				(int)(0x81))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x22))), Id(unchecked(
				(int)(0x82))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x33))), Id(unchecked(
				(int)(0x83))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x44))), Id(unchecked(
				(int)(0x84))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x55))), Id(unchecked(
				(int)(0x85))), null));
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x55))), null, null));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x01))), null));
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x81))), b.GetNote(Id(unchecked(
				(int)(0x11))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x82))), b.GetNote(Id(unchecked(
				(int)(0x22))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x83))), b.GetNote(Id(unchecked(
				(int)(0x33))), null).GetData());
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x84))), b.GetNote(Id(unchecked(
				(int)(0x44))), null).GetData());
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x55))), null));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x66))), null));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRemoveMakesEmpty()
		{
			LeafBucket b = new LeafBucket(0);
			NUnit.Framework.Assert.AreSame(b, b.Set(Id(unchecked((int)(0x11))), Id(unchecked(
				(int)(0x81))), null));
			NUnit.Framework.Assert.AreEqual(Id(unchecked((int)(0x81))), b.GetNote(Id(unchecked(
				(int)(0x11))), null).GetData());
			NUnit.Framework.Assert.IsNull(b.Set(Id(unchecked((int)(0x11))), null, null));
			NUnit.Framework.Assert.IsNull(b.GetNote(Id(unchecked((int)(0x11))), null));
		}

		private static AnyObjectId Id(int first)
		{
			MutableObjectId id = new MutableObjectId();
			id.SetByte(1, first);
			return id;
		}
	}
}
