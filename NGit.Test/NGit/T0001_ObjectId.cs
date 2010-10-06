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
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	public class T0001_ObjectId : TestCase
	{
		public virtual void Test001_toString()
		{
			string x = "def4c620bc3713bb1bb26b808ec9312548e73946";
			ObjectId oid = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(x, oid.Name);
		}

		public virtual void Test002_toString()
		{
			string x = "ff00eedd003713bb1bb26b808ec9312548e73946";
			ObjectId oid = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(x, oid.Name);
		}

		public virtual void Test003_equals()
		{
			string x = "def4c620bc3713bb1bb26b808ec9312548e73946";
			ObjectId a = ObjectId.FromString(x);
			ObjectId b = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
			NUnit.Framework.Assert.IsTrue("a and b are same", a.Equals(b));
		}

		public virtual void Test004_isId()
		{
			NUnit.Framework.Assert.IsTrue("valid id", ObjectId.IsId("def4c620bc3713bb1bb26b808ec9312548e73946"
				));
		}

		public virtual void Test005_notIsId()
		{
			NUnit.Framework.Assert.IsFalse("bob is not an id", ObjectId.IsId("bob"));
		}

		public virtual void Test006_notIsId()
		{
			NUnit.Framework.Assert.IsFalse("39 digits is not an id", ObjectId.IsId("def4c620bc3713bb1bb26b808ec9312548e7394"
				));
		}

		public virtual void Test007_isId()
		{
			NUnit.Framework.Assert.IsTrue("uppercase is accepted", ObjectId.IsId("Def4c620bc3713bb1bb26b808ec9312548e73946"
				));
		}

		public virtual void Test008_notIsId()
		{
			NUnit.Framework.Assert.IsFalse("g is not a valid hex digit", ObjectId.IsId("gef4c620bc3713bb1bb26b808ec9312548e73946"
				));
		}

		public virtual void Test009_toString()
		{
			string x = "ff00eedd003713bb1bb26b808ec9312548e73946";
			ObjectId oid = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(x, ObjectId.ToString(oid));
		}

		public virtual void Test010_toString()
		{
			string x = "0000000000000000000000000000000000000000";
			NUnit.Framework.Assert.AreEqual(x, ObjectId.ToString(null));
		}

		public virtual void Test011_toString()
		{
			string x = "0123456789ABCDEFabcdef1234567890abcdefAB";
			ObjectId oid = ObjectId.FromString(x);
			NUnit.Framework.Assert.AreEqual(x.ToLower(), oid.Name);
		}
	}
}
