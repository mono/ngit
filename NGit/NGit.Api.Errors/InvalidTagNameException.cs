using NGit.Api.Errors;
using Sharpen;

namespace NGit.Api.Errors
{
	/// <summary>
	/// Exception thrown when a tag command was called with an invalid tag name (or
	/// null), such as bad~tag.
	/// </summary>
	/// <remarks>
	/// Exception thrown when a tag command was called with an invalid tag name (or
	/// null), such as bad~tag.
	/// </remarks>
	[System.Serializable]
	public class InvalidTagNameException : GitAPIException
	{
		private const long serialVersionUID = 1L;

		/// <param name="msg"></param>
		public InvalidTagNameException(string msg) : base(msg)
		{
		}
	}
}
