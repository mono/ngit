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
using NGit.Errors;
using Sharpen;
using System.Collections.Generic;

namespace NGit
{
	public class WorkDirCheckoutTest : RepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFindingConflicts()
		{
			GitIndex index = new GitIndex(db);
			index.Add(trash, WriteTrashFile("bar", "bar"));
			index.Add(trash, WriteTrashFile("foo/bar/baz/qux", "foo/bar"));
			RecursiveDelete(new FilePath(trash, "bar"));
			RecursiveDelete(new FilePath(trash, "foo"));
			WriteTrashFile("bar/baz/qux/foo", "another nasty one");
			WriteTrashFile("foo", "troublesome little bugger");
			WorkDirCheckout workDirCheckout = new WorkDirCheckout(db, trash, index, index);
			workDirCheckout.PrescanOneTree();
			IList<string> conflictingEntries = workDirCheckout.GetConflicts();
			IList<string> removedEntries = workDirCheckout.GetRemoved();
			NUnit.Framework.Assert.AreEqual("bar/baz/qux/foo", conflictingEntries[0]);
			NUnit.Framework.Assert.AreEqual("foo", conflictingEntries[1]);
			GitIndex index2 = new GitIndex(db);
			RecursiveDelete(new FilePath(trash, "bar"));
			RecursiveDelete(new FilePath(trash, "foo"));
			index2.Add(trash, WriteTrashFile("bar/baz/qux/foo", "bar"));
			index2.Add(trash, WriteTrashFile("foo", "lalala"));
			workDirCheckout = new WorkDirCheckout(db, trash, index2, index);
			workDirCheckout.PrescanOneTree();
			conflictingEntries = workDirCheckout.GetConflicts();
			removedEntries = workDirCheckout.GetRemoved();
			NUnit.Framework.Assert.IsTrue(conflictingEntries.IsEmpty());
			NUnit.Framework.Assert.IsTrue(removedEntries.Contains("bar/baz/qux/foo"));
			NUnit.Framework.Assert.IsTrue(removedEntries.Contains("foo"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckingOutWithConflicts()
		{
			GitIndex index = new GitIndex(db);
			index.Add(trash, WriteTrashFile("bar", "bar"));
			index.Add(trash, WriteTrashFile("foo/bar/baz/qux", "foo/bar"));
			RecursiveDelete(new FilePath(trash, "bar"));
			RecursiveDelete(new FilePath(trash, "foo"));
			WriteTrashFile("bar/baz/qux/foo", "another nasty one");
			WriteTrashFile("foo", "troublesome little bugger");
			try
			{
				WorkDirCheckout workDirCheckout = new WorkDirCheckout(db, trash, index, index);
				workDirCheckout.Checkout();
				NUnit.Framework.Assert.Fail("Should have thrown exception");
			}
			catch (CheckoutConflictException)
			{
			}
			// all is well
			WorkDirCheckout workDirCheckout_1 = new WorkDirCheckout(db, trash, index, index);
			workDirCheckout_1.SetFailOnConflict(false);
			workDirCheckout_1.Checkout();
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "bar").IsFile());
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "foo/bar/baz/qux").IsFile());
			GitIndex index2 = new GitIndex(db);
			RecursiveDelete(new FilePath(trash, "bar"));
			RecursiveDelete(new FilePath(trash, "foo"));
			index2.Add(trash, WriteTrashFile("bar/baz/qux/foo", "bar"));
			WriteTrashFile("bar/baz/qux/bar", "evil? I thought it said WEEVIL!");
			index2.Add(trash, WriteTrashFile("foo", "lalala"));
			workDirCheckout_1 = new WorkDirCheckout(db, trash, index2, index);
			workDirCheckout_1.SetFailOnConflict(false);
			workDirCheckout_1.Checkout();
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "bar").IsFile());
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "foo/bar/baz/qux").IsFile());
			NUnit.Framework.Assert.IsNotNull(index2.GetEntry("bar"));
			NUnit.Framework.Assert.IsNotNull(index2.GetEntry("foo/bar/baz/qux"));
			NUnit.Framework.Assert.IsNull(index2.GetEntry("bar/baz/qux/foo"));
			NUnit.Framework.Assert.IsNull(index2.GetEntry("foo"));
		}
	}
}
