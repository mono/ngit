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
		protected override void SetUp()
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
