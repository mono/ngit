using NGit;
using NGit.Storage.File;
using Sharpen;

namespace NGit
{
	public class ReflogConfigTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestlogAllRefUpdates()
		{
			long commitTime = 1154236443000L;
			int tz = -4 * 60;
			// check that there are no entries in the reflog and turn off writing
			// reflogs
			NUnit.Framework.Assert.AreEqual(0, db.GetReflogReader(Constants.HEAD).GetReverseEntries
				().Count);
			((FileBasedConfig)db.GetConfig()).SetBoolean("core", null, "logallrefupdates", false
				);
			// do one commit and check that reflog size is 0: no reflogs should be
			// written
			Tree t = new Tree(db);
			AddFileToTree(t, "i-am-a-file", "and this is the data in me\n");
			Commit(t, "A Commit\n", new PersonIdent(author, commitTime, tz), new PersonIdent(
				committer, commitTime, tz));
			commitTime += 100;
			NUnit.Framework.Assert.IsTrue("Reflog for HEAD still contain no entry", db.GetReflogReader
				(Constants.HEAD).GetReverseEntries().Count == 0);
			// set the logAllRefUpdates parameter to true and check it
			((FileBasedConfig)db.GetConfig()).SetBoolean("core", null, "logallrefupdates", true
				);
			NUnit.Framework.Assert.IsTrue(((FileBasedConfig)db.GetConfig()).Get(CoreConfig.KEY
				).IsLogAllRefUpdates());
			// do one commit and check that reflog size is increased to 1
			AddFileToTree(t, "i-am-another-file", "and this is other data in me\n");
			Commit(t, "A Commit\n", new PersonIdent(author, commitTime, tz), new PersonIdent(
				committer, commitTime, tz));
			commitTime += 100;
			NUnit.Framework.Assert.IsTrue("Reflog for HEAD should contain one entry", db.GetReflogReader
				(Constants.HEAD).GetReverseEntries().Count == 1);
			// set the logAllRefUpdates parameter to false and check it
			((FileBasedConfig)db.GetConfig()).SetBoolean("core", null, "logallrefupdates", false
				);
			NUnit.Framework.Assert.IsFalse(((FileBasedConfig)db.GetConfig()).Get(CoreConfig.KEY
				).IsLogAllRefUpdates());
			// do one commit and check that reflog size is 2
			AddFileToTree(t, "i-am-anotheranother-file", "and this is other other data in me\n"
				);
			Commit(t, "A Commit\n", new PersonIdent(author, commitTime, tz), new PersonIdent(
				committer, commitTime, tz));
			NUnit.Framework.Assert.IsTrue("Reflog for HEAD should contain two entries", db.GetReflogReader
				(Constants.HEAD).GetReverseEntries().Count == 2);
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
			try
			{
				inserter.Insert(commit);
				inserter.Flush();
			}
			finally
			{
				inserter.Release();
			}
			int nl = commitMsg.IndexOf('\n');
			RefUpdate ru = db.UpdateRef(Constants.HEAD);
			ru.SetNewObjectId(commit.CommitId);
			ru.SetRefLogMessage("commit : " + ((nl == -1) ? commitMsg : Sharpen.Runtime.Substring
				(commitMsg, 0, nl)), false);
			ru.ForceUpdate();
		}
	}
}
