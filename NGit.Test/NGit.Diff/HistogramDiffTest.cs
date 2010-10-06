using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	public class HistogramDiffTest : AbstractDiffTestCase
	{
		protected internal override DiffAlgorithm Algorithm()
		{
			HistogramDiff hd = new HistogramDiff();
			hd.SetFallbackAlgorithm(null);
			return hd;
		}

		public virtual void TestEdit_NoUniqueMiddleSide_FlipBlocks()
		{
			EditList r = Diff(T("aRRSSz"), T("aSSRRz"));
			NUnit.Framework.Assert.AreEqual(2, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 3, 1, 1), r[0]);
			// DELETE "RR"
			NUnit.Framework.Assert.AreEqual(new Edit(5, 5, 3, 5), r[1]);
		}

		// INSERT "RR
		public virtual void TestEdit_NoUniqueMiddleSide_Insert2()
		{
			EditList r = Diff(T("aRSz"), T("aRRSSz"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(2, 2, 2, 4), r[0]);
		}

		public virtual void TestEdit_NoUniqueMiddleSide_FlipAndExpand()
		{
			EditList r = Diff(T("aRSz"), T("aSSRRz"));
			NUnit.Framework.Assert.AreEqual(2, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 2, 1, 1), r[0]);
			// DELETE "R"
			NUnit.Framework.Assert.AreEqual(new Edit(3, 3, 2, 5), r[1]);
		}

		// INSERT "SRR"
		public virtual void TestExceedsChainLenght_DuringScanOfA()
		{
			HistogramDiff hd = new HistogramDiff();
			hd.SetFallbackAlgorithm(null);
			hd.SetMaxChainLength(3);
			SequenceComparator<RawText> cmp = new _SequenceComparator_82();
			EditList r = hd.Diff(cmp, T("RabS"), T("QabT"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 4, 0, 4), r[0]);
		}

		private sealed class _SequenceComparator_82 : SequenceComparator<RawText>
		{
			public _SequenceComparator_82()
			{
			}

			public override bool Equals(RawText a, int ai, RawText b, int bi)
			{
				return RawTextComparator.DEFAULT.Equals(a, ai, b, bi);
			}

			public override int Hash(RawText a, int ai)
			{
				return 1;
			}
		}

		public virtual void TestExceedsChainLenght_DuringScanOfB()
		{
			HistogramDiff hd = new HistogramDiff();
			hd.SetFallbackAlgorithm(null);
			hd.SetMaxChainLength(1);
			EditList r = hd.Diff(RawTextComparator.DEFAULT, T("RaaS"), T("QaaT"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 4, 0, 4), r[0]);
		}

		public virtual void TestFallbackToMyersDiff()
		{
			HistogramDiff hd = new HistogramDiff();
			hd.SetMaxChainLength(64);
			string a = DiffTestDataGenerator.GenerateSequence(40000, 971, 3);
			string b = DiffTestDataGenerator.GenerateSequence(40000, 1621, 5);
			DiffPerformanceTest.CharCmp cmp = new DiffPerformanceTest.CharCmp();
			DiffPerformanceTest.CharArray ac = new DiffPerformanceTest.CharArray(a);
			DiffPerformanceTest.CharArray bc = new DiffPerformanceTest.CharArray(b);
			EditList r;
			// Without fallback our results are limited due to collisions.
			hd.SetFallbackAlgorithm(null);
			r = hd.Diff(cmp, ac, bc);
			NUnit.Framework.Assert.AreEqual(70, r.Count);
			// Results go up when we add a fallback for the high collision regions.
			hd.SetFallbackAlgorithm(MyersDiff.INSTANCE);
			r = hd.Diff(cmp, ac, bc);
			NUnit.Framework.Assert.AreEqual(73, r.Count);
			// But they still differ from Myers due to the way we did early steps.
			EditList myersResult = MyersDiff.INSTANCE.Diff(cmp, ac, bc);
			NUnit.Framework.Assert.IsFalse("Not same as Myers", myersResult.Equals(r));
		}

		public virtual void TestPerformanceTestDeltaLength()
		{
			HistogramDiff hd = new HistogramDiff();
			hd.SetFallbackAlgorithm(null);
			string a = DiffTestDataGenerator.GenerateSequence(40000, 971, 3);
			string b = DiffTestDataGenerator.GenerateSequence(40000, 1621, 5);
			DiffPerformanceTest.CharCmp cmp = new DiffPerformanceTest.CharCmp();
			DiffPerformanceTest.CharArray ac = new DiffPerformanceTest.CharArray(a);
			DiffPerformanceTest.CharArray bc = new DiffPerformanceTest.CharArray(b);
			EditList r;
			hd.SetMaxChainLength(64);
			r = hd.Diff(cmp, ac, bc);
			NUnit.Framework.Assert.AreEqual(70, r.Count);
			hd.SetMaxChainLength(176);
			r = hd.Diff(cmp, ac, bc);
			NUnit.Framework.Assert.AreEqual(72, r.Count);
		}
	}
}
