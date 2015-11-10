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
using System.Text;
using NGit;
using NGit.Api;
using NGit.Diff;
using NUnit.Framework;
using Sharpen;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class ApplyCommandTest : RepositoryTestCase
	{
		private RawText a;

		private RawText b;

		/// <exception cref="System.Exception"></exception>
		private ApplyResult Init(string name)
		{
			return Init(name, true, true);
		}

		/// <exception cref="System.Exception"></exception>
		private ApplyResult Init(string name, bool preExists, bool postExists)
		{
			Git git = new Git(db);
			if (preExists)
			{
				a = new RawText(ReadFile(name + "_PreImage"));
				Write(new FilePath(db.Directory.GetParent(), name), a.GetString(0, a.Size(), false
					));
				git.Add().AddFilepattern(name).Call();
				git.Commit().SetMessage("PreImage").Call();
			}
			if (postExists)
			{
				b = new RawText(ReadFile(name + "_PostImage"));
			}
			return git.Apply().SetPatch(typeof(DiffFormatterReflowTest).GetResourceAsStream(name
				 + ".patch")).Call();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddA1()
		{
			ApplyResult result = Init("A1", false, true);
			NUnit.Framework.Assert.AreEqual(1, result.GetUpdatedFiles().Count);
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, "A1"), result.GetUpdatedFiles
				()[0]);
			CheckFile(new FilePath(db.WorkTree, "A1"), b.GetString(0, b.Size(), false));
		}

	    [Test]
	    public void TestThatPatchingWhichMakesFileEmptyCanBeApplied()
	    {
            ApplyResult result = Init("ToEmpty", true, true);
            NUnit.Framework.Assert.AreEqual(1, result.GetUpdatedFiles().Count);
            NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, "ToEmpty"), result.GetUpdatedFiles
                ()[0]);
            CheckFile(new FilePath(db.WorkTree, "ToEmpty"), b.GetString(0, b.Size(), false));
        }

	    [Test]
	    public void TestThatPatchWhichHasUtf8ByteOrderMarkInContextCanBeApplied()
	    {
            ApplyResult result = Init("FileStartingWithUtf8Bom", true, true);
            Assert.AreEqual(1, result.GetUpdatedFiles().Count);
            Assert.AreEqual(new FilePath(db.WorkTree, "FileStartingWithUtf8Bom"), result.GetUpdatedFiles()[0]);
	        Assert.That(
	            File.ReadAllBytes(new FilePath(db.WorkTree, "FileStartingWithUtf8Bom")),
	            Is.EqualTo(Encoding.UTF8.GetBytes(b.GetString(0, b.Size(), false))));
	    }

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddA2()
		{
			ApplyResult result = Init("A2", false, true);
			NUnit.Framework.Assert.AreEqual(1, result.GetUpdatedFiles().Count);
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, "A2"), result.GetUpdatedFiles
				()[0]);
			CheckFile(new FilePath(db.WorkTree, "A2"), b.GetString(0, b.Size(), false));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddA1Sub()
		{
			ApplyResult result = Init("A1_sub", false, false);
			NUnit.Framework.Assert.AreEqual(1, result.GetUpdatedFiles().Count);
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, "sub/A1"), result.GetUpdatedFiles
				()[0]);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteD()
		{
			ApplyResult result = Init("D", true, false);
			NUnit.Framework.Assert.AreEqual(1, result.GetUpdatedFiles().Count);
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, "D"), result.GetUpdatedFiles
				()[0]);
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "D").Exists());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFailureF1()
		{
			Init("F1", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFailureF2()
		{
			Init("F2", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestModifyE()
		{
			ApplyResult result = Init("E");
			NUnit.Framework.Assert.AreEqual(1, result.GetUpdatedFiles().Count);
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, "E"), result.GetUpdatedFiles
				()[0]);
			CheckFile(new FilePath(db.WorkTree, "E"), b.GetString(0, b.Size(), false));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestModifyX()
		{
			ApplyResult result = Init("X");
			NUnit.Framework.Assert.AreEqual(1, result.GetUpdatedFiles().Count);
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, "X"), result.GetUpdatedFiles
				()[0]);
			CheckFile(new FilePath(db.WorkTree, "X"), b.GetString(0, b.Size(), false));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestModifyY()
		{
			ApplyResult result = Init("Y");
			NUnit.Framework.Assert.AreEqual(1, result.GetUpdatedFiles().Count);
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, "Y"), result.GetUpdatedFiles
				()[0]);
			CheckFile(new FilePath(db.WorkTree, "Y"), b.GetString(0, b.Size(), false));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestModifyZ()
		{
			ApplyResult result = Init("Z");
			NUnit.Framework.Assert.AreEqual(1, result.GetUpdatedFiles().Count);
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, "Z"), result.GetUpdatedFiles
				()[0]);
			CheckFile(new FilePath(db.WorkTree, "Z"), b.GetString(0, b.Size(), false));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private byte[] ReadFile(string patchFile)
		{
			InputStream @in = typeof(DiffFormatterReflowTest).GetResourceAsStream(patchFile);
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
	}
}
