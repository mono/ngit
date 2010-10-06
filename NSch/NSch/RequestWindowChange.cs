using NSch;
using Sharpen;

namespace NSch
{
	internal class RequestWindowChange : Request
	{
		internal int width_columns = 80;

		internal int height_rows = 24;

		internal int width_pixels = 640;

		internal int height_pixels = 480;

		internal virtual void SetSize(int col, int row, int wp, int hp)
		{
			this.width_columns = col;
			this.height_rows = row;
			this.width_pixels = wp;
			this.height_pixels = hp;
		}

		/// <exception cref="System.Exception"></exception>
		internal override void DoRequest(Session session, Channel channel)
		{
			base.DoRequest(session, channel);
			Buffer buf = new Buffer();
			Packet packet = new Packet(buf);
			//byte      SSH_MSG_CHANNEL_REQUEST
			//uint32    recipient_channel
			//string    "window-change"
			//boolean   FALSE
			//uint32    terminal width, columns
			//uint32    terminal height, rows
			//uint32    terminal width, pixels
			//uint32    terminal height, pixels
			packet.Reset();
			buf.PutByte(unchecked((byte)Session.SSH_MSG_CHANNEL_REQUEST));
			buf.PutInt(channel.GetRecipient());
			buf.PutString(Util.Str2byte("window-change"));
			buf.PutByte(unchecked((byte)(WaitForReply() ? 1 : 0)));
			buf.PutInt(width_columns);
			buf.PutInt(height_rows);
			buf.PutInt(width_pixels);
			buf.PutInt(height_pixels);
			Write(packet);
		}
	}
}
