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
