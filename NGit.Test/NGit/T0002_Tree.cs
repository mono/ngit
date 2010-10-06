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
	public class T0002_Tree : SampleDataRepositoryTestCase
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
		public virtual void Test000_sort_01()
		{
			NUnit.Framework.Assert.AreEqual(0, CompareNamesUsingSpecialCompare("a", "a"));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		public virtual void Test000_sort_02()
		{
			NUnit.Framework.Assert.AreEqual(-1, CompareNamesUsingSpecialCompare("a", "b"));
			NUnit.Framework.Assert.AreEqual(1, CompareNamesUsingSpecialCompare("b", "a"));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
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
		public virtual void Test000_sort_04()
		{
			NUnit.Framework.Assert.AreEqual(-1, CompareNamesUsingSpecialCompare("a.a", "a/a")
				);
			NUnit.Framework.Assert.AreEqual(1, CompareNamesUsingSpecialCompare("a/a", "a.a"));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		public virtual void Test000_sort_05()
		{
			NUnit.Framework.Assert.AreEqual(-1, CompareNamesUsingSpecialCompare("a.", "a/"));
			NUnit.Framework.Assert.AreEqual(1, CompareNamesUsingSpecialCompare("a/", "a."));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Test001_createEmpty()
		{
			Tree t = new Tree(db);
			NUnit.Framework.Assert.IsTrue("isLoaded", t.IsLoaded());
			NUnit.Framework.Assert.IsTrue("isModified", t.IsModified());
			NUnit.Framework.Assert.IsTrue("no parent", t.GetParent() == null);
			NUnit.Framework.Assert.IsTrue("isRoot", t.IsRoot());
			NUnit.Framework.Assert.IsTrue("no name", t.GetName() == null);
			NUnit.Framework.Assert.IsTrue("no nameUTF8", t.GetNameUTF8() == null);
			NUnit.Framework.Assert.IsTrue("has entries array", t.Members() != null);
			NUnit.Framework.Assert.IsTrue("entries is empty", t.Members().Length == 0);
			NUnit.Framework.Assert.AreEqual("full name is empty", string.Empty, t.GetFullName
				());
			NUnit.Framework.Assert.IsTrue("no id", t.GetId() == null);
			NUnit.Framework.Assert.IsTrue("tree is self", t.GetTree() == t);
			NUnit.Framework.Assert.IsTrue("database is r", t.GetRepository() == db);
			NUnit.Framework.Assert.IsTrue("no foo child", t.FindTreeMember("foo") == null);
			NUnit.Framework.Assert.IsTrue("no foo child", t.FindBlobMember("foo") == null);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Test002_addFile()
		{
			Tree t = new Tree(db);
			t.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue("has id", t.GetId() != null);
			NUnit.Framework.Assert.IsFalse("not modified", t.IsModified());
			string n = "bob";
			FileTreeEntry f = t.AddFile(n);
			NUnit.Framework.Assert.IsNotNull("have file", f);
			NUnit.Framework.Assert.AreEqual("name matches", n, f.GetName());
			NUnit.Framework.Assert.AreEqual("name matches", f.GetName(), Sharpen.Extensions.CreateString
				(f.GetNameUTF8(), "UTF-8"));
			NUnit.Framework.Assert.AreEqual("full name matches", n, f.GetFullName());
			NUnit.Framework.Assert.IsTrue("no id", f.GetId() == null);
			NUnit.Framework.Assert.IsTrue("is modified", t.IsModified());
			NUnit.Framework.Assert.IsTrue("has no id", t.GetId() == null);
			NUnit.Framework.Assert.IsTrue("found bob", t.FindBlobMember(f.GetName()) == f);
			TreeEntry[] i = t.Members();
			NUnit.Framework.Assert.IsNotNull("members array not null", i);
			NUnit.Framework.Assert.IsTrue("iterator is not empty", i != null && i.Length > 0);
			NUnit.Framework.Assert.IsTrue("iterator returns file", i != null && i[0] == f);
			NUnit.Framework.Assert.IsTrue("iterator is empty", i != null && i.Length == 1);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Test004_addTree()
		{
			Tree t = new Tree(db);
			t.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue("has id", t.GetId() != null);
			NUnit.Framework.Assert.IsFalse("not modified", t.IsModified());
			string n = "bob";
			Tree f = t.AddTree(n);
			NUnit.Framework.Assert.IsNotNull("have tree", f);
			NUnit.Framework.Assert.AreEqual("name matches", n, f.GetName());
			NUnit.Framework.Assert.AreEqual("name matches", f.GetName(), Sharpen.Extensions.CreateString
				(f.GetNameUTF8(), "UTF-8"));
			NUnit.Framework.Assert.AreEqual("full name matches", n, f.GetFullName());
			NUnit.Framework.Assert.IsTrue("no id", f.GetId() == null);
			NUnit.Framework.Assert.IsTrue("parent matches", f.GetParent() == t);
			NUnit.Framework.Assert.IsTrue("repository matches", f.GetRepository() == db);
			NUnit.Framework.Assert.IsTrue("isLoaded", f.IsLoaded());
			NUnit.Framework.Assert.IsFalse("has items", f.Members().Length > 0);
			NUnit.Framework.Assert.IsFalse("is root", f.IsRoot());
			NUnit.Framework.Assert.IsTrue("tree is self", f.GetTree() == f);
			NUnit.Framework.Assert.IsTrue("parent is modified", t.IsModified());
			NUnit.Framework.Assert.IsTrue("parent has no id", t.GetId() == null);
			NUnit.Framework.Assert.IsTrue("found bob child", t.FindTreeMember(f.GetName()) ==
				 f);
			TreeEntry[] i = t.Members();
			NUnit.Framework.Assert.IsTrue("iterator is not empty", i.Length > 0);
			NUnit.Framework.Assert.IsTrue("iterator returns file", i[0] == f);
			NUnit.Framework.Assert.IsTrue("iterator is empty", i.Length == 1);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Test005_addRecursiveFile()
		{
			Tree t = new Tree(db);
			FileTreeEntry f = t.AddFile("a/b/c");
			NUnit.Framework.Assert.IsNotNull("created f", f);
			NUnit.Framework.Assert.AreEqual("c", f.GetName());
			NUnit.Framework.Assert.AreEqual("b", f.GetParent().GetName());
			NUnit.Framework.Assert.AreEqual("a", f.GetParent().GetParent().GetName());
			NUnit.Framework.Assert.IsTrue("t is great-grandparent", t == f.GetParent().GetParent
				().GetParent());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Test005_addRecursiveTree()
		{
			Tree t = new Tree(db);
			Tree f = t.AddTree("a/b/c");
			NUnit.Framework.Assert.IsNotNull("created f", f);
			NUnit.Framework.Assert.AreEqual("c", f.GetName());
			NUnit.Framework.Assert.AreEqual("b", f.GetParent().GetName());
			NUnit.Framework.Assert.AreEqual("a", f.GetParent().GetParent().GetName());
			NUnit.Framework.Assert.IsTrue("t is great-grandparent", t == f.GetParent().GetParent
				().GetParent());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Test006_addDeepTree()
		{
			Tree t = new Tree(db);
			Tree e = t.AddTree("e");
			NUnit.Framework.Assert.IsNotNull("have e", e);
			NUnit.Framework.Assert.IsTrue("e.parent == t", e.GetParent() == t);
			Tree f = t.AddTree("f");
			NUnit.Framework.Assert.IsNotNull("have f", f);
			NUnit.Framework.Assert.IsTrue("f.parent == t", f.GetParent() == t);
			Tree g = f.AddTree("g");
			NUnit.Framework.Assert.IsNotNull("have g", g);
			NUnit.Framework.Assert.IsTrue("g.parent == f", g.GetParent() == f);
			Tree h = g.AddTree("h");
			NUnit.Framework.Assert.IsNotNull("have h", h);
			NUnit.Framework.Assert.IsTrue("h.parent = g", h.GetParent() == g);
			h.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue("h not modified", !h.IsModified());
			g.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue("g not modified", !g.IsModified());
			f.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue("f not modified", !f.IsModified());
			e.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue("e not modified", !e.IsModified());
			t.SetId(SOME_FAKE_ID);
			NUnit.Framework.Assert.IsTrue("t not modified.", !t.IsModified());
			NUnit.Framework.Assert.AreEqual("full path of h ok", "f/g/h", h.GetFullName());
			NUnit.Framework.Assert.IsTrue("Can find h", t.FindTreeMember(h.GetFullName()) == 
				h);
			NUnit.Framework.Assert.IsTrue("Can't find f/z", t.FindBlobMember("f/z") == null);
			NUnit.Framework.Assert.IsTrue("Can't find y/z", t.FindBlobMember("y/z") == null);
			FileTreeEntry i = h.AddFile("i");
			NUnit.Framework.Assert.IsNotNull(i);
			NUnit.Framework.Assert.AreEqual("full path of i ok", "f/g/h/i", i.GetFullName());
			NUnit.Framework.Assert.IsTrue("Can find i", t.FindBlobMember(i.GetFullName()) == 
				i);
			NUnit.Framework.Assert.IsTrue("h modified", h.IsModified());
			NUnit.Framework.Assert.IsTrue("g modified", g.IsModified());
			NUnit.Framework.Assert.IsTrue("f modified", f.IsModified());
			NUnit.Framework.Assert.IsTrue("e not modified", !e.IsModified());
			NUnit.Framework.Assert.IsTrue("t modified", t.IsModified());
			NUnit.Framework.Assert.IsTrue("h no id", h.GetId() == null);
			NUnit.Framework.Assert.IsTrue("g no id", g.GetId() == null);
			NUnit.Framework.Assert.IsTrue("f no id", f.GetId() == null);
			NUnit.Framework.Assert.IsTrue("e has id", e.GetId() != null);
			NUnit.Framework.Assert.IsTrue("t no id", t.GetId() == null);
		}

		/// <exception cref="System.IO.IOException"></exception>
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
					NUnit.Framework.Assert.IsNotNull("File " + n + " added.", f);
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
				NUnit.Framework.Assert.IsTrue("File " + files[k].GetName() + " is at " + k + ".", 
					files[k] == ents[k]);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
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
		public virtual void Test009_SymlinkAndGitlink()
		{
			Tree symlinkTree = db.MapTree("symlink");
			NUnit.Framework.Assert.IsTrue("Symlink entry exists", symlinkTree.ExistsBlob("symlink.txt"
				));
			Tree gitlinkTree = db.MapTree("gitlink");
			NUnit.Framework.Assert.IsTrue("Gitlink entry exists", gitlinkTree.ExistsBlob("submodule"
				));
		}
	}
}
