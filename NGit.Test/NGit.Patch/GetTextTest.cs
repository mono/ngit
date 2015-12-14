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

using System.Collections.Generic;
using System.Text;
using NGit.Junit;
using NGit.Patch;
using NGit.Test.NGit.Util.IO;
using Sharpen;

namespace NGit.Patch
{
	[NUnit.Framework.TestFixture]
	public class GetTextTest
	{
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetText_BothISO88591()
		{
			Encoding cs = Sharpen.Extensions.GetEncoding("ISO-8859-1");
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			FileHeader fh = p.GetFiles()[0];
			NUnit.Framework.Assert.AreEqual(2, fh.GetHunks().Count);
			NUnit.Framework.Assert.AreEqual(ReadTestPatchFile(cs), fh.GetScriptText(cs, cs));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetText_NoBinary()
		{
			Encoding cs = Sharpen.Extensions.GetEncoding("ISO-8859-1");
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			FileHeader fh = p.GetFiles()[0];
			NUnit.Framework.Assert.AreEqual(0, fh.GetHunks().Count);
			NUnit.Framework.Assert.AreEqual(ReadTestPatchFile(cs), fh.GetScriptText(cs, cs));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("Test relies on java regex and java character escape codes")]
		public virtual void TestGetText_Convert()
		{
			Encoding csOld = Sharpen.Extensions.GetEncoding("ISO-8859-1");
			Encoding csNew = Sharpen.Extensions.GetEncoding("UTF-8");
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			FileHeader fh = p.GetFiles()[0];
			NUnit.Framework.Assert.AreEqual(2, fh.GetHunks().Count);
			// Read the original file as ISO-8859-1 and fix up the one place
			// where we changed the character encoding. That makes the exp
			// string match what we really expect to get back.
			//
			string exp = ReadTestPatchFile(csOld);
			exp = exp.Replace("\x12f\xcdngstr\x12f\x10am", "\u00c5ngstr\u00f6m");
			NUnit.Framework.Assert.AreEqual(exp, fh.GetScriptText(csOld, csNew));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("Test relies on java regex and java character escape codes")]
		public virtual void TestGetText_DiffCc()
		{
			Encoding csOld = Sharpen.Extensions.GetEncoding("ISO-8859-1");
			Encoding csNew = Sharpen.Extensions.GetEncoding("UTF-8");
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			CombinedFileHeader fh = (CombinedFileHeader)p.GetFiles()[0];
			NUnit.Framework.Assert.AreEqual(1, fh.GetHunks().Count
				);
			// Read the original file as ISO-8859-1 and fix up the one place
			// where we changed the character encoding. That makes the exp
			// string match what we really expect to get back.
			//
			string exp = ReadTestPatchFile(csOld);
			exp = exp.Replace("\x12f\xcdngstr\x12f\x10am", "\u00c5ngstr\u00f6m");
			NUnit.Framework.Assert.AreEqual(exp, fh.GetScriptText(new Encoding[] { csNew, csOld
				, csNew }));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private NGit.Patch.Patch ParseTestPatchFile()
		{
			string patchFile = Sharpen.Extensions.GetTestName() + ".patch";
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

		/// <exception cref="System.IO.IOException"></exception>
		private string ReadTestPatchFile(Encoding cs)
		{
			string patchFile = Sharpen.Extensions.GetTestName() + ".patch";
			InputStream @in = GetType().GetResourceAsStream(patchFile);
			if (@in == null)
			{
				NUnit.Framework.Assert.Fail("No " + patchFile + " test vector");
				return null;
			}
			// Never happens
			try
			{
				InputStreamReader r = new InputStreamReader(@in, cs);
				char[] tmp = new char[2048];
				StringBuilder s = new StringBuilder();
				int n;
				while ((n = r.Read(tmp)) > 0)
				{
					s.Append(tmp, 0, n);
				}
				return s.ToString();
			}
			finally
			{
				@in.Close();
			}
		}
	}
}
