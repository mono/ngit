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
using System.Text;
using NGit;
using NGit.Revwalk;
using NGit.Util;
using Sharpen;

namespace NGit.Merge
{
	/// <summary>Formatter for constructing the commit message for a squashed commit.</summary>
	/// <remarks>
	/// Formatter for constructing the commit message for a squashed commit.
	/// <p>
	/// The format should be the same as C Git does it, for compatibility.
	/// </remarks>
	public class SquashMessageFormatter
	{
		private GitDateFormatter dateFormatter;

		/// <summary>Create a new squash message formatter.</summary>
		/// <remarks>Create a new squash message formatter.</remarks>
		public SquashMessageFormatter()
		{
			dateFormatter = new GitDateFormatter(GitDateFormatter.Format.DEFAULT);
		}

		/// <summary>Construct the squashed commit message.</summary>
		/// <remarks>Construct the squashed commit message.</remarks>
		/// <param name="squashedCommits">the squashed commits</param>
		/// <param name="target">the target branch</param>
		/// <returns>squashed commit message</returns>
		public virtual string Format(IList<RevCommit> squashedCommits, Ref target)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Squashed commit of the following:\n");
			foreach (RevCommit c in squashedCommits)
			{
				sb.Append("\ncommit ");
				sb.Append(c.GetName());
				sb.Append("\n");
				sb.Append(ToString(c.GetAuthorIdent()));
				sb.Append("\n\t");
				sb.Append(c.GetShortMessage());
				sb.Append("\n");
			}
			return sb.ToString();
		}

		private string ToString(PersonIdent author)
		{
			StringBuilder a = new StringBuilder();
			a.Append("Author: ");
			a.Append(author.GetName());
			a.Append(" <");
			a.Append(author.GetEmailAddress());
			a.Append(">\n");
			a.Append("Date:   ");
			a.Append(dateFormatter.FormatDate(author));
			a.Append("\n");
			return a.ToString();
		}
	}
}
