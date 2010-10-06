using NGit.Api.Errors;
using Sharpen;

namespace NGit.Api.Errors
{
	/// <summary>Exception thrown when a fetch command was called with an invalid remote</summary>
	[System.Serializable]
	public class InvalidRemoteException : GitAPIException
	{
		private const long serialVersionUID = 1L;

		/// <param name="msg"></param>
		public InvalidRemoteException(string msg) : base(msg)
		{
		}
	}
}
