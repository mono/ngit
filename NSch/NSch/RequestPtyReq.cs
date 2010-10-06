using NSch;
using Sharpen;

namespace NSch
{
	internal class RequestPtyReq : Request
	{
		private string ttype = "vt100";

		private int tcol = 80;

		private int trow = 24;

		private int twp = 640;

		private int thp = 480;

		private byte[] terminal_mode = Util.empty;

		internal virtual void SetCode(string cookie)
		{
		}

		internal virtual void SetTType(string ttype)
		{
			this.ttype = ttype;
		}

		internal virtual void SetTerminalMode(byte[] terminal_mode)
		{
			this.terminal_mode = terminal_mode;
		}

		internal virtual void SetTSize(int tcol, int trow, int twp, int thp)
		{
			this.tcol = tcol;
			this.trow = trow;
			this.twp = twp;
			this.thp = thp;
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
			buf.PutString(Util.Str2byte("pty-req"));
			buf.PutByte(unchecked((byte)(WaitForReply() ? 1 : 0)));
			buf.PutString(Util.Str2byte(ttype));
			buf.PutInt(tcol);
			buf.PutInt(trow);
			buf.PutInt(twp);
			buf.PutInt(thp);
			buf.PutString(terminal_mode);
			Write(packet);
		}
	}
}
