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
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>
	/// Utility methods for
	/// <see cref="RevWalk">RevWalk</see>
	/// .
	/// </summary>
	public sealed class RevWalkUtils
	{
		public RevWalkUtils()
		{
		}

		// Utility class
		/// <summary>
		/// Count the number of commits that are reachable from <code>start</code>
		/// until a commit that is reachable from <code>end</code> is encountered.
		/// </summary>
		/// <remarks>
		/// Count the number of commits that are reachable from <code>start</code>
		/// until a commit that is reachable from <code>end</code> is encountered. In
		/// other words, count the number of commits that are in <code>start</code>,
		/// but not in <code>end</code>.
		/// <p>
		/// Note that this method calls
		/// <see cref="RevWalk.Reset()">RevWalk.Reset()</see>
		/// at the beginning.
		/// Also note that the existing rev filter on the walk is left as-is, so be
		/// sure to set the right rev filter before calling this method.
		/// </remarks>
		/// <param name="walk">the rev walk to use</param>
		/// <param name="start">the commit to start counting from</param>
		/// <param name="end">
		/// the commit where counting should end, or null if counting
		/// should be done until there are no more commits
		/// </param>
		/// <returns>the number of commits</returns>
		/// <exception cref="NGit.Errors.MissingObjectException">NGit.Errors.MissingObjectException
		/// 	</exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException">NGit.Errors.IncorrectObjectTypeException
		/// 	</exception>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public static int Count(RevWalk walk, RevCommit start, RevCommit end)
		{
			return Find(walk, start, end).Count;
		}

		/// <summary>
		/// Find commits that are reachable from <code>start</code> until a commit
		/// that is reachable from <code>end</code> is encountered.
		/// </summary>
		/// <remarks>
		/// Find commits that are reachable from <code>start</code> until a commit
		/// that is reachable from <code>end</code> is encountered. In other words,
		/// Find of commits that are in <code>start</code>, but not in
		/// <code>end</code>.
		/// <p>
		/// Note that this method calls
		/// <see cref="RevWalk.Reset()">RevWalk.Reset()</see>
		/// at the beginning.
		/// Also note that the existing rev filter on the walk is left as-is, so be
		/// sure to set the right rev filter before calling this method.
		/// </remarks>
		/// <param name="walk">the rev walk to use</param>
		/// <param name="start">the commit to start counting from</param>
		/// <param name="end">
		/// the commit where counting should end, or null if counting
		/// should be done until there are no more commits
		/// </param>
		/// <returns>the commits found</returns>
		/// <exception cref="NGit.Errors.MissingObjectException">NGit.Errors.MissingObjectException
		/// 	</exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException">NGit.Errors.IncorrectObjectTypeException
		/// 	</exception>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public static IList<RevCommit> Find(RevWalk walk, RevCommit start, RevCommit end)
		{
			walk.Reset();
			walk.MarkStart(start);
			if (end != null)
			{
				walk.MarkUninteresting(end);
			}
			IList<RevCommit> commits = new AList<RevCommit>();
			foreach (RevCommit c in walk)
			{
				commits.AddItem(c);
			}
			return commits;
		}

		/// <summary>
		/// Find the list of branches a given commit is reachable from when following
		/// parent.s
		/// <p>
		/// Note that this method calls
		/// <see cref="RevWalk.Reset()">RevWalk.Reset()</see>
		/// at the beginning.
		/// <p>
		/// In order to improve performance this method assumes clock skew among
		/// committers is never larger than 24 hours.
		/// </summary>
		/// <param name="commit">the commit we are looking at</param>
		/// <param name="revWalk">The RevWalk to be used.</param>
		/// <param name="refs">the set of branches we want to see reachability from</param>
		/// <returns>the list of branches a given commit is reachable from</returns>
		/// <exception cref="NGit.Errors.MissingObjectException">NGit.Errors.MissingObjectException
		/// 	</exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException">NGit.Errors.IncorrectObjectTypeException
		/// 	</exception>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public static IList<Ref> FindBranchesReachableFrom(RevCommit commit, RevWalk revWalk
			, ICollection<Ref> refs)
		{
			IList<Ref> result = new AList<Ref>();
			// searches from branches can be cut off early if any parent of the
			// search-for commit is found. This is quite likely, so optimize for this.
			revWalk.MarkStart(Arrays.AsList(commit.Parents));
			ObjectIdSubclassMap<ObjectId> cutOff = new ObjectIdSubclassMap<ObjectId>();
			int SKEW = 24 * 3600;
			// one day clock skew
			foreach (Ref @ref in refs)
			{
				RevObject maybehead = revWalk.ParseAny(@ref.GetObjectId());
				if (!(maybehead is RevCommit))
				{
					continue;
				}
				RevCommit headCommit = (RevCommit)maybehead;
				// if commit is in the ref branch, then the tip of ref should be
				// newer than the commit we are looking for. Allow for a large
				// clock skew.
				if (headCommit.CommitTime + SKEW < commit.CommitTime)
				{
					continue;
				}
				IList<ObjectId> maybeCutOff = new AList<ObjectId>(cutOff.Size());
				// guess rough size
				revWalk.ResetRetain();
				revWalk.MarkStart(headCommit);
				RevCommit current;
				Ref found = null;
				while ((current = revWalk.Next()) != null)
				{
					if (AnyObjectId.Equals(current, commit))
					{
						found = @ref;
						break;
					}
					if (cutOff.Contains(current))
					{
						break;
					}
					maybeCutOff.AddItem(current.ToObjectId());
				}
				if (found != null)
				{
					result.AddItem(@ref);
				}
				else
				{
					foreach (ObjectId id in maybeCutOff)
					{
						cutOff.AddIfAbsent(id);
					}
				}
			}
			return result;
		}
	}
}
