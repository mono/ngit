using System;
using System.Net;
using System.Net.Sockets;
using NSch;
using Sharpen;

namespace NSch
{
	public class ProxySOCKS4 : Proxy
	{
		private static int DEFAULTPORT = 1080;

		private string proxy_host;

		private int proxy_port;

		private InputStream @in;

		private OutputStream @out;

		private Socket socket;

		private string user;

		private string passwd;

		public ProxySOCKS4(string proxy_host)
		{
			int port = DEFAULTPORT;
			string host = proxy_host;
			if (proxy_host.IndexOf(':') != -1)
			{
				try
				{
					host = Sharpen.Runtime.Substring(proxy_host, 0, proxy_host.IndexOf(':'));
					port = System.Convert.ToInt32(Sharpen.Runtime.Substring(proxy_host, proxy_host.IndexOf
						(':') + 1));
				}
				catch (Exception)
				{
				}
			}
			this.proxy_host = host;
			this.proxy_port = port;
		}

		public ProxySOCKS4(string proxy_host, int proxy_port)
		{
			this.proxy_host = proxy_host;
			this.proxy_port = proxy_port;
		}

		public virtual void SetUserPasswd(string user, string passwd)
		{
			this.user = user;
			this.passwd = passwd;
		}

		/// <exception cref="NSch.JSchException"></exception>
		public virtual void Connect(SocketFactory socket_factory, string host, int port, 
			int timeout)
		{
			try
			{
				if (socket_factory == null)
				{
					socket = Util.CreateSocket(proxy_host, proxy_port, timeout);
					//socket=new Socket(proxy_host, proxy_port);    
					@in = socket.GetInputStream();
					@out = socket.GetOutputStream();
				}
				else
				{
					socket = socket_factory.CreateSocket(proxy_host, proxy_port);
					@in = socket_factory.GetInputStream(socket);
					@out = socket_factory.GetOutputStream(socket);
				}
				if (timeout > 0)
				{
					socket.ReceiveTimeout = timeout;
				}
				socket.NoDelay = true;
				byte[] buf = new byte[1024];
				int index = 0;
				index = 0;
				buf[index++] = 4;
				buf[index++] = 1;
				buf[index++] = unchecked((byte)((int)(((uint)port) >> 8)));
				buf[index++] = unchecked((byte)(port & unchecked((int)(0xff))));
				try
				{
					IPAddress addr = Sharpen.Extensions.GetAddressByName(host);
					byte[] byteAddress = addr.GetAddressBytes();
					for (int i = 0; i < byteAddress.Length; i++)
					{
						buf[index++] = byteAddress[i];
					}
				}
				catch (UnknownHostException uhe)
				{
					throw new JSchException("ProxySOCKS4: " + uhe.ToString(), uhe);
				}
				if (user != null)
				{
					System.Array.Copy(Util.Str2byte(user), 0, buf, index, user.Length);
					index += user.Length;
				}
				buf[index++] = 0;
				@out.Write(buf, 0, index);
				int len = 8;
				int s = 0;
				while (s < len)
				{
					int i = @in.Read(buf, s, len - s);
					if (i <= 0)
					{
						throw new JSchException("ProxySOCKS4: stream is closed");
					}
					s += i;
				}
				if (buf[0] != 0)
				{
					throw new JSchException("ProxySOCKS4: server returns VN " + buf[0]);
				}
				if (buf[1] != 90)
				{
					try
					{
						socket.Close();
					}
					catch (Exception)
					{
					}
					string message = "ProxySOCKS4: server returns CD " + buf[1];
					throw new JSchException(message);
				}
			}
			catch (RuntimeException e)
			{
				throw;
			}
			catch (Exception e)
			{
				try
				{
					if (socket != null)
					{
						socket.Close();
					}
				}
				catch (Exception)
				{
				}
				throw new JSchException("ProxySOCKS4: " + e.ToString());
			}
		}

		public virtual InputStream GetInputStream()
		{
			return @in;
		}

		public virtual OutputStream GetOutputStream()
		{
			return @out;
		}

		public virtual Socket GetSocket()
		{
			return socket;
		}

		public virtual void Close()
		{
			try
			{
				if (@in != null)
				{
					@in.Close();
				}
				if (@out != null)
				{
					@out.Close();
				}
				if (socket != null)
				{
					socket.Close();
				}
			}
			catch (Exception)
			{
			}
			@in = null;
			@out = null;
			socket = null;
		}

		public static int GetDefaultPort()
		{
			return DEFAULTPORT;
		}
	}
}
