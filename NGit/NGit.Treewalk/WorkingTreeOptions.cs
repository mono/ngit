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
using Sharpen;

namespace NGit.Treewalk
{
	/// <summary>
	/// Options used by the
	/// <see cref="WorkingTreeIterator">WorkingTreeIterator</see>
	/// .
	/// </summary>
	public class WorkingTreeOptions
	{
		private sealed class _SectionParser_53 : Config.SectionParser<NGit.Treewalk.WorkingTreeOptions
			>
		{
			public _SectionParser_53()
			{
			}

			public NGit.Treewalk.WorkingTreeOptions Parse(Config cfg)
			{
				return new NGit.Treewalk.WorkingTreeOptions(cfg);
			}
		}

		/// <summary>
		/// Key for
		/// <see cref="NGit.Config.Get{T}(NGit.Config.SectionParser{T})">NGit.Config.Get&lt;T&gt;(NGit.Config.SectionParser&lt;T&gt;)
		/// 	</see>
		/// .
		/// </summary>
		public static readonly Config.SectionParser<NGit.Treewalk.WorkingTreeOptions> KEY
			 = new _SectionParser_53();

		private readonly bool fileMode;

		private readonly CoreConfig.AutoCRLF autoCRLF;

		private WorkingTreeOptions(Config rc)
		{
			fileMode = rc.GetBoolean(ConfigConstants.CONFIG_CORE_SECTION, ConfigConstants.CONFIG_KEY_FILEMODE
				, true);
			autoCRLF = rc.GetEnum(ConfigConstants.CONFIG_CORE_SECTION, null, ConfigConstants.
				CONFIG_KEY_AUTOCRLF, CoreConfig.AutoCRLF.FALSE);
		}

		/// <returns>true if the execute bit on working files should be trusted.</returns>
		public virtual bool IsFileMode()
		{
			return fileMode;
		}

		/// <returns>how automatic CRLF conversion has been configured.</returns>
		public virtual CoreConfig.AutoCRLF GetAutoCRLF()
		{
			return autoCRLF;
		}
	}
}
