using NSch;
using Sharpen;

namespace NSch
{
	internal class RequestX11 : Request
	{
		public virtual void SetCookie(string cookie)
		{
			ChannelX11.cookie = Util.Str2byte(cookie);
		}

		/// <exception cref="System.Exception"></exception>
		internal override void DoRequest(Session session, Channel channel)
		{
			base.DoRequest(session, channel);
			Buffer buf = new Buffer();
			Packet packet = new Packet(buf);
			// byte      SSH_MSG_CHANNEL_REQUEST(98)
			// uint32 recipient channel
			// string request type        // "x11-req"
			// boolean want reply         // 0
			// boolean   single connection
			// string    x11 authentication protocol // "MIT-MAGIC-COOKIE-1".
			// string    x11 authentication cookie
			// uint32    x11 screen number
			packet.Reset();
			buf.PutByte(unchecked((byte)Session.SSH_MSG_CHANNEL_REQUEST));
			buf.PutInt(channel.GetRecipient());
			buf.PutString(Util.Str2byte("x11-req"));
			buf.PutByte(unchecked((byte)(WaitForReply() ? 1 : 0)));
			buf.PutByte(unchecked((byte)0));
			buf.PutString(Util.Str2byte("MIT-MAGIC-COOKIE-1"));
			buf.PutString(ChannelX11.GetFakedCookie(session));
			buf.PutInt(0);
			Write(packet);
			session.x11_forwarding = true;
		}
	}
}
