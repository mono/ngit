using NGit.Fnmatch;
using Sharpen;

namespace NGit.Fnmatch
{
	internal sealed class WildCardHead : AbstractHead
	{
		internal WildCardHead(bool star) : base(star)
		{
		}

		protected internal sealed override bool Matches(char c)
		{
			return true;
		}
	}
}
