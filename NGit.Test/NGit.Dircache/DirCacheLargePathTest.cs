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

using System.Text;
using NGit;
using NGit.Dircache;
using Sharpen;

namespace NGit.Dircache
{
	public class DirCacheLargePathTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPath_4090()
		{
			TestLongPath(4090);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPath_4094()
		{
			TestLongPath(4094);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPath_4095()
		{
			TestLongPath(4095);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPath_4096()
		{
			TestLongPath(4096);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPath_16384()
		{
			TestLongPath(16384);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private void TestLongPath(int len)
		{
			string longPath = MakeLongPath(len);
			string shortPath = "~~~ shorter-path";
			DirCacheEntry longEnt = new DirCacheEntry(longPath);
			DirCacheEntry shortEnt = new DirCacheEntry(shortPath);
			longEnt.FileMode = FileMode.REGULAR_FILE;
			shortEnt.FileMode = FileMode.REGULAR_FILE;
			NUnit.Framework.Assert.AreEqual(longPath, longEnt.PathString);
			NUnit.Framework.Assert.AreEqual(shortPath, shortEnt.PathString);
			{
				DirCache dc1 = db.LockDirCache();
				{
					DirCacheBuilder b = dc1.Builder();
					b.Add(longEnt);
					b.Add(shortEnt);
					NUnit.Framework.Assert.IsTrue(b.Commit());
				}
				NUnit.Framework.Assert.AreEqual(2, dc1.GetEntryCount());
				NUnit.Framework.Assert.AreSame(longEnt, dc1.GetEntry(0));
				NUnit.Framework.Assert.AreSame(shortEnt, dc1.GetEntry(1));
			}
			{
				DirCache dc2 = db.ReadDirCache();
				NUnit.Framework.Assert.AreEqual(2, dc2.GetEntryCount());
				NUnit.Framework.Assert.AreNotSame(longEnt, dc2.GetEntry(0));
				NUnit.Framework.Assert.AreEqual(longPath, dc2.GetEntry(0).PathString);
				NUnit.Framework.Assert.AreNotSame(shortEnt, dc2.GetEntry(1));
				NUnit.Framework.Assert.AreEqual(shortPath, dc2.GetEntry(1).PathString);
			}
		}

		private static string MakeLongPath(int len)
		{
			StringBuilder r = new StringBuilder(len);
			for (int i = 0; i < len; i++)
			{
				r.Append('a' + (i % 26));
			}
			return r.ToString();
		}
	}
}
