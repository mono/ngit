using System;
using NSch;
using Sharpen;

namespace NSch
{
	public abstract class Request
	{
		private bool reply = false;

		private Session session = null;

		private Channel channel = null;

		/// <exception cref="System.Exception"></exception>
		internal virtual void DoRequest(Session session, Channel channel)
		{
			this.session = session;
			this.channel = channel;
			if (channel.connectTimeout > 0)
			{
				SetReply(true);
			}
		}

		internal virtual bool WaitForReply()
		{
			return reply;
		}

		internal virtual void SetReply(bool reply)
		{
			this.reply = reply;
		}

		/// <exception cref="System.Exception"></exception>
		internal virtual void Write(Packet packet)
		{
			if (reply)
			{
				channel.reply = -1;
			}
			session.Write(packet);
			if (reply)
			{
				long start = Runtime.CurrentTimeMillis();
				long timeout = channel.connectTimeout;
				while (channel.IsConnected() && channel.reply == -1)
				{
					try
					{
						Sharpen.Thread.Sleep(10);
					}
					catch (Exception)
					{
					}
					if (timeout > 0L && (Runtime.CurrentTimeMillis() - start) > timeout)
					{
						channel.reply = 0;
						throw new JSchException("channel request: timeout");
					}
				}
				if (channel.reply == 0)
				{
					throw new JSchException("failed to send channel request");
				}
			}
		}
	}
}
