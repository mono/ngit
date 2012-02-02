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
using NGit.Api;
using NGit.Util;
using Sharpen;

namespace NGit.Api
{
	/// <summary>Offers a "GitPorcelain"-like API to interact with a git repository.</summary>
	/// <remarks>
	/// Offers a "GitPorcelain"-like API to interact with a git repository.
	/// <p>
	/// The GitPorcelain commands are described in the &lt;a href="http://www.kernel.org/pub/software/scm/git/docs/git.html#_high_level_commands_porcelain"
	/// &gt;Git Documentation</a>.
	/// <p>
	/// This class only offers methods to construct so-called command classes. Each
	/// GitPorcelain command is represented by one command class.<br />
	/// Example: this class offers a
	/// <code>commit()</code>
	/// method returning an instance of
	/// the
	/// <code>CommitCommand</code>
	/// class. The
	/// <code>CommitCommand</code>
	/// class has setters
	/// for all the arguments and options. The
	/// <code>CommitCommand</code>
	/// class also has a
	/// <code>call</code>
	/// method to actually execute the commit. The following code show's
	/// how to do a simple commit:
	/// <pre>
	/// Git git = new Git(myRepo);
	/// git.commit().setMessage(&quot;Fix393&quot;).setAuthor(developerIdent).call();
	/// </pre>
	/// All mandatory parameters for commands have to be specified in the methods of
	/// this class, the optional parameters have to be specified by the
	/// setter-methods of the Command class.
	/// <p>
	/// This class is intended to be used internally (e.g. by JGit tests) or by
	/// external components (EGit, third-party tools) when they need exactly the
	/// functionality of a GitPorcelain command. There are use-cases where this class
	/// is not optimal and where you should use the more low-level JGit classes. The
	/// methods in this class may for example offer too much functionality or they
	/// offer the functionality with the wrong arguments.
	/// </remarks>
	public class Git
	{
		/// <summary>The git repository this class is interacting with</summary>
		private readonly Repository repo;

		/// <param name="dir">
		/// the repository to open. May be either the GIT_DIR, or the
		/// working tree directory that contains
		/// <code>.git</code>
		/// .
		/// </param>
		/// <returns>
		/// a
		/// <see cref="Git">Git</see>
		/// object for the existing git repository
		/// </returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public static NGit.Api.Git Open(FilePath dir)
		{
			return Open(dir, FS.DETECTED);
		}

		/// <param name="dir">
		/// the repository to open. May be either the GIT_DIR, or the
		/// working tree directory that contains
		/// <code>.git</code>
		/// .
		/// </param>
		/// <param name="fs">filesystem abstraction to use when accessing the repository.</param>
		/// <returns>
		/// a
		/// <see cref="Git">Git</see>
		/// object for the existing git repository
		/// </returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public static NGit.Api.Git Open(FilePath dir, FS fs)
		{
			RepositoryCache.FileKey key;
			key = RepositoryCache.FileKey.Lenient(dir, fs);
			return Wrap(new RepositoryBuilder().SetFS(fs).SetGitDir(key.GetFile()).SetMustExist
				(true).Build());
		}

		/// <param name="repo">
		/// the git repository this class is interacting with.
		/// <code>null</code>
		/// is not allowed
		/// </param>
		/// <returns>
		/// a
		/// <see cref="Git">Git</see>
		/// object for the existing git repository
		/// </returns>
		public static NGit.Api.Git Wrap(Repository repo)
		{
			return new NGit.Api.Git(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>clone</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-clone.html"
		/// *      >Git documentation about clone</a></seealso>
		/// <returns>
		/// a
		/// <see cref="CloneCommand">CloneCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>clone</code>
		/// command
		/// </returns>
		public static CloneCommand CloneRepository()
		{
			return new CloneCommand();
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>init</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-init.html"
		/// *      >Git documentation about init</a></seealso>
		/// <returns>
		/// a
		/// <see cref="InitCommand">InitCommand</see>
		/// used to collect all optional parameters and
		/// to finally execute the
		/// <code>init</code>
		/// command
		/// </returns>
		public static InitCommand Init()
		{
			return new InitCommand();
		}

		/// <summary>
		/// Constructs a new
		/// <see cref="Git">Git</see>
		/// object which can interact with the specified
		/// git repository. All command classes returned by methods of this class
		/// will always interact with this git repository.
		/// </summary>
		/// <param name="repo">
		/// the git repository this class is interacting with.
		/// <code>null</code>
		/// is not allowed
		/// </param>
		public Git(Repository repo)
		{
			if (repo == null)
			{
				throw new ArgumentNullException();
			}
			this.repo = repo;
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>Commit</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-commit.html"
		/// *      >Git documentation about Commit</a></seealso>
		/// <returns>
		/// a
		/// <see cref="CommitCommand">CommitCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>Commit</code>
		/// command
		/// </returns>
		public virtual CommitCommand Commit()
		{
			return new CommitCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>Log</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-log.html"
		/// *      >Git documentation about Log</a></seealso>
		/// <returns>
		/// a
		/// <see cref="LogCommand">LogCommand</see>
		/// used to collect all optional parameters and
		/// to finally execute the
		/// <code>Log</code>
		/// command
		/// </returns>
		public virtual LogCommand Log()
		{
			return new LogCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>Merge</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-merge.html"
		/// *      >Git documentation about Merge</a></seealso>
		/// <returns>
		/// a
		/// <see cref="MergeCommand">MergeCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>Merge</code>
		/// command
		/// </returns>
		public virtual MergeCommand Merge()
		{
			return new MergeCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>Pull</code>
		/// command
		/// </summary>
		/// <returns>
		/// a
		/// <see cref="PullCommand">PullCommand</see>
		/// </returns>
		public virtual PullCommand Pull()
		{
			return new PullCommand(repo);
		}

		/// <summary>Returns a command object used to create branches</summary>
		/// <returns>
		/// a
		/// <see cref="CreateBranchCommand">CreateBranchCommand</see>
		/// </returns>
		public virtual CreateBranchCommand BranchCreate()
		{
			return new CreateBranchCommand(repo);
		}

		/// <summary>Returns a command object used to delete branches</summary>
		/// <returns>
		/// a
		/// <see cref="DeleteBranchCommand">DeleteBranchCommand</see>
		/// </returns>
		public virtual DeleteBranchCommand BranchDelete()
		{
			return new DeleteBranchCommand(repo);
		}

		/// <summary>Returns a command object used to list branches</summary>
		/// <returns>
		/// a
		/// <see cref="ListBranchCommand">ListBranchCommand</see>
		/// </returns>
		public virtual ListBranchCommand BranchList()
		{
			return new ListBranchCommand(repo);
		}

		/// <summary>Returns a command object used to list tags</summary>
		/// <returns>
		/// a
		/// <see cref="ListTagCommand">ListTagCommand</see>
		/// </returns>
		public virtual ListTagCommand TagList()
		{
			return new ListTagCommand(repo);
		}

		/// <summary>Returns a command object used to rename branches</summary>
		/// <returns>
		/// a
		/// <see cref="RenameBranchCommand">RenameBranchCommand</see>
		/// </returns>
		public virtual RenameBranchCommand BranchRename()
		{
			return new RenameBranchCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>Add</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-add.html"
		/// *      >Git documentation about Add</a></seealso>
		/// <returns>
		/// a
		/// <see cref="AddCommand">AddCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>Add</code>
		/// command
		/// </returns>
		public virtual AddCommand Add()
		{
			return new AddCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>Tag</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-tag.html"
		/// *      >Git documentation about Tag</a></seealso>
		/// <returns>
		/// a
		/// <see cref="TagCommand">TagCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>Tag</code>
		/// command
		/// </returns>
		public virtual TagCommand Tag()
		{
			return new TagCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>Fetch</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-fetch.html"
		/// *      >Git documentation about Fetch</a></seealso>
		/// <returns>
		/// a
		/// <see cref="FetchCommand">FetchCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>Fetch</code>
		/// command
		/// </returns>
		public virtual FetchCommand Fetch()
		{
			return new FetchCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>Push</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-push.html"
		/// *      >Git documentation about Push</a></seealso>
		/// <returns>
		/// a
		/// <see cref="PushCommand">PushCommand</see>
		/// used to collect all optional parameters and
		/// to finally execute the
		/// <code>Push</code>
		/// command
		/// </returns>
		public virtual PushCommand Push()
		{
			return new PushCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>cherry-pick</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-cherry-pick.html"
		/// *      >Git documentation about cherry-pick</a></seealso>
		/// <returns>
		/// a
		/// <see cref="CherryPickCommand">CherryPickCommand</see>
		/// used to collect all optional
		/// parameters and to finally execute the
		/// <code>cherry-pick</code>
		/// command
		/// </returns>
		public virtual CherryPickCommand CherryPick()
		{
			return new CherryPickCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>revert</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-revert.html"
		/// *      >Git documentation about reverting changes</a></seealso>
		/// <returns>
		/// a
		/// <see cref="RevertCommand">RevertCommand</see>
		/// used to collect all optional
		/// parameters and to finally execute the
		/// <code>cherry-pick</code>
		/// command
		/// </returns>
		public virtual RevertCommand Revert()
		{
			return new RevertCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>Rebase</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-rebase.html"
		/// *      >Git documentation about rebase</a></seealso>
		/// <returns>
		/// a
		/// <see cref="RebaseCommand">RebaseCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>rebase</code>
		/// command
		/// </returns>
		public virtual RebaseCommand Rebase()
		{
			return new RebaseCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>rm</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-rm.html"
		/// *      >Git documentation about rm</a></seealso>
		/// <returns>
		/// a
		/// <see cref="RmCommand">RmCommand</see>
		/// used to collect all optional parameters and
		/// to finally execute the
		/// <code>rm</code>
		/// command
		/// </returns>
		public virtual RmCommand Rm()
		{
			return new RmCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>checkout</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-checkout.html"
		/// *      >Git documentation about checkout</a></seealso>
		/// <returns>
		/// a
		/// <see cref="CheckoutCommand">CheckoutCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>checkout</code>
		/// command
		/// </returns>
		public virtual CheckoutCommand Checkout()
		{
			return new CheckoutCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>reset</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-reset.html"
		/// *      >Git documentation about reset</a></seealso>
		/// <returns>
		/// a
		/// <see cref="ResetCommand">ResetCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>reset</code>
		/// command
		/// </returns>
		public virtual ResetCommand Reset()
		{
			return new ResetCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>status</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-status.html"
		/// *      >Git documentation about status</a></seealso>
		/// <returns>
		/// a
		/// <see cref="StatusCommand">StatusCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>status</code>
		/// command
		/// </returns>
		public virtual StatusCommand Status()
		{
			return new StatusCommand(repo);
		}

		/// <summary>Returns a command to add notes to an object</summary>
		/// <returns>
		/// a
		/// <see cref="AddNoteCommand">AddNoteCommand</see>
		/// </returns>
		public virtual AddNoteCommand NotesAdd()
		{
			return new AddNoteCommand(repo);
		}

		/// <summary>Returns a command to remove notes on an object</summary>
		/// <returns>
		/// a
		/// <see cref="RemoveNoteCommand">RemoveNoteCommand</see>
		/// </returns>
		public virtual RemoveNoteCommand NotesRemove()
		{
			return new RemoveNoteCommand(repo);
		}

		/// <summary>Returns a command to list all notes</summary>
		/// <returns>
		/// a
		/// <see cref="ListNotesCommand">ListNotesCommand</see>
		/// </returns>
		public virtual ListNotesCommand NotesList()
		{
			return new ListNotesCommand(repo);
		}

		/// <summary>Returns a command to show notes on an object</summary>
		/// <returns>
		/// a
		/// <see cref="ShowNoteCommand">ShowNoteCommand</see>
		/// </returns>
		public virtual ShowNoteCommand NotesShow()
		{
			return new ShowNoteCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>ls-remote</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-ls-remote.html"
		/// *      >Git documentation about ls-remote</a></seealso>
		/// <returns>
		/// a
		/// <see cref="LsRemoteCommand">LsRemoteCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>status</code>
		/// command
		/// </returns>
		public virtual LsRemoteCommand LsRemote()
		{
			return new LsRemoteCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>clean</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-clean.html"
		/// *      >Git documentation about Clean</a></seealso>
		/// <returns>
		/// a
		/// <see cref="CleanCommand">CleanCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>clean</code>
		/// command
		/// </returns>
		public virtual CleanCommand Clean()
		{
			return new CleanCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>blame</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-blame.html"
		/// *      >Git documentation about Blame</a></seealso>
		/// <returns>
		/// a
		/// <see cref="BlameCommand">BlameCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>blame</code>
		/// command
		/// </returns>
		public virtual BlameCommand Blame()
		{
			return new BlameCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>reflog</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-reflog.html"
		/// *      >Git documentation about reflog</a></seealso>
		/// <returns>
		/// a
		/// <see cref="ReflogCommand">ReflogCommand</see>
		/// used to collect all optional parameters
		/// and to finally execute the
		/// <code>reflog</code>
		/// command
		/// </returns>
		public virtual ReflogCommand Reflog()
		{
			return new ReflogCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>diff</code>
		/// command
		/// </summary>
		/// <seealso><a
		/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-diff.html"
		/// *      >Git documentation about diff</a></seealso>
		/// <returns>
		/// a
		/// <see cref="DiffCommand">DiffCommand</see>
		/// used to collect all optional parameters and
		/// to finally execute the
		/// <code>diff</code>
		/// command
		/// </returns>
		public virtual DiffCommand Diff()
		{
			return new DiffCommand(repo);
		}

		/// <summary>Returns a command object used to delete tags</summary>
		/// <returns>
		/// a
		/// <see cref="DeleteTagCommand">DeleteTagCommand</see>
		/// </returns>
		public virtual DeleteTagCommand TagDelete()
		{
			return new DeleteTagCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>submodule add</code>
		/// command
		/// </summary>
		/// <returns>
		/// a
		/// <see cref="SubmoduleAddCommand">SubmoduleAddCommand</see>
		/// used to add a new submodule to a
		/// parent repository
		/// </returns>
		public virtual SubmoduleAddCommand SubmoduleAdd()
		{
			return new SubmoduleAddCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>submodule init</code>
		/// command
		/// </summary>
		/// <returns>
		/// a
		/// <see cref="SubmoduleInitCommand">SubmoduleInitCommand</see>
		/// used to initialize the
		/// repository's config with settings from the .gitmodules file in
		/// the working tree
		/// </returns>
		public virtual SubmoduleInitCommand SubmoduleInit()
		{
			return new SubmoduleInitCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>submodule status</code>
		/// command
		/// </summary>
		/// <returns>
		/// a
		/// <see cref="SubmoduleStatusCommand">SubmoduleStatusCommand</see>
		/// used to report the status of a
		/// repository's configured submodules
		/// </returns>
		public virtual SubmoduleStatusCommand SubmoduleStatus()
		{
			return new SubmoduleStatusCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>submodule sync</code>
		/// command
		/// </summary>
		/// <returns>
		/// a
		/// <see cref="SubmoduleSyncCommand">SubmoduleSyncCommand</see>
		/// used to update the URL of a
		/// submodule from the parent repository's .gitmodules file
		/// </returns>
		public virtual SubmoduleSyncCommand SubmoduleSync()
		{
			return new SubmoduleSyncCommand(repo);
		}

		/// <summary>
		/// Returns a command object to execute a
		/// <code>submodule update</code>
		/// command
		/// </summary>
		/// <returns>
		/// a
		/// <see cref="SubmoduleUpdateCommand">SubmoduleUpdateCommand</see>
		/// used to update the submodules in
		/// a repository to the configured revision
		/// </returns>
		public virtual SubmoduleUpdateCommand SubmoduleUpdate()
		{
			return new SubmoduleUpdateCommand(repo);
		}

		/// <summary>Returns a command object used to list stashed commits</summary>
		/// <returns>
		/// a
		/// <see cref="StashListCommand">StashListCommand</see>
		/// </returns>
		public virtual StashListCommand StashList()
		{
			return new StashListCommand(repo);
		}

		/// <returns>the git repository this class is interacting with</returns>
		public virtual Repository GetRepository()
		{
			return repo;
		}
	}
}
