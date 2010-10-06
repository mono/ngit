/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

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

		[NUnit.Framework.Test]
		public virtual void TestEdit_NoUniqueMiddleSide_FlipBlocks()
		{
			EditList r = Diff(T("aRRSSz"), T("aSSRRz"));
			NUnit.Framework.Assert.AreEqual(2, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 3, 1, 1), r[0]);
			// DELETE "RR"
			NUnit.Framework.Assert.AreEqual(new Edit(5, 5, 3, 5), r[1]);
		}

		// INSERT "RR
		[NUnit.Framework.Test]
		public virtual void TestEdit_NoUniqueMiddleSide_Insert2()
		{
			EditList r = Diff(T("aRSz"), T("aRRSSz"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(2, 2, 2, 4), r[0]);
		}

		[NUnit.Framework.Test]
		public virtual void TestEdit_NoUniqueMiddleSide_FlipAndExpand()
		{
			EditList r = Diff(T("aRSz"), T("aSSRRz"));
			NUnit.Framework.Assert.AreEqual(2, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 2, 1, 1), r[0]);
			// DELETE "R"
			NUnit.Framework.Assert.AreEqual(new Edit(3, 3, 2, 5), r[1]);
		}

		// INSERT "SRR"
		[NUnit.Framework.Test]
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

		[NUnit.Framework.Test]
		public virtual void TestExceedsChainLenght_DuringScanOfB()
		{
			HistogramDiff hd = new HistogramDiff();
			hd.SetFallbackAlgorithm(null);
			hd.SetMaxChainLength(1);
			EditList r = hd.Diff(RawTextComparator.DEFAULT, T("RaaS"), T("QaaT"));
			NUnit.Framework.Assert.AreEqual(1, r.Count);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 4, 0, 4), r[0]);
		}

		[NUnit.Framework.Test]
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
			hd.SetFallbackAlgorithm(MyersDiff<Sequence>.INSTANCE);
			r = hd.Diff(cmp, ac, bc);
			NUnit.Framework.Assert.AreEqual(73, r.Count);
			// But they still differ from Myers due to the way we did early steps.
			EditList myersResult = MyersDiff<Sequence>.INSTANCE.Diff(cmp, ac, bc);
			NUnit.Framework.Assert.IsFalse(myersResult.Equals(r), "Not same as Myers");
		}

		[NUnit.Framework.Test]
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
