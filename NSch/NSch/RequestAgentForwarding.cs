using NSch;
using Sharpen;

namespace NSch
{
	internal class RequestAgentForwarding : Request
	{
		/// <exception cref="System.Exception"></exception>
		internal override void DoRequest(Session session, Channel channel)
		{
			base.DoRequest(session, channel);
			SetReply(false);
			Buffer buf = new Buffer();
			Packet packet = new Packet(buf);
			// byte      SSH_MSG_CHANNEL_REQUEST(98)
			// uint32 recipient channel
			// string request type        // "auth-agent-req@openssh.com"
			// boolean want reply         // 0
			packet.Reset();
			buf.PutByte(unchecked((byte)Session.SSH_MSG_CHANNEL_REQUEST));
			buf.PutInt(channel.GetRecipient());
			buf.PutString(Util.Str2byte("auth-agent-req@openssh.com"));
			buf.PutByte(unchecked((byte)(WaitForReply() ? 1 : 0)));
			Write(packet);
			session.agent_forwarding = true;
		}
	}
}
