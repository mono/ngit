using NSch;
using Sharpen;

namespace NSch
{
	public class RequestSubsystem : NSch.Request
	{
		private string subsystem = null;

		/// <exception cref="System.Exception"></exception>
		public virtual void Request(Session session, Channel channel, string subsystem, bool
			 want_reply)
		{
			SetReply(want_reply);
			this.subsystem = subsystem;
			this.DoRequest(session, channel);
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
			buf.PutString(Util.Str2byte("subsystem"));
			buf.PutByte(unchecked((byte)(WaitForReply() ? 1 : 0)));
			buf.PutString(Util.Str2byte(subsystem));
			Write(packet);
		}
	}
}
