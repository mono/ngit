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
using NGit.Storage.File;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Storage.File
{
	[NUnit.Framework.TestFixture]
	public class FileSnapshotTest
	{
		private IList<FilePath> files = new AList<FilePath>();

		private readonly FilePath trash = new FilePath(new FilePath("target"), "trash");

		/// <exception cref="System.Exception"></exception>
		[SetUp]
		[NUnit.Framework.TearDown]
		public virtual void TearDown()
		{
			FileUtils.Delete(trash, FileUtils.RECURSIVE | FileUtils.SKIP_MISSING);
		}

		private void WaitNextSec(FilePath f)
		{
			long initialLastModified = f.LastModified();
			do
			{
				f.SetLastModified(Runtime.CurrentTimeMillis());
			}
			while (f.LastModified() == initialLastModified);
		}

		/// <summary>Change data and time stamp.</summary>
		/// <remarks>Change data and time stamp.</remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestActuallyIsModifiedTrivial()
		{
			FilePath f1 = CreateFile("simple");
			WaitNextSec(f1);
			FileSnapshot save = FileSnapshot.Save(f1);
			Append(f1, unchecked((byte)'x'));
			WaitNextSec(f1);
			NUnit.Framework.Assert.IsTrue(save.IsModified(f1));
		}

		/// <summary>Create a file, wait long enough and verify that it has not been modified.
		/// 	</summary>
		/// <remarks>
		/// Create a file, wait long enough and verify that it has not been modified.
		/// 3.5 seconds mean any difference between file system timestamp and system
		/// clock should be significant.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestOldFile()
		{
			FilePath f1 = CreateFile("oldfile");
			WaitNextSec(f1);
			FileSnapshot save = FileSnapshot.Save(f1);
			Sharpen.Thread.Sleep(3500);
			NUnit.Framework.Assert.IsFalse(save.IsModified(f1));
		}

		/// <summary>
		/// Create a file, but don't wait long enough for the difference between file
		/// system clock and system clock to be significant.
		/// </summary>
		/// <remarks>
		/// Create a file, but don't wait long enough for the difference between file
		/// system clock and system clock to be significant. Assume the file may have
		/// been modified. It may have been, but the clock alone cannot determine
		/// this
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestNewFileWithWait()
		{
			FilePath f1 = CreateFile("newfile");
			WaitNextSec(f1);
			FileSnapshot save = FileSnapshot.Save(f1);
			Sharpen.Thread.Sleep(1500);
			NUnit.Framework.Assert.IsTrue(save.IsModified(f1));
		}

		/// <summary>
		/// Same as
		/// <see cref="TestNewFileWithWait()">TestNewFileWithWait()</see>
		/// but do not wait at all
		/// </summary>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestNewFileNoWait()
		{
			FilePath f1 = CreateFile("newfile");
			WaitNextSec(f1);
			FileSnapshot save = FileSnapshot.Save(f1);
			Sharpen.Thread.Sleep(1500);
			NUnit.Framework.Assert.IsTrue(save.IsModified(f1));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private FilePath CreateFile(string @string)
		{
			trash.Mkdirs();
			FilePath f = FilePath.CreateTempFile(@string, "tdat", trash);
			files.AddItem(f);
			return f;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Append(FilePath f, byte b)
		{
			FileOutputStream os = new FileOutputStream(f, true);
			try
			{
				os.Write(b);
			}
			finally
			{
				os.Close();
			}
		}
	}
}
