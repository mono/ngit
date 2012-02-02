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
using NGit.Revwalk;
using Sharpen;

namespace NGit.Api
{
	/// <summary>Used to obtain a list of tags.</summary>
	/// <remarks>Used to obtain a list of tags.</remarks>
	/// <seealso><a href="http://www.kernel.org/pub/software/scm/git/docs/git-tag.html"
	/// *      >Git documentation about Tag</a></seealso>
	public class ListTagCommand : GitCommand<IList<RevTag>>
	{
		/// <param name="repo"></param>
		protected internal ListTagCommand(Repository repo) : base(repo)
		{
		}

		/// <exception cref="NGit.Api.Errors.JGitInternalException">upon internal failure</exception>
		/// <returns>the tags available</returns>
		public override IList<RevTag> Call()
		{
			CheckCallable();
			IDictionary<string, Ref> refList;
			IList<RevTag> tags = new AList<RevTag>();
			RevWalk revWalk = new RevWalk(repo);
			try
			{
				refList = repo.RefDatabase.GetRefs(Constants.R_TAGS);
				foreach (Ref @ref in refList.Values)
				{
					RevTag tag = revWalk.ParseTag(@ref.GetObjectId());
					tags.AddItem(tag);
				}
			}
			catch (IOException e)
			{
				throw new JGitInternalException(e.Message, e);
			}
			finally
			{
				revWalk.Release();
			}
			tags.Sort(new _IComparer_95());
			SetCallable(false);
			return tags;
		}

		private sealed class _IComparer_95 : IComparer<RevTag>
		{
			public _IComparer_95()
			{
			}

			public int Compare(RevTag o1, RevTag o2)
			{
				return Sharpen.Runtime.CompareOrdinal(o1.GetTagName(), o2.GetTagName());
			}
		}
	}
}
