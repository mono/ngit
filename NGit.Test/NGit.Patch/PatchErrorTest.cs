using NGit.Patch;
using NUnit.Framework;
using Sharpen;

namespace NGit.Patch
{
	public class PatchErrorTest : TestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
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
			NUnit.Framework.Assert.AreSame(FormatError.Severity.ERROR, e.GetSeverity());
			NUnit.Framework.Assert.AreEqual("Hunk disconnected from file", e.GetMessage());
			NUnit.Framework.Assert.AreEqual(18, e.GetOffset());
			NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@ -109,4 +109,11 @@ assert"
				));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestError_TruncatedOld()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.AreEqual(1, p.GetErrors().Count);
			FormatError e = p.GetErrors()[0];
			NUnit.Framework.Assert.AreSame(FormatError.Severity.ERROR, e.GetSeverity());
			NUnit.Framework.Assert.AreEqual("Truncated hunk, at least 1 old lines is missing"
				, e.GetMessage());
			NUnit.Framework.Assert.AreEqual(313, e.GetOffset());
			NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@ -236,9 +236,9 @@ protected "
				));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestError_TruncatedNew()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.AreEqual(1, p.GetErrors().Count);
			FormatError e = p.GetErrors()[0];
			NUnit.Framework.Assert.AreSame(FormatError.Severity.ERROR, e.GetSeverity());
			NUnit.Framework.Assert.AreEqual("Truncated hunk, at least 1 new lines is missing"
				, e.GetMessage());
			NUnit.Framework.Assert.AreEqual(313, e.GetOffset());
			NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@ -236,9 +236,9 @@ protected "
				));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestError_BodyTooLong()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.AreEqual(1, p.GetErrors().Count);
			FormatError e = p.GetErrors()[0];
			NUnit.Framework.Assert.AreSame(FormatError.Severity.WARNING, e.GetSeverity());
			NUnit.Framework.Assert.AreEqual("Hunk header 4:11 does not match body line count of 4:12"
				, e.GetMessage());
			NUnit.Framework.Assert.AreEqual(349, e.GetOffset());
			NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@ -109,4 +109,11 @@ assert"
				));
		}

		/// <exception cref="System.IO.IOException"></exception>
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
			NUnit.Framework.Assert.AreSame(FormatError.Severity.WARNING, e.GetSeverity());
			NUnit.Framework.Assert.AreEqual("Unexpected hunk trailer", e.GetMessage());
			NUnit.Framework.Assert.AreEqual(926, e.GetOffset());
			NUnit.Framework.Assert.AreEqual("I AM NOT HERE\n", e.GetLineText());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestError_GitBinaryNoForwardHunk()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(2, p.GetFiles().Count);
			{
				FileHeader fh = p.GetFiles()[0];
				NUnit.Framework.Assert.AreEqual("org.spearce.egit.ui/icons/toolbar/fetchd.png", fh
					.GetNewPath());
				NUnit.Framework.Assert.AreSame(FileHeader.PatchType.GIT_BINARY, fh.GetPatchType()
					);
				NUnit.Framework.Assert.IsTrue(fh.GetHunks().IsEmpty());
				NUnit.Framework.Assert.IsNull(fh.GetForwardBinaryHunk());
			}
			{
				FileHeader fh = p.GetFiles()[1];
				NUnit.Framework.Assert.AreEqual("org.spearce.egit.ui/icons/toolbar/fetche.png", fh
					.GetNewPath());
				NUnit.Framework.Assert.AreSame(FileHeader.PatchType.UNIFIED, fh.GetPatchType());
				NUnit.Framework.Assert.IsTrue(fh.GetHunks().IsEmpty());
				NUnit.Framework.Assert.IsNull(fh.GetForwardBinaryHunk());
			}
			NUnit.Framework.Assert.AreEqual(1, p.GetErrors().Count);
			FormatError e = p.GetErrors()[0];
			NUnit.Framework.Assert.AreSame(FormatError.Severity.ERROR, e.GetSeverity());
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
