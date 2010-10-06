using NSch.ZLib;
using Sharpen;

namespace NSch.ZLib
{
	public sealed class ZStream
	{
		private const int MAX_WBITS = 15;

		private const int DEF_WBITS = MAX_WBITS;

		private const int Z_NO_FLUSH = 0;

		private const int Z_PARTIAL_FLUSH = 1;

		private const int Z_SYNC_FLUSH = 2;

		private const int Z_FULL_FLUSH = 3;

		private const int Z_FINISH = 4;

		private const int MAX_MEM_LEVEL = 9;

		private const int Z_OK = 0;

		private const int Z_STREAM_END = 1;

		private const int Z_NEED_DICT = 2;

		private const int Z_ERRNO = -1;

		private const int Z_STREAM_ERROR = -2;

		private const int Z_DATA_ERROR = -3;

		private const int Z_MEM_ERROR = -4;

		private const int Z_BUF_ERROR = -5;

		private const int Z_VERSION_ERROR = -6;

		public byte[] next_in;

		public int next_in_index;

		public int avail_in;

		public long total_in;

		public byte[] next_out;

		public int next_out_index;

		public int avail_out;

		public long total_out;

		public string msg;

		internal NSch.ZLib.Deflate dstate;

		internal NSch.ZLib.Inflate istate;

		internal int data_type;

		public long adler;

		internal Adler32 _adler = new Adler32();

		// 32K LZ77 window
		// next input byte
		// number of bytes available at next_in
		// total nb of input bytes read so far
		// next output byte should be put there
		// remaining free space at next_out
		// total nb of bytes output so far
		// best guess about the data type: ascii or binary
		public int InflateInit()
		{
			return InflateInit(DEF_WBITS);
		}

		public int InflateInit(bool nowrap)
		{
			return InflateInit(DEF_WBITS, nowrap);
		}

		public int InflateInit(int w)
		{
			return InflateInit(w, false);
		}

		public int InflateInit(int w, bool nowrap)
		{
			istate = new NSch.ZLib.Inflate();
			return istate.InflateInit(this, nowrap ? -w : w);
		}

		public int Inflate(int f)
		{
			if (istate == null)
			{
				return Z_STREAM_ERROR;
			}
			return istate.DoInflate(this, f);
		}

		public int InflateEnd()
		{
			if (istate == null)
			{
				return Z_STREAM_ERROR;
			}
			int ret = istate.InflateEnd(this);
			istate = null;
			return ret;
		}

		public int InflateSync()
		{
			if (istate == null)
			{
				return Z_STREAM_ERROR;
			}
			return istate.InflateSync(this);
		}

		public int InflateSetDictionary(byte[] dictionary, int dictLength)
		{
			if (istate == null)
			{
				return Z_STREAM_ERROR;
			}
			return istate.InflateSetDictionary(this, dictionary, dictLength);
		}

		public int DeflateInit(int level)
		{
			return DeflateInit(level, MAX_WBITS);
		}

		public int DeflateInit(int level, bool nowrap)
		{
			return DeflateInit(level, MAX_WBITS, nowrap);
		}

		public int DeflateInit(int level, int bits)
		{
			return DeflateInit(level, bits, false);
		}

		public int DeflateInit(int level, int bits, bool nowrap)
		{
			dstate = new NSch.ZLib.Deflate();
			return dstate.DeflateInit(this, level, nowrap ? -bits : bits);
		}

		public int Deflate(int flush)
		{
			if (dstate == null)
			{
				return Z_STREAM_ERROR;
			}
			return dstate.DoDeflate(this, flush);
		}

		public int DeflateEnd()
		{
			if (dstate == null)
			{
				return Z_STREAM_ERROR;
			}
			int ret = dstate.DeflateEnd();
			dstate = null;
			return ret;
		}

		public int DeflateParams(int level, int strategy)
		{
			if (dstate == null)
			{
				return Z_STREAM_ERROR;
			}
			return dstate.DeflateParams(this, level, strategy);
		}

		public int DeflateSetDictionary(byte[] dictionary, int dictLength)
		{
			if (dstate == null)
			{
				return Z_STREAM_ERROR;
			}
			return dstate.DeflateSetDictionary(this, dictionary, dictLength);
		}

		// Flush as much pending output as possible. All deflate() output goes
		// through this function so some applications may wish to modify it
		// to avoid allocating a large strm->next_out buffer and copying into it.
		// (See also read_buf()).
		internal void Flush_pending()
		{
			int len = dstate.pending;
			if (len > avail_out)
			{
				len = avail_out;
			}
			if (len == 0)
			{
				return;
			}
			if (dstate.pending_buf.Length <= dstate.pending_out || next_out.Length <= next_out_index
				 || dstate.pending_buf.Length < (dstate.pending_out + len) || next_out.Length < 
				(next_out_index + len))
			{
				System.Console.Out.WriteLine(dstate.pending_buf.Length + ", " + dstate.pending_out
					 + ", " + next_out.Length + ", " + next_out_index + ", " + len);
				System.Console.Out.WriteLine("avail_out=" + avail_out);
			}
			System.Array.Copy(dstate.pending_buf, dstate.pending_out, next_out, next_out_index
				, len);
			next_out_index += len;
			dstate.pending_out += len;
			total_out += len;
			avail_out -= len;
			dstate.pending -= len;
			if (dstate.pending == 0)
			{
				dstate.pending_out = 0;
			}
		}

		// Read a new buffer from the current input stream, update the adler32
		// and total number of bytes read.  All deflate() input goes through
		// this function so some applications may wish to modify it to avoid
		// allocating a large strm->next_in buffer and copying from it.
		// (See also flush_pending()).
		internal int Read_buf(byte[] buf, int start, int size)
		{
			int len = avail_in;
			if (len > size)
			{
				len = size;
			}
			if (len == 0)
			{
				return 0;
			}
			avail_in -= len;
			if (dstate.noheader == 0)
			{
				adler = _adler.Adler(adler, next_in, next_in_index, len);
			}
			System.Array.Copy(next_in, next_in_index, buf, start, len);
			next_in_index += len;
			total_in += len;
			return len;
		}

		public void Free()
		{
			next_in = null;
			next_out = null;
			msg = null;
			_adler = null;
		}
	}
}
