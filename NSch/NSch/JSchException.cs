using System;
using Sharpen;

namespace NSch
{
	[System.Serializable]
	public class JSchException : Exception
	{
		public JSchException() : base()
		{
		}

		public JSchException(string s) : base(s)
		{
		}

		public JSchException(string s, Exception e) : base(s, e)
		{
		}
	}
}
