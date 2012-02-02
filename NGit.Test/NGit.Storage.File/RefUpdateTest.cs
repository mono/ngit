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
using NGit.Revwalk;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	[NUnit.Framework.TestFixture]
	public class RefUpdateTest : SampleDataRepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		private void WriteSymref(string src, string dst)
		{
			RefUpdate u = db.UpdateRef(src);
			switch (u.Link(dst))
			{
				case RefUpdate.Result.NEW:
				case RefUpdate.Result.FORCED:
				case RefUpdate.Result.NO_CHANGE:
				{
					break;
				}

				default:
				{
					NUnit.Framework.Assert.Fail("link " + src + " to " + dst);
					break;
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private RefUpdate UpdateRef(string name)
		{
			RefUpdate @ref = db.UpdateRef(name);
			@ref.SetNewObjectId(db.Resolve(Constants.HEAD));
			return @ref;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Delete(RefUpdate @ref, RefUpdate.Result expected)
		{
			Delete(@ref, expected, true, true);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Delete(RefUpdate @ref, RefUpdate.Result expected, bool exists, bool 
			removed)
		{
			NUnit.Framework.Assert.AreEqual(exists, db.GetAllRefs().ContainsKey(@ref.GetName(
				)));
			NUnit.Framework.Assert.AreEqual(expected, @ref.Delete());
			NUnit.Framework.Assert.AreEqual(!removed, db.GetAllRefs().ContainsKey(@ref.GetName
				()));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNoCacheObjectIdSubclass()
		{
			string newRef = "refs/heads/abc";
			RefUpdate ru = UpdateRef(newRef);
			RefUpdateTest.SubclassedId newid = new RefUpdateTest.SubclassedId(ru.GetNewObjectId
				());
			ru.SetNewObjectId(newid);
			RefUpdate.Result update = ru.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, update);
			Ref r = db.GetAllRefs().Get(newRef);
			NUnit.Framework.Assert.IsNotNull(r);
			NUnit.Framework.Assert.AreEqual(newRef, r.GetName());
			NUnit.Framework.Assert.IsNotNull(r.GetObjectId());
			NUnit.Framework.Assert.AreNotSame(newid, r.GetObjectId());
			NUnit.Framework.Assert.AreSame(typeof(ObjectId), r.GetObjectId().GetType());
			NUnit.Framework.Assert.AreEqual(newid, r.GetObjectId());
			IList<ReflogEntry> reverseEntries1 = db.GetReflogReader("refs/heads/abc").GetReverseEntries
				();
			ReflogEntry entry1 = reverseEntries1[0];
			NUnit.Framework.Assert.AreEqual(1, reverseEntries1.Count);
			NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, entry1.GetOldId());
			NUnit.Framework.Assert.AreEqual(r.GetObjectId(), entry1.GetNewId());
			NUnit.Framework.Assert.AreEqual(new PersonIdent(db).ToString(), entry1.GetWho().ToString
				());
			NUnit.Framework.Assert.AreEqual(string.Empty, entry1.GetComment());
			IList<ReflogEntry> reverseEntries2 = db.GetReflogReader("HEAD").GetReverseEntries
				();
			NUnit.Framework.Assert.AreEqual(0, reverseEntries2.Count);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNewNamespaceConflictWithLoosePrefixNameExists()
		{
			string newRef = "refs/heads/z";
			RefUpdate ru = UpdateRef(newRef);
			RefUpdate.Result update = ru.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, update);
			// end setup
			string newRef2 = "refs/heads/z/a";
			RefUpdate ru2 = UpdateRef(newRef2);
			RefUpdate.Result update2 = ru2.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.LOCK_FAILURE, update2);
			NUnit.Framework.Assert.AreEqual(1, db.GetReflogReader("refs/heads/z").GetReverseEntries
				().Count);
			NUnit.Framework.Assert.AreEqual(0, db.GetReflogReader("HEAD").GetReverseEntries()
				.Count);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNewNamespaceConflictWithPackedPrefixNameExists()
		{
			string newRef = "refs/heads/master/x";
			RefUpdate ru = UpdateRef(newRef);
			RefUpdate.Result update = ru.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.LOCK_FAILURE, update);
			NUnit.Framework.Assert.IsNull(db.GetReflogReader("refs/heads/master/x"));
			NUnit.Framework.Assert.AreEqual(0, db.GetReflogReader("HEAD").GetReverseEntries()
				.Count);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNewNamespaceConflictWithLoosePrefixOfExisting()
		{
			string newRef = "refs/heads/z/a";
			RefUpdate ru = UpdateRef(newRef);
			RefUpdate.Result update = ru.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, update);
			// end setup
			string newRef2 = "refs/heads/z";
			RefUpdate ru2 = UpdateRef(newRef2);
			RefUpdate.Result update2 = ru2.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.LOCK_FAILURE, update2);
			NUnit.Framework.Assert.AreEqual(1, db.GetReflogReader("refs/heads/z/a").GetReverseEntries
				().Count);
			NUnit.Framework.Assert.IsNull(db.GetReflogReader("refs/heads/z"));
			NUnit.Framework.Assert.AreEqual(0, db.GetReflogReader("HEAD").GetReverseEntries()
				.Count);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNewNamespaceConflictWithPackedPrefixOfExisting()
		{
			string newRef = "refs/heads/prefix";
			RefUpdate ru = UpdateRef(newRef);
			RefUpdate.Result update = ru.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.LOCK_FAILURE, update);
			NUnit.Framework.Assert.IsNull(db.GetReflogReader("refs/heads/prefix"));
			NUnit.Framework.Assert.AreEqual(0, db.GetReflogReader("HEAD").GetReverseEntries()
				.Count);
		}

		/// <summary>Delete a ref that is pointed to by HEAD</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteHEADreferencedRef()
		{
			ObjectId pid = db.Resolve("refs/heads/master^");
			RefUpdate updateRef = db.UpdateRef("refs/heads/master");
			updateRef.SetNewObjectId(pid);
			updateRef.SetForceUpdate(true);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update);
			// internal
			RefUpdate updateRef2 = db.UpdateRef("refs/heads/master");
			RefUpdate.Result delete = updateRef2.Delete();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.REJECTED_CURRENT_BRANCH, delete);
			NUnit.Framework.Assert.AreEqual(pid, db.Resolve("refs/heads/master"));
			NUnit.Framework.Assert.AreEqual(1, db.GetReflogReader("refs/heads/master").GetReverseEntries
				().Count);
			NUnit.Framework.Assert.AreEqual(0, db.GetReflogReader("HEAD").GetReverseEntries()
				.Count);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLooseDelete()
		{
			string newRef = "refs/heads/abc";
			RefUpdate @ref = UpdateRef(newRef);
			@ref.Update();
			// create loose ref
			@ref = UpdateRef(newRef);
			// refresh
			Delete(@ref, RefUpdate.Result.NO_CHANGE);
			NUnit.Framework.Assert.IsNull(db.GetReflogReader("refs/heads/abc"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteHead()
		{
			RefUpdate @ref = UpdateRef(Constants.HEAD);
			Delete(@ref, RefUpdate.Result.REJECTED_CURRENT_BRANCH, true, false);
			NUnit.Framework.Assert.AreEqual(0, db.GetReflogReader("refs/heads/master").GetReverseEntries
				().Count);
			NUnit.Framework.Assert.AreEqual(0, db.GetReflogReader("HEAD").GetReverseEntries()
				.Count);
		}

		/// <summary>
		/// Delete a loose ref and make sure the directory in refs is deleted too,
		/// and the reflog dir too
		/// </summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteLooseAndItsDirectory()
		{
			ObjectId pid = db.Resolve("refs/heads/c^");
			RefUpdate updateRef = db.UpdateRef("refs/heads/z/c");
			updateRef.SetNewObjectId(pid);
			updateRef.SetForceUpdate(true);
			updateRef.SetRefLogMessage("new test ref", false);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, update);
			// internal
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, Constants.R_HEADS + "z")
				.Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "logs/refs/heads/z").Exists
				());
			// The real test here
			RefUpdate updateRef2 = db.UpdateRef("refs/heads/z/c");
			updateRef2.SetForceUpdate(true);
			RefUpdate.Result delete = updateRef2.Delete();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, delete);
			NUnit.Framework.Assert.IsNull(db.Resolve("refs/heads/z/c"));
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, Constants.R_HEADS + "z"
				).Exists());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "logs/refs/heads/z").Exists
				());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteNotFound()
		{
			RefUpdate @ref = UpdateRef("refs/heads/xyz");
			Delete(@ref, RefUpdate.Result.NEW, false, true);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteFastForward()
		{
			RefUpdate @ref = UpdateRef("refs/heads/a");
			Delete(@ref, RefUpdate.Result.FAST_FORWARD);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteForce()
		{
			RefUpdate @ref = db.UpdateRef("refs/heads/b");
			@ref.SetNewObjectId(db.Resolve("refs/heads/a"));
			Delete(@ref, RefUpdate.Result.REJECTED, true, false);
			@ref.SetForceUpdate(true);
			Delete(@ref, RefUpdate.Result.FORCED);
		}

		[NUnit.Framework.Test]
		public virtual void TestRefKeySameAsName()
		{
			IDictionary<string, Ref> allRefs = db.GetAllRefs();
			foreach (KeyValuePair<string, Ref> e in allRefs.EntrySet())
			{
				NUnit.Framework.Assert.AreEqual(e.Key, e.Value.GetName());
			}
		}

		/// <summary>Try modify a ref forward, fast forward</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateRefForward()
		{
			ObjectId ppid = db.Resolve("refs/heads/master^");
			ObjectId pid = db.Resolve("refs/heads/master");
			RefUpdate updateRef = db.UpdateRef("refs/heads/master");
			updateRef.SetNewObjectId(ppid);
			updateRef.SetForceUpdate(true);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update);
			NUnit.Framework.Assert.AreEqual(ppid, db.Resolve("refs/heads/master"));
			// real test
			RefUpdate updateRef2 = db.UpdateRef("refs/heads/master");
			updateRef2.SetNewObjectId(pid);
			RefUpdate.Result update2 = updateRef2.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FAST_FORWARD, update2);
			NUnit.Framework.Assert.AreEqual(pid, db.Resolve("refs/heads/master"));
		}

		/// <summary>Update the HEAD ref.</summary>
		/// <remarks>Update the HEAD ref. Only it should be changed, not what it points to.</remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateRefDetached()
		{
			ObjectId pid = db.Resolve("refs/heads/master");
			ObjectId ppid = db.Resolve("refs/heads/master^");
			RefUpdate updateRef = db.UpdateRef("HEAD", true);
			updateRef.SetForceUpdate(true);
			updateRef.SetNewObjectId(ppid);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update);
			NUnit.Framework.Assert.AreEqual(ppid, db.Resolve("HEAD"));
			Ref @ref = db.GetRef("HEAD");
			NUnit.Framework.Assert.AreEqual("HEAD", @ref.GetName());
			NUnit.Framework.Assert.IsTrue(!@ref.IsSymbolic(), "is detached");
			// the branch HEAD referred to is left untouched
			NUnit.Framework.Assert.AreEqual(pid, db.Resolve("refs/heads/master"));
			ReflogReader reflogReader = new ReflogReader(db, "HEAD");
			ReflogEntry e = reflogReader.GetReverseEntries()[0];
			NUnit.Framework.Assert.AreEqual(pid, e.GetOldId());
			NUnit.Framework.Assert.AreEqual(ppid, e.GetNewId());
			NUnit.Framework.Assert.AreEqual("GIT_COMMITTER_EMAIL", e.GetWho().GetEmailAddress
				());
			NUnit.Framework.Assert.AreEqual("GIT_COMMITTER_NAME", e.GetWho().GetName());
			NUnit.Framework.Assert.AreEqual(1250379778000L, e.GetWho().GetWhen().GetTime());
		}

		/// <summary>Update the HEAD ref when the referenced branch is unborn</summary>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateRefDetachedUnbornHead()
		{
			ObjectId ppid = db.Resolve("refs/heads/master^");
			WriteSymref("HEAD", "refs/heads/unborn");
			RefUpdate updateRef = db.UpdateRef("HEAD", true);
			updateRef.SetForceUpdate(true);
			updateRef.SetNewObjectId(ppid);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, update);
			NUnit.Framework.Assert.AreEqual(ppid, db.Resolve("HEAD"));
			Ref @ref = db.GetRef("HEAD");
			NUnit.Framework.Assert.AreEqual("HEAD", @ref.GetName());
			NUnit.Framework.Assert.IsTrue(!@ref.IsSymbolic(), "is detached");
			// the branch HEAD referred to is left untouched
			NUnit.Framework.Assert.IsNull(db.Resolve("refs/heads/unborn"));
			ReflogReader reflogReader = new ReflogReader(db, "HEAD");
			ReflogEntry e = reflogReader.GetReverseEntries()[0];
			NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, e.GetOldId());
			NUnit.Framework.Assert.AreEqual(ppid, e.GetNewId());
			NUnit.Framework.Assert.AreEqual("GIT_COMMITTER_EMAIL", e.GetWho().GetEmailAddress
				());
			NUnit.Framework.Assert.AreEqual("GIT_COMMITTER_NAME", e.GetWho().GetName());
			NUnit.Framework.Assert.AreEqual(1250379778000L, e.GetWho().GetWhen().GetTime());
		}

		/// <summary>Delete a ref that exists both as packed and loose.</summary>
		/// <remarks>
		/// Delete a ref that exists both as packed and loose. Make sure the ref
		/// cannot be resolved after delete.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteLoosePacked()
		{
			ObjectId pid = db.Resolve("refs/heads/c^");
			RefUpdate updateRef = db.UpdateRef("refs/heads/c");
			updateRef.SetNewObjectId(pid);
			updateRef.SetForceUpdate(true);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update);
			// internal
			// The real test here
			RefUpdate updateRef2 = db.UpdateRef("refs/heads/c");
			updateRef2.SetForceUpdate(true);
			RefUpdate.Result delete = updateRef2.Delete();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, delete);
			NUnit.Framework.Assert.IsNull(db.Resolve("refs/heads/c"));
		}

		/// <summary>Try modify a ref to same</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateRefNoChange()
		{
			ObjectId pid = db.Resolve("refs/heads/master");
			RefUpdate updateRef = db.UpdateRef("refs/heads/master");
			updateRef.SetNewObjectId(pid);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NO_CHANGE, update);
			NUnit.Framework.Assert.AreEqual(pid, db.Resolve("refs/heads/master"));
		}

		/// <summary>
		/// Test case originating from
		/// <a href="http://bugs.eclipse.org/285991">bug 285991</a>
		/// Make sure the in memory cache is updated properly after
		/// update of symref.
		/// </summary>
		/// <remarks>
		/// Test case originating from
		/// <a href="http://bugs.eclipse.org/285991">bug 285991</a>
		/// Make sure the in memory cache is updated properly after
		/// update of symref. This one did not fail because the
		/// ref was packed due to implementation issues.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestRefsCacheAfterUpdate()
		{
			// Do not use the defalt repo for this case.
			IDictionary<string, Ref> allRefs = db.GetAllRefs();
			ObjectId oldValue = db.Resolve("HEAD");
			ObjectId newValue = db.Resolve("HEAD^");
			// first make HEAD refer to loose ref
			RefUpdate updateRef = db.UpdateRef(Constants.HEAD);
			updateRef.SetForceUpdate(true);
			updateRef.SetNewObjectId(newValue);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update);
			// now update that ref
			updateRef = db.UpdateRef(Constants.HEAD);
			updateRef.SetForceUpdate(true);
			updateRef.SetNewObjectId(oldValue);
			update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FAST_FORWARD, update);
			allRefs = db.GetAllRefs();
			Ref master = allRefs.Get("refs/heads/master");
			Ref head = allRefs.Get("HEAD");
			NUnit.Framework.Assert.AreEqual("refs/heads/master", master.GetName());
			NUnit.Framework.Assert.AreEqual("HEAD", head.GetName());
			NUnit.Framework.Assert.IsTrue(head.IsSymbolic(), "is symbolic reference");
			NUnit.Framework.Assert.AreSame(master, head.GetTarget());
		}

		/// <summary>
		/// Test case originating from
		/// <a href="http://bugs.eclipse.org/285991">bug 285991</a>
		/// Make sure the in memory cache is updated properly after
		/// update of symref.
		/// </summary>
		/// <remarks>
		/// Test case originating from
		/// <a href="http://bugs.eclipse.org/285991">bug 285991</a>
		/// Make sure the in memory cache is updated properly after
		/// update of symref.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestRefsCacheAfterUpdateLooseOnly()
		{
			// Do not use the defalt repo for this case.
			IDictionary<string, Ref> allRefs = db.GetAllRefs();
			ObjectId oldValue = db.Resolve("HEAD");
			WriteSymref(Constants.HEAD, "refs/heads/newref");
			RefUpdate updateRef = db.UpdateRef(Constants.HEAD);
			updateRef.SetForceUpdate(true);
			updateRef.SetNewObjectId(oldValue);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, update);
			allRefs = db.GetAllRefs();
			Ref head = allRefs.Get("HEAD");
			Ref newref = allRefs.Get("refs/heads/newref");
			NUnit.Framework.Assert.AreEqual("refs/heads/newref", newref.GetName());
			NUnit.Framework.Assert.AreEqual("HEAD", head.GetName());
			NUnit.Framework.Assert.IsTrue(head.IsSymbolic(), "is symbolic reference");
			NUnit.Framework.Assert.AreSame(newref, head.GetTarget());
		}

		/// <summary>Try modify a ref, but get wrong expected old value</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateRefLockFailureWrongOldValue()
		{
			ObjectId pid = db.Resolve("refs/heads/master");
			RefUpdate updateRef = db.UpdateRef("refs/heads/master");
			updateRef.SetNewObjectId(pid);
			updateRef.SetExpectedOldObjectId(db.Resolve("refs/heads/master^"));
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.LOCK_FAILURE, update);
			NUnit.Framework.Assert.AreEqual(pid, db.Resolve("refs/heads/master"));
		}

		/// <summary>Try modify a ref forward, fast forward, checking old value first</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateRefForwardWithCheck1()
		{
			ObjectId ppid = db.Resolve("refs/heads/master^");
			ObjectId pid = db.Resolve("refs/heads/master");
			RefUpdate updateRef = db.UpdateRef("refs/heads/master");
			updateRef.SetNewObjectId(ppid);
			updateRef.SetForceUpdate(true);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update);
			NUnit.Framework.Assert.AreEqual(ppid, db.Resolve("refs/heads/master"));
			// real test
			RefUpdate updateRef2 = db.UpdateRef("refs/heads/master");
			updateRef2.SetExpectedOldObjectId(ppid);
			updateRef2.SetNewObjectId(pid);
			RefUpdate.Result update2 = updateRef2.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FAST_FORWARD, update2);
			NUnit.Framework.Assert.AreEqual(pid, db.Resolve("refs/heads/master"));
		}

		/// <summary>Try modify a ref forward, fast forward, checking old commit first</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateRefForwardWithCheck2()
		{
			ObjectId ppid = db.Resolve("refs/heads/master^");
			ObjectId pid = db.Resolve("refs/heads/master");
			RefUpdate updateRef = db.UpdateRef("refs/heads/master");
			updateRef.SetNewObjectId(ppid);
			updateRef.SetForceUpdate(true);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update);
			NUnit.Framework.Assert.AreEqual(ppid, db.Resolve("refs/heads/master"));
			// real test
			RevCommit old = new RevWalk(db).ParseCommit(ppid);
			RefUpdate updateRef2 = db.UpdateRef("refs/heads/master");
			updateRef2.SetExpectedOldObjectId(old);
			updateRef2.SetNewObjectId(pid);
			RefUpdate.Result update2 = updateRef2.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FAST_FORWARD, update2);
			NUnit.Framework.Assert.AreEqual(pid, db.Resolve("refs/heads/master"));
		}

		/// <summary>Try modify a ref that is locked</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestUpdateRefLockFailureLocked()
		{
			ObjectId opid = db.Resolve("refs/heads/master");
			ObjectId pid = db.Resolve("refs/heads/master^");
			RefUpdate updateRef = db.UpdateRef("refs/heads/master");
			updateRef.SetNewObjectId(pid);
			LockFile lockFile1 = new LockFile(new FilePath(db.Directory, "refs/heads/master")
				, db.FileSystem);
			try
			{
				NUnit.Framework.Assert.IsTrue(lockFile1.Lock());
				// precondition to test
				RefUpdate.Result update = updateRef.Update();
				NUnit.Framework.Assert.AreEqual(RefUpdate.Result.LOCK_FAILURE, update);
				NUnit.Framework.Assert.AreEqual(opid, db.Resolve("refs/heads/master"));
				LockFile lockFile2 = new LockFile(new FilePath(db.Directory, "refs/heads/master")
					, db.FileSystem);
				NUnit.Framework.Assert.IsFalse(lockFile2.Lock());
			}
			finally
			{
				// was locked, still is
				lockFile1.Unlock();
			}
		}

		/// <summary>Try to delete a ref.</summary>
		/// <remarks>Try to delete a ref. Delete requires force.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteLoosePackedRejected()
		{
			ObjectId pid = db.Resolve("refs/heads/c^");
			ObjectId oldpid = db.Resolve("refs/heads/c");
			RefUpdate updateRef = db.UpdateRef("refs/heads/c");
			updateRef.SetNewObjectId(pid);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.REJECTED, update);
			NUnit.Framework.Assert.AreEqual(oldpid, db.Resolve("refs/heads/c"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameBranchNoPreviousLog()
		{
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "logs/refs/heads/b").Exists
				(), "precondition, no log on old branchg");
			ObjectId rb = db.Resolve("refs/heads/b");
			ObjectId oldHead = db.Resolve(Constants.HEAD);
			NUnit.Framework.Assert.IsFalse(rb.Equals(oldHead));
			// assumption for this test
			RefRename renameRef = db.RenameRef("refs/heads/b", "refs/heads/new/name");
			RefUpdate.Result result = renameRef.Rename();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.RENAMED, result);
			NUnit.Framework.Assert.AreEqual(rb, db.Resolve("refs/heads/new/name"));
			NUnit.Framework.Assert.IsNull(db.Resolve("refs/heads/b"));
			NUnit.Framework.Assert.AreEqual(1, db.GetReflogReader("new/name").GetReverseEntries
				().Count);
			NUnit.Framework.Assert.AreEqual("Branch: renamed b to new/name", db.GetReflogReader
				("new/name").GetLastEntry().GetComment());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "logs/refs/heads/b").Exists
				());
			NUnit.Framework.Assert.AreEqual(oldHead, db.Resolve(Constants.HEAD));
		}

		// unchanged
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameBranchHasPreviousLog()
		{
			ObjectId rb = db.Resolve("refs/heads/b");
			ObjectId oldHead = db.Resolve(Constants.HEAD);
			NUnit.Framework.Assert.IsFalse(rb.Equals(oldHead), "precondition for this test, branch b != HEAD"
				);
			WriteReflog(db, rb, rb, "Just a message", "refs/heads/b");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "logs/refs/heads/b").Exists
				(), "log on old branch");
			RefRename renameRef = db.RenameRef("refs/heads/b", "refs/heads/new/name");
			RefUpdate.Result result = renameRef.Rename();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.RENAMED, result);
			NUnit.Framework.Assert.AreEqual(rb, db.Resolve("refs/heads/new/name"));
			NUnit.Framework.Assert.IsNull(db.Resolve("refs/heads/b"));
			NUnit.Framework.Assert.AreEqual(2, db.GetReflogReader("new/name").GetReverseEntries
				().Count);
			NUnit.Framework.Assert.AreEqual("Branch: renamed b to new/name", db.GetReflogReader
				("new/name").GetLastEntry().GetComment());
			NUnit.Framework.Assert.AreEqual("Just a message", db.GetReflogReader("new/name").
				GetReverseEntries()[1].GetComment());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "logs/refs/heads/b").Exists
				());
			NUnit.Framework.Assert.AreEqual(oldHead, db.Resolve(Constants.HEAD));
		}

		// unchanged
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameCurrentBranch()
		{
			ObjectId rb = db.Resolve("refs/heads/b");
			WriteSymref(Constants.HEAD, "refs/heads/b");
			ObjectId oldHead = db.Resolve(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(rb.Equals(oldHead), "internal test condition, b == HEAD"
				);
			WriteReflog(db, rb, rb, "Just a message", "refs/heads/b");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "logs/refs/heads/b").Exists
				(), "log on old branch");
			RefRename renameRef = db.RenameRef("refs/heads/b", "refs/heads/new/name");
			RefUpdate.Result result = renameRef.Rename();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.RENAMED, result);
			NUnit.Framework.Assert.AreEqual(rb, db.Resolve("refs/heads/new/name"));
			NUnit.Framework.Assert.IsNull(db.Resolve("refs/heads/b"));
			NUnit.Framework.Assert.AreEqual("Branch: renamed b to new/name", db.GetReflogReader
				("new/name").GetLastEntry().GetComment());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "logs/refs/heads/b").Exists
				());
			NUnit.Framework.Assert.AreEqual(rb, db.Resolve(Constants.HEAD));
			NUnit.Framework.Assert.AreEqual(2, db.GetReflogReader("new/name").GetReverseEntries
				().Count);
			NUnit.Framework.Assert.AreEqual("Branch: renamed b to new/name", db.GetReflogReader
				("new/name").GetReverseEntries()[0].GetComment());
			NUnit.Framework.Assert.AreEqual("Just a message", db.GetReflogReader("new/name").
				GetReverseEntries()[1].GetComment());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameBranchAlsoInPack()
		{
			ObjectId rb = db.Resolve("refs/heads/b");
			ObjectId rb2 = db.Resolve("refs/heads/b~1");
			NUnit.Framework.Assert.AreEqual(RefStorage.PACKED, db.GetRef("refs/heads/b").GetStorage
				());
			RefUpdate updateRef = db.UpdateRef("refs/heads/b");
			updateRef.SetNewObjectId(rb2);
			updateRef.SetForceUpdate(true);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update, "internal check new ref is loose"
				);
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, db.GetRef("refs/heads/b").GetStorage
				());
			WriteReflog(db, rb, rb, "Just a message", "refs/heads/b");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "logs/refs/heads/b").Exists
				(), "log on old branch");
			RefRename renameRef = db.RenameRef("refs/heads/b", "refs/heads/new/name");
			RefUpdate.Result result = renameRef.Rename();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.RENAMED, result);
			NUnit.Framework.Assert.AreEqual(rb2, db.Resolve("refs/heads/new/name"));
			NUnit.Framework.Assert.IsNull(db.Resolve("refs/heads/b"));
			NUnit.Framework.Assert.AreEqual("Branch: renamed b to new/name", db.GetReflogReader
				("new/name").GetLastEntry().GetComment());
			NUnit.Framework.Assert.AreEqual(3, db.GetReflogReader("refs/heads/new/name").GetReverseEntries
				().Count);
			NUnit.Framework.Assert.AreEqual("Branch: renamed b to new/name", db.GetReflogReader
				("refs/heads/new/name").GetReverseEntries()[0].GetComment());
			NUnit.Framework.Assert.AreEqual(0, db.GetReflogReader("HEAD").GetReverseEntries()
				.Count);
			// make sure b's log file is gone too.
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "logs/refs/heads/b").Exists
				());
			// Create new Repository instance, to reread caches and make sure our
			// assumptions are persistent.
			Repository ndb = new FileRepository(db.Directory);
			NUnit.Framework.Assert.AreEqual(rb2, ndb.Resolve("refs/heads/new/name"));
			NUnit.Framework.Assert.IsNull(ndb.Resolve("refs/heads/b"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TryRenameWhenLocked(string toLock, string fromName, string toName
			, string headPointsTo)
		{
			// setup
			WriteSymref(Constants.HEAD, headPointsTo);
			ObjectId oldfromId = db.Resolve(fromName);
			ObjectId oldHeadId = db.Resolve(Constants.HEAD);
			WriteReflog(db, oldfromId, oldfromId, "Just a message", fromName);
			IList<ReflogEntry> oldFromLog = db.GetReflogReader(fromName).GetReverseEntries();
			IList<ReflogEntry> oldHeadLog = oldHeadId != null ? db.GetReflogReader(Constants.
				HEAD).GetReverseEntries() : null;
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "logs/" + fromName).Exists
				(), "internal check, we have a log");
			// "someone" has branch X locked
			LockFile lockFile = new LockFile(new FilePath(db.Directory, toLock), db.FileSystem
				);
			try
			{
				NUnit.Framework.Assert.IsTrue(lockFile.Lock());
				// Now this is our test
				RefRename renameRef = db.RenameRef(fromName, toName);
				RefUpdate.Result result = renameRef.Rename();
				NUnit.Framework.Assert.AreEqual(RefUpdate.Result.LOCK_FAILURE, result);
				// Check that the involved refs are the same despite the failure
				AssertExists(false, toName);
				if (!toLock.Equals(toName))
				{
					AssertExists(false, toName + ".lock");
				}
				AssertExists(true, toLock + ".lock");
				if (!toLock.Equals(fromName))
				{
					AssertExists(false, "logs/" + fromName + ".lock");
				}
				AssertExists(false, "logs/" + toName + ".lock");
				NUnit.Framework.Assert.AreEqual(oldHeadId, db.Resolve(Constants.HEAD));
				NUnit.Framework.Assert.AreEqual(oldfromId, db.Resolve(fromName));
				NUnit.Framework.Assert.IsNull(db.Resolve(toName));
				NUnit.Framework.Assert.AreEqual(oldFromLog.ToString(), db.GetReflogReader(fromName
					).GetReverseEntries().ToString());
				if (oldHeadId != null)
				{
					NUnit.Framework.Assert.AreEqual(oldHeadLog.ToString(), db.GetReflogReader(Constants
						.HEAD).GetReverseEntries().ToString());
				}
			}
			finally
			{
				lockFile.Unlock();
			}
		}

		private void AssertExists(bool positive, string toName)
		{
			NUnit.Framework.Assert.AreEqual(positive, new FilePath(db.Directory, toName).Exists
				(), toName + (positive ? " " : " does not ") + "exist");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameBranchCannotLockAFileHEADisFromLockHEAD()
		{
			TryRenameWhenLocked("HEAD", "refs/heads/b", "refs/heads/new/name", "refs/heads/b"
				);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameBranchCannotLockAFileHEADisFromLockFrom()
		{
			TryRenameWhenLocked("refs/heads/b", "refs/heads/b", "refs/heads/new/name", "refs/heads/b"
				);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameBranchCannotLockAFileHEADisFromLockTo()
		{
			TryRenameWhenLocked("refs/heads/new/name", "refs/heads/b", "refs/heads/new/name", 
				"refs/heads/b");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameBranchCannotLockAFileHEADisToLockFrom()
		{
			TryRenameWhenLocked("refs/heads/b", "refs/heads/b", "refs/heads/new/name", "refs/heads/new/name"
				);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameBranchCannotLockAFileHEADisToLockTo()
		{
			TryRenameWhenLocked("refs/heads/new/name", "refs/heads/b", "refs/heads/new/name", 
				"refs/heads/new/name");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameBranchCannotLockAFileHEADisOtherLockFrom()
		{
			TryRenameWhenLocked("refs/heads/b", "refs/heads/b", "refs/heads/new/name", "refs/heads/a"
				);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameBranchCannotLockAFileHEADisOtherLockTo()
		{
			TryRenameWhenLocked("refs/heads/new/name", "refs/heads/b", "refs/heads/new/name", 
				"refs/heads/a");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameRefNameColission1avoided()
		{
			// setup
			ObjectId rb = db.Resolve("refs/heads/b");
			WriteSymref(Constants.HEAD, "refs/heads/a");
			RefUpdate updateRef = db.UpdateRef("refs/heads/a");
			updateRef.SetNewObjectId(rb);
			updateRef.SetRefLogMessage("Setup", false);
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FAST_FORWARD, updateRef.Update()
				);
			ObjectId oldHead = db.Resolve(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(rb.Equals(oldHead));
			// assumption for this test
			WriteReflog(db, rb, rb, "Just a message", "refs/heads/a");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "logs/refs/heads/a").Exists
				(), "internal check, we have a log");
			// Now this is our test
			RefRename renameRef = db.RenameRef("refs/heads/a", "refs/heads/a/b");
			RefUpdate.Result result = renameRef.Rename();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.RENAMED, result);
			NUnit.Framework.Assert.IsNull(db.Resolve("refs/heads/a"));
			NUnit.Framework.Assert.AreEqual(rb, db.Resolve("refs/heads/a/b"));
			NUnit.Framework.Assert.AreEqual(3, db.GetReflogReader("a/b").GetReverseEntries().
				Count);
			NUnit.Framework.Assert.AreEqual("Branch: renamed a to a/b", db.GetReflogReader("a/b"
				).GetReverseEntries()[0].GetComment());
			NUnit.Framework.Assert.AreEqual("Just a message", db.GetReflogReader("a/b").GetReverseEntries
				()[1].GetComment());
			NUnit.Framework.Assert.AreEqual("Setup", db.GetReflogReader("a/b").GetReverseEntries
				()[2].GetComment());
			// same thing was logged to HEAD
			NUnit.Framework.Assert.AreEqual("Branch: renamed a to a/b", db.GetReflogReader("HEAD"
				).GetReverseEntries()[0].GetComment());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameRefNameColission2avoided()
		{
			// setup
			ObjectId rb = db.Resolve("refs/heads/b");
			WriteSymref(Constants.HEAD, "refs/heads/prefix/a");
			RefUpdate updateRef = db.UpdateRef("refs/heads/prefix/a");
			updateRef.SetNewObjectId(rb);
			updateRef.SetRefLogMessage("Setup", false);
			updateRef.SetForceUpdate(true);
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, updateRef.Update());
			ObjectId oldHead = db.Resolve(Constants.HEAD);
			NUnit.Framework.Assert.IsTrue(rb.Equals(oldHead));
			// assumption for this test
			WriteReflog(db, rb, rb, "Just a message", "refs/heads/prefix/a");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "logs/refs/heads/prefix/a"
				).Exists(), "internal check, we have a log");
			// Now this is our test
			RefRename renameRef = db.RenameRef("refs/heads/prefix/a", "refs/heads/prefix");
			RefUpdate.Result result = renameRef.Rename();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.RENAMED, result);
			NUnit.Framework.Assert.IsNull(db.Resolve("refs/heads/prefix/a"));
			NUnit.Framework.Assert.AreEqual(rb, db.Resolve("refs/heads/prefix"));
			NUnit.Framework.Assert.AreEqual(3, db.GetReflogReader("prefix").GetReverseEntries
				().Count);
			NUnit.Framework.Assert.AreEqual("Branch: renamed prefix/a to prefix", db.GetReflogReader
				("prefix").GetReverseEntries()[0].GetComment());
			NUnit.Framework.Assert.AreEqual("Just a message", db.GetReflogReader("prefix").GetReverseEntries
				()[1].GetComment());
			NUnit.Framework.Assert.AreEqual("Setup", db.GetReflogReader("prefix").GetReverseEntries
				()[2].GetComment());
			NUnit.Framework.Assert.AreEqual("Branch: renamed prefix/a to prefix", db.GetReflogReader
				("HEAD").GetReverseEntries()[0].GetComment());
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WriteReflog(Repository db, ObjectId oldId, ObjectId newId, string msg
			, string refName)
		{
			RefDirectory refs = (RefDirectory)db.RefDatabase;
			RefDirectoryUpdate update = ((RefDirectoryUpdate)refs.NewUpdate(refName, true));
			update.SetNewObjectId(newId);
			refs.Log(update, msg, true);
		}

		[System.Serializable]
		internal class SubclassedId : ObjectId
		{
			internal SubclassedId(AnyObjectId src) : base(src)
			{
			}
		}
	}
}
