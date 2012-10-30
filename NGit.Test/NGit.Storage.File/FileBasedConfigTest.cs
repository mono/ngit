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

using NGit.Storage.File;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Storage.File
{
	[NUnit.Framework.TestFixture]
	public class FileBasedConfigTest
	{
		private static readonly string USER = "user";

		private static readonly string NAME = "name";

		private static readonly string ALICE = "Alice";

		private static readonly string BOB = "Bob";

		private static readonly string CONTENT1 = "[" + USER + "]\n\t" + NAME + " = " + ALICE
			 + "\n";

		private static readonly string CONTENT2 = "[" + USER + "]\n\t" + NAME + " = " + BOB
			 + "\n";

		private readonly FilePath trash = new FilePath(new FilePath("target"), "trash");

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.TearDown]
		public virtual void TearDown()
		{
			FileUtils.Delete(trash, FileUtils.RECURSIVE | FileUtils.SKIP_MISSING);
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSystemEncoding()
		{
			FilePath file = CreateFile(Sharpen.Runtime.GetBytesForString(CONTENT1));
			FileBasedConfig config = new FileBasedConfig(file, FS.DETECTED);
			config.Load();
			NUnit.Framework.Assert.AreEqual(ALICE, config.GetString(USER, null, NAME));
			config.SetString(USER, null, NAME, BOB);
			config.Save();
			Assert.AssertArrayEquals(Sharpen.Runtime.GetBytesForString(CONTENT2), IOUtil.ReadFully
				(file));
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUTF8withoutBOM()
		{
			FilePath file = CreateFile(Sharpen.Runtime.GetBytesForString(CONTENT1, "UTF-8"));
			FileBasedConfig config = new FileBasedConfig(file, FS.DETECTED);
			config.Load();
			NUnit.Framework.Assert.AreEqual(ALICE, config.GetString(USER, null, NAME));
			config.SetString(USER, null, NAME, BOB);
			config.Save();
			Assert.AssertArrayEquals(Sharpen.Runtime.GetBytesForString(CONTENT2), IOUtil.ReadFully
				(file));
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUTF8withBOM()
		{
			ByteArrayOutputStream bos1 = new ByteArrayOutputStream();
			bos1.Write(unchecked((int)(0xEF)));
			bos1.Write(unchecked((int)(0xBB)));
			bos1.Write(unchecked((int)(0xBF)));
			bos1.Write(Sharpen.Runtime.GetBytesForString(CONTENT1, "UTF-8"));
			FilePath file = CreateFile(bos1.ToByteArray());
			FileBasedConfig config = new FileBasedConfig(file, FS.DETECTED);
			config.Load();
			NUnit.Framework.Assert.AreEqual(ALICE, config.GetString(USER, null, NAME));
			config.SetString(USER, null, NAME, BOB);
			config.Save();
			ByteArrayOutputStream bos2 = new ByteArrayOutputStream();
			bos2.Write(unchecked((int)(0xEF)));
			bos2.Write(unchecked((int)(0xBB)));
			bos2.Write(unchecked((int)(0xBF)));
			bos2.Write(Sharpen.Runtime.GetBytesForString(CONTENT2, "UTF-8"));
			Assert.AssertArrayEquals(bos2.ToByteArray(), IOUtil.ReadFully(file));
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLeadingWhitespaces()
		{
			ByteArrayOutputStream bos1 = new ByteArrayOutputStream();
			bos1.Write(Sharpen.Runtime.GetBytesForString(" \n\t"));
			bos1.Write(Sharpen.Runtime.GetBytesForString(CONTENT1));
			FilePath file = CreateFile(bos1.ToByteArray());
			FileBasedConfig config = new FileBasedConfig(file, FS.DETECTED);
			config.Load();
			NUnit.Framework.Assert.AreEqual(ALICE, config.GetString(USER, null, NAME));
			config.SetString(USER, null, NAME, BOB);
			config.Save();
			ByteArrayOutputStream bos2 = new ByteArrayOutputStream();
			bos2.Write(Sharpen.Runtime.GetBytesForString(" \n\t"));
			bos2.Write(Sharpen.Runtime.GetBytesForString(CONTENT2));
			Assert.AssertArrayEquals(bos2.ToByteArray(), IOUtil.ReadFully(file));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private FilePath CreateFile(byte[] content)
		{
			trash.Mkdirs();
			FilePath f = FilePath.CreateTempFile(GetType().FullName, null, trash);
			FileOutputStream os = new FileOutputStream(f, true);
			try
			{
				os.Write(content);
			}
			finally
			{
				os.Close();
			}
			return f;
		}
	}
}
