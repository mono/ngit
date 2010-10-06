using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	public class PatienceDiffTest : AbstractDiffTestCase
	{
		protected internal override DiffAlgorithm Algorithm()
		{
			PatienceDiff pd = new PatienceDiff();
			pd.SetFallbackAlgorithm(null);
			return pd;
		}

		public virtual void TestEdit_NoUniqueMiddleSideA()
		{
			EditList r = Diff(T("aRRSSz"), T("aSSRRz"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 5, 1, 5), r[0]);
		}

		public virtual void TestEdit_NoUniqueMiddleSideB()
		{
			EditList r = Diff(T("aRSz"), T("aSSRRz"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 3, 1, 5), r[0]);
		}

		public virtual void TestPerformanceTestDeltaLength()
		{
			string a = DiffTestDataGenerator.GenerateSequence(40000, 971, 3);
			string b = DiffTestDataGenerator.GenerateSequence(40000, 1621, 5);
			DiffPerformanceTest.CharArray ac = new DiffPerformanceTest.CharArray(a);
			DiffPerformanceTest.CharArray bc = new DiffPerformanceTest.CharArray(b);
			EditList r = Algorithm().Diff(new DiffPerformanceTest.CharCmp(), ac, bc);
			NUnit.Framework.Assert.AreEqual(25, r.Count);
		}
	}
}
