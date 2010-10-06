using System;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Thrown when a pattern passed in an argument was wrong.</summary>
	/// <remarks>Thrown when a pattern passed in an argument was wrong.</remarks>
	[System.Serializable]
	public class InvalidPatternException : Exception
	{
		private const long serialVersionUID = 1L;

		private readonly string pattern;

		/// <param name="message">explains what was wrong with the pattern.</param>
		/// <param name="pattern">the invalid pattern.</param>
		public InvalidPatternException(string message, string pattern) : base(message)
		{
			this.pattern = pattern;
		}

		/// <returns>the invalid pattern.</returns>
		public virtual string GetPattern()
		{
			return pattern;
		}
	}
}
