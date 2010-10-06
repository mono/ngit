using System.IO;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Thrown when a PackFile previously failed and is known to be unusable</summary>
	[System.Serializable]
	public class PackInvalidException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Construct a pack invalid error.</summary>
		/// <remarks>Construct a pack invalid error.</remarks>
		/// <param name="path">path of the invalid pack file.</param>
		public PackInvalidException(FilePath path) : base(MessageFormat.Format(JGitText.Get
			().packFileInvalid, path.GetAbsolutePath()))
		{
		}
	}
}
