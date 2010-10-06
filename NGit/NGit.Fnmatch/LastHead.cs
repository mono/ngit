using System.Collections.Generic;
using NGit.Fnmatch;
using Sharpen;

namespace NGit.Fnmatch
{
	internal sealed class LastHead : Head
	{
		internal static readonly Head INSTANCE = new NGit.Fnmatch.LastHead();

		/// <summary>
		/// Don't call this constructor, use
		/// <see cref="INSTANCE">INSTANCE</see>
		/// </summary>
		public LastHead()
		{
		}

		// defined because of javadoc and visibility modifier.
		public IList<Head> GetNextHeads(char c)
		{
			return FileNameMatcher.EMPTY_HEAD_LIST;
		}
	}
}
