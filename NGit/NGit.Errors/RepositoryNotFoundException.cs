using System;
using NGit;
using NGit.Errors;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Indicates a local repository does not exist.</summary>
	/// <remarks>Indicates a local repository does not exist.</remarks>
	[System.Serializable]
	public class RepositoryNotFoundException : TransportException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Constructs an exception indicating a local repository does not exist.</summary>
		/// <remarks>Constructs an exception indicating a local repository does not exist.</remarks>
		/// <param name="location">description of the repository not found, usually file path.
		/// 	</param>
		public RepositoryNotFoundException(FilePath location) : this(location.GetPath())
		{
		}

		/// <summary>Constructs an exception indicating a local repository does not exist.</summary>
		/// <remarks>Constructs an exception indicating a local repository does not exist.</remarks>
		/// <param name="location">description of the repository not found, usually file path.
		/// 	</param>
		/// <param name="why">why the repository does not exist.</param>
		public RepositoryNotFoundException(FilePath location, Exception why) : this(location
			.GetPath(), why)
		{
		}

		/// <summary>Constructs an exception indicating a local repository does not exist.</summary>
		/// <remarks>Constructs an exception indicating a local repository does not exist.</remarks>
		/// <param name="location">description of the repository not found, usually file path.
		/// 	</param>
		public RepositoryNotFoundException(string location) : base(Message(location))
		{
		}

		/// <summary>Constructs an exception indicating a local repository does not exist.</summary>
		/// <remarks>Constructs an exception indicating a local repository does not exist.</remarks>
		/// <param name="location">description of the repository not found, usually file path.
		/// 	</param>
		/// <param name="why">why the repository does not exist.</param>
		public RepositoryNotFoundException(string location, Exception why) : base(Message
			(location), why)
		{
		}

		private static string Message(string location)
		{
			return MessageFormat.Format(JGitText.Get().repositoryNotFound, location);
		}
	}
}
