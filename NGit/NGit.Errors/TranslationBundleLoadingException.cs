using System;
using System.Globalization;
using NGit.Errors;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>
	/// This exception will be thrown when a translation bundle loading
	/// fails.
	/// </summary>
	/// <remarks>
	/// This exception will be thrown when a translation bundle loading
	/// fails.
	/// </remarks>
	[System.Serializable]
	public class TranslationBundleLoadingException : TranslationBundleException
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Construct a
		/// <see cref="TranslationBundleLoadingException">TranslationBundleLoadingException</see>
		/// for the specified
		/// bundle class and locale.
		/// </summary>
		/// <param name="bundleClass">the bundle class for which the loading failed</param>
		/// <param name="locale">the locale for which the loading failed</param>
		/// <param name="cause">
		/// the original exception thrown from the
		/// <see cref="Sharpen.ResourceBundle.GetBundle(string, System.Globalization.CultureInfo)
		/// 	">Sharpen.ResourceBundle.GetBundle(string, System.Globalization.CultureInfo)</see>
		/// method.
		/// </param>
		public TranslationBundleLoadingException(Type bundleClass, CultureInfo locale, Exception
			 cause) : base("Loading of translation bundle failed for [" + bundleClass.FullName
			 + ", " + locale.ToString() + "]", bundleClass, locale, cause)
		{
		}
	}
}
