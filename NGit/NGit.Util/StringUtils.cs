using System;
using System.Text;
using NGit;
using Sharpen;

namespace NGit.Util
{
	/// <summary>Miscellaneous string comparison utility methods.</summary>
	/// <remarks>Miscellaneous string comparison utility methods.</remarks>
	public sealed class StringUtils
	{
		private static readonly char[] LC;

		static StringUtils()
		{
			LC = new char['Z' + 1];
			for (char c = (char)0; c < LC.Length; c++)
			{
				LC[c] = c;
			}
			for (char c_1 = 'A'; c_1 <= 'Z'; c_1++)
			{
				LC[c_1] = (char)('a' + (c_1 - 'A'));
			}
		}

		/// <summary>Convert the input to lowercase.</summary>
		/// <remarks>
		/// Convert the input to lowercase.
		/// <p>
		/// This method does not honor the JVM locale, but instead always behaves as
		/// though it is in the US-ASCII locale. Only characters in the range 'A'
		/// through 'Z' are converted. All other characters are left as-is, even if
		/// they otherwise would have a lowercase character equivalent.
		/// </remarks>
		/// <param name="c">the input character.</param>
		/// <returns>lowercase version of the input.</returns>
		public static char ToLowerCase(char c)
		{
			return c <= 'Z' ? LC[c] : c;
		}

		/// <summary>Convert the input string to lower case, according to the "C" locale.</summary>
		/// <remarks>
		/// Convert the input string to lower case, according to the "C" locale.
		/// <p>
		/// This method does not honor the JVM locale, but instead always behaves as
		/// though it is in the US-ASCII locale. Only characters in the range 'A'
		/// through 'Z' are converted, all other characters are left as-is, even if
		/// they otherwise would have a lowercase character equivalent.
		/// </remarks>
		/// <param name="in">the input string. Must not be null.</param>
		/// <returns>
		/// a copy of the input string, after converting characters in the
		/// range 'A'..'Z' to 'a'..'z'.
		/// </returns>
		public static string ToLowerCase(string @in)
		{
			StringBuilder r = new StringBuilder(@in.Length);
			for (int i = 0; i < @in.Length; i++)
			{
				r.Append(ToLowerCase(@in[i]));
			}
			return r.ToString();
		}

		/// <summary>Test if two strings are equal, ignoring case.</summary>
		/// <remarks>
		/// Test if two strings are equal, ignoring case.
		/// <p>
		/// This method does not honor the JVM locale, but instead always behaves as
		/// though it is in the US-ASCII locale.
		/// </remarks>
		/// <param name="a">first string to compare.</param>
		/// <param name="b">second string to compare.</param>
		/// <returns>true if a equals b</returns>
		public static bool EqualsIgnoreCase(string a, string b)
		{
			if (a == b)
			{
				return true;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			for (int i = 0; i < a.Length; i++)
			{
				if (ToLowerCase(a[i]) != ToLowerCase(b[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Parse a string as a standard Git boolean value.</summary>
		/// <remarks>
		/// Parse a string as a standard Git boolean value.
		/// <p>
		/// The terms
		/// <code>yes</code>
		/// ,
		/// <code>true</code>
		/// ,
		/// <code>1</code>
		/// ,
		/// <code>on</code>
		/// can all be
		/// used to mean
		/// <code>true</code>
		/// .
		/// <p>
		/// The terms
		/// <code>no</code>
		/// ,
		/// <code>false</code>
		/// ,
		/// <code>0</code>
		/// ,
		/// <code>off</code>
		/// can all be
		/// used to mean
		/// <code>false</code>
		/// .
		/// <p>
		/// Comparisons ignore case, via
		/// <see cref="EqualsIgnoreCase(string, string)">EqualsIgnoreCase(string, string)</see>
		/// .
		/// </remarks>
		/// <param name="stringValue">the string to parse.</param>
		/// <returns>
		/// the boolean interpretation of
		/// <code>value</code>
		/// .
		/// </returns>
		/// <exception cref="System.ArgumentException">
		/// if
		/// <code>value</code>
		/// is not recognized as one of the standard
		/// boolean names.
		/// </exception>
		public static bool ToBoolean(string stringValue)
		{
			if (stringValue == null)
			{
				throw new ArgumentNullException(JGitText.Get().expectedBooleanStringValue);
			}
			if (EqualsIgnoreCase("yes", stringValue) || EqualsIgnoreCase("true", stringValue)
				 || EqualsIgnoreCase("1", stringValue) || EqualsIgnoreCase("on", stringValue))
			{
				return true;
			}
			else
			{
				if (EqualsIgnoreCase("no", stringValue) || EqualsIgnoreCase("false", stringValue)
					 || EqualsIgnoreCase("0", stringValue) || EqualsIgnoreCase("off", stringValue))
				{
					return false;
				}
				else
				{
					throw new ArgumentException(MessageFormat.Format(JGitText.Get().notABoolean, stringValue
						));
				}
			}
		}

		public StringUtils()
		{
		}
		// Do not create instances
	}
}
