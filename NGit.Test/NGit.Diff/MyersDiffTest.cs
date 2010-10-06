using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	public class MyersDiffTest : AbstractDiffTestCase
	{
		protected internal override DiffAlgorithm Algorithm()
		{
			return MyersDiff.INSTANCE;
		}

		public virtual void TestPerformanceTestDeltaLength()
		{
			string a = DiffTestDataGenerator.GenerateSequence(40000, 971, 3);
			string b = DiffTestDataGenerator.GenerateSequence(40000, 1621, 5);
			DiffPerformanceTest.CharArray ac = new DiffPerformanceTest.CharArray(a);
			DiffPerformanceTest.CharArray bc = new DiffPerformanceTest.CharArray(b);
			EditList r = Algorithm().Diff(new DiffPerformanceTest.CharCmp(), ac, bc);
			NUnit.Framework.Assert.AreEqual(131, r.Count);
		}
	}
}
