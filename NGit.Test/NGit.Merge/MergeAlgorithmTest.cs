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

using NGit;
using NGit.Diff;
using NGit.Merge;
using Sharpen;

namespace NGit.Merge
{
	[NUnit.Framework.TestFixture]
	public class MergeAlgorithmTest
	{
		internal MergeFormatter fmt = new MergeFormatter();

		private static readonly string A = "aaa\n";

		private static readonly string B = "bbbbb\nbb\nbbb\n";

		private static readonly string C = "c\n";

		private static readonly string D = "dd\n";

		private static readonly string E = "ee\n";

		private static readonly string F = "fff\nff\n";

		private static readonly string G = "gg\n";

		private static readonly string H = "h\nhhh\nhh\n";

		private static readonly string I = "iiii\n";

		private static readonly string J = "jj\n";

		private static readonly string Z = "zzz\n";

		private static readonly string Y = "y\n";

		private static readonly string XXX_0 = "<<<<<<< O\n";

		private static readonly string XXX_1 = "=======\n";

		private static readonly string XXX_2 = ">>>>>>> T\n";

		internal string @base = A + B + C + D + E + F + G + H + I + J;

		internal string replace_C_by_Z = A + B + Z + D + E + F + G + H + I + J;

		internal string replace_A_by_Y = Y + B + C + D + E + F + G + H + I + J;

		internal string replace_A_by_Z = Z + B + C + D + E + F + G + H + I + J;

		internal string replace_J_by_Y = A + B + C + D + E + F + G + H + I + Y;

		internal string replace_J_by_Z = A + B + C + D + E + F + G + H + I + Z;

		internal string replace_BC_by_ZZ = A + Z + Z + D + E + F + G + H + I + J;

		internal string replace_BCD_by_ZZZ = A + Z + Z + Z + E + F + G + H + I + J;

		internal string replace_BD_by_ZZ = A + Z + C + Z + E + F + G + H + I + J;

		internal string replace_BCDEGI_by_ZZZZZZ = A + Z + Z + Z + Z + F + Z + H + Z + J;

		internal string replace_CEFGHJ_by_YYYYYY = A + B + Y + D + Y + Y + Y + Y + I + Y;

		internal string replace_BDE_by_ZZY = A + Z + C + Z + Y + F + G + H + I + J;

		// the texts which are used in this merge-tests are constructed by
		// concatenating fixed chunks of text defined by the String constants
		// A..Y. The common base text is always the text A+B+C+D+E+F+G+H+I+J.
		// The two texts being merged are constructed by deleting some chunks
		// or inserting new chunks. Some of the chunks are one-liners, others
		// contain more than one line.
		// constants which define how conflict-regions are expected to be reported.
		// the common base from which all merges texts derive from
		// the following constants define the merged texts. The name of the
		// constants describe how they are created out of the common base. E.g.
		// the constant named replace_XYZ_by_MNO stands for the text which is
		// created from common base by replacing first chunk X by chunk M, then
		// Y by N and then Z by O.
		/// <summary>
		/// Check for a conflict where the second text was changed similar to the
		/// first one, but the second texts modification covers one more line.
		/// </summary>
		/// <remarks>
		/// Check for a conflict where the second text was changed similar to the
		/// first one, but the second texts modification covers one more line.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoConflictingModifications()
		{
			NUnit.Framework.Assert.AreEqual(A + XXX_0 + B + XXX_1 + Z + XXX_2 + Z + D + E + F
				 + G + H + I + J, Merge(@base, replace_C_by_Z, replace_BC_by_ZZ));
		}

		/// <summary>Test a case where we have three consecutive chunks.</summary>
		/// <remarks>
		/// Test a case where we have three consecutive chunks. The first text
		/// modifies all three chunks. The second text modifies the first and the
		/// last chunk. This should be reported as one conflicting region.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestOneAgainstTwoConflictingModifications()
		{
			NUnit.Framework.Assert.AreEqual(A + Z + XXX_0 + Z + XXX_1 + C + XXX_2 + Z + E + F
				 + G + H + I + J, Merge(@base, replace_BCD_by_ZZZ, replace_BD_by_ZZ));
		}

		/// <summary>Test a merge where only the second text contains modifications.</summary>
		/// <remarks>
		/// Test a merge where only the second text contains modifications. Expect as
		/// merge result the second text.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestNoAgainstOneModification()
		{
			NUnit.Framework.Assert.AreEqual(replace_BD_by_ZZ.ToString(), Merge(@base, @base, 
				replace_BD_by_ZZ));
		}

		/// <summary>Both texts contain modifications but not on the same chunks.</summary>
		/// <remarks>
		/// Both texts contain modifications but not on the same chunks. Expect a
		/// non-conflict merge result.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoNonConflictingModifications()
		{
			NUnit.Framework.Assert.AreEqual(Y + B + Z + D + E + F + G + H + I + J, Merge(@base
				, replace_C_by_Z, replace_A_by_Y));
		}

		/// <summary>Merge two complicated modifications.</summary>
		/// <remarks>
		/// Merge two complicated modifications. The merge algorithm has to extend
		/// and combine conflicting regions to get to the expected merge result.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestTwoComplicatedModifications()
		{
			NUnit.Framework.Assert.AreEqual(A + XXX_0 + Z + Z + Z + Z + F + Z + H + XXX_1 + B
				 + Y + D + Y + Y + Y + Y + XXX_2 + Z + Y, Merge(@base, replace_BCDEGI_by_ZZZZZZ, 
				replace_CEFGHJ_by_YYYYYY));
		}

		/// <summary>Test a conflicting region at the very start of the text.</summary>
		/// <remarks>Test a conflicting region at the very start of the text.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestConflictAtStart()
		{
			NUnit.Framework.Assert.AreEqual(XXX_0 + Z + XXX_1 + Y + XXX_2 + B + C + D + E + F
				 + G + H + I + J, Merge(@base, replace_A_by_Z, replace_A_by_Y));
		}

		/// <summary>Test a conflicting region at the very end of the text.</summary>
		/// <remarks>Test a conflicting region at the very end of the text.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestConflictAtEnd()
		{
			NUnit.Framework.Assert.AreEqual(A + B + C + D + E + F + G + H + I + XXX_0 + Z + XXX_1
				 + Y + XXX_2, Merge(@base, replace_J_by_Z, replace_J_by_Y));
		}

		/// <summary>
		/// Check for a conflict where the second text was changed similar to the
		/// first one, but the second texts modification covers one more line.
		/// </summary>
		/// <remarks>
		/// Check for a conflict where the second text was changed similar to the
		/// first one, but the second texts modification covers one more line.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestSameModification()
		{
			NUnit.Framework.Assert.AreEqual(replace_C_by_Z, Merge(@base, replace_C_by_Z, replace_C_by_Z
				));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private string Merge(string commonBase, string ours, string theirs)
		{
			MergeResult<RawText> r = MergeAlgorithm.Merge(RawTextComparator.DEFAULT, new RawText(Constants
				.Encode(commonBase)), new RawText(Constants.Encode(ours)), new RawText(Constants
				.Encode(theirs)));
			ByteArrayOutputStream bo = new ByteArrayOutputStream(50);
			fmt.FormatMerge(bo, r, "B", "O", "T", Constants.CHARACTER_ENCODING);
			return Sharpen.Extensions.CreateString(bo.ToByteArray(), Constants.CHARACTER_ENCODING
				);
		}
	}
}
