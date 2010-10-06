using NGit;
using NGit.Errors;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>
	/// Thrown when a pattern contains a character group which is open to the right
	/// side or a character class which is open to the right side.
	/// </summary>
	/// <remarks>
	/// Thrown when a pattern contains a character group which is open to the right
	/// side or a character class which is open to the right side.
	/// </remarks>
	[System.Serializable]
	public class NoClosingBracketException : InvalidPatternException
	{
		private const long serialVersionUID = 1L;

		/// <param name="indexOfOpeningBracket">the position of the [ character which has no ] character.
		/// 	</param>
		/// <param name="openingBracket">the unclosed bracket.</param>
		/// <param name="closingBracket">the missing closing bracket.</param>
		/// <param name="pattern">the invalid pattern.</param>
		public NoClosingBracketException(int indexOfOpeningBracket, string openingBracket
			, string closingBracket, string pattern) : base(CreateMessage(indexOfOpeningBracket
			, openingBracket, closingBracket), pattern)
		{
		}

		private static string CreateMessage(int indexOfOpeningBracket, string openingBracket
			, string closingBracket)
		{
			return MessageFormat.Format(JGitText.Get().noClosingBracket, closingBracket, openingBracket
				, Sharpen.Extensions.ValueOf(indexOfOpeningBracket));
		}
	}
}
