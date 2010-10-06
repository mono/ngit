using NGit.Nls;
using Sharpen;

namespace NGit.Nls
{
	public class NoPropertiesBundle : TranslationBundle
	{
		public static NoPropertiesBundle Get()
		{
			return NLS.GetBundleFor<NoPropertiesBundle>();
		}
	}
}
