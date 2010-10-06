using System;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>
	/// Indicates a checked exception was thrown inside of
	/// <see cref="NGit.Revwalk.RevWalk">NGit.Revwalk.RevWalk</see>
	/// .
	/// <p>
	/// Usually this exception is thrown from the Iterator created around a RevWalk
	/// instance, as the Iterator API does not allow checked exceptions to be thrown
	/// from hasNext() or next(). The
	/// <see cref="System.Exception.InnerException()">System.Exception.InnerException()</see>
	/// of this exception
	/// is the original checked exception that we really wanted to throw back to the
	/// application for handling and recovery.
	/// </summary>
	[System.Serializable]
	public class RevWalkException : RuntimeException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Create a new walk exception an original cause.</summary>
		/// <remarks>Create a new walk exception an original cause.</remarks>
		/// <param name="cause">the checked exception that describes why the walk failed.</param>
		public RevWalkException(Exception cause) : base(JGitText.Get().walkFailure, cause
			)
		{
		}
	}
}
