using System;
using System.Globalization;
using NGit.Errors;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>
	/// This exception will be thrown when a translation string for a translation
	/// bundle and locale is missing.
	/// </summary>
	/// <remarks>
	/// This exception will be thrown when a translation string for a translation
	/// bundle and locale is missing.
	/// </remarks>
	[System.Serializable]
	public class TranslationStringMissingException : TranslationBundleException
	{
		private const long serialVersionUID = 1L;

		private readonly string key;

		/// <summary>
		/// Construct a
		/// <see cref="TranslationStringMissingException">TranslationStringMissingException</see>
		/// for the specified
		/// bundle class, locale and translation key
		/// </summary>
		/// <param name="bundleClass">the bundle class for which a translation string was missing
		/// 	</param>
		/// <param name="locale">the locale for which a translation string was missing</param>
		/// <param name="key">the key of the missing translation string</param>
		/// <param name="cause">
		/// the original exception thrown from the
		/// <see cref="Sharpen.ResourceBundle.GetString(string)">Sharpen.ResourceBundle.GetString(string)
		/// 	</see>
		/// method.
		/// </param>
		public TranslationStringMissingException(Type bundleClass, CultureInfo locale, string
			 key, Exception cause) : base("Translation missing for [" + bundleClass.FullName
			 + ", " + locale.ToString() + ", " + key + "]", bundleClass, locale, cause)
		{
			this.key = key;
		}

		/// <returns>the key of the missing translation string</returns>
		public virtual string GetKey()
		{
			return key;
		}
	}
}
