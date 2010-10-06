using NGit.Errors;
using NGit.Nls;
using NUnit.Framework;
using Sharpen;

namespace NGit.Nls
{
	public class TestTranslationBundle : TestCase
	{
		public virtual void TestMissingPropertiesFile()
		{
			try
			{
				new NoPropertiesBundle().Load(NLS.ROOT_LOCALE);
				NUnit.Framework.Assert.Fail("Expected TranslationBundleLoadingException");
			}
			catch (TranslationBundleLoadingException e)
			{
				NUnit.Framework.Assert.AreEqual(typeof(NoPropertiesBundle), e.GetBundleClass());
				NUnit.Framework.Assert.AreEqual(NLS.ROOT_LOCALE, e.GetLocale());
			}
		}

		// pass
		public virtual void TestMissingString()
		{
			try
			{
				new MissingPropertyBundle().Load(NLS.ROOT_LOCALE);
				NUnit.Framework.Assert.Fail("Expected TranslationStringMissingException");
			}
			catch (TranslationStringMissingException e)
			{
				NUnit.Framework.Assert.AreEqual("nonTranslatedKey", e.GetKey());
				NUnit.Framework.Assert.AreEqual(typeof(MissingPropertyBundle), e.GetBundleClass()
					);
				NUnit.Framework.Assert.AreEqual(NLS.ROOT_LOCALE, e.GetLocale());
			}
		}

		// pass
		public virtual void TestNonTranslatedBundle()
		{
			NonTranslatedBundle bundle = new NonTranslatedBundle();
			bundle.Load(NLS.ROOT_LOCALE);
			NUnit.Framework.Assert.AreEqual(NLS.ROOT_LOCALE, bundle.EffectiveLocale());
			NUnit.Framework.Assert.AreEqual("Good morning {0}", bundle.goodMorning);
			bundle.Load(Sharpen.Extensions.GetEnglishCulture());
			NUnit.Framework.Assert.AreEqual(NLS.ROOT_LOCALE, bundle.EffectiveLocale());
			NUnit.Framework.Assert.AreEqual("Good morning {0}", bundle.goodMorning);
			bundle.Load(Sharpen.Extensions.GetGermanCulture());
			NUnit.Framework.Assert.AreEqual(NLS.ROOT_LOCALE, bundle.EffectiveLocale());
			NUnit.Framework.Assert.AreEqual("Good morning {0}", bundle.goodMorning);
		}

		public virtual void TestGermanTranslation()
		{
			GermanTranslatedBundle bundle = new GermanTranslatedBundle();
			bundle.Load(NLS.ROOT_LOCALE);
			NUnit.Framework.Assert.AreEqual(NLS.ROOT_LOCALE, bundle.EffectiveLocale());
			NUnit.Framework.Assert.AreEqual("Good morning {0}", bundle.goodMorning);
			bundle.Load(Sharpen.Extensions.GetGermanCulture());
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.GetGermanCulture(), bundle.EffectiveLocale
				());
			NUnit.Framework.Assert.AreEqual("Guten Morgen {0}", bundle.goodMorning);
		}
	}
}
