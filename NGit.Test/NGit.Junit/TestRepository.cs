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
using System.Collections.Generic;
using System.IO;
using System.Text;
using NGit;
using NGit.Dircache;
using NGit.Errors;
using NGit.Junit;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Storage.Pack;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using NUnit.Framework;
using R = NGit.Repository;
using Sharpen;

namespace NGit.Junit
{
	/// <summary>Wrapper to make creating test data easier.</summary>
	/// <remarks>Wrapper to make creating test data easier.</remarks>
	/// <?></?>
	public class TestRepository<T>: TestRepository
	{
		public TestRepository(R db) : base(db)
		{
		}

		public TestRepository(R db, RevWalk rw) : base (db, rw)
		{
		}
	}
	
	public class TestRepository
	{
		private static readonly PersonIdent author;

		private static readonly PersonIdent committer;

		static TestRepository()
		{
			MockSystemReader m = new MockSystemReader();
			long now = m.GetCurrentTime();
			int tz = m.GetTimezone(now);
			string an = "J. Author";
			string ae = "jauthor@example.com";
			author = new PersonIdent(an, ae, now, tz);
			string cn = "J. Committer";
			string ce = "jcommitter@example.com";
			committer = new PersonIdent(cn, ce, now, tz);
		}

		private readonly R db;

		private readonly RevWalk pool;

		private readonly ObjectInserter inserter;

		private long now;

		/// <summary>Wrap a repository with test building tools.</summary>
		/// <remarks>Wrap a repository with test building tools.</remarks>
		/// <param name="db">the test repository to write into.</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public TestRepository(R db) : this(db, new RevWalk(db))
		{
		}

		/// <summary>Wrap a repository with test building tools.</summary>
		/// <remarks>Wrap a repository with test building tools.</remarks>
		/// <param name="db">the test repository to write into.</param>
		/// <param name="rw">the RevObject pool to use for object lookup.</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public TestRepository(R db, RevWalk rw)
		{
			this.db = db;
			this.pool = rw;
			this.inserter = db.NewObjectInserter();
			this.now = 1236977987000L;
		}

		/// <returns>the repository this helper class operates against.</returns>
		public virtual R GetRepository()
		{
			return db;
		}

		/// <returns>get the RevWalk pool all objects are allocated through.</returns>
		public virtual RevWalk GetRevWalk()
		{
			return pool;
		}

		/// <returns>
		/// current time adjusted by
		/// <see cref="TestRepository{R}.Tick(int)">TestRepository&lt;R&gt;.Tick(int)</see>
		/// .
		/// </returns>
		public virtual DateTime GetClock()
		{
			return Sharpen.Extensions.CreateDate(now);
		}

		/// <summary>Adjust the current time that will used by the next commit.</summary>
		/// <remarks>Adjust the current time that will used by the next commit.</remarks>
		/// <param name="secDelta">number of seconds to add to the current time.</param>
		public virtual void Tick(int secDelta)
		{
			now += secDelta * 1000L;
		}

		/// <summary>
		/// Set the author and committer using
		/// <see cref="TestRepository{R}.GetClock()">TestRepository&lt;R&gt;.GetClock()</see>
		/// .
		/// </summary>
		/// <param name="c">the commit builder to store.</param>
		public virtual void SetAuthorAndCommitter(NGit.CommitBuilder c)
		{
			c.Author = new PersonIdent(author, Sharpen.Extensions.CreateDate(now));
			c.Committer = new PersonIdent(committer, Sharpen.Extensions.CreateDate(now));
		}

		/// <summary>Create a new blob object in the repository.</summary>
		/// <remarks>Create a new blob object in the repository.</remarks>
		/// <param name="content">file content, will be UTF-8 encoded.</param>
		/// <returns>reference to the blob.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual RevBlob Blob(string content)
		{
			return Blob(Sharpen.Runtime.GetBytesForString(content, "UTF-8"));
		}

		/// <summary>Create a new blob object in the repository.</summary>
		/// <remarks>Create a new blob object in the repository.</remarks>
		/// <param name="content">binary file content.</param>
		/// <returns>reference to the blob.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual RevBlob Blob(byte[] content)
		{
			ObjectId id;
			try
			{
				id = inserter.Insert(Constants.OBJ_BLOB, content);
				inserter.Flush();
			}
			finally
			{
				inserter.Release();
			}
			return pool.LookupBlob(id);
		}

		/// <summary>Construct a regular file mode tree entry.</summary>
		/// <remarks>Construct a regular file mode tree entry.</remarks>
		/// <param name="path">path of the file.</param>
		/// <param name="blob">a blob, previously constructed in the repository.</param>
		/// <returns>the entry.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual DirCacheEntry File(string path, RevBlob blob)
		{
			DirCacheEntry e = new DirCacheEntry(path);
			e.SetFileMode(FileMode.REGULAR_FILE);
			e.SetObjectId(blob);
			return e;
		}

		/// <summary>Construct a tree from a specific listing of file entries.</summary>
		/// <remarks>Construct a tree from a specific listing of file entries.</remarks>
		/// <param name="entries">
		/// the files to include in the tree. The collection does not need
		/// to be sorted properly and may be empty.
		/// </param>
		/// <returns>reference to the tree specified by the entry list.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual RevTree Tree(params DirCacheEntry[] entries)
		{
			DirCache dc = DirCache.NewInCore();
			DirCacheBuilder b = dc.Builder();
			foreach (DirCacheEntry e in entries)
			{
				b.Add(e);
			}
			b.Finish();
			ObjectId root;
			try
			{
				root = dc.WriteTree(inserter);
				inserter.Flush();
			}
			finally
			{
				inserter.Release();
			}
			return pool.LookupTree(root);
		}

		/// <summary>Lookup an entry stored in a tree, failing if not present.</summary>
		/// <remarks>Lookup an entry stored in a tree, failing if not present.</remarks>
		/// <param name="tree">the tree to search.</param>
		/// <param name="path">the path to find the entry of.</param>
		/// <returns>the parsed object entry at this path, never null.</returns>
		/// <exception cref="NUnit.Framework.AssertionFailedError">if the path does not exist in the given tree.
		/// 	</exception>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual RevObject Get(RevTree tree, string path)
		{
			TreeWalk tw = new TreeWalk(pool.GetObjectReader());
			tw.Filter = PathFilterGroup.CreateFromStrings(Collections.Singleton(path));
			tw.Reset(tree);
			while (tw.Next())
			{
				if (tw.IsSubtree && !path.Equals(tw.PathString))
				{
					tw.EnterSubtree();
					continue;
				}
				ObjectId entid = tw.GetObjectId(0);
				FileMode entmode = tw.GetFileMode(0);
				return pool.LookupAny(entid, entmode.GetObjectType());
			}
			NUnit.Framework.Assert.Fail("Can't find " + path + " in tree " + tree.Name);
			return null;
		}

		// never reached.
		/// <summary>Create a new commit.</summary>
		/// <remarks>
		/// Create a new commit.
		/// <p>
		/// See
		/// <see cref="TestRepository{R}.Commit(int, NGit.Revwalk.RevTree, org.eclipse.jgit.revwalk.RevCommit[])
		/// 	">TestRepository&lt;R&gt;.Commit(int, NGit.Revwalk.RevTree, org.eclipse.jgit.revwalk.RevCommit[])
		/// 	</see>
		/// . The tree is the empty
		/// tree (no files or subdirectories).
		/// </remarks>
		/// <param name="parents">zero or more parents of the commit.</param>
		/// <returns>the new commit.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual RevCommit Commit(params RevCommit[] parents)
		{
			return Commit(1, Tree(), parents);
		}

		/// <summary>Create a new commit.</summary>
		/// <remarks>
		/// Create a new commit.
		/// <p>
		/// See
		/// <see cref="TestRepository{R}.Commit(int, NGit.Revwalk.RevTree, org.eclipse.jgit.revwalk.RevCommit[])
		/// 	">TestRepository&lt;R&gt;.Commit(int, NGit.Revwalk.RevTree, org.eclipse.jgit.revwalk.RevCommit[])
		/// 	</see>
		/// .
		/// </remarks>
		/// <param name="tree">the root tree for the commit.</param>
		/// <param name="parents">zero or more parents of the commit.</param>
		/// <returns>the new commit.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual RevCommit Commit(RevTree tree, params RevCommit[] parents)
		{
			return Commit(1, tree, parents);
		}

		/// <summary>Create a new commit.</summary>
		/// <remarks>
		/// Create a new commit.
		/// <p>
		/// See
		/// <see cref="TestRepository{R}.Commit(int, NGit.Revwalk.RevTree, org.eclipse.jgit.revwalk.RevCommit[])
		/// 	">TestRepository&lt;R&gt;.Commit(int, NGit.Revwalk.RevTree, org.eclipse.jgit.revwalk.RevCommit[])
		/// 	</see>
		/// . The tree is the empty
		/// tree (no files or subdirectories).
		/// </remarks>
		/// <param name="secDelta">
		/// number of seconds to advance
		/// <see cref="TestRepository{R}.Tick(int)">TestRepository&lt;R&gt;.Tick(int)</see>
		/// by.
		/// </param>
		/// <param name="parents">zero or more parents of the commit.</param>
		/// <returns>the new commit.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual RevCommit Commit(int secDelta, params RevCommit[] parents)
		{
			return Commit(secDelta, Tree(), parents);
		}

		/// <summary>Create a new commit.</summary>
		/// <remarks>
		/// Create a new commit.
		/// <p>
		/// The author and committer identities are stored using the current
		/// timestamp, after being incremented by
		/// <code>secDelta</code>
		/// . The message body
		/// is empty.
		/// </remarks>
		/// <param name="secDelta">
		/// number of seconds to advance
		/// <see cref="TestRepository{R}.Tick(int)">TestRepository&lt;R&gt;.Tick(int)</see>
		/// by.
		/// </param>
		/// <param name="tree">the root tree for the commit.</param>
		/// <param name="parents">zero or more parents of the commit.</param>
		/// <returns>the new commit.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual RevCommit Commit(int secDelta, RevTree tree, params RevCommit[] parents
			)
		{
			Tick(secDelta);
			NGit.CommitBuilder c;
			c = new NGit.CommitBuilder();
			c.TreeId = tree;
			c.SetParentIds((IList<RevCommit>)parents);
			c.Author = new PersonIdent(author, Sharpen.Extensions.CreateDate(now));
			c.Committer = new PersonIdent(committer, Sharpen.Extensions.CreateDate(now));
			c.Message = string.Empty;
			ObjectId id;
			try
			{
				id = inserter.Insert(c);
				inserter.Flush();
			}
			finally
			{
				inserter.Release();
			}
			return pool.LookupCommit(id);
		}

		/// <returns>a new commit builder.</returns>
		public virtual TestRepository.CommitBuilder Commit()
		{
			return new TestRepository.CommitBuilder(this);
		}

		/// <summary>Construct an annotated tag object pointing at another object.</summary>
		/// <remarks>
		/// Construct an annotated tag object pointing at another object.
		/// <p>
		/// The tagger is the committer identity, at the current time as specified by
		/// <see cref="TestRepository{R}.Tick(int)">TestRepository&lt;R&gt;.Tick(int)</see>
		/// . The time is not increased.
		/// <p>
		/// The tag message is empty.
		/// </remarks>
		/// <param name="name">
		/// name of the tag. Traditionally a tag name should not start
		/// with
		/// <code>refs/tags/</code>
		/// .
		/// </param>
		/// <param name="dst">object the tag should be pointed at.</param>
		/// <returns>the annotated tag object.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual RevTag Tag(string name, RevObject dst)
		{
			TagBuilder t = new TagBuilder();
			t.SetObjectId(dst);
			t.SetTag(name);
			t.SetTagger(new PersonIdent(committer, Sharpen.Extensions.CreateDate(now)));
			t.SetMessage(string.Empty);
			ObjectId id;
			try
			{
				id = inserter.Insert(t);
				inserter.Flush();
			}
			finally
			{
				inserter.Release();
			}
			return (RevTag)pool.LookupAny(id, Constants.OBJ_TAG);
		}

		/// <summary>Update a reference to point to an object.</summary>
		/// <remarks>Update a reference to point to an object.</remarks>
		/// <param name="ref">
		/// the name of the reference to update to. If
		/// <code>ref</code>
		/// does
		/// not start with
		/// <code>refs/</code>
		/// and is not the magic names
		/// <code>HEAD</code>
		/// 
		/// <code>FETCH_HEAD</code>
		/// or
		/// <code>MERGE_HEAD</code>
		/// , then
		/// <code>refs/heads/</code>
		/// will be prefixed in front of the given
		/// name, thereby assuming it is a branch.
		/// </param>
		/// <param name="to">the target object.</param>
		/// <returns>the target object.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual RevCommit Update(string @ref, TestRepository.CommitBuilder to)
		{
			return Update(@ref, to.Create());
		}

		/// <summary>Update a reference to point to an object.</summary>
		/// <remarks>Update a reference to point to an object.</remarks>
		/// <?></?>
		/// <param name="ref">
		/// the name of the reference to update to. If
		/// <code>ref</code>
		/// does
		/// not start with
		/// <code>refs/</code>
		/// and is not the magic names
		/// <code>HEAD</code>
		/// 
		/// <code>FETCH_HEAD</code>
		/// or
		/// <code>MERGE_HEAD</code>
		/// , then
		/// <code>refs/heads/</code>
		/// will be prefixed in front of the given
		/// name, thereby assuming it is a branch.
		/// </param>
		/// <param name="obj">the target object.</param>
		/// <returns>the target object.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual T Update<T>(string @ref, T obj) where T:AnyObjectId
		{
			if (Constants.HEAD.Equals(@ref))
			{
			}
			else
			{
				if ("FETCH_HEAD".Equals(@ref))
				{
				}
				else
				{
					if ("MERGE_HEAD".Equals(@ref))
					{
					}
					else
					{
						if (@ref.StartsWith(Constants.R_REFS))
						{
						}
						else
						{
							@ref = Constants.R_HEADS + @ref;
						}
					}
				}
			}
			RefUpdate u = db.UpdateRef(@ref);
			u.SetNewObjectId(obj);
			switch (u.ForceUpdate())
			{
				case RefUpdate.Result.FAST_FORWARD:
				case RefUpdate.Result.FORCED:
				case RefUpdate.Result.NEW:
				case RefUpdate.Result.NO_CHANGE:
				{
					UpdateServerInfo();
					return obj;
				}

				default:
				{
					throw new IOException("Cannot write " + @ref + " " + u.GetResult());
				}
			}
		}

		/// <summary>Update the dumb client server info files.</summary>
		/// <remarks>Update the dumb client server info files.</remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual void UpdateServerInfo()
		{
			if (db is FileRepository)
			{
				FileRepository fr = (FileRepository)db;
				RefWriter rw = new _RefWriter_492(this, fr, fr.GetAllRefs().Values);
				rw.WritePackedRefs();
				rw.WriteInfoRefs();
				StringBuilder w = new StringBuilder();
				foreach (PackFile p in ((ObjectDirectory)fr.ObjectDatabase).GetPacks())
				{
					w.Append("P ");
					w.Append(p.GetPackFile().GetName());
					w.Append('\n');
				}
				WriteFile(new FilePath(new FilePath(((ObjectDirectory)fr.ObjectDatabase).GetDirectory
					(), "info"), "packs"), Constants.EncodeASCII(w.ToString()));
			}
		}

		private sealed class _RefWriter_492 : RefWriter
		{
			public _RefWriter_492(TestRepository _enclosing, FileRepository fr, ICollection
				<Ref> baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
				this.fr = fr;
			}

			/// <exception cref="System.IO.IOException"></exception>
			protected override void WriteFile(string name, byte[] bin)
			{
				FilePath path = new FilePath(fr.Directory, name);
				this._enclosing.WriteFile(path, bin);
			}

			private readonly TestRepository _enclosing;

			private readonly FileRepository fr;
		}

		/// <summary>Ensure the body of the given object has been parsed.</summary>
		/// <remarks>Ensure the body of the given object has been parsed.</remarks>
		/// <?></?>
		/// <param name="object">
		/// reference to the (possibly unparsed) object to force body
		/// parsing of.
		/// </param>
		/// <returns>
		/// 
		/// <code>object</code>
		/// </returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual T ParseBody<T>(T @object) where T:RevObject
		{
			pool.ParseBody(@object);
			return @object;
		}

		/// <summary>Create a new branch builder for this repository.</summary>
		/// <remarks>Create a new branch builder for this repository.</remarks>
		/// <param name="ref">
		/// name of the branch to be constructed. If
		/// <code>ref</code>
		/// does not
		/// start with
		/// <code>refs/</code>
		/// the prefix
		/// <code>refs/heads/</code>
		/// will
		/// be added.
		/// </param>
		/// <returns>builder for the named branch.</returns>
		public virtual TestRepository.BranchBuilder Branch(string @ref)
		{
			if (Constants.HEAD.Equals(@ref))
			{
			}
			else
			{
				if (@ref.StartsWith(Constants.R_REFS))
				{
				}
				else
				{
					@ref = Constants.R_HEADS + @ref;
				}
			}
			return new TestRepository.BranchBuilder(this, @ref);
		}

		/// <summary>Run consistency checks against the object database.</summary>
		/// <remarks>
		/// Run consistency checks against the object database.
		/// <p>
		/// This method completes silently if the checks pass. A temporary revision
		/// pool is constructed during the checking.
		/// </remarks>
		/// <param name="tips">
		/// the tips to start checking from; if not supplied the refs of
		/// the repository are used instead.
		/// </param>
		/// <exception cref="NGit.Errors.MissingObjectException">NGit.Errors.MissingObjectException
		/// 	</exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException">NGit.Errors.IncorrectObjectTypeException
		/// 	</exception>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void Fsck(params RevObject[] tips)
		{
			ObjectWalk ow = new ObjectWalk(db);
			if (tips.Length != 0)
			{
				foreach (RevObject o in tips)
				{
					ow.MarkStart(ow.ParseAny(o));
				}
			}
			else
			{
				foreach (Ref r in db.GetAllRefs().Values)
				{
					ow.MarkStart(ow.ParseAny(r.GetObjectId()));
				}
			}
			ObjectChecker oc = new ObjectChecker();
			for (; ; )
			{
				RevCommit o = ow.Next();
				if (o == null)
				{
					break;
				}
				byte[] bin = db.Open(o, o.Type).GetCachedBytes();
				oc.CheckCommit(bin);
				AssertHash(o, bin);
			}
			for (; ; )
			{
				RevObject o = ow.NextObject();
				if (o == null)
				{
					break;
				}
				byte[] bin = db.Open(o, o.Type).GetCachedBytes();
				oc.Check(o.Type, bin);
				AssertHash(o, bin);
			}
		}

		private static void AssertHash(RevObject id, byte[] bin)
		{
			MessageDigest md = Constants.NewMessageDigest();
			md.Update(Constants.EncodedTypeString(id.Type));
			md.Update(unchecked((byte)' '));
			md.Update(Constants.EncodeASCII(bin.Length));
			md.Update(unchecked((byte)0));
			md.Update(bin);
			NUnit.Framework.Assert.AreEqual(id, ObjectId.FromRaw(md.Digest()));
		}

		/// <summary>Pack all reachable objects in the repository into a single pack file.</summary>
		/// <remarks>
		/// Pack all reachable objects in the repository into a single pack file.
		/// <p>
		/// All loose objects are automatically pruned. Existing packs however are
		/// not removed.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual void PackAndPrune()
		{
			if (db.ObjectDatabase is ObjectDirectory)
			{
				ObjectDirectory odb = (ObjectDirectory)db.ObjectDatabase;
				NullProgressMonitor m = NullProgressMonitor.INSTANCE;
				FilePath pack;
				FilePath idx;
				PackWriter pw = new PackWriter(db);
				try
				{
					ICollection<ObjectId> all = new HashSet<ObjectId>();
					foreach (Ref r in db.GetAllRefs().Values)
					{
						all.AddItem(r.GetObjectId());
					}
					pw.PreparePack(m, all, Sharpen.Collections.EmptySet<ObjectId>());
					ObjectId name = pw.ComputeName();
					OutputStream @out;
					pack = NameFor(odb, name, ".pack");
					@out = new BufferedOutputStream(new FileOutputStream(pack));
					try
					{
						pw.WritePack(m, m, @out);
					}
					finally
					{
						@out.Close();
					}
					pack.SetReadOnly();
					idx = NameFor(odb, name, ".idx");
					@out = new BufferedOutputStream(new FileOutputStream(idx));
					try
					{
						pw.WriteIndex(@out);
					}
					finally
					{
						@out.Close();
					}
					idx.SetReadOnly();
				}
				finally
				{
					pw.Release();
				}
				odb.OpenPack(pack, idx);
				UpdateServerInfo();
				PrunePacked(odb);
			}
		}

		private void PrunePacked(ObjectDirectory odb)
		{
			foreach (PackFile p in odb.GetPacks())
			{
				foreach (PackIndex.MutableEntry e in p)
				{
					odb.FileFor(e.ToObjectId()).Delete();
				}
			}
		}

		private static FilePath NameFor(ObjectDirectory odb, ObjectId name, string t)
		{
			FilePath packdir = new FilePath(odb.GetDirectory(), "pack");
			return new FilePath(packdir, "pack-" + name.Name + t);
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ObjectWritingException"></exception>
		private void WriteFile(FilePath p, byte[] bin)
		{
			LockFile lck = new LockFile(p, db.FileSystem);
			if (!lck.Lock())
			{
				throw new ObjectWritingException("Can't write " + p);
			}
			try
			{
				lck.Write(bin);
			}
			catch (IOException)
			{
				throw new ObjectWritingException("Can't write " + p);
			}
			if (!lck.Commit())
			{
				throw new ObjectWritingException("Can't write " + p);
			}
		}

		/// <summary>Helper to build a branch with one or more commits</summary>
		public class BranchBuilder
		{
			internal readonly string @ref;

			internal BranchBuilder(TestRepository _enclosing, string @ref)
			{
				this._enclosing = _enclosing;
				this.@ref = @ref;
			}

			/// <returns>
			/// construct a new commit builder that updates this branch. If
			/// the branch already exists, the commit builder will have its
			/// first parent as the current commit and its tree will be
			/// initialized to the current files.
			/// </returns>
			/// <exception cref="System.Exception">the commit builder can't read the current branch state
			/// 	</exception>
			public virtual TestRepository.CommitBuilder Commit()
			{
				return new TestRepository.CommitBuilder(_enclosing, this);
			}

			/// <summary>Forcefully update this branch to a particular commit.</summary>
			/// <remarks>Forcefully update this branch to a particular commit.</remarks>
			/// <param name="to">the commit to update to.</param>
			/// <returns>
			/// 
			/// <code>to</code>
			/// .
			/// </returns>
			/// <exception cref="System.Exception">System.Exception</exception>
			public virtual RevCommit Update(TestRepository.CommitBuilder to)
			{
				return this.Update(to.Create());
			}

			/// <summary>Forcefully update this branch to a particular commit.</summary>
			/// <remarks>Forcefully update this branch to a particular commit.</remarks>
			/// <param name="to">the commit to update to.</param>
			/// <returns>
			/// 
			/// <code>to</code>
			/// .
			/// </returns>
			/// <exception cref="System.Exception">System.Exception</exception>
			public virtual RevCommit Update(RevCommit to)
			{
				return this._enclosing.Update(this.@ref, to);
			}

			private readonly TestRepository _enclosing;
		}

		/// <summary>Helper to generate a commit.</summary>
		/// <remarks>Helper to generate a commit.</remarks>
		public class CommitBuilder
		{
			private readonly TestRepository.BranchBuilder branch;

			private readonly DirCache tree = DirCache.NewInCore();

			private readonly IList<RevCommit> parents = new AList<RevCommit>(2);

			private int tick = 1;

			private string message = string.Empty;

			private RevCommit self;

			public CommitBuilder(TestRepository _enclosing)
			{
				this._enclosing = _enclosing;
				this.branch = null;
			}

			/// <exception cref="System.Exception"></exception>
			internal CommitBuilder(TestRepository _enclosing, TestRepository.BranchBuilder
				 b)
			{
				this._enclosing = _enclosing;
				this.branch = b;
				Ref @ref = this._enclosing.db.GetRef(this.branch.@ref);
				if (@ref != null)
				{
					this.Parent(this._enclosing.pool.ParseCommit(@ref.GetObjectId()));
				}
			}

			/// <exception cref="System.Exception"></exception>
			internal CommitBuilder(TestRepository _enclosing, TestRepository.CommitBuilder
				 prior)
			{
				this._enclosing = _enclosing;
				this.branch = prior.branch;
				DirCacheBuilder b = this.tree.Builder();
				for (int i = 0; i < prior.tree.GetEntryCount(); i++)
				{
					b.Add(prior.tree.GetEntry(i));
				}
				b.Finish();
				this.parents.AddItem(prior.Create());
			}

			/// <exception cref="System.Exception"></exception>
			public virtual TestRepository.CommitBuilder Parent(RevCommit p)
			{
				if (this.parents.IsEmpty())
				{
					DirCacheBuilder b = this.tree.Builder();
					this._enclosing.ParseBody(p);
					b.AddTree(new byte[0], DirCacheEntry.STAGE_0, this._enclosing.pool.GetObjectReader
						(), p.Tree);
					b.Finish();
				}
				this.parents.AddItem(p);
				return this;
			}

			public virtual TestRepository.CommitBuilder NoParents()
			{
				this.parents.Clear();
				return this;
			}

			public virtual TestRepository.CommitBuilder NoFiles()
			{
				this.tree.Clear();
				return this;
			}

			/// <exception cref="System.Exception"></exception>
			public virtual TestRepository.CommitBuilder Add(string path, string content)
			{
				return this.Add(path, this._enclosing.Blob(content));
			}

			/// <exception cref="System.Exception"></exception>
			public virtual TestRepository.CommitBuilder Add(string path, RevBlob id)
			{
				DirCacheEditor e = this.tree.Editor();
				e.Add(new _PathEdit_792(id, path));
				e.Finish();
				return this;
			}

			private sealed class _PathEdit_792 : DirCacheEditor.PathEdit
			{
				public _PathEdit_792(RevBlob id, string baseArg1) : base(baseArg1)
				{
					this.id = id;
				}

				public override void Apply(DirCacheEntry ent)
				{
					ent.SetFileMode(FileMode.REGULAR_FILE);
					ent.SetObjectId(id);
				}

				private readonly RevBlob id;
			}

			public virtual TestRepository.CommitBuilder Rm(string path)
			{
				DirCacheEditor e = this.tree.Editor();
				e.Add(new DirCacheEditor.DeletePath(path));
				e.Add(new DirCacheEditor.DeleteTree(path));
				e.Finish();
				return this;
			}

			public virtual TestRepository.CommitBuilder Message(string m)
			{
				this.message = m;
				return this;
			}

			public virtual TestRepository.CommitBuilder Tick(int secs)
			{
				this.tick = secs;
				return this;
			}

			/// <exception cref="System.Exception"></exception>
			public virtual RevCommit Create()
			{
				if (this.self == null)
				{
					this._enclosing.Tick(this.tick);
					NGit.CommitBuilder c;
					c = new NGit.CommitBuilder();
					c.SetParentIds(this.parents);
					this._enclosing.SetAuthorAndCommitter(c);
					c.Message = this.message;
					ObjectId commitId;
					try
					{
						c.TreeId = this.tree.WriteTree(this._enclosing.inserter);
						commitId = this._enclosing.inserter.Insert(c);
						this._enclosing.inserter.Flush();
					}
					finally
					{
						this._enclosing.inserter.Release();
					}
					this.self = this._enclosing.pool.LookupCommit(commitId);
					if (this.branch != null)
					{
						this.branch.Update(this.self);
					}
				}
				return this.self;
			}

			/// <exception cref="System.Exception"></exception>
			public virtual TestRepository.CommitBuilder Child()
			{
				return new TestRepository.CommitBuilder(_enclosing, this);
			}

			private readonly TestRepository _enclosing;
		}
	}
}
