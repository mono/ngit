using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	internal class EndGenerator : Generator
	{
		internal static readonly NGit.Revwalk.EndGenerator INSTANCE = new NGit.Revwalk.EndGenerator
			();

		public EndGenerator()
		{
		}

		// We have nothing to initialize.
		internal override RevCommit Next()
		{
			return null;
		}

		internal override int OutputType()
		{
			return 0;
		}
	}
}
