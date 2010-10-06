using NGit.Nls;
using Sharpen;

namespace NGit.Nls
{
	public class GermanTranslatedBundle : TranslationBundle
	{
		public static GermanTranslatedBundle Get()
		{
			return NLS.GetBundleFor<GermanTranslatedBundle>();
		}

		public string goodMorning;
	}
}
