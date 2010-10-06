using NGit.Fnmatch;
using Sharpen;

namespace NGit.Fnmatch
{
	internal sealed class RestrictedWildCardHead : AbstractHead
	{
		private readonly char excludedCharacter;

		internal RestrictedWildCardHead(char excludedCharacter, bool star) : base(star)
		{
			this.excludedCharacter = excludedCharacter;
		}

		protected internal sealed override bool Matches(char c)
		{
			return c != excludedCharacter;
		}
	}
}
