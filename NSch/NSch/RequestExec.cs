using NSch;
using Sharpen;

namespace NSch
{
	internal class RequestExec : Request
	{
		private byte[] command = new byte[0];

		internal RequestExec(byte[] command)
		{
			this.command = command;
		}

		/// <exception cref="System.Exception"></exception>
		internal override void DoRequest(Session session, Channel channel)
		{
			base.DoRequest(session, channel);
			Buffer buf = new Buffer();
			Packet packet = new Packet(buf);
			// send
			// byte     SSH_MSG_CHANNEL_REQUEST(98)
			// uint32 recipient channel
			// string request type       // "exec"
			// boolean want reply        // 0
			// string command
			packet.Reset();
			buf.PutByte(unchecked((byte)Session.SSH_MSG_CHANNEL_REQUEST));
			buf.PutInt(channel.GetRecipient());
			buf.PutString(Util.Str2byte("exec"));
			buf.PutByte(unchecked((byte)(WaitForReply() ? 1 : 0)));
			buf.PutString(command);
			Write(packet);
		}
	}
}
