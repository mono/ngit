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
using NGit.Dircache;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Dircache
{
	public class DirCacheIteratorTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestEmptyTree_NoTreeWalk()
		{
			DirCache dc = db.ReadDirCache();
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
			DirCacheIterator i = new DirCacheIterator(dc);
			NUnit.Framework.Assert.IsTrue(i.Eof());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestEmptyTree_WithTreeWalk()
		{
			DirCache dc = db.ReadDirCache();
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
			TreeWalk tw = new TreeWalk(db);
			tw.Reset();
			tw.AddTree(new DirCacheIterator(dc));
			NUnit.Framework.Assert.IsFalse(tw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNoSubtree_NoTreeWalk()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(FileMode.REGULAR_FILE);
			}
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			DirCacheIterator i_2 = new DirCacheIterator(dc);
			int pathIdx = 0;
			for (; !i_2.Eof(); i_2.Next(1))
			{
				NUnit.Framework.Assert.AreEqual(pathIdx, i_2.ptr);
				NUnit.Framework.Assert.AreSame(ents[pathIdx], i_2.GetDirCacheEntry());
				pathIdx++;
			}
			NUnit.Framework.Assert.AreEqual(paths.Length, pathIdx);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNoSubtree_WithTreeWalk()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a0b" };
			FileMode[] modes = new FileMode[] { FileMode.EXECUTABLE_FILE, FileMode.GITLINK };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(modes[i]);
			}
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			DirCacheIterator i_2 = new DirCacheIterator(dc);
			TreeWalk tw = new TreeWalk(db);
			tw.Reset();
			tw.AddTree(i_2);
			int pathIdx = 0;
			while (tw.Next())
			{
				NUnit.Framework.Assert.AreSame(i_2, tw.GetTree<DirCacheIterator>(0));
				NUnit.Framework.Assert.AreEqual(pathIdx, i_2.ptr);
				NUnit.Framework.Assert.AreSame(ents[pathIdx], i_2.GetDirCacheEntry());
				NUnit.Framework.Assert.AreEqual(paths[pathIdx], tw.PathString);
				NUnit.Framework.Assert.AreEqual(modes[pathIdx].GetBits(), tw.GetRawMode(0));
				NUnit.Framework.Assert.AreSame(modes[pathIdx], tw.GetFileMode(0));
				pathIdx++;
			}
			NUnit.Framework.Assert.AreEqual(paths.Length, pathIdx);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSingleSubtree_NoRecursion()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a/b", "a/c", "a/d", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(FileMode.REGULAR_FILE);
			}
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			string[] expPaths = new string[] { "a.", "a", "a0b" };
			FileMode[] expModes = new FileMode[] { FileMode.REGULAR_FILE, FileMode.TREE, FileMode
				.REGULAR_FILE };
			int[] expPos = new int[] { 0, -1, 4 };
			DirCacheIterator i_2 = new DirCacheIterator(dc);
			TreeWalk tw = new TreeWalk(db);
			tw.Reset();
			tw.AddTree(i_2);
			tw.Recursive = false;
			int pathIdx = 0;
			while (tw.Next())
			{
				NUnit.Framework.Assert.AreSame(i_2, tw.GetTree<DirCacheIterator>(0));
				NUnit.Framework.Assert.AreEqual(expModes[pathIdx].GetBits(), tw.GetRawMode(0));
				NUnit.Framework.Assert.AreSame(expModes[pathIdx], tw.GetFileMode(0));
				NUnit.Framework.Assert.AreEqual(expPaths[pathIdx], tw.PathString);
				if (expPos[pathIdx] >= 0)
				{
					NUnit.Framework.Assert.AreEqual(expPos[pathIdx], i_2.ptr);
					NUnit.Framework.Assert.AreSame(ents[expPos[pathIdx]], i_2.GetDirCacheEntry());
				}
				else
				{
					NUnit.Framework.Assert.AreSame(FileMode.TREE, tw.GetFileMode(0));
				}
				pathIdx++;
			}
			NUnit.Framework.Assert.AreEqual(expPaths.Length, pathIdx);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSingleSubtree_Recursive()
		{
			DirCache dc = db.ReadDirCache();
			FileMode mode = FileMode.REGULAR_FILE;
			string[] paths = new string[] { "a.", "a/b", "a/c", "a/d", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(mode);
			}
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			DirCacheIterator i_2 = new DirCacheIterator(dc);
			TreeWalk tw = new TreeWalk(db);
			tw.Reset();
			tw.AddTree(i_2);
			tw.Recursive = true;
			int pathIdx = 0;
			while (tw.Next())
			{
				DirCacheIterator c = tw.GetTree<DirCacheIterator>(0);
				NUnit.Framework.Assert.IsNotNull(c);
				NUnit.Framework.Assert.AreEqual(pathIdx, c.ptr);
				NUnit.Framework.Assert.AreSame(ents[pathIdx], c.GetDirCacheEntry());
				NUnit.Framework.Assert.AreEqual(paths[pathIdx], tw.PathString);
				NUnit.Framework.Assert.AreEqual(mode.GetBits(), tw.GetRawMode(0));
				NUnit.Framework.Assert.AreSame(mode, tw.GetFileMode(0));
				pathIdx++;
			}
			NUnit.Framework.Assert.AreEqual(paths.Length, pathIdx);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoLevelSubtree_Recursive()
		{
			DirCache dc = db.ReadDirCache();
			FileMode mode = FileMode.REGULAR_FILE;
			string[] paths = new string[] { "a.", "a/b", "a/c/e", "a/c/f", "a/d", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(mode);
			}
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			TreeWalk tw = new TreeWalk(db);
			tw.Reset();
			tw.AddTree(new DirCacheIterator(dc));
			tw.Recursive = true;
			int pathIdx = 0;
			while (tw.Next())
			{
				DirCacheIterator c = tw.GetTree<DirCacheIterator>(0);
				NUnit.Framework.Assert.IsNotNull(c);
				NUnit.Framework.Assert.AreEqual(pathIdx, c.ptr);
				NUnit.Framework.Assert.AreSame(ents[pathIdx], c.GetDirCacheEntry());
				NUnit.Framework.Assert.AreEqual(paths[pathIdx], tw.PathString);
				NUnit.Framework.Assert.AreEqual(mode.GetBits(), tw.GetRawMode(0));
				NUnit.Framework.Assert.AreSame(mode, tw.GetFileMode(0));
				pathIdx++;
			}
			NUnit.Framework.Assert.AreEqual(paths.Length, pathIdx);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoLevelSubtree_FilterPath()
		{
			DirCache dc = db.ReadDirCache();
			FileMode mode = FileMode.REGULAR_FILE;
			string[] paths = new string[] { "a.", "a/b", "a/c/e", "a/c/f", "a/d", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(mode);
			}
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			TreeWalk tw = new TreeWalk(db);
			for (int victimIdx = 0; victimIdx < paths.Length; victimIdx++)
			{
				tw.Reset();
				tw.AddTree(new DirCacheIterator(dc));
				tw.Filter = PathFilterGroup.CreateFromStrings(Collections.Singleton(paths[victimIdx
					]));
				tw.Recursive = tw.Filter.ShouldBeRecursive();
				NUnit.Framework.Assert.IsTrue(tw.Next());
				DirCacheIterator c = tw.GetTree<DirCacheIterator>(0);
				NUnit.Framework.Assert.IsNotNull(c);
				NUnit.Framework.Assert.AreEqual(victimIdx, c.ptr);
				NUnit.Framework.Assert.AreSame(ents[victimIdx], c.GetDirCacheEntry());
				NUnit.Framework.Assert.AreEqual(paths[victimIdx], tw.PathString);
				NUnit.Framework.Assert.AreEqual(mode.GetBits(), tw.GetRawMode(0));
				NUnit.Framework.Assert.AreSame(mode, tw.GetFileMode(0));
				NUnit.Framework.Assert.IsFalse(tw.Next());
			}
		}
	}
}
