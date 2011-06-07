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

using NGit;
using NGit.Api;
using NGit.Diff;
using NGit.Dircache;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Diff
{
	[NUnit.Framework.TestFixture]
	public class PatchIdDiffFormatterTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDiff()
		{
			Write(new FilePath(db.Directory.GetParent(), "test.txt"), "test");
			FilePath folder = new FilePath(db.Directory.GetParent(), "folder");
			folder.Mkdir();
			Write(new FilePath(folder, "folder.txt"), "folder");
			Git git = new Git(db);
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("Initial commit").Call();
			Write(new FilePath(folder, "folder.txt"), "folder change");
			PatchIdDiffFormatter df = new PatchIdDiffFormatter();
			df.SetRepository(db);
			df.SetPathFilter(PathFilter.Create("folder"));
			DirCacheIterator oldTree = new DirCacheIterator(db.ReadDirCache());
			FileTreeIterator newTree = new FileTreeIterator(db);
			df.Format(oldTree, newTree);
			df.Flush();
			NUnit.Framework.Assert.AreEqual("1ff64e0f9333e9b81967c3e8d7a81362b14d5441", df.GetCalulatedPatchId
				().Name);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSameDiff()
		{
			Write(new FilePath(db.Directory.GetParent(), "test.txt"), "test");
			FilePath folder = new FilePath(db.Directory.GetParent(), "folder");
			folder.Mkdir();
			Write(new FilePath(folder, "folder.txt"), "\n\n\n\nfolder");
			Git git = new Git(db);
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("Initial commit").Call();
			Write(new FilePath(folder, "folder.txt"), "\n\n\n\nfolder change");
			PatchIdDiffFormatter df = new PatchIdDiffFormatter();
			df.SetRepository(db);
			df.SetPathFilter(PathFilter.Create("folder"));
			DirCacheIterator oldTree = new DirCacheIterator(db.ReadDirCache());
			FileTreeIterator newTree = new FileTreeIterator(db);
			df.Format(oldTree, newTree);
			df.Flush();
			NUnit.Framework.Assert.AreEqual("08fca5ac531383eb1da8bf6b6f7cf44411281407", df.GetCalulatedPatchId
				().Name);
			Write(new FilePath(folder, "folder.txt"), "a\n\n\n\nfolder");
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("Initial commit").Call();
			Write(new FilePath(folder, "folder.txt"), "a\n\n\n\nfolder change");
			df = new PatchIdDiffFormatter();
			df.SetRepository(db);
			df.SetPathFilter(PathFilter.Create("folder"));
			oldTree = new DirCacheIterator(db.ReadDirCache());
			newTree = new FileTreeIterator(db);
			df.Format(oldTree, newTree);
			df.Flush();
			NUnit.Framework.Assert.AreEqual("08fca5ac531383eb1da8bf6b6f7cf44411281407", df.GetCalulatedPatchId
				().Name);
		}
	}
}
