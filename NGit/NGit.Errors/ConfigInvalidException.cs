using System;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Indicates a text string is not a valid Git style configuration.</summary>
	/// <remarks>Indicates a text string is not a valid Git style configuration.</remarks>
	[System.Serializable]
	public class ConfigInvalidException : Exception
	{
		private const long serialVersionUID = 1L;

		/// <summary>Construct an invalid configuration error.</summary>
		/// <remarks>Construct an invalid configuration error.</remarks>
		/// <param name="message">why the configuration is invalid.</param>
		public ConfigInvalidException(string message) : base(message)
		{
		}

		/// <summary>Construct an invalid configuration error.</summary>
		/// <remarks>Construct an invalid configuration error.</remarks>
		/// <param name="message">why the configuration is invalid.</param>
		/// <param name="cause">root cause of the error.</param>
		public ConfigInvalidException(string message, Exception cause) : base(message, cause
			)
		{
		}
	}
}
