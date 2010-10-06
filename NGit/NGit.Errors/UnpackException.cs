using System;
using System.IO;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Indicates a ReceivePack failure while scanning the pack stream.</summary>
	/// <remarks>Indicates a ReceivePack failure while scanning the pack stream.</remarks>
	[System.Serializable]
	public class UnpackException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Creates an exception with a root cause.</summary>
		/// <remarks>Creates an exception with a root cause.</remarks>
		/// <param name="why">the root cause of the unpacking failure.</param>
		public UnpackException(Exception why) : base(JGitText.Get().unpackException)
		{
			Sharpen.Extensions.InitCause(this, why);
		}
	}
}
