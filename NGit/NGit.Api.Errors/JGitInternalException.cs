using System;
using Sharpen;

namespace NGit.Api.Errors
{
	/// <summary>
	/// Exception thrown when during command execution a low-level exception from the
	/// JGit library is thrown.
	/// </summary>
	/// <remarks>
	/// Exception thrown when during command execution a low-level exception from the
	/// JGit library is thrown. Also when certain low-level error situations are
	/// reported by JGit through return codes this Exception will be thrown.
	/// <p>
	/// During command execution a lot of exceptions may be thrown. Some of them
	/// represent error situations which can be handled specifically by the caller of
	/// the command. But a lot of exceptions are so low-level that is is unlikely
	/// that the caller of the command can handle them effectively. The huge number
	/// of these low-level exceptions which are thrown by the commands lead to a
	/// complicated and wide interface of the commands. Callers of the API have to
	/// deal with a lot of exceptions they don't understand.
	/// <p>
	/// To overcome this situation this class was introduced. Commands will wrap all
	/// exceptions they declare as low-level in their context into an instance of
	/// this class. Callers of the commands have to deal with one type of low-level
	/// exceptions. Callers will always get access to the original exception (if
	/// available) by calling
	/// <code>#getCause()</code>
	/// .
	/// </remarks>
	[System.Serializable]
	public class JGitInternalException : RuntimeException
	{
		private const long serialVersionUID = 1L;

		/// <param name="message"></param>
		/// <param name="cause"></param>
		public JGitInternalException(string message, Exception cause) : base(message, cause
			)
		{
		}

		/// <param name="message"></param>
		public JGitInternalException(string message) : base(message)
		{
		}
	}
}
