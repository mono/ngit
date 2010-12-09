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
using NGit.Dircache;
using Sharpen;

namespace NGit.Dircache
{
	[NUnit.Framework.TestFixture]
	public class DirCacheEntryTest
	{
		[NUnit.Framework.Test]
		public virtual void TestIsValidPath()
		{
			NUnit.Framework.Assert.IsTrue(IsValidPath("a"));
			NUnit.Framework.Assert.IsTrue(IsValidPath("a/b"));
			NUnit.Framework.Assert.IsTrue(IsValidPath("ab/cd/ef"));
			NUnit.Framework.Assert.IsFalse(IsValidPath(string.Empty));
			NUnit.Framework.Assert.IsFalse(IsValidPath("/a"));
			NUnit.Framework.Assert.IsFalse(IsValidPath("a//b"));
			NUnit.Framework.Assert.IsFalse(IsValidPath("ab/cd//ef"));
			NUnit.Framework.Assert.IsFalse(IsValidPath("a/"));
			NUnit.Framework.Assert.IsFalse(IsValidPath("ab/cd/ef/"));
			NUnit.Framework.Assert.IsFalse(IsValidPath("a\u0000b"));
		}

		private static bool IsValidPath(string path)
		{
			return DirCacheEntry.IsValidPath(Constants.Encode(path));
		}

		[NUnit.Framework.Test]
		public virtual void TestCreate_ByStringPath()
		{
			NUnit.Framework.Assert.AreEqual("a", new DirCacheEntry("a").PathString);
			NUnit.Framework.Assert.AreEqual("a/b", new DirCacheEntry("a/b").PathString);
			try
			{
				new DirCacheEntry("/a");
				NUnit.Framework.Assert.Fail("Incorrectly created DirCacheEntry");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Invalid path: /a", err.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestCreate_ByStringPathAndStage()
		{
			DirCacheEntry e;
			e = new DirCacheEntry("a", 0);
			NUnit.Framework.Assert.AreEqual("a", e.PathString);
			NUnit.Framework.Assert.AreEqual(0, e.Stage);
			e = new DirCacheEntry("a/b", 1);
			NUnit.Framework.Assert.AreEqual("a/b", e.PathString);
			NUnit.Framework.Assert.AreEqual(1, e.Stage);
			e = new DirCacheEntry("a/c", 2);
			NUnit.Framework.Assert.AreEqual("a/c", e.PathString);
			NUnit.Framework.Assert.AreEqual(2, e.Stage);
			e = new DirCacheEntry("a/d", 3);
			NUnit.Framework.Assert.AreEqual("a/d", e.PathString);
			NUnit.Framework.Assert.AreEqual(3, e.Stage);
			try
			{
				new DirCacheEntry("/a", 1);
				NUnit.Framework.Assert.Fail("Incorrectly created DirCacheEntry");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Invalid path: /a", err.Message);
			}
			try
			{
				new DirCacheEntry("a", -11);
				NUnit.Framework.Assert.Fail("Incorrectly created DirCacheEntry");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Invalid stage -11 for path a", err.Message);
			}
			try
			{
				new DirCacheEntry("a", 4);
				NUnit.Framework.Assert.Fail("Incorrectly created DirCacheEntry");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Invalid stage 4 for path a", err.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestSetFileMode()
		{
			DirCacheEntry e = new DirCacheEntry("a");
			NUnit.Framework.Assert.AreEqual(0, e.RawMode);
			e.FileMode = FileMode.REGULAR_FILE;
			NUnit.Framework.Assert.AreSame(FileMode.REGULAR_FILE, e.FileMode);
			NUnit.Framework.Assert.AreEqual(FileMode.REGULAR_FILE.GetBits(), e.RawMode);
			e.FileMode = FileMode.EXECUTABLE_FILE;
			NUnit.Framework.Assert.AreSame(FileMode.EXECUTABLE_FILE, e.FileMode);
			NUnit.Framework.Assert.AreEqual(FileMode.EXECUTABLE_FILE.GetBits(), e.RawMode);
			e.FileMode = FileMode.SYMLINK;
			NUnit.Framework.Assert.AreSame(FileMode.SYMLINK, e.FileMode);
			NUnit.Framework.Assert.AreEqual(FileMode.SYMLINK.GetBits(), e.RawMode);
			e.FileMode = FileMode.GITLINK;
			NUnit.Framework.Assert.AreSame(FileMode.GITLINK, e.FileMode);
			NUnit.Framework.Assert.AreEqual(FileMode.GITLINK.GetBits(), e.RawMode);
			try
			{
				e.FileMode = FileMode.MISSING;
				NUnit.Framework.Assert.Fail("incorrectly accepted FileMode.MISSING");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Invalid mode 0 for path a", err.Message);
			}
			try
			{
				e.FileMode = FileMode.TREE;
				NUnit.Framework.Assert.Fail("incorrectly accepted FileMode.TREE");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Invalid mode 40000 for path a", err.Message);
			}
		}
	}
}
