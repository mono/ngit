using System;
using NSch;
using Sharpen;

namespace NSch
{
	public class ChannelExec : ChannelSession
	{
		internal byte[] command = new byte[0];

		/// <exception cref="NSch.JSchException"></exception>
		public override void Start()
		{
			Session _session = GetSession();
			try
			{
				SendRequests();
				Request request = new RequestExec(command);
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
					throw new JSchException("ChannelExec", (Exception)e);
				}
				throw new JSchException("ChannelExec");
			}
			if (io.@in != null)
			{
				thread = new Sharpen.Thread(this);
				thread.SetName("Exec thread " + _session.GetHost());
				if (_session.daemon_thread)
				{
					thread.SetDaemon(_session.daemon_thread);
				}
				thread.Start();
			}
		}

		public virtual void SetCommand(string command)
		{
			this.command = Util.Str2byte(command);
		}

		public virtual void SetCommand(byte[] command)
		{
			this.command = command;
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

		public virtual void SetErrStream(OutputStream @out, bool dontclose)
		{
			SetExtOutputStream(@out, dontclose);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual InputStream GetErrStream()
		{
			return GetExtInputStream();
		}
	}
}
