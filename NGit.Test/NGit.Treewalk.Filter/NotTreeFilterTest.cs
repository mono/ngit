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
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Treewalk.Filter
{
	public class NotTreeFilterTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestWrap()
		{
			TreeWalk tw = new TreeWalk(db);
			TreeFilter a = TreeFilter.ALL;
			TreeFilter n = NotTreeFilter.Create(a);
			NUnit.Framework.Assert.IsNotNull(n);
			NUnit.Framework.Assert.IsTrue(a.Include(tw));
			NUnit.Framework.Assert.IsFalse(n.Include(tw));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNegateIsUnwrap()
		{
			TreeFilter a = PathFilter.Create("a/b");
			TreeFilter n = NotTreeFilter.Create(a);
			NUnit.Framework.Assert.AreSame(a, n.Negate());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestShouldBeRecursive_ALL()
		{
			TreeFilter a = TreeFilter.ALL;
			TreeFilter n = NotTreeFilter.Create(a);
			NUnit.Framework.Assert.AreEqual(a.ShouldBeRecursive(), n.ShouldBeRecursive());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestShouldBeRecursive_PathFilter()
		{
			TreeFilter a = PathFilter.Create("a/b");
			NUnit.Framework.Assert.IsTrue(a.ShouldBeRecursive());
			TreeFilter n = NotTreeFilter.Create(a);
			NUnit.Framework.Assert.IsTrue(n.ShouldBeRecursive());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCloneIsDeepClone()
		{
			TreeFilter a = new AlwaysCloneTreeFilter();
			NUnit.Framework.Assert.AreNotSame(a, a.Clone());
			TreeFilter n = NotTreeFilter.Create(a);
			NUnit.Framework.Assert.AreNotSame(n, n.Clone());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCloneIsSparseWhenPossible()
		{
			TreeFilter a = TreeFilter.ALL;
			NUnit.Framework.Assert.AreSame(a, a.Clone());
			TreeFilter n = NotTreeFilter.Create(a);
			NUnit.Framework.Assert.AreSame(n, n.Clone());
		}
	}
}
