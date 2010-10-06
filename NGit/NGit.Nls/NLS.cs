using System;
using System.Globalization;
using NGit.Nls;
using Sharpen;

namespace NGit.Nls
{
	/// <summary>
	/// The purpose of this class is to provide NLS (National Language Support)
	/// configurable per thread.
	/// </summary>
	/// <remarks>
	/// The purpose of this class is to provide NLS (National Language Support)
	/// configurable per thread.
	/// <p>
	/// The
	/// <see cref="SetLocale(System.Globalization.CultureInfo)">SetLocale(System.Globalization.CultureInfo)
	/// 	</see>
	/// method is used to configure locale for the
	/// calling thread. The locale setting is thread inheritable. This means that a
	/// child thread will have the same locale setting as its creator thread until it
	/// changes it explicitly.
	/// <p>
	/// Example of usage:
	/// <pre>
	/// NLS.setLocale(Locale.GERMAN);
	/// TransportText t = NLS.getBundleFor(TransportText.class);
	/// </pre>
	/// </remarks>
	public class NLS
	{
		/// <summary>The root locale constant.</summary>
		/// <remarks>The root locale constant. It is defined here because the Locale.ROOT is not defined in Java 5
		/// 	</remarks>
		public static readonly CultureInfo ROOT_LOCALE = CultureInfo.InvariantCulture;

		private sealed class _InheritableThreadLocal_74 : InheritableThreadLocal<NGit.Nls.NLS
			>
		{
			public _InheritableThreadLocal_74()
			{
			}

			protected override NGit.Nls.NLS InitialValue()
			{
				return new NGit.Nls.NLS(CultureInfo.CurrentCulture);
			}
		}

		private static readonly InheritableThreadLocal<NGit.Nls.NLS> local = new _InheritableThreadLocal_74
			();

		/// <summary>Sets the locale for the calling thread.</summary>
		/// <remarks>
		/// Sets the locale for the calling thread.
		/// <p>
		/// The
		/// <see cref="GetBundleFor{T}(System.Type{T})">GetBundleFor&lt;T&gt;(System.Type&lt;T&gt;)
		/// 	</see>
		/// method will honor this setting if if it
		/// is supported by the provided resource bundle property files. Otherwise,
		/// it will use a fall back locale as described in the
		/// <see cref="TranslationBundle">TranslationBundle</see>
		/// </remarks>
		/// <param name="locale">the preferred locale</param>
		public static void SetLocale(CultureInfo locale)
		{
			local.Set(new NGit.Nls.NLS(locale));
		}

		/// <summary>Sets the JVM default locale as the locale for the calling thread.</summary>
		/// <remarks>
		/// Sets the JVM default locale as the locale for the calling thread.
		/// <p>
		/// Semantically this is equivalent to <code>NLS.setLocale(Locale.getDefault())</code>.
		/// </remarks>
		public static void UseJVMDefaultLocale()
		{
			local.Set(new NGit.Nls.NLS(CultureInfo.CurrentCulture));
		}

		/// <summary>Returns an instance of the translation bundle of the required type.</summary>
		/// <remarks>
		/// Returns an instance of the translation bundle of the required type. All
		/// public String fields of the bundle instance will get their values
		/// injected as described in the
		/// <see cref="TranslationBundle">TranslationBundle</see>
		/// .
		/// </remarks>
		/// <?></?>
		/// <param name="type">required bundle type</param>
		/// <returns>an instance of the required bundle type</returns>
		/// <exception>
		/// TranslationBundleLoadingException
		/// see
		/// <see cref="NGit.Errors.TranslationBundleLoadingException">NGit.Errors.TranslationBundleLoadingException
		/// 	</see>
		/// </exception>
		/// <exception>
		/// TranslationStringMissingException
		/// see
		/// <see cref="NGit.Errors.TranslationStringMissingException">NGit.Errors.TranslationStringMissingException
		/// 	</see>
		/// </exception>
		public static T GetBundleFor<T>() where T:TranslationBundle
		{
			System.Type type = typeof(T);
			return local.Get().Get<T>();
		}

		private readonly CultureInfo locale;

		private readonly ConcurrentHashMap<Type, TranslationBundle> map = new ConcurrentHashMap
			<Type, TranslationBundle>();

		private NLS(CultureInfo locale)
		{
			this.locale = locale;
		}

		private T Get<T>() where T:TranslationBundle
		{
			System.Type type = typeof(T);
			TranslationBundle bundle = map.Get(type);
			if (bundle == null)
			{
				bundle = GlobalBundleCache.LookupBundle<T>(locale);
				// There is a small opportunity for a race, which we may
				// lose. Accept defeat and return the winner's instance.
				TranslationBundle old = map.PutIfAbsent(type, bundle);
				if (old != null)
				{
					bundle = old;
				}
			}
			return (T)bundle;
		}
	}
}
