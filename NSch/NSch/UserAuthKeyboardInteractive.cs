using NSch;
using Sharpen;

namespace NSch
{
	internal class UserAuthKeyboardInteractive : UserAuth
	{
		/// <exception cref="System.Exception"></exception>
		public override bool Start(Session session)
		{
			base.Start(session);
			if (userinfo != null && !(userinfo is UIKeyboardInteractive))
			{
				return false;
			}
			string dest = username + "@" + session.host;
			if (session.port != 22)
			{
				dest += (":" + session.port);
			}
			byte[] password = session.password;
			bool cancel = false;
			byte[] _username = null;
			_username = Util.Str2byte(username);
			while (true)
			{
				// send
				// byte      SSH_MSG_USERAUTH_REQUEST(50)
				// string    user name (ISO-10646 UTF-8, as defined in [RFC-2279])
				// string    service name (US-ASCII) "ssh-userauth" ? "ssh-connection"
				// string    "keyboard-interactive" (US-ASCII)
				// string    language tag (as defined in [RFC-3066])
				// string    submethods (ISO-10646 UTF-8)
				packet.Reset();
				buf.PutByte(unchecked((byte)SSH_MSG_USERAUTH_REQUEST));
				buf.PutString(_username);
				buf.PutString(Util.Str2byte("ssh-connection"));
				//buf.putString("ssh-userauth".getBytes());
				buf.PutString(Util.Str2byte("keyboard-interactive"));
				buf.PutString(Util.empty);
				buf.PutString(Util.empty);
				session.Write(packet);
				bool firsttime = true;
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
					if (command == SSH_MSG_USERAUTH_FAILURE)
					{
						buf.GetInt();
						buf.GetByte();
						buf.GetByte();
						byte[] foo = buf.GetString();
						int partial_success = buf.GetByte();
						//	  System.err.println(new String(foo)+
						//			     " partial_success:"+(partial_success!=0));
						if (partial_success != 0)
						{
							throw new JSchPartialAuthException(Util.Byte2str(foo));
						}
						if (firsttime)
						{
							return false;
						}
						//throw new JSchException("USERAUTH KI is not supported");
						//cancel=true;  // ??
						break;
					}
					if (command == SSH_MSG_USERAUTH_INFO_REQUEST)
					{
						firsttime = false;
						buf.GetInt();
						buf.GetByte();
						buf.GetByte();
						string name = Util.Byte2str(buf.GetString());
						string instruction = Util.Byte2str(buf.GetString());
						string languate_tag = Util.Byte2str(buf.GetString());
						int num = buf.GetInt();
						string[] prompt = new string[num];
						bool[] echo = new bool[num];
						for (int i = 0; i < num; i++)
						{
							prompt[i] = Util.Byte2str(buf.GetString());
							echo[i] = (buf.GetByte() != 0);
						}
						byte[][] response = null;
						if (password != null && prompt.Length == 1 && !echo[0] && prompt[0].ToLower().StartsWith
							("password:"))
						{
							response = new byte[1][];
							response[0] = password;
							password = null;
						}
						else
						{
							if (num > 0 || (name.Length > 0 || instruction.Length > 0))
							{
								if (userinfo != null)
								{
									UIKeyboardInteractive kbi = (UIKeyboardInteractive)userinfo;
									string[] _response = kbi.PromptKeyboardInteractive(dest, name, instruction, prompt
										, echo);
									if (_response != null)
									{
										response = new byte[_response.Length][];
										for (int i_1 = 0; i_1 < _response.Length; i_1++)
										{
											response[i_1] = Util.Str2byte(_response[i_1]);
										}
									}
								}
							}
						}
						// byte      SSH_MSG_USERAUTH_INFO_RESPONSE(61)
						// int       num-responses
						// string    response[1] (ISO-10646 UTF-8)
						// ...
						// string    response[num-responses] (ISO-10646 UTF-8)
						packet.Reset();
						buf.PutByte(unchecked((byte)SSH_MSG_USERAUTH_INFO_RESPONSE));
						if (num > 0 && (response == null || num != response.Length))
						{
							// cancel
							if (response == null)
							{
								// working around the bug in OpenSSH ;-<
								buf.PutInt(num);
								for (int i_1 = 0; i_1 < num; i_1++)
								{
									buf.PutString(Util.empty);
								}
							}
							else
							{
								buf.PutInt(0);
							}
							if (response == null)
							{
								cancel = true;
							}
						}
						else
						{
							buf.PutInt(num);
							for (int i_1 = 0; i_1 < num; i_1++)
							{
								buf.PutString(response[i_1]);
							}
						}
						session.Write(packet);
						goto loop_continue;
					}
					//throw new JSchException("USERAUTH fail ("+command+")");
					return false;
loop_continue: ;
				}
loop_break: ;
				if (cancel)
				{
					throw new JSchAuthCancelException("keyboard-interactive");
				}
			}
		}
		//break;
		//return false;
	}
}
