using NGit;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>
	/// Constructs a
	/// <see cref="FileRepository">FileRepository</see>
	/// .
	/// <p>
	/// Applications must set one of
	/// <see cref="NGit.BaseRepositoryBuilder{B, R}.SetGitDir(Sharpen.FilePath)">NGit.BaseRepositoryBuilder&lt;B, R&gt;.SetGitDir(Sharpen.FilePath)
	/// 	</see>
	/// or
	/// <see cref="NGit.BaseRepositoryBuilder{B, R}.SetWorkTree(Sharpen.FilePath)">NGit.BaseRepositoryBuilder&lt;B, R&gt;.SetWorkTree(Sharpen.FilePath)
	/// 	</see>
	/// , or use
	/// <see cref="NGit.BaseRepositoryBuilder{B, R}.ReadEnvironment()">NGit.BaseRepositoryBuilder&lt;B, R&gt;.ReadEnvironment()
	/// 	</see>
	/// or
	/// <see cref="NGit.BaseRepositoryBuilder{B, R}.FindGitDir()">NGit.BaseRepositoryBuilder&lt;B, R&gt;.FindGitDir()
	/// 	</see>
	/// in order to configure the minimum property set
	/// necessary to open a repository.
	/// <p>
	/// Single repository applications trying to be compatible with other Git
	/// implementations are encouraged to use a model such as:
	/// <pre>
	/// new FileRepositoryBuilder() //
	/// .setGitDir(gitDirArgument) // --git-dir if supplied, no-op if null
	/// .readEnviroment() // scan environment GIT_* variables
	/// .findGitDir() // scan up the file system tree
	/// .build()
	/// </pre>
	/// </summary>
	public class FileRepositoryBuilder : BaseRepositoryBuilder<FileRepositoryBuilder, 
		FileRepository>
	{
		/// <summary>Create a repository matching the configuration in this builder.</summary>
		/// <remarks>
		/// Create a repository matching the configuration in this builder.
		/// <p>
		/// If an option was not set, the build method will try to default the option
		/// based on other options. If insufficient information is available, an
		/// exception is thrown to the caller.
		/// </remarks>
		/// <returns>a repository matching this configuration.</returns>
		/// <exception cref="System.ArgumentException">insufficient parameters were set.</exception>
		/// <exception cref="System.IO.IOException">
		/// the repository could not be accessed to configure the rest of
		/// the builder's parameters.
		/// </exception>
		public override FileRepository Build()
		{
			return new FileRepository(Setup());
		}
	}
}
