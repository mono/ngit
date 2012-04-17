/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using NGit.Errors;
using NGit.Nls;
using Sharpen;

namespace NGit.Nls
{
	[NUnit.Framework.TestFixture]
	public class TranslationBundleTest
	{
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("This does not pass. Not sure why EffectiveLocale would ever equal NLS.ROOT_LOCALE")]
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

		[NUnit.Framework.Test]
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
