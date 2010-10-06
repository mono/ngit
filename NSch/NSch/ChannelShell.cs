using System;
using NSch;
using Sharpen;

namespace NSch
{
	public class ChannelShell : ChannelSession
	{
		public ChannelShell() : base()
		{
			pty = true;
		}

		/// <exception cref="NSch.JSchException"></exception>
		public override void Start()
		{
			Session _session = GetSession();
			try
			{
				SendRequests();
				Request request = new RequestShell();
				request.DoRequest(_session, this);
			}
			catch (Exception e)
			{
				if (e is JSchException)
				{
					throw (JSchException)e;
				}
				if (e is Exception)
				{
					throw new JSchException("ChannelShell", (Exception)e);
				}
				throw new JSchException("ChannelShell");
			}
			if (io.@in != null)
			{
				thread = new Sharpen.Thread(this);
				thread.SetName("Shell for " + _session.host);
				if (_session.daemon_thread)
				{
					thread.SetDaemon(_session.daemon_thread);
				}
				thread.Start();
			}
		}

		/// <exception cref="NSch.JSchException"></exception>
		internal override void Init()
		{
			io.SetInputStream(GetSession().@in);
			io.SetOutputStream(GetSession().@out);
		}
	}
}
