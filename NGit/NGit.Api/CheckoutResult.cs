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
using NGit.Api;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Encapsulates the result of a
	/// <see cref="CheckoutCommand">CheckoutCommand</see>
	/// </summary>
	public class CheckoutResult
	{
		/// <summary>
		/// The
		/// <see cref="Status.ERROR">Status.ERROR</see>
		/// result;
		/// </summary>
		public static readonly NGit.Api.CheckoutResult ERROR_RESULT = new NGit.Api.CheckoutResult
			(CheckoutResult.Status.ERROR, null);

		/// <summary>
		/// The
		/// <see cref="Status.NOT_TRIED">Status.NOT_TRIED</see>
		/// result;
		/// </summary>
		public static readonly NGit.Api.CheckoutResult NOT_TRIED_RESULT = new NGit.Api.CheckoutResult
			(CheckoutResult.Status.NOT_TRIED, null);

		/// <summary>The status</summary>
		public enum Status
		{
			NOT_TRIED,
			OK,
			CONFLICTS,
			NONDELETED,
			ERROR
		}

		private readonly CheckoutResult.Status myStatus;

		private readonly IList<string> conflictList;

		private readonly IList<string> undeletedList;

		private readonly IList<string> modifiedList;

		private readonly IList<string> removedList;

		/// <summary>Create a new fail result.</summary>
		/// <remarks>
		/// Create a new fail result. If status is
		/// <see cref="Status.CONFLICTS">Status.CONFLICTS</see>
		/// ,
		/// <code>fileList</code> is a list of conflicting files, if status is
		/// <see cref="Status.NONDELETED">Status.NONDELETED</see>
		/// , <code>fileList</code> is a list of not deleted
		/// files. All other values ignore <code>fileList</code>. To create a result
		/// for
		/// <see cref="Status.OK">Status.OK</see>
		/// , see
		/// <see cref="CheckoutResult(System.Collections.Generic.IList{E}, System.Collections.Generic.IList{E})
		/// 	">CheckoutResult(System.Collections.Generic.IList&lt;E&gt;, System.Collections.Generic.IList&lt;E&gt;)
		/// 	</see>
		/// .
		/// </remarks>
		/// <param name="status">the failure status</param>
		/// <param name="fileList">
		/// the list of files to store, status has to be either
		/// <see cref="Status.CONFLICTS">Status.CONFLICTS</see>
		/// or
		/// <see cref="Status.NONDELETED">Status.NONDELETED</see>
		/// .
		/// </param>
		internal CheckoutResult(CheckoutResult.Status status, IList<string> fileList)
		{
			myStatus = status;
			if (status == CheckoutResult.Status.CONFLICTS)
			{
				this.conflictList = fileList;
			}
			else
			{
				this.conflictList = new AList<string>(0);
			}
			if (status == CheckoutResult.Status.NONDELETED)
			{
				this.undeletedList = fileList;
			}
			else
			{
				this.undeletedList = new AList<string>(0);
			}
			this.modifiedList = new AList<string>(0);
			this.removedList = new AList<string>(0);
		}

		/// <summary>Create a new OK result with modified and removed files.</summary>
		/// <remarks>Create a new OK result with modified and removed files.</remarks>
		/// <param name="modified">the modified files</param>
		/// <param name="removed">the removed files.</param>
		internal CheckoutResult(IList<string> modified, IList<string> removed)
		{
			myStatus = CheckoutResult.Status.OK;
			this.conflictList = new AList<string>(0);
			this.undeletedList = new AList<string>(0);
			this.modifiedList = modified;
			this.removedList = removed;
		}

		/// <returns>the status</returns>
		public virtual CheckoutResult.Status GetStatus()
		{
			return myStatus;
		}

		/// <returns>
		/// the list of files that created a checkout conflict, or an empty
		/// list if
		/// <see cref="GetStatus()">GetStatus()</see>
		/// is not
		/// <see cref="Status.CONFLICTS">Status.CONFLICTS</see>
		/// ;
		/// </returns>
		public virtual IList<string> GetConflictList()
		{
			return conflictList;
		}

		/// <returns>
		/// the list of files that could not be deleted during checkout, or
		/// an empty list if
		/// <see cref="GetStatus()">GetStatus()</see>
		/// is not
		/// <see cref="Status.NONDELETED">Status.NONDELETED</see>
		/// ;
		/// </returns>
		public virtual IList<string> GetUndeletedList()
		{
			return undeletedList;
		}

		/// <returns>
		/// the list of files that where modified during checkout, or an
		/// empty list if
		/// <see cref="GetStatus()">GetStatus()</see>
		/// is not
		/// <see cref="Status.OK">Status.OK</see>
		/// </returns>
		public virtual IList<string> GetModifiedList()
		{
			return modifiedList;
		}

		/// <returns>
		/// the list of files that where removed during checkout, or an empty
		/// list if
		/// <see cref="GetStatus()">GetStatus()</see>
		/// is not
		/// <see cref="Status.OK">Status.OK</see>
		/// </returns>
		public virtual IList<string> GetRemovedList()
		{
			return removedList;
		}
	}
}
