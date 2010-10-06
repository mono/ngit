using System;
using System.Globalization;
using System.Reflection;
using NGit.Errors;
using NGit.Nls;
using Sharpen;

namespace NGit.Nls
{
	/// <summary>
	/// Base class for all translation bundles that provides injection of translated
	/// texts into public String fields.
	/// </summary>
	/// <remarks>
	/// Base class for all translation bundles that provides injection of translated
	/// texts into public String fields.
	/// <p>
	/// The usage pattern is shown with the following example. First define a new
	/// translation bundle:
	/// <pre>
	/// public class TransportText extends TranslationBundle {
	/// public static TransportText get() {
	/// return NLS.getBundleFor(TransportText.class);
	/// }
	/// public String repositoryNotFound;
	/// public String transportError;
	/// }
	/// </pre>
	/// Second, define one or more resource bundle property files.
	/// <pre>
	/// TransportText_en_US.properties:
	/// repositoryNotFound=repository {0} not found
	/// transportError=unknown error talking to {0}
	/// TransportText_de.properties:
	/// repositoryNotFound=repository {0} nicht gefunden
	/// transportError=unbekannter Fehler während der Kommunikation mit {0}
	/// ...
	/// </pre>
	/// Then make use of it:
	/// <pre>
	/// NLS.setLocale(Locale.GERMAN); // or skip this call to stick to the JVM default locale
	/// ...
	/// throw new TransportException(uri, TransportText.get().transportError);
	/// </pre>
	/// The translated text is automatically injected into the public String fields
	/// according to the locale set with
	/// <see cref="NLS.SetLocale(System.Globalization.CultureInfo)">NLS.SetLocale(System.Globalization.CultureInfo)
	/// 	</see>
	/// . However, the
	/// <see cref="NLS.SetLocale(System.Globalization.CultureInfo)">NLS.SetLocale(System.Globalization.CultureInfo)
	/// 	</see>
	/// method defines only prefered locale which will
	/// be honored only if it is supported by the provided resource bundle property
	/// files. Basically, this class will use
	/// <see cref="Sharpen.ResourceBundle.GetBundle(string, System.Globalization.CultureInfo)
	/// 	">Sharpen.ResourceBundle.GetBundle(string, System.Globalization.CultureInfo)</see>
	/// method to load a resource
	/// bundle. See the documentation of this method for a detailed explanation of
	/// resource bundle loading strategy. After a bundle is created the
	/// <see cref="EffectiveLocale()">EffectiveLocale()</see>
	/// method can be used to determine whether the
	/// bundle really corresponds to the requested locale or is a fallback.
	/// <p>
	/// To load a String from a resource bundle property file this class uses the
	/// <see cref="Sharpen.ResourceBundle.GetString(string)">Sharpen.ResourceBundle.GetString(string)
	/// 	</see>
	/// . This method can throw the
	/// <see cref="Sharpen.MissingResourceException">Sharpen.MissingResourceException</see>
	/// and this class is not making any effort to
	/// catch and/or translate this exception.
	/// <p>
	/// To define a concrete translation bundle one has to:
	/// <ul>
	/// <li>extend this class
	/// <li>define a public static get() method like in the example above
	/// <li>define public static String fields for each text message
	/// <li>make sure the translation bundle class provide public no arg constructor
	/// <li>provide one or more resource bundle property files in the same package
	/// where the translation bundle class resides
	/// </ul>
	/// </remarks>
	public abstract class TranslationBundle
	{
		private CultureInfo effectiveLocale;

		private Sharpen.ResourceBundle resourceBundle;

		/// <returns>
		/// the locale locale used for loading the resource bundle from which
		/// the field values were taken
		/// </returns>
		public virtual CultureInfo EffectiveLocale()
		{
			return effectiveLocale;
		}

		/// <returns>the resource bundle on which this translation bundle is based</returns>
		public virtual Sharpen.ResourceBundle ResourceBundle()
		{
			return resourceBundle;
		}

		/// <summary>Injects locale specific text in all instance fields of this instance.</summary>
		/// <remarks>
		/// Injects locale specific text in all instance fields of this instance.
		/// Only public instance fields of type <code>String</code> are considered.
		/// <p>
		/// The name of this (sub)class plus the given <code>locale</code> parameter
		/// define the resource bundle to be loaded. In other words the
		/// <code>this.getClass().getName()</code> is used as the
		/// <code>baseName</code> parameter in the
		/// <see cref="Sharpen.ResourceBundle.GetBundle(string, System.Globalization.CultureInfo)
		/// 	">Sharpen.ResourceBundle.GetBundle(string, System.Globalization.CultureInfo)</see>
		/// parameter to load the
		/// resource bundle.
		/// <p>
		/// </remarks>
		/// <param name="locale">defines the locale to be used when loading the resource bundle
		/// 	</param>
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
		/// <exception cref="NGit.Errors.TranslationBundleLoadingException"></exception>
		internal virtual void Load(CultureInfo locale)
		{
			Type bundleClass = GetType();
			try
			{
				resourceBundle = Sharpen.ResourceBundle.GetBundle(bundleClass.FullName, locale);
			}
			catch (MissingResourceException e)
			{
				throw new TranslationBundleLoadingException(bundleClass, locale, e);
			}
			this.effectiveLocale = resourceBundle.GetLocale();
			foreach (FieldInfo field in bundleClass.GetFields())
			{
				if (field.FieldType.Equals(typeof(string)))
				{
					try
					{
						string translatedText = resourceBundle.GetString(field.Name);
						field.SetValue(this, translatedText);
					}
					catch (MissingResourceException e)
					{
						throw new TranslationStringMissingException(bundleClass, locale, field.Name, e);
					}
					catch (ArgumentException e)
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
}
