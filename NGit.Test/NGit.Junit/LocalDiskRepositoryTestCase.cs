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
using NGit.Junit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Junit
{
	/// <summary>JUnit TestCase with specialized support for temporary local repository.</summary>
	/// <remarks>
	/// JUnit TestCase with specialized support for temporary local repository.
	/// <p>
	/// A temporary directory is created for each test, allowing each test to use a
	/// fresh environment. The temporary directory is cleaned up after the test ends.
	/// <p>
	/// Callers should not use
	/// <see cref="NGit.RepositoryCache">NGit.RepositoryCache</see>
	/// from within these tests as it
	/// may wedge file descriptors open past the end of the test.
	/// <p>
	/// A system property
	/// <code>jgit.junit.usemmap</code>
	/// defines whether memory mapping
	/// is used. Memory mapping has an effect on the file system, in that memory
	/// mapped files in Java cannot be deleted as long as the mapped arrays have not
	/// been reclaimed by the garbage collector. The programmer cannot control this
	/// with precision, so temporary files may hang around longer than desired during
	/// a test, or tests may fail altogether if there is insufficient file
	/// descriptors or address space for the test process.
	/// </remarks>
	public abstract class LocalDiskRepositoryTestCase
	{
		private static Sharpen.Thread shutdownHook;

		private static int testCount;

		private static readonly bool useMMAP = "true".Equals(Runtime.GetProperty("jgit.junit.usemmap"
			));

		/// <summary>A fake (but stable) identity for author fields in the test.</summary>
		/// <remarks>A fake (but stable) identity for author fields in the test.</remarks>
		protected internal PersonIdent author;

		/// <summary>A fake (but stable) identity for committer fields in the test.</summary>
		/// <remarks>A fake (but stable) identity for committer fields in the test.</remarks>
		protected internal PersonIdent committer;

		private readonly FilePath trash = new FilePath(new FilePath("target"), "trash");

		private readonly IList<Repository> toClose = new AList<Repository>();

		private MockSystemReader mockSystemReader;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			lock (this)
			{
				if (shutdownHook == null)
				{
					shutdownHook = new _Thread_118(this);
					// On windows accidentally open files or memory
					// mapped regions may prevent files from being deleted.
					// Suggesting a GC increases the likelihood that our
					// test repositories actually get removed after the
					// tests, even in the case of failure.
					Runtime.GetRuntime().AddShutdownHook(shutdownHook);
				}
			}
			RecursiveDelete(TestId(), trash, true, false);
			mockSystemReader = new MockSystemReader();
			mockSystemReader.userGitConfig = new FileBasedConfig(new FilePath(trash, "usergitconfig"
				), FS.DETECTED);
			CeilTestDirectories(GetCeilings());
			SystemReader.SetInstance(mockSystemReader);
			long now = mockSystemReader.GetCurrentTime();
			int tz = mockSystemReader.GetTimezone(now);
			author = new PersonIdent("J. Author", "jauthor@example.com");
			author = new PersonIdent(author, now, tz);
			committer = new PersonIdent("J. Committer", "jcommitter@example.com");
			committer = new PersonIdent(committer, now, tz);
			WindowCacheConfig c = new WindowCacheConfig();
			c.SetPackedGitLimit(128 * WindowCacheConfig.KB);
			c.SetPackedGitWindowSize(8 * WindowCacheConfig.KB);
			c.SetPackedGitMMAP(useMMAP);
			c.SetDeltaBaseCacheLimit(8 * WindowCacheConfig.KB);
			WindowCache.Reconfigure(c);
		}

		private sealed class _Thread_118 : Sharpen.Thread
		{
			public _Thread_118(LocalDiskRepositoryTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public override void Run()
			{
				System.GC.Collect();
				LocalDiskRepositoryTestCase.RecursiveDelete("SHUTDOWN", this._enclosing.trash, false
					, false);
			}

			private readonly LocalDiskRepositoryTestCase _enclosing;
		}

		protected internal virtual IList<FilePath> GetCeilings()
		{
			return Sharpen.Collections.SingletonList(trash.GetParentFile().GetAbsoluteFile());
		}

		private void CeilTestDirectories(IList<FilePath> ceilings)
		{
			mockSystemReader.SetProperty(Constants.GIT_CEILING_DIRECTORIES_KEY, MakePath(ceilings
				));
		}

		private string MakePath<_T0>(IList<_T0> objects)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object @object in objects)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(FilePath.pathSeparatorChar);
				}
				stringBuilder.Append(@object.ToString());
			}
			return stringBuilder.ToString();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.TearDown]
		public virtual void TearDown()
		{
			RepositoryCache.Clear();
			foreach (Repository r in toClose)
			{
				r.Close();
			}
			toClose.Clear();
			// Since memory mapping is controlled by the GC we need to
			// tell it this is a good time to clean up and unlock
			// memory mapped files.
			//
			if (useMMAP)
			{
				System.GC.Collect();
			}
			RecursiveDelete(TestId(), trash, false, true);
		}

		/// <summary>
		/// Increment the
		/// <see cref="author">author</see>
		/// and
		/// <see cref="committer">committer</see>
		/// times.
		/// </summary>
		protected internal virtual void Tick()
		{
			long delta = TimeUnit.MILLISECONDS.Convert(5 * 60, TimeUnit.SECONDS);
			long now = author.GetWhen().GetTime() + delta;
			int tz = mockSystemReader.GetTimezone(now);
			author = new PersonIdent(author, now, tz);
			committer = new PersonIdent(committer, now, tz);
		}

		/// <summary>Recursively delete a directory, failing the test if the delete fails.</summary>
		/// <remarks>Recursively delete a directory, failing the test if the delete fails.</remarks>
		/// <param name="dir">the recursively directory to delete, if present.</param>
		protected internal virtual void RecursiveDelete(FilePath dir)
		{
			RecursiveDelete(TestId(), dir, false, true);
		}

		private static bool RecursiveDelete(string testName, FilePath dir, bool silent, bool
			 failOnError)
		{
			if (!dir.Exists())
			{
				return silent;
			}
			FilePath[] ls = dir.ListFiles();
			if (ls != null)
			{
				for (int k = 0; k < ls.Length; k++)
				{
					FilePath e = ls[k];
					if (e.IsDirectory())
					{
						silent = RecursiveDelete(testName, e, silent, failOnError);
					}
					else
					{
						if (!e.Delete())
						{
							if (!silent)
							{
								ReportDeleteFailure(testName, failOnError, e);
							}
							silent = !failOnError;
						}
					}
				}
			}
			if (!dir.Delete())
			{
				if (!silent)
				{
					ReportDeleteFailure(testName, failOnError, dir);
				}
				silent = !failOnError;
			}
			return silent;
		}

		private static void ReportDeleteFailure(string testName, bool failOnError, FilePath
			 e)
		{
			string severity;
			if (failOnError)
			{
				severity = "ERROR";
			}
			else
			{
				severity = "WARNING";
			}
			string msg = severity + ": Failed to delete " + e + " in " + testName;
			if (failOnError)
			{
				NUnit.Framework.Assert.Fail(msg);
			}
			else
			{
				System.Console.Error.WriteLine(msg);
			}
		}

		/// <summary>Creates a new empty bare repository.</summary>
		/// <remarks>Creates a new empty bare repository.</remarks>
		/// <returns>the newly created repository, opened for access</returns>
		/// <exception cref="System.IO.IOException">the repository could not be created in the temporary area
		/// 	</exception>
		protected internal virtual FileRepository CreateBareRepository()
		{
			return CreateRepository(true);
		}

		/// <summary>Creates a new empty repository within a new empty working directory.</summary>
		/// <remarks>Creates a new empty repository within a new empty working directory.</remarks>
		/// <returns>the newly created repository, opened for access</returns>
		/// <exception cref="System.IO.IOException">the repository could not be created in the temporary area
		/// 	</exception>
		protected internal virtual FileRepository CreateWorkRepository()
		{
			return CreateRepository(false);
		}

		/// <summary>Creates a new empty repository.</summary>
		/// <remarks>Creates a new empty repository.</remarks>
		/// <param name="bare">
		/// true to create a bare repository; false to make a repository
		/// within its working directory
		/// </param>
		/// <returns>the newly created repository, opened for access</returns>
		/// <exception cref="System.IO.IOException">the repository could not be created in the temporary area
		/// 	</exception>
		private FileRepository CreateRepository(bool bare)
		{
			FilePath gitdir = CreateUniqueTestGitDir(bare);
			FileRepository db = new FileRepository(gitdir);
			NUnit.Framework.Assert.IsFalse(gitdir.Exists());
			db.Create();
			toClose.AddItem(db);
			return db;
		}

		/// <summary>
		/// Adds a repository to the list of repositories which is closed at the end
		/// of the tests
		/// </summary>
		/// <param name="r">the repository to be closed</param>
		public virtual void AddRepoToClose(Repository r)
		{
			toClose.AddItem(r);
		}

		private string CreateUniqueTestFolderPrefix()
		{
			return "test" + (Runtime.CurrentTimeMillis() + "_" + (testCount++));
		}

		/// <summary>Creates a unique directory for a test</summary>
		/// <param name="name">a subdirectory</param>
		/// <returns>a unique directory for a test</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		protected internal virtual FilePath CreateTempDirectory(string name)
		{
			string gitdirName = CreateUniqueTestFolderPrefix();
			FilePath parent = new FilePath(trash, gitdirName);
			FilePath directory = new FilePath(parent, name);
			FileUtils.Mkdirs(directory);
			return directory.GetCanonicalFile();
		}

		/// <summary>Creates a new unique directory for a test repository</summary>
		/// <param name="bare">
		/// true for a bare repository; false for a repository with a
		/// working directory
		/// </param>
		/// <returns>a unique directory for a test repository</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		protected internal virtual FilePath CreateUniqueTestGitDir(bool bare)
		{
			string gitdirName = CreateUniqueTestFolderPrefix();
			if (!bare)
			{
				gitdirName += "/";
			}
			gitdirName += Constants.DOT_GIT;
			FilePath gitdir = new FilePath(trash, gitdirName);
			return gitdir.GetCanonicalFile();
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal virtual FilePath CreateTempFile()
		{
			return new FilePath(trash, "tmp-" + System.Guid.NewGuid ().ToString ()).GetCanonicalFile();
		}

		/// <summary>Run a hook script in the repository, returning the exit status.</summary>
		/// <remarks>Run a hook script in the repository, returning the exit status.</remarks>
		/// <param name="db">repository the script should see in GIT_DIR environment</param>
		/// <param name="hook">
		/// path of the hook script to execute, must be executable file
		/// type on this platform
		/// </param>
		/// <param name="args">arguments to pass to the hook script</param>
		/// <returns>exit status code of the invoked hook</returns>
		/// <exception cref="System.IO.IOException">the hook could not be executed</exception>
		/// <exception cref="System.Exception">the caller was interrupted before the hook completed
		/// 	</exception>
		protected internal virtual int RunHook(Repository db, FilePath hook, params string
			[] args)
		{
			string[] argv = new string[1 + args.Length];
			argv[0] = hook.GetAbsolutePath();
			System.Array.Copy(args, 0, argv, 1, args.Length);
			IDictionary<string, string> env = CloneEnv();
			env.Put("GIT_DIR", db.Directory.GetAbsolutePath());
			PutPersonIdent(env, "AUTHOR", author);
			PutPersonIdent(env, "COMMITTER", committer);
			FilePath cwd = db.WorkTree;
			SystemProcess p = Runtime.GetRuntime().Exec(argv, ToEnvArray(env), cwd);
			p.GetOutputStream().Close();
			p.GetErrorStream().Close();
			p.GetInputStream().Close();
			return p.WaitFor();
		}

		private static void PutPersonIdent(IDictionary<string, string> env, string type, 
			PersonIdent who)
		{
			string ident = who.ToExternalString();
			string date = Sharpen.Runtime.Substring(ident, ident.IndexOf("> ") + 2);
			env.Put("GIT_" + type + "_NAME", who.GetName());
			env.Put("GIT_" + type + "_EMAIL", who.GetEmailAddress());
			env.Put("GIT_" + type + "_DATE", date);
		}

		/// <summary>Create a string to a UTF-8 temporary file and return the path.</summary>
		/// <remarks>Create a string to a UTF-8 temporary file and return the path.</remarks>
		/// <param name="body">
		/// complete content to write to the file. If the file should end
		/// with a trailing LF, the string should end with an LF.
		/// </param>
		/// <returns>path of the temporary file created within the trash area.</returns>
		/// <exception cref="System.IO.IOException">the file could not be written.</exception>
		protected internal virtual FilePath Write(string body)
		{
			FilePath f = FilePath.CreateTempFile("temp", "txt", trash);
			try
			{
				Write(f, body);
				return f;
			}
			catch (Error e)
			{
				f.Delete();
				throw;
			}
			catch (RuntimeException e)
			{
				f.Delete();
				throw;
			}
			catch (IOException e)
			{
				f.Delete();
				throw;
			}
		}

		/// <summary>Write a string as a UTF-8 file.</summary>
		/// <remarks>Write a string as a UTF-8 file.</remarks>
		/// <param name="f">
		/// file to write the string to. Caller is responsible for making
		/// sure it is in the trash directory or will otherwise be cleaned
		/// up at the end of the test. If the parent directory does not
		/// exist, the missing parent directories are automatically
		/// created.
		/// </param>
		/// <param name="body">content to write to the file.</param>
		/// <exception cref="System.IO.IOException">the file could not be written.</exception>
		protected internal virtual void Write(FilePath f, string body)
		{
			JGitTestUtil.Write(f, body);
		}

		/// <summary>Fully read a UTF-8 file and return as a string.</summary>
		/// <remarks>Fully read a UTF-8 file and return as a string.</remarks>
		/// <param name="f">file to read the content of.</param>
		/// <returns>
		/// UTF-8 decoded content of the file, empty string if the file
		/// exists but has no content.
		/// </returns>
		/// <exception cref="System.IO.IOException">the file does not exist, or could not be read.
		/// 	</exception>
		protected internal virtual string Read(FilePath f)
		{
			byte[] body = IOUtil.ReadFully(f);
			return Sharpen.Runtime.GetStringForBytes(body, 0, body.Length, "UTF-8");
		}

		private static string[] ToEnvArray(IDictionary<string, string> env)
		{
			string[] envp = new string[env.Count];
			int i = 0;
			foreach (KeyValuePair<string, string> e in env.EntrySet())
			{
				envp[i++] = e.Key + "=" + e.Value;
			}
			return envp;
		}

		private static Dictionary<string, string> CloneEnv()
		{
			return new Dictionary<string, string>(Sharpen.Runtime.GetEnv());
		}

		private string TestId()
		{
			return GetType().FullName + "." + testCount;
		}
	}
}
