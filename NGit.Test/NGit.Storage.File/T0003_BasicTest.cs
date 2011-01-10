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
using System.IO;
using NGit;
using NGit.Revwalk;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	[NUnit.Framework.TestFixture]
	public class T0003_BasicTest : SampleDataRepositoryTestCase
	{
		[NUnit.Framework.Test]
		public virtual void Test001_Initalize()
		{
			FilePath gitdir = new FilePath(trash, Constants.DOT_GIT);
			FilePath objects = new FilePath(gitdir, "objects");
			FilePath objects_pack = new FilePath(objects, "pack");
			FilePath objects_info = new FilePath(objects, "info");
			FilePath refs = new FilePath(gitdir, "refs");
			FilePath refs_heads = new FilePath(refs, "heads");
			FilePath refs_tags = new FilePath(refs, "tags");
			FilePath HEAD = new FilePath(gitdir, "HEAD");
			NUnit.Framework.Assert.IsTrue(trash.IsDirectory(), "Exists " + trash);
			NUnit.Framework.Assert.IsTrue(objects.IsDirectory(), "Exists " + objects);
			NUnit.Framework.Assert.IsTrue(objects_pack.IsDirectory(), "Exists " + objects_pack
				);
			NUnit.Framework.Assert.IsTrue(objects_info.IsDirectory(), "Exists " + objects_info
				);
			NUnit.Framework.Assert.AreEqual(2L, objects.ListFiles().Length);
			NUnit.Framework.Assert.IsTrue(refs.IsDirectory(), "Exists " + refs);
			NUnit.Framework.Assert.IsTrue(refs_heads.IsDirectory(), "Exists " + refs_heads);
			NUnit.Framework.Assert.IsTrue(refs_tags.IsDirectory(), "Exists " + refs_tags);
			NUnit.Framework.Assert.IsTrue(HEAD.IsFile(), "Exists " + HEAD);
			NUnit.Framework.Assert.AreEqual(23, HEAD.Length());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test000_openRepoBadArgs()
		{
			try
			{
				new FileRepositoryBuilder().Build();
				NUnit.Framework.Assert.Fail("Must pass either GIT_DIR or GIT_WORK_TREE");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual(JGitText.Get().eitherGitDirOrWorkTreeRequired, e.
					Message);
			}
		}

		/// <summary>
		/// Check the default rules for looking up directories and files within a
		/// repo when the gitDir is given.
		/// </summary>
		/// <remarks>
		/// Check the default rules for looking up directories and files within a
		/// repo when the gitDir is given.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void Test000_openrepo_default_gitDirSet()
		{
			FilePath repo1Parent = new FilePath(trash.GetParentFile(), "r1");
			Repository repo1initial = new FileRepository(new FilePath(repo1Parent, Constants.
				DOT_GIT));
			repo1initial.Create();
			repo1initial.Close();
			FilePath theDir = new FilePath(repo1Parent, Constants.DOT_GIT);
			FileRepository r = new FileRepositoryBuilder().SetGitDir(theDir).Build();
			AssertEqualsPath(theDir, r.Directory);
			AssertEqualsPath(repo1Parent, r.WorkTree);
			AssertEqualsPath(new FilePath(theDir, "index"), r.GetIndexFile());
			AssertEqualsPath(new FilePath(theDir, "objects"), ((ObjectDirectory)r.ObjectDatabase
				).GetDirectory());
		}

		/// <summary>
		/// Check that we can pass both a git directory and a work tree repo when the
		/// gitDir is given.
		/// </summary>
		/// <remarks>
		/// Check that we can pass both a git directory and a work tree repo when the
		/// gitDir is given.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void Test000_openrepo_default_gitDirAndWorkTreeSet()
		{
			FilePath repo1Parent = new FilePath(trash.GetParentFile(), "r1");
			Repository repo1initial = new FileRepository(new FilePath(repo1Parent, Constants.
				DOT_GIT));
			repo1initial.Create();
			repo1initial.Close();
			FilePath theDir = new FilePath(repo1Parent, Constants.DOT_GIT);
			FileRepository r = new FileRepositoryBuilder().SetGitDir(theDir).SetWorkTree(repo1Parent
				.GetParentFile()).Build();
			AssertEqualsPath(theDir, r.Directory);
			AssertEqualsPath(repo1Parent.GetParentFile(), r.WorkTree);
			AssertEqualsPath(new FilePath(theDir, "index"), r.GetIndexFile());
			AssertEqualsPath(new FilePath(theDir, "objects"), ((ObjectDirectory)r.ObjectDatabase
				).GetDirectory());
		}

		/// <summary>
		/// Check the default rules for looking up directories and files within a
		/// repo when the workTree is given.
		/// </summary>
		/// <remarks>
		/// Check the default rules for looking up directories and files within a
		/// repo when the workTree is given.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void Test000_openrepo_default_workDirSet()
		{
			FilePath repo1Parent = new FilePath(trash.GetParentFile(), "r1");
			Repository repo1initial = new FileRepository(new FilePath(repo1Parent, Constants.
				DOT_GIT));
			repo1initial.Create();
			repo1initial.Close();
			FilePath theDir = new FilePath(repo1Parent, Constants.DOT_GIT);
			FileRepository r = new FileRepositoryBuilder().SetWorkTree(repo1Parent).Build();
			AssertEqualsPath(theDir, r.Directory);
			AssertEqualsPath(repo1Parent, r.WorkTree);
			AssertEqualsPath(new FilePath(theDir, "index"), r.GetIndexFile());
			AssertEqualsPath(new FilePath(theDir, "objects"), ((ObjectDirectory)r.ObjectDatabase
				).GetDirectory());
		}

		/// <summary>Check that worktree config has an effect, given absolute path.</summary>
		/// <remarks>Check that worktree config has an effect, given absolute path.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void Test000_openrepo_default_absolute_workdirconfig()
		{
			FilePath repo1Parent = new FilePath(trash.GetParentFile(), "r1");
			FilePath workdir = new FilePath(trash.GetParentFile(), "rw");
			workdir.Mkdir();
			FileRepository repo1initial = new FileRepository(new FilePath(repo1Parent, Constants
				.DOT_GIT));
			repo1initial.Create();
			FileBasedConfig cfg = ((FileBasedConfig)repo1initial.GetConfig());
			cfg.SetString("core", null, "worktree", workdir.GetAbsolutePath());
			cfg.Save();
			repo1initial.Close();
			FilePath theDir = new FilePath(repo1Parent, Constants.DOT_GIT);
			FileRepository r = new FileRepositoryBuilder().SetGitDir(theDir).Build();
			AssertEqualsPath(theDir, r.Directory);
			AssertEqualsPath(workdir, r.WorkTree);
			AssertEqualsPath(new FilePath(theDir, "index"), r.GetIndexFile());
			AssertEqualsPath(new FilePath(theDir, "objects"), ((ObjectDirectory)r.ObjectDatabase
				).GetDirectory());
		}

		/// <summary>Check that worktree config has an effect, given a relative path.</summary>
		/// <remarks>Check that worktree config has an effect, given a relative path.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void Test000_openrepo_default_relative_workdirconfig()
		{
			FilePath repo1Parent = new FilePath(trash.GetParentFile(), "r1");
			FilePath workdir = new FilePath(trash.GetParentFile(), "rw");
			workdir.Mkdir();
			FileRepository repo1initial = new FileRepository(new FilePath(repo1Parent, Constants
				.DOT_GIT));
			repo1initial.Create();
			FileBasedConfig cfg = ((FileBasedConfig)repo1initial.GetConfig());
			cfg.SetString("core", null, "worktree", "../../rw");
			cfg.Save();
			repo1initial.Close();
			FilePath theDir = new FilePath(repo1Parent, Constants.DOT_GIT);
			FileRepository r = new FileRepositoryBuilder().SetGitDir(theDir).Build();
			AssertEqualsPath(theDir, r.Directory);
			AssertEqualsPath(workdir, r.WorkTree);
			AssertEqualsPath(new FilePath(theDir, "index"), r.GetIndexFile());
			AssertEqualsPath(new FilePath(theDir, "objects"), ((ObjectDirectory)r.ObjectDatabase
				).GetDirectory());
		}

		/// <summary>
		/// Check that the given index file is honored and the alternate object
		/// directories too
		/// </summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void Test000_openrepo_alternate_index_file_and_objdirs()
		{
			FilePath repo1Parent = new FilePath(trash.GetParentFile(), "r1");
			FilePath indexFile = new FilePath(trash, "idx");
			FilePath objDir = new FilePath(trash, "../obj");
			FilePath altObjDir = ((ObjectDirectory)db.ObjectDatabase).GetDirectory();
			Repository repo1initial = new FileRepository(new FilePath(repo1Parent, Constants.
				DOT_GIT));
			repo1initial.Create();
			repo1initial.Close();
			FilePath theDir = new FilePath(repo1Parent, Constants.DOT_GIT);
			FileRepository r = new FileRepositoryBuilder().SetGitDir(theDir).SetObjectDirectory
				(objDir).AddAlternateObjectDirectory(altObjDir).SetIndexFile(indexFile).Build();
			//
			//
			//
			//
			AssertEqualsPath(theDir, r.Directory);
			AssertEqualsPath(theDir.GetParentFile(), r.WorkTree);
			AssertEqualsPath(indexFile, r.GetIndexFile());
			AssertEqualsPath(objDir, ((ObjectDirectory)r.ObjectDatabase).GetDirectory());
			NUnit.Framework.Assert.IsNotNull(r.Open(ObjectId.FromString("6db9c2ebf75590eef973081736730a9ea169a0c4"
				)));
			// Must close or the default repo pack files created by this test gets
			// locked via the alternate object directories on Windows.
			r.Close();
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal virtual void AssertEqualsPath(FilePath expected, FilePath actual
			)
		{
			NUnit.Framework.Assert.AreEqual(expected.GetCanonicalPath(), actual.GetCanonicalPath
				());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test002_WriteEmptyTree()
		{
			// One of our test packs contains the empty tree object. If the pack is
			// open when we create it we won't write the object file out as a loose
			// object (as it already exists in the pack).
			//
			Repository newdb = CreateBareRepository();
			ObjectInserter oi = newdb.NewObjectInserter();
			ObjectId treeId = oi.Insert(new TreeFormatter());
			oi.Release();
			NUnit.Framework.Assert.AreEqual("4b825dc642cb6eb9a060e54bf8d69288fbee4904", treeId
				.Name);
			FilePath o = new FilePath(new FilePath(new FilePath(newdb.Directory, "objects"), 
				"4b"), "825dc642cb6eb9a060e54bf8d69288fbee4904");
			NUnit.Framework.Assert.IsTrue(o.IsFile(), "Exists " + o);
			NUnit.Framework.Assert.IsTrue(!o.CanWrite(), "Read-only " + o);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test002_WriteEmptyTree2()
		{
			// File shouldn't exist as it is in a test pack.
			//
			ObjectId treeId = InsertTree(new TreeFormatter());
			NUnit.Framework.Assert.AreEqual("4b825dc642cb6eb9a060e54bf8d69288fbee4904", treeId
				.Name);
			FilePath o = new FilePath(new FilePath(new FilePath(db.Directory, "objects"), "4b"
				), "825dc642cb6eb9a060e54bf8d69288fbee4904");
			NUnit.Framework.Assert.IsFalse(o.IsFile(), "Exists " + o);
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test006_ReadUglyConfig()
		{
			FilePath cfg = new FilePath(db.Directory, "config");
			FileBasedConfig c = new FileBasedConfig(cfg, db.FileSystem);
			string configStr = "  [core];comment\n\tfilemode = yes\n" + "[user]\n" + "  email = A U Thor <thor@example.com> # Just an example...\n"
				 + " name = \"A  Thor \\\\ \\\"\\t \"\n" + "    defaultCheckInComment = a many line\\n\\\ncomment\\n\\\n"
				 + " to test\n";
			Write(cfg, configStr);
			c.Load();
			NUnit.Framework.Assert.AreEqual("yes", c.GetString("core", null, "filemode"));
			NUnit.Framework.Assert.AreEqual("A U Thor <thor@example.com>", c.GetString("user"
				, null, "email"));
			NUnit.Framework.Assert.AreEqual("A  Thor \\ \"\t ", c.GetString("user", null, "name"
				));
			NUnit.Framework.Assert.AreEqual("a many line\ncomment\n to test", c.GetString("user"
				, null, "defaultCheckInComment"));
			c.Save();
			FileReader fr = new FileReader(cfg);
			char[] cbuf = new char[configStr.Length];
			fr.Read(cbuf);
			fr.Close();
			NUnit.Framework.Assert.AreEqual(configStr, Sharpen.Extensions.CreateString(cbuf));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test007_Open()
		{
			FileRepository db2 = new FileRepository(db.Directory);
			NUnit.Framework.Assert.AreEqual(db.Directory, db2.Directory);
			NUnit.Framework.Assert.AreEqual(((ObjectDirectory)db.ObjectDatabase).GetDirectory
				(), ((ObjectDirectory)db2.ObjectDatabase).GetDirectory());
			NUnit.Framework.Assert.AreNotSame(((FileBasedConfig)db.GetConfig()), ((FileBasedConfig
				)db2.GetConfig()));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test008_FailOnWrongVersion()
		{
			FilePath cfg = new FilePath(db.Directory, "config");
			string badvers = "ihopethisisneveraversion";
			string configStr = "[core]\n" + "\trepositoryFormatVersion=" + badvers + "\n";
			Write(cfg, configStr);
			try
			{
				new FileRepository(db.Directory);
				NUnit.Framework.Assert.Fail("incorrectly opened a bad repository");
			}
			catch (IOException ioe)
			{
				NUnit.Framework.Assert.IsTrue(ioe.Message.IndexOf("format") > 0);
				NUnit.Framework.Assert.IsTrue(ioe.Message.IndexOf(badvers) > 0);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test009_CreateCommitOldFormat()
		{
			ObjectId treeId = InsertTree(new TreeFormatter());
			NGit.CommitBuilder c = new NGit.CommitBuilder();
			c.Author = new PersonIdent(author, 1154236443000L, -4 * 60);
			c.Committer = new PersonIdent(committer, 1154236443000L, -4 * 60);
			c.Message = "A Commit\n";
			c.TreeId = treeId;
			NUnit.Framework.Assert.AreEqual(treeId, c.TreeId);
			ObjectId actid = InsertCommit(c);
			ObjectId cmtid = ObjectId.FromString("9208b2459ea6609a5af68627cc031796d0d9329b");
			NUnit.Framework.Assert.AreEqual(cmtid, actid);
			// Verify the commit we just wrote is in the correct format.
			ObjectDatabase odb = ((ObjectDirectory)db.ObjectDatabase);
			NUnit.Framework.Assert.IsTrue(odb is ObjectDirectory, "is ObjectDirectory");
			XInputStream xis = new XInputStream(new FileInputStream(((ObjectDirectory)odb).FileFor
				(cmtid)));
			try
			{
				NUnit.Framework.Assert.AreEqual(unchecked((int)(0x78)), xis.ReadUInt8());
				NUnit.Framework.Assert.AreEqual(unchecked((int)(0x9c)), xis.ReadUInt8());
				NUnit.Framework.Assert.IsTrue(unchecked((int)(0x789c)) % 31 == 0);
			}
			finally
			{
				xis.Close();
			}
			// Verify we can read it.
			RevCommit c2 = ParseCommit(actid);
			NUnit.Framework.Assert.IsNotNull(c2);
			NUnit.Framework.Assert.AreEqual(c.Message, c2.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(c.TreeId, c2.Tree);
			NUnit.Framework.Assert.AreEqual(c.Author, c2.GetAuthorIdent());
			NUnit.Framework.Assert.AreEqual(c.Committer, c2.GetCommitterIdent());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test012_SubtreeExternalSorting()
		{
			ObjectId emptyBlob = InsertEmptyBlob();
			Tree t = new Tree(db);
			FileTreeEntry e0 = t.AddFile("a-");
			FileTreeEntry e1 = t.AddFile("a-b");
			FileTreeEntry e2 = t.AddFile("a/b");
			FileTreeEntry e3 = t.AddFile("a=");
			FileTreeEntry e4 = t.AddFile("a=b");
			e0.SetId(emptyBlob);
			e1.SetId(emptyBlob);
			e2.SetId(emptyBlob);
			e3.SetId(emptyBlob);
			e4.SetId(emptyBlob);
			Tree a = (Tree)t.FindTreeMember("a");
			a.SetId(InsertTree(a));
			NUnit.Framework.Assert.AreEqual(ObjectId.FromString("b47a8f0a4190f7572e11212769090523e23eb1ea"
				), InsertTree(t));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test020_createBlobTag()
		{
			ObjectId emptyId = InsertEmptyBlob();
			TagBuilder t = new TagBuilder();
			t.SetObjectId(emptyId, Constants.OBJ_BLOB);
			t.SetTag("test020");
			t.SetTagger(new PersonIdent(author, 1154236443000L, -4 * 60));
			t.SetMessage("test020 tagged\n");
			ObjectId actid = InsertTag(t);
			NUnit.Framework.Assert.AreEqual("6759556b09fbb4fd8ae5e315134481cc25d46954", actid
				.Name);
			RevTag mapTag = ParseTag(actid);
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_BLOB, mapTag.GetObject().Type);
			NUnit.Framework.Assert.AreEqual("test020 tagged\n", mapTag.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(new PersonIdent(author, 1154236443000L, -4 * 60), 
				mapTag.GetTaggerIdent());
			NUnit.Framework.Assert.AreEqual("e69de29bb2d1d6434b8b29ae775ad8c2e48c5391", mapTag
				.GetObject().Id.Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test021_createTreeTag()
		{
			ObjectId emptyId = InsertEmptyBlob();
			Tree almostEmptyTree = new Tree(db);
			almostEmptyTree.AddEntry(new FileTreeEntry(almostEmptyTree, emptyId, Sharpen.Runtime.GetBytesForString
				("empty"), false));
			ObjectId almostEmptyTreeId = InsertTree(almostEmptyTree);
			TagBuilder t = new TagBuilder();
			t.SetObjectId(almostEmptyTreeId, Constants.OBJ_TREE);
			t.SetTag("test021");
			t.SetTagger(new PersonIdent(author, 1154236443000L, -4 * 60));
			t.SetMessage("test021 tagged\n");
			ObjectId actid = InsertTag(t);
			NUnit.Framework.Assert.AreEqual("b0517bc8dbe2096b419d42424cd7030733f4abe5", actid
				.Name);
			RevTag mapTag = ParseTag(actid);
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_TREE, mapTag.GetObject().Type);
			NUnit.Framework.Assert.AreEqual("test021 tagged\n", mapTag.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(new PersonIdent(author, 1154236443000L, -4 * 60), 
				mapTag.GetTaggerIdent());
			NUnit.Framework.Assert.AreEqual("417c01c8795a35b8e835113a85a5c0c1c77f67fb", mapTag
				.GetObject().Id.Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test022_createCommitTag()
		{
			ObjectId emptyId = InsertEmptyBlob();
			Tree almostEmptyTree = new Tree(db);
			almostEmptyTree.AddEntry(new FileTreeEntry(almostEmptyTree, emptyId, Sharpen.Runtime.GetBytesForString
				("empty"), false));
			ObjectId almostEmptyTreeId = InsertTree(almostEmptyTree);
			NGit.CommitBuilder almostEmptyCommit = new NGit.CommitBuilder();
			almostEmptyCommit.Author = new PersonIdent(author, 1154236443000L, -2 * 60);
			// not exactly the same
			almostEmptyCommit.Committer = new PersonIdent(author, 1154236443000L, -2 * 60);
			almostEmptyCommit.Message = "test022\n";
			almostEmptyCommit.TreeId = almostEmptyTreeId;
			ObjectId almostEmptyCommitId = InsertCommit(almostEmptyCommit);
			TagBuilder t = new TagBuilder();
			t.SetObjectId(almostEmptyCommitId, Constants.OBJ_COMMIT);
			t.SetTag("test022");
			t.SetTagger(new PersonIdent(author, 1154236443000L, -4 * 60));
			t.SetMessage("test022 tagged\n");
			ObjectId actid = InsertTag(t);
			NUnit.Framework.Assert.AreEqual("0ce2ebdb36076ef0b38adbe077a07d43b43e3807", actid
				.Name);
			RevTag mapTag = ParseTag(actid);
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_COMMIT, mapTag.GetObject().Type);
			NUnit.Framework.Assert.AreEqual("test022 tagged\n", mapTag.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(new PersonIdent(author, 1154236443000L, -4 * 60), 
				mapTag.GetTaggerIdent());
			NUnit.Framework.Assert.AreEqual("b5d3b45a96b340441f5abb9080411705c51cc86c", mapTag
				.GetObject().Id.Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test023_createCommitNonAnullii()
		{
			ObjectId emptyId = InsertEmptyBlob();
			Tree almostEmptyTree = new Tree(db);
			almostEmptyTree.AddEntry(new FileTreeEntry(almostEmptyTree, emptyId, Sharpen.Runtime.GetBytesForString
				("empty"), false));
			ObjectId almostEmptyTreeId = InsertTree(almostEmptyTree);
			NGit.CommitBuilder commit = new NGit.CommitBuilder();
			commit.TreeId = almostEmptyTreeId;
			commit.Author = new PersonIdent("Joe H\u00e4cker", "joe@example.com", 4294967295000L
				, 60);
			commit.Committer = new PersonIdent("Joe Hacker", "joe2@example.com", 4294967295000L
				, 60);
			commit.SetEncoding("UTF-8");
			commit.Message = "\u00dcbergeeks";
			ObjectId cid = InsertCommit(commit);
			NUnit.Framework.Assert.AreEqual("4680908112778718f37e686cbebcc912730b3154", cid.Name
				);
			RevCommit loadedCommit = ParseCommit(cid);
			NUnit.Framework.Assert.AreEqual(commit.Message, loadedCommit.GetFullMessage());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test024_createCommitNonAscii()
		{
			ObjectId emptyId = InsertEmptyBlob();
			Tree almostEmptyTree = new Tree(db);
			almostEmptyTree.AddEntry(new FileTreeEntry(almostEmptyTree, emptyId, Sharpen.Runtime.GetBytesForString
				("empty"), false));
			ObjectId almostEmptyTreeId = InsertTree(almostEmptyTree);
			NGit.CommitBuilder commit = new NGit.CommitBuilder();
			commit.TreeId = almostEmptyTreeId;
			commit.Author = new PersonIdent("Joe H\u00e4cker", "joe@example.com", 4294967295000L
				, 60);
			commit.Committer = new PersonIdent("Joe Hacker", "joe2@example.com", 4294967295000L
				, 60);
			commit.SetEncoding("ISO-8859-1");
			commit.Message = "\u00dcbergeeks";
			ObjectId cid = InsertCommit(commit);
			NUnit.Framework.Assert.AreEqual("2979b39d385014b33287054b87f77bcb3ecb5ebf", cid.Name
				);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test025_computeSha1NoStore()
		{
			byte[] data = Sharpen.Runtime.GetBytesForString("test025 some data, more than 16 bytes to get good coverage"
				, "ISO-8859-1");
			ObjectId id = new ObjectInserter.Formatter().IdFor(Constants.OBJ_BLOB, data);
			NUnit.Framework.Assert.AreEqual("4f561df5ecf0dfbd53a0dc0f37262fef075d9dde", id.Name
				);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test026_CreateCommitMultipleparents()
		{
			ObjectId treeId;
			ObjectInserter oi = db.NewObjectInserter();
			try
			{
				ObjectId blobId = oi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString
					("and this is the data in me\n", Constants.CHARSET.EncodingName));
				TreeFormatter fmt = new TreeFormatter();
				fmt.Append("i-am-a-file", FileMode.REGULAR_FILE, blobId);
				treeId = oi.Insert(fmt);
				oi.Flush();
			}
			finally
			{
				oi.Release();
			}
			NUnit.Framework.Assert.AreEqual(ObjectId.FromString("00b1f73724f493096d1ffa0b0f1f1482dbb8c936"
				), treeId);
			NGit.CommitBuilder c1 = new NGit.CommitBuilder();
			c1.Author = new PersonIdent(author, 1154236443000L, -4 * 60);
			c1.Committer = new PersonIdent(committer, 1154236443000L, -4 * 60);
			c1.Message = "A Commit\n";
			c1.TreeId = treeId;
			NUnit.Framework.Assert.AreEqual(treeId, c1.TreeId);
			ObjectId actid1 = InsertCommit(c1);
			ObjectId cmtid1 = ObjectId.FromString("803aec4aba175e8ab1d666873c984c0308179099");
			NUnit.Framework.Assert.AreEqual(cmtid1, actid1);
			NGit.CommitBuilder c2 = new NGit.CommitBuilder();
			c2.Author = new PersonIdent(author, 1154236443000L, -4 * 60);
			c2.Committer = new PersonIdent(committer, 1154236443000L, -4 * 60);
			c2.Message = "A Commit 2\n";
			c2.TreeId = treeId;
			NUnit.Framework.Assert.AreEqual(treeId, c2.TreeId);
			c2.SetParentIds(actid1);
			ObjectId actid2 = InsertCommit(c2);
			ObjectId cmtid2 = ObjectId.FromString("95d068687c91c5c044fb8c77c5154d5247901553");
			NUnit.Framework.Assert.AreEqual(cmtid2, actid2);
			RevCommit rm2 = ParseCommit(cmtid2);
			NUnit.Framework.Assert.AreNotSame(c2, rm2);
			// assert the parsed objects is not from the
			// cache
			NUnit.Framework.Assert.AreEqual(c2.Author, rm2.GetAuthorIdent());
			NUnit.Framework.Assert.AreEqual(actid2, rm2.Id);
			NUnit.Framework.Assert.AreEqual(c2.Message, rm2.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(c2.TreeId, rm2.Tree.Id);
			NUnit.Framework.Assert.AreEqual(1, rm2.ParentCount);
			NUnit.Framework.Assert.AreEqual(actid1, rm2.GetParent(0));
			NGit.CommitBuilder c3 = new NGit.CommitBuilder();
			c3.Author = new PersonIdent(author, 1154236443000L, -4 * 60);
			c3.Committer = new PersonIdent(committer, 1154236443000L, -4 * 60);
			c3.Message = "A Commit 3\n";
			c3.TreeId = treeId;
			NUnit.Framework.Assert.AreEqual(treeId, c3.TreeId);
			c3.SetParentIds(actid1, actid2);
			ObjectId actid3 = InsertCommit(c3);
			ObjectId cmtid3 = ObjectId.FromString("ce6e1ce48fbeeb15a83f628dc8dc2debefa066f4");
			NUnit.Framework.Assert.AreEqual(cmtid3, actid3);
			RevCommit rm3 = ParseCommit(cmtid3);
			NUnit.Framework.Assert.AreNotSame(c3, rm3);
			// assert the parsed objects is not from the
			// cache
			NUnit.Framework.Assert.AreEqual(c3.Author, rm3.GetAuthorIdent());
			NUnit.Framework.Assert.AreEqual(actid3, rm3.Id);
			NUnit.Framework.Assert.AreEqual(c3.Message, rm3.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(c3.TreeId, rm3.Tree.Id);
			NUnit.Framework.Assert.AreEqual(2, rm3.ParentCount);
			NUnit.Framework.Assert.AreEqual(actid1, rm3.GetParent(0));
			NUnit.Framework.Assert.AreEqual(actid2, rm3.GetParent(1));
			NGit.CommitBuilder c4 = new NGit.CommitBuilder();
			c4.Author = new PersonIdent(author, 1154236443000L, -4 * 60);
			c4.Committer = new PersonIdent(committer, 1154236443000L, -4 * 60);
			c4.Message = "A Commit 4\n";
			c4.TreeId = treeId;
			NUnit.Framework.Assert.AreEqual(treeId, c3.TreeId);
			c4.SetParentIds(actid1, actid2, actid3);
			ObjectId actid4 = InsertCommit(c4);
			ObjectId cmtid4 = ObjectId.FromString("d1fca9fe3fef54e5212eb67902c8ed3e79736e27");
			NUnit.Framework.Assert.AreEqual(cmtid4, actid4);
			RevCommit rm4 = ParseCommit(cmtid4);
			NUnit.Framework.Assert.AreNotSame(c4, rm3);
			// assert the parsed objects is not from the
			// cache
			NUnit.Framework.Assert.AreEqual(c4.Author, rm4.GetAuthorIdent());
			NUnit.Framework.Assert.AreEqual(actid4, rm4.Id);
			NUnit.Framework.Assert.AreEqual(c4.Message, rm4.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(c4.TreeId, rm4.Tree.Id);
			NUnit.Framework.Assert.AreEqual(3, rm4.ParentCount);
			NUnit.Framework.Assert.AreEqual(actid1, rm4.GetParent(0));
			NUnit.Framework.Assert.AreEqual(actid2, rm4.GetParent(1));
			NUnit.Framework.Assert.AreEqual(actid3, rm4.GetParent(2));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test027_UnpackedRefHigherPriorityThanPacked()
		{
			string unpackedId = "7f822839a2fe9760f386cbbbcb3f92c5fe81def7";
			Write(new FilePath(db.Directory, "refs/heads/a"), unpackedId + "\n");
			ObjectId resolved = db.Resolve("refs/heads/a");
			NUnit.Framework.Assert.AreEqual(unpackedId, resolved.Name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test028_LockPackedRef()
		{
			WriteTrashFile(".git/packed-refs", "7f822839a2fe9760f386cbbbcb3f92c5fe81def7 refs/heads/foobar"
				);
			WriteTrashFile(".git/HEAD", "ref: refs/heads/foobar\n");
			BUG_WorkAroundRacyGitIssues("packed-refs");
			BUG_WorkAroundRacyGitIssues("HEAD");
			ObjectId resolve = db.Resolve("HEAD");
			NUnit.Framework.Assert.AreEqual("7f822839a2fe9760f386cbbbcb3f92c5fe81def7", resolve
				.Name);
			RefUpdate lockRef = db.UpdateRef("HEAD");
			ObjectId newId = ObjectId.FromString("07f822839a2fe9760f386cbbbcb3f92c5fe81def");
			lockRef.SetNewObjectId(newId);
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, lockRef.ForceUpdate());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "refs/heads/foobar").Exists
				());
			NUnit.Framework.Assert.AreEqual(newId, db.Resolve("refs/heads/foobar"));
			// Again. The ref already exists
			RefUpdate lockRef2 = db.UpdateRef("HEAD");
			ObjectId newId2 = ObjectId.FromString("7f822839a2fe9760f386cbbbcb3f92c5fe81def7");
			lockRef2.SetNewObjectId(newId2);
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, lockRef2.ForceUpdate());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "refs/heads/foobar").Exists
				());
			NUnit.Framework.Assert.AreEqual(newId2, db.Resolve("refs/heads/foobar"));
		}

		[NUnit.Framework.Test]
		public virtual void Test30_stripWorkDir()
		{
			FilePath relCwd = new FilePath(".");
			FilePath absCwd = relCwd.GetAbsoluteFile();
			FilePath absBase = new FilePath(new FilePath(absCwd, "repo"), "workdir");
			FilePath relBase = new FilePath(new FilePath(relCwd, "repo"), "workdir");
			NUnit.Framework.Assert.AreEqual(absBase.GetAbsolutePath(), relBase.GetAbsolutePath
				());
			FilePath relBaseFile = new FilePath(new FilePath(relBase, "other"), "module.c");
			FilePath absBaseFile = new FilePath(new FilePath(absBase, "other"), "module.c");
			NUnit.Framework.Assert.AreEqual("other/module.c", Repository.StripWorkDir(relBase
				, relBaseFile));
			NUnit.Framework.Assert.AreEqual("other/module.c", Repository.StripWorkDir(relBase
				, absBaseFile));
			NUnit.Framework.Assert.AreEqual("other/module.c", Repository.StripWorkDir(absBase
				, relBaseFile));
			NUnit.Framework.Assert.AreEqual("other/module.c", Repository.StripWorkDir(absBase
				, absBaseFile));
			FilePath relNonFile = new FilePath(new FilePath(relCwd, "not-repo"), ".gitignore"
				);
			FilePath absNonFile = new FilePath(new FilePath(absCwd, "not-repo"), ".gitignore"
				);
			NUnit.Framework.Assert.AreEqual(string.Empty, Repository.StripWorkDir(relBase, relNonFile
				));
			NUnit.Framework.Assert.AreEqual(string.Empty, Repository.StripWorkDir(absBase, absNonFile
				));
			NUnit.Framework.Assert.AreEqual(string.Empty, Repository.StripWorkDir(db.WorkTree
				, db.WorkTree));
			FilePath file = new FilePath(new FilePath(db.WorkTree, "subdir"), "File.java");
			NUnit.Framework.Assert.AreEqual("subdir/File.java", Repository.StripWorkDir(db.WorkTree
				, file));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private ObjectId InsertEmptyBlob()
		{
			ObjectId emptyId;
			ObjectInserter oi = db.NewObjectInserter();
			try
			{
				emptyId = oi.Insert(Constants.OBJ_BLOB, new byte[] {  });
				oi.Flush();
			}
			finally
			{
				oi.Release();
			}
			return emptyId;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private ObjectId InsertTree(Tree tree)
		{
			ObjectInserter oi = db.NewObjectInserter();
			try
			{
				ObjectId id = oi.Insert(Constants.OBJ_TREE, tree.Format());
				oi.Flush();
				return id;
			}
			finally
			{
				oi.Release();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private ObjectId InsertTree(TreeFormatter tree)
		{
			ObjectInserter oi = db.NewObjectInserter();
			try
			{
				ObjectId id = oi.Insert(tree);
				oi.Flush();
				return id;
			}
			finally
			{
				oi.Release();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		private ObjectId InsertCommit(NGit.CommitBuilder builder)
		{
			ObjectInserter oi = db.NewObjectInserter();
			try
			{
				ObjectId id = oi.Insert(builder);
				oi.Flush();
				return id;
			}
			finally
			{
				oi.Release();
			}
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private RevCommit ParseCommit(AnyObjectId id)
		{
			RevWalk rw = new RevWalk(db);
			try
			{
				return rw.ParseCommit(id);
			}
			finally
			{
				rw.Release();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		private ObjectId InsertTag(TagBuilder tag)
		{
			ObjectInserter oi = db.NewObjectInserter();
			try
			{
				ObjectId id = oi.Insert(tag);
				oi.Flush();
				return id;
			}
			finally
			{
				oi.Release();
			}
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private RevTag ParseTag(AnyObjectId id)
		{
			RevWalk rw = new RevWalk(db);
			try
			{
				return rw.ParseTag(id);
			}
			finally
			{
				rw.Release();
			}
		}

		/// <summary>Kick the timestamp of a local file.</summary>
		/// <remarks>
		/// Kick the timestamp of a local file.
		/// <p>
		/// We shouldn't have to make these method calls. The cache is using file
		/// system timestamps, and on many systems unit tests run faster than the
		/// modification clock. Dumping the cache after we make an edit behind
		/// RefDirectory's back allows the tests to pass.
		/// </remarks>
		/// <param name="name">the file in the repository to force a time change on.</param>
		private void BUG_WorkAroundRacyGitIssues(string name)
		{
			FilePath path = new FilePath(db.Directory, name);
			long old = path.LastModified();
			long set = 1250379778668L;
			// Sat Aug 15 20:12:58 GMT-03:30 2009
			path.SetLastModified(set);
			NUnit.Framework.Assert.IsTrue(old != path.LastModified(), "time changed");
		}
	}
}
