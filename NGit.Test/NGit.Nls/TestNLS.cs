using System;
using System.Globalization;
using NGit.Nls;
using NUnit.Framework;
using Sharpen;

namespace NGit.Nls
{
	public class TestNLS : TestCase
	{
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
		public virtual void TestThreadTranslationBundleInheritance()
		{
			NLS.SetLocale(NLS.ROOT_LOCALE);
			GermanTranslatedBundle mainThreadsBundle = GermanTranslatedBundle.Get();
			_T180027710 t = new _T180027710(this);
			t.Start();
			t.Join();
			NUnit.Framework.Assert.AreSame(mainThreadsBundle, t.bundle);
			NLS.SetLocale(Sharpen.Extensions.GetGermanCulture());
			mainThreadsBundle = GermanTranslatedBundle.Get();
			t = new _T180027710(this);
			t.Start();
			t.Join();
			NUnit.Framework.Assert.AreSame(mainThreadsBundle, t.bundle);
		}

		internal class _T180027710 : Sharpen.Thread
		{
			internal GermanTranslatedBundle bundle;

			public override void Run()
			{
				this.bundle = GermanTranslatedBundle.Get();
			}

			internal _T180027710(TestNLS _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly TestNLS _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestParallelThreadsWithDifferentLocales()
		{
			CyclicBarrier barrier = new CyclicBarrier(2);
			// wait for the other thread to set its locale
			_T1972088541 t1 = new _T1972088541(this, NLS.ROOT_LOCALE);
			_T1972088541 t2 = new _T1972088541(this, Sharpen.Extensions.GetGermanCulture());
			t1.Start();
			t2.Start();
			t1.Join();
			t2.Join();
			NUnit.Framework.Assert.IsNull("t1 was interrupted or barrier was broken", t1.e);
			NUnit.Framework.Assert.IsNull("t2 was interrupted or barrier was broken", t2.e);
			NUnit.Framework.Assert.AreEqual(NLS.ROOT_LOCALE, t1.bundle.EffectiveLocale());
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.GetGermanCulture(), t2.bundle.
				EffectiveLocale());
		}

		internal class _T1972088541 : Sharpen.Thread
		{
			internal CultureInfo locale;

			internal GermanTranslatedBundle bundle;

			internal Exception e;

			internal _T1972088541(TestNLS _enclosing, CultureInfo locale)
			{
				this._enclosing = _enclosing;
				this.locale = locale;
			}

			public override void Run()
			{
				try
				{
					NLS.SetLocale(this.locale);
					barrier.Await();
					this.bundle = GermanTranslatedBundle.Get();
				}
				catch (Exception e)
				{
					this.e = e;
				}
				catch (BrokenBarrierException e)
				{
					this.e = e;
				}
			}

			private readonly TestNLS _enclosing;
		}
	}
}
