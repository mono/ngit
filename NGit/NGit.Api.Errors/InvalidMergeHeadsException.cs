using NGit.Api.Errors;
using Sharpen;

namespace NGit.Api.Errors
{
	/// <summary>
	/// Exception thrown when a merge command was called without specifying the
	/// proper amount/type of merge heads.
	/// </summary>
	/// <remarks>
	/// Exception thrown when a merge command was called without specifying the
	/// proper amount/type of merge heads. E.g. a non-octopus merge strategy was
	/// confronted with more than one head to be merged into HEAD. Another
	/// case would be if a merge was called without including any head.
	/// </remarks>
	[System.Serializable]
	public class InvalidMergeHeadsException : GitAPIException
	{
		private const long serialVersionUID = 1L;

		/// <param name="msg"></param>
		public InvalidMergeHeadsException(string msg) : base(msg)
		{
		}
	}
}
