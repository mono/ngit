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

using NGit.Revplot;
using NGit.Revwalk;
using NUnit.Framework;
using Sharpen;

namespace NGit.Revplot
{
	[NUnit.Framework.TestFixture]
	public class PlotCommitListTest : RevWalkTestCase
	{
		internal class CommitListAssert
		{
			private PlotCommitList<PlotLane> pcl;

			private PlotCommit<PlotLane> current;

			private int nextIndex = 0;

			internal CommitListAssert(PlotCommitListTest _enclosing, PlotCommitList<PlotLane>
				 pcl)
			{
				this._enclosing = _enclosing;
				this.pcl = pcl;
			}

			public virtual PlotCommitListTest.CommitListAssert Commit(RevCommit id)
			{
				NUnit.Framework.Assert.IsTrue(this.pcl.Count > this.nextIndex, "Unexpected end of list at pos#"
					 + this.nextIndex);
				this.current = this.pcl[this.nextIndex++];
				NUnit.Framework.Assert.AreEqual(id.Id, this.current.Id, "Expected commit not found at pos#"
					 + (this.nextIndex - 1));
				return this;
			}

			public virtual PlotCommitListTest.CommitListAssert LanePos(int pos)
			{
				PlotLane lane = this.current.GetLane();
				NUnit.Framework.Assert.AreEqual(pos, lane.GetPosition(), "Position of lane of commit #"
					 + (this.nextIndex - 1) + " not as expected.");
				return this;
			}

			public virtual PlotCommitListTest.CommitListAssert Parents(params RevCommit[] parents
				)
			{
				NUnit.Framework.Assert.AreEqual(parents.Length, this.current.ParentCount, "Number of parents of commit #"
					 + (this.nextIndex - 1) + " not as expected.");
				for (int i = 0; i < parents.Length; i++)
				{
					NUnit.Framework.Assert.AreEqual(parents[i], this.current.GetParent(i), "Unexpected parent of commit #"
						 + (this.nextIndex - 1));
				}
				return this;
			}

			public virtual PlotCommitListTest.CommitListAssert NoMoreCommits()
			{
				NUnit.Framework.Assert.AreEqual(this.nextIndex, this.pcl.Count, "Unexpected size of list"
					);
				return this;
			}

			private readonly PlotCommitListTest _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLinear()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			PlotWalk pw = new PlotWalk(db);
			pw.MarkStart(pw.LookupCommit(c.Id));
			PlotCommitList<PlotLane> pcl = new PlotCommitList<PlotLane>();
			pcl.Source(pw);
			pcl.FillTo(int.MaxValue);
			PlotCommitListTest.CommitListAssert test = new PlotCommitListTest.CommitListAssert
				(this, pcl);
			test.Commit(c).LanePos(0).Parents(b);
			test.Commit(b).LanePos(0).Parents(a);
			test.Commit(a).LanePos(0).Parents();
			test.NoMoreCommits();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMerged()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(a);
			RevCommit d = Commit(b, c);
			PlotWalk pw = new PlotWalk(db);
			pw.MarkStart(pw.LookupCommit(d.Id));
			PlotCommitList<PlotLane> pcl = new PlotCommitList<PlotLane>();
			pcl.Source(pw);
			pcl.FillTo(int.MaxValue);
			PlotCommitListTest.CommitListAssert test = new PlotCommitListTest.CommitListAssert
				(this, pcl);
			test.Commit(d).LanePos(0).Parents(b, c);
			test.Commit(c).LanePos(0).Parents(a);
			test.Commit(b).LanePos(1).Parents(a);
			test.Commit(a).LanePos(0).Parents();
			test.NoMoreCommits();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSideBranch()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(a);
			PlotWalk pw = new PlotWalk(db);
			pw.MarkStart(pw.LookupCommit(b.Id));
			pw.MarkStart(pw.LookupCommit(c.Id));
			PlotCommitList<PlotLane> pcl = new PlotCommitList<PlotLane>();
			pcl.Source(pw);
			pcl.FillTo(int.MaxValue);
			PlotCommitListTest.CommitListAssert test = new PlotCommitListTest.CommitListAssert
				(this, pcl);
			test.Commit(c).LanePos(0).Parents(a);
			test.Commit(b).LanePos(1).Parents(a);
			test.Commit(a).LanePos(0).Parents();
			test.NoMoreCommits();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void Test2SideBranches()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(a);
			RevCommit d = Commit(a);
			PlotWalk pw = new PlotWalk(db);
			pw.MarkStart(pw.LookupCommit(b.Id));
			pw.MarkStart(pw.LookupCommit(c.Id));
			pw.MarkStart(pw.LookupCommit(d.Id));
			PlotCommitList<PlotLane> pcl = new PlotCommitList<PlotLane>();
			pcl.Source(pw);
			pcl.FillTo(int.MaxValue);
			PlotCommitListTest.CommitListAssert test = new PlotCommitListTest.CommitListAssert
				(this, pcl);
			test.Commit(d).LanePos(0).Parents(a);
			test.Commit(c).LanePos(1).Parents(a);
			test.Commit(b).LanePos(1).Parents(a);
			test.Commit(a).LanePos(0).Parents();
			test.NoMoreCommits();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBug300282_1()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(a);
			RevCommit d = Commit(a);
			RevCommit e = Commit(a);
			RevCommit f = Commit(a);
			RevCommit g = Commit(f);
			PlotWalk pw = new PlotWalk(db);
			// TODO: when we add unnecessary commit's as tips (e.g. a commit which
			// is a parent of another tip) the walk will return those commits twice.
			// Find out why!
			// pw.markStart(pw.lookupCommit(a.getId()));
			pw.MarkStart(pw.LookupCommit(b.Id));
			pw.MarkStart(pw.LookupCommit(c.Id));
			pw.MarkStart(pw.LookupCommit(d.Id));
			pw.MarkStart(pw.LookupCommit(e.Id));
			// pw.markStart(pw.lookupCommit(f.getId()));
			pw.MarkStart(pw.LookupCommit(g.Id));
			PlotCommitList<PlotLane> pcl = new PlotCommitList<PlotLane>();
			pcl.Source(pw);
			pcl.FillTo(int.MaxValue);
			PlotCommitListTest.CommitListAssert test = new PlotCommitListTest.CommitListAssert
				(this, pcl);
			test.Commit(g).LanePos(0).Parents(f);
			test.Commit(f).LanePos(0).Parents(a);
			test.Commit(e).LanePos(1).Parents(a);
			test.Commit(d).LanePos(1).Parents(a);
			test.Commit(c).LanePos(1).Parents(a);
			test.Commit(b).LanePos(1).Parents(a);
			test.Commit(a).LanePos(0).Parents();
			test.NoMoreCommits();
		}

		// test the history of the egit project between 9fdaf3c1 and e76ad9170f
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEgitHistory()
		{
			RevCommit merge_fix = Commit();
			RevCommit add_simple = Commit(merge_fix);
			RevCommit remove_unused = Commit(merge_fix);
			RevCommit merge_remove = Commit(add_simple, remove_unused);
			RevCommit resolve_handler = Commit(merge_fix);
			RevCommit clear_repositorycache = Commit(merge_remove);
			RevCommit add_Maven = Commit(clear_repositorycache);
			RevCommit use_remote = Commit(clear_repositorycache);
			RevCommit findToolBar_layout = Commit(clear_repositorycache);
			RevCommit merge_add_Maven = Commit(findToolBar_layout, add_Maven);
			RevCommit update_eclipse_iplog = Commit(merge_add_Maven);
			RevCommit changeset_implementation = Commit(clear_repositorycache);
			RevCommit merge_use_remote = Commit(update_eclipse_iplog, use_remote);
			RevCommit disable_source = Commit(merge_use_remote);
			RevCommit update_eclipse_iplog2 = Commit(merge_use_remote);
			RevCommit merge_disable_source = Commit(update_eclipse_iplog2, disable_source);
			RevCommit merge_changeset_implementation = Commit(merge_disable_source, changeset_implementation
				);
			RevCommit clone_operation = Commit(merge_disable_source, merge_changeset_implementation
				);
			RevCommit update_eclipse = Commit(add_Maven);
			RevCommit merge_resolve_handler = Commit(clone_operation, resolve_handler);
			RevCommit disable_comment = Commit(clone_operation);
			RevCommit merge_disable_comment = Commit(merge_resolve_handler, disable_comment);
			RevCommit fix_broken = Commit(merge_disable_comment);
			RevCommit add_a_clear = Commit(fix_broken);
			RevCommit merge_update_eclipse = Commit(add_a_clear, update_eclipse);
			RevCommit sort_roots = Commit(merge_update_eclipse);
			RevCommit fix_logged_npe = Commit(merge_changeset_implementation);
			RevCommit merge_fixed_logged_npe = Commit(sort_roots, fix_logged_npe);
			PlotWalk pw = new PlotWalk(db);
			pw.MarkStart(pw.LookupCommit(merge_fixed_logged_npe.Id));
			PlotCommitList<PlotLane> pcl = new PlotCommitList<PlotLane>();
			pcl.Source(pw);
			pcl.FillTo(int.MaxValue);
			PlotCommitListTest.CommitListAssert test = new PlotCommitListTest.CommitListAssert
				(this, pcl);
			test.Commit(merge_fixed_logged_npe).Parents(sort_roots, fix_logged_npe).LanePos(0
				);
			test.Commit(fix_logged_npe).Parents(merge_changeset_implementation).LanePos(0);
			test.Commit(sort_roots).Parents(merge_update_eclipse).LanePos(1);
			test.Commit(merge_update_eclipse).Parents(add_a_clear, update_eclipse).LanePos(1);
			test.Commit(add_a_clear).Parents(fix_broken).LanePos(1);
			test.Commit(fix_broken).Parents(merge_disable_comment).LanePos(1);
			test.Commit(merge_disable_comment).Parents(merge_resolve_handler, disable_comment
				).LanePos(1);
			test.Commit(disable_comment).Parents(clone_operation).LanePos(1);
			test.Commit(merge_resolve_handler).Parents(clone_operation, resolve_handler).LanePos
				(2);
			test.Commit(update_eclipse).Parents(add_Maven).LanePos(3);
			test.Commit(clone_operation).Parents(merge_disable_source, merge_changeset_implementation
				).LanePos(1);
			test.Commit(merge_changeset_implementation).Parents(merge_disable_source, changeset_implementation
				).LanePos(0);
			test.Commit(merge_disable_source).Parents(update_eclipse_iplog2, disable_source).
				LanePos(1);
			test.Commit(update_eclipse_iplog2).Parents(merge_use_remote).LanePos(0);
			test.Commit(disable_source).Parents(merge_use_remote).LanePos(1);
			test.Commit(merge_use_remote).Parents(update_eclipse_iplog, use_remote).LanePos(0
				);
			test.Commit(changeset_implementation).Parents(clear_repositorycache).LanePos(2);
			test.Commit(update_eclipse_iplog).Parents(merge_add_Maven).LanePos(0);
			test.Commit(merge_add_Maven).Parents(findToolBar_layout, add_Maven).LanePos(0);
			test.Commit(findToolBar_layout).Parents(clear_repositorycache).LanePos(0);
			test.Commit(use_remote).Parents(clear_repositorycache).LanePos(1);
			test.Commit(add_Maven).Parents(clear_repositorycache).LanePos(3);
			test.Commit(clear_repositorycache).Parents(merge_remove).LanePos(2);
			test.Commit(resolve_handler).Parents(merge_fix).LanePos(4);
			test.Commit(merge_remove).Parents(add_simple, remove_unused).LanePos(2);
			test.Commit(remove_unused).Parents(merge_fix).LanePos(0);
			test.Commit(add_simple).Parents(merge_fix).LanePos(1);
			test.Commit(merge_fix).Parents().LanePos(3);
			test.NoMoreCommits();
		}
	}
}
