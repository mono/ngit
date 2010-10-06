using NSch;
using Sharpen;

namespace NSch
{
	[System.Serializable]
	internal class JSchAuthCancelException : JSchException
	{
		internal string method;

		public JSchAuthCancelException() : base()
		{
		}

		public JSchAuthCancelException(string s) : base(s)
		{
			//private static final long serialVersionUID=3204965907117900987L;
			this.method = s;
		}

		public virtual string GetMethod()
		{
			return method;
		}
	}
}
