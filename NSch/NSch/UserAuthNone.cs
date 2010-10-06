using NSch;
using Sharpen;

namespace NSch
{
	internal class UserAuthNone : UserAuth
	{
		private const int SSH_MSG_SERVICE_ACCEPT = 6;

		private string methods = null;

		/// <exception cref="System.Exception"></exception>
		public override bool Start(Session session)
		{
			base.Start(session);
			// send
			// byte      SSH_MSG_SERVICE_REQUEST(5)
			// string    service name "ssh-userauth"
			packet.Reset();
			buf.PutByte(unchecked((byte)Session.SSH_MSG_SERVICE_REQUEST));
			buf.PutString(Util.Str2byte("ssh-userauth"));
			session.Write(packet);
			if (JSch.GetLogger().IsEnabled(Logger.INFO))
			{
				JSch.GetLogger().Log(Logger.INFO, "SSH_MSG_SERVICE_REQUEST sent");
			}
			// receive
			// byte      SSH_MSG_SERVICE_ACCEPT(6)
			// string    service name
			buf = session.Read(buf);
			int command = buf.GetCommand();
			bool result = (command == SSH_MSG_SERVICE_ACCEPT);
			if (JSch.GetLogger().IsEnabled(Logger.INFO))
			{
				JSch.GetLogger().Log(Logger.INFO, "SSH_MSG_SERVICE_ACCEPT received");
			}
			if (!result)
			{
				return false;
			}
			byte[] _username = null;
			_username = Util.Str2byte(username);
			// send
			// byte      SSH_MSG_USERAUTH_REQUEST(50)
			// string    user name
			// string    service name ("ssh-connection")
			// string    "none"
			packet.Reset();
			buf.PutByte(unchecked((byte)SSH_MSG_USERAUTH_REQUEST));
			buf.PutString(_username);
			buf.PutString(Util.Str2byte("ssh-connection"));
			buf.PutString(Util.Str2byte("none"));
			session.Write(packet);
			while (true)
			{
				buf = session.Read(buf);
				command = buf.GetCommand() & unchecked((int)(0xff));
				if (command == SSH_MSG_USERAUTH_SUCCESS)
				{
					return true;
				}
				if (command == SSH_MSG_USERAUTH_BANNER)
				{
					buf.GetInt();
					buf.GetByte();
					buf.GetByte();
					byte[] _message = buf.GetString();
					byte[] lang = buf.GetString();
					string message = Util.Byte2str(_message);
					if (userinfo != null)
					{
						try
						{
							userinfo.ShowMessage(message);
						}
						catch (RuntimeException)
						{
						}
					}
					goto loop_continue;
				}
				if (command == SSH_MSG_USERAUTH_FAILURE)
				{
					buf.GetInt();
					buf.GetByte();
					buf.GetByte();
					byte[] foo = buf.GetString();
					int partial_success = buf.GetByte();
					methods = Util.Byte2str(foo);
					//System.err.println("UserAuthNONE: "+methods+
					//		   " partial_success:"+(partial_success!=0));
					//	if(partial_success!=0){
					//	  throw new JSchPartialAuthException(new String(foo));
					//	}
					break;
				}
				else
				{
					//      System.err.println("USERAUTH fail ("+command+")");
					throw new JSchException("USERAUTH fail (" + command + ")");
				}
loop_continue: ;
			}
loop_break: ;
			//throw new JSchException("USERAUTH fail");
			return false;
		}

		internal virtual string GetMethods()
		{
			return methods;
		}
	}
}
