using System.IO;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>An expected object is missing.</summary>
	/// <remarks>An expected object is missing.</remarks>
	[System.Serializable]
	public class MissingObjectException : IOException
	{
		private const long serialVersionUID = 1L;

		private readonly ObjectId missing;

		/// <summary>Construct a MissingObjectException for the specified object id.</summary>
		/// <remarks>
		/// Construct a MissingObjectException for the specified object id.
		/// Expected type is reported to simplify tracking down the problem.
		/// </remarks>
		/// <param name="id">SHA-1</param>
		/// <param name="type">object type</param>
		public MissingObjectException(ObjectId id, string type) : base(MessageFormat.Format
			(JGitText.Get().missingObject, type, id.Name))
		{
			missing = id.Copy();
		}

		/// <summary>Construct a MissingObjectException for the specified object id.</summary>
		/// <remarks>
		/// Construct a MissingObjectException for the specified object id.
		/// Expected type is reported to simplify tracking down the problem.
		/// </remarks>
		/// <param name="id">SHA-1</param>
		/// <param name="type">object type</param>
		public MissingObjectException(ObjectId id, int type) : this(id, Constants.TypeString
			(type))
		{
		}

		/// <summary>Construct a MissingObjectException for the specified object id.</summary>
		/// <remarks>
		/// Construct a MissingObjectException for the specified object id. Expected
		/// type is reported to simplify tracking down the problem.
		/// </remarks>
		/// <param name="id">SHA-1</param>
		/// <param name="type">object type</param>
		public MissingObjectException(AbbreviatedObjectId id, int type) : base(MessageFormat
			.Format(JGitText.Get().missingObject, Constants.TypeString(type), id.Name))
		{
			missing = null;
		}

		/// <returns>the ObjectId that was not found.</returns>
		public virtual ObjectId GetObjectId()
		{
			return missing;
		}
	}
}
