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

using NGit.Api;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Receives a callback allowing type-specific configuration to be set
	/// on the Transport instance after it's been created.
	/// </summary>
	/// <remarks>
	/// Receives a callback allowing type-specific configuration to be set
	/// on the Transport instance after it's been created.
	/// <p>
	/// This allows consumers of the JGit command API to perform custom
	/// configuration that would be difficult anticipate and expose on the
	/// API command builders.
	/// <p>
	/// For instance, if a client needs to replace the SshSessionFactorys
	/// on any SSHTransport used (eg to control available SSH identities),
	/// they can set the TransportConfigCallback on the JGit API command -
	/// once the transport has been created by the command, the callback
	/// will be invoked and passed the transport instance, which the
	/// client can then inspect and configure as necessary.
	/// </remarks>
	public interface TransportConfigCallback
	{
		/// <summary>Add any additional transport-specific configuration required.</summary>
		/// <remarks>Add any additional transport-specific configuration required.</remarks>
		/// <param name="transport"></param>
		void Configure(NGit.Transport.Transport transport);
	}
}
