using System;
using NGit.Api.Errors;
using Sharpen;

namespace NGit.Api.Errors
{
	/// <summary>
	/// Exception thrown when a command expected the
	/// <code>HEAD</code>
	/// reference to exist
	/// but couldn't find such a reference
	/// </summary>
	[System.Serializable]
	public class NoHeadException : GitAPIException
	{
		private const long serialVersionUID = 1L;

		/// <param name="message"></param>
		/// <param name="cause"></param>
		public NoHeadException(string message, Exception cause) : base(message, cause)
		{
		}

		/// <param name="message"></param>
		public NoHeadException(string message) : base(message)
		{
		}
	}
}
