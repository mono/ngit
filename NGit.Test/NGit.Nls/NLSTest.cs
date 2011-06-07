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

using System.Globalization;
using NGit.Nls;
using Sharpen;

namespace NGit.Nls
{
	[NUnit.Framework.TestFixture]
	public class NLSTest
	{
		[NUnit.Framework.Test]
		public virtual void TestNLSLocale()
		{
			NLS.SetLocale(NLS.ROOT_LOCALE);
			GermanTranslatedBundle bundle = GermanTranslatedBundle.Get();
			NUnit.Framework.Assert.AreEqual(NLS.ROOT_LOCALE, bundle.EffectiveLocale());
			NLS.SetLocale(Sharpen.Extensions.GetGermanCulture());
			bundle = GermanTranslatedBundle.Get();
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.GetGermanCulture(), bundle.EffectiveLocale
				());
		}

		[NUnit.Framework.Test]
		public virtual void TestJVMDefaultLocale()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = NLS.ROOT_LOCALE;
			NLS.UseJVMDefaultLocale();
			GermanTranslatedBundle bundle = GermanTranslatedBundle.Get();
			NUnit.Framework.Assert.AreEqual(NLS.ROOT_LOCALE, bundle.EffectiveLocale());
			System.Threading.Thread.CurrentThread.CurrentCulture = Sharpen.Extensions.GetGermanCulture();
			NLS.UseJVMDefaultLocale();
			bundle = GermanTranslatedBundle.Get();
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.GetGermanCulture(), bundle.EffectiveLocale
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestThreadTranslationBundleInheritance()
		{
			NLS.SetLocale(NLS.ROOT_LOCALE);
			GermanTranslatedBundle mainThreadsBundle = GermanTranslatedBundle.Get();
			_T498707310 t = new _T498707310(this);
			t.Start();
			t.Join();
			NUnit.Framework.Assert.AreSame(mainThreadsBundle, t.bundle);
			NLS.SetLocale(Sharpen.Extensions.GetGermanCulture());
			mainThreadsBundle = GermanTranslatedBundle.Get();
			t = new _T498707310(this);
			t.Start();
			t.Join();
			NUnit.Framework.Assert.AreSame(mainThreadsBundle, t.bundle);
		}

		internal class _T498707310 : Sharpen.Thread
		{
			internal GermanTranslatedBundle bundle;

			public override void Run()
			{
				this.bundle = GermanTranslatedBundle.Get();
			}

			internal _T498707310(NLSTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly NLSTest _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		/// <exception cref="Sharpen.ExecutionException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParallelThreadsWithDifferentLocales()
		{
			CyclicBarrier barrier = new CyclicBarrier(2);
			// wait for the other thread to set its locale
			ExecutorService pool = Executors.NewFixedThreadPool(2);
			try
			{
				Future<TranslationBundle> root = pool.Submit(new _T879158014(this, NLS.ROOT_LOCALE,
					barrier));
				Future<TranslationBundle> german = pool.Submit(new _T879158014(this, Sharpen.Extensions.GetGermanCulture(),
					barrier));
				NUnit.Framework.Assert.AreEqual(NLS.ROOT_LOCALE, root.Get().EffectiveLocale());
				NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.GetGermanCulture(), german.Get
					().EffectiveLocale());
			}
			finally
			{
				pool.Shutdown();
				pool.AwaitTermination(long.MaxValue, TimeUnit.SECONDS);
			}
		}

		internal class _T879158014 : Callable<TranslationBundle>
		{
			private CultureInfo locale;
			CyclicBarrier barrier;

			internal _T879158014(NLSTest _enclosing, CultureInfo locale, CyclicBarrier barrier)
			{
				this._enclosing = _enclosing;
				this.locale = locale;
				this.barrier = barrier;
			}

			/// <exception cref="System.Exception"></exception>
			public virtual TranslationBundle Call()
			{
				NLS.SetLocale(this.locale);
				barrier.Await();
				return GermanTranslatedBundle.Get();
			}

			private readonly NLSTest _enclosing;
		}
	}
}
