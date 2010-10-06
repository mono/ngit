using NGit;
using NGit.Patch;
using NUnit.Framework;
using Sharpen;

namespace NGit.Patch
{
	public class PatchCcErrorTest : TestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestError_CcTruncatedOld()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.AreEqual(3, p.GetErrors().Count);
			{
				FormatError e = p.GetErrors()[0];
				NUnit.Framework.Assert.AreSame(FormatError.Severity.ERROR, e.GetSeverity());
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().truncatedHunkLinesMissingForAncestor
					, 1, 1), e.GetMessage());
				NUnit.Framework.Assert.AreEqual(346, e.GetOffset());
				NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@@ -55,12 -163,13 +163,15 @@@ public "
					));
			}
			{
				FormatError e = p.GetErrors()[1];
				NUnit.Framework.Assert.AreSame(FormatError.Severity.ERROR, e.GetSeverity());
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().truncatedHunkLinesMissingForAncestor
					, 2, 2), e.GetMessage());
				NUnit.Framework.Assert.AreEqual(346, e.GetOffset());
				NUnit.Framework.Assert.IsTrue(e.GetLineText().StartsWith("@@@ -55,12 -163,13 +163,15 @@@ public "
					));
			}
			{
				FormatError e = p.GetErrors()[2];
				NUnit.Framework.Assert.AreSame(FormatError.Severity.ERROR, e.GetSeverity());
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
