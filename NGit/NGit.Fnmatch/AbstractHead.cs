using System;
using System.Collections.Generic;
using NGit;
using NGit.Fnmatch;
using Sharpen;

namespace NGit.Fnmatch
{
	internal abstract class AbstractHead : Head
	{
		private IList<Head> newHeads = null;

		private readonly bool star;

		protected internal abstract bool Matches(char c);

		internal AbstractHead(bool star)
		{
			this.star = star;
		}

		/// <param name="newHeads">
		/// a list of
		/// <see cref="Head">Head</see>
		/// s which will not be modified.
		/// </param>
		public void SetNewHeads(IList<Head> newHeads)
		{
			if (this.newHeads != null)
			{
				throw new InvalidOperationException(JGitText.Get().propertyIsAlreadyNonNull);
			}
			this.newHeads = newHeads;
		}

		public virtual IList<Head> GetNextHeads(char c)
		{
			if (Matches(c))
			{
				return newHeads;
			}
			else
			{
				return FileNameMatcher.EMPTY_HEAD_LIST;
			}
		}

		internal virtual bool IsStar()
		{
			return star;
		}
	}
}
