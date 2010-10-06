using System.IO;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Exception thrown when an object cannot be read from Git.</summary>
	/// <remarks>Exception thrown when an object cannot be read from Git.</remarks>
	[System.Serializable]
	public class CorruptObjectException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Construct a CorruptObjectException for reporting a problem specified
		/// object id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="why"></param>
		public CorruptObjectException(AnyObjectId id, string why) : this(id.ToObjectId(), 
			why)
		{
		}

		/// <summary>
		/// Construct a CorruptObjectException for reporting a problem specified
		/// object id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="why"></param>
		public CorruptObjectException(ObjectId id, string why) : base(MessageFormat.Format
			(JGitText.Get().objectIsCorrupt, id.Name, why))
		{
		}

		/// <summary>
		/// Construct a CorruptObjectException for reporting a problem not associated
		/// with a specific object id.
		/// </summary>
		/// <remarks>
		/// Construct a CorruptObjectException for reporting a problem not associated
		/// with a specific object id.
		/// </remarks>
		/// <param name="why"></param>
		public CorruptObjectException(string why) : base(why)
		{
		}
	}
}
