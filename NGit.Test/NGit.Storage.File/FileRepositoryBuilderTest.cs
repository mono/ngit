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
using System.IO;
using NGit;
using NGit.Junit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	[NUnit.Framework.TestFixture]
	public class FileRepositoryBuilderTest : LocalDiskRepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestShouldAutomagicallyDetectGitDirectory()
		{
			FileRepository r = CreateWorkRepository();
			FilePath d = new FilePath(r.Directory, "sub-dir");
			FileUtils.Mkdir(d);
			NUnit.Framework.Assert.AreEqual(r.Directory, new FileRepositoryBuilder().FindGitDir
				(d).GetGitDir());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void EmptyRepositoryFormatVersion()
		{
			FileRepository r = CreateWorkRepository();
			FileBasedConfig config = ((FileBasedConfig)r.GetConfig());
			config.SetString(ConfigConstants.CONFIG_CORE_SECTION, null, ConfigConstants.CONFIG_KEY_REPO_FORMAT_VERSION
				, string.Empty);
			config.Save();
			new FileRepository(r.Directory);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void InvalidRepositoryFormatVersion()
		{
			FileRepository r = CreateWorkRepository();
			FileBasedConfig config = ((FileBasedConfig)r.GetConfig());
			config.SetString(ConfigConstants.CONFIG_CORE_SECTION, null, ConfigConstants.CONFIG_KEY_REPO_FORMAT_VERSION
				, "notanumber");
			config.Save();
			try
			{
				new FileRepository(r.Directory);
				NUnit.Framework.Assert.Fail("IllegalArgumentException not thrown");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.IsNotNull(e.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void UnknownRepositoryFormatVersion()
		{
			FileRepository r = CreateWorkRepository();
			FileBasedConfig config = ((FileBasedConfig)r.GetConfig());
			config.SetLong(ConfigConstants.CONFIG_CORE_SECTION, null, ConfigConstants.CONFIG_KEY_REPO_FORMAT_VERSION
				, 1);
			config.Save();
			try
			{
				new FileRepository(r.Directory);
				NUnit.Framework.Assert.Fail("IOException not thrown");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.IsNotNull(e.Message);
			}
		}
	}
}
