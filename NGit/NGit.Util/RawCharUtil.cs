using Sharpen;

namespace NGit.Util
{
	/// <summary>
	/// Utility class for character functions on raw bytes
	/// <p>
	/// Characters are assumed to be 8-bit US-ASCII.
	/// </summary>
	/// <remarks>
	/// Utility class for character functions on raw bytes
	/// <p>
	/// Characters are assumed to be 8-bit US-ASCII.
	/// </remarks>
	public class RawCharUtil
	{
		private static readonly bool[] WHITESPACE = new bool[256];

		static RawCharUtil()
		{
			WHITESPACE['\r'] = true;
			WHITESPACE['\n'] = true;
			WHITESPACE['\t'] = true;
			WHITESPACE[' '] = true;
		}

		/// <summary>Determine if an 8-bit US-ASCII encoded character is represents whitespace
		/// 	</summary>
		/// <param name="c">the 8-bit US-ASCII encoded character</param>
		/// <returns>true if c represents a whitespace character in 8-bit US-ASCII</returns>
		public static bool IsWhitespace(byte c)
		{
			return WHITESPACE[c & unchecked((int)(0xff))];
		}

		/// <summary>
		/// Returns the new end point for the byte array passed in after trimming any
		/// trailing whitespace characters, as determined by the isWhitespace()
		/// function.
		/// </summary>
		/// <remarks>
		/// Returns the new end point for the byte array passed in after trimming any
		/// trailing whitespace characters, as determined by the isWhitespace()
		/// function. start and end are assumed to be within the bounds of raw.
		/// </remarks>
		/// <param name="raw">the byte array containing the portion to trim whitespace for</param>
		/// <param name="start">the start of the section of bytes</param>
		/// <param name="end">the end of the section of bytes</param>
		/// <returns>the new end point</returns>
		public static int TrimTrailingWhitespace(byte[] raw, int start, int end)
		{
			int ptr = end - 1;
			while (start <= ptr && IsWhitespace(raw[ptr]))
			{
				ptr--;
			}
			return ptr + 1;
		}

		/// <summary>
		/// Returns the new start point for the byte array passed in after trimming
		/// any leading whitespace characters, as determined by the isWhitespace()
		/// function.
		/// </summary>
		/// <remarks>
		/// Returns the new start point for the byte array passed in after trimming
		/// any leading whitespace characters, as determined by the isWhitespace()
		/// function. start and end are assumed to be within the bounds of raw.
		/// </remarks>
		/// <param name="raw">the byte array containing the portion to trim whitespace for</param>
		/// <param name="start">the start of the section of bytes</param>
		/// <param name="end">the end of the section of bytes</param>
		/// <returns>the new start point</returns>
		public static int TrimLeadingWhitespace(byte[] raw, int start, int end)
		{
			while (start < end && IsWhitespace(raw[start]))
			{
				start++;
			}
			return start;
		}

		public RawCharUtil()
		{
		}
		// This will never be called
	}
}
