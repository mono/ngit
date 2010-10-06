using NGit;
using NGit.Api;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Transport;
using Sharpen;

namespace NGit.Api
{
	public class FetchCommandTest : RepositoryTestCase
	{
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestFetch()
		{
			// create other repository
			Repository db2 = CreateWorkRepository();
			Git git2 = new Git(db2);
			// setup the first repository to fetch from the second repository
			Config config = ((FileBasedConfig)db.GetConfig());
			RemoteConfig remoteConfig = new RemoteConfig(config, "test");
			URIish uri = new URIish(db2.Directory.ToURI().ToURL());
			remoteConfig.AddURI(uri);
			remoteConfig.Update(config);
			// create some refs via commits and tag
			RevCommit commit = git2.Commit().SetMessage("initial commit").Call();
			RevTag tag = git2.Tag().SetName("tag").Call();
			Git git1 = new Git(db);
			RefSpec spec = new RefSpec("refs/heads/master:refs/heads/x");
			git1.Fetch().SetRemote("test").SetRefSpecs(spec).Call();
			AssertEquals(commit.Id, db.Resolve(commit.Id.GetName() + "^{commit}"));
			AssertEquals(tag.Id, db.Resolve(tag.Id.GetName()));
		}
	}
}
