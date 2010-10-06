using System.IO;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>An inconsistency with respect to handling different object types.</summary>
	/// <remarks>
	/// An inconsistency with respect to handling different object types.
	/// This most likely signals a programming error rather than a corrupt
	/// object database.
	/// </remarks>
	[System.Serializable]
	public class IncorrectObjectTypeException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Construct and IncorrectObjectTypeException for the specified object id.</summary>
		/// <remarks>
		/// Construct and IncorrectObjectTypeException for the specified object id.
		/// Provide the type to make it easier to track down the problem.
		/// </remarks>
		/// <param name="id">SHA-1</param>
		/// <param name="type">object type</param>
		public IncorrectObjectTypeException(ObjectId id, string type) : base(MessageFormat
			.Format(JGitText.Get().objectIsNotA, id.Name, type))
		{
		}

		/// <summary>Construct and IncorrectObjectTypeException for the specified object id.</summary>
		/// <remarks>
		/// Construct and IncorrectObjectTypeException for the specified object id.
		/// Provide the type to make it easier to track down the problem.
		/// </remarks>
		/// <param name="id">SHA-1</param>
		/// <param name="type">object type</param>
		public IncorrectObjectTypeException(ObjectId id, int type) : this(id, Constants.TypeString
			(type))
		{
		}
	}
}
