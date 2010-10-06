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
using NGit.Events;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Events
{
	public class ConfigChangeEventTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestFileRepository_ChangeEventsOnlyOnSave()
		{
			ConfigChangedEvent[] events = new ConfigChangedEvent[1];
			db.Listeners.AddConfigChangedListener(new _ConfigChangedListener_52(events));
			FileBasedConfig config = ((FileBasedConfig)db.GetConfig());
			NUnit.Framework.Assert.IsNull(events[0]);
			// set a value to some arbitrary key
			config.SetString("test", "section", "event", "value");
			// no changes until we save
			NUnit.Framework.Assert.IsNull(events[0]);
			config.Save();
			NUnit.Framework.Assert.IsNotNull(events[0]);
			// correct repository?
			NUnit.Framework.Assert.AreEqual(events[0].GetRepository(), db);
			// reset for the next test
			events[0] = null;
			// unset the value we have just set above
			config.Unset("test", "section", "event");
			// no changes until we save
			NUnit.Framework.Assert.IsNull(events[0]);
			config.Save();
			NUnit.Framework.Assert.IsNotNull(events[0]);
			// correct repository?
			NUnit.Framework.Assert.AreEqual(events[0].GetRepository(), db);
		}

		private sealed class _ConfigChangedListener_52 : ConfigChangedListener
		{
			public _ConfigChangedListener_52(ConfigChangedEvent[] events)
			{
				this.events = events;
			}

			public void OnConfigChanged(ConfigChangedEvent @event)
			{
				events[0] = @event;
			}

			private readonly ConfigChangedEvent[] events;
		}
	}
}
