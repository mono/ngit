using System;
using NSch;
using Sharpen;

namespace NSch
{
	public class ChannelDirectTCPIP : Channel
	{
		private const int LOCAL_WINDOW_SIZE_MAX = unchecked((int)(0x20000));

		private const int LOCAL_MAXIMUM_PACKET_SIZE = unchecked((int)(0x4000));

		internal string host;

		internal int port;

		internal string originator_IP_address = "127.0.0.1";

		internal int originator_port = 0;

		public ChannelDirectTCPIP() : base()
		{
			SetLocalWindowSizeMax(LOCAL_WINDOW_SIZE_MAX);
			SetLocalWindowSize(LOCAL_WINDOW_SIZE_MAX);
			SetLocalPacketSize(LOCAL_MAXIMUM_PACKET_SIZE);
		}

		internal override void Init()
		{
			try
			{
				io = new IO();
			}
			catch (Exception e)
			{
				System.Console.Error.WriteLine(e);
			}
		}

		/// <exception cref="NSch.JSchException"></exception>
		public override void Connect()
		{
			try
			{
				Session _session = GetSession();
				if (!_session.IsConnected())
				{
					throw new JSchException("session is down");
				}
				Buffer buf = new Buffer(150);
				Packet packet = new Packet(buf);
				// send
				// byte   SSH_MSG_CHANNEL_OPEN(90)
				// string channel type         //
				// uint32 sender channel       // 0
				// uint32 initial window size  // 0x100000(65536)
				// uint32 maxmum packet size   // 0x4000(16384)
				packet.Reset();
				buf.PutByte(unchecked((byte)90));
				buf.PutString(Util.Str2byte("direct-tcpip"));
				buf.PutInt(id);
				buf.PutInt(lwsize);
				buf.PutInt(lmpsize);
				buf.PutString(Util.Str2byte(host));
				buf.PutInt(port);
				buf.PutString(Util.Str2byte(originator_IP_address));
				buf.PutInt(originator_port);
				_session.Write(packet);
				int retry = 1000;
				try
				{
					while (this.GetRecipient() == -1 && _session.IsConnected() && retry > 0 && !eof_remote
						)
					{
						//Thread.sleep(500);
						Sharpen.Thread.Sleep(50);
						retry--;
					}
				}
				catch (Exception)
				{
				}
				if (!_session.IsConnected())
				{
					throw new JSchException("session is down");
				}
				if (retry == 0 || this.eof_remote)
				{
					throw new JSchException("channel is not opened.");
				}
				connected = true;
				if (io.@in != null)
				{
					thread = new Sharpen.Thread(this);
					thread.SetName("DirectTCPIP thread " + _session.GetHost());
					if (_session.daemon_thread)
					{
						thread.SetDaemon(_session.daemon_thread);
					}
					thread.Start();
				}
			}
			catch (Exception e)
			{
				io.Close();
				io = null;
				Channel.Del(this);
				if (e is JSchException)
				{
					throw (JSchException)e;
				}
			}
		}

		public override void Run()
		{
			Buffer buf = new Buffer(rmpsize);
			Packet packet = new Packet(buf);
			int i = 0;
			try
			{
				Session _session = GetSession();
				while (IsConnected() && thread != null && io != null && io.@in != null)
				{
					i = io.@in.Read(buf.buffer, 14, buf.buffer.Length - 14 - 32 - 20);
					// padding and mac
					if (i <= 0)
					{
						Eof();
						break;
					}
					if (close)
					{
						break;
					}
					packet.Reset();
					buf.PutByte(unchecked((byte)Session.SSH_MSG_CHANNEL_DATA));
					buf.PutInt(recipient);
					buf.PutInt(i);
					buf.Skip(i);
					_session.Write(packet, this, i);
				}
			}
			catch (Exception)
			{
			}
			Disconnect();
		}

		//System.err.println("connect end");
		public override void SetInputStream(InputStream @in)
		{
			io.SetInputStream(@in);
		}

		public override void SetOutputStream(OutputStream @out)
		{
			io.SetOutputStream(@out);
		}

		public virtual void SetHost(string host)
		{
			this.host = host;
		}

		public virtual void SetPort(int port)
		{
			this.port = port;
		}

		public virtual void SetOrgIPAddress(string foo)
		{
			this.originator_IP_address = foo;
		}

		public virtual void SetOrgPort(int foo)
		{
			this.originator_port = foo;
		}
	}
}
