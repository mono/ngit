using System;
using System.IO;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Cannot store an object in the object database.</summary>
	/// <remarks>
	/// Cannot store an object in the object database. This is a serious
	/// error that users need to be made aware of.
	/// </remarks>
	[System.Serializable]
	public class ObjectWritingException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Constructs an ObjectWritingException with the specified detail message.</summary>
		/// <remarks>Constructs an ObjectWritingException with the specified detail message.</remarks>
		/// <param name="s">message</param>
		public ObjectWritingException(string s) : base(s)
		{
		}

		/// <summary>Constructs an ObjectWritingException with the specified detail message.</summary>
		/// <remarks>Constructs an ObjectWritingException with the specified detail message.</remarks>
		/// <param name="s">message</param>
		/// <param name="cause">root cause exception</param>
		public ObjectWritingException(string s, Exception cause) : base(s)
		{
			Sharpen.Extensions.InitCause(this, cause);
		}
	}
}
