using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// Base class to support constructing a
	/// <see cref="Repository">Repository</see>
	/// .
	/// <p>
	/// Applications must set one of
	/// <see cref="BaseRepositoryBuilder{B, R}.SetGitDir(Sharpen.FilePath)">BaseRepositoryBuilder&lt;B, R&gt;.SetGitDir(Sharpen.FilePath)
	/// 	</see>
	/// or
	/// <see cref="BaseRepositoryBuilder{B, R}.SetWorkTree(Sharpen.FilePath)">BaseRepositoryBuilder&lt;B, R&gt;.SetWorkTree(Sharpen.FilePath)
	/// 	</see>
	/// , or use
	/// <see cref="BaseRepositoryBuilder{B, R}.ReadEnvironment()">BaseRepositoryBuilder&lt;B, R&gt;.ReadEnvironment()
	/// 	</see>
	/// or
	/// <see cref="BaseRepositoryBuilder{B, R}.FindGitDir()">BaseRepositoryBuilder&lt;B, R&gt;.FindGitDir()
	/// 	</see>
	/// in order to configure the minimum property set
	/// necessary to open a repository.
	/// <p>
	/// Single repository applications trying to be compatible with other Git
	/// implementations are encouraged to use a model such as:
	/// <pre>
	/// new RepositoryBuilder() //
	/// .setGitDir(gitDirArgument) // --git-dir if supplied, no-op if null
	/// .readEnviroment() // scan environment GIT_* variables
	/// .findGitDir() // scan up the file system tree
	/// .build()
	/// </pre>
	/// </summary>
	/// <seealso cref="NGit.Storage.File.FileRepositoryBuilder">NGit.Storage.File.FileRepositoryBuilder
	/// 	</seealso>
	public class RepositoryBuilder : BaseRepositoryBuilder<RepositoryBuilder, Repository
		>
	{
		// Empty implementation, everything is inherited.
	}
}
