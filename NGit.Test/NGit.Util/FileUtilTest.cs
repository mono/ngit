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

using System.IO;
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	[NUnit.Framework.TestFixture]
	public class FileUtilTest
	{
		private readonly FilePath trash = new FilePath(new FilePath("target"), "trash");

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			NUnit.Framework.Assert.IsTrue(trash.Mkdirs());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.TearDown]
		public virtual void TearDown()
		{
			FileUtils.Delete(trash, FileUtils.RECURSIVE | FileUtils.RETRY);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteFile()
		{
			FilePath f = new FilePath(trash, "test");
			NUnit.Framework.Assert.IsTrue(f.CreateNewFile());
			FileUtils.Delete(f);
			NUnit.Framework.Assert.IsFalse(f.Exists());
			try
			{
				FileUtils.Delete(f);
				NUnit.Framework.Assert.Fail("deletion of non-existing file must fail");
			}
			catch (IOException)
			{
			}
			// expected
			try
			{
				FileUtils.Delete(f, FileUtils.SKIP_MISSING);
			}
			catch (IOException)
			{
				NUnit.Framework.Assert.Fail("deletion of non-existing file must not fail with option SKIP_MISSING"
					);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteRecursive()
		{
			FilePath f1 = new FilePath(trash, "test/test/a");
			f1.Mkdirs();
			f1.CreateNewFile();
			FilePath f2 = new FilePath(trash, "test/test/b");
			f2.CreateNewFile();
			FilePath d = new FilePath(trash, "test");
			FileUtils.Delete(d, FileUtils.RECURSIVE);
			NUnit.Framework.Assert.IsFalse(d.Exists());
			try
			{
				FileUtils.Delete(d, FileUtils.RECURSIVE);
				NUnit.Framework.Assert.Fail("recursive deletion of non-existing directory must fail"
					);
			}
			catch (IOException)
			{
			}
			// expected
			try
			{
				FileUtils.Delete(d, FileUtils.RECURSIVE | FileUtils.SKIP_MISSING);
			}
			catch (IOException)
			{
				NUnit.Framework.Assert.Fail("recursive deletion of non-existing directory must not fail with option SKIP_MISSING"
					);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMkdir()
		{
			FilePath d = new FilePath(trash, "test");
			FileUtils.Mkdir(d);
			NUnit.Framework.Assert.IsTrue(d.Exists() && d.IsDirectory());
			try
			{
				FileUtils.Mkdir(d);
				NUnit.Framework.Assert.Fail("creation of existing directory must fail");
			}
			catch (IOException)
			{
			}
			// expected
			FileUtils.Mkdir(d, true);
			NUnit.Framework.Assert.IsTrue(d.Exists() && d.IsDirectory());
			NUnit.Framework.Assert.IsTrue(d.Delete());
			FilePath f = new FilePath(trash, "test");
			NUnit.Framework.Assert.IsTrue(f.CreateNewFile());
			try
			{
				FileUtils.Mkdir(d);
				NUnit.Framework.Assert.Fail("creation of directory having same path as existing file must"
					 + " fail");
			}
			catch (IOException)
			{
			}
			// expected
			NUnit.Framework.Assert.IsTrue(f.Delete());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMkdirs()
		{
			FilePath root = new FilePath(trash, "test");
			NUnit.Framework.Assert.IsTrue(root.Mkdir());
			FilePath d = new FilePath(root, "test/test");
			FileUtils.Mkdirs(d);
			NUnit.Framework.Assert.IsTrue(d.Exists() && d.IsDirectory());
			try
			{
				FileUtils.Mkdirs(d);
				NUnit.Framework.Assert.Fail("creation of existing directory hierarchy must fail");
			}
			catch (IOException)
			{
			}
			// expected
			FileUtils.Mkdirs(d, true);
			NUnit.Framework.Assert.IsTrue(d.Exists() && d.IsDirectory());
			FileUtils.Delete(root, FileUtils.RECURSIVE);
			FilePath f = new FilePath(trash, "test");
			NUnit.Framework.Assert.IsTrue(f.CreateNewFile());
			try
			{
				FileUtils.Mkdirs(d);
				NUnit.Framework.Assert.Fail("creation of directory having path conflicting with existing"
					 + " file must fail");
			}
			catch (IOException)
			{
			}
			// expected
			NUnit.Framework.Assert.IsTrue(f.Delete());
		}
	}
}
