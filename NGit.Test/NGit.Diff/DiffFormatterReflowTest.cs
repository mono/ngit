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
using System.Linq;
using NGit.Diff;
using NGit.Junit;
using NGit.Patch;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Diff
{
	[NUnit.Framework.TestFixture]
	public class DiffFormatterReflowTest
	{
		private RawText a;

		private RawText b;

		private FileHeader file;

		private ByteArrayOutputStream @out;

		private DiffFormatter fmt;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			@out = new ByteArrayOutputStream();
			fmt = new DiffFormatter(@out);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNegativeContextFails()
		{
			Init("X");
			try
			{
				fmt.SetContext(-1);
				NUnit.Framework.Assert.Fail("accepted negative context");
			}
			catch (ArgumentException)
			{
			}
		}

		// pass
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestContext0()
		{
			Init("X");
			fmt.SetContext(0);
			AssertFormatted();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestContext1()
		{
			Init("X");
			fmt.SetContext(1);
			AssertFormatted();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestContext3()
		{
			Init("X");
			fmt.SetContext(3);
			AssertFormatted();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestContext5()
		{
			Init("X");
			fmt.SetContext(5);
			AssertFormatted();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestContext10()
		{
			Init("X");
			fmt.SetContext(10);
			AssertFormatted();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestContext100()
		{
			Init("X");
			fmt.SetContext(100);
			AssertFormatted();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmpty1()
		{
			Init("E");
			AssertFormatted("E.patch");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNoNewLine1()
		{
			Init("Y");
			AssertFormatted("Y.patch");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNoNewLine2()
		{
			Init("Z");
			AssertFormatted("Z.patch");
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Init(string name)
		{
			a = new RawText(ReadFile(name + "_PreImage"));
			b = new RawText(ReadFile(name + "_PostImage"));
			file = ParseTestPatchFile(name + ".patch").GetFiles()[0];
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertFormatted()
		{
			AssertFormatted(GetTestName() + ".out");
		}

	    private static string GetTestName()
	    {
	        var testName = TestContext.CurrentContext.Test.Name;
	        var firstChar = testName.First().ToString();
	        return string.Format("{0}{1}", firstChar.ToLower(), testName.Substring(1));
	    }

	    /// <exception cref="System.IO.IOException"></exception>
		private void AssertFormatted(string name)
		{
			fmt.Format(file, a, b);
			string exp = RawParseUtils.Decode(ReadFile(name));
			NUnit.Framework.Assert.AreEqual(exp, RawParseUtils.Decode(@out.ToByteArray()));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private byte[] ReadFile(string patchFile)
		{
			InputStream @in = GetType().GetResourceAsStream(patchFile);
			if (@in == null)
			{
				NUnit.Framework.Assert.Fail("No " + patchFile + " test vector");
				return null;
			}
			// Never happens
			try
			{
				byte[] buf = new byte[1024];
				ByteArrayOutputStream temp = new ByteArrayOutputStream();
				int n;
				while ((n = @in.Read(buf)) > 0)
				{
					temp.Write(buf, 0, n);
				}
				return temp.ToByteArray();
			}
			finally
			{
				@in.Close();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private NGit.Patch.Patch ParseTestPatchFile(string patchFile)
		{
			InputStream @in = GetType().GetResourceAsStream(patchFile);
			if (@in == null)
			{
				NUnit.Framework.Assert.Fail("No " + patchFile + " test vector");
				return null;
			}
			// Never happens
			try
			{
				NGit.Patch.Patch p = new NGit.Patch.Patch();
				p.Parse(@in);
				return p;
			}
			finally
			{
				@in.Close();
			}
		}
	}
}
