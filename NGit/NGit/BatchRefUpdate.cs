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
using NGit;
using NGit.Internal;
using NGit.Revwalk;
using NGit.Transport;
using Sharpen;

namespace NGit
{
	/// <summary>Batch of reference updates to be applied to a repository.</summary>
	/// <remarks>
	/// Batch of reference updates to be applied to a repository.
	/// <p>
	/// The batch update is primarily useful in the transport code, where a client or
	/// server is making changes to more than one reference at a time.
	/// </remarks>
	public class BatchRefUpdate
	{
		private readonly RefDatabase refdb;

		/// <summary>Commands to apply during this batch.</summary>
		/// <remarks>Commands to apply during this batch.</remarks>
		private readonly IList<ReceiveCommand> commands;

		/// <summary>Does the caller permit a forced update on a reference?</summary>
		private bool allowNonFastForwards;

		/// <summary>Identity to record action as within the reflog.</summary>
		/// <remarks>Identity to record action as within the reflog.</remarks>
		private PersonIdent refLogIdent;

		/// <summary>Message the caller wants included in the reflog.</summary>
		/// <remarks>Message the caller wants included in the reflog.</remarks>
		private string refLogMessage;

		/// <summary>
		/// Should the result value be appended to
		/// <see cref="refLogMessage">refLogMessage</see>
		/// .
		/// </summary>
		private bool refLogIncludeResult;

		/// <summary>Initialize a new batch update.</summary>
		/// <remarks>Initialize a new batch update.</remarks>
		/// <param name="refdb">the reference database of the repository to be updated.</param>
		protected internal BatchRefUpdate(RefDatabase refdb)
		{
			this.refdb = refdb;
			this.commands = new AList<ReceiveCommand>();
		}

		/// <returns>
		/// true if the batch update will permit a non-fast-forward update to
		/// an existing reference.
		/// </returns>
		public virtual bool IsAllowNonFastForwards()
		{
			return allowNonFastForwards;
		}

		/// <summary>Set if this update wants to permit a forced update.</summary>
		/// <remarks>Set if this update wants to permit a forced update.</remarks>
		/// <param name="allow">true if this update batch should ignore merge tests.</param>
		/// <returns>
		/// 
		/// <code>this</code>
		/// .
		/// </returns>
		public virtual NGit.BatchRefUpdate SetAllowNonFastForwards(bool allow)
		{
			allowNonFastForwards = allow;
			return this;
		}

		/// <returns>identity of the user making the change in the reflog.</returns>
		public virtual PersonIdent GetRefLogIdent()
		{
			return refLogIdent;
		}

		/// <summary>Set the identity of the user appearing in the reflog.</summary>
		/// <remarks>
		/// Set the identity of the user appearing in the reflog.
		/// <p>
		/// The timestamp portion of the identity is ignored. A new identity with the
		/// current timestamp will be created automatically when the update occurs
		/// and the log record is written.
		/// </remarks>
		/// <param name="pi">
		/// identity of the user. If null the identity will be
		/// automatically determined based on the repository
		/// configuration.
		/// </param>
		/// <returns>
		/// 
		/// <code>this</code>
		/// .
		/// </returns>
		public virtual NGit.BatchRefUpdate SetRefLogIdent(PersonIdent pi)
		{
			refLogIdent = pi;
			return this;
		}

		/// <summary>Get the message to include in the reflog.</summary>
		/// <remarks>Get the message to include in the reflog.</remarks>
		/// <returns>
		/// message the caller wants to include in the reflog; null if the
		/// update should not be logged.
		/// </returns>
		public virtual string GetRefLogMessage()
		{
			return refLogMessage;
		}

		/// <returns>
		/// 
		/// <code>true</code>
		/// if the ref log message should show the result.
		/// </returns>
		public virtual bool IsRefLogIncludingResult()
		{
			return refLogIncludeResult;
		}

		/// <summary>Set the message to include in the reflog.</summary>
		/// <remarks>Set the message to include in the reflog.</remarks>
		/// <param name="msg">
		/// the message to describe this change. It may be null if
		/// appendStatus is null in order not to append to the reflog
		/// </param>
		/// <param name="appendStatus">
		/// true if the status of the ref change (fast-forward or
		/// forced-update) should be appended to the user supplied
		/// message.
		/// </param>
		/// <returns>
		/// 
		/// <code>this</code>
		/// .
		/// </returns>
		public virtual NGit.BatchRefUpdate SetRefLogMessage(string msg, bool appendStatus
			)
		{
			if (msg == null && !appendStatus)
			{
				DisableRefLog();
			}
			else
			{
				if (msg == null && appendStatus)
				{
					refLogMessage = string.Empty;
					refLogIncludeResult = true;
				}
				else
				{
					refLogMessage = msg;
					refLogIncludeResult = appendStatus;
				}
			}
			return this;
		}

		/// <summary>Don't record this update in the ref's associated reflog.</summary>
		/// <remarks>Don't record this update in the ref's associated reflog.</remarks>
		/// <returns>
		/// 
		/// <code>this</code>
		/// .
		/// </returns>
		public virtual NGit.BatchRefUpdate DisableRefLog()
		{
			refLogMessage = null;
			refLogIncludeResult = false;
			return this;
		}

		/// <returns>
		/// true if log has been disabled by
		/// <see cref="DisableRefLog()">DisableRefLog()</see>
		/// .
		/// </returns>
		public virtual bool IsRefLogDisabled()
		{
			return refLogMessage == null;
		}

		/// <returns>commands this update will process.</returns>
		public virtual IList<ReceiveCommand> GetCommands()
		{
			return Sharpen.Collections.UnmodifiableList(commands);
		}

		/// <summary>Add a single command to this batch update.</summary>
		/// <remarks>Add a single command to this batch update.</remarks>
		/// <param name="cmd">the command to add, must not be null.</param>
		/// <returns>
		/// 
		/// <code>this</code>
		/// .
		/// </returns>
		public virtual NGit.BatchRefUpdate AddCommand(ReceiveCommand cmd)
		{
			commands.AddItem(cmd);
			return this;
		}

		/// <summary>Add commands to this batch update.</summary>
		/// <remarks>Add commands to this batch update.</remarks>
		/// <param name="cmd">the commands to add, must not be null.</param>
		/// <returns>
		/// 
		/// <code>this</code>
		/// .
		/// </returns>
		public virtual NGit.BatchRefUpdate AddCommand(params ReceiveCommand[] cmd)
		{
			return AddCommand(Arrays.AsList(cmd));
		}

		/// <summary>Add commands to this batch update.</summary>
		/// <remarks>Add commands to this batch update.</remarks>
		/// <param name="cmd">the commands to add, must not be null.</param>
		/// <returns>
		/// 
		/// <code>this</code>
		/// .
		/// </returns>
		public virtual NGit.BatchRefUpdate AddCommand(ICollection<ReceiveCommand> cmd)
		{
			Sharpen.Collections.AddAll(commands, cmd);
			return this;
		}

		/// <summary>Execute this batch update.</summary>
		/// <remarks>
		/// Execute this batch update.
		/// <p>
		/// The default implementation of this method performs a sequential reference
		/// update over each reference.
		/// </remarks>
		/// <param name="walk">
		/// a RevWalk to parse tags in case the storage system wants to
		/// store them pre-peeled, a common performance optimization.
		/// </param>
		/// <param name="update">progress monitor to receive update status on.</param>
		/// <exception cref="System.IO.IOException">
		/// the database is unable to accept the update. Individual
		/// command status must be tested to determine if there is a
		/// partial failure, or a total failure.
		/// </exception>
		public virtual void Execute(RevWalk walk, ProgressMonitor update)
		{
			update.BeginTask(JGitText.Get().updatingReferences, commands.Count);
			foreach (ReceiveCommand cmd in commands)
			{
				try
				{
					update.Update(1);
					if (cmd.GetResult() == ReceiveCommand.Result.NOT_ATTEMPTED)
					{
						cmd.UpdateType(walk);
						RefUpdate ru = NewUpdate(cmd);
						switch (cmd.GetType())
						{
							case ReceiveCommand.Type.DELETE:
							{
								cmd.SetResult(ru.Delete(walk));
								continue;
								goto case ReceiveCommand.Type.CREATE;
							}

							case ReceiveCommand.Type.CREATE:
							case ReceiveCommand.Type.UPDATE:
							case ReceiveCommand.Type.UPDATE_NONFASTFORWARD:
							{
								cmd.SetResult(ru.Update(walk));
								continue;
							}
						}
					}
				}
				catch (IOException err)
				{
					cmd.SetResult(ReceiveCommand.Result.REJECTED_OTHER_REASON, MessageFormat.Format(JGitText
						.Get().lockError, err.Message));
				}
			}
			update.EndTask();
		}

		/// <summary>Create a new RefUpdate copying the batch settings.</summary>
		/// <remarks>Create a new RefUpdate copying the batch settings.</remarks>
		/// <param name="cmd">specific command the update should be created to copy.</param>
		/// <returns>a single reference update command.</returns>
		/// <exception cref="System.IO.IOException">
		/// the reference database cannot make a new update object for
		/// the given reference.
		/// </exception>
		protected internal virtual RefUpdate NewUpdate(ReceiveCommand cmd)
		{
			RefUpdate ru = refdb.NewUpdate(cmd.GetRefName(), false);
			if (IsRefLogDisabled())
			{
				ru.DisableRefLog();
			}
			else
			{
				ru.SetRefLogIdent(refLogIdent);
				ru.SetRefLogMessage(refLogMessage, refLogIncludeResult);
			}
			switch (cmd.GetType())
			{
				case ReceiveCommand.Type.DELETE:
				{
					if (!ObjectId.ZeroId.Equals(cmd.GetOldId()))
					{
						ru.SetExpectedOldObjectId(cmd.GetOldId());
					}
					ru.SetForceUpdate(true);
					return ru;
				}

				case ReceiveCommand.Type.CREATE:
				case ReceiveCommand.Type.UPDATE:
				case ReceiveCommand.Type.UPDATE_NONFASTFORWARD:
				default:
				{
					ru.SetForceUpdate(IsAllowNonFastForwards());
					ru.SetExpectedOldObjectId(cmd.GetOldId());
					ru.SetNewObjectId(cmd.GetNewId());
					return ru;
					break;
				}
			}
		}
	}
}
