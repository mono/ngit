using Sharpen;

namespace NSch
{
	public class Buffer
	{
		internal readonly byte[] tmp = new byte[4];

		internal byte[] buffer;

		internal int index;

		internal int s;

		public Buffer(int size)
		{
			buffer = new byte[size];
			index = 0;
			s = 0;
		}

		public Buffer(byte[] buffer)
		{
			this.buffer = buffer;
			index = 0;
			s = 0;
		}

		public Buffer() : this(1024 * 10 * 2)
		{
		}

		public virtual void PutByte(byte foo)
		{
			buffer[index++] = foo;
		}

		public virtual void PutByte(byte[] foo)
		{
			PutByte(foo, 0, foo.Length);
		}

		public virtual void PutByte(byte[] foo, int begin, int length)
		{
			System.Array.Copy(foo, begin, buffer, index, length);
			index += length;
		}

		public virtual void PutString(byte[] foo)
		{
			PutString(foo, 0, foo.Length);
		}

		public virtual void PutString(byte[] foo, int begin, int length)
		{
			PutInt(length);
			PutByte(foo, begin, length);
		}

		public virtual void PutInt(int val)
		{
			tmp[0] = unchecked((byte)((int)(((uint)val) >> 24)));
			tmp[1] = unchecked((byte)((int)(((uint)val) >> 16)));
			tmp[2] = unchecked((byte)((int)(((uint)val) >> 8)));
			tmp[3] = unchecked((byte)(val));
			System.Array.Copy(tmp, 0, buffer, index, 4);
			index += 4;
		}

		public virtual void PutLong(long val)
		{
			tmp[0] = unchecked((byte)((long)(((ulong)val) >> 56)));
			tmp[1] = unchecked((byte)((long)(((ulong)val) >> 48)));
			tmp[2] = unchecked((byte)((long)(((ulong)val) >> 40)));
			tmp[3] = unchecked((byte)((long)(((ulong)val) >> 32)));
			System.Array.Copy(tmp, 0, buffer, index, 4);
			tmp[0] = unchecked((byte)((long)(((ulong)val) >> 24)));
			tmp[1] = unchecked((byte)((long)(((ulong)val) >> 16)));
			tmp[2] = unchecked((byte)((long)(((ulong)val) >> 8)));
			tmp[3] = unchecked((byte)(val));
			System.Array.Copy(tmp, 0, buffer, index + 4, 4);
			index += 8;
		}

		internal virtual void Skip(int n)
		{
			index += n;
		}

		internal virtual void PutPad(int n)
		{
			while (n > 0)
			{
				buffer[index++] = unchecked((byte)0);
				n--;
			}
		}

		public virtual void PutMPInt(byte[] foo)
		{
			int i = foo.Length;
			if ((foo[0] & unchecked((int)(0x80))) != 0)
			{
				i++;
				PutInt(i);
				PutByte(unchecked((byte)0));
			}
			else
			{
				PutInt(i);
			}
			PutByte(foo);
		}

		public virtual int GetLength()
		{
			return index - s;
		}

		public virtual int GetOffSet()
		{
			return s;
		}

		public virtual void SetOffSet(int s)
		{
			this.s = s;
		}

		public virtual long GetLong()
		{
			long foo = GetInt() & unchecked((long)(0xffffffffL));
			foo = ((foo << 32)) | (GetInt() & unchecked((long)(0xffffffffL)));
			return foo;
		}

		public virtual int GetInt()
		{
			int foo = GetShort();
			foo = ((foo << 16) & unchecked((int)(0xffff0000))) | (GetShort() & unchecked((int
				)(0xffff)));
			return foo;
		}

		public virtual long GetUInt()
		{
			long foo = 0L;
			long bar = 0L;
			foo = GetByte();
			foo = ((foo << 8) & unchecked((int)(0xff00))) | (GetByte() & unchecked((int)(0xff
				)));
			bar = GetByte();
			bar = ((bar << 8) & unchecked((int)(0xff00))) | (GetByte() & unchecked((int)(0xff
				)));
			foo = ((foo << 16) & unchecked((int)(0xffff0000))) | (bar & unchecked((int)(0xffff
				)));
			return foo;
		}

		internal virtual int GetShort()
		{
			int foo = GetByte();
			foo = ((foo << 8) & unchecked((int)(0xff00))) | (GetByte() & unchecked((int)(0xff
				)));
			return foo;
		}

		public virtual int GetByte()
		{
			return (buffer[s++] & unchecked((int)(0xff)));
		}

		public virtual void GetByte(byte[] foo)
		{
			GetByte(foo, 0, foo.Length);
		}

		internal virtual void GetByte(byte[] foo, int start, int len)
		{
			System.Array.Copy(buffer, s, foo, start, len);
			s += len;
		}

		public virtual int GetByte(int len)
		{
			int foo = s;
			s += len;
			return foo;
		}

		public virtual byte[] GetMPInt()
		{
			int i = GetInt();
			byte[] foo = new byte[i];
			GetByte(foo, 0, i);
			return foo;
		}

		public virtual byte[] GetMPIntBits()
		{
			int bits = GetInt();
			int bytes = (bits + 7) / 8;
			byte[] foo = new byte[bytes];
			GetByte(foo, 0, bytes);
			if ((foo[0] & unchecked((int)(0x80))) != 0)
			{
				byte[] bar = new byte[foo.Length + 1];
				bar[0] = 0;
				// ??
				System.Array.Copy(foo, 0, bar, 1, foo.Length);
				foo = bar;
			}
			return foo;
		}

		public virtual byte[] GetString()
		{
			int i = GetInt();
			// uint32
			byte[] foo = new byte[i];
			GetByte(foo, 0, i);
			return foo;
		}

		internal virtual byte[] GetString(int[] start, int[] len)
		{
			int i = GetInt();
			start[0] = GetByte(i);
			len[0] = i;
			return buffer;
		}

		public virtual void Reset()
		{
			index = 0;
			s = 0;
		}

		public virtual void Shift()
		{
			if (s == 0)
			{
				return;
			}
			System.Array.Copy(buffer, s, buffer, 0, index - s);
			index = index - s;
			s = 0;
		}

		internal virtual void Rewind()
		{
			s = 0;
		}

		internal virtual byte GetCommand()
		{
			return buffer[5];
		}
	}
}
