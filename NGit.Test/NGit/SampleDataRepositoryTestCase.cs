using NGit;
using NGit.Util;
using Sharpen;

namespace NGit
{
	/// <summary>Test case which includes C Git generated pack files for testing.</summary>
	/// <remarks>Test case which includes C Git generated pack files for testing.</remarks>
	public abstract class SampleDataRepositoryTestCase : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			string[] packs = new string[] { "pack-34be9032ac282b11fa9babdc2b2a93ca996c9c2f", 
				"pack-df2982f284bbabb6bdb59ee3fcc6eb0983e20371", "pack-9fb5b411fe6dfa89cc2e6b89d2bd8e5de02b5745"
				, "pack-546ff360fe3488adb20860ce3436a2d6373d2796", "pack-cbdeda40019ae0e6e789088ea0f51f164f489d14"
				, "pack-e6d07037cbcf13376308a0a995d1fa48f8f76aaa", "pack-3280af9c07ee18a87705ef50b0cc4cd20266cf12"
				 };
			FilePath packDir = new FilePath(db.ObjectDatabase.GetDirectory(), "pack");
			foreach (string n in packs)
			{
				CopyFile(JGitTestUtil.GetTestResourceFile(n + ".pack"), new FilePath(packDir, n +
					 ".pack"));
				CopyFile(JGitTestUtil.GetTestResourceFile(n + ".idx"), new FilePath(packDir, n + 
					".idx"));
			}
			CopyFile(JGitTestUtil.GetTestResourceFile("packed-refs"), new FilePath(db.Directory
				, "packed-refs"));
		}
	}
}
