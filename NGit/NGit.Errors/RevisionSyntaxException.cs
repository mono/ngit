using System.IO;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>
	/// This signals a revision or object reference was not
	/// properly formatted.
	/// </summary>
	/// <remarks>
	/// This signals a revision or object reference was not
	/// properly formatted.
	/// </remarks>
	[System.Serializable]
	public class RevisionSyntaxException : IOException
	{
		private const long serialVersionUID = 1L;

		private readonly string revstr;

		/// <summary>
		/// Construct a RevisionSyntaxException indicating a syntax problem with a
		/// revision (or object) string.
		/// </summary>
		/// <remarks>
		/// Construct a RevisionSyntaxException indicating a syntax problem with a
		/// revision (or object) string.
		/// </remarks>
		/// <param name="revstr">The problematic revision string</param>
		public RevisionSyntaxException(string revstr)
		{
			this.revstr = revstr;
		}

		/// <summary>
		/// Construct a RevisionSyntaxException indicating a syntax problem with a
		/// revision (or object) string.
		/// </summary>
		/// <remarks>
		/// Construct a RevisionSyntaxException indicating a syntax problem with a
		/// revision (or object) string.
		/// </remarks>
		/// <param name="message">a specific reason</param>
		/// <param name="revstr">The problematic revision string</param>
		public RevisionSyntaxException(string message, string revstr) : base(message)
		{
			this.revstr = revstr;
		}

		public override string ToString()
		{
			return base.ToString() + ":" + revstr;
		}
	}
}
