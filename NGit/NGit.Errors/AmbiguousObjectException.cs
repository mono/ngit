using System.Collections.Generic;
using System.IO;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>
	/// An
	/// <see cref="NGit.AbbreviatedObjectId">NGit.AbbreviatedObjectId</see>
	/// cannot be extended.
	/// </summary>
	[System.Serializable]
	public class AmbiguousObjectException : IOException
	{
		private const long serialVersionUID = 1L;

		private readonly AbbreviatedObjectId missing;

		private readonly ICollection<ObjectId> candidates;

		/// <summary>Construct a MissingObjectException for the specified object id.</summary>
		/// <remarks>
		/// Construct a MissingObjectException for the specified object id. Expected
		/// type is reported to simplify tracking down the problem.
		/// </remarks>
		/// <param name="id">SHA-1</param>
		/// <param name="candidates">the candidate matches returned by the ObjectReader.</param>
		public AmbiguousObjectException(AbbreviatedObjectId id, ICollection<ObjectId> candidates
			) : base(MessageFormat.Format(JGitText.Get().ambiguousObjectAbbreviation, id.Name
			))
		{
			this.missing = id;
			this.candidates = candidates;
		}

		/// <returns>the AbbreviatedObjectId that has more than one result.</returns>
		public virtual AbbreviatedObjectId GetAbbreviatedObjectId()
		{
			return missing;
		}

		/// <returns>the matching candidates (or at least a subset of them).</returns>
		public virtual ICollection<ObjectId> GetCandidates()
		{
			return candidates;
		}
	}
}
