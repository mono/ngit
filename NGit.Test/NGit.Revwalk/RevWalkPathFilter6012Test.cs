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
using System.Reflection;
using System.Text;
using NGit.Revwalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Revwalk
{
	public class RevWalkPathFilter6012Test : RevWalkTestCase
	{
		private static readonly string pA = "pA";

		private static readonly string pF = "pF";

		private static readonly string pE = "pE";

		private RevCommit a;

		private RevCommit b;

		private RevCommit c;

		private RevCommit d;

		private RevCommit e;

		private RevCommit f;

		private RevCommit g;

		private RevCommit h;

		private RevCommit i;

		private Dictionary<RevCommit, string> byName;

		// Note: Much of this test case is broken as it depends upon
		// the graph applying topological sorting *before* doing merge
		// simplification.  It also depends upon a difference between
		// full history and non-full history for a path, something we
		// don't quite yet have a distiction for in JGit.
		//
		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			// Test graph was stolen from git-core t6012-rev-list-simplify
			// (by Junio C Hamano in 65347030590bcc251a9ff2ed96487a0f1b9e9fa8)
			//
			RevBlob zF = Blob("zF");
			RevBlob zH = Blob("zH");
			RevBlob zI = Blob("zI");
			RevBlob zS = Blob("zS");
			RevBlob zY = Blob("zY");
			a = Commit(Tree(File(pF, zH)));
			b = Commit(Tree(File(pF, zI)), a);
			c = Commit(Tree(File(pF, zI)), a);
			d = Commit(Tree(File(pA, zS), File(pF, zI)), c);
			ParseBody(d);
			e = Commit(d.Tree, d, b);
			f = Commit(Tree(File(pA, zS), File(pE, zY), File(pF, zI)), e);
			ParseBody(f);
			g = Commit(Tree(File(pE, zY), File(pF, zI)), b);
			h = Commit(f.Tree, g, f);
			i = Commit(Tree(File(pA, zS), File(pE, zY), File(pF, zF)), h);
			byName = new Dictionary<RevCommit, string>();
			foreach (FieldInfo z in Sharpen.Runtime.GetDeclaredFields(typeof(RevWalkPathFilter6012Test
				)))
			{
				if (z.FieldType == typeof(RevCommit))
				{
					byName.Put((RevCommit)z.GetValue(this), z.Name);
				}
			}
		}

		/// <exception cref="System.Exception"></exception>
		protected internal virtual void Check(params RevCommit[] order)
		{
			MarkStart(i);
			StringBuilder act = new StringBuilder();
			foreach (RevCommit z in rw)
			{
				string name = byName.Get(z);
				NUnit.Framework.Assert.IsNotNull(name);
				act.Append(name);
				act.Append(' ');
			}
			StringBuilder exp = new StringBuilder();
			foreach (RevCommit z_1 in order)
			{
				string name = byName.Get(z_1);
				NUnit.Framework.Assert.IsNotNull(name);
				exp.Append(name);
				exp.Append(' ');
			}
			NUnit.Framework.Assert.AreEqual(exp.ToString(), act.ToString());
		}

		protected internal virtual void Filter(string path)
		{
			rw.SetTreeFilter(AndTreeFilter.Create(PathFilterGroup.CreateFromStrings(Sharpen.Collections
				.Singleton(path)), TreeFilter.ANY_DIFF));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test1()
		{
			// TODO --full-history
			Check(i, h, g, f, e, d, c, b, a);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test2()
		{
			// TODO --full-history
			Filter(pF);
		}

		// TODO fix broken test
		// check(i, h, e, c, b, a);
		/// <exception cref="System.Exception"></exception>
		public virtual void Test3()
		{
			// TODO --full-history
			rw.Sort(RevSort.TOPO);
			Filter(pF);
		}

		// TODO fix broken test
		// check(i, h, e, c, b, a);
		/// <exception cref="System.Exception"></exception>
		public virtual void Test4()
		{
			// TODO --full-history
			rw.Sort(RevSort.COMMIT_TIME_DESC);
			Filter(pF);
		}

		// TODO fix broken test
		// check(i, h, e, c, b, a);
		/// <exception cref="System.Exception"></exception>
		public virtual void Test5()
		{
			// TODO --simplify-merges
			Filter(pF);
		}

		// TODO fix broken test
		// check(i, e, c, b, a);
		/// <exception cref="System.Exception"></exception>
		public virtual void Test6()
		{
			Filter(pF);
			Check(i, b, a);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test7()
		{
			rw.Sort(RevSort.TOPO);
			Filter(pF);
			Check(i, b, a);
		}
	}
}
