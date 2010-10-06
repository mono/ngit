using System;
using System.IO;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Cannot write a modified index.</summary>
	/// <remarks>
	/// Cannot write a modified index. This is a serious error that users need to be
	/// made aware of.
	/// </remarks>
	[System.Serializable]
	public class IndexWriteException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Constructs an IndexWriteException with the default message.</summary>
		/// <remarks>Constructs an IndexWriteException with the default message.</remarks>
		public IndexWriteException() : base(JGitText.Get().indexWriteException)
		{
		}

		/// <summary>Constructs an IndexWriteException with the specified detail message.</summary>
		/// <remarks>Constructs an IndexWriteException with the specified detail message.</remarks>
		/// <param name="s">message</param>
		public IndexWriteException(string s) : base(s)
		{
		}

		/// <summary>Constructs an IndexWriteException with the specified detail message.</summary>
		/// <remarks>Constructs an IndexWriteException with the specified detail message.</remarks>
		/// <param name="s">message</param>
		/// <param name="cause">root cause exception</param>
		public IndexWriteException(string s, Exception cause) : base(s)
		{
			Sharpen.Extensions.InitCause(this, cause);
		}
	}
}
