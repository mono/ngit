using System;
using NGit.Api.Errors;
using Sharpen;

namespace NGit.Api.Errors
{
	/// <summary>
	/// Exception thrown when the options given to a command don't include a
	/// file pattern which is mandatory for processing.
	/// </summary>
	/// <remarks>
	/// Exception thrown when the options given to a command don't include a
	/// file pattern which is mandatory for processing.
	/// </remarks>
	[System.Serializable]
	public class NoFilepatternException : GitAPIException
	{
		private const long serialVersionUID = 1L;

		/// <param name="message"></param>
		/// <param name="cause"></param>
		public NoFilepatternException(string message, Exception cause) : base(message, cause
			)
		{
		}

		/// <param name="message"></param>
		public NoFilepatternException(string message) : base(message)
		{
		}
	}
}
