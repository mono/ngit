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
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	public class WindowCacheReconfigureTest : RepositoryTestCase
	{
		public virtual void TestConfigureCache_PackedGitLimit_0()
		{
			try
			{
				WindowCacheConfig cfg = new WindowCacheConfig();
				cfg.SetPackedGitLimit(0);
				WindowCache.Reconfigure(cfg);
				NUnit.Framework.Assert.Fail("incorrectly permitted PackedGitLimit = 0");
			}
			catch (ArgumentException)
			{
			}
		}

		//
		public virtual void TestConfigureCache_PackedGitWindowSize_0()
		{
			try
			{
				WindowCacheConfig cfg = new WindowCacheConfig();
				cfg.SetPackedGitWindowSize(0);
				WindowCache.Reconfigure(cfg);
				NUnit.Framework.Assert.Fail("incorrectly permitted PackedGitWindowSize = 0");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("Invalid window size", e.Message);
			}
		}

		public virtual void TestConfigureCache_PackedGitWindowSize_512()
		{
			try
			{
				WindowCacheConfig cfg = new WindowCacheConfig();
				cfg.SetPackedGitWindowSize(512);
				WindowCache.Reconfigure(cfg);
				NUnit.Framework.Assert.Fail("incorrectly permitted PackedGitWindowSize = 512");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("Invalid window size", e.Message);
			}
		}

		public virtual void TestConfigureCache_PackedGitWindowSize_4097()
		{
			try
			{
				WindowCacheConfig cfg = new WindowCacheConfig();
				cfg.SetPackedGitWindowSize(4097);
				WindowCache.Reconfigure(cfg);
				NUnit.Framework.Assert.Fail("incorrectly permitted PackedGitWindowSize = 4097");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("Window size must be power of 2", e.Message);
			}
		}

		public virtual void TestConfigureCache_PackedGitOpenFiles_0()
		{
			try
			{
				WindowCacheConfig cfg = new WindowCacheConfig();
				cfg.SetPackedGitOpenFiles(0);
				WindowCache.Reconfigure(cfg);
				NUnit.Framework.Assert.Fail("incorrectly permitted PackedGitOpenFiles = 0");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("Open files must be >= 1", e.Message);
			}
		}

		public virtual void TestConfigureCache_PackedGitWindowSizeAbovePackedGitLimit()
		{
			try
			{
				WindowCacheConfig cfg = new WindowCacheConfig();
				cfg.SetPackedGitLimit(1024);
				cfg.SetPackedGitWindowSize(8192);
				WindowCache.Reconfigure(cfg);
				NUnit.Framework.Assert.Fail("incorrectly permitted PackedGitWindowSize > PackedGitLimit"
					);
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("Window size must be < limit", e.Message);
			}
		}

		public virtual void TestConfigureCache_Limits1()
		{
			// This test is just to force coverage over some lower bounds for
			// the table. We don't want the table to wind up with too small
			// of a size. This is highly dependent upon the table allocation
			// algorithm actually implemented in WindowCache.
			//
			WindowCacheConfig cfg = new WindowCacheConfig();
			cfg.SetPackedGitLimit(6 * 4096 / 5);
			cfg.SetPackedGitWindowSize(4096);
			WindowCache.Reconfigure(cfg);
		}
	}
}
