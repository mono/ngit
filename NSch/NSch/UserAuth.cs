using NSch;
using Sharpen;

namespace NSch
{
	public abstract class UserAuth
	{
		protected internal const int SSH_MSG_USERAUTH_REQUEST = 50;

		protected internal const int SSH_MSG_USERAUTH_FAILURE = 51;

		protected internal const int SSH_MSG_USERAUTH_SUCCESS = 52;

		protected internal const int SSH_MSG_USERAUTH_BANNER = 53;

		protected internal const int SSH_MSG_USERAUTH_INFO_REQUEST = 60;

		protected internal const int SSH_MSG_USERAUTH_INFO_RESPONSE = 61;

		protected internal const int SSH_MSG_USERAUTH_PK_OK = 60;

		protected internal UserInfo userinfo;

		protected internal Packet packet;

		protected internal Buffer buf;

		protected internal string username;

		/// <exception cref="System.Exception"></exception>
		public virtual bool Start(Session session)
		{
			this.userinfo = session.GetUserInfo();
			this.packet = session.packet;
			this.buf = packet.GetBuffer();
			this.username = session.GetUserName();
			return true;
		}
	}
}
