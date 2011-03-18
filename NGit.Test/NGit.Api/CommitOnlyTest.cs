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
using NGit.Api;
using NGit.Api.Errors;
using NGit.Revwalk;
using NGit.Treewalk;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Testing the 'commit only' option:
	/// I.
	/// </summary>
	/// <remarks>
	/// Testing the 'commit only' option:
	/// I. A single file (f1.txt) specified as part of the --only/ -o option can have
	/// one of the following (14) states:
	/// <pre>
	/// |                          | expected result
	/// ---------------------------------------------------------------------
	/// | HEAD  DirCache  Worktree | HEAD  DirCache
	/// ---------------------------------------------------------------------
	/// f1_1  |  -       -       c       |                =&gt; e: path unknown
	/// f1_2  |  -       c       -       |                =&gt; no changes
	/// f1_3  |  c       -       -       |  -       -
	/// f1_4  |  -       c       c       |  c       c
	/// f1_5  |  c       c       -       |  -       -
	/// f1_6  |  c       -       c       |                =&gt; no changes
	/// f1_7  |  c       c       c       |                =&gt; no changes
	/// ---------------------------------------------------------------------
	/// f1_8  |  -       c       c'      |  c'      c'
	/// f1_9  |  c       -       c'      |  c'      c'
	/// f1_10  |  c       c'      -       |  -       -
	/// f1_11  |  c       c       c'      |  c'      c'
	/// f1_12  |  c       c'      c       |                =&gt; no changes
	/// f1_13  |  c       c'      c'      |  c'      c'
	/// ---------------------------------------------------------------------
	/// f1_14  |  c       c'      c''     |  c''     c''
	/// </pre>
	/// II. Scenarios that do not end with a successful commit (1, 2, 6, 7, 12) have
	/// to be tested with a second file (f2.txt) specified that would lead to a
	/// successful commit, if it were executed separately (e.g. scenario 14).
	/// <pre>
	/// |                          | expected result
	/// ---------------------------------------------------------------------------
	/// | HEAD  DirCache  Worktree | HEAD  DirCache
	/// ---------------------------------------------------------------------------
	/// f1_1_f2_14  |  -       -       c       |                =&gt; e: path unknown
	/// f1_2_f2_14  |  -       c       -       |  -       -
	/// f1_6_f2_14  |  c       -       c       |  c       c
	/// f1_7_f2_14  |  c       c       c       |  c       c
	/// ---------------------------------------------------------------------------
	/// f1_12_f2_14  |  c       c'      c       |  c       c
	/// </pre>
	/// III. All scenarios (1-14, I-II) have to be tested with different repository
	/// states, to check that the --only/ -o option does not change existing content
	/// (HEAD and DirCache). The following states for a file (f3.txt) not specified
	/// shall be tested:
	/// <pre>
	/// | HEAD  DirCache
	/// --------------------
	/// *_a  |  -       -
	/// *_b  |  -       c
	/// *_c  |  c       c
	/// *_d  |  c       -
	/// --------------------
	/// *_e  |  c       c'
	/// </pre>
	/// </remarks>
	[NUnit.Framework.TestFixture]
	public class CommitOnlyTest : RepositoryTestCase
	{
		private static readonly string F1 = "f1.txt";

		private static readonly string F2 = "f2.txt";

		private static readonly string F3 = "f3.txt";

		private static readonly string MSG = "commit";

		private static int A = 0;

		private static int B = 1;

		private static int C = 2;

		private static int D = 3;

		private static int E = 4;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_1_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_1(git);
			ExecuteAndCheck_f1_1(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_1_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_1(git);
			ExecuteAndCheck_f1_1(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_1_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_1(git);
			ExecuteAndCheck_f1_1(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_1_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_1(git);
			ExecuteAndCheck_f1_1(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_1_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_1(git);
			ExecuteAndCheck_f1_1(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_1_f2_14_a()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, A);
			Prepare_f1_1(git);
			ExecuteAndCheck_f1_1_f2_f14(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_1_f2_14_b()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, B);
			Prepare_f1_1(git);
			ExecuteAndCheck_f1_1_f2_f14(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_1_f2_14_c()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, C);
			Prepare_f1_1(git);
			ExecuteAndCheck_f1_1_f2_f14(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_1_f2_14_d()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, D);
			Prepare_f1_1(git);
			ExecuteAndCheck_f1_1_f2_f14(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_1_f2_14_e()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, E);
			Prepare_f1_1(git);
			ExecuteAndCheck_f1_1_f2_f14(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_2_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_2(git);
			ExecuteAndCheck_f1_2(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_2_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_2(git);
			ExecuteAndCheck_f1_2(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_2_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_2(git);
			ExecuteAndCheck_f1_2(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_2_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_2(git);
			ExecuteAndCheck_f1_2(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_2_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_2(git);
			ExecuteAndCheck_f1_2(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_2_f2_14_a()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, A);
			Prepare_f1_2(git);
			ExecuteAndCheck_f1_2_f2_f14(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_2_f2_14_b()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, B);
			Prepare_f1_2(git);
			ExecuteAndCheck_f1_2_f2_f14(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_2_f2_14_c()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, C);
			Prepare_f1_2(git);
			ExecuteAndCheck_f1_2_f2_f14(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_2_f2_14_d()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, D);
			Prepare_f1_2(git);
			ExecuteAndCheck_f1_2_f2_f14(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_2_f2_14_e()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, E);
			Prepare_f1_2(git);
			ExecuteAndCheck_f1_2_f2_f14(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_3_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_3(git);
			ExecuteAndCheck_f1_3(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_3_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_3(git);
			ExecuteAndCheck_f1_3(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_3_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_3(git);
			ExecuteAndCheck_f1_3(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_3_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_3(git);
			ExecuteAndCheck_f1_3(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_3_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_3(git);
			ExecuteAndCheck_f1_3(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_4_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_4(git);
			ExecuteAndCheck_f1_4(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_4_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_4(git);
			ExecuteAndCheck_f1_4(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_4_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_4(git);
			ExecuteAndCheck_f1_4(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_4_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_4(git);
			ExecuteAndCheck_f1_4(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_4_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_4(git);
			ExecuteAndCheck_f1_4(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_5_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_5(git);
			ExecuteAndCheck_f1_5(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_5_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_5(git);
			ExecuteAndCheck_f1_5(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_5_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_5(git);
			ExecuteAndCheck_f1_5(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_5_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_5(git);
			ExecuteAndCheck_f1_5(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_5_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_5(git);
			ExecuteAndCheck_f1_5(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_6_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_6(git);
			ExecuteAndCheck_f1_6(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_6_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_6(git);
			ExecuteAndCheck_f1_6(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_6_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_6(git);
			ExecuteAndCheck_f1_6(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_6_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_6(git);
			ExecuteAndCheck_f1_6(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_6_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_6(git);
			ExecuteAndCheck_f1_6(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_6_f2_14_a()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, A);
			Prepare_f1_6(git);
			ExecuteAndCheck_f1_6_f2_14(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_6_f2_14_b()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, B);
			Prepare_f1_6(git);
			ExecuteAndCheck_f1_6_f2_14(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_6_f2_14_c()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, C);
			Prepare_f1_6(git);
			ExecuteAndCheck_f1_6_f2_14(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_6_f2_14_d()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, D);
			Prepare_f1_6(git);
			ExecuteAndCheck_f1_6_f2_14(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_6_f2_14_e()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, E);
			Prepare_f1_6(git);
			ExecuteAndCheck_f1_6_f2_14(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_7_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_7(git);
			ExecuteAndCheck_f1_7(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_7_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_7(git);
			ExecuteAndCheck_f1_7(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_7_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_7(git);
			ExecuteAndCheck_f1_7(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_7_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_7(git);
			ExecuteAndCheck_f1_7(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_7_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_7(git);
			ExecuteAndCheck_f1_7(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_7_f2_14_a()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, A);
			Prepare_f1_7(git);
			ExecuteAndCheck_f1_7_f2_14(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_7_f2_14_b()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, B);
			Prepare_f1_7(git);
			ExecuteAndCheck_f1_7_f2_14(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_7_f2_14_c()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, C);
			Prepare_f1_7(git);
			ExecuteAndCheck_f1_7_f2_14(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_7_f2_14_d()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, D);
			Prepare_f1_7(git);
			ExecuteAndCheck_f1_7_f2_14(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_7_f2_14_e()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, E);
			Prepare_f1_7(git);
			ExecuteAndCheck_f1_7_f2_14(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_8_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_8(git);
			ExecuteAndCheck_f1_8(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_8_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_8(git);
			ExecuteAndCheck_f1_8(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_8_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_8(git);
			ExecuteAndCheck_f1_8(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_8_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_8(git);
			ExecuteAndCheck_f1_8(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_8_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_8(git);
			ExecuteAndCheck_f1_8(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_9_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_9(git);
			ExecuteAndCheck_f1_9(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_9_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_9(git);
			ExecuteAndCheck_f1_9(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_9_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_9(git);
			ExecuteAndCheck_f1_9(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_9_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_9(git);
			ExecuteAndCheck_f1_9(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_9_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_9(git);
			ExecuteAndCheck_f1_9(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_10_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_10(git);
			ExecuteAndCheck_f1_10(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_10_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_10(git);
			ExecuteAndCheck_f1_10(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_10_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_10(git);
			ExecuteAndCheck_f1_10(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_10_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_10(git);
			ExecuteAndCheck_f1_10(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_10_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_10(git);
			ExecuteAndCheck_f1_10(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_11_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_11(git);
			ExecuteAndCheck_f1_11(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_11_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_11(git);
			ExecuteAndCheck_f1_11(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_11_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_11(git);
			ExecuteAndCheck_f1_11(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_11_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_11(git);
			ExecuteAndCheck_f1_11(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_11_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_11(git);
			ExecuteAndCheck_f1_11(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_12_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_12(git);
			ExecuteAndCheck_f1_12(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_12_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_12(git);
			ExecuteAndCheck_f1_12(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_12_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_12(git);
			ExecuteAndCheck_f1_12(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_12_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_12(git);
			ExecuteAndCheck_f1_12(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_12_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_12(git);
			ExecuteAndCheck_f1_12(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_12_f2_14_a()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, A);
			Prepare_f1_12(git);
			ExecuteAndCheck_f1_12_f2_14(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_12_f2_14_b()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, B);
			Prepare_f1_12(git);
			ExecuteAndCheck_f1_12_f2_14(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_12_f2_14_c()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, C);
			Prepare_f1_12(git);
			ExecuteAndCheck_f1_12_f2_14(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_12_f2_14_d()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, D);
			Prepare_f1_12(git);
			ExecuteAndCheck_f1_12_f2_14(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_12_f2_14_e()
		{
			Git git = new Git(db);
			Prepare_f3_f2_14(git, E);
			Prepare_f1_12(git);
			ExecuteAndCheck_f1_12_f2_14(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_13_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_13(git);
			ExecuteAndCheck_f1_13(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_13_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_13(git);
			ExecuteAndCheck_f1_13(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_13_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_13(git);
			ExecuteAndCheck_f1_13(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_13_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_13(git);
			ExecuteAndCheck_f1_13(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_13_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_13(git);
			ExecuteAndCheck_f1_13(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_14_a()
		{
			Git git = new Git(db);
			Prepare_f3(git, A);
			Prepare_f1_14(git);
			ExecuteAndCheck_f1_14(git, A);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_14_b()
		{
			Git git = new Git(db);
			Prepare_f3(git, B);
			Prepare_f1_14(git);
			ExecuteAndCheck_f1_14(git, B);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_14_c()
		{
			Git git = new Git(db);
			Prepare_f3(git, C);
			Prepare_f1_14(git);
			ExecuteAndCheck_f1_14(git, C);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_14_d()
		{
			Git git = new Git(db);
			Prepare_f3(git, D);
			Prepare_f1_14(git);
			ExecuteAndCheck_f1_14(git, D);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOption_f1_14_e()
		{
			Git git = new Git(db);
			Prepare_f3(git, E);
			Prepare_f1_14(git);
			ExecuteAndCheck_f1_14(git, E);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnlyOptionWithDirectory()
		{
			Git git = new Git(db);
			// write files
			FilePath f1 = WriteTrashFile("d1/d2/f1.txt", "c1");
			WriteTrashFile("d1/d2/f2.txt", "c2");
			FilePath f3 = WriteTrashFile("d1/f3.txt", "c3");
			WriteTrashFile("d1/f4.txt", "c4");
			FilePath f5 = WriteTrashFile("d3/d4/f5.txt", "c5");
			WriteTrashFile("d3/d4/f6.txt", "c6");
			FilePath f7 = WriteTrashFile("d3/f7.txt", "c7");
			WriteTrashFile("d3/f8.txt", "c8");
			FilePath f9 = WriteTrashFile("d5/f9.txt", "c9");
			WriteTrashFile("d5/f10.txt", "c10");
			FilePath f11 = WriteTrashFile("d6/f11.txt", "c11");
			WriteTrashFile("d6/f12.txt", "c12");
			// add files
			git.Add().AddFilepattern(".").Call();
			// modify files, but do not stage changes
			Write(f1, "c1'");
			Write(f3, "c3'");
			Write(f5, "c5'");
			Write(f7, "c7'");
			Write(f9, "c9'");
			Write(f11, "c11'");
			// commit selected files only
			git.Commit().SetOnly("d1").SetOnly("d3/d4/").SetOnly("d5").SetOnly("d6/f11.txt").
				SetMessage(MSG).Call();
			NUnit.Framework.Assert.AreEqual("c1'", GetHead(git, "d1/d2/f1.txt"));
			NUnit.Framework.Assert.AreEqual("c2", GetHead(git, "d1/d2/f2.txt"));
			NUnit.Framework.Assert.AreEqual("c3'", GetHead(git, "d1/f3.txt"));
			NUnit.Framework.Assert.AreEqual("c4", GetHead(git, "d1/f4.txt"));
			NUnit.Framework.Assert.AreEqual("c5'", GetHead(git, "d3/d4/f5.txt"));
			NUnit.Framework.Assert.AreEqual("c6", GetHead(git, "d3/d4/f6.txt"));
			NUnit.Framework.Assert.AreEqual(string.Empty, GetHead(git, "d3/f7.txt"));
			NUnit.Framework.Assert.AreEqual(string.Empty, GetHead(git, "d3/f8.txt"));
			NUnit.Framework.Assert.AreEqual("c9'", GetHead(git, "d5/f9.txt"));
			NUnit.Framework.Assert.AreEqual("c10", GetHead(git, "d5/f10.txt"));
			NUnit.Framework.Assert.AreEqual("c11'", GetHead(git, "d6/f11.txt"));
			NUnit.Framework.Assert.AreEqual(string.Empty, GetHead(git, "d6/f12.txt"));
			NUnit.Framework.Assert.AreEqual("[d1/d2/f1.txt, mode:100644, content:c1']" + "[d1/d2/f2.txt, mode:100644, content:c2]"
				 + "[d1/f3.txt, mode:100644, content:c3']" + "[d1/f4.txt, mode:100644, content:c4]"
				 + "[d3/d4/f5.txt, mode:100644, content:c5']" + "[d3/d4/f6.txt, mode:100644, content:c6]"
				 + "[d3/f7.txt, mode:100644, content:c7]" + "[d3/f8.txt, mode:100644, content:c8]"
				 + "[d5/f10.txt, mode:100644, content:c10]" + "[d5/f9.txt, mode:100644, content:c9']"
				 + "[d6/f11.txt, mode:100644, content:c11']" + "[d6/f12.txt, mode:100644, content:c12]"
				, IndexState(CONTENT));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private FilePath Prepare_f1_1(Git git)
		{
			return WriteTrashFile(F1, "c1");
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_2(Git git)
		{
			FilePath f1 = Prepare_f1_4(git);
			f1.Delete();
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_3(Git git)
		{
			FilePath f1 = Prepare_f1_7(git);
			git.Rm().AddFilepattern(F1).Call();
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_4(Git git)
		{
			FilePath f1 = Prepare_f1_1(git);
			git.Add().AddFilepattern(F1).Call();
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_5(Git git)
		{
			FilePath f1 = Prepare_f1_7(git);
			f1.Delete();
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_6(Git git)
		{
			FilePath f1 = Prepare_f1_3(git);
			Write(f1, "c1");
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_7(Git git)
		{
			FilePath f1 = Prepare_f1_4(git);
			git.Commit().SetOnly(F1).SetMessage(MSG).Call();
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_8(Git git)
		{
			FilePath f1 = Prepare_f1_4(git);
			Write(f1, "c1'");
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_9(Git git)
		{
			FilePath f1 = Prepare_f1_3(git);
			Write(f1, "c1'");
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_10(Git git)
		{
			FilePath f1 = Prepare_f1_9(git);
			git.Add().AddFilepattern(F1).Call();
			f1.Delete();
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_11(Git git)
		{
			FilePath f1 = Prepare_f1_7(git);
			Write(f1, "c1'");
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_12(Git git)
		{
			FilePath f1 = Prepare_f1_13(git);
			Write(f1, "c1");
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_13(Git git)
		{
			FilePath f1 = Prepare_f1_11(git);
			git.Add().AddFilepattern(F1).Call();
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private FilePath Prepare_f1_14(Git git)
		{
			FilePath f1 = Prepare_f1_13(git);
			Write(f1, "c1''");
			return f1;
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_1(Git git, int state)
		{
			JGitInternalException exception = null;
			try
			{
				git.Commit().SetOnly(F1).SetMessage(MSG).Call();
			}
			catch (JGitInternalException e)
			{
				exception = e;
			}
			NUnit.Framework.Assert.IsNotNull(exception);
			NUnit.Framework.Assert.IsTrue(exception.Message.Contains(F1));
			NUnit.Framework.Assert.AreEqual(Expected_f3_head(state), GetHead(git, F3));
			NUnit.Framework.Assert.AreEqual(Expected_f3_idx(state), IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_1_f2_f14(Git git, int state)
		{
			JGitInternalException exception = null;
			try
			{
				git.Commit().SetOnly(F1).SetOnly(F2).SetMessage(MSG).Call();
			}
			catch (JGitInternalException e)
			{
				exception = e;
			}
			NUnit.Framework.Assert.IsNotNull(exception);
			NUnit.Framework.Assert.IsTrue(exception.Message.Contains(F1));
			NUnit.Framework.Assert.AreEqual("c2", GetHead(git, F2));
			NUnit.Framework.Assert.AreEqual(Expected_f3_head(state), GetHead(git, F3));
			NUnit.Framework.Assert.AreEqual("[f2.txt, mode:100644, content:c2']" + Expected_f3_idx
				(state), IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_2(Git git, int state)
		{
			JGitInternalException exception = null;
			try
			{
				git.Commit().SetOnly(F1).SetMessage(MSG).Call();
			}
			catch (JGitInternalException e)
			{
				exception = e;
			}
			NUnit.Framework.Assert.IsNotNull(exception);
			NUnit.Framework.Assert.IsTrue(exception.Message.Contains("No changes"));
			NUnit.Framework.Assert.AreEqual(Expected_f3_head(state), GetHead(git, F3));
			NUnit.Framework.Assert.AreEqual("[f1.txt, mode:100644, content:c1]" + Expected_f3_idx
				(state), IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_2_f2_f14(Git git, int state)
		{
			git.Commit().SetOnly(F1).SetOnly(F2).SetMessage(MSG).Call();
			NUnit.Framework.Assert.AreEqual(string.Empty, GetHead(git, F1));
			NUnit.Framework.Assert.AreEqual("c2''", GetHead(git, F2));
			NUnit.Framework.Assert.AreEqual(Expected_f3_head(state), GetHead(git, F3));
			NUnit.Framework.Assert.AreEqual("[f2.txt, mode:100644, content:c2'']" + Expected_f3_idx
				(state), IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_3(Git git, int state)
		{
			git.Commit().SetOnly(F1).SetMessage(MSG).Call();
			NUnit.Framework.Assert.AreEqual(string.Empty, GetHead(git, F1));
			NUnit.Framework.Assert.AreEqual(Expected_f3_head(state), GetHead(git, F3));
			NUnit.Framework.Assert.AreEqual(Expected_f3_idx(state), IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_4(Git git, int state)
		{
			git.Commit().SetOnly(F1).SetMessage(MSG).Call();
			NUnit.Framework.Assert.AreEqual("c1", GetHead(git, F1));
			NUnit.Framework.Assert.AreEqual(Expected_f3_head(state), GetHead(git, F3));
			NUnit.Framework.Assert.AreEqual("[f1.txt, mode:100644, content:c1]" + Expected_f3_idx
				(state), IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_5(Git git, int state)
		{
			ExecuteAndCheck_f1_3(git, state);
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_6(Git git, int state)
		{
			JGitInternalException exception = null;
			try
			{
				git.Commit().SetOnly(F1).SetMessage(MSG).Call();
			}
			catch (JGitInternalException e)
			{
				exception = e;
			}
			NUnit.Framework.Assert.IsNotNull(exception);
			NUnit.Framework.Assert.IsTrue(exception.Message.Contains("No changes"));
			NUnit.Framework.Assert.AreEqual(Expected_f3_head(state), GetHead(git, F3));
			NUnit.Framework.Assert.AreEqual(Expected_f3_idx(state), IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_6_f2_14(Git git, int state)
		{
			git.Commit().SetOnly(F1).SetOnly(F2).SetMessage(MSG).Call();
			NUnit.Framework.Assert.AreEqual("c1", GetHead(git, F1));
			NUnit.Framework.Assert.AreEqual("c2''", GetHead(git, F2));
			NUnit.Framework.Assert.AreEqual(Expected_f3_head(state), GetHead(git, F3));
			NUnit.Framework.Assert.AreEqual("[f1.txt, mode:100644, content:c1]" + "[f2.txt, mode:100644, content:c2'']"
				 + Expected_f3_idx(state), IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_7(Git git, int state)
		{
			ExecuteAndCheck_f1_2(git, state);
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_7_f2_14(Git git, int state)
		{
			ExecuteAndCheck_f1_6_f2_14(git, state);
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_8(Git git, int state)
		{
			git.Commit().SetOnly(F1).SetMessage(MSG).Call();
			NUnit.Framework.Assert.AreEqual("c1'", GetHead(git, F1));
			NUnit.Framework.Assert.AreEqual(Expected_f3_head(state), GetHead(git, F3));
			NUnit.Framework.Assert.AreEqual("[f1.txt, mode:100644, content:c1']" + Expected_f3_idx
				(state), IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_9(Git git, int state)
		{
			ExecuteAndCheck_f1_8(git, state);
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_10(Git git, int state)
		{
			ExecuteAndCheck_f1_3(git, state);
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_11(Git git, int state)
		{
			ExecuteAndCheck_f1_8(git, state);
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_12(Git git, int state)
		{
			JGitInternalException exception = null;
			try
			{
				git.Commit().SetOnly(F1).SetMessage(MSG).Call();
			}
			catch (JGitInternalException e)
			{
				exception = e;
			}
			NUnit.Framework.Assert.IsNotNull(exception);
			NUnit.Framework.Assert.IsTrue(exception.Message.Contains("No changes"));
			NUnit.Framework.Assert.AreEqual(Expected_f3_head(state), GetHead(git, F3));
			NUnit.Framework.Assert.AreEqual("[f1.txt, mode:100644, content:c1']" + Expected_f3_idx
				(state), IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_12_f2_14(Git git, int state)
		{
			ExecuteAndCheck_f1_6_f2_14(git, state);
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_13(Git git, int state)
		{
			ExecuteAndCheck_f1_8(git, state);
		}

		/// <exception cref="System.Exception"></exception>
		private void ExecuteAndCheck_f1_14(Git git, int state)
		{
			git.Commit().SetOnly(F1).SetMessage(MSG).Call();
			NUnit.Framework.Assert.AreEqual("c1''", GetHead(git, F1));
			NUnit.Framework.Assert.AreEqual(Expected_f3_head(state), GetHead(git, F3));
			NUnit.Framework.Assert.AreEqual("[f1.txt, mode:100644, content:c1'']" + Expected_f3_idx
				(state), IndexState(CONTENT));
		}

		/// <exception cref="System.Exception"></exception>
		private void Prepare_f3(Git git, int state)
		{
			Prepare_f3_f2_14(git, state, false);
		}

		/// <exception cref="System.Exception"></exception>
		private void Prepare_f3_f2_14(Git git, int state)
		{
			Prepare_f3_f2_14(git, state, true);
		}

		/// <exception cref="System.Exception"></exception>
		private void Prepare_f3_f2_14(Git git, int state, bool include_f2)
		{
			FilePath f2 = null;
			if (include_f2)
			{
				f2 = WriteTrashFile(F2, "c2");
				git.Add().AddFilepattern(F2).Call();
				git.Commit().SetMessage(MSG).Call();
			}
			if (state >= 1)
			{
				WriteTrashFile(F3, "c3");
				git.Add().AddFilepattern(F3).Call();
			}
			if (state >= 2)
			{
				git.Commit().SetMessage(MSG).Call();
			}
			if (state >= 3)
			{
				git.Rm().AddFilepattern(F3).Call();
			}
			if (state == 4)
			{
				WriteTrashFile(F3, "c3'");
				git.Add().AddFilepattern(F3).Call();
			}
			if (include_f2)
			{
				Write(f2, "c2'");
				git.Add().AddFilepattern(F2).Call();
				Write(f2, "c2''");
			}
		}

		private string Expected_f3_head(int state)
		{
			switch (state)
			{
				case 0:
				case 1:
				{
					return string.Empty;
				}

				case 2:
				case 3:
				case 4:
				{
					return "c3";
				}
			}
			return null;
		}

		private string Expected_f3_idx(int state)
		{
			switch (state)
			{
				case 0:
				case 3:
				{
					return string.Empty;
				}

				case 1:
				case 2:
				{
					return "[f3.txt, mode:100644, content:c3]";
				}

				case 4:
				{
					return "[f3.txt, mode:100644, content:c3']";
				}
			}
			return null;
		}

		/// <exception cref="System.Exception"></exception>
		private string GetHead(Git git, string path)
		{
			try
			{
				Repository repo = git.GetRepository();
				ObjectId headId = repo.Resolve(Constants.HEAD + "^{commit}");
				TreeWalk tw = TreeWalk.ForPath(repo, path, new RevWalk(repo).ParseTree(headId));
				return Sharpen.Runtime.GetStringForBytes(tw.ObjectReader.Open(tw.GetObjectId(0)).
					GetBytes());
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}
}
