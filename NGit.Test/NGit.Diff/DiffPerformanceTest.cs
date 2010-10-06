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

using System.Collections.Generic;
using NGit.Diff;
using NGit.Util;
using Sharpen;

namespace NGit.Diff
{
	/// <summary>Test cases for the performance of the diff implementation.</summary>
	/// <remarks>
	/// Test cases for the performance of the diff implementation. The tests test
	/// that the performance of the MyersDiff algorithm is really O(N*D). Means the
	/// time for computing the diff between a and b should depend on the product of
	/// a.length+b.length and the number of found differences. The tests compute
	/// diffs between chunks of different length, measure the needed time and check
	/// that time/(N*D) does not differ more than a certain factor.
	/// </remarks>
	[NUnit.Framework.TestFixture]
	public class DiffPerformanceTest
	{
/*		private const long longTaskBoundary = 5000000000L;

		private const int minCPUTimerTicks = 10;

		private const int maxFactor = 15;

		private CPUTimeStopWatch stopwatch = CPUTimeStopWatch.CreateInstance();

		public class PerfData
		{
			private NumberFormat fmt = new DecimalFormat("#.##E0");

			public long runningTime;

			public long D;

			public long N;

			private double p1 = -1;

			private double p2 = -1;

			public virtual double Perf1()
			{
				if (this.p1 < 0)
				{
					this.p1 = this.runningTime / ((double)this.N * this.D);
				}
				return this.p1;
			}

			public virtual double Perf2()
			{
				if (this.p2 < 0)
				{
					this.p2 = this.runningTime / ((double)this.N * this.D * this.D);
				}
				return this.p2;
			}

			public override string ToString()
			{
				return ("diffing " + this.N / 2 + " bytes took " + this.runningTime + " ns. N=" +
					 this.N + ", D=" + this.D + ", time/(N*D):" + this.fmt.Format(this.Perf1()) + ", time/(N*D^2):"
					 + this.fmt.Format(this.Perf2()) + "\n");
			}

			internal PerfData(DiffPerformanceTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly DiffPerformanceTest _enclosing;
		}

		public static IComparer<DiffPerformanceTest.PerfData> GetComparator(int whichPerf
			)
		{
			return new _IComparer_108(whichPerf);
		}

		private sealed class _IComparer_108 : IComparer<DiffPerformanceTest.PerfData>
		{
			public _IComparer_108(int whichPerf)
			{
				this.whichPerf = whichPerf;
			}

			public int Compare(DiffPerformanceTest.PerfData o1, DiffPerformanceTest.PerfData 
				o2)
			{
				double p1 = (whichPerf == 1) ? o1.Perf1() : o1.Perf2();
				double p2 = (whichPerf == 1) ? o2.Perf1() : o2.Perf2();
				return (p1 < p2) ? -1 : (p1 > p2) ? 1 : 0;
			}

			private readonly int whichPerf;
		}

		[NUnit.Framework.Test]
		public virtual void Test()
		{
			if (stopwatch != null)
			{
				// run some tests without recording to let JIT do its optimization
				Test(10000);
				Test(20000);
				Test(10000);
				Test(20000);
				IList<DiffPerformanceTest.PerfData> perfData = new List<DiffPerformanceTest.PerfData
					>();
				perfData.AddItem(Test(10000));
				perfData.AddItem(Test(20000));
				perfData.AddItem(Test(40000));
				perfData.AddItem(Test(80000));
				perfData.AddItem(Test(160000));
				perfData.AddItem(Test(320000));
				perfData.AddItem(Test(640000));
				perfData.AddItem(Test(1280000));
				IComparer<DiffPerformanceTest.PerfData> c = GetComparator(1);
				double factor = Sharpen.Collections.Max(perfData, c).Perf1() / Sharpen.Collections
					.Min(perfData, c).Perf1();
				NUnit.Framework.Assert.IsTrue(factor < maxFactor, "minimun and maximum of performance-index t/(N*D) differed too much. Measured factor of "
					 + factor + " (maxFactor=" + maxFactor + "). Perfdata=<" + perfData.ToString() +
					 ">");
			}
		}

		/// <summary>
		/// Tests the performance of MyersDiff for texts which are similar (not
		/// random data).
		/// </summary>
		/// <remarks>
		/// Tests the performance of MyersDiff for texts which are similar (not
		/// random data). The CPU time is measured and returned. Because of bad
		/// accuracy of CPU time information the diffs are repeated. During each
		/// repetition the interim CPU time is checked. The diff operation is
		/// repeated until we have seen the CPU time clock changed its value at least
		/// <see cref="minCPUTimerTicks">minCPUTimerTicks</see>
		/// times.
		/// </remarks>
		/// <param name="characters">the size of the diffed character sequences.</param>
		/// <returns>performance data</returns>
		private DiffPerformanceTest.PerfData Test(int characters)
		{
			DiffPerformanceTest.PerfData ret = new DiffPerformanceTest.PerfData(this);
			string a = DiffTestDataGenerator.GenerateSequence(characters, 971, 3);
			string b = DiffTestDataGenerator.GenerateSequence(characters, 1621, 5);
			DiffPerformanceTest.CharArray ac = new DiffPerformanceTest.CharArray(a);
			DiffPerformanceTest.CharArray bc = new DiffPerformanceTest.CharArray(b);
			DiffPerformanceTest.CharCmp cmp = new DiffPerformanceTest.CharCmp();
			int D = 0;
			int cpuTimeChanges = 0;
			long lastReadout = 0;
			long interimTime = 0;
			int repetitions = 0;
			stopwatch.Start();
			while (cpuTimeChanges < minCPUTimerTicks && interimTime < longTaskBoundary)
			{
				D = MyersDiff.INSTANCE.Diff(cmp, ac, bc).Count;
				repetitions++;
				interimTime = stopwatch.Readout();
				if (interimTime != lastReadout)
				{
					cpuTimeChanges++;
					lastReadout = interimTime;
				}
			}
			ret.runningTime = stopwatch.Stop() / repetitions;
			ret.N = ac.Size() + bc.Size();
			ret.D = D;
			return ret;
		}
		 */
		internal class CharArray : Sequence
		{
			internal readonly char[] array;

			public CharArray(string s)
			{
				array = s.ToCharArray();
			}

			public override int Size()
			{
				return array.Length;
			}
		}

		internal class CharCmp : SequenceComparator<DiffPerformanceTest.CharArray>
		{
			public override bool Equals(DiffPerformanceTest.CharArray a, int ai, DiffPerformanceTest.CharArray
				 b, int bi)
			{
				return a.array[ai] == b.array[bi];
			}

			public override int Hash(DiffPerformanceTest.CharArray seq, int ptr)
			{
				return seq.array[ptr];
			}
		}
	}
}
