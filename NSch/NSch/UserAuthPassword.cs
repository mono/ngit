using NSch;
using Sharpen;

namespace NSch
{
	internal class UserAuthPassword : UserAuth
	{
		private readonly int SSH_MSG_USERAUTH_PASSWD_CHANGEREQ = 60;

		/// <exception cref="System.Exception"></exception>
		public override bool Start(Session session)
		{
			base.Start(session);
			byte[] password = session.password;
			string dest = username + "@" + session.host;
			if (session.port != 22)
			{
				dest += (":" + session.port);
			}
			try
			{
				while (true)
				{
					if (password == null)
					{
						if (userinfo == null)
						{
							//throw new JSchException("USERAUTH fail");
							return false;
						}
						if (!userinfo.PromptPassword("Password for " + dest))
						{
							throw new JSchAuthCancelException("password");
						}
						//break;
						string _password = userinfo.GetPassword();
						if (_password == null)
						{
							throw new JSchAuthCancelException("password");
						}
						//break;
						password = Util.Str2byte(_password);
					}
					byte[] _username = null;
					_username = Util.Str2byte(username);
					// send
					// byte      SSH_MSG_USERAUTH_REQUEST(50)
					// string    user name
					// string    service name ("ssh-connection")
					// string    "password"
					// boolen    FALSE
					// string    plaintext password (ISO-10646 UTF-8)
					packet.Reset();
					buf.PutByte(unchecked((byte)SSH_MSG_USERAUTH_REQUEST));
					buf.PutString(_username);
					buf.PutString(Util.Str2byte("ssh-connection"));
					buf.PutString(Util.Str2byte("password"));
					buf.PutByte(unchecked((byte)0));
					buf.PutString(password);
					session.Write(packet);
					while (true)
					{
						buf = session.Read(buf);
						int command = buf.GetCommand() & unchecked((int)(0xff));
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
								userinfo.ShowMessage(message);
							}
							goto loop_continue;
						}
						if (command == SSH_MSG_USERAUTH_PASSWD_CHANGEREQ)
						{
							buf.GetInt();
							buf.GetByte();
							buf.GetByte();
							byte[] instruction = buf.GetString();
							byte[] tag = buf.GetString();
							if (userinfo == null || !(userinfo is UIKeyboardInteractive))
							{
								if (userinfo != null)
								{
									userinfo.ShowMessage("Password must be changed.");
								}
								return false;
							}
							UIKeyboardInteractive kbi = (UIKeyboardInteractive)userinfo;
							string[] response;
							string name = "Password Change Required";
							string[] prompt = new string[] { "New Password: " };
							bool[] echo = new bool[] { false };
							response = kbi.PromptKeyboardInteractive(dest, name, Util.Byte2str(instruction), 
								prompt, echo);
							if (response == null)
							{
								throw new JSchAuthCancelException("password");
							}
							byte[] newpassword = Util.Str2byte(response[0]);
							// send
							// byte      SSH_MSG_USERAUTH_REQUEST(50)
							// string    user name
							// string    service name ("ssh-connection")
							// string    "password"
							// boolen    TRUE
							// string    plaintext old password (ISO-10646 UTF-8)
							// string    plaintext new password (ISO-10646 UTF-8)
							packet.Reset();
							buf.PutByte(unchecked((byte)SSH_MSG_USERAUTH_REQUEST));
							buf.PutString(_username);
							buf.PutString(Util.Str2byte("ssh-connection"));
							buf.PutString(Util.Str2byte("password"));
							buf.PutByte(unchecked((byte)1));
							buf.PutString(password);
							buf.PutString(newpassword);
							Util.Bzero(newpassword);
							response = null;
							session.Write(packet);
							goto loop_continue;
						}
						if (command == SSH_MSG_USERAUTH_FAILURE)
						{
							buf.GetInt();
							buf.GetByte();
							buf.GetByte();
							byte[] foo = buf.GetString();
							int partial_success = buf.GetByte();
							//System.err.println(new String(foo)+
							//		 " partial_success:"+(partial_success!=0));
							if (partial_success != 0)
							{
								throw new JSchPartialAuthException(Util.Byte2str(foo));
							}
							break;
						}
						else
						{
							//System.err.println("USERAUTH fail ("+buf.getCommand()+")");
							//	  throw new JSchException("USERAUTH fail ("+buf.getCommand()+")");
							return false;
						}
loop_continue: ;
					}
loop_break: ;
					if (password != null)
					{
						Util.Bzero(password);
						password = null;
					}
				}
			}
			finally
			{
				if (password != null)
				{
					Util.Bzero(password);
					password = null;
				}
			}
		}
		//throw new JSchException("USERAUTH fail");
		//return false;
	}
}
