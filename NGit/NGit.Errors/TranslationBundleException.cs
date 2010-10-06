using System;
using System.Globalization;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Common base class for all translation bundle related exceptions.</summary>
	/// <remarks>Common base class for all translation bundle related exceptions.</remarks>
	[System.Serializable]
	public abstract class TranslationBundleException : RuntimeException
	{
		private const long serialVersionUID = 1L;

		private readonly Type bundleClass;

		private readonly CultureInfo locale;

		/// <summary>
		/// To construct an instance of
		/// <see cref="TranslationBundleException">TranslationBundleException</see>
		/// </summary>
		/// <param name="message">exception message</param>
		/// <param name="bundleClass">bundle class for which the exception occurred</param>
		/// <param name="locale">locale for which the exception occurred</param>
		/// <param name="cause">
		/// original exception that caused this exception. Usually thrown
		/// from the
		/// <see cref="Sharpen.ResourceBundle">Sharpen.ResourceBundle</see>
		/// class.
		/// </param>
		protected internal TranslationBundleException(string message, Type bundleClass, CultureInfo
			 locale, Exception cause) : base(message, cause)
		{
			this.bundleClass = bundleClass;
			this.locale = locale;
		}

		/// <returns>bundle class for which the exception occurred</returns>
		public Type GetBundleClass()
		{
			return bundleClass;
		}

		/// <returns>locale for which the exception occurred</returns>
		public CultureInfo GetLocale()
		{
			return locale;
		}
	}
}
