using System.Text;
using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	public class DiffTestDataGenerator
	{
		/// <summary>Generate sequence of characters in ascending order.</summary>
		/// <remarks>
		/// Generate sequence of characters in ascending order. The first character
		/// is a space. All subsequent characters have an ASCII code one greater then
		/// the ASCII code of the preceding character. On exception: the character
		/// following which follows '~' is again a ' '.
		/// </remarks>
		/// <param name="len">length of the String to be returned</param>
		/// <returns>the sequence of characters as String</returns>
		public static string GenerateSequence(int len)
		{
			return GenerateSequence(len, 0, 0);
		}

		/// <summary>
		/// Generate sequence of characters similar to the one returned by
		/// <see cref="GenerateSequence(int)">GenerateSequence(int)</see>
		/// . But this time in each chunk of
		/// <skipPeriod> characters the last <skipLength> characters are left out. By
		/// calling this method twice with two different prime skipPeriod values and
		/// short skipLength values you create test data which is similar to what
		/// programmers do to their source code - huge files with only few
		/// insertions/deletions/changes.
		/// </summary>
		/// <param name="len">length of the String to be returned</param>
		/// <param name="skipPeriod"></param>
		/// <param name="skipLength"></param>
		/// <returns>the sequence of characters as String</returns>
		public static string GenerateSequence(int len, int skipPeriod, int skipLength)
		{
			StringBuilder text = new StringBuilder(len);
			int skipStart = skipPeriod - skipLength;
			int skippedChars = 0;
			int block = 0;
			for (int i = 0; i - skippedChars < len; ++i)
			{
				if ((i % skipPeriod) == 1)
				{
					text.Append((char)(256 + block++));
				}
				else
				{
					if (skipPeriod == 0 || i % skipPeriod < skipStart)
					{
						text.Append((char)(32 + i % 95));
					}
					else
					{
						skippedChars++;
					}
				}
			}
			return text.ToString();
		}
	}
}
