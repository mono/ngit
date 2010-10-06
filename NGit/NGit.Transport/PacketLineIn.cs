using System;
using System.IO;
using NGit;
using NGit.Errors;
using NGit.Transport;
using NGit.Util;
using Sharpen;

namespace NGit.Transport
{
	public class PacketLineIn
	{
		internal static readonly string END = Sharpen.Extensions.CreateString(string.Empty
			);

		internal enum AckNackResult
		{
			NAK,
			ACK,
			ACK_CONTINUE,
			ACK_COMMON,
			ACK_READY
		}

		private readonly InputStream @in;

		private readonly byte[] lineBuffer;

		internal PacketLineIn(InputStream i)
		{
			@in = i;
			lineBuffer = new byte[SideBandOutputStream.SMALL_BUF];
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual PacketLineIn.AckNackResult ReadACK(MutableObjectId returnedId)
		{
			string line = ReadString();
			if (line.Length == 0)
			{
				throw new PackProtocolException(JGitText.Get().expectedACKNAKFoundEOF);
			}
			if ("NAK".Equals(line))
			{
				return PacketLineIn.AckNackResult.NAK;
			}
			if (line.StartsWith("ACK "))
			{
				returnedId.FromString(Sharpen.Runtime.Substring(line, 4, 44));
				if (line.Length == 44)
				{
					return PacketLineIn.AckNackResult.ACK;
				}
				string arg = Sharpen.Runtime.Substring(line, 44);
				if (arg.Equals(" continue"))
				{
					return PacketLineIn.AckNackResult.ACK_CONTINUE;
				}
				else
				{
					if (arg.Equals(" common"))
					{
						return PacketLineIn.AckNackResult.ACK_COMMON;
					}
					else
					{
						if (arg.Equals(" ready"))
						{
							return PacketLineIn.AckNackResult.ACK_READY;
						}
					}
				}
			}
			throw new PackProtocolException(MessageFormat.Format(JGitText.Get().expectedACKNAKGot
				, line));
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual string ReadString()
		{
			int len = ReadLength();
			if (len == 0)
			{
				return END;
			}
			len -= 4;
			// length header (4 bytes)
			if (len == 0)
			{
				return string.Empty;
			}
			byte[] raw = new byte[len];
			IOUtil.ReadFully(@in, raw, 0, len);
			if (raw[len - 1] == '\n')
			{
				len--;
			}
			return RawParseUtils.Decode(Constants.CHARSET, raw, 0, len);
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual string ReadStringRaw()
		{
			int len = ReadLength();
			if (len == 0)
			{
				return END;
			}
			len -= 4;
			// length header (4 bytes)
			byte[] raw;
			if (len <= lineBuffer.Length)
			{
				raw = lineBuffer;
			}
			else
			{
				raw = new byte[len];
			}
			IOUtil.ReadFully(@in, raw, 0, len);
			return RawParseUtils.Decode(Constants.CHARSET, raw, 0, len);
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual int ReadLength()
		{
			IOUtil.ReadFully(@in, lineBuffer, 0, 4);
			try
			{
				int len = RawParseUtils.ParseHexInt16(lineBuffer, 0);
				if (len != 0 && len < 4)
				{
					throw new IndexOutOfRangeException();
				}
				return len;
			}
			catch (IndexOutOfRangeException)
			{
				throw new IOException(MessageFormat.Format(JGitText.Get().invalidPacketLineHeader
					, string.Empty + (char)lineBuffer[0] + (char)lineBuffer[1] + (char)lineBuffer[2]
					 + (char)lineBuffer[3]));
			}
		}
	}
}
