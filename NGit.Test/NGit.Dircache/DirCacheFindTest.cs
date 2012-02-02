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
using NGit.Dircache;
using Sharpen;

namespace NGit.Dircache
{
	[NUnit.Framework.TestFixture]
	public class DirCacheFindTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEntriesWithin()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a/b", "a/c", "a/d", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].FileMode = FileMode.REGULAR_FILE;
			}
			int aFirst = 1;
			int aLast = 3;
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			NUnit.Framework.Assert.AreEqual(paths.Length, dc.GetEntryCount());
			for (int i_2 = 0; i_2 < ents.Length; i_2++)
			{
				NUnit.Framework.Assert.AreSame(ents[i_2], dc.GetEntry(i_2));
			}
			{
				DirCacheEntry[] aContents = dc.GetEntriesWithin("a");
				NUnit.Framework.Assert.IsNotNull(aContents);
				NUnit.Framework.Assert.AreEqual(aLast - aFirst + 1, aContents.Length);
				for (int i_3 = aFirst, j=0; i_3 <= aLast; i_3++, j++)
				{
					NUnit.Framework.Assert.AreSame(ents[i_3], aContents[j]);
				}
			}
			{
				DirCacheEntry[] aContents = dc.GetEntriesWithin("a/");
				NUnit.Framework.Assert.IsNotNull(aContents);
				NUnit.Framework.Assert.AreEqual(aLast - aFirst + 1, aContents.Length);
				for (int i_3 = aFirst, j=0; i_3 <= aLast; i_3++, j++)
				{
					NUnit.Framework.Assert.AreSame(ents[i_3], aContents[j]);
				}
			}
			{
				DirCacheEntry[] aContents = dc.GetEntriesWithin(string.Empty);
				NUnit.Framework.Assert.IsNotNull(aContents);
				NUnit.Framework.Assert.AreEqual(ents.Length, aContents.Length);
				for (int i_3 = 0; i_3 < ents.Length; i_3++)
				{
					NUnit.Framework.Assert.AreSame(ents[i_3], aContents[i_3]);
				}
			}
			NUnit.Framework.Assert.IsNotNull(dc.GetEntriesWithin("a."));
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntriesWithin("a.").Length);
			NUnit.Framework.Assert.IsNotNull(dc.GetEntriesWithin("a0b"));
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntriesWithin("a0b.").Length);
			NUnit.Framework.Assert.IsNotNull(dc.GetEntriesWithin("zoo"));
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntriesWithin("zoo.").Length);
		}
	}
}
