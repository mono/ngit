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
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	public abstract class PackIndexTestCase : RepositoryTestCase
	{
		internal PackIndex smallIdx;

		internal PackIndex denseIdx;

		/// <exception cref="System.Exception"></exception>
		public override void SetUp()
		{
			base.SetUp();
			smallIdx = PackIndex.Open(GetFileForPack34be9032());
			denseIdx = PackIndex.Open(GetFileForPackdf2982f28());
		}

		/// <summary>Return file with appropriate index version for prepared pack.</summary>
		/// <remarks>Return file with appropriate index version for prepared pack.</remarks>
		/// <returns>file with index</returns>
		public abstract FilePath GetFileForPack34be9032();

		/// <summary>Return file with appropriate index version for prepared pack.</summary>
		/// <remarks>Return file with appropriate index version for prepared pack.</remarks>
		/// <returns>file with index</returns>
		public abstract FilePath GetFileForPackdf2982f28();

		/// <summary>Verify CRC32 support.</summary>
		/// <remarks>Verify CRC32 support.</remarks>
		/// <exception cref="NGit.Errors.MissingObjectException">NGit.Errors.MissingObjectException
		/// 	</exception>
		/// <exception cref="System.NotSupportedException">System.NotSupportedException</exception>
		public abstract void TestCRC32();

		/// <summary>
		/// Test contracts of Iterator methods and this implementation remove()
		/// limitations.
		/// </summary>
		/// <remarks>
		/// Test contracts of Iterator methods and this implementation remove()
		/// limitations.
		/// </remarks>
		[NUnit.Framework.Test]
		public virtual void TestIteratorMethodsContract()
		{
			Iterator<PackIndex.MutableEntry> iter = smallIdx.Iterator();
			while (iter.HasNext())
			{
				iter.Next();
			}
			try
			{
				iter.Next();
				NUnit.Framework.Assert.Fail("next() unexpectedly returned element");
			}
			catch (NoSuchElementException)
			{
			}
			// expected
			try
			{
				iter.Remove();
				NUnit.Framework.Assert.Fail("remove() shouldn't be implemented");
			}
			catch (NotSupportedException)
			{
			}
		}

		// expected
		/// <summary>
		/// Test results of iterator comparing to content of well-known (prepared)
		/// small index.
		/// </summary>
		/// <remarks>
		/// Test results of iterator comparing to content of well-known (prepared)
		/// small index.
		/// </remarks>
		[NUnit.Framework.Test]
		public virtual void TestIteratorReturnedValues1()
		{
			Iterator<PackIndex.MutableEntry> iter = smallIdx.Iterator();
			NUnit.Framework.Assert.AreEqual("4b825dc642cb6eb9a060e54bf8d69288fbee4904", iter.
				Next().Name());
			NUnit.Framework.Assert.AreEqual("540a36d136cf413e4b064c2b0e0a4db60f77feab", iter.
				Next().Name());
			NUnit.Framework.Assert.AreEqual("5b6e7c66c276e7610d4a73c70ec1a1f7c1003259", iter.
				Next().Name());
			NUnit.Framework.Assert.AreEqual("6ff87c4664981e4397625791c8ea3bbb5f2279a3", iter.
				Next().Name());
			NUnit.Framework.Assert.AreEqual("82c6b885ff600be425b4ea96dee75dca255b69e7", iter.
				Next().Name());
			NUnit.Framework.Assert.AreEqual("902d5476fa249b7abc9d84c611577a81381f0327", iter.
				Next().Name());
			NUnit.Framework.Assert.AreEqual("aabf2ffaec9b497f0950352b3e582d73035c2035", iter.
				Next().Name());
			NUnit.Framework.Assert.AreEqual("c59759f143fb1fe21c197981df75a7ee00290799", iter.
				Next().Name());
			NUnit.Framework.Assert.IsFalse(iter.HasNext());
		}

		/// <summary>Compare offset from iterator entries with output of findOffset() method.
		/// 	</summary>
		/// <remarks>Compare offset from iterator entries with output of findOffset() method.
		/// 	</remarks>
		[NUnit.Framework.Test]
		public virtual void TestCompareEntriesOffsetsWithFindOffsets()
		{
			foreach (PackIndex.MutableEntry me in smallIdx)
			{
				NUnit.Framework.Assert.AreEqual(smallIdx.FindOffset(me.ToObjectId()), me.GetOffset
					());
			}
			foreach (PackIndex.MutableEntry me_1 in denseIdx)
			{
				NUnit.Framework.Assert.AreEqual(denseIdx.FindOffset(me_1.ToObjectId()), me_1.GetOffset
					());
			}
		}

		/// <summary>
		/// Test partial results of iterator comparing to content of well-known
		/// (prepared) dense index, that may need multi-level indexing.
		/// </summary>
		/// <remarks>
		/// Test partial results of iterator comparing to content of well-known
		/// (prepared) dense index, that may need multi-level indexing.
		/// </remarks>
		[NUnit.Framework.Test]
		public virtual void TestIteratorReturnedValues2()
		{
			Iterator<PackIndex.MutableEntry> iter = denseIdx.Iterator();
			while (!iter.Next().Name().Equals("0a3d7772488b6b106fb62813c4d6d627918d9181"))
			{
			}
			// just iterating
			NUnit.Framework.Assert.AreEqual("1004d0d7ac26fbf63050a234c9b88a46075719d3", iter.
				Next().Name());
			// same level-1
			NUnit.Framework.Assert.AreEqual("10da5895682013006950e7da534b705252b03be6", iter.
				Next().Name());
			// same level-1
			NUnit.Framework.Assert.AreEqual("1203b03dc816ccbb67773f28b3c19318654b0bc8", iter.
				Next().Name());
		}
	}
}
