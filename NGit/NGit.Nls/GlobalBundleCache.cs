using System;
using System.Collections.Generic;
using System.Globalization;
using NGit.Nls;
using Sharpen;

namespace NGit.Nls
{
	/// <summary>Global cache of translation bundles.</summary>
	/// <remarks>
	/// Global cache of translation bundles.
	/// <p>
	/// Every translation bundle will be cached here when it gets loaded for the
	/// first time from a thread. Another lookup for the same translation bundle
	/// (same locale and type) from the same or a different thread will return the
	/// cached one.
	/// <p>
	/// Note that NLS instances maintain per-thread Map of loaded translation
	/// bundles. Once a thread accesses a translation bundle it will keep reference
	/// to it and will not call
	/// <see cref="LookupBundle{T}(System.Globalization.CultureInfo, System.Type{T})">LookupBundle&lt;T&gt;(System.Globalization.CultureInfo, System.Type&lt;T&gt;)
	/// 	</see>
	/// again for the
	/// same translation bundle as long as its locale doesn't change.
	/// </remarks>
	internal class GlobalBundleCache
	{
		private static readonly IDictionary<CultureInfo, IDictionary<Type, TranslationBundle
			>> cachedBundles = new Dictionary<CultureInfo, IDictionary<Type, TranslationBundle
			>>();

		/// <summary>Looks up for a translation bundle in the global cache.</summary>
		/// <remarks>
		/// Looks up for a translation bundle in the global cache. If found returns
		/// the cached bundle. If not found creates a new instance puts it into the
		/// cache and returns it.
		/// </remarks>
		/// <?></?>
		/// <param name="locale">the preferred locale</param>
		/// <param name="type">required bundle type</param>
		/// <returns>an instance of the required bundle type</returns>
		/// <exception>
		/// TranslationBundleLoadingException
		/// see
		/// <see cref="TranslationBundle.Load(System.Globalization.CultureInfo)">TranslationBundle.Load(System.Globalization.CultureInfo)
		/// 	</see>
		/// </exception>
		/// <exception>
		/// TranslationStringMissingException
		/// see
		/// <see cref="TranslationBundle.Load(System.Globalization.CultureInfo)">TranslationBundle.Load(System.Globalization.CultureInfo)
		/// 	</see>
		/// </exception>
		internal static T LookupBundle<T>(CultureInfo locale) where T:TranslationBundle
		{
			System.Type type = typeof(T);
			lock (typeof(GlobalBundleCache))
			{
				try
				{
					IDictionary<Type, TranslationBundle> bundles = cachedBundles.Get(locale);
					if (bundles == null)
					{
						bundles = new Dictionary<Type, TranslationBundle>();
						cachedBundles.Put(locale, bundles);
					}
					TranslationBundle bundle = bundles.Get(type);
					if (bundle == null)
					{
						bundle = (TranslationBundle) System.Activator.CreateInstance(type);
						bundle.Load(locale);
						bundles.Put(type, bundle);
					}
					return (T)bundle;
				}
				catch (InstantiationException e)
				{
					throw new Error(e);
				}
				catch (MemberAccessException e)
				{
					throw new Error(e);
				}
			}
		}
	}
}
