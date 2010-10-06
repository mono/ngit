using System.Collections.Generic;
using NGit;
using NGit.Diff;
using NGit.Patch;
using NUnit.Framework;
using Sharpen;

namespace NGit.Patch
{
	public class PatchCcTest : TestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestParse_OneFileCc()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			CombinedFileHeader cfh = (CombinedFileHeader)p.GetFiles()[0];
			NUnit.Framework.Assert.AreEqual("org.spearce.egit.ui/src/org/spearce/egit/ui/UIText.java"
				, cfh.GetNewPath());
			NUnit.Framework.Assert.AreEqual(cfh.GetNewPath(), cfh.GetOldPath());
			NUnit.Framework.Assert.AreEqual(98, cfh.startOffset);
			NUnit.Framework.Assert.AreEqual(2, cfh.GetParentCount());
			NUnit.Framework.Assert.AreSame(cfh.GetOldId(0), cfh.GetOldId());
			NUnit.Framework.Assert.AreEqual("169356b", cfh.GetOldId(0).Name);
			NUnit.Framework.Assert.AreEqual("dd8c317", cfh.GetOldId(1).Name);
			NUnit.Framework.Assert.AreEqual("fd85931", cfh.GetNewId().Name);
			NUnit.Framework.Assert.AreSame(cfh.GetOldMode(0), cfh.GetOldMode());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, cfh.GetOldMode(0));
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, cfh.GetOldMode(1));
			NUnit.Framework.Assert.AreSame(FileMode.EXECUTABLE_FILE, cfh.GetNewMode());
			NUnit.Framework.Assert.AreSame(DiffEntry.ChangeType.MODIFY, cfh.GetChangeType());
			NUnit.Framework.Assert.AreSame(FileHeader.PatchType.UNIFIED, cfh.GetPatchType());
			NUnit.Framework.Assert.AreEqual(1, ((IList<CombinedHunkHeader>)cfh.GetHunks()).Count
				);
			{
				CombinedHunkHeader h = ((IList<CombinedHunkHeader>)cfh.GetHunks())[0];
				NUnit.Framework.Assert.AreSame(cfh, ((CombinedFileHeader)h.GetFileHeader()));
				NUnit.Framework.Assert.AreEqual(346, h.startOffset);
				NUnit.Framework.Assert.AreEqual(764, h.endOffset);
				NUnit.Framework.Assert.AreSame(h.GetOldImage(0), h.GetOldImage());
				NUnit.Framework.Assert.AreSame(cfh.GetOldId(0), h.GetOldImage(0).GetId());
				NUnit.Framework.Assert.AreSame(cfh.GetOldId(1), h.GetOldImage(1).GetId());
				NUnit.Framework.Assert.AreEqual(55, h.GetOldImage(0).GetStartLine());
				NUnit.Framework.Assert.AreEqual(12, h.GetOldImage(0).GetLineCount());
				NUnit.Framework.Assert.AreEqual(3, h.GetOldImage(0).GetLinesAdded());
				NUnit.Framework.Assert.AreEqual(0, h.GetOldImage(0).GetLinesDeleted());
				NUnit.Framework.Assert.AreEqual(163, h.GetOldImage(1).GetStartLine());
				NUnit.Framework.Assert.AreEqual(13, h.GetOldImage(1).GetLineCount());
				NUnit.Framework.Assert.AreEqual(2, h.GetOldImage(1).GetLinesAdded());
				NUnit.Framework.Assert.AreEqual(0, h.GetOldImage(1).GetLinesDeleted());
				NUnit.Framework.Assert.AreEqual(163, h.GetNewStartLine());
				NUnit.Framework.Assert.AreEqual(15, h.GetNewLineCount());
				NUnit.Framework.Assert.AreEqual(10, h.GetLinesContext());
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestParse_CcNewFile()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			CombinedFileHeader cfh = (CombinedFileHeader)p.GetFiles()[0];
			NUnit.Framework.Assert.AreSame(DiffEntry.DEV_NULL, cfh.GetOldPath());
			NUnit.Framework.Assert.AreEqual("d", cfh.GetNewPath());
			NUnit.Framework.Assert.AreEqual(187, cfh.startOffset);
			NUnit.Framework.Assert.AreEqual(2, cfh.GetParentCount());
			NUnit.Framework.Assert.AreSame(cfh.GetOldId(0), cfh.GetOldId());
			NUnit.Framework.Assert.AreEqual("0000000", cfh.GetOldId(0).Name);
			NUnit.Framework.Assert.AreEqual("0000000", cfh.GetOldId(1).Name);
			NUnit.Framework.Assert.AreEqual("4bcfe98", cfh.GetNewId().Name);
			NUnit.Framework.Assert.AreSame(cfh.GetOldMode(0), cfh.GetOldMode());
			NUnit.Framework.Assert.AreSame(FileMode.MISSING, cfh.GetOldMode(0));
			NUnit.Framework.Assert.AreSame(FileMode.MISSING, cfh.GetOldMode(1));
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, cfh.GetNewMode());
			NUnit.Framework.Assert.AreSame(DiffEntry.ChangeType.ADD, cfh.GetChangeType());
			NUnit.Framework.Assert.AreSame(FileHeader.PatchType.UNIFIED, cfh.GetPatchType());
			NUnit.Framework.Assert.AreEqual(1, ((IList<CombinedHunkHeader>)cfh.GetHunks()).Count
				);
			{
				CombinedHunkHeader h = ((IList<CombinedHunkHeader>)cfh.GetHunks())[0];
				NUnit.Framework.Assert.AreSame(cfh, ((CombinedFileHeader)h.GetFileHeader()));
				NUnit.Framework.Assert.AreEqual(273, h.startOffset);
				NUnit.Framework.Assert.AreEqual(300, h.endOffset);
				NUnit.Framework.Assert.AreSame(h.GetOldImage(0), h.GetOldImage());
				NUnit.Framework.Assert.AreSame(cfh.GetOldId(0), h.GetOldImage(0).GetId());
				NUnit.Framework.Assert.AreSame(cfh.GetOldId(1), h.GetOldImage(1).GetId());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage(0).GetStartLine());
				NUnit.Framework.Assert.AreEqual(0, h.GetOldImage(0).GetLineCount());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage(0).GetLinesAdded());
				NUnit.Framework.Assert.AreEqual(0, h.GetOldImage(0).GetLinesDeleted());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage(1).GetStartLine());
				NUnit.Framework.Assert.AreEqual(0, h.GetOldImage(1).GetLineCount());
				NUnit.Framework.Assert.AreEqual(1, h.GetOldImage(1).GetLinesAdded());
				NUnit.Framework.Assert.AreEqual(0, h.GetOldImage(1).GetLinesDeleted());
				NUnit.Framework.Assert.AreEqual(1, h.GetNewStartLine());
				NUnit.Framework.Assert.AreEqual(1, h.GetNewLineCount());
				NUnit.Framework.Assert.AreEqual(0, h.GetLinesContext());
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestParse_CcDeleteFile()
		{
			NGit.Patch.Patch p = ParseTestPatchFile();
			NUnit.Framework.Assert.AreEqual(1, p.GetFiles().Count);
			NUnit.Framework.Assert.IsTrue(p.GetErrors().IsEmpty());
			CombinedFileHeader cfh = (CombinedFileHeader)p.GetFiles()[0];
			NUnit.Framework.Assert.AreEqual("a", cfh.GetOldPath());
			NUnit.Framework.Assert.AreSame(DiffEntry.DEV_NULL, cfh.GetNewPath());
			NUnit.Framework.Assert.AreEqual(187, cfh.startOffset);
			NUnit.Framework.Assert.AreEqual(2, cfh.GetParentCount());
			NUnit.Framework.Assert.AreSame(cfh.GetOldId(0), cfh.GetOldId());
			NUnit.Framework.Assert.AreEqual("7898192", cfh.GetOldId(0).Name);
			NUnit.Framework.Assert.AreEqual("2e65efe", cfh.GetOldId(1).Name);
			NUnit.Framework.Assert.AreEqual("0000000", cfh.GetNewId().Name);
			NUnit.Framework.Assert.AreSame(cfh.GetOldMode(0), cfh.GetOldMode());
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, cfh.GetOldMode(0));
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, cfh.GetOldMode(1));
			NUnit.Framework.Assert.AreSame(FileMode.MISSING, cfh.GetNewMode());
			NUnit.Framework.Assert.AreSame(DiffEntry.ChangeType.DELETE, cfh.GetChangeType());
			NUnit.Framework.Assert.AreSame(FileHeader.PatchType.UNIFIED, cfh.GetPatchType());
			NUnit.Framework.Assert.IsTrue(((IList<CombinedHunkHeader>)cfh.GetHunks()).IsEmpty
				());
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
