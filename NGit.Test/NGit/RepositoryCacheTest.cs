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
using NGit.Errors;
using Sharpen;

namespace NGit
{
	public class RepositoryCacheTest : RepositoryTestCase
	{
		[NUnit.Framework.Test]
		public virtual void TestNonBareFileKey()
		{
			FilePath gitdir = db.Directory;
			FilePath parent = gitdir.GetParentFile();
			FilePath other = new FilePath(parent, "notagit");
			NUnit.Framework.Assert.AreEqual(gitdir, RepositoryCache.FileKey.Exact(gitdir, db.
				FileSystem).GetFile());
			NUnit.Framework.Assert.AreEqual(parent, RepositoryCache.FileKey.Exact(parent, db.
				FileSystem).GetFile());
			NUnit.Framework.Assert.AreEqual(other, RepositoryCache.FileKey.Exact(other, db.FileSystem
				).GetFile());
			NUnit.Framework.Assert.AreEqual(gitdir, RepositoryCache.FileKey.Lenient(gitdir, db
				.FileSystem).GetFile());
			NUnit.Framework.Assert.AreEqual(gitdir, RepositoryCache.FileKey.Lenient(parent, db
				.FileSystem).GetFile());
			NUnit.Framework.Assert.AreEqual(other, RepositoryCache.FileKey.Lenient(other, db.
				FileSystem).GetFile());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBareFileKey()
		{
			Repository bare = CreateBareRepository();
			FilePath gitdir = bare.Directory;
			FilePath parent = gitdir.GetParentFile();
			string name = gitdir.GetName();
			NUnit.Framework.Assert.IsTrue(name.EndsWith(".git"));
			name = Sharpen.Runtime.Substring(name, 0, name.Length - 4);
			NUnit.Framework.Assert.AreEqual(gitdir, RepositoryCache.FileKey.Exact(gitdir, db.
				FileSystem).GetFile());
			NUnit.Framework.Assert.AreEqual(gitdir, RepositoryCache.FileKey.Lenient(gitdir, db
				.FileSystem).GetFile());
			NUnit.Framework.Assert.AreEqual(gitdir, RepositoryCache.FileKey.Lenient(new FilePath
				(parent, name), db.FileSystem).GetFile());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileKeyOpenExisting()
		{
			Repository r;
			r = new RepositoryCache.FileKey(db.Directory, db.FileSystem).Open(true);
			NUnit.Framework.Assert.IsNotNull(r);
			NUnit.Framework.Assert.AreEqual(db.Directory, r.Directory);
			r.Close();
			r = new RepositoryCache.FileKey(db.Directory, db.FileSystem).Open(false);
			NUnit.Framework.Assert.IsNotNull(r);
			NUnit.Framework.Assert.AreEqual(db.Directory, r.Directory);
			r.Close();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileKeyOpenNew()
		{
			Repository n = CreateBareRepository();
			FilePath gitdir = n.Directory;
			n.Close();
			RecursiveDelete(gitdir);
			NUnit.Framework.Assert.IsFalse(gitdir.Exists());
			try
			{
				new RepositoryCache.FileKey(gitdir, db.FileSystem).Open(true);
				NUnit.Framework.Assert.Fail("incorrectly opened a non existant repository");
			}
			catch (RepositoryNotFoundException e)
			{
				NUnit.Framework.Assert.AreEqual("repository not found: " + gitdir, e.Message);
			}
			Repository o = new RepositoryCache.FileKey(gitdir, db.FileSystem).Open(false);
			NUnit.Framework.Assert.IsNotNull(o);
			NUnit.Framework.Assert.AreEqual(gitdir, o.Directory);
			NUnit.Framework.Assert.IsFalse(gitdir.Exists());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCacheRegisterOpen()
		{
			FilePath dir = db.Directory;
			RepositoryCache.Register(db);
			NUnit.Framework.Assert.AreSame(db, RepositoryCache.Open(RepositoryCache.FileKey.Exact
				(dir, db.FileSystem)));
			NUnit.Framework.Assert.AreEqual(".git", dir.GetName());
			FilePath parent = dir.GetParentFile();
			NUnit.Framework.Assert.AreSame(db, RepositoryCache.Open(RepositoryCache.FileKey.Lenient
				(parent, db.FileSystem)));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCacheOpen()
		{
			RepositoryCache.FileKey loc = RepositoryCache.FileKey.Exact(db.Directory, db.FileSystem
				);
			Repository d2 = RepositoryCache.Open(loc);
			NUnit.Framework.Assert.AreNotSame(db, d2);
			NUnit.Framework.Assert.AreSame(d2, RepositoryCache.Open(RepositoryCache.FileKey.Exact
				(loc.GetFile(), db.FileSystem)));
			d2.Close();
			d2.Close();
		}
	}
}
