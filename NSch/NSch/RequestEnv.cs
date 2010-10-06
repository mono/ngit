using NSch;
using Sharpen;

namespace NSch
{
	internal class RequestEnv : Request
	{
		internal byte[] name = new byte[0];

		internal byte[] value = new byte[0];

		internal virtual void SetEnv(byte[] name, byte[] value)
		{
			this.name = name;
			this.value = value;
		}

		/// <exception cref="System.Exception"></exception>
		internal override void DoRequest(Session session, Channel channel)
		{
			base.DoRequest(session, channel);
			Buffer buf = new Buffer();
			Packet packet = new Packet(buf);
			packet.Reset();
			buf.PutByte(unchecked((byte)Session.SSH_MSG_CHANNEL_REQUEST));
			buf.PutInt(channel.GetRecipient());
			buf.PutString(Util.Str2byte("env"));
			buf.PutByte(unchecked((byte)(WaitForReply() ? 1 : 0)));
			buf.PutString(name);
			buf.PutString(value);
			Write(packet);
		}
	}
}
