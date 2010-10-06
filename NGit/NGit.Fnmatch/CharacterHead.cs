using NGit.Fnmatch;
using Sharpen;

namespace NGit.Fnmatch
{
	internal sealed class CharacterHead : AbstractHead
	{
		private readonly char expectedCharacter;

		protected internal CharacterHead(char expectedCharacter) : base(false)
		{
			this.expectedCharacter = expectedCharacter;
		}

		protected internal sealed override bool Matches(char c)
		{
			return c == expectedCharacter;
		}
	}
}
