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
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class T0002_TreeTest : SampleDataRepositoryTestCase
	{
		private static readonly ObjectId SOME_FAKE_ID = ObjectId.FromString("0123456789abcdef0123456789abcdef01234567"
			);

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		private int CompareNamesUsingSpecialCompare(string a, string b)
		{
			char lasta = '\0';
			byte[] abytes;
			if (a.Length > 0 && a[a.Length - 1] == '/')
			{
				lasta = '/';
				a = Sharpen.Runtime.Substring(a, 0, a.Length - 1);
			}
			abytes = Sharpen.Runtime.GetBytesForString(a, "ISO-8859-1");
			char lastb = '\0';
			byte[] bbytes;
			if (b.Length > 0 && b[b.Length - 1] == '/')
			{
				lastb = '/';
				b = Sharpen.Runtime.Substring(b, 0, b.Length - 1);
			}
			bbytes = Sharpen.Runtime.GetBytesForString(b, "ISO-8859-1");
			return Tree.CompareNames(abytes, bbytes, lasta, lastb);
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test000_sort_01()
		{
			NUnit.Framework.Assert.AreEqual(0, CompareNamesUsingSpecialCompare("a", "a"));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test000_sort_02()
		{
			NUnit.Framework.Assert.AreEqual(-1, CompareNamesUsingSpecialCompare("a", "b"));
			NUnit.Framework.Assert.AreEqual(1, CompareNamesUsingSpecialCompare("b", "a"));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test000_sort_03()
		{
			NUnit.Framework.Assert.AreEqual(1, CompareNamesUsingSpecialCompare("a:", "a"));
			NUnit.Framework.Assert.AreEqual(1, CompareNamesUsingSpecialCompare("a/", "a"));
			NUnit.Framework.Assert.AreEqual(-1, CompareNamesUsingSpecialCompare("a", "a/"));
			NUnit.Framework.Assert.AreEqual(-1, CompareNamesUsingSpecialCompare("a", "a:"));
			NUnit.Framework.Assert.AreEqual(1, CompareNamesUsingSpecialCompare("a:", "a/"));
			NUnit.Framework.Assert.AreEqual(-1, CompareNamesUsingSpecialCompare("a/", "a:"));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test000_sort_04()
		{
			NUnit.Framework.Assert.AreEqual(-1, CompareNamesUsingSpecialCompare("a.a", "a/a")
				);
			NUnit.Framework.Assert.AreEqual(1, CompareNamesUsingSpecialCompare("a/a", "a.a"));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test000_sort_05()
		{
			NUnit.Framework.Assert.AreEqual(-1, CompareNamesUsingSpecialCompare("a.", "a/"));
			NUnit.Framework.Assert.AreEqual(1, CompareNamesUsingSpecialCompare("a/", "a."));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test001_createEmpty()
		{
			Tree t = new Tree(db);
			NUnit.Framework.Assert.IsTrue(t.IsLoaded(), "isLoaded");
			NUnit.Framework.Assert.IsTrue(t.IsModified(), "isModified");
			NUnit.Framework.Assert.IsTrue(t.GetParent() == null, "no parent");
			NUnit.Framework.Assert.IsTrue(t.IsRoot(), "isRoot");
			NUnit.Framework.Assert.IsTrue(t.GetName() == null, "no name");
			NUnit.Framework.Assert.IsTrue(t.GetNameUTF8() == null, "no nameUTF8");
			NUnit.Framework.Assert.IsTrue(t.Members() != null, "has entries array");
			NUnit.Framework.Assert.IsTrue(t.Members().Length == 0, "entries is empty");
			NUnit.Framework.Assert.AreEqual(string.Empty, t.GetFullName(), "full name is empty"
				);
			NUnit.Framework.Assert.IsTrue(t.GetId() == null, "no id");
			NUnit.Framework.Assert.IsTrue(t.GetTree() == t, "tree is self");
			NUnit.Framework.Assert.IsTrue(t.GetRepository() == db, "database is r");
			NUnit.Framework.Assert.IsTrue(t.FindTreeMember("foo") == null, "no foo child");
			NUnit.Framework.Assert.IsTrue(t.FindBlobMember("foo") == null, "no foo child");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test002_addFile()
		{
			Tree t = new Tree(db);
			t.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue(t.GetId() != null, "has id");
			NUnit.Framework.Assert.IsFalse(t.IsModified(), "not modified");
			string n = "bob";
			FileTreeEntry f = t.AddFile(n);
			NUnit.Framework.Assert.IsNotNull(f, "have file");
			NUnit.Framework.Assert.AreEqual(n, f.GetName(), "name matches");
			NUnit.Framework.Assert.AreEqual(f.GetName(), Sharpen.Runtime.GetStringForBytes(f.
				GetNameUTF8(), "UTF-8"), "name matches");
			NUnit.Framework.Assert.AreEqual(n, f.GetFullName(), "full name matches");
			NUnit.Framework.Assert.IsTrue(f.GetId() == null, "no id");
			NUnit.Framework.Assert.IsTrue(t.IsModified(), "is modified");
			NUnit.Framework.Assert.IsTrue(t.GetId() == null, "has no id");
			NUnit.Framework.Assert.IsTrue(t.FindBlobMember(f.GetName()) == f, "found bob");
			TreeEntry[] i = t.Members();
			NUnit.Framework.Assert.IsNotNull(i, "members array not null");
			NUnit.Framework.Assert.IsTrue(i != null && i.Length > 0, "iterator is not empty");
			NUnit.Framework.Assert.IsTrue(i != null && i[0] == f, "iterator returns file");
			NUnit.Framework.Assert.IsTrue(i != null && i.Length == 1, "iterator is empty");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test004_addTree()
		{
			Tree t = new Tree(db);
			t.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue(t.GetId() != null, "has id");
			NUnit.Framework.Assert.IsFalse(t.IsModified(), "not modified");
			string n = "bob";
			Tree f = t.AddTree(n);
			NUnit.Framework.Assert.IsNotNull(f, "have tree");
			NUnit.Framework.Assert.AreEqual(n, f.GetName(), "name matches");
			NUnit.Framework.Assert.AreEqual(f.GetName(), Sharpen.Runtime.GetStringForBytes(f.
				GetNameUTF8(), "UTF-8"), "name matches");
			NUnit.Framework.Assert.AreEqual(n, f.GetFullName(), "full name matches");
			NUnit.Framework.Assert.IsTrue(f.GetId() == null, "no id");
			NUnit.Framework.Assert.IsTrue(f.GetParent() == t, "parent matches");
			NUnit.Framework.Assert.IsTrue(f.GetRepository() == db, "repository matches");
			NUnit.Framework.Assert.IsTrue(f.IsLoaded(), "isLoaded");
			NUnit.Framework.Assert.IsFalse(f.Members().Length > 0, "has items");
			NUnit.Framework.Assert.IsFalse(f.IsRoot(), "is root");
			NUnit.Framework.Assert.IsTrue(f.GetTree() == f, "tree is self");
			NUnit.Framework.Assert.IsTrue(t.IsModified(), "parent is modified");
			NUnit.Framework.Assert.IsTrue(t.GetId() == null, "parent has no id");
			NUnit.Framework.Assert.IsTrue(t.FindTreeMember(f.GetName()) == f, "found bob child"
				);
			TreeEntry[] i = t.Members();
			NUnit.Framework.Assert.IsTrue(i.Length > 0, "iterator is not empty");
			NUnit.Framework.Assert.IsTrue(i[0] == f, "iterator returns file");
			NUnit.Framework.Assert.IsTrue(i.Length == 1, "iterator is empty");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test005_addRecursiveFile()
		{
			Tree t = new Tree(db);
			FileTreeEntry f = t.AddFile("a/b/c");
			NUnit.Framework.Assert.IsNotNull(f, "created f");
			NUnit.Framework.Assert.AreEqual("c", f.GetName());
			NUnit.Framework.Assert.AreEqual("b", f.GetParent().GetName());
			NUnit.Framework.Assert.AreEqual("a", f.GetParent().GetParent().GetName());
			NUnit.Framework.Assert.IsTrue(t == f.GetParent().GetParent().GetParent(), "t is great-grandparent"
				);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test005_addRecursiveTree()
		{
			Tree t = new Tree(db);
			Tree f = t.AddTree("a/b/c");
			NUnit.Framework.Assert.IsNotNull(f, "created f");
			NUnit.Framework.Assert.AreEqual("c", f.GetName());
			NUnit.Framework.Assert.AreEqual("b", f.GetParent().GetName());
			NUnit.Framework.Assert.AreEqual("a", f.GetParent().GetParent().GetName());
			NUnit.Framework.Assert.IsTrue(t == f.GetParent().GetParent().GetParent(), "t is great-grandparent"
				);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test006_addDeepTree()
		{
			Tree t = new Tree(db);
			Tree e = t.AddTree("e");
			NUnit.Framework.Assert.IsNotNull(e, "have e");
			NUnit.Framework.Assert.IsTrue(e.GetParent() == t, "e.parent == t");
			Tree f = t.AddTree("f");
			NUnit.Framework.Assert.IsNotNull(f, "have f");
			NUnit.Framework.Assert.IsTrue(f.GetParent() == t, "f.parent == t");
			Tree g = f.AddTree("g");
			NUnit.Framework.Assert.IsNotNull(g, "have g");
			NUnit.Framework.Assert.IsTrue(g.GetParent() == f, "g.parent == f");
			Tree h = g.AddTree("h");
			NUnit.Framework.Assert.IsNotNull(h, "have h");
			NUnit.Framework.Assert.IsTrue(h.GetParent() == g, "h.parent = g");
			h.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue(!h.IsModified(), "h not modified");
			g.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue(!g.IsModified(), "g not modified");
			f.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue(!f.IsModified(), "f not modified");
			e.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue(!e.IsModified(), "e not modified");
			t.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue(!t.IsModified(), "t not modified.");
			NUnit.Framework.Assert.AreEqual("f/g/h", h.GetFullName(), "full path of h ok");
			NUnit.Framework.Assert.IsTrue(t.FindTreeMember(h.GetFullName()) == h, "Can find h"
				);
			NUnit.Framework.Assert.IsTrue(t.FindBlobMember("f/z") == null, "Can't find f/z");
			NUnit.Framework.Assert.IsTrue(t.FindBlobMember("y/z") == null, "Can't find y/z");
			FileTreeEntry i = h.AddFile("i");
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual("f/g/h/i", i.GetFullName(), "full path of i ok");
			NUnit.Framework.Assert.IsTrue(t.FindBlobMember(i.GetFullName()) == i, "Can find i"
				);
			NUnit.Framework.Assert.IsTrue(h.IsModified(), "h modified");
			NUnit.Framework.Assert.IsTrue(g.IsModified(), "g modified");
			NUnit.Framework.Assert.IsTrue(f.IsModified(), "f modified");
			NUnit.Framework.Assert.IsTrue(!e.IsModified(), "e not modified");
			NUnit.Framework.Assert.IsTrue(t.IsModified(), "t modified");
			NUnit.Framework.Assert.IsTrue(h.GetId() == null, "h no id");
			NUnit.Framework.Assert.IsTrue(g.GetId() == null, "g no id");
			NUnit.Framework.Assert.IsTrue(f.GetId() == null, "f no id");
			NUnit.Framework.Assert.IsTrue(e.GetId() != null, "e has id");
			NUnit.Framework.Assert.IsTrue(t.GetId() == null, "t no id");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test007_manyFileLookup()
		{
			Tree t = new Tree(db);
			IList<FileTreeEntry> files = new AList<FileTreeEntry>(26 * 26);
			for (char level1 = 'a'; level1 <= 'z'; level1++)
			{
				for (char level2 = 'a'; level2 <= 'z'; level2++)
				{
					string n = "." + level1 + level2 + "9";
					FileTreeEntry f = t.AddFile(n);
					NUnit.Framework.Assert.IsNotNull(f, "File " + n + " added.");
					NUnit.Framework.Assert.AreEqual(n, f.GetName());
					files.AddItem(f);
				}
			}
			NUnit.Framework.Assert.AreEqual(files.Count, t.MemberCount());
			TreeEntry[] ents = t.Members();
			NUnit.Framework.Assert.IsNotNull(ents);
			NUnit.Framework.Assert.AreEqual(files.Count, ents.Length);
			for (int k = 0; k < ents.Length; k++)
			{
				NUnit.Framework.Assert.IsTrue(files[k] == ents[k], "File " + files[k].GetName() +
					 " is at " + k + ".");
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test008_SubtreeInternalSorting()
		{
			Tree t = new Tree(db);
			FileTreeEntry e0 = t.AddFile("a-b");
			FileTreeEntry e1 = t.AddFile("a-");
			FileTreeEntry e2 = t.AddFile("a=b");
			Tree e3 = t.AddTree("a");
			FileTreeEntry e4 = t.AddFile("a=");
			TreeEntry[] ents = t.Members();
			NUnit.Framework.Assert.AreSame(e1, ents[0]);
			NUnit.Framework.Assert.AreSame(e0, ents[1]);
			NUnit.Framework.Assert.AreSame(e3, ents[2]);
			NUnit.Framework.Assert.AreSame(e4, ents[3]);
			NUnit.Framework.Assert.AreSame(e2, ents[4]);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test009_SymlinkAndGitlink()
		{
			Tree symlinkTree = db.MapTree("symlink");
			NUnit.Framework.Assert.IsTrue(symlinkTree.ExistsBlob("symlink.txt"), "Symlink entry exists"
				);
			Tree gitlinkTree = db.MapTree("gitlink");
			NUnit.Framework.Assert.IsTrue(gitlinkTree.ExistsBlob("submodule"), "Gitlink entry exists"
				);
		}
	}
}
