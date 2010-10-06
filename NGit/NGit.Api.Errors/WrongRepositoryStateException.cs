using System;
using NGit.Api.Errors;
using Sharpen;

namespace NGit.Api.Errors
{
	/// <summary>
	/// Exception thrown when the state of the repository doesn't allow the execution
	/// of a certain command.
	/// </summary>
	/// <remarks>
	/// Exception thrown when the state of the repository doesn't allow the execution
	/// of a certain command. E.g. when a CommitCommand should be executed on a
	/// repository with unresolved conflicts this exception will be thrown.
	/// </remarks>
	[System.Serializable]
	public class WrongRepositoryStateException : GitAPIException
	{
		private const long serialVersionUID = 1L;

		/// <param name="message"></param>
		/// <param name="cause"></param>
		public WrongRepositoryStateException(string message, Exception cause) : base(message
			, cause)
		{
		}

		/// <param name="message"></param>
		public WrongRepositoryStateException(string message) : base(message)
		{
		}
	}
}
