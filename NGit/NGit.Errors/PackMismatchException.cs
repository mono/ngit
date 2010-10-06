using System.IO;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Thrown when a PackFile no longer matches the PackIndex.</summary>
	/// <remarks>Thrown when a PackFile no longer matches the PackIndex.</remarks>
	[System.Serializable]
	public class PackMismatchException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Construct a pack modification error.</summary>
		/// <remarks>Construct a pack modification error.</remarks>
		/// <param name="why">description of the type of error.</param>
		public PackMismatchException(string why) : base(why)
		{
		}
	}
}
