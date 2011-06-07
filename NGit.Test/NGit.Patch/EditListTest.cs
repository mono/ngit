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
using NGit.Patch;
using Sharpen;

namespace NGit.Patch
{
	[NUnit.Framework.TestFixture]
	public class EditListTest
	{
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestHunkHeader()
		{
			NGit.Patch.Patch p = ParseTestPatchFile("testGetText_BothISO88591.patch");
			FileHeader fh = p.GetFiles()[0];
			EditList list0 = fh.GetHunks()[0].ToEditList();
			NUnit.Framework.Assert.AreEqual(1, list0.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(4 - 1, 5 - 1, 4 - 1, 5 - 1), list0[0]);
			EditList list1 = fh.GetHunks()[1].ToEditList();
			NUnit.Framework.Assert.AreEqual(1, list1.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(16 - 1, 17 - 1, 16 - 1, 17 - 1), list1[0
				]);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileHeader()
		{
			NGit.Patch.Patch p = ParseTestPatchFile("testGetText_BothISO88591.patch");
			FileHeader fh = p.GetFiles()[0];
			EditList e = fh.ToEditList();
			NUnit.Framework.Assert.AreEqual(2, e.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(4 - 1, 5 - 1, 4 - 1, 5 - 1), e[0]);
			NUnit.Framework.Assert.AreEqual(new Edit(16 - 1, 17 - 1, 16 - 1, 17 - 1), e[1]);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTypes()
		{
			NGit.Patch.Patch p = ParseTestPatchFile("testEditList_Types.patch");
			FileHeader fh = p.GetFiles()[0];
			EditList e = fh.ToEditList();
			NUnit.Framework.Assert.AreEqual(3, e.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(3 - 1, 3 - 1, 3 - 1, 4 - 1), e[0]);
			NUnit.Framework.Assert.AreEqual(new Edit(17 - 1, 19 - 1, 18 - 1, 18 - 1), e[1]);
			NUnit.Framework.Assert.AreEqual(new Edit(23 - 1, 25 - 1, 22 - 1, 28 - 1), e[2]);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private NGit.Patch.Patch ParseTestPatchFile(string patchFile)
		{
			InputStream @in = GetType().GetResourceAsStream(patchFile);
			if (@in == null)
			{
				NUnit.Framework.Assert.Fail("No " + patchFile + " test vector");
				return null;
			}
			// Never happens
			try
			{
				NGit.Patch.Patch p = new NGit.Patch.Patch();
				p.Parse(@in);
				return p;
			}
			finally
			{
				@in.Close();
			}
		}
	}
}
