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
using NGit.Treewalk;
using Sharpen;

namespace NGit.Treewalk
{
	public class EmptyTreeIteratorTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAtEOF()
		{
			EmptyTreeIterator etp = new EmptyTreeIterator();
			NUnit.Framework.Assert.IsTrue(etp.First());
			NUnit.Framework.Assert.IsTrue(etp.Eof());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCreateSubtreeIterator()
		{
			EmptyTreeIterator etp = new EmptyTreeIterator();
			ObjectReader reader = db.NewObjectReader();
			AbstractTreeIterator sub = etp.CreateSubtreeIterator(reader);
			NUnit.Framework.Assert.IsNotNull(sub);
			NUnit.Framework.Assert.IsTrue(sub.First());
			NUnit.Framework.Assert.IsTrue(sub.Eof());
			NUnit.Framework.Assert.IsTrue(sub is EmptyTreeIterator);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEntryObjectId()
		{
			EmptyTreeIterator etp = new EmptyTreeIterator();
			NUnit.Framework.Assert.AreSame(ObjectId.ZeroId, etp.GetEntryObjectId());
			NUnit.Framework.Assert.IsNotNull(etp.IdBuffer());
			NUnit.Framework.Assert.AreEqual(0, etp.IdOffset());
			AssertEquals(ObjectId.ZeroId, ObjectId.FromRaw(etp.IdBuffer()));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNextDoesNothing()
		{
			EmptyTreeIterator etp = new EmptyTreeIterator();
			etp.Next(1);
			NUnit.Framework.Assert.IsTrue(etp.First());
			NUnit.Framework.Assert.IsTrue(etp.Eof());
			AssertEquals(ObjectId.ZeroId, ObjectId.FromRaw(etp.IdBuffer()));
			etp.Next(1);
			NUnit.Framework.Assert.IsTrue(etp.First());
			NUnit.Framework.Assert.IsTrue(etp.Eof());
			AssertEquals(ObjectId.ZeroId, ObjectId.FromRaw(etp.IdBuffer()));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBackDoesNothing()
		{
			EmptyTreeIterator etp = new EmptyTreeIterator();
			etp.Back(1);
			NUnit.Framework.Assert.IsTrue(etp.First());
			NUnit.Framework.Assert.IsTrue(etp.Eof());
			AssertEquals(ObjectId.ZeroId, ObjectId.FromRaw(etp.IdBuffer()));
			etp.Back(1);
			NUnit.Framework.Assert.IsTrue(etp.First());
			NUnit.Framework.Assert.IsTrue(etp.Eof());
			AssertEquals(ObjectId.ZeroId, ObjectId.FromRaw(etp.IdBuffer()));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStopWalkCallsParent()
		{
			bool[] called = new bool[1];
			NUnit.Framework.Assert.IsFalse(called[0]);
			EmptyTreeIterator parent = new _EmptyTreeIterator_105(called);
			ObjectReader reader = db.NewObjectReader();
			parent.CreateSubtreeIterator(reader).StopWalk();
			NUnit.Framework.Assert.IsTrue(called[0]);
		}

		private sealed class _EmptyTreeIterator_105 : EmptyTreeIterator
		{
			public _EmptyTreeIterator_105(bool[] called)
			{
				this.called = called;
			}

			public override void StopWalk()
			{
				called[0] = true;
			}

			private readonly bool[] called;
		}
	}
}
