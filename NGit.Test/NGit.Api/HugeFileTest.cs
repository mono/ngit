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
using NGit;
using NGit.Api;
using Sharpen;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class HugeFileTest : RepositoryTestCase
	{
		private long t = Runtime.CurrentTimeMillis();

		private long lastt;

		private void Measure(string name)
		{
			long c = Runtime.CurrentTimeMillis();
			System.Console.Out.WriteLine(name + ", dt=" + (c - lastt) / 1000.0 + "s");
			lastt = c;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("This test takes FOREVER to run. Can they not fake the filestream?")]
		public virtual void TestAddHugeFile()
		{
			Measure("Commencing test");
			FilePath file = new FilePath(db.WorkTree, "a.txt");
			RandomAccessFile rf = new RandomAccessFile(file, "rw");
			rf.SetLength(4429185024L);
			rf.Close();
			Measure("Created file");
			Git git = new Git(db);
			git.Add().AddFilepattern("a.txt").Call();
			Measure("Added file");
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, length:134217728, sha1:b8cfba97c2b962a44f080b3ca4e03b3204b6a350]"
				, IndexState(LENGTH | CONTENT_ID));
			Status status = git.Status().Call();
			Measure("Status after add");
			AssertCollectionEquals(Arrays.AsList("a.txt"), status.GetAdded());
			NUnit.Framework.Assert.AreEqual(0, status.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetConflicting().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetMissing().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetModified().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetRemoved().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetUntracked().Count);
			// Does not change anything, but modified timestamp
			rf = new RandomAccessFile(file, "rw");
			rf.Write(0);
			rf.Close();
			status = git.Status().Call();
			Measure("Status after non-modifying update");
			AssertCollectionEquals(Arrays.AsList("a.txt"), status.GetAdded());
			NUnit.Framework.Assert.AreEqual(0, status.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetConflicting().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetMissing().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetModified().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetRemoved().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetUntracked().Count);
			// Change something
			rf = new RandomAccessFile(file, "rw");
			rf.Write('a');
			rf.Close();
			status = git.Status().Call();
			Measure("Status after modifying update");
			AssertCollectionEquals(Arrays.AsList("a.txt"), status.GetAdded());
			NUnit.Framework.Assert.AreEqual(0, status.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetConflicting().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetMissing().Count);
			AssertCollectionEquals(Arrays.AsList("a.txt"), status.GetModified());
			NUnit.Framework.Assert.AreEqual(0, status.GetRemoved().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetUntracked().Count);
			// Truncate mod 4G and re-establish equality
			rf = new RandomAccessFile(file, "rw");
			rf.SetLength(134217728L);
			rf.Write(0);
			rf.Close();
			status = git.Status().Call();
			Measure("Status after truncating update");
			AssertCollectionEquals(Arrays.AsList("a.txt"), status.GetAdded());
			NUnit.Framework.Assert.AreEqual(0, status.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetConflicting().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetMissing().Count);
			AssertCollectionEquals(Arrays.AsList("a.txt"), status.GetModified());
			NUnit.Framework.Assert.AreEqual(0, status.GetRemoved().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetUntracked().Count);
			// Change something
			rf = new RandomAccessFile(file, "rw");
			rf.Write('a');
			rf.Close();
			status = git.Status().Call();
			Measure("Status after modifying and truncating update");
			AssertCollectionEquals(Arrays.AsList("a.txt"), status.GetAdded());
			NUnit.Framework.Assert.AreEqual(0, status.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetConflicting().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetMissing().Count);
			AssertCollectionEquals(Arrays.AsList("a.txt"), status.GetModified());
			NUnit.Framework.Assert.AreEqual(0, status.GetRemoved().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetUntracked().Count);
			// Truncate to entry length becomes negative int
			rf = new RandomAccessFile(file, "rw");
			rf.SetLength(3429185024L);
			rf.Write(0);
			rf.Close();
			git.Add().AddFilepattern("a.txt").Call();
			Measure("Added truncated file");
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, length:-865782272, sha1:59b3282f8f59f22d953df956ad3511bf2dc660fd]"
				, IndexState(LENGTH | CONTENT_ID));
			status = git.Status().Call();
			Measure("Status after status on truncated file");
			AssertCollectionEquals(Arrays.AsList("a.txt"), status.GetAdded());
			NUnit.Framework.Assert.AreEqual(0, status.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetConflicting().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetMissing().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetModified().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetRemoved().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetUntracked().Count);
			// Change something
			rf = new RandomAccessFile(file, "rw");
			rf.Write('a');
			rf.Close();
			status = git.Status().Call();
			Measure("Status after modifying and truncating update");
			AssertCollectionEquals(Arrays.AsList("a.txt"), status.GetAdded());
			NUnit.Framework.Assert.AreEqual(0, status.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetConflicting().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetMissing().Count);
			AssertCollectionEquals(Arrays.AsList("a.txt"), status.GetModified());
			NUnit.Framework.Assert.AreEqual(0, status.GetRemoved().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetUntracked().Count);
			git.Commit().SetMessage("make a commit").Call();
			Measure("After commit");
			status = git.Status().Call();
			Measure("After status after commit");
			NUnit.Framework.Assert.AreEqual(0, status.GetAdded().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetConflicting().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetMissing().Count);
			AssertCollectionEquals(Arrays.AsList("a.txt"), status.GetModified());
			NUnit.Framework.Assert.AreEqual(0, status.GetRemoved().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetUntracked().Count);
			git.Reset().SetMode(ResetCommand.ResetType.HARD).Call();
			Measure("After reset --hard");
			NUnit.Framework.Assert.AreEqual("[a.txt, mode:100644, length:-865782272, sha1:59b3282f8f59f22d953df956ad3511bf2dc660fd]"
				, IndexState(LENGTH | CONTENT_ID));
			status = git.Status().Call();
			Measure("Status after hard reset");
			NUnit.Framework.Assert.AreEqual(0, status.GetAdded().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetConflicting().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetMissing().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetModified().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetRemoved().Count);
			NUnit.Framework.Assert.AreEqual(0, status.GetUntracked().Count);
		}

		private void AssertCollectionEquals<_T0, _T1>(ICollection<_T0> asList, ICollection
			<_T1> added)
		{
			NUnit.Framework.Assert.AreEqual(asList.ToString(), added.ToString());
		}

		public HugeFileTest()
		{
			lastt = t;
		}
	}
}
