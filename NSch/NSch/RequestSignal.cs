using NSch;
using Sharpen;

namespace NSch
{
	internal class RequestSignal : Request
	{
		private string signal = "KILL";

		public virtual void SetSignal(string foo)
		{
			signal = foo;
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
			buf.PutString(Util.Str2byte("signal"));
			buf.PutByte(unchecked((byte)(WaitForReply() ? 1 : 0)));
			buf.PutString(Util.Str2byte(signal));
			Write(packet);
		}
	}
}
