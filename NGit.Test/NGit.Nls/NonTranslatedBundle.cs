using NGit.Nls;
using Sharpen;

namespace NGit.Nls
{
	public class NonTranslatedBundle : TranslationBundle
	{
		public static NonTranslatedBundle Get()
		{
			return NLS.GetBundleFor<NonTranslatedBundle>();
		}

		public string goodMorning;
	}
}
