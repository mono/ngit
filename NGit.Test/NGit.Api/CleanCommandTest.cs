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
	/// <summary>Tests for CleanCommand</summary>
	[NUnit.Framework.TestFixture]
	public class CleanCommandTest : RepositoryTestCase
	{
		private Git git;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			git = new Git(db);
			// create test files
			WriteTrashFile("File1.txt", "Hello world");
			WriteTrashFile("File2.txt", "Delete Me");
			WriteTrashFile("File3.txt", "Delete Me");
			// add and commit first file
			git.Add().AddFilepattern("File1.txt").Call();
			git.Commit().SetMessage("Initial commit").Call();
		}

		/// <exception cref="NGit.Errors.NoWorkTreeException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestClean()
		{
			// create status
			StatusCommand command = git.Status();
			Status status = command.Call();
			ICollection<string> files = status.GetUntracked();
			NUnit.Framework.Assert.IsTrue(files.Count > 0);
			// run clean
			ICollection<string> cleanedFiles = git.Clean().Call();
			status = git.Status().Call();
			files = status.GetUntracked();
			NUnit.Framework.Assert.AreEqual(0, files.Count);
			NUnit.Framework.Assert.IsTrue(cleanedFiles.Contains("File2.txt"));
			NUnit.Framework.Assert.IsTrue(cleanedFiles.Contains("File3.txt"));
		}

		/// <exception cref="NGit.Errors.NoWorkTreeException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCleanWithPaths()
		{
			// create status
			StatusCommand command = git.Status();
			Status status = command.Call();
			ICollection<string> files = status.GetUntracked();
			NUnit.Framework.Assert.IsTrue(files.Count > 0);
			// run clean with setPaths
			ICollection<string> paths = new TreeSet<string>();
			paths.AddItem("File3.txt");
			ICollection<string> cleanedFiles = git.Clean().SetPaths(paths).Call();
			status = git.Status().Call();
			files = status.GetUntracked();
			NUnit.Framework.Assert.AreEqual(1, files.Count);
			NUnit.Framework.Assert.IsTrue(cleanedFiles.Contains("File3.txt"));
			NUnit.Framework.Assert.IsFalse(cleanedFiles.Contains("File2.txt"));
		}

		/// <exception cref="NGit.Errors.NoWorkTreeException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCleanWithDryRun()
		{
			// create status
			StatusCommand command = git.Status();
			Status status = command.Call();
			ICollection<string> files = status.GetUntracked();
			NUnit.Framework.Assert.IsTrue(files.Count > 0);
			// run clean
			ICollection<string> cleanedFiles = git.Clean().SetDryRun(true).Call();
			status = git.Status().Call();
			files = status.GetUntracked();
			NUnit.Framework.Assert.AreEqual(2, files.Count);
			NUnit.Framework.Assert.IsTrue(cleanedFiles.Contains("File2.txt"));
			NUnit.Framework.Assert.IsTrue(cleanedFiles.Contains("File3.txt"));
		}
	}
}
