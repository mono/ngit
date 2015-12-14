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

using NGit.Internal;
using NGit.Junit;
using NGit.Patch;
using NGit.Test.NGit.Util.IO;
using Sharpen;

namespace NGit.Patch
{
	[NUnit.Framework.TestFixture]
	public class PatchCcErrorTest
	{
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestError_CcTruncatedOld()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.AreEqual(3, p.GetErrors().Count);
			{
				FormatError e = p.GetErrors()[0];
				NUnit.Framework.Assert.AreEqual(FormatError.Severity.ERROR, e.GetSeverity());
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().truncatedHunkLinesMissingForAncestor
					, Sharpen.Extensions.ValueOf(1), Sharpen.Extensions.ValueOf(1)), e.GetMessage());
				NUnit.Framework.Assert.AreEqual(346, e.GetOffset());
				NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@@ -55,12 -163,13 +163,15 @@@ public "
					));
			}
			{
				FormatError e = p.GetErrors()[1];
				NUnit.Framework.Assert.AreEqual(FormatError.Severity.ERROR, e.GetSeverity());
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().truncatedHunkLinesMissingForAncestor
					, Sharpen.Extensions.ValueOf(2), Sharpen.Extensions.ValueOf(2)), e.GetMessage());
				NUnit.Framework.Assert.AreEqual(346, e.GetOffset());
				NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@@ -55,12 -163,13 +163,15 @@@ public "
					));
			}
			{
				FormatError e = p.GetErrors()[2];
				NUnit.Framework.Assert.AreEqual(FormatError.Severity.ERROR, e.GetSeverity());
				NUnit.Framework.Assert.AreEqual("Truncated hunk, at least 3 new lines is missing"
					, e.GetMessage());
				NUnit.Framework.Assert.AreEqual(346, e.GetOffset());
				NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@@ -55,12 -163,13 +163,15 @@@ public "
					));
			}
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
	}
}
