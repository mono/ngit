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

using NGit;
using NGit.Submodule;
using Sharpen;

namespace NGit.Submodule
{
	/// <summary>Status class containing the type, path, and commit id of the submodule.</summary>
	/// <remarks>Status class containing the type, path, and commit id of the submodule.</remarks>
	public class SubmoduleStatus
	{
		private readonly SubmoduleStatusType type;

		private readonly string path;

		private readonly ObjectId indexId;

		private readonly ObjectId headId;

		/// <summary>Create submodule status</summary>
		/// <param name="type"></param>
		/// <param name="path"></param>
		/// <param name="indexId"></param>
		public SubmoduleStatus(SubmoduleStatusType type, string path, ObjectId indexId) : 
			this(type, path, indexId, null)
		{
		}

		/// <summary>Create submodule status</summary>
		/// <param name="type"></param>
		/// <param name="path"></param>
		/// <param name="indexId"></param>
		/// <param name="headId"></param>
		public SubmoduleStatus(SubmoduleStatusType type, string path, ObjectId indexId, ObjectId
			 headId)
		{
			this.type = type;
			this.path = path;
			this.indexId = indexId;
			this.headId = headId;
		}

		/// <returns>type</returns>
		public virtual SubmoduleStatusType GetType()
		{
			return type;
		}

		/// <returns>path</returns>
		public virtual string GetPath()
		{
			return path;
		}

		/// <returns>index object id</returns>
		public virtual ObjectId GetIndexId()
		{
			return indexId;
		}

		/// <returns>HEAD object id</returns>
		public virtual ObjectId GetHeadId()
		{
			return headId;
		}
	}
}
