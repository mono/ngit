using System.IO;
using NGit;
using NGit.Dircache;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Indicates one or more paths in a DirCache have non-zero stages present.</summary>
	/// <remarks>Indicates one or more paths in a DirCache have non-zero stages present.</remarks>
	[System.Serializable]
	public class UnmergedPathException : IOException
	{
		private const long serialVersionUID = 1L;

		private readonly DirCacheEntry entry;

		/// <summary>Create a new unmerged path exception.</summary>
		/// <remarks>Create a new unmerged path exception.</remarks>
		/// <param name="dce">the first non-zero stage of the unmerged path.</param>
		public UnmergedPathException(DirCacheEntry dce) : base(MessageFormat.Format(JGitText
			.Get().unmergedPath, dce.GetPathString()))
		{
			entry = dce;
		}

		/// <returns>the first non-zero stage of the unmerged path</returns>
		public virtual DirCacheEntry GetDirCacheEntry()
		{
			return entry;
		}
	}
}
