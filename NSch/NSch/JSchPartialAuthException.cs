using NSch;
using Sharpen;

namespace NSch
{
	[System.Serializable]
	internal class JSchPartialAuthException : JSchException
	{
		internal string methods;

		public JSchPartialAuthException() : base()
		{
		}

		public JSchPartialAuthException(string s) : base(s)
		{
			//private static final long serialVersionUID=-378849862323360367L;
			this.methods = s;
		}

		public virtual string GetMethods()
		{
			return methods;
		}
	}
}
