using NGit.Diff;
using NGit.Patch;
using NUnit.Framework;
using Sharpen;

namespace NGit.Patch
{
	public class EditListTest : TestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
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
