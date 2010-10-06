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
	public class ObjectIdRefTest
	{
		private static readonly ObjectId ID_A = ObjectId.FromString("41eb0d88f833b558bddeb269b7ab77399cdf98ed"
			);

		private static readonly ObjectId ID_B = ObjectId.FromString("698dd0b8d0c299f080559a1cffc7fe029479a408"
			);

		private static readonly string name = "refs/heads/a.test.ref";

		[NUnit.Framework.Test]
		public virtual void TestConstructor_PeeledStatusNotKnown()
		{
			ObjectIdRef r;
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, name, ID_A);
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, r.GetStorage());
			NUnit.Framework.Assert.AreSame(name, r.GetName());
			NUnit.Framework.Assert.AreSame(ID_A, r.GetObjectId());
			NUnit.Framework.Assert.IsFalse(r.IsPeeled(), "not peeled");
			NUnit.Framework.Assert.IsNull(r.GetPeeledObjectId(), "no peel id");
			NUnit.Framework.Assert.AreSame(r, r.GetLeaf(), "leaf is this");
			NUnit.Framework.Assert.AreSame(r, r.GetTarget(), "target is this");
			NUnit.Framework.Assert.IsFalse(r.IsSymbolic(), "not symbolic");
			r = new ObjectIdRef.Unpeeled(RefStorage.PACKED, name, ID_A);
			NUnit.Framework.Assert.AreEqual(RefStorage.PACKED, r.GetStorage());
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE_PACKED, name, ID_A);
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE_PACKED, r.GetStorage());
			r = new ObjectIdRef.Unpeeled(RefStorage.NEW, name, null);
			NUnit.Framework.Assert.AreEqual(RefStorage.NEW, r.GetStorage());
			NUnit.Framework.Assert.AreSame(name, r.GetName());
			NUnit.Framework.Assert.IsNull(r.GetObjectId(), "no id on new ref");
			NUnit.Framework.Assert.IsFalse(r.IsPeeled(), "not peeled");
			NUnit.Framework.Assert.IsNull(r.GetPeeledObjectId(), "no peel id");
			NUnit.Framework.Assert.AreSame(r, r.GetLeaf(), "leaf is this");
			NUnit.Framework.Assert.AreSame(r, r.GetTarget(), "target is this");
			NUnit.Framework.Assert.IsFalse(r.IsSymbolic(), "not symbolic");
		}

		[NUnit.Framework.Test]
		public virtual void TestConstructor_Peeled()
		{
			ObjectIdRef r;
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, name, ID_A);
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, r.GetStorage());
			NUnit.Framework.Assert.AreSame(name, r.GetName());
			NUnit.Framework.Assert.AreSame(ID_A, r.GetObjectId());
			NUnit.Framework.Assert.IsFalse(r.IsPeeled(), "not peeled");
			NUnit.Framework.Assert.IsNull(r.GetPeeledObjectId(), "no peel id");
			NUnit.Framework.Assert.AreSame(r, r.GetLeaf(), "leaf is this");
			NUnit.Framework.Assert.AreSame(r, r.GetTarget(), "target is this");
			NUnit.Framework.Assert.IsFalse(r.IsSymbolic(), "not symbolic");
			r = new ObjectIdRef.PeeledNonTag(RefStorage.LOOSE, name, ID_A);
			NUnit.Framework.Assert.IsTrue(r.IsPeeled(), "is peeled");
			NUnit.Framework.Assert.IsNull(r.GetPeeledObjectId(), "no peel id");
			r = new ObjectIdRef.PeeledTag(RefStorage.LOOSE, name, ID_A, ID_B);
			NUnit.Framework.Assert.IsTrue(r.IsPeeled(), "is peeled");
			NUnit.Framework.Assert.AreSame(ID_B, r.GetPeeledObjectId());
		}

		[NUnit.Framework.Test]
		public virtual void TestToString()
		{
			ObjectIdRef r;
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, name, ID_A);
			NUnit.Framework.Assert.AreEqual("Ref[" + name + "=" + ID_A.Name + "]", r.ToString
				());
		}
	}
}
