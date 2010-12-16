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
using NGit.Storage.File;
using Sharpen;

namespace NGit
{
	public class ReflogConfigTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestlogAllRefUpdates()
		{
			long commitTime = 1154236443000L;
			int tz = -4 * 60;
			// check that there are no entries in the reflog and turn off writing
			// reflogs
			NUnit.Framework.Assert.AreEqual(0, db.GetReflogReader(Constants.HEAD).GetReverseEntries
				().Count);
			FileBasedConfig cfg = ((FileBasedConfig)db.GetConfig());
			cfg.SetBoolean("core", null, "logallrefupdates", false);
			cfg.Save();
			// do one commit and check that reflog size is 0: no reflogs should be
			// written
			Tree t = new Tree(db);
			AddFileToTree(t, "i-am-a-file", "and this is the data in me\n");
			Commit(t, "A Commit\n", new PersonIdent(author, commitTime, tz), new PersonIdent(
				committer, commitTime, tz));
			commitTime += 100;
			NUnit.Framework.Assert.IsTrue(db.GetReflogReader(Constants.HEAD).GetReverseEntries
				().Count == 0, "Reflog for HEAD still contain no entry");
			// set the logAllRefUpdates parameter to true and check it
			cfg.SetBoolean("core", null, "logallrefupdates", true);
			cfg.Save();
			NUnit.Framework.Assert.IsTrue(cfg.Get(CoreConfig.KEY).IsLogAllRefUpdates());
			// do one commit and check that reflog size is increased to 1
			AddFileToTree(t, "i-am-another-file", "and this is other data in me\n");
			Commit(t, "A Commit\n", new PersonIdent(author, commitTime, tz), new PersonIdent(
				committer, commitTime, tz));
			commitTime += 100;
			NUnit.Framework.Assert.IsTrue(db.GetReflogReader(Constants.HEAD).GetReverseEntries
				().Count == 1, "Reflog for HEAD should contain one entry");
			// set the logAllRefUpdates parameter to false and check it
			cfg.SetBoolean("core", null, "logallrefupdates", false);
			cfg.Save();
			NUnit.Framework.Assert.IsFalse(cfg.Get(CoreConfig.KEY).IsLogAllRefUpdates());
			// do one commit and check that reflog size is 2
			AddFileToTree(t, "i-am-anotheranother-file", "and this is other other data in me\n"
				);
			Commit(t, "A Commit\n", new PersonIdent(author, commitTime, tz), new PersonIdent(
				committer, commitTime, tz));
			NUnit.Framework.Assert.IsTrue(db.GetReflogReader(Constants.HEAD).GetReverseEntries
				().Count == 2, "Reflog for HEAD should contain two entries");
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AddFileToTree(Tree t, string filename, string content)
		{
			FileTreeEntry f = t.AddFile(filename);
			WriteTrashFile(f.GetName(), content);
			t.Accept(new WriteTree(trash, db), TreeEntry.MODIFIED_ONLY);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Commit(Tree t, string commitMsg, PersonIdent author, PersonIdent committer
			)
		{
			NGit.CommitBuilder commit = new NGit.CommitBuilder();
			commit.Author = author;
			commit.Committer = committer;
			commit.Message = commitMsg;
			commit.TreeId = t.GetTreeId();
			ObjectInserter inserter = db.NewObjectInserter();
			ObjectId id;
			try
			{
				id = inserter.Insert(commit);
				inserter.Flush();
			}
			finally
			{
				inserter.Release();
			}
			int nl = commitMsg.IndexOf('\n');
			RefUpdate ru = db.UpdateRef(Constants.HEAD);
			ru.SetNewObjectId(id);
			ru.SetRefLogMessage("commit : " + ((nl == -1) ? commitMsg : Sharpen.Runtime.Substring
				(commitMsg, 0, nl)), false);
			ru.ForceUpdate();
		}
	}
}
