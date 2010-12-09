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
using NGit;
using NGit.Dircache;
using NGit.Errors;
using NGit.Junit;
using NGit.Treewalk;
using NGit.Util;
using Sharpen;

namespace NGit.Dircache
{
	public class DirCacheCGitCompatabilityTest : LocalDiskRepositoryTestCase
	{
		private readonly FilePath index;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadIndex_LsFiles()
		{
			IDictionary<string, DirCacheCGitCompatabilityTest.CGitIndexRecord> ls = ReadLsFiles
				();
			DirCache dc = new DirCache(index, FS.DETECTED);
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
			dc.Read();
			NUnit.Framework.Assert.AreEqual(ls.Count, dc.GetEntryCount());
			{
				Iterator<DirCacheCGitCompatabilityTest.CGitIndexRecord> rItr = ls.Values.Iterator
					();
				for (int i = 0; rItr.HasNext(); i++)
				{
					AssertEqual(rItr.Next(), dc.GetEntry(i));
				}
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTreeWalk_LsFiles()
		{
			Repository db = CreateBareRepository();
			IDictionary<string, DirCacheCGitCompatabilityTest.CGitIndexRecord> ls = ReadLsFiles
				();
			DirCache dc = new DirCache(index, db.FileSystem);
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
			dc.Read();
			NUnit.Framework.Assert.AreEqual(ls.Count, dc.GetEntryCount());
			{
				Iterator<DirCacheCGitCompatabilityTest.CGitIndexRecord> rItr = ls.Values.Iterator
					();
				TreeWalk tw = new TreeWalk(db);
				tw.Recursive = true;
				tw.AddTree(new DirCacheIterator(dc));
				while (rItr.HasNext())
				{
					DirCacheIterator dcItr;
					NUnit.Framework.Assert.IsTrue(tw.Next());
					dcItr = tw.GetTree<DirCacheIterator>(0);
					NUnit.Framework.Assert.IsNotNull(dcItr);
					AssertEqual(rItr.Next(), dcItr.GetDirCacheEntry());
				}
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUnsupportedOptionalExtension()
		{
			DirCache dc = new DirCache(PathOf("gitgit.index.ZZZZ"), FS.DETECTED);
			dc.Read();
			NUnit.Framework.Assert.AreEqual(1, dc.GetEntryCount());
			NUnit.Framework.Assert.AreEqual("A", dc.GetEntry(0).PathString);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUnsupportedRequiredExtension()
		{
			DirCache dc = new DirCache(PathOf("gitgit.index.aaaa"), FS.DETECTED);
			try
			{
				dc.Read();
				NUnit.Framework.Assert.Fail("Cache loaded an unsupported extension");
			}
			catch (CorruptObjectException err)
			{
				NUnit.Framework.Assert.AreEqual("DIRC extension 'aaaa'" + " not supported by this version."
					, err.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCorruptChecksumAtFooter()
		{
			DirCache dc = new DirCache(PathOf("gitgit.index.badchecksum"), FS.DETECTED);
			try
			{
				dc.Read();
				NUnit.Framework.Assert.Fail("Cache loaded despite corrupt checksum");
			}
			catch (CorruptObjectException err)
			{
				NUnit.Framework.Assert.AreEqual("DIRC checksum mismatch", err.Message);
			}
		}

		private static void AssertEqual(DirCacheCGitCompatabilityTest.CGitIndexRecord c, 
			DirCacheEntry j)
		{
			NUnit.Framework.Assert.IsNotNull(c);
			NUnit.Framework.Assert.IsNotNull(j);
			NUnit.Framework.Assert.AreEqual(c.path, j.PathString);
			AssertEquals(c.id, j.GetObjectId());
			NUnit.Framework.Assert.AreEqual(c.mode, j.RawMode);
			NUnit.Framework.Assert.AreEqual(c.stage, j.Stage);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadIndex_DirCacheTree()
		{
			IDictionary<string, DirCacheCGitCompatabilityTest.CGitIndexRecord> cList = ReadLsFiles
				();
			IDictionary<string, DirCacheCGitCompatabilityTest.CGitLsTreeRecord> cTree = ReadLsTree
				();
			DirCache dc = new DirCache(index, FS.DETECTED);
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
			dc.Read();
			NUnit.Framework.Assert.AreEqual(cList.Count, dc.GetEntryCount());
			DirCacheTree jTree = dc.GetCacheTree(false);
			NUnit.Framework.Assert.IsNotNull(jTree);
			NUnit.Framework.Assert.AreEqual(string.Empty, jTree.GetNameString());
			NUnit.Framework.Assert.AreEqual(string.Empty, jTree.GetPathString());
			NUnit.Framework.Assert.IsTrue(jTree.IsValid());
			AssertEquals(ObjectId.FromString("698dd0b8d0c299f080559a1cffc7fe029479a408"), jTree
				.GetObjectId());
			NUnit.Framework.Assert.AreEqual(cList.Count, jTree.GetEntrySpan());
			AList<DirCacheCGitCompatabilityTest.CGitLsTreeRecord> subtrees = new AList<DirCacheCGitCompatabilityTest.CGitLsTreeRecord
				>();
			foreach (DirCacheCGitCompatabilityTest.CGitLsTreeRecord r in cTree.Values)
			{
				if (FileMode.TREE.Equals(r.mode))
				{
					subtrees.AddItem(r);
				}
			}
			NUnit.Framework.Assert.AreEqual(subtrees.Count, jTree.GetChildCount());
			for (int i = 0; i < jTree.GetChildCount(); i++)
			{
				DirCacheTree sj = jTree.GetChild(i);
				DirCacheCGitCompatabilityTest.CGitLsTreeRecord sc = subtrees[i];
				NUnit.Framework.Assert.AreEqual(sc.path, sj.GetNameString());
				NUnit.Framework.Assert.AreEqual(sc.path + "/", sj.GetPathString());
				NUnit.Framework.Assert.IsTrue(sj.IsValid());
				AssertEquals(sc.id, sj.GetObjectId());
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadWriteV3()
		{
			FilePath file = PathOf("gitgit.index.v3");
			DirCache dc = new DirCache(file, FS.DETECTED);
			dc.Read();
			NUnit.Framework.Assert.AreEqual(10, dc.GetEntryCount());
			AssertV3TreeEntry(0, "dir1/file1.txt", false, false, dc);
			AssertV3TreeEntry(1, "dir2/file2.txt", true, false, dc);
			AssertV3TreeEntry(2, "dir3/file3.txt", false, false, dc);
			AssertV3TreeEntry(3, "dir3/file3a.txt", true, false, dc);
			AssertV3TreeEntry(4, "dir4/file4.txt", true, false, dc);
			AssertV3TreeEntry(5, "dir4/file4a.txt", false, false, dc);
			AssertV3TreeEntry(6, "file.txt", true, false, dc);
			AssertV3TreeEntry(7, "newdir1/newfile1.txt", false, true, dc);
			AssertV3TreeEntry(8, "newdir1/newfile2.txt", false, true, dc);
			AssertV3TreeEntry(9, "newfile.txt", false, true, dc);
			ByteArrayOutputStream bos = new ByteArrayOutputStream();
			dc.WriteTo(bos);
			byte[] indexBytes = bos.ToByteArray();
			byte[] expectedBytes = IOUtil.ReadFully(file);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(expectedBytes, indexBytes));
		}

		private static void AssertV3TreeEntry(int indexPosition, string path, bool skipWorkTree
			, bool intentToAdd, DirCache dc)
		{
			DirCacheEntry entry = dc.GetEntry(indexPosition);
			NUnit.Framework.Assert.AreEqual(path, entry.PathString);
			NUnit.Framework.Assert.AreEqual(skipWorkTree, entry.IsSkipWorkTree);
			NUnit.Framework.Assert.AreEqual(intentToAdd, entry.IsIntentToAdd);
		}

		private FilePath PathOf(string name)
		{
			return JGitTestUtil.GetTestResourceFile(name);
		}

		/// <exception cref="System.Exception"></exception>
		private IDictionary<string, DirCacheCGitCompatabilityTest.CGitIndexRecord> ReadLsFiles
			()
		{
			LinkedHashMap<string, DirCacheCGitCompatabilityTest.CGitIndexRecord> r = new LinkedHashMap
				<string, DirCacheCGitCompatabilityTest.CGitIndexRecord>();
			BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(
				PathOf("gitgit.lsfiles")), "UTF-8"));
			try
			{
				string line;
				while ((line = br.ReadLine()) != null)
				{
					DirCacheCGitCompatabilityTest.CGitIndexRecord cr = new DirCacheCGitCompatabilityTest.CGitIndexRecord
						(line);
					r.Put(cr.path, cr);
				}
			}
			finally
			{
				br.Close();
			}
			return r;
		}

		/// <exception cref="System.Exception"></exception>
		private IDictionary<string, DirCacheCGitCompatabilityTest.CGitLsTreeRecord> ReadLsTree
			()
		{
			LinkedHashMap<string, DirCacheCGitCompatabilityTest.CGitLsTreeRecord> r = new LinkedHashMap
				<string, DirCacheCGitCompatabilityTest.CGitLsTreeRecord>();
			BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(
				PathOf("gitgit.lstree")), "UTF-8"));
			try
			{
				string line;
				while ((line = br.ReadLine()) != null)
				{
					DirCacheCGitCompatabilityTest.CGitLsTreeRecord cr = new DirCacheCGitCompatabilityTest.CGitLsTreeRecord
						(line);
					r.Put(cr.path, cr);
				}
			}
			finally
			{
				br.Close();
			}
			return r;
		}

		private class CGitIndexRecord
		{
			internal readonly int mode;

			internal readonly ObjectId id;

			internal readonly int stage;

			internal readonly string path;

			internal CGitIndexRecord(string line)
			{
				int tab = line.IndexOf('\t');
				int sp1 = line.IndexOf(' ');
				int sp2 = line.IndexOf(' ', sp1 + 1);
				mode = System.Convert.ToInt32(Sharpen.Runtime.Substring(line, 0, sp1), 8);
				id = ObjectId.FromString(Sharpen.Runtime.Substring(line, sp1 + 1, sp2));
				stage = System.Convert.ToInt32(Sharpen.Runtime.Substring(line, sp2 + 1, tab));
				path = Sharpen.Runtime.Substring(line, tab + 1);
			}
		}

		private class CGitLsTreeRecord
		{
			internal readonly int mode;

			internal readonly ObjectId id;

			internal readonly string path;

			internal CGitLsTreeRecord(string line)
			{
				int tab = line.IndexOf('\t');
				int sp1 = line.IndexOf(' ');
				int sp2 = line.IndexOf(' ', sp1 + 1);
				mode = System.Convert.ToInt32(Sharpen.Runtime.Substring(line, 0, sp1), 8);
				id = ObjectId.FromString(Sharpen.Runtime.Substring(line, sp2 + 1, tab));
				path = Sharpen.Runtime.Substring(line, tab + 1);
			}
		}

		public DirCacheCGitCompatabilityTest()
		{
			index = PathOf("gitgit.index");
		}
	}
}
