using System;
using System.IO;
using NSch;
using Sharpen;

namespace NSch
{
	public class IO
	{
		internal InputStream @in;

		internal OutputStream @out;

		internal OutputStream out_ext;

		private bool in_dontclose = false;

		private bool out_dontclose = false;

		private bool out_ext_dontclose = false;

		internal virtual void SetOutputStream(OutputStream @out)
		{
			this.@out = @out;
		}

		internal virtual void SetOutputStream(OutputStream @out, bool dontclose)
		{
			this.out_dontclose = dontclose;
			SetOutputStream(@out);
		}

		internal virtual void SetExtOutputStream(OutputStream @out)
		{
			this.out_ext = @out;
		}

		internal virtual void SetExtOutputStream(OutputStream @out, bool dontclose)
		{
			this.out_ext_dontclose = dontclose;
			SetExtOutputStream(@out);
		}

		internal virtual void SetInputStream(InputStream @in)
		{
			this.@in = @in;
		}

		internal virtual void SetInputStream(InputStream @in, bool dontclose)
		{
			this.in_dontclose = dontclose;
			SetInputStream(@in);
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="System.Net.Sockets.SocketException"></exception>
		public virtual void Put(Packet p)
		{
			@out.Write(p.buffer.buffer, 0, p.buffer.index);
			@out.Flush();
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual void Put(byte[] array, int begin, int length)
		{
			@out.Write(array, begin, length);
			@out.Flush();
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual void Put_ext(byte[] array, int begin, int length)
		{
			out_ext.Write(array, begin, length);
			out_ext.Flush();
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual int GetByte()
		{
			return @in.Read();
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual void GetByte(byte[] array)
		{
			GetByte(array, 0, array.Length);
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual void GetByte(byte[] array, int begin, int length)
		{
			do
			{
				int completed = @in.Read(array, begin, length);
				if (completed < 0)
				{
					throw new IOException("End of IO Stream Read");
				}
				begin += completed;
				length -= completed;
			}
			while (length > 0);
		}

		internal virtual void Out_close()
		{
			try
			{
				if (@out != null && !out_dontclose)
				{
					@out.Close();
				}
				@out = null;
			}
			catch (Exception)
			{
			}
		}

		public virtual void Close()
		{
			try
			{
				if (@in != null && !in_dontclose)
				{
					@in.Close();
				}
				@in = null;
			}
			catch (Exception)
			{
			}
			Out_close();
			try
			{
				if (out_ext != null && !out_ext_dontclose)
				{
					out_ext.Close();
				}
				out_ext = null;
			}
			catch (Exception)
			{
			}
		}
	}
}
