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
using NGit;
using NGit.Dircache;
using NGit.Events;
using Sharpen;

namespace NGit.Dircache
{
	[NUnit.Framework.TestFixture]
	public class DirCacheBuilderTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBuildEmpty()
		{
			{
				DirCache dc = db.LockDirCache();
				DirCacheBuilder b = dc.Builder();
				NUnit.Framework.Assert.IsNotNull(b);
				b.Finish();
				dc.Write();
				NUnit.Framework.Assert.IsTrue(dc.Commit());
			}
			{
				DirCache dc = db.ReadDirCache();
				NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBuildRejectsUnsetFileMode()
		{
			DirCache dc = DirCache.NewInCore();
			DirCacheBuilder b = dc.Builder();
			NUnit.Framework.Assert.IsNotNull(b);
			DirCacheEntry e = new DirCacheEntry("a");
			NUnit.Framework.Assert.AreEqual(0, e.RawMode);
			try
			{
				b.Add(e);
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("FileMode not set for path a", err.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBuildOneFile_FinishWriteCommit()
		{
			string path = "a-file-path";
			FileMode mode = FileMode.REGULAR_FILE;
			long lastModified = 1218123387057L;
			int length = 1342;
			DirCacheEntry entOrig;
			{
				DirCache dc = db.LockDirCache();
				DirCacheBuilder b = dc.Builder();
				NUnit.Framework.Assert.IsNotNull(b);
				entOrig = new DirCacheEntry(path);
				entOrig.FileMode = mode;
				entOrig.LastModified = lastModified;
				entOrig.SetLength(length);
				NUnit.Framework.Assert.AreNotSame(path, entOrig.PathString);
				NUnit.Framework.Assert.AreEqual(path, entOrig.PathString);
				NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, entOrig.GetObjectId());
				NUnit.Framework.Assert.AreEqual(mode.GetBits(), entOrig.RawMode);
				NUnit.Framework.Assert.AreEqual(0, entOrig.Stage);
				NUnit.Framework.Assert.AreEqual(lastModified, entOrig.LastModified);
				NUnit.Framework.Assert.AreEqual(length, entOrig.Length);
				NUnit.Framework.Assert.IsFalse(entOrig.IsAssumeValid);
				b.Add(entOrig);
				b.Finish();
				NUnit.Framework.Assert.AreEqual(1, dc.GetEntryCount());
				NUnit.Framework.Assert.AreSame(entOrig, dc.GetEntry(0));
				dc.Write();
				NUnit.Framework.Assert.IsTrue(dc.Commit());
			}
			{
				DirCache dc = db.ReadDirCache();
				NUnit.Framework.Assert.AreEqual(1, dc.GetEntryCount());
				DirCacheEntry entRead = dc.GetEntry(0);
				NUnit.Framework.Assert.AreNotSame(entOrig, entRead);
				NUnit.Framework.Assert.AreEqual(path, entRead.PathString);
				NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, entOrig.GetObjectId());
				NUnit.Framework.Assert.AreEqual(mode.GetBits(), entOrig.RawMode);
				NUnit.Framework.Assert.AreEqual(0, entOrig.Stage);
				NUnit.Framework.Assert.AreEqual(lastModified, entOrig.LastModified);
				NUnit.Framework.Assert.AreEqual(length, entOrig.Length);
				NUnit.Framework.Assert.IsFalse(entOrig.IsAssumeValid);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBuildOneFile_Commit()
		{
			string path = "a-file-path";
			FileMode mode = FileMode.REGULAR_FILE;
			long lastModified = 1218123387057L;
			int length = 1342;
			DirCacheEntry entOrig;
			{
				DirCache dc = db.LockDirCache();
				DirCacheBuilder b = dc.Builder();
				NUnit.Framework.Assert.IsNotNull(b);
				entOrig = new DirCacheEntry(path);
				entOrig.FileMode = mode;
				entOrig.LastModified = lastModified;
				entOrig.SetLength(length);
				NUnit.Framework.Assert.AreNotSame(path, entOrig.PathString);
				NUnit.Framework.Assert.AreEqual(path, entOrig.PathString);
				NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, entOrig.GetObjectId());
				NUnit.Framework.Assert.AreEqual(mode.GetBits(), entOrig.RawMode);
				NUnit.Framework.Assert.AreEqual(0, entOrig.Stage);
				NUnit.Framework.Assert.AreEqual(lastModified, entOrig.LastModified);
				NUnit.Framework.Assert.AreEqual(length, entOrig.Length);
				NUnit.Framework.Assert.IsFalse(entOrig.IsAssumeValid);
				b.Add(entOrig);
				NUnit.Framework.Assert.IsTrue(b.Commit());
				NUnit.Framework.Assert.AreEqual(1, dc.GetEntryCount());
				NUnit.Framework.Assert.AreSame(entOrig, dc.GetEntry(0));
				NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "index.lock").Exists());
			}
			{
				DirCache dc = db.ReadDirCache();
				NUnit.Framework.Assert.AreEqual(1, dc.GetEntryCount());
				DirCacheEntry entRead = dc.GetEntry(0);
				NUnit.Framework.Assert.AreNotSame(entOrig, entRead);
				NUnit.Framework.Assert.AreEqual(path, entRead.PathString);
				NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, entOrig.GetObjectId());
				NUnit.Framework.Assert.AreEqual(mode.GetBits(), entOrig.RawMode);
				NUnit.Framework.Assert.AreEqual(0, entOrig.Stage);
				NUnit.Framework.Assert.AreEqual(lastModified, entOrig.LastModified);
				NUnit.Framework.Assert.AreEqual(length, entOrig.Length);
				NUnit.Framework.Assert.IsFalse(entOrig.IsAssumeValid);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBuildOneFile_Commit_IndexChangedEvent()
		{
			// empty
			string path = "a-file-path";
			FileMode mode = FileMode.REGULAR_FILE;
			// "old" date in 2008
			long lastModified = 1218123387057L;
			int length = 1342;
			DirCacheEntry entOrig;
			bool receivedEvent = false;
			DirCache dc = db.LockDirCache();
			IndexChangedListener listener = new _IndexChangedListener_212();
			ListenerList l = db.Listeners;
			l.AddIndexChangedListener(listener);
			DirCacheBuilder b = dc.Builder();
			entOrig = new DirCacheEntry(path);
			entOrig.FileMode = mode;
			entOrig.LastModified = lastModified;
			entOrig.SetLength(length);
			b.Add(entOrig);
			try
			{
				b.Commit();
			}
			catch (_T123327308)
			{
				receivedEvent = true;
			}
			if (!receivedEvent)
			{
				NUnit.Framework.Assert.Fail("did not receive IndexChangedEvent");
			}
			// do the same again, as this doesn't change index compared to first
			// round we should get no event this time
			dc = db.LockDirCache();
			listener = new _IndexChangedListener_239();
			l = db.Listeners;
			l.AddIndexChangedListener(listener);
			b = dc.Builder();
			entOrig = new DirCacheEntry(path);
			entOrig.FileMode = mode;
			entOrig.LastModified = lastModified;
			entOrig.SetLength(length);
			b.Add(entOrig);
			try
			{
				b.Commit();
			}
			catch (_T123327308)
			{
				NUnit.Framework.Assert.Fail("unexpected IndexChangedEvent");
			}
		}

		[System.Serializable]
		internal sealed class _T123327308 : RuntimeException
		{
			private const long serialVersionUID = 1L;

			internal _T123327308(DirCacheBuilderTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly DirCacheBuilderTest _enclosing;
		}

		private sealed class _IndexChangedListener_212 : IndexChangedListener
		{
			public _IndexChangedListener_212()
			{
			}

			public void OnIndexChanged(IndexChangedEvent @event)
			{
				throw new _T123327308(null);
			}
		}

		private sealed class _IndexChangedListener_239 : IndexChangedListener
		{
			public _IndexChangedListener_239()
			{
			}

			public void OnIndexChanged(IndexChangedEvent @event)
			{
				throw new _T123327308(null);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFindSingleFile()
		{
			string path = "a-file-path";
			DirCache dc = db.ReadDirCache();
			DirCacheBuilder b = dc.Builder();
			NUnit.Framework.Assert.IsNotNull(b);
			DirCacheEntry entOrig = new DirCacheEntry(path);
			entOrig.FileMode = FileMode.REGULAR_FILE;
			NUnit.Framework.Assert.AreNotSame(path, entOrig.PathString);
			NUnit.Framework.Assert.AreEqual(path, entOrig.PathString);
			b.Add(entOrig);
			b.Finish();
			NUnit.Framework.Assert.AreEqual(1, dc.GetEntryCount());
			NUnit.Framework.Assert.AreSame(entOrig, dc.GetEntry(0));
			NUnit.Framework.Assert.AreEqual(0, dc.FindEntry(path));
			NUnit.Framework.Assert.AreEqual(-1, dc.FindEntry("@@-before"));
			NUnit.Framework.Assert.AreEqual(0, Real(dc.FindEntry("@@-before")));
			NUnit.Framework.Assert.AreEqual(-2, dc.FindEntry("a-zoo"));
			NUnit.Framework.Assert.AreEqual(1, Real(dc.FindEntry("a-zoo")));
			NUnit.Framework.Assert.AreSame(entOrig, dc.GetEntry(path));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAdd_InGitSortOrder()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a.b", "a/b", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].FileMode = FileMode.REGULAR_FILE;
			}
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = 0; i_1 < ents.Length; i_1++)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			NUnit.Framework.Assert.AreEqual(paths.Length, dc.GetEntryCount());
			for (int i_2 = 0; i_2 < paths.Length; i_2++)
			{
				NUnit.Framework.Assert.AreSame(ents[i_2], dc.GetEntry(i_2));
				NUnit.Framework.Assert.AreEqual(paths[i_2], dc.GetEntry(i_2).PathString);
				NUnit.Framework.Assert.AreEqual(i_2, dc.FindEntry(paths[i_2]));
				NUnit.Framework.Assert.AreSame(ents[i_2], dc.GetEntry(paths[i_2]));
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAdd_ReverseGitSortOrder()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a.b", "a/b", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].FileMode = FileMode.REGULAR_FILE;
			}
			DirCacheBuilder b = dc.Builder();
			for (int i_1 = ents.Length - 1; i_1 >= 0; i_1--)
			{
				b.Add(ents[i_1]);
			}
			b.Finish();
			NUnit.Framework.Assert.AreEqual(paths.Length, dc.GetEntryCount());
			for (int i_2 = 0; i_2 < paths.Length; i_2++)
			{
				NUnit.Framework.Assert.AreSame(ents[i_2], dc.GetEntry(i_2));
				NUnit.Framework.Assert.AreEqual(paths[i_2], dc.GetEntry(i_2).PathString);
				NUnit.Framework.Assert.AreEqual(i_2, dc.FindEntry(paths[i_2]));
				NUnit.Framework.Assert.AreSame(ents[i_2], dc.GetEntry(paths[i_2]));
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBuilderClear()
		{
			DirCache dc = db.ReadDirCache();
			string[] paths = new string[] { "a.", "a.b", "a/b", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].FileMode = FileMode.REGULAR_FILE;
			}
			{
				DirCacheBuilder b = dc.Builder();
				for (int i_1 = 0; i_1 < ents.Length; i_1++)
				{
					b.Add(ents[i_1]);
				}
				b.Finish();
			}
			NUnit.Framework.Assert.AreEqual(paths.Length, dc.GetEntryCount());
			{
				DirCacheBuilder b = dc.Builder();
				b.Finish();
			}
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
		}

		private static int Real(int eIdx)
		{
			if (eIdx < 0)
			{
				eIdx = -(eIdx + 1);
			}
			return eIdx;
		}
	}
}
