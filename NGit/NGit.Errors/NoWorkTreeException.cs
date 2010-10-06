using System;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>
	/// Indicates a
	/// <see cref="NGit.Repository">NGit.Repository</see>
	/// has no working directory, and is thus bare.
	/// </summary>
	[System.Serializable]
	public class NoWorkTreeException : InvalidOperationException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Creates an exception indicating there is no work tree for a repository.</summary>
		/// <remarks>Creates an exception indicating there is no work tree for a repository.</remarks>
		public NoWorkTreeException() : base(JGitText.Get().bareRepositoryNoWorkdirAndIndex
			)
		{
		}
	}
}
