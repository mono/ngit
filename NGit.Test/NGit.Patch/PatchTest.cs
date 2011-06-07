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
using NGit.Diff;
using NGit.Junit;
using NGit.Patch;
using Sharpen;

namespace NGit.Patch
{
	[NUnit.Framework.TestFixture]
	public class PatchTest
	{
		[NUnit.Framework.Test]
		public virtual void TestEmpty()
		{
			NGit.Patch.Patch p = new NGit.Patch.Patch();
			NUnit.Framework.Assert.IsTrue(p.GetFiles().IsEmpty());
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_ConfigCaseInsensitive()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(2, p.GetFiles().Count);
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			FileHeader fRepositoryConfigTest = p.GetFiles()[0];
			FileHeader fRepositoryConfig = p.GetFiles()[1];
			NUnit.Framework.Assert.AreEqual("org.eclipse.jgit.test/tst/org/spearce/jgit/lib/RepositoryConfigTest.java"
				, fRepositoryConfigTest.GetNewPath());
			NUnit.Framework.Assert.AreEqual("org.eclipse.jgit/src/org/spearce/jgit/lib/RepositoryConfig.java"
				, fRepositoryConfig.GetNewPath());
			NUnit.Framework.Assert.AreEqual(572, fRepositoryConfigTest.startOffset);
			NUnit.Framework.Assert.AreEqual(1490, fRepositoryConfig.startOffset);
			NUnit.Framework.Assert.AreEqual("da7e704", fRepositoryConfigTest.GetOldId().Name);
			NUnit.Framework.Assert.AreEqual("34ce04a", fRepositoryConfigTest.GetNewId().Name);
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, fRepositoryConfigTest
				.GetPatchType());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fRepositoryConfigTest.GetOldMode
				());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fRepositoryConfigTest.GetNewMode
				());
			NUnit.Framework.Assert.AreEqual(1, fRepositoryConfigTest.GetHunks().Count);
			{
				HunkHeader h = fRepositoryConfigTest.GetHunks()[0];
				NUnit.Framework.Assert.AreSame(fRepositoryConfigTest, h.GetFileHeader());
				NUnit.Framework.Assert.AreEqual(921, h.startOffset);
				NUnit.Framework.Assert.AreEqual(109, h.GetOldImage().GetStartLine());
				NUnit.Framework.Assert.AreEqual(4, h.GetOldImage().GetLineCount());
				NUnit.Framework.Assert.AreEqual(109, h.GetNewStartLine());
				NUnit.Framework.Assert.AreEqual(11, h.GetNewLineCount());
				NUnit.Framework.Assert.AreEqual(4, h.GetLinesContext());
				NUnit.Framework.Assert.AreEqual(7, h.GetOldImage().GetLinesAdded());
				NUnit.Framework.Assert.AreEqual(0, h.GetOldImage().GetLinesDeleted());
				NUnit.Framework.Assert.AreSame(fRepositoryConfigTest.GetOldId(), h.GetOldImage().
					GetId());
				NUnit.Framework.Assert.AreEqual(1490, h.endOffset);
			}
			NUnit.Framework.Assert.AreEqual("45c2f8a", fRepositoryConfig.GetOldId().Name);
			NUnit.Framework.Assert.AreEqual("3291bba", fRepositoryConfig.GetNewId().Name);
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, fRepositoryConfig.GetPatchType
				());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fRepositoryConfig.GetOldMode
				());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fRepositoryConfig.GetNewMode
				());
			NUnit.Framework.Assert.AreEqual(3, fRepositoryConfig.GetHunks().Count);
			{
				HunkHeader h = fRepositoryConfig.GetHunks()[0];
				NUnit.Framework.Assert.AreSame(fRepositoryConfig, h.GetFileHeader());
				NUnit.Framework.Assert.AreEqual(1803, h.startOffset);
				NUnit.Framework.Assert.AreEqual(236, h.GetOldImage().GetStartLine());
				NUnit.Framework.Assert.AreEqual(9, h.GetOldImage().GetLineCount());
				NUnit.Framework.Assert.AreEqual(236, h.GetNewStartLine());
				NUnit.Framework.Assert.AreEqual(9, h.GetNewLineCount());
				NUnit.Framework.Assert.AreEqual(7, h.GetLinesContext());
				NUnit.Framework.Assert.AreEqual(2, h.GetOldImage().GetLinesAdded());
				NUnit.Framework.Assert.AreEqual(2, h.GetOldImage().GetLinesDeleted());
				NUnit.Framework.Assert.AreSame(fRepositoryConfig.GetOldId(), h.GetOldImage().GetId
					());
				NUnit.Framework.Assert.AreEqual(2434, h.endOffset);
			}
			{
				HunkHeader h = fRepositoryConfig.GetHunks()[1];
				NUnit.Framework.Assert.AreEqual(2434, h.startOffset);
				NUnit.Framework.Assert.AreEqual(300, h.GetOldImage().GetStartLine());
				NUnit.Framework.Assert.AreEqual(7, h.GetOldImage().GetLineCount());
				NUnit.Framework.Assert.AreEqual(300, h.GetNewStartLine());
				NUnit.Framework.Assert.AreEqual(7, h.GetNewLineCount());
				NUnit.Framework.Assert.AreEqual(6, h.GetLinesContext());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage().GetLinesAdded());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage().GetLinesDeleted());
				NUnit.Framework.Assert.AreEqual(2816, h.endOffset);
			}
			{
				HunkHeader h = fRepositoryConfig.GetHunks()[2];
				NUnit.Framework.Assert.AreEqual(2816, h.startOffset);
				NUnit.Framework.Assert.AreEqual(954, h.GetOldImage().GetStartLine());
				NUnit.Framework.Assert.AreEqual(7, h.GetOldImage().GetLineCount());
				NUnit.Framework.Assert.AreEqual(954, h.GetNewStartLine());
				NUnit.Framework.Assert.AreEqual(7, h.GetNewLineCount());
				NUnit.Framework.Assert.AreEqual(6, h.GetLinesContext());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage().GetLinesAdded());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage().GetLinesDeleted());
				NUnit.Framework.Assert.AreEqual(3035, h.endOffset);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_NoBinary()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(5, p.GetFiles().Count);
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			for (int i = 0; i < 4; i++)
			{
				FileHeader fh = p.GetFiles()[i];
				NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.ADD, fh.GetChangeType());
				NUnit.Framework.Assert.IsNotNull(fh.GetOldId());
				NUnit.Framework.Assert.IsNotNull(fh.GetNewId());
				NUnit.Framework.Assert.AreEqual("0000000", fh.GetOldId().Name);
				NUnit.Framework.Assert.AreSame(FileMode.MISSING, fh.GetOldMode());
				NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fh.GetNewMode());
				NUnit.Framework.Assert.IsTrue(fh.GetNewPath().StartsWith("org.spearce.egit.ui/icons/toolbar/"
					));
				NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.BINARY, fh.GetPatchType());
				NUnit.Framework.Assert.IsTrue(fh.GetHunks().IsEmpty());
				NUnit.Framework.Assert.IsTrue(fh.HasMetaDataChanges());
				NUnit.Framework.Assert.IsNull(fh.GetForwardBinaryHunk());
				NUnit.Framework.Assert.IsNull(fh.GetReverseBinaryHunk());
			}
			FileHeader fh_1 = p.GetFiles()[4];
			NUnit.Framework.Assert.AreEqual("org.spearce.egit.ui/plugin.xml", fh_1.GetNewPath
				());
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, fh_1.GetChangeType()
				);
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, fh_1.GetPatchType()
				);
			NUnit.Framework.Assert.IsFalse(fh_1.HasMetaDataChanges());
			NUnit.Framework.Assert.AreEqual("ee8a5a0", fh_1.GetNewId().Name);
			NUnit.Framework.Assert.IsNull(fh_1.GetForwardBinaryHunk());
			NUnit.Framework.Assert.IsNull(fh_1.GetReverseBinaryHunk());
			NUnit.Framework.Assert.AreEqual(1, fh_1.GetHunks().Count);
			NUnit.Framework.Assert.AreEqual(272, fh_1.GetHunks()[0].GetOldImage().GetStartLine
				());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_GitBinaryLiteral()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			int[] binsizes = new int[] { 359, 393, 372, 404 };
			NUnit.Framework.Assert.AreEqual(5, p.GetFiles().Count);
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			for (int i = 0; i < 4; i++)
			{
				FileHeader fh = p.GetFiles()[i];
				NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.ADD, fh.GetChangeType());
				NUnit.Framework.Assert.IsNotNull(fh.GetOldId());
				NUnit.Framework.Assert.IsNotNull(fh.GetNewId());
				NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId.Name, fh.GetOldId().Name);
				NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fh.GetNewMode());
				NUnit.Framework.Assert.IsTrue(fh.GetNewPath().StartsWith("org.spearce.egit.ui/icons/toolbar/"
					));
				NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.GIT_BINARY, fh.GetPatchType(
					));
				NUnit.Framework.Assert.IsTrue(fh.GetHunks().IsEmpty());
				NUnit.Framework.Assert.IsTrue(fh.HasMetaDataChanges());
				BinaryHunk fwd = fh.GetForwardBinaryHunk();
				BinaryHunk rev = fh.GetReverseBinaryHunk();
				NUnit.Framework.Assert.IsNotNull(fwd);
				NUnit.Framework.Assert.IsNotNull(rev);
				NUnit.Framework.Assert.AreEqual(binsizes[i], fwd.GetSize());
				NUnit.Framework.Assert.AreEqual(0, rev.GetSize());
				NUnit.Framework.Assert.AreSame(fh, fwd.GetFileHeader());
				NUnit.Framework.Assert.AreSame(fh, rev.GetFileHeader());
				NUnit.Framework.Assert.AreEqual(BinaryHunk.Type.LITERAL_DEFLATED, fwd.GetType());
				NUnit.Framework.Assert.AreEqual(BinaryHunk.Type.LITERAL_DEFLATED, rev.GetType());
			}
			FileHeader fh_1 = p.GetFiles()[4];
			NUnit.Framework.Assert.AreEqual("org.spearce.egit.ui/plugin.xml", fh_1.GetNewPath
				());
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, fh_1.GetChangeType()
				);
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, fh_1.GetPatchType()
				);
			NUnit.Framework.Assert.IsFalse(fh_1.HasMetaDataChanges());
			NUnit.Framework.Assert.AreEqual("ee8a5a0", fh_1.GetNewId().Name);
			NUnit.Framework.Assert.IsNull(fh_1.GetForwardBinaryHunk());
			NUnit.Framework.Assert.IsNull(fh_1.GetReverseBinaryHunk());
			NUnit.Framework.Assert.AreEqual(1, fh_1.GetHunks().Count);
			NUnit.Framework.Assert.AreEqual(272, fh_1.GetHunks()[0].GetOldImage().GetStartLine
				());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_GitBinaryDelta()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			FileHeader fh = p.GetFiles()[0];
			NUnit.Framework.Assert.IsTrue(fh.GetNewPath().StartsWith("zero.bin"));
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, fh.GetChangeType());
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.GIT_BINARY, fh.GetPatchType(
				));
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fh.GetNewMode());
			NUnit.Framework.Assert.IsNotNull(fh.GetOldId());
			NUnit.Framework.Assert.IsNotNull(fh.GetNewId());
			NUnit.Framework.Assert.AreEqual("08e7df176454f3ee5eeda13efa0adaa54828dfd8", fh.GetOldId
				().Name);
			NUnit.Framework.Assert.AreEqual("d70d8710b6d32ff844af0ee7c247e4b4b051867f", fh.GetNewId
				().Name);
			NUnit.Framework.Assert.IsTrue(fh.GetHunks().IsEmpty());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
			BinaryHunk fwd = fh.GetForwardBinaryHunk();
			BinaryHunk rev = fh.GetReverseBinaryHunk();
			NUnit.Framework.Assert.IsNotNull(fwd);
			NUnit.Framework.Assert.IsNotNull(rev);
			NUnit.Framework.Assert.AreEqual(12, fwd.GetSize());
			NUnit.Framework.Assert.AreEqual(11, rev.GetSize());
			NUnit.Framework.Assert.AreSame(fh, fwd.GetFileHeader());
			NUnit.Framework.Assert.AreSame(fh, rev.GetFileHeader());
			NUnit.Framework.Assert.AreEqual(BinaryHunk.Type.DELTA_DEFLATED, fwd.GetType());
			NUnit.Framework.Assert.AreEqual(BinaryHunk.Type.DELTA_DEFLATED, rev.GetType());
			NUnit.Framework.Assert.AreEqual(496, fh.endOffset);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_FixNoNewline()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			FileHeader f = p.GetFiles()[0];
			NUnit.Framework.Assert.AreEqual("a", f.GetNewPath());
			NUnit.Framework.Assert.AreEqual(252, f.startOffset);
			NUnit.Framework.Assert.AreEqual("2e65efe", f.GetOldId().Name);
			NUnit.Framework.Assert.AreEqual("f2ad6c7", f.GetNewId().Name);
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, f.GetPatchType());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, f.GetOldMode());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, f.GetNewMode());
			NUnit.Framework.Assert.AreEqual(1, f.GetHunks().Count);
			{
				HunkHeader h = f.GetHunks()[0];
				NUnit.Framework.Assert.AreSame(f, h.GetFileHeader());
				NUnit.Framework.Assert.AreEqual(317, h.startOffset);
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage().GetStartLine());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage().GetLineCount());
				NUnit.Framework.Assert.AreEqual(1, h.GetNewStartLine());
				NUnit.Framework.Assert.AreEqual(1, h.GetNewLineCount());
				NUnit.Framework.Assert.AreEqual(0, h.GetLinesContext());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage().GetLinesAdded());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage().GetLinesDeleted());
				NUnit.Framework.Assert.AreSame(f.GetOldId(), h.GetOldImage().GetId());
				NUnit.Framework.Assert.AreEqual(363, h.endOffset);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_AddNoNewline()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			FileHeader f = p.GetFiles()[0];
			NUnit.Framework.Assert.AreEqual("a", f.GetNewPath());
			NUnit.Framework.Assert.AreEqual(256, f.startOffset);
			NUnit.Framework.Assert.AreEqual("f2ad6c7", f.GetOldId().Name);
			NUnit.Framework.Assert.AreEqual("c59d9b6", f.GetNewId().Name);
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, f.GetPatchType());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, f.GetOldMode());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, f.GetNewMode());
			NUnit.Framework.Assert.AreEqual(1, f.GetHunks().Count);
			{
				HunkHeader h = f.GetHunks()[0];
				NUnit.Framework.Assert.AreSame(f, h.GetFileHeader());
				NUnit.Framework.Assert.AreEqual(321, h.startOffset);
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage().GetStartLine());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage().GetLineCount());
				NUnit.Framework.Assert.AreEqual(1, h.GetNewStartLine());
				NUnit.Framework.Assert.AreEqual(1, h.GetNewLineCount());
				NUnit.Framework.Assert.AreEqual(0, h.GetLinesContext());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage().GetLinesAdded());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage().GetLinesDeleted());
				NUnit.Framework.Assert.AreSame(f.GetOldId(), h.GetOldImage().GetId());
				NUnit.Framework.Assert.AreEqual(367, h.endOffset);
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
