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

using NGit.Patch;
using Sharpen;

namespace NGit.Patch
{
	[NUnit.Framework.TestFixture]
	public class PatchErrorTest
	{
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestError_DisconnectedHunk()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			{
				FileHeader fh = p.GetFiles()[0];
				NUnit.Framework.Assert.AreEqual("org.eclipse.jgit/src/org/spearce/jgit/lib/RepositoryConfig.java"
					, fh.GetNewPath());
				NUnit.Framework.Assert.AreEqual(1, fh.GetHunks().Count);
			}
			NUnit.Framework.Assert.AreEqual(1, p.GetErrors().Count);
			FormatError e = p.GetErrors()[0];
			NUnit.Framework.Assert.AreEqual(FormatError.Severity.ERROR, e.GetSeverity());
			NUnit.Framework.Assert.AreEqual("Hunk disconnected from file", e.GetMessage());
			NUnit.Framework.Assert.AreEqual(18, e.GetOffset());
			NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@ -109,4 +109,11 @@ assert"
				));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestError_TruncatedOld()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.AreEqual(1, p.GetErrors().Count);
			FormatError e = p.GetErrors()[0];
			NUnit.Framework.Assert.AreEqual(FormatError.Severity.ERROR, e.GetSeverity());
			NUnit.Framework.Assert.AreEqual("Truncated hunk, at least 1 old lines is missing"
				, e.GetMessage());
			NUnit.Framework.Assert.AreEqual(313, e.GetOffset());
			NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@ -236,9 +236,9 @@ protected "
				));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestError_TruncatedNew()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.AreEqual(1, p.GetErrors().Count);
			FormatError e = p.GetErrors()[0];
			NUnit.Framework.Assert.AreEqual(FormatError.Severity.ERROR, e.GetSeverity());
			NUnit.Framework.Assert.AreEqual("Truncated hunk, at least 1 new lines is missing"
				, e.GetMessage());
			NUnit.Framework.Assert.AreEqual(313, e.GetOffset());
			NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@ -236,9 +236,9 @@ protected "
				));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestError_BodyTooLong()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.AreEqual(1, p.GetErrors().Count);
			FormatError e = p.GetErrors()[0];
			NUnit.Framework.Assert.AreEqual(FormatError.Severity.WARNING, e.GetSeverity());
			NUnit.Framework.Assert.AreEqual("Hunk header 4:11 does not match body line count of 4:12"
				, e.GetMessage());
			NUnit.Framework.Assert.AreEqual(349, e.GetOffset());
			NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@ -109,4 +109,11 @@ assert"
				));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestError_GarbageBetweenFiles()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(2, p.GetFiles().Count);
			{
				FileHeader fh = p.GetFiles()[0];
				NUnit.Framework.Assert.AreEqual("org.eclipse.jgit.test/tst/org/spearce/jgit/lib/RepositoryConfigTest.java"
					, fh.GetNewPath());
				NUnit.Framework.Assert.AreEqual(1, fh.GetHunks().Count);
			}
			{
				FileHeader fh = p.GetFiles()[1];
				NUnit.Framework.Assert.AreEqual("org.eclipse.jgit/src/org/spearce/jgit/lib/RepositoryConfig.java"
					, fh.GetNewPath());
				NUnit.Framework.Assert.AreEqual(1, fh.GetHunks().Count);
			}
			NUnit.Framework.Assert.AreEqual(1, p.GetErrors().Count);
			FormatError e = p.GetErrors()[0];
			NUnit.Framework.Assert.AreEqual(FormatError.Severity.WARNING, e.GetSeverity());
			NUnit.Framework.Assert.AreEqual("Unexpected hunk trailer", e.GetMessage());
			NUnit.Framework.Assert.AreEqual(926, e.GetOffset());
			NUnit.Framework.Assert.AreEqual("I AM NOT HERE\n", e.GetLineText());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestError_GitBinaryNoForwardHunk()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(2, p.GetFiles().Count);
			{
				FileHeader fh = p.GetFiles()[0];
				NUnit.Framework.Assert.AreEqual("org.spearce.egit.ui/icons/toolbar/fetchd.png", fh
					.GetNewPath());
				NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.GIT_BINARY, fh.GetPatchType(
					));
				NUnit.Framework.Assert.IsTrue(fh.GetHunks().IsEmpty());
				NUnit.Framework.Assert.IsNull(fh.GetForwardBinaryHunk());
			}
			{
				FileHeader fh = p.GetFiles()[1];
				NUnit.Framework.Assert.AreEqual("org.spearce.egit.ui/icons/toolbar/fetche.png", fh
					.GetNewPath());
				NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, fh.GetPatchType());
				NUnit.Framework.Assert.IsTrue(fh.GetHunks().IsEmpty());
				NUnit.Framework.Assert.IsNull(fh.GetForwardBinaryHunk());
			}
			NUnit.Framework.Assert.AreEqual(1, p.GetErrors().Count);
			FormatError e = p.GetErrors()[0];
			NUnit.Framework.Assert.AreEqual(FormatError.Severity.ERROR, e.GetSeverity());
			NUnit.Framework.Assert.AreEqual("Missing forward-image in GIT binary patch", e.GetMessage
				());
			NUnit.Framework.Assert.AreEqual(297, e.GetOffset());
			NUnit.Framework.Assert.AreEqual("\n", e.GetLineText());
		}

		/// <exception cref="System.IO.IOException"></exception>
		private NGit.Patch.Patch ParseTestPatchFile()
		{
			string patchFile = Sharpen.Extensions.GetTestName(this) + ".patch";
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
