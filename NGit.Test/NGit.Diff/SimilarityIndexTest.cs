using NGit;
using NGit.Diff;
using NUnit.Framework;
using Sharpen;

namespace NGit.Diff
{
	public class SimilarityIndexTest : TestCase
	{
		public virtual void TestIndexingSmallObject()
		{
			SimilarityIndex si = Hash(string.Empty + "A\n" + "B\n" + "D\n" + "B\n");
			//
			//
			//
			//
			//
			int key_A = KeyFor("A\n");
			int key_B = KeyFor("B\n");
			int key_D = KeyFor("D\n");
			NUnit.Framework.Assert.IsTrue(key_A != key_B && key_A != key_D && key_B != key_D);
			NUnit.Framework.Assert.AreEqual(3, si.Size());
			NUnit.Framework.Assert.AreEqual(2, si.Count(si.FindIndex(key_A)));
			NUnit.Framework.Assert.AreEqual(4, si.Count(si.FindIndex(key_B)));
			NUnit.Framework.Assert.AreEqual(2, si.Count(si.FindIndex(key_D)));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestIndexingLargeObject()
		{
			byte[] @in = Sharpen.Runtime.GetBytesForString((string.Empty + "A\n" + "B\n" + "B\n"
				 + "B\n"), "UTF-8");
			//
			//
			//
			//
			SimilarityIndex si = new SimilarityIndex();
			si.Hash(new ByteArrayInputStream(@in), @in.Length);
			NUnit.Framework.Assert.AreEqual(2, si.Size());
		}

		public virtual void TestCommonScore_SameFiles()
		{
			string text = string.Empty + "A\n" + "B\n" + "D\n" + "B\n";
			//
			//
			//
			//
			SimilarityIndex src = Hash(text);
			SimilarityIndex dst = Hash(text);
			NUnit.Framework.Assert.AreEqual(8, src.Common(dst));
			NUnit.Framework.Assert.AreEqual(8, dst.Common(src));
			NUnit.Framework.Assert.AreEqual(100, src.Score(dst, 100));
			NUnit.Framework.Assert.AreEqual(100, dst.Score(src, 100));
		}

		public virtual void TestCommonScore_EmptyFiles()
		{
			SimilarityIndex src = Hash(string.Empty);
			SimilarityIndex dst = Hash(string.Empty);
			NUnit.Framework.Assert.AreEqual(0, src.Common(dst));
			NUnit.Framework.Assert.AreEqual(0, dst.Common(src));
		}

		public virtual void TestCommonScore_TotallyDifferentFiles()
		{
			SimilarityIndex src = Hash("A\n");
			SimilarityIndex dst = Hash("D\n");
			NUnit.Framework.Assert.AreEqual(0, src.Common(dst));
			NUnit.Framework.Assert.AreEqual(0, dst.Common(src));
		}

		public virtual void TestCommonScore_SimiliarBy75()
		{
			SimilarityIndex src = Hash("A\nB\nC\nD\n");
			SimilarityIndex dst = Hash("A\nB\nC\nQ\n");
			NUnit.Framework.Assert.AreEqual(6, src.Common(dst));
			NUnit.Framework.Assert.AreEqual(6, dst.Common(src));
			NUnit.Framework.Assert.AreEqual(75, src.Score(dst, 100));
			NUnit.Framework.Assert.AreEqual(75, dst.Score(src, 100));
		}

		private static SimilarityIndex Hash(string text)
		{
			SimilarityIndex src = new _SimilarityIndex_124();
			byte[] raw = Constants.Encode(text);
			src.SetFileSize(raw.Length);
			src.Hash(raw, 0, raw.Length);
			src.Sort();
			return src;
		}

		private sealed class _SimilarityIndex_124 : SimilarityIndex
		{
			public _SimilarityIndex_124()
			{
			}

			internal override void Hash(byte[] raw, int ptr, int end)
			{
				while (ptr < end)
				{
					int hash = raw[ptr] & unchecked((int)(0xff));
					int start = ptr;
					do
					{
						int c = raw[ptr++] & unchecked((int)(0xff));
						if (c == '\n')
						{
							break;
						}
					}
					while (ptr < end && ptr - start < 64);
					this.Add(hash, ptr - start);
				}
			}
		}

		private static int KeyFor(string line)
		{
			SimilarityIndex si = Hash(line);
			NUnit.Framework.Assert.AreEqual("single line scored", 1, si.Size());
			return si.Key(0);
		}
	}
}
