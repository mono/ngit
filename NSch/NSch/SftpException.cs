using System;
using Sharpen;

namespace NSch
{
	[System.Serializable]
	public class SftpException : Exception
	{
		public int id;

		public SftpException(int id, string message) : base(message)
		{
			//private static final long serialVersionUID=-5616888495583253811L;
			this.id = id;
		}

		public SftpException(int id, string message, Exception e) : base(message, e)
		{
			this.id = id;
		}

		public override string ToString()
		{
			return id + ": " + Message;
		}
	}
}
