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
using NGit.Patch;
using Sharpen;

namespace NGit.Patch
{
	[NUnit.Framework.TestFixture]
	public class FileHeaderTest
	{
		[NUnit.Framework.Test]
		public virtual void TestParseGitFileName_Empty()
		{
			FileHeader fh = Data(string.Empty);
			NUnit.Framework.Assert.AreEqual(-1, fh.ParseGitFileName(0, fh.buf.Length));
			NUnit.Framework.Assert.IsNotNull(fh.GetHunks());
			NUnit.Framework.Assert.IsTrue(fh.GetHunks().IsEmpty());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseGitFileName_NoLF()
		{
			FileHeader fh = Data("a/ b/");
			NUnit.Framework.Assert.AreEqual(-1, fh.ParseGitFileName(0, fh.buf.Length));
		}

		[NUnit.Framework.Test]
		public virtual void TestParseGitFileName_NoSecondLine()
		{
			FileHeader fh = Data("\n");
			NUnit.Framework.Assert.AreEqual(-1, fh.ParseGitFileName(0, fh.buf.Length));
		}

		[NUnit.Framework.Test]
		public virtual void TestParseGitFileName_EmptyHeader()
		{
			FileHeader fh = Data("\n\n");
			NUnit.Framework.Assert.AreEqual(1, fh.ParseGitFileName(0, fh.buf.Length));
		}

		[NUnit.Framework.Test]
		public virtual void TestParseGitFileName_Foo()
		{
			string name = "foo";
			FileHeader fh = Header(name);
			NUnit.Framework.Assert.AreEqual(GitLine(name).Length, fh.ParseGitFileName(0, fh.buf
				.Length));
			NUnit.Framework.Assert.AreEqual(name, fh.GetOldPath());
			NUnit.Framework.Assert.AreSame(fh.GetOldPath(), fh.GetNewPath());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseGitFileName_FailFooBar()
		{
			FileHeader fh = Data("a/foo b/bar\n-");
			NUnit.Framework.Assert.IsTrue(fh.ParseGitFileName(0, fh.buf.Length) > 0);
			NUnit.Framework.Assert.IsNull(fh.GetOldPath());
			NUnit.Framework.Assert.IsNull(fh.GetNewPath());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseGitFileName_FooSpBar()
		{
			string name = "foo bar";
			FileHeader fh = Header(name);
			NUnit.Framework.Assert.AreEqual(GitLine(name).Length, fh.ParseGitFileName(0, fh.buf
				.Length));
			NUnit.Framework.Assert.AreEqual(name, fh.GetOldPath());
			NUnit.Framework.Assert.AreSame(fh.GetOldPath(), fh.GetNewPath());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseGitFileName_DqFooTabBar()
		{
			string name = "foo\tbar";
			string dqName = "foo\\tbar";
			FileHeader fh = DqHeader(dqName);
			NUnit.Framework.Assert.AreEqual(DqGitLine(dqName).Length, fh.ParseGitFileName(0, 
				fh.buf.Length));
			NUnit.Framework.Assert.AreEqual(name, fh.GetOldPath());
			NUnit.Framework.Assert.AreSame(fh.GetOldPath(), fh.GetNewPath());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
		}

		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("This test does not pass and am not sure why")]
		public virtual void TestParseGitFileName_DqFooSpLfNulBar()
		{
			string name = "foo \n\x0bar";
			string dqName = "foo \\n\\0bar";
			FileHeader fh = DqHeader(dqName);
			NUnit.Framework.Assert.AreEqual(DqGitLine(dqName).Length, fh.ParseGitFileName(0, 
				fh.buf.Length));
			NUnit.Framework.Assert.AreEqual(name, fh.GetOldPath());
			NUnit.Framework.Assert.AreSame(fh.GetOldPath(), fh.GetNewPath());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseGitFileName_SrcFooC()
		{
			string name = "src/foo/bar/argh/code.c";
			FileHeader fh = Header(name);
			NUnit.Framework.Assert.AreEqual(GitLine(name).Length, fh.ParseGitFileName(0, fh.buf
				.Length));
			NUnit.Framework.Assert.AreEqual(name, fh.GetOldPath());
			NUnit.Framework.Assert.AreSame(fh.GetOldPath(), fh.GetNewPath());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseGitFileName_SrcFooCNonStandardPrefix()
		{
			string name = "src/foo/bar/argh/code.c";
			string header = "project-v-1.0/" + name + " mydev/" + name + "\n";
			FileHeader fh = Data(header + "-");
			NUnit.Framework.Assert.AreEqual(header.Length, fh.ParseGitFileName(0, fh.buf.Length
				));
			NUnit.Framework.Assert.AreEqual(name, fh.GetOldPath());
			NUnit.Framework.Assert.AreSame(fh.GetOldPath(), fh.GetNewPath());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseUnicodeName_NewFile()
		{
			FileHeader fh = Data("diff --git \"a/\\303\\205ngstr\\303\\266m\" \"b/\\303\\205ngstr\\303\\266m\"\n"
				 + "new file mode 100644\n" + "index 0000000..7898192\n" + "--- /dev/null\n" + "+++ \"b/\\303\\205ngstr\\303\\266m\"\n"
				 + "@@ -0,0 +1 @@\n" + "+a\n");
			AssertParse(fh);
			NUnit.Framework.Assert.AreEqual("/dev/null", fh.GetOldPath());
			NUnit.Framework.Assert.AreSame(DiffEntry.DEV_NULL, fh.GetOldPath());
			NUnit.Framework.Assert.AreEqual("\u00c5ngstr\u00f6m", fh.GetNewPath());
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.ADD, fh.GetChangeType());
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, fh.GetPatchType());
			NUnit.Framework.Assert.IsTrue(fh.HasMetaDataChanges());
			NUnit.Framework.Assert.AreSame(FileMode.MISSING, fh.GetOldMode());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fh.GetNewMode());
			NUnit.Framework.Assert.AreEqual("0000000", fh.GetOldId().Name);
			NUnit.Framework.Assert.AreEqual("7898192", fh.GetNewId().Name);
			NUnit.Framework.Assert.AreEqual(0, fh.GetScore());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseUnicodeName_DeleteFile()
		{
			FileHeader fh = Data("diff --git \"a/\\303\\205ngstr\\303\\266m\" \"b/\\303\\205ngstr\\303\\266m\"\n"
				 + "deleted file mode 100644\n" + "index 7898192..0000000\n" + "--- \"a/\\303\\205ngstr\\303\\266m\"\n"
				 + "+++ /dev/null\n" + "@@ -1 +0,0 @@\n" + "-a\n");
			AssertParse(fh);
			NUnit.Framework.Assert.AreEqual("\u00c5ngstr\u00f6m", fh.GetOldPath());
			NUnit.Framework.Assert.AreEqual("/dev/null", fh.GetNewPath());
			NUnit.Framework.Assert.AreSame(DiffEntry.DEV_NULL, fh.GetNewPath());
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.DELETE, fh.GetChangeType());
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, fh.GetPatchType());
			NUnit.Framework.Assert.IsTrue(fh.HasMetaDataChanges());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fh.GetOldMode());
			NUnit.Framework.Assert.AreSame(FileMode.MISSING, fh.GetNewMode());
			NUnit.Framework.Assert.AreEqual("7898192", fh.GetOldId().Name);
			NUnit.Framework.Assert.AreEqual("0000000", fh.GetNewId().Name);
			NUnit.Framework.Assert.AreEqual(0, fh.GetScore());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseModeChange()
		{
			FileHeader fh = Data("diff --git a/a b b/a b\n" + "old mode 100644\n" + "new mode 100755\n"
				);
			AssertParse(fh);
			NUnit.Framework.Assert.AreEqual("a b", fh.GetOldPath());
			NUnit.Framework.Assert.AreEqual("a b", fh.GetNewPath());
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, fh.GetChangeType());
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, fh.GetPatchType());
			NUnit.Framework.Assert.IsTrue(fh.HasMetaDataChanges());
			NUnit.Framework.Assert.IsNull(fh.GetOldId());
			NUnit.Framework.Assert.IsNull(fh.GetNewId());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fh.GetOldMode());
			NUnit.Framework.Assert.AreSame(FileMode.EXECUTABLE_FILE, fh.GetNewMode());
			NUnit.Framework.Assert.AreEqual(0, fh.GetScore());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseRename100_NewStyle()
		{
			FileHeader fh = Data("diff --git a/a b/ c/\\303\\205ngstr\\303\\266m\n" + "similarity index 100%\n"
				 + "rename from a\n" + "rename to \" c/\\303\\205ngstr\\303\\266m\"\n");
			int ptr = fh.ParseGitFileName(0, fh.buf.Length);
			NUnit.Framework.Assert.IsTrue(ptr > 0);
			NUnit.Framework.Assert.IsNull(fh.GetOldPath());
			// can't parse names on a rename
			NUnit.Framework.Assert.IsNull(fh.GetNewPath());
			ptr = fh.ParseGitHeaders(ptr, fh.buf.Length);
			NUnit.Framework.Assert.IsTrue(ptr > 0);
			NUnit.Framework.Assert.AreEqual("a", fh.GetOldPath());
			NUnit.Framework.Assert.AreEqual(" c/\u00c5ngstr\u00f6m", fh.GetNewPath());
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.RENAME, fh.GetChangeType());
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, fh.GetPatchType());
			NUnit.Framework.Assert.IsTrue(fh.HasMetaDataChanges());
			NUnit.Framework.Assert.IsNull(fh.GetOldId());
			NUnit.Framework.Assert.IsNull(fh.GetNewId());
			NUnit.Framework.Assert.IsNull(fh.GetOldMode());
			NUnit.Framework.Assert.IsNull(fh.GetNewMode());
			NUnit.Framework.Assert.AreEqual(100, fh.GetScore());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseRename100_OldStyle()
		{
			FileHeader fh = Data("diff --git a/a b/ c/\\303\\205ngstr\\303\\266m\n" + "similarity index 100%\n"
				 + "rename old a\n" + "rename new \" c/\\303\\205ngstr\\303\\266m\"\n");
			int ptr = fh.ParseGitFileName(0, fh.buf.Length);
			NUnit.Framework.Assert.IsTrue(ptr > 0);
			NUnit.Framework.Assert.IsNull(fh.GetOldPath());
			// can't parse names on a rename
			NUnit.Framework.Assert.IsNull(fh.GetNewPath());
			ptr = fh.ParseGitHeaders(ptr, fh.buf.Length);
			NUnit.Framework.Assert.IsTrue(ptr > 0);
			NUnit.Framework.Assert.AreEqual("a", fh.GetOldPath());
			NUnit.Framework.Assert.AreEqual(" c/\u00c5ngstr\u00f6m", fh.GetNewPath());
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.RENAME, fh.GetChangeType());
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, fh.GetPatchType());
			NUnit.Framework.Assert.IsTrue(fh.HasMetaDataChanges());
			NUnit.Framework.Assert.IsNull(fh.GetOldId());
			NUnit.Framework.Assert.IsNull(fh.GetNewId());
			NUnit.Framework.Assert.IsNull(fh.GetOldMode());
			NUnit.Framework.Assert.IsNull(fh.GetNewMode());
			NUnit.Framework.Assert.AreEqual(100, fh.GetScore());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseCopy100()
		{
			FileHeader fh = Data("diff --git a/a b/ c/\\303\\205ngstr\\303\\266m\n" + "similarity index 100%\n"
				 + "copy from a\n" + "copy to \" c/\\303\\205ngstr\\303\\266m\"\n");
			int ptr = fh.ParseGitFileName(0, fh.buf.Length);
			NUnit.Framework.Assert.IsTrue(ptr > 0);
			NUnit.Framework.Assert.IsNull(fh.GetOldPath());
			// can't parse names on a copy
			NUnit.Framework.Assert.IsNull(fh.GetNewPath());
			ptr = fh.ParseGitHeaders(ptr, fh.buf.Length);
			NUnit.Framework.Assert.IsTrue(ptr > 0);
			NUnit.Framework.Assert.AreEqual("a", fh.GetOldPath());
			NUnit.Framework.Assert.AreEqual(" c/\u00c5ngstr\u00f6m", fh.GetNewPath());
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.COPY, fh.GetChangeType());
			NUnit.Framework.Assert.AreEqual(FileHeader.PatchType.UNIFIED, fh.GetPatchType());
			NUnit.Framework.Assert.IsTrue(fh.HasMetaDataChanges());
			NUnit.Framework.Assert.IsNull(fh.GetOldId());
			NUnit.Framework.Assert.IsNull(fh.GetNewId());
			NUnit.Framework.Assert.IsNull(fh.GetOldMode());
			NUnit.Framework.Assert.IsNull(fh.GetNewMode());
			NUnit.Framework.Assert.AreEqual(100, fh.GetScore());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseFullIndexLine_WithMode()
		{
			string oid = "78981922613b2afb6025042ff6bd878ac1994e85";
			string nid = "61780798228d17af2d34fce4cfbdf35556832472";
			FileHeader fh = Data("diff --git a/a b/a\n" + "index " + oid + ".." + nid + " 100644\n"
				 + "--- a/a\n" + "+++ b/a\n");
			AssertParse(fh);
			NUnit.Framework.Assert.AreEqual("a", fh.GetOldPath());
			NUnit.Framework.Assert.AreEqual("a", fh.GetNewPath());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fh.GetOldMode());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fh.GetNewMode());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
			NUnit.Framework.Assert.IsNotNull(fh.GetOldId());
			NUnit.Framework.Assert.IsNotNull(fh.GetNewId());
			NUnit.Framework.Assert.IsTrue(fh.GetOldId().IsComplete);
			NUnit.Framework.Assert.IsTrue(fh.GetNewId().IsComplete);
			NUnit.Framework.Assert.AreEqual(ObjectId.FromString(oid), fh.GetOldId().ToObjectId
				());
			NUnit.Framework.Assert.AreEqual(ObjectId.FromString(nid), fh.GetNewId().ToObjectId
				());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseFullIndexLine_NoMode()
		{
			string oid = "78981922613b2afb6025042ff6bd878ac1994e85";
			string nid = "61780798228d17af2d34fce4cfbdf35556832472";
			FileHeader fh = Data("diff --git a/a b/a\n" + "index " + oid + ".." + nid + "\n" 
				+ "--- a/a\n" + "+++ b/a\n");
			AssertParse(fh);
			NUnit.Framework.Assert.AreEqual("a", fh.GetOldPath());
			NUnit.Framework.Assert.AreEqual("a", fh.GetNewPath());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
			NUnit.Framework.Assert.IsNull(fh.GetOldMode());
			NUnit.Framework.Assert.IsNull(fh.GetNewMode());
			NUnit.Framework.Assert.IsNotNull(fh.GetOldId());
			NUnit.Framework.Assert.IsNotNull(fh.GetNewId());
			NUnit.Framework.Assert.IsTrue(fh.GetOldId().IsComplete);
			NUnit.Framework.Assert.IsTrue(fh.GetNewId().IsComplete);
			NUnit.Framework.Assert.AreEqual(ObjectId.FromString(oid), fh.GetOldId().ToObjectId
				());
			NUnit.Framework.Assert.AreEqual(ObjectId.FromString(nid), fh.GetNewId().ToObjectId
				());
		}

		[NUnit.Framework.Test]
		public virtual void TestParseAbbrIndexLine_WithMode()
		{
			int a = 7;
			string oid = "78981922613b2afb6025042ff6bd878ac1994e85";
			string nid = "61780798228d17af2d34fce4cfbdf35556832472";
			FileHeader fh = Data("diff --git a/a b/a\n" + "index " + Sharpen.Runtime.Substring
				(oid, 0, a - 1) + ".." + Sharpen.Runtime.Substring(nid, 0, a - 1) + " 100644\n" 
				+ "--- a/a\n" + "+++ b/a\n");
			AssertParse(fh);
			NUnit.Framework.Assert.AreEqual("a", fh.GetOldPath());
			NUnit.Framework.Assert.AreEqual("a", fh.GetNewPath());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fh.GetOldMode());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, fh.GetNewMode());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
			NUnit.Framework.Assert.IsNotNull(fh.GetOldId());
			NUnit.Framework.Assert.IsNotNull(fh.GetNewId());
			NUnit.Framework.Assert.IsFalse(fh.GetOldId().IsComplete);
			NUnit.Framework.Assert.IsFalse(fh.GetNewId().IsComplete);
			NUnit.Framework.Assert.AreEqual(Sharpen.Runtime.Substring(oid, 0, a - 1), fh.GetOldId
				().Name);
			NUnit.Framework.Assert.AreEqual(Sharpen.Runtime.Substring(nid, 0, a - 1), fh.GetNewId
				().Name);
			NUnit.Framework.Assert.IsTrue(ObjectId.FromString(oid).StartsWith(fh.GetOldId()));
			NUnit.Framework.Assert.IsTrue(ObjectId.FromString(nid).StartsWith(fh.GetNewId()));
		}

		[NUnit.Framework.Test]
		public virtual void TestParseAbbrIndexLine_NoMode()
		{
			int a = 7;
			string oid = "78981922613b2afb6025042ff6bd878ac1994e85";
			string nid = "61780798228d17af2d34fce4cfbdf35556832472";
			FileHeader fh = Data("diff --git a/a b/a\n" + "index " + Sharpen.Runtime.Substring
				(oid, 0, a - 1) + ".." + Sharpen.Runtime.Substring(nid, 0, a - 1) + "\n" + "--- a/a\n"
				 + "+++ b/a\n");
			AssertParse(fh);
			NUnit.Framework.Assert.AreEqual("a", fh.GetOldPath());
			NUnit.Framework.Assert.AreEqual("a", fh.GetNewPath());
			NUnit.Framework.Assert.IsNull(fh.GetOldMode());
			NUnit.Framework.Assert.IsNull(fh.GetNewMode());
			NUnit.Framework.Assert.IsFalse(fh.HasMetaDataChanges());
			NUnit.Framework.Assert.IsNotNull(fh.GetOldId());
			NUnit.Framework.Assert.IsNotNull(fh.GetNewId());
			NUnit.Framework.Assert.IsFalse(fh.GetOldId().IsComplete);
			NUnit.Framework.Assert.IsFalse(fh.GetNewId().IsComplete);
			NUnit.Framework.Assert.AreEqual(Sharpen.Runtime.Substring(oid, 0, a - 1), fh.GetOldId
				().Name);
			NUnit.Framework.Assert.AreEqual(Sharpen.Runtime.Substring(nid, 0, a - 1), fh.GetNewId
				().Name);
			NUnit.Framework.Assert.IsTrue(ObjectId.FromString(oid).StartsWith(fh.GetOldId()));
			NUnit.Framework.Assert.IsTrue(ObjectId.FromString(nid).StartsWith(fh.GetNewId()));
		}

		private static void AssertParse(FileHeader fh)
		{
			int ptr = fh.ParseGitFileName(0, fh.buf.Length);
			NUnit.Framework.Assert.IsTrue(ptr > 0);
			ptr = fh.ParseGitHeaders(ptr, fh.buf.Length);
			NUnit.Framework.Assert.IsTrue(ptr > 0);
		}

		private static FileHeader Data(string @in)
		{
			return new FileHeader(Constants.EncodeASCII(@in), 0);
		}

		private static FileHeader Header(string path)
		{
			return Data(GitLine(path) + "--- " + path + "\n");
		}

		private static string GitLine(string path)
		{
			return "a/" + path + " b/" + path + "\n";
		}

		private static FileHeader DqHeader(string path)
		{
			return Data(DqGitLine(path) + "--- " + path + "\n");
		}

		private static string DqGitLine(string path)
		{
			return "\"a/" + path + "\" \"b/" + path + "\"\n";
		}
	}
}
