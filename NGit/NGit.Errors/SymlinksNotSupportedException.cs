using System.IO;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>
	/// An exception thrown when a symlink entry is found and cannot be
	/// handled.
	/// </summary>
	/// <remarks>
	/// An exception thrown when a symlink entry is found and cannot be
	/// handled.
	/// </remarks>
	[System.Serializable]
	public class SymlinksNotSupportedException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Construct a SymlinksNotSupportedException for the specified link</summary>
		/// <param name="s">name of link in tree or workdir</param>
		public SymlinksNotSupportedException(string s) : base(s)
		{
		}
	}
}
