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
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Treewalk.Filter
{
	public class PathSuffixFilterTestCase : RepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNonRecursiveFiltering()
		{
			ObjectInserter odi = db.NewObjectInserter();
			ObjectId aSth = odi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(
				"a.sth"));
			ObjectId aTxt = odi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(
				"a.txt"));
			DirCache dc = db.ReadDirCache();
			DirCacheBuilder builder = dc.Builder();
			DirCacheEntry aSthEntry = new DirCacheEntry("a.sth");
			aSthEntry.FileMode = FileMode.REGULAR_FILE;
			aSthEntry.SetObjectId(aSth);
			DirCacheEntry aTxtEntry = new DirCacheEntry("a.txt");
			aTxtEntry.FileMode = FileMode.REGULAR_FILE;
			aTxtEntry.SetObjectId(aTxt);
			builder.Add(aSthEntry);
			builder.Add(aTxtEntry);
			builder.Finish();
			ObjectId treeId = dc.WriteTree(odi);
			odi.Flush();
			TreeWalk tw = new TreeWalk(db);
			tw.Filter = PathSuffixFilter.Create(".txt");
			tw.AddTree(treeId);
			IList<string> paths = new List<string>();
			while (tw.Next())
			{
				paths.AddItem(tw.PathString);
			}
			IList<string> expected = new List<string>();
			expected.AddItem("a.txt");
			NUnit.Framework.Assert.AreEqual(expected, paths);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRecursiveFiltering()
		{
			ObjectInserter odi = db.NewObjectInserter();
			ObjectId aSth = odi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(
				"a.sth"));
			ObjectId aTxt = odi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(
				"a.txt"));
			ObjectId bSth = odi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(
				"b.sth"));
			ObjectId bTxt = odi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(
				"b.txt"));
			DirCache dc = db.ReadDirCache();
			DirCacheBuilder builder = dc.Builder();
			DirCacheEntry aSthEntry = new DirCacheEntry("a.sth");
			aSthEntry.FileMode = FileMode.REGULAR_FILE;
			aSthEntry.SetObjectId(aSth);
			DirCacheEntry aTxtEntry = new DirCacheEntry("a.txt");
			aTxtEntry.FileMode = FileMode.REGULAR_FILE;
			aTxtEntry.SetObjectId(aTxt);
			builder.Add(aSthEntry);
			builder.Add(aTxtEntry);
			DirCacheEntry bSthEntry = new DirCacheEntry("sub/b.sth");
			bSthEntry.FileMode = FileMode.REGULAR_FILE;
			bSthEntry.SetObjectId(bSth);
			DirCacheEntry bTxtEntry = new DirCacheEntry("sub/b.txt");
			bTxtEntry.FileMode = FileMode.REGULAR_FILE;
			bTxtEntry.SetObjectId(bTxt);
			builder.Add(bSthEntry);
			builder.Add(bTxtEntry);
			builder.Finish();
			ObjectId treeId = dc.WriteTree(odi);
			odi.Flush();
			TreeWalk tw = new TreeWalk(db);
			tw.Recursive = true;
			tw.Filter = PathSuffixFilter.Create(".txt");
			tw.AddTree(treeId);
			IList<string> paths = new List<string>();
			while (tw.Next())
			{
				paths.AddItem(tw.PathString);
			}
			IList<string> expected = new List<string>();
			expected.AddItem("a.txt");
			expected.AddItem("sub/b.txt");
			NUnit.Framework.Assert.AreEqual(expected, paths);
		}
	}
}
