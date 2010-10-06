using System;
using NGit.Api.Errors;
using Sharpen;

namespace NGit.Api.Errors
{
	/// <summary>
	/// Exception thrown when the options given to a command don't include a
	/// specification of a message text (e.g.
	/// </summary>
	/// <remarks>
	/// Exception thrown when the options given to a command don't include a
	/// specification of a message text (e.g. a commit was called without explicitly
	/// specifying a commit message (or other options telling where to take the
	/// message from.
	/// </remarks>
	[System.Serializable]
	public class NoMessageException : GitAPIException
	{
		private const long serialVersionUID = 1L;

		/// <param name="message"></param>
		/// <param name="cause"></param>
		public NoMessageException(string message, Exception cause) : base(message, cause)
		{
		}

		/// <param name="message"></param>
		public NoMessageException(string message) : base(message)
		{
		}
	}
}
