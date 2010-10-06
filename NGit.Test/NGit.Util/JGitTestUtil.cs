using System;
using Sharpen;

namespace NGit.Util
{
	public abstract class JGitTestUtil
	{
		public static readonly string CLASSPATH_TO_RESOURCES = "org/eclipse/jgit/test/resources/";

		public JGitTestUtil()
		{
			throw new NotSupportedException();
		}

		public static FilePath GetTestResourceFile(string fileName)
		{
			if (fileName == null || fileName.Length <= 0)
			{
				return null;
			}
			Uri url = Cl().GetResource(CLASSPATH_TO_RESOURCES + fileName);
			if (url == null)
			{
				// If URL is null then try to load it as it was being
				// loaded previously
				return new FilePath("tst", fileName);
			}
			try
			{
				return new FilePath(url.ToURI());
			}
			catch (URISyntaxException)
			{
				return new FilePath(url.AbsolutePath);
			}
		}

		private static ClassLoader Cl()
		{
			return typeof(NGit.Util.JGitTestUtil).GetClassLoader();
		}
	}
}
