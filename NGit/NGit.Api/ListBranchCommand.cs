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
using NGit.Api;
using NGit.Api.Errors;
using Sharpen;

namespace NGit.Api
{
	/// <summary>Used to obtain a list of branches.</summary>
	/// <remarks>Used to obtain a list of branches.</remarks>
	/// <seealso><a
	/// *      href="http://www.kernel.org/pub/software/scm/git/docs/git-branch.html"
	/// *      >Git documentation about Branch</a></seealso>
	public class ListBranchCommand : GitCommand<IList<Ref>>
	{
		private ListBranchCommand.ListMode listMode = ListMode.ALL;

		/// <summary>
		/// The modes available for listing branches (corresponding to the -r and -a
		/// options)
		/// </summary>
		public enum ListMode
		{
			ALL,
			REMOTE,
			HEAD
		}

		/// <param name="repo"></param>
		protected internal ListBranchCommand(Repository repo) : base(repo)
		{
		}

		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		public override IList<Ref> Call()
		{
			CheckCallable();
			IDictionary<string, Ref> refList;
			try
			{
				if (listMode == ListMode.HEAD)
				{
					refList = repo.RefDatabase.GetRefs(Constants.R_HEADS);
				}
				else
				{
					if (listMode == ListBranchCommand.ListMode.REMOTE)
					{
						refList = repo.RefDatabase.GetRefs(Constants.R_REMOTES);
					}
					else
					{
						refList = new Dictionary<string, Ref>(repo.RefDatabase.GetRefs(Constants.R_HEADS)
							);
						refList.PutAll(repo.RefDatabase.GetRefs(Constants.R_REMOTES));
					}
				}
			}
			catch (IOException e)
			{
				throw new JGitInternalException(e.Message, e);
			}
			IList<Ref> resultRefs = new AList<Ref>();
			Sharpen.Collections.AddAll(resultRefs, refList.Values);
			resultRefs.Sort(new _IComparer_111());
			SetCallable(false);
			return resultRefs;
		}

		private sealed class _IComparer_111 : IComparer<Ref>
		{
			public _IComparer_111()
			{
			}

			public int Compare(Ref o1, Ref o2)
			{
				return Sharpen.Runtime.CompareOrdinal(o1.GetName(), o2.GetName());
			}
		}

		/// <param name="listMode">
		/// optional: corresponds to the -r/-a options; by default, only
		/// local branches will be listed
		/// </param>
		/// <returns>this instance</returns>
		public virtual NGit.Api.ListBranchCommand SetListMode(ListBranchCommand.ListMode 
			listMode)
		{
			CheckCallable();
			this.listMode = listMode;
			return this;
		}
	}
}
