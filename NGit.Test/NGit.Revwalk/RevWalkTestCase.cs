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
using NGit.Dircache;
using NGit.Junit;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>
	/// Support for tests of the
	/// <see cref="RevWalk">RevWalk</see>
	/// class.
	/// </summary>
	public abstract class RevWalkTestCase : RepositoryTestCase
	{
		private TestRepository<Repository> util;

		protected internal RevWalk rw;

		/// <exception cref="System.Exception"></exception>
		public override void SetUp()
		{
			base.SetUp();
			util = new TestRepository<Repository>(db, CreateRevWalk());
			rw = util.GetRevWalk();
		}

		protected internal virtual RevWalk CreateRevWalk()
		{
			return new RevWalk(db);
		}

		protected internal virtual DateTime GetClock()
		{
			return util.GetClock();
		}

		protected internal virtual void Tick(int secDelta)
		{
			util.Tick(secDelta);
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual RevBlob Blob(string content)
		{
			return util.Blob(content);
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual DirCacheEntry File(string path, RevBlob blob)
		{
			return util.File(path, blob);
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual RevTree Tree(params DirCacheEntry[] entries)
		{
			return util.Tree(entries);
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual RevObject Get(RevTree tree, string path)
		{
			return util.Get(tree, path);
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual RevCommit Commit(params RevCommit[] parents)
		{
			return util.Commit(parents);
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual RevCommit Commit(RevTree tree, params RevCommit[] parents
			)
		{
			return util.Commit(tree, parents);
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual RevCommit Commit(int secDelta, params RevCommit[] parents
			)
		{
			return util.Commit(secDelta, parents);
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual RevCommit Commit(int secDelta, RevTree tree, params RevCommit
			[] parents)
		{
			return util.Commit(secDelta, tree, parents);
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual RevTag Tag(string name, RevObject dst)
		{
			return util.Tag(name, dst);
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual T ParseBody<T>(T t) where T:RevObject
		{
			return util.ParseBody(t);
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual void MarkStart(RevCommit commit)
		{
			rw.MarkStart(commit);
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual void MarkUninteresting(RevCommit commit)
		{
			rw.MarkUninteresting(commit);
		}

		protected internal virtual void AssertCommit(RevCommit exp, RevCommit act)
		{
			NUnit.Framework.Assert.AreSame(exp, act);
		}
	}
}
