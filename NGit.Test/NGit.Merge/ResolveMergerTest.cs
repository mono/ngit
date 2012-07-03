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
using NGit.Merge;
using NGit.Revwalk;
using NGit.Treewalk;
using NGit.Util;
using Sharpen;

namespace NGit.Merge
{
	[NUnit.Framework.TestFixture]
	public class ResolveMergerTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void FailingPathsShouldNotResultInOKReturnValue()
		{
			FilePath folder1 = new FilePath(db.WorkTree, "folder1");
			FileUtils.Mkdir(folder1);
			FilePath file = new FilePath(folder1, "file1.txt");
			Write(file, "folder1--file1.txt");
			file = new FilePath(folder1, "file2.txt");
			Write(file, "folder1--file2.txt");
			Git git = new Git(db);
			git.Add().AddFilepattern(folder1.GetName()).Call();
			RevCommit @base = git.Commit().SetMessage("adding folder").Call();
			RecursiveDelete(folder1);
			git.Rm().AddFilepattern("folder1/file1.txt").AddFilepattern("folder1/file2.txt").
				Call();
			RevCommit other = git.Commit().SetMessage("removing folders on 'other'").Call();
			git.Checkout().SetName(@base.Name).Call();
			file = new FilePath(db.WorkTree, "unrelated.txt");
			Write(file, "unrelated");
			git.Add().AddFilepattern("unrelated").Call();
			RevCommit head = git.Commit().SetMessage("Adding another file").Call();
			// Untracked file to cause failing path for delete() of folder1
			file = new FilePath(folder1, "file3.txt");
			Write(file, "folder1--file3.txt");
			ResolveMerger merger = new ResolveMerger(db, false);
			merger.SetCommitNames(new string[] { "BASE", "HEAD", "other" });
			merger.SetWorkingTreeIterator(new FileTreeIterator(db));
			bool ok = merger.Merge(head.Id, other.Id);
			NUnit.Framework.Assert.IsFalse(merger.GetFailingPaths().IsEmpty());
			NUnit.Framework.Assert.IsFalse(ok);
		}
	}
}
