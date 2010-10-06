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
using Sharpen;

namespace NGit.Dircache
{
	public class DirCacheTreeTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestEmptyCache_NoCacheTree()
		{
			DirCache dc = db.ReadDirCache();
			NUnit.Framework.Assert.IsNull(dc.GetCacheTree(false));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestEmptyCache_CreateEmptyCacheTree()
		{
			DirCache dc = db.ReadDirCache();
			DirCacheTree tree = dc.GetCacheTree(true);
			NUnit.Framework.Assert.IsNotNull(tree);
			NUnit.Framework.Assert.AreSame(tree, dc.GetCacheTree(false));
			NUnit.Framework.Assert.AreSame(tree, dc.GetCacheTree(true));
			NUnit.Framework.Assert.AreEqual(string.Empty, tree.GetNameString());
			NUnit.Framework.Assert.AreEqual(string.Empty, tree.GetPathString());
			NUnit.Framework.Assert.AreEqual(0, tree.GetChildCount());
			NUnit.Framework.Assert.AreEqual(0, tree.GetEntrySpan());
			NUnit.Framework.Assert.IsFalse(tree.IsValid());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestEmptyCache_Clear_NoCacheTree()
		{
			DirCache dc = db.ReadDirCache();
			DirCacheTree tree = dc.GetCacheTree(true);
			NUnit.Framework.Assert.IsNotNull(tree);
			dc.Clear();
			NUnit.Framework.Assert.IsNull(dc.GetCacheTree(false));
			NUnit.Framework.Assert.AreNotSame(tree, dc.GetCacheTree(true));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSingleSubtree()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a/b", "a/c", "a/d", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(FileMode.REGULAR_FILE);
			}
			int aFirst = 1;
			int aLast = 3;
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			NUnit.Framework.Assert.IsNull(dc.GetCacheTree(false));
			DirCacheTree root = dc.GetCacheTree(true);
			NUnit.Framework.Assert.IsNotNull(root);
			NUnit.Framework.Assert.AreSame(root, dc.GetCacheTree(true));
			NUnit.Framework.Assert.AreEqual(string.Empty, root.GetNameString());
			NUnit.Framework.Assert.AreEqual(string.Empty, root.GetPathString());
			NUnit.Framework.Assert.AreEqual(1, root.GetChildCount());
			NUnit.Framework.Assert.AreEqual(dc.GetEntryCount(), root.GetEntrySpan());
			NUnit.Framework.Assert.IsFalse(root.IsValid());
			DirCacheTree aTree = root.GetChild(0);
			NUnit.Framework.Assert.IsNotNull(aTree);
			NUnit.Framework.Assert.AreSame(aTree, root.GetChild(0));
			NUnit.Framework.Assert.AreEqual("a", aTree.GetNameString());
			NUnit.Framework.Assert.AreEqual("a/", aTree.GetPathString());
			NUnit.Framework.Assert.AreEqual(0, aTree.GetChildCount());
			NUnit.Framework.Assert.AreEqual(aLast - aFirst + 1, aTree.GetEntrySpan());
			NUnit.Framework.Assert.IsFalse(aTree.IsValid());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoLevelSubtree()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a/b", "a/c/e", "a/c/f", "a/d", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(FileMode.REGULAR_FILE);
			}
			int aFirst = 1;
			int aLast = 4;
			int acFirst = 2;
			int acLast = 3;
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			NUnit.Framework.Assert.IsNull(dc.GetCacheTree(false));
			DirCacheTree root = dc.GetCacheTree(true);
			NUnit.Framework.Assert.IsNotNull(root);
			NUnit.Framework.Assert.AreSame(root, dc.GetCacheTree(true));
			NUnit.Framework.Assert.AreEqual(string.Empty, root.GetNameString());
			NUnit.Framework.Assert.AreEqual(string.Empty, root.GetPathString());
			NUnit.Framework.Assert.AreEqual(1, root.GetChildCount());
			NUnit.Framework.Assert.AreEqual(dc.GetEntryCount(), root.GetEntrySpan());
			NUnit.Framework.Assert.IsFalse(root.IsValid());
			DirCacheTree aTree = root.GetChild(0);
			NUnit.Framework.Assert.IsNotNull(aTree);
			NUnit.Framework.Assert.AreSame(aTree, root.GetChild(0));
			NUnit.Framework.Assert.AreEqual("a", aTree.GetNameString());
			NUnit.Framework.Assert.AreEqual("a/", aTree.GetPathString());
			NUnit.Framework.Assert.AreEqual(1, aTree.GetChildCount());
			NUnit.Framework.Assert.AreEqual(aLast - aFirst + 1, aTree.GetEntrySpan());
			NUnit.Framework.Assert.IsFalse(aTree.IsValid());
			DirCacheTree acTree = aTree.GetChild(0);
			NUnit.Framework.Assert.IsNotNull(acTree);
			NUnit.Framework.Assert.AreSame(acTree, aTree.GetChild(0));
			NUnit.Framework.Assert.AreEqual("c", acTree.GetNameString());
			NUnit.Framework.Assert.AreEqual("a/c/", acTree.GetPathString());
			NUnit.Framework.Assert.AreEqual(0, acTree.GetChildCount());
			NUnit.Framework.Assert.AreEqual(acLast - acFirst + 1, acTree.GetEntrySpan());
			NUnit.Framework.Assert.IsFalse(acTree.IsValid());
		}

		/// <summary>We had bugs related to buffer size in the DirCache.</summary>
		/// <remarks>
		/// We had bugs related to buffer size in the DirCache. This test creates an
		/// index larger than the default BufferedInputStream buffer size. This made
		/// the DirCache unable to read the extensions when index size exceeded the
		/// buffer size (in some cases at least).
		/// </remarks>
		/// <exception cref="NGit.Errors.CorruptObjectException">NGit.Errors.CorruptObjectException
		/// 	</exception>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestWriteReadTree()
		{
			DirCache dc = db.LockDirCache();
			string A = string.Format("a%2000s", "a");
			string B = string.Format("b%2000s", "b");
			string[] paths = new string[] { A + ".", A + "." + B, A + "/" + B, A + "0" + B };
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
			b.Commit();
			DirCache read = db.ReadDirCache();
			NUnit.Framework.Assert.AreEqual(paths.Length, read.GetEntryCount());
			NUnit.Framework.Assert.AreEqual(1, read.GetCacheTree(true).GetChildCount());
		}
	}
}
