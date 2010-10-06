using System.Collections.Generic;
using NGit.Fnmatch;
using Sharpen;

namespace NGit.Fnmatch
{
	internal interface Head
	{
		/// <param name="c">the character which decides which heads are returned.</param>
		/// <returns>a list of heads based on the input.</returns>
		IList<Head> GetNextHeads(char c);
	}
}
