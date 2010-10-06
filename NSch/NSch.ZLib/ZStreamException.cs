using System.IO;
using Sharpen;

namespace NSch.ZLib
{
	[System.Serializable]
	public class ZStreamException : IOException
	{
		public ZStreamException() : base()
		{
		}

		public ZStreamException(string s) : base(s)
		{
		}
	}
}
