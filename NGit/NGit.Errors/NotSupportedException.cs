using System;
using System.IO;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>JGit encountered a case that it knows it cannot yet handle.</summary>
	/// <remarks>JGit encountered a case that it knows it cannot yet handle.</remarks>
	[System.Serializable]
	public class NotSupportedException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Construct a NotSupportedException for some issue JGit cannot
		/// yet handle.
		/// </summary>
		/// <remarks>
		/// Construct a NotSupportedException for some issue JGit cannot
		/// yet handle.
		/// </remarks>
		/// <param name="s">message describing the issue</param>
		public NotSupportedException(string s) : base(s)
		{
		}

		/// <summary>Construct a NotSupportedException for some issue JGit cannot yet handle.
		/// 	</summary>
		/// <remarks>Construct a NotSupportedException for some issue JGit cannot yet handle.
		/// 	</remarks>
		/// <param name="s">message describing the issue</param>
		/// <param name="why">a lower level implementation specific issue.</param>
		public NotSupportedException(string s, Exception why) : base(s)
		{
			Sharpen.Extensions.InitCause(this, why);
		}
	}
}
