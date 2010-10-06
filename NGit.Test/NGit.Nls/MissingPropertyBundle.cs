using NGit.Nls;
using Sharpen;

namespace NGit.Nls
{
	public class MissingPropertyBundle : TranslationBundle
	{
		public static MissingPropertyBundle Get()
		{
			return NLS.GetBundleFor<MissingPropertyBundle>();
		}

		public string goodMorning;

		public string nonTranslatedKey;
	}
}
