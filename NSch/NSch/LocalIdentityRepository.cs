/*
Copyright (c) 2006-2010 ymnk, JCraft,Inc. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

  1. Redistributions of source code must retain the above copyright notice,
     this list of conditions and the following disclaimer.

  2. Redistributions in binary form must reproduce the above copyright 
     notice, this list of conditions and the following disclaimer in 
     the documentation and/or other materials provided with the distribution.

  3. The names of the authors may not be used to endorse or promote products
     derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL JCRAFT,
INC. OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

This code is based on jsch (http://www.jcraft.com/jsch).
All credit should go to the authors of jsch.
*/

using System.Collections;
using NSch;
using Sharpen;

namespace NSch
{
	internal class LocalIdentityRepository : IdentityRepository
	{
		private ArrayList identities = new ArrayList();

		private JSch jsch;

		internal LocalIdentityRepository(JSch jsch)
		{
			this.jsch = jsch;
		}

		public virtual ArrayList GetIdentities()
		{
			lock (this)
			{
				ArrayList v = new ArrayList();
				for (int i = 0; i < identities.Count; i++)
				{
					v.Add(identities[i]);
				}
				return v;
			}
		}

		public virtual void Add(Identity identity)
		{
			lock (this)
			{
				if (!identities.Contains(identity))
				{
					identities.Add(identity);
				}
			}
		}

		public virtual bool Add(byte[] identity)
		{
			lock (this)
			{
				try
				{
					Identity _identity = IdentityFile.NewInstance("from remote:", identity, null, jsch
						);
					identities.Add(_identity);
					return true;
				}
				catch (JSchException)
				{
					return false;
				}
			}
		}

		public virtual bool Remove(byte[] blob)
		{
			lock (this)
			{
				if (blob == null)
				{
					return false;
				}
				for (int i = 0; i < identities.Count; i++)
				{
					Identity _identity = (Identity)(identities[i]);
					byte[] _blob = _identity.GetPublicKeyBlob();
					if (_blob == null || !Util.Array_equals(blob, _blob))
					{
						continue;
					}
					identities.RemoveElement(_identity);
					_identity.Clear();
					return true;
				}
				return false;
			}
		}

		public virtual void RemoveAll()
		{
			lock (this)
			{
				for (int i = 0; i < identities.Count; i++)
				{
					Identity identity = (Identity)(identities[i]);
					identity.Clear();
				}
				identities.Clear();
			}
		}
	}
}
