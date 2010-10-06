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
using Sharpen;

namespace NGit
{
	public class MergeHeadMsgTest : RepositoryTestCase
	{
		private static readonly string mergeMsg = "merge a and b";

		private static readonly string sampleId = "1c6db447abdbb291b25f07be38ea0b1bf94947c5";

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestReadWriteMergeHeads()
		{
			NUnit.Framework.Assert.AreEqual(db.ReadMergeHeads(), null);
			db.WriteMergeHeads(Arrays.AsList(ObjectId.ZeroId, ObjectId.FromString(sampleId)));
			NUnit.Framework.Assert.AreEqual(Read(new FilePath(db.Directory, "MERGE_HEAD")), "0000000000000000000000000000000000000000\n1c6db447abdbb291b25f07be38ea0b1bf94947c5\n"
				);
			NUnit.Framework.Assert.AreEqual(db.ReadMergeHeads().Count, 2);
			AssertEquals(db.ReadMergeHeads()[0], ObjectId.ZeroId);
			AssertEquals(db.ReadMergeHeads()[1], ObjectId.FromString(sampleId));
			// same test again, this time with lower-level io
			FileOutputStream fos = new FileOutputStream(new FilePath(db.Directory, "MERGE_HEAD"
				));
			try
			{
				fos.Write(Sharpen.Runtime.GetBytesForString("0000000000000000000000000000000000000000\n1c6db447abdbb291b25f07be38ea0b1bf94947c5\n"
					, Constants.CHARACTER_ENCODING));
			}
			finally
			{
				fos.Close();
			}
			NUnit.Framework.Assert.AreEqual(db.ReadMergeHeads().Count, 2);
			AssertEquals(db.ReadMergeHeads()[0], ObjectId.ZeroId);
			AssertEquals(db.ReadMergeHeads()[1], ObjectId.FromString(sampleId));
			db.WriteMergeHeads(Collections.EMPTY_LIST);
			NUnit.Framework.Assert.AreEqual(Read(new FilePath(db.Directory, "MERGE_HEAD")), string.Empty
				);
			NUnit.Framework.Assert.AreEqual(db.ReadMergeHeads(), null);
			fos = new FileOutputStream(new FilePath(db.Directory, "MERGE_HEAD"));
			try
			{
				fos.Write(Sharpen.Runtime.GetBytesForString(sampleId, Constants.CHARACTER_ENCODING
					));
			}
			finally
			{
				fos.Close();
			}
			NUnit.Framework.Assert.AreEqual(db.ReadMergeHeads().Count, 1);
			AssertEquals(db.ReadMergeHeads()[0], ObjectId.FromString(sampleId));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestReadWriteMergeMsg()
		{
			NUnit.Framework.Assert.AreEqual(db.ReadMergeCommitMsg(), null);
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "MERGE_MSG").Exists());
			db.WriteMergeCommitMsg(mergeMsg);
			NUnit.Framework.Assert.AreEqual(db.ReadMergeCommitMsg(), mergeMsg);
			NUnit.Framework.Assert.AreEqual(Read(new FilePath(db.Directory, "MERGE_MSG")), mergeMsg
				);
			db.WriteMergeCommitMsg(null);
			NUnit.Framework.Assert.AreEqual(db.ReadMergeCommitMsg(), null);
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "MERGE_MSG").Exists());
			FileOutputStream fos = new FileOutputStream(new FilePath(db.Directory, Constants.
				MERGE_MSG));
			try
			{
				fos.Write(Sharpen.Runtime.GetBytesForString(mergeMsg, Constants.CHARACTER_ENCODING
					));
			}
			finally
			{
				fos.Close();
			}
			NUnit.Framework.Assert.AreEqual(db.ReadMergeCommitMsg(), mergeMsg);
		}
	}
}
