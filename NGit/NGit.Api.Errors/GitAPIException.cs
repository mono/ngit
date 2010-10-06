using System;
using Sharpen;

namespace NGit.Api.Errors
{
	/// <summary>
	/// Superclass of all exceptions thrown by the API classes in
	/// <code>org.eclipse.jgit.api</code>
	/// </summary>
	[System.Serializable]
	public abstract class GitAPIException : Exception
	{
		private const long serialVersionUID = 1L;

		public GitAPIException(string message, Exception cause) : base(message, cause)
		{
		}

		public GitAPIException(string message) : base(message)
		{
		}
	}
}
