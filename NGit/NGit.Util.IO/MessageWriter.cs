using System.IO;
using NGit;
using NGit.Util;
using Sharpen;

namespace NGit.Util.IO
{
	/// <summary>Combines messages from an OutputStream (hopefully in UTF-8) and a Writer.
	/// 	</summary>
	/// <remarks>
	/// Combines messages from an OutputStream (hopefully in UTF-8) and a Writer.
	/// <p>
	/// This class is primarily meant for
	/// <see cref="NGit.Transport.BaseConnection">NGit.Transport.BaseConnection</see>
	/// in contexts where a
	/// standard error stream from a command execution, as well as messages from a
	/// side-band channel, need to be combined together into a buffer to represent
	/// the complete set of messages from a remote repository.
	/// <p>
	/// Writes made to the writer are re-encoded as UTF-8 and interleaved into the
	/// buffer that
	/// <see cref="GetRawStream()">GetRawStream()</see>
	/// also writes to.
	/// <p>
	/// <see cref="ToString()">ToString()</see>
	/// returns all written data, after converting it to a String
	/// under the assumption of UTF-8 encoding.
	/// <p>
	/// Internally
	/// <see cref="NGit.Util.RawParseUtils.Decode(byte[])">NGit.Util.RawParseUtils.Decode(byte[])
	/// 	</see>
	/// is used by
	/// <code>toString()</code>
	/// tries to work out a reasonably correct character set for the raw data.
	/// </remarks>
	public class MessageWriter : TextWriter
	{
		private readonly ByteArrayOutputStream buf;

		private readonly OutputStreamWriter enc;

		/// <summary>Create an empty writer.</summary>
		/// <remarks>Create an empty writer.</remarks>
		public MessageWriter()
		{
			buf = new ByteArrayOutputStream();
			enc = new OutputStreamWriter(GetRawStream(), Constants.CHARSET);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Write(char[] cbuf, int off, int len)
		{
			lock (buf)
			{
				enc.Write(cbuf, off, len);
				enc.Flush();
			}
		}

		/// <returns>
		/// the underlying byte stream that character writes to this writer
		/// drop into. Writes to this stream should should be in UTF-8.
		/// </returns>
		public virtual OutputStream GetRawStream()
		{
			return buf;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Close()
		{
		}

		// Do nothing, we are buffered with no resources.
		/// <exception cref="System.IO.IOException"></exception>
		public override void Flush()
		{
		}

		// Do nothing, we are buffered with no resources.
		/// <returns>string version of all buffered data.</returns>
		public override string ToString()
		{
			return RawParseUtils.Decode(buf.ToByteArray());
		}
		
		public override System.Text.Encoding Encoding {
			get {
				return Constants.CHARSET;
			}
		}
	}
}
