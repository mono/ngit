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
using System.IO;
using System.Text;
using NGit;
using NGit.Dircache;
using NGit.Junit;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Treewalk;
using NGit.Util;
using Sharpen;

namespace NGit
{
	/// <summary>Base class for most JGit unit tests.</summary>
	/// <remarks>
	/// Base class for most JGit unit tests.
	/// Sets up a predefined test repository and has support for creating additional
	/// repositories and destroying them when the tests are finished.
	/// </remarks>
	public abstract class RepositoryTestCase : LocalDiskRepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		protected internal static void CopyFile(FilePath src, FilePath dst)
		{
			FileInputStream fis = new FileInputStream(src);
			try
			{
				FileOutputStream fos = new FileOutputStream(dst);
				try
				{
					byte[] buf = new byte[4096];
					int r;
					while ((r = fis.Read(buf)) > 0)
					{
						fos.Write(buf, 0, r);
					}
				}
				finally
				{
					fos.Close();
				}
			}
			finally
			{
				fis.Close();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal virtual FilePath WriteTrashFile(string name, string data)
		{
			return JGitTestUtil.WriteTrashFile(db, name, data);
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal virtual void DeleteTrashFile(string name)
		{
			JGitTestUtil.DeleteTrashFile(db, name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal static void CheckFile(FilePath f, string checkData)
		{
			StreamReader r = new InputStreamReader(new FileInputStream(f), "ISO-8859-1");
			try
			{
				char[] data = new char[(int)f.Length()];
				if (f.Length() != r.Read(data))
				{
					throw new IOException("Internal error reading file data from " + f);
				}
				NUnit.Framework.Assert.AreEqual(checkData, new string(data));
			}
			finally
			{
				r.Close();
			}
		}

		/// <summary>Test repository, initialized for this test case.</summary>
		/// <remarks>Test repository, initialized for this test case.</remarks>
		protected internal FileRepository db;

		/// <summary>
		/// Working directory of
		/// <see cref="db">db</see>
		/// .
		/// </summary>
		protected internal FilePath trash;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			db = CreateWorkRepository();
			trash = db.WorkTree;
		}

		public const int MOD_TIME = 1;

		public const int SMUDGE = 2;

		public const int LENGTH = 4;

		public const int CONTENT_ID = 8;

		public const int CONTENT = 16;

		public const int ASSUME_UNCHANGED = 32;

		/// <summary>Represent the state of the index in one String.</summary>
		/// <remarks>
		/// Represent the state of the index in one String. This representation is
		/// useful when writing tests which do assertions on the state of the index.
		/// By default information about path, mode, stage (if different from 0) is
		/// included. A bitmask controls which additional info about
		/// modificationTimes, smudge state and length is included.
		/// <p>
		/// The format of the returned string is described with this BNF:
		/// <pre>
		/// result = ( "[" path mode stage? time? smudge? length? sha1? content? "]" )* .
		/// mode = ", mode:" number .
		/// stage = ", stage:" number .
		/// time = ", time:t" timestamp-index .
		/// smudge = "" | ", smudged" .
		/// length = ", length:" number .
		/// sha1 = ", sha1:" hex-sha1 .
		/// content = ", content:" blob-data .
		/// </pre>
		/// 'stage' is only presented when the stage is different from 0. All
		/// reported time stamps are mapped to strings like "t0", "t1", ... "tn". The
		/// smallest reported time-stamp will be called "t0". This allows to write
		/// assertions against the string although the concrete value of the time
		/// stamps is unknown.
		/// </remarks>
		/// <param name="repo">the repository the index state should be determined for</param>
		/// <param name="includedOptions">
		/// a bitmask constructed out of the constants
		/// <see cref="MOD_TIME">MOD_TIME</see>
		/// ,
		/// <see cref="SMUDGE">SMUDGE</see>
		/// ,
		/// <see cref="LENGTH">LENGTH</see>
		/// ,
		/// <see cref="CONTENT_ID">CONTENT_ID</see>
		/// and
		/// <see cref="CONTENT">CONTENT</see>
		/// controlling which info is present in the
		/// resulting string.
		/// </param>
		/// <returns>a string encoding the index state</returns>
		/// <exception cref="System.InvalidOperationException">System.InvalidOperationException
		/// 	</exception>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual string IndexState(Repository repo, int includedOptions)
		{
			DirCache dc = repo.ReadDirCache();
			StringBuilder sb = new StringBuilder();
			TreeSet<long> timeStamps = null;
			// iterate once over the dircache just to collect all time stamps
			if (0 != (includedOptions & MOD_TIME))
			{
				timeStamps = new TreeSet<long>();
				for (int i = 0; i < dc.GetEntryCount(); ++i)
				{
					timeStamps.AddItem(Sharpen.Extensions.ValueOf(dc.GetEntry(i).LastModified));
				}
			}
			// iterate again, now produce the result string
			for (int i_1 = 0; i_1 < dc.GetEntryCount(); ++i_1)
			{
				DirCacheEntry entry = dc.GetEntry(i_1);
				sb.Append("[" + entry.PathString + ", mode:" + entry.FileMode);
				int stage = entry.Stage;
				if (stage != 0)
				{
					sb.Append(", stage:" + stage);
				}
				if (0 != (includedOptions & MOD_TIME))
				{
					sb.Append(", time:t" + timeStamps.HeadSet(Sharpen.Extensions.ValueOf(entry.LastModified
						)).Count);
				}
				if (0 != (includedOptions & SMUDGE))
				{
					if (entry.IsSmudged)
					{
						sb.Append(", smudged");
					}
				}
				if (0 != (includedOptions & LENGTH))
				{
					sb.Append(", length:" + Sharpen.Extensions.ToString(entry.Length));
				}
				if (0 != (includedOptions & CONTENT_ID))
				{
					sb.Append(", sha1:" + ObjectId.ToString(entry.GetObjectId()));
				}
				if (0 != (includedOptions & CONTENT))
				{
					sb.Append(", content:" + Sharpen.Runtime.GetStringForBytes(db.Open(entry.GetObjectId
						(), Constants.OBJ_BLOB).GetCachedBytes(), "UTF-8"));
				}
				if (0 != (includedOptions & ASSUME_UNCHANGED))
				{
					sb.Append(", assume-unchanged:" + entry.IsAssumeValid.ToString().ToLower());
				}
				sb.Append("]");
			}
			return sb.ToString();
		}

		/// <summary>Represent the state of the index in one String.</summary>
		/// <remarks>
		/// Represent the state of the index in one String. This representation is
		/// useful when writing tests which do assertions on the state of the index.
		/// By default information about path, mode, stage (if different from 0) is
		/// included. A bitmask controls which additional info about
		/// modificationTimes, smudge state and length is included.
		/// <p>
		/// The format of the returned string is described with this BNF:
		/// <pre>
		/// result = ( "[" path mode stage? time? smudge? length? sha1? content? "]" )* .
		/// mode = ", mode:" number .
		/// stage = ", stage:" number .
		/// time = ", time:t" timestamp-index .
		/// smudge = "" | ", smudged" .
		/// length = ", length:" number .
		/// sha1 = ", sha1:" hex-sha1 .
		/// content = ", content:" blob-data .
		/// </pre>
		/// 'stage' is only presented when the stage is different from 0. All
		/// reported time stamps are mapped to strings like "t0", "t1", ... "tn". The
		/// smallest reported time-stamp will be called "t0". This allows to write
		/// assertions against the string although the concrete value of the time
		/// stamps is unknown.
		/// </remarks>
		/// <param name="includedOptions">
		/// a bitmask constructed out of the constants
		/// <see cref="MOD_TIME">MOD_TIME</see>
		/// ,
		/// <see cref="SMUDGE">SMUDGE</see>
		/// ,
		/// <see cref="LENGTH">LENGTH</see>
		/// ,
		/// <see cref="CONTENT_ID">CONTENT_ID</see>
		/// and
		/// <see cref="CONTENT">CONTENT</see>
		/// controlling which info is present in the
		/// resulting string.
		/// </param>
		/// <returns>a string encoding the index state</returns>
		/// <exception cref="System.InvalidOperationException">System.InvalidOperationException
		/// 	</exception>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual string IndexState(int includedOptions)
		{
			return IndexState(db, includedOptions);
		}

		/// <summary>Resets the index to represent exactly some filesystem content.</summary>
		/// <remarks>
		/// Resets the index to represent exactly some filesystem content. E.g. the
		/// following call will replace the index with the working tree content:
		/// <p>
		/// <code>resetIndex(new FileSystemIterator(db))</code>
		/// <p>
		/// This method can be used by testcases which first prepare a new commit
		/// somewhere in the filesystem (e.g. in the working-tree) and then want to
		/// have an index which matches their prepared content.
		/// </remarks>
		/// <param name="treeItr">
		/// a
		/// <see cref="NGit.Treewalk.FileTreeIterator">NGit.Treewalk.FileTreeIterator</see>
		/// which determines which files should
		/// go into the new index
		/// </param>
		/// <exception cref="System.IO.FileNotFoundException">System.IO.FileNotFoundException
		/// 	</exception>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		protected internal virtual void ResetIndex(FileTreeIterator treeItr)
		{
			ObjectInserter inserter = db.NewObjectInserter();
			DirCacheBuilder builder = db.LockDirCache().Builder();
			DirCacheEntry dce;
			while (!treeItr.Eof)
			{
				long len = treeItr.GetEntryLength();
				dce = new DirCacheEntry(treeItr.EntryPathString);
				dce.FileMode = treeItr.EntryFileMode;
				dce.LastModified = treeItr.GetEntryLastModified();
				dce.SetLength((int)len);
				FileInputStream @in = new FileInputStream(treeItr.GetEntryFile());
				dce.SetObjectId(inserter.Insert(Constants.OBJ_BLOB, len, @in));
				@in.Close();
				builder.Add(dce);
				treeItr.Next(1);
			}
			builder.Commit();
			inserter.Flush();
			inserter.Release();
		}

		/// <summary>Helper method to map arbitrary objects to user-defined names.</summary>
		/// <remarks>
		/// Helper method to map arbitrary objects to user-defined names. This can be
		/// used create short names for objects to produce small and stable debug
		/// output. It is guaranteed that when you lookup the same object multiple
		/// times even with different nameTemplates this method will always return
		/// the same name which was derived from the first nameTemplate.
		/// nameTemplates can contain "%n" which will be replaced by a running number
		/// before used as a name.
		/// </remarks>
		/// <param name="l">the object to lookup</param>
		/// <param name="nameTemplate">
		/// the name for that object. Can contain "%n" which will be
		/// replaced by a running number before used as a name. If the
		/// lookup table already contains the object this parameter will
		/// be ignored
		/// </param>
		/// <param name="lookupTable">a table storing object-name mappings.</param>
		/// <returns>
		/// a name of that object. Is not guaranteed to be unique. Use
		/// nameTemplates containing "%n" to always have unique names
		/// </returns>
		public static string Lookup(object l, string nameTemplate, IDictionary<object, string
			> lookupTable)
		{
			string name = lookupTable.Get(l);
			if (name == null)
			{
				name = nameTemplate.ReplaceAll("%n", Sharpen.Extensions.ToString(lookupTable.Count
					));
				lookupTable.Put(l, name);
			}
			return name;
		}

		/// <summary>
		/// Waits until it is guaranteed that a subsequent file modification has a
		/// younger modification timestamp than the modification timestamp of the
		/// given file.
		/// </summary>
		/// <remarks>
		/// Waits until it is guaranteed that a subsequent file modification has a
		/// younger modification timestamp than the modification timestamp of the
		/// given file. This is done by touching a temporary file, reading the
		/// lastmodified attribute and, if needed, sleeping. After sleeping this loop
		/// starts again until the filesystem timer has advanced enough.
		/// </remarks>
		/// <param name="lastFile">
		/// the file on which we want to wait until the filesystem timer
		/// has advanced more than the lastmodification timestamp of this
		/// file
		/// </param>
		/// <returns>
		/// return the last measured value of the filesystem timer which is
		/// greater than then the lastmodification time of lastfile.
		/// </returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public static long FsTick(FilePath lastFile)
		{
			long sleepTime = 1;
			FilePath tmp = FilePath.CreateTempFile("FileTreeIteratorWithTimeControl", null);
			try
			{
				long startTime = (lastFile == null) ? tmp.LastModified() : lastFile.LastModified(
					);
				long actTime = tmp.LastModified();
				while (actTime <= startTime)
				{
					Sharpen.Thread.Sleep(sleepTime);
					sleepTime *= 5;
					tmp.SetLastModified(Runtime.CurrentTimeMillis());
					actTime = tmp.LastModified();
				}
				return actTime;
			}
			finally
			{
				FileUtils.Delete(tmp);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal virtual void CreateBranch(ObjectId objectId, string branchName
			)
		{
			RefUpdate updateRef = db.UpdateRef(branchName);
			updateRef.SetNewObjectId(objectId);
			updateRef.Update();
		}

		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		protected internal virtual void CheckoutBranch(string branchName)
		{
			RevWalk walk = new RevWalk(db);
			RevCommit head = walk.ParseCommit(db.Resolve(Constants.HEAD));
			RevCommit branch = walk.ParseCommit(db.Resolve(branchName));
			DirCacheCheckout dco = new DirCacheCheckout(db, head.Tree.Id, db.LockDirCache(), 
				branch.Tree.Id);
			dco.SetFailOnConflict(true);
			dco.Checkout();
			walk.Release();
			// update the HEAD
			RefUpdate refUpdate = db.UpdateRef(Constants.HEAD);
			refUpdate.Link(branchName);
		}

		/// <summary>Writes a number of files in the working tree.</summary>
		/// <remarks>
		/// Writes a number of files in the working tree. The first content specified
		/// will be written into a file named '0', the second into a file named "1"
		/// and so on. If <code>null</code> is specified as content then this file is
		/// skipped.
		/// </remarks>
		/// <param name="ensureDistinctTimestamps">
		/// if set to <code>true</code> then between two write operations
		/// this method will wait to ensure that the second file will get
		/// a different lastmodification timestamp than the first file.
		/// </param>
		/// <param name="contents">the contents which should be written into the files</param>
		/// <returns>the File object associated to the last written file.</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		/// <exception cref="System.Exception">System.Exception</exception>
		protected internal virtual FilePath WriteTrashFiles(bool ensureDistinctTimestamps
			, params string[] contents)
		{
			FilePath f = null;
			for (int i = 0; i < contents.Length; i++)
			{
				if (contents[i] != null)
				{
					if (ensureDistinctTimestamps && (f != null))
					{
						FsTick(f);
					}
					f = WriteTrashFile(Sharpen.Extensions.ToString(i), contents[i]);
				}
			}
			return f;
		}
	}
}
