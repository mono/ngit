using System;
using NGit;
using NGit.Errors;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	public class PackReverseIndexTest : RepositoryTestCase
	{
		private PackIndex idx;

		private PackReverseIndex reverseIdx;

		/// <summary>Set up tested class instance, test constructor by the way.</summary>
		/// <remarks>Set up tested class instance, test constructor by the way.</remarks>
		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			// index with both small (< 2^31) and big offsets
			idx = PackIndex.Open(JGitTestUtil.GetTestResourceFile("pack-huge.idx"));
			reverseIdx = new PackReverseIndex(idx);
		}

		/// <summary>Test findObject() for all index entries.</summary>
		/// <remarks>Test findObject() for all index entries.</remarks>
		public virtual void TestFindObject()
		{
			foreach (PackIndex.MutableEntry me in idx)
			{
				AssertEquals(me.ToObjectId(), reverseIdx.FindObject(me.GetOffset()));
			}
		}

		/// <summary>Test findObject() with illegal argument.</summary>
		/// <remarks>Test findObject() with illegal argument.</remarks>
		public virtual void TestFindObjectWrongOffset()
		{
			NUnit.Framework.Assert.IsNull(reverseIdx.FindObject(0));
		}

		/// <summary>Test findNextOffset() for all index entries.</summary>
		/// <remarks>Test findNextOffset() for all index entries.</remarks>
		/// <exception cref="NGit.Errors.CorruptObjectException">NGit.Errors.CorruptObjectException
		/// 	</exception>
		public virtual void TestFindNextOffset()
		{
			long offset = FindFirstOffset();
			NUnit.Framework.Assert.IsTrue(offset > 0);
			for (int i = 0; i < idx.GetObjectCount(); i++)
			{
				long newOffset = reverseIdx.FindNextOffset(offset, long.MaxValue);
				NUnit.Framework.Assert.IsTrue(newOffset > offset);
				if (i == idx.GetObjectCount() - 1)
				{
					NUnit.Framework.Assert.AreEqual(newOffset, long.MaxValue);
				}
				else
				{
					NUnit.Framework.Assert.AreEqual(newOffset, idx.FindOffset(reverseIdx.FindObject(newOffset
						)));
				}
				offset = newOffset;
			}
		}

		/// <summary>Test findNextOffset() with wrong illegal argument as offset.</summary>
		/// <remarks>Test findNextOffset() with wrong illegal argument as offset.</remarks>
		public virtual void TestFindNextOffsetWrongOffset()
		{
			try
			{
				reverseIdx.FindNextOffset(0, long.MaxValue);
				NUnit.Framework.Assert.Fail("findNextOffset() should throw exception");
			}
			catch (CorruptObjectException)
			{
			}
		}

		// expected
		private long FindFirstOffset()
		{
			long min = long.MaxValue;
			foreach (PackIndex.MutableEntry me in idx)
			{
				min = Math.Min(min, me.GetOffset());
			}
			return min;
		}
	}
}
