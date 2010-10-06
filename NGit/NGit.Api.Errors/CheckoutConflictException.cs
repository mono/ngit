using System;
using System.Collections.Generic;
using NGit.Api.Errors;
using Sharpen;

namespace NGit.Api.Errors
{
	/// <summary>
	/// Exception thrown when a command can't succeed because of unresolved
	/// conflicts.
	/// </summary>
	/// <remarks>
	/// Exception thrown when a command can't succeed because of unresolved
	/// conflicts.
	/// </remarks>
	[System.Serializable]
	public class CheckoutConflictException : GitAPIException
	{
		private const long serialVersionUID = 1L;

		private IList<string> conflictingPaths;

		public CheckoutConflictException(string message, Exception cause) : base(message, 
			cause)
		{
		}

		internal CheckoutConflictException(string message, IList<string> conflictingPaths
			, Exception cause) : base(message, cause)
		{
			this.conflictingPaths = conflictingPaths;
		}

		public CheckoutConflictException(string message) : base(message)
		{
		}

		internal CheckoutConflictException(string message, IList<string> conflictingPaths
			) : base(message)
		{
			this.conflictingPaths = conflictingPaths;
		}

		/// <returns>all the paths where unresolved conflicts have been detected</returns>
		public virtual IList<string> GetConflictingPaths()
		{
			return conflictingPaths;
		}

		/// <summary>Adds a new conflicting path</summary>
		/// <param name="conflictingPath"></param>
		/// <returns>
		/// 
		/// <code>this</code>
		/// </returns>
		internal virtual NGit.Api.Errors.CheckoutConflictException AddConflictingPath(string
			 conflictingPath)
		{
			if (conflictingPaths == null)
			{
				conflictingPaths = new List<string>();
			}
			conflictingPaths.AddItem(conflictingPath);
			return this;
		}
	}
}
