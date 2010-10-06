using System.IO;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>
	/// An exception thrown when a gitlink entry is found and cannot be
	/// handled.
	/// </summary>
	/// <remarks>
	/// An exception thrown when a gitlink entry is found and cannot be
	/// handled.
	/// </remarks>
	[System.Serializable]
	public class GitlinksNotSupportedException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Construct a GitlinksNotSupportedException for the specified link</summary>
		/// <param name="s">name of link in tree or workdir</param>
		public GitlinksNotSupportedException(string s) : base(s)
		{
		}
	}
}
