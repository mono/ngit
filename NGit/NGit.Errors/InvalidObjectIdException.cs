using System;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Thrown when an invalid object id is passed in as an argument.</summary>
	/// <remarks>Thrown when an invalid object id is passed in as an argument.</remarks>
	[System.Serializable]
	public class InvalidObjectIdException : ArgumentException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Create exception with bytes of the invalid object id.</summary>
		/// <remarks>Create exception with bytes of the invalid object id.</remarks>
		/// <param name="bytes">containing the invalid id.</param>
		/// <param name="offset">in the byte array where the error occurred.</param>
		/// <param name="length">of the sequence of invalid bytes.</param>
		public InvalidObjectIdException(byte[] bytes, int offset, int length) : base(MessageFormat
			.Format(JGitText.Get().invalidId, AsAscii(bytes, offset, length)))
		{
		}

		private static string AsAscii(byte[] bytes, int offset, int length)
		{
			try
			{
				return ": " + Sharpen.Extensions.CreateString(bytes, offset, length, "US-ASCII");
			}
			catch (UnsupportedEncodingException)
			{
				return string.Empty;
			}
			catch (StringIndexOutOfBoundsException)
			{
				return string.Empty;
			}
		}
	}
}
