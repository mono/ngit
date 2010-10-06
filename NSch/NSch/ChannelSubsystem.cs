using System;
using NSch;
using Sharpen;

namespace NSch
{
	public class ChannelSubsystem : ChannelSession
	{
		internal bool xforwading = false;

		internal bool pty = false;

		internal bool want_reply = true;

		internal string subsystem = string.Empty;

		public override void SetXForwarding(bool foo)
		{
			xforwading = true;
		}

		public override void SetPty(bool foo)
		{
			pty = foo;
		}

		public virtual void SetWantReply(bool foo)
		{
			want_reply = foo;
		}

		public virtual void SetSubsystem(string foo)
		{
			subsystem = foo;
		}

		/// <exception cref="NSch.JSchException"></exception>
		public override void Start()
		{
			Session _session = GetSession();
			try
			{
				Request request;
				if (xforwading)
				{
					request = new RequestX11();
					request.DoRequest(_session, this);
				}
				if (pty)
				{
					request = new RequestPtyReq();
					request.DoRequest(_session, this);
				}
				request = new RequestSubsystem();
				((RequestSubsystem)request).Request(_session, this, subsystem, want_reply);
			}
			catch (Exception e)
			{
				if (e is JSchException)
				{
					throw (JSchException)e;
				}
				if (e is Exception)
				{
					throw new JSchException("ChannelSubsystem", (Exception)e);
				}
				throw new JSchException("ChannelSubsystem");
			}
			if (io.@in != null)
			{
				thread = new Sharpen.Thread(this);
				thread.SetName("Subsystem for " + _session.host);
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

		public virtual void SetErrStream(OutputStream @out)
		{
			SetExtOutputStream(@out);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual InputStream GetErrStream()
		{
			return GetExtInputStream();
		}
	}
}
