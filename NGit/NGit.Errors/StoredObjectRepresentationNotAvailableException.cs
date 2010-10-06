using System;
using NGit.Storage.Pack;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>A previously selected representation is no longer available.</summary>
	/// <remarks>A previously selected representation is no longer available.</remarks>
	[System.Serializable]
	public class StoredObjectRepresentationNotAvailableException : Exception
	{
		private const long serialVersionUID = 1L;

		/// <summary>Construct an error for an object.</summary>
		/// <remarks>Construct an error for an object.</remarks>
		/// <param name="otp">the object whose current representation is no longer present.</param>
		public StoredObjectRepresentationNotAvailableException(ObjectToPack otp)
		{
		}
		// Do nothing.
	}
}
