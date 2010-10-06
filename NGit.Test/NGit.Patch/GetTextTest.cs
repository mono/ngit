using System.Collections.Generic;
using System.Text;
using NGit.Patch;
using NUnit.Framework;
using Sharpen;

namespace NGit.Patch
{
	public class GetTextTest : TestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
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
		public virtual void TestGetText_DiffCc()
		{
			Encoding csOld = Sharpen.Extensions.GetEncoding("ISO-8859-1");
			Encoding csNew = Sharpen.Extensions.GetEncoding("UTF-8");
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			CombinedFileHeader fh = (CombinedFileHeader)p.GetFiles()[0];
			NUnit.Framework.Assert.AreEqual(1, ((IList<CombinedHunkHeader>)fh.GetHunks()).Count
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

		/// <exception cref="System.IO.IOException"></exception>
		private string ReadTestPatchFile(Encoding cs)
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
