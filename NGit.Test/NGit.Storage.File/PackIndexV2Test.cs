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
using NGit.Junit;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	[NUnit.Framework.TestFixture]
	public class PackIndexV2Test : PackIndexTestCase
	{
		public override FilePath GetFileForPack34be9032()
		{
			return JGitTestUtil.GetTestResourceFile("pack-34be9032ac282b11fa9babdc2b2a93ca996c9c2f.idxV2"
				);
		}

		public override FilePath GetFileForPackdf2982f28()
		{
			return JGitTestUtil.GetTestResourceFile("pack-df2982f284bbabb6bdb59ee3fcc6eb0983e20371.idxV2"
				);
		}

		/// <summary>Verify CRC32 indexing.</summary>
		/// <remarks>Verify CRC32 indexing.</remarks>
		/// <exception cref="System.NotSupportedException">System.NotSupportedException</exception>
		/// <exception cref="NGit.Errors.MissingObjectException">NGit.Errors.MissingObjectException
		/// 	</exception>
		[NUnit.Framework.Test]
		public override void TestCRC32()
		{
			NUnit.Framework.Assert.IsTrue(smallIdx.HasCRC32Support());
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x00000000C2B64258L)), smallIdx.
				FindCRC32(ObjectId.FromString("4b825dc642cb6eb9a060e54bf8d69288fbee4904")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x0000000072AD57C2L)), smallIdx.
				FindCRC32(ObjectId.FromString("540a36d136cf413e4b064c2b0e0a4db60f77feab")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x00000000FF10A479L)), smallIdx.
				FindCRC32(ObjectId.FromString("5b6e7c66c276e7610d4a73c70ec1a1f7c1003259")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x0000000034B27DDCL)), smallIdx.
				FindCRC32(ObjectId.FromString("6ff87c4664981e4397625791c8ea3bbb5f2279a3")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x000000004743F1E4L)), smallIdx.
				FindCRC32(ObjectId.FromString("82c6b885ff600be425b4ea96dee75dca255b69e7")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x00000000640B358BL)), smallIdx.
				FindCRC32(ObjectId.FromString("902d5476fa249b7abc9d84c611577a81381f0327")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x000000002A17CB5EL)), smallIdx.
				FindCRC32(ObjectId.FromString("aabf2ffaec9b497f0950352b3e582d73035c2035")));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x000000000B3B5BA6L)), smallIdx.
				FindCRC32(ObjectId.FromString("c59759f143fb1fe21c197981df75a7ee00290799")));
		}
	}
}
