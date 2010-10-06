using NGit;
using NGit.Api;
using NGit.Errors;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Transport;
using Sharpen;

namespace NGit.Api
{
	public class PushCommandTest : RepositoryTestCase
	{
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestPush()
		{
			// create other repository
			Repository db2 = CreateWorkRepository();
			// setup the first repository
			Config config = ((FileBasedConfig)db.GetConfig());
			RemoteConfig remoteConfig = new RemoteConfig(config, "test");
			URIish uri = new URIish(db2.Directory.ToURI().ToURL());
			remoteConfig.AddURI(uri);
			remoteConfig.Update(config);
			Git git1 = new Git(db);
			// create some refs via commits and tag
			RevCommit commit = git1.Commit().SetMessage("initial commit").Call();
			RevTag tag = git1.Tag().SetName("tag").Call();
			try
			{
				db2.Resolve(commit.Id.GetName() + "^{commit}");
				NUnit.Framework.Assert.Fail("id shouldn't exist yet");
			}
			catch (MissingObjectException)
			{
			}
			// we should get here
			RefSpec spec = new RefSpec("refs/heads/master:refs/heads/x");
			git1.Push().SetRemote("test").SetRefSpecs(spec).Call();
			AssertEquals(commit.Id, db2.Resolve(commit.Id.GetName() + "^{commit}"));
			AssertEquals(tag.Id, db2.Resolve(tag.Id.GetName()));
		}
	}
}
