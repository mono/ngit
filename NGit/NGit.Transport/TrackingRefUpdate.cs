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

using System.Text;
using NGit;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Update of a locally stored tracking branch.</summary>
	/// <remarks>Update of a locally stored tracking branch.</remarks>
	public class TrackingRefUpdate
	{
		private readonly string remoteName;

		private readonly string localName;

		private bool forceUpdate;

		private ObjectId oldObjectId;

		private ObjectId newObjectId;

		private RefUpdate.Result result;

		internal TrackingRefUpdate(bool canForceUpdate, string remoteName, string localName
			, AnyObjectId oldValue, AnyObjectId newValue)
		{
			this.remoteName = remoteName;
			this.localName = localName;
			this.forceUpdate = canForceUpdate;
			this.oldObjectId = oldValue.Copy();
			this.newObjectId = newValue.Copy();
		}

		/// <summary>Get the name of the remote ref.</summary>
		/// <remarks>
		/// Get the name of the remote ref.
		/// <p>
		/// Usually this is of the form "refs/heads/master".
		/// </remarks>
		/// <returns>the name used within the remote repository.</returns>
		public virtual string GetRemoteName()
		{
			return remoteName;
		}

		/// <summary>Get the name of the local tracking ref.</summary>
		/// <remarks>
		/// Get the name of the local tracking ref.
		/// <p>
		/// Usually this is of the form "refs/remotes/origin/master".
		/// </remarks>
		/// <returns>the name used within this local repository.</returns>
		public virtual string GetLocalName()
		{
			return localName;
		}

		/// <summary>Get the new value the ref will be (or was) updated to.</summary>
		/// <remarks>Get the new value the ref will be (or was) updated to.</remarks>
		/// <returns>new value. Null if the caller has not configured it.</returns>
		public virtual ObjectId GetNewObjectId()
		{
			return newObjectId;
		}

		/// <summary>The old value of the ref, prior to the update being attempted.</summary>
		/// <remarks>
		/// The old value of the ref, prior to the update being attempted.
		/// <p>
		/// This value may differ before and after the update method. Initially it is
		/// populated with the value of the ref before the lock is taken, but the old
		/// value may change if someone else modified the ref between the time we
		/// last read it and when the ref was locked for update.
		/// </remarks>
		/// <returns>the value of the ref prior to the update being attempted.</returns>
		public virtual ObjectId GetOldObjectId()
		{
			return oldObjectId;
		}

		/// <summary>Get the status of this update.</summary>
		/// <remarks>Get the status of this update.</remarks>
		/// <returns>the status of the update.</returns>
		public virtual RefUpdate.Result GetResult()
		{
			return result;
		}

		internal virtual void SetResult(RefUpdate.Result result)
		{
			this.result = result;
		}

		internal virtual ReceiveCommand AsReceiveCommand()
		{
			return new TrackingRefUpdate.Command(this);
		}

		internal sealed class Command : ReceiveCommand
		{
			public Command(TrackingRefUpdate _enclosing) : base(_enclosing.oldObjectId, 
				_enclosing.newObjectId, _enclosing.localName)
			{
				this._enclosing = _enclosing;
			}

			internal bool CanForceUpdate()
			{
				return this._enclosing.forceUpdate;
			}

			public override void SetResult(RefUpdate.Result status)
			{
				this._enclosing.result = status;
				base.SetResult(status);
			}

			public override void SetResult(ReceiveCommand.Result status)
			{
				this._enclosing.result = this.Decode(status);
				base.SetResult(status);
			}

			public override void SetResult(ReceiveCommand.Result status, string msg)
			{
				this._enclosing.result = this.Decode(status);
				base.SetResult(status, msg);
			}

			private RefUpdate.Result Decode(ReceiveCommand.Result status)
			{
				switch (status)
				{
					case ReceiveCommand.Result.OK:
					{
						if (AnyObjectId.Equals(this._enclosing.oldObjectId, this._enclosing.newObjectId))
						{
							return RefUpdate.Result.NO_CHANGE;
						}
						switch (this.GetType())
						{
							case ReceiveCommand.Type.CREATE:
							{
								return RefUpdate.Result.NEW;
							}

							case ReceiveCommand.Type.UPDATE:
							{
								return RefUpdate.Result.FAST_FORWARD;
							}

							case ReceiveCommand.Type.DELETE:
							case ReceiveCommand.Type.UPDATE_NONFASTFORWARD:
							default:
							{
								return RefUpdate.Result.FORCED;
								break;
							}
						}
						goto case ReceiveCommand.Result.REJECTED_NOCREATE;
					}

					case ReceiveCommand.Result.REJECTED_NOCREATE:
					case ReceiveCommand.Result.REJECTED_NODELETE:
					case ReceiveCommand.Result.REJECTED_NONFASTFORWARD:
					{
						return RefUpdate.Result.REJECTED;
					}

					case ReceiveCommand.Result.REJECTED_CURRENT_BRANCH:
					{
						return RefUpdate.Result.REJECTED_CURRENT_BRANCH;
					}

					case ReceiveCommand.Result.REJECTED_MISSING_OBJECT:
					{
						return RefUpdate.Result.IO_FAILURE;
					}

					case ReceiveCommand.Result.LOCK_FAILURE:
					case ReceiveCommand.Result.NOT_ATTEMPTED:
					case ReceiveCommand.Result.REJECTED_OTHER_REASON:
					default:
					{
						return RefUpdate.Result.LOCK_FAILURE;
						break;
					}
				}
			}

			private readonly TrackingRefUpdate _enclosing;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("TrackingRefUpdate[");
			sb.Append(remoteName);
			sb.Append(" -> ");
			sb.Append(localName);
			if (forceUpdate)
			{
				sb.Append(" (forced)");
			}
			sb.Append(" ");
			sb.Append(oldObjectId == null ? string.Empty : oldObjectId.Abbreviate(7).Name);
			sb.Append("..");
			sb.Append(newObjectId == null ? string.Empty : newObjectId.Abbreviate(7).Name);
			sb.Append("]");
			return sb.ToString();
		}
	}
}
