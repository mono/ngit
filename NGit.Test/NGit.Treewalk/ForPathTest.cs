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
using Sharpen;

namespace NGit.Treewalk
{
	[NUnit.Framework.TestFixture]
	public class ForPathTest : RepositoryTestCase
	{
		private static readonly FileMode SYMLINK = FileMode.SYMLINK;

		private static readonly FileMode REGULAR_FILE = FileMode.REGULAR_FILE;

		private static readonly FileMode EXECUTABLE_FILE = FileMode.EXECUTABLE_FILE;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFindObjects()
		{
			DirCache tree0 = DirCache.NewInCore();
			DirCacheBuilder b0 = tree0.Builder();
			ObjectReader or = db.NewObjectReader();
			ObjectInserter oi = db.NewObjectInserter();
			DirCacheEntry aDotB = CreateEntry("a.b", EXECUTABLE_FILE);
			b0.Add(aDotB);
			DirCacheEntry aSlashB = CreateEntry("a/b", REGULAR_FILE);
			b0.Add(aSlashB);
			DirCacheEntry aSlashCSlashD = CreateEntry("a/c/d", REGULAR_FILE);
			b0.Add(aSlashCSlashD);
			DirCacheEntry aZeroB = CreateEntry("a0b", SYMLINK);
			b0.Add(aZeroB);
			b0.Finish();
			NUnit.Framework.Assert.AreEqual(4, tree0.GetEntryCount());
			ObjectId tree = tree0.WriteTree(oi);
			// Find the directories that were implicitly created above.
			TreeWalk tw = new TreeWalk(or);
			tw.AddTree(tree);
			ObjectId a = null;
			ObjectId aSlashC = null;
			while (tw.Next())
			{
				if (tw.PathString.Equals("a"))
				{
					a = tw.GetObjectId(0);
					tw.EnterSubtree();
					while (tw.Next())
					{
						if (tw.PathString.Equals("a/c"))
						{
							aSlashC = tw.GetObjectId(0);
							break;
						}
					}
					break;
				}
			}
			NUnit.Framework.Assert.AreEqual(a, TreeWalk.ForPath(or, "a", tree).GetObjectId(0)
				);
			NUnit.Framework.Assert.AreEqual(a, TreeWalk.ForPath(or, "a/", tree).GetObjectId(0
				));
			NUnit.Framework.Assert.AreEqual(null, TreeWalk.ForPath(or, "/a", tree));
			NUnit.Framework.Assert.AreEqual(null, TreeWalk.ForPath(or, "/a/", tree));
			NUnit.Framework.Assert.AreEqual(aDotB.GetObjectId(), TreeWalk.ForPath(or, "a.b", 
				tree).GetObjectId(0));
			NUnit.Framework.Assert.AreEqual(null, TreeWalk.ForPath(or, "/a.b", tree));
			NUnit.Framework.Assert.AreEqual(null, TreeWalk.ForPath(or, "/a.b/", tree));
			NUnit.Framework.Assert.AreEqual(aDotB.GetObjectId(), TreeWalk.ForPath(or, "a.b/", 
				tree).GetObjectId(0));
			NUnit.Framework.Assert.AreEqual(aZeroB.GetObjectId(), TreeWalk.ForPath(or, "a0b", 
				tree).GetObjectId(0));
			NUnit.Framework.Assert.AreEqual(aSlashB.GetObjectId(), TreeWalk.ForPath(or, "a/b"
				, tree).GetObjectId(0));
			NUnit.Framework.Assert.AreEqual(aSlashB.GetObjectId(), TreeWalk.ForPath(or, "b", 
				a).GetObjectId(0));
			NUnit.Framework.Assert.AreEqual(aSlashC, TreeWalk.ForPath(or, "a/c", tree).GetObjectId
				(0));
			NUnit.Framework.Assert.AreEqual(aSlashC, TreeWalk.ForPath(or, "c", a).GetObjectId
				(0));
			NUnit.Framework.Assert.AreEqual(aSlashCSlashD.GetObjectId(), TreeWalk.ForPath(or, 
				"a/c/d", tree).GetObjectId(0));
			NUnit.Framework.Assert.AreEqual(aSlashCSlashD.GetObjectId(), TreeWalk.ForPath(or, 
				"c/d", a).GetObjectId(0));
			or.Release();
			oi.Release();
		}
	}
}
