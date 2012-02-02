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
using NGit.Errors;
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class RepositoryResolveTest : SampleDataRepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestObjectId_existing()
		{
			NUnit.Framework.Assert.AreEqual("49322bb17d3acc9146f98c97d078513228bbf3c0", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestObjectId_nonexisting()
		{
			NUnit.Framework.Assert.AreEqual("49322bb17d3acc9146f98c97d078513228bbf3c1", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c1").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestObjectId_objectid_implicit_firstparent()
		{
			NUnit.Framework.Assert.AreEqual("6e1475206e57110fcef4b92320436c1e9872a322", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^").Name);
			NUnit.Framework.Assert.AreEqual("1203b03dc816ccbb67773f28b3c19318654b0bc8", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^^").Name);
			NUnit.Framework.Assert.AreEqual("bab66b48f836ed950c99134ef666436fb07a09a0", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^^^").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestObjectId_objectid_self()
		{
			NUnit.Framework.Assert.AreEqual("49322bb17d3acc9146f98c97d078513228bbf3c0", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^0").Name);
			NUnit.Framework.Assert.AreEqual("49322bb17d3acc9146f98c97d078513228bbf3c0", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^0^0").Name);
			NUnit.Framework.Assert.AreEqual("49322bb17d3acc9146f98c97d078513228bbf3c0", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^0^0^0").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestObjectId_objectid_explicit_firstparent()
		{
			NUnit.Framework.Assert.AreEqual("6e1475206e57110fcef4b92320436c1e9872a322", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^1").Name);
			NUnit.Framework.Assert.AreEqual("1203b03dc816ccbb67773f28b3c19318654b0bc8", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^1^1").Name);
			NUnit.Framework.Assert.AreEqual("bab66b48f836ed950c99134ef666436fb07a09a0", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^1^1^1").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestObjectId_objectid_explicit_otherparents()
		{
			NUnit.Framework.Assert.AreEqual("6e1475206e57110fcef4b92320436c1e9872a322", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^1").Name);
			NUnit.Framework.Assert.AreEqual("f73b95671f326616d66b2afb3bdfcdbbce110b44", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^2").Name);
			NUnit.Framework.Assert.AreEqual("d0114ab8ac326bab30e3a657a0397578c5a1af88", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^3").Name);
			NUnit.Framework.Assert.AreEqual("d0114ab8ac326bab30e3a657a0397578c5a1af88", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^03").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRef_refname()
		{
			NUnit.Framework.Assert.AreEqual("49322bb17d3acc9146f98c97d078513228bbf3c0", db.Resolve
				("master^0").Name);
			NUnit.Framework.Assert.AreEqual("6e1475206e57110fcef4b92320436c1e9872a322", db.Resolve
				("master^").Name);
			NUnit.Framework.Assert.AreEqual("6e1475206e57110fcef4b92320436c1e9872a322", db.Resolve
				("refs/heads/master^1").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDistance()
		{
			NUnit.Framework.Assert.AreEqual("49322bb17d3acc9146f98c97d078513228bbf3c0", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0~0").Name);
			NUnit.Framework.Assert.AreEqual("6e1475206e57110fcef4b92320436c1e9872a322", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0~1").Name);
			NUnit.Framework.Assert.AreEqual("1203b03dc816ccbb67773f28b3c19318654b0bc8", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0~2").Name);
			NUnit.Framework.Assert.AreEqual("bab66b48f836ed950c99134ef666436fb07a09a0", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0~3").Name);
			NUnit.Framework.Assert.AreEqual("bab66b48f836ed950c99134ef666436fb07a09a0", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0~03").Name);
			NUnit.Framework.Assert.AreEqual("6e1475206e57110fcef4b92320436c1e9872a322", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0~").Name);
			NUnit.Framework.Assert.AreEqual("1203b03dc816ccbb67773f28b3c19318654b0bc8", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0~~").Name);
			NUnit.Framework.Assert.AreEqual("bab66b48f836ed950c99134ef666436fb07a09a0", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0~~~").Name);
			NUnit.Framework.Assert.AreEqual("1203b03dc816ccbb67773f28b3c19318654b0bc8", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0~~1").Name);
			NUnit.Framework.Assert.AreEqual("1203b03dc816ccbb67773f28b3c19318654b0bc8", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0~~~0").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTree()
		{
			NUnit.Framework.Assert.AreEqual("6020a3b8d5d636e549ccbd0c53e2764684bb3125", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^{tree}").Name);
			NUnit.Framework.Assert.AreEqual("02ba32d3649e510002c21651936b7077aa75ffa9", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^^{tree}").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestHEAD()
		{
			NUnit.Framework.Assert.AreEqual("6020a3b8d5d636e549ccbd0c53e2764684bb3125", db.Resolve
				("HEAD^{tree}").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDerefCommit()
		{
			NUnit.Framework.Assert.AreEqual("49322bb17d3acc9146f98c97d078513228bbf3c0", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^{}").Name);
			NUnit.Framework.Assert.AreEqual("49322bb17d3acc9146f98c97d078513228bbf3c0", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^{commit}").Name);
			// double deref
			NUnit.Framework.Assert.AreEqual("6020a3b8d5d636e549ccbd0c53e2764684bb3125", db.Resolve
				("49322bb17d3acc9146f98c97d078513228bbf3c0^{commit}^{tree}").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDerefTag()
		{
			NUnit.Framework.Assert.AreEqual("17768080a2318cd89bba4c8b87834401e2095703", db.Resolve
				("refs/tags/B").Name);
			NUnit.Framework.Assert.AreEqual("d86a2aada2f5e7ccf6f11880bfb9ab404e8a8864", db.Resolve
				("refs/tags/B^{commit}").Name);
			NUnit.Framework.Assert.AreEqual("032c063ce34486359e3ee3d4f9e5c225b9e1a4c2", db.Resolve
				("refs/tags/B10th").Name);
			NUnit.Framework.Assert.AreEqual("d86a2aada2f5e7ccf6f11880bfb9ab404e8a8864", db.Resolve
				("refs/tags/B10th^{commit}").Name);
			NUnit.Framework.Assert.AreEqual("d86a2aada2f5e7ccf6f11880bfb9ab404e8a8864", db.Resolve
				("refs/tags/B10th^{}").Name);
			NUnit.Framework.Assert.AreEqual("d86a2aada2f5e7ccf6f11880bfb9ab404e8a8864", db.Resolve
				("refs/tags/B10th^0").Name);
			NUnit.Framework.Assert.AreEqual("d86a2aada2f5e7ccf6f11880bfb9ab404e8a8864", db.Resolve
				("refs/tags/B10th~0").Name);
			NUnit.Framework.Assert.AreEqual("0966a434eb1a025db6b71485ab63a3bfbea520b6", db.Resolve
				("refs/tags/B10th^").Name);
			NUnit.Framework.Assert.AreEqual("0966a434eb1a025db6b71485ab63a3bfbea520b6", db.Resolve
				("refs/tags/B10th^1").Name);
			NUnit.Framework.Assert.AreEqual("0966a434eb1a025db6b71485ab63a3bfbea520b6", db.Resolve
				("refs/tags/B10th~1").Name);
			NUnit.Framework.Assert.AreEqual("2c349335b7f797072cf729c4f3bb0914ecb6dec9", db.Resolve
				("refs/tags/B10th~2").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDerefBlob()
		{
			NUnit.Framework.Assert.AreEqual("fd608fbe625a2b456d9f15c2b1dc41f252057dd7", db.Resolve
				("spearce-gpg-pub^{}").Name);
			NUnit.Framework.Assert.AreEqual("fd608fbe625a2b456d9f15c2b1dc41f252057dd7", db.Resolve
				("spearce-gpg-pub^{blob}").Name);
			NUnit.Framework.Assert.AreEqual("fd608fbe625a2b456d9f15c2b1dc41f252057dd7", db.Resolve
				("fd608fbe625a2b456d9f15c2b1dc41f252057dd7^{}").Name);
			NUnit.Framework.Assert.AreEqual("fd608fbe625a2b456d9f15c2b1dc41f252057dd7", db.Resolve
				("fd608fbe625a2b456d9f15c2b1dc41f252057dd7^{blob}").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDerefTree()
		{
			NUnit.Framework.Assert.AreEqual("032c063ce34486359e3ee3d4f9e5c225b9e1a4c2", db.Resolve
				("refs/tags/B10th").Name);
			NUnit.Framework.Assert.AreEqual("856ec208ae6cadac25a6d74f19b12bb27a24fe24", db.Resolve
				("032c063ce34486359e3ee3d4f9e5c225b9e1a4c2^{tree}").Name);
			NUnit.Framework.Assert.AreEqual("856ec208ae6cadac25a6d74f19b12bb27a24fe24", db.Resolve
				("refs/tags/B10th^{tree}").Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParseGitDescribeOutput()
		{
			ObjectId exp = db.Resolve("b");
			NUnit.Framework.Assert.AreEqual(exp, db.Resolve("B-g7f82283"));
			// old style
			NUnit.Framework.Assert.AreEqual(exp, db.Resolve("B-6-g7f82283"));
			// new style
			NUnit.Framework.Assert.AreEqual(exp, db.Resolve("B-6-g7f82283^0"));
			NUnit.Framework.Assert.AreEqual(exp, db.Resolve("B-6-g7f82283^{commit}"));
			try
			{
				db.Resolve("B-6-g7f82283^{blob}");
				NUnit.Framework.Assert.Fail("expected IncorrectObjectTypeException");
			}
			catch (IncorrectObjectTypeException)
			{
			}
			// Expected
			NUnit.Framework.Assert.AreEqual(db.Resolve("b^1"), db.Resolve("B-6-g7f82283^1"));
			NUnit.Framework.Assert.AreEqual(db.Resolve("b~2"), db.Resolve("B-6-g7f82283~2"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParseNonGitDescribe()
		{
			ObjectId id = Id("49322bb17d3acc9146f98c97d078513228bbf3c0");
			RefUpdate ru = db.UpdateRef("refs/heads/foo-g032c");
			ru.SetNewObjectId(id);
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, ru.Update());
			NUnit.Framework.Assert.AreEqual(id, db.Resolve("refs/heads/foo-g032c"));
			NUnit.Framework.Assert.AreEqual(id, db.Resolve("foo-g032c"));
			NUnit.Framework.Assert.IsNull(db.Resolve("foo-g032"));
			NUnit.Framework.Assert.IsNull(db.Resolve("foo-g03"));
			NUnit.Framework.Assert.IsNull(db.Resolve("foo-g0"));
			NUnit.Framework.Assert.IsNull(db.Resolve("foo-g"));
			ru = db.UpdateRef("refs/heads/foo-g032c-dev");
			ru.SetNewObjectId(id);
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, ru.Update());
			NUnit.Framework.Assert.AreEqual(id, db.Resolve("refs/heads/foo-g032c-dev"));
			NUnit.Framework.Assert.AreEqual(id, db.Resolve("foo-g032c-dev"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParseLookupPath()
		{
			ObjectId b2_txt = Id("10da5895682013006950e7da534b705252b03be6");
			ObjectId b3_b2_txt = Id("e6bfff5c1d0f0ecd501552b43a1e13d8008abc31");
			ObjectId b_root = Id("acd0220f06f7e4db50ea5ba242f0dfed297b27af");
			ObjectId master_txt = Id("82b1d08466e9505f8666b778744f9a3471a70c81");
			NUnit.Framework.Assert.AreEqual(b2_txt, db.Resolve("b:b/b2.txt"));
			NUnit.Framework.Assert.AreEqual(b_root, db.Resolve("b:"));
			NUnit.Framework.Assert.AreEqual(Id("6020a3b8d5d636e549ccbd0c53e2764684bb3125"), db
				.Resolve("master:"));
			NUnit.Framework.Assert.AreEqual(Id("10da5895682013006950e7da534b705252b03be6"), db
				.Resolve("master:b/b2.txt"));
			NUnit.Framework.Assert.AreEqual(master_txt, db.Resolve(":master.txt"));
			NUnit.Framework.Assert.AreEqual(b3_b2_txt, db.Resolve("b~3:b/b2.txt"));
			NUnit.Framework.Assert.IsNull(db.Resolve("b:FOO"), "no FOO");
			NUnit.Framework.Assert.IsNull(db.Resolve("b:b/FOO"), "no b/FOO");
			NUnit.Framework.Assert.IsNull(db.Resolve(":b/FOO"), "no b/FOO");
			NUnit.Framework.Assert.IsNull(db.Resolve("not-a-branch:"), "no not-a-branch:");
		}

		private static ObjectId Id(string name)
		{
			return ObjectId.FromString(name);
		}
	}
}
