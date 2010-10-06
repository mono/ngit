using System.Text;
using NGit;
using NGit.Patch;
using NGit.Util;
using Sharpen;

namespace NGit.Patch
{
	/// <summary>An error in a patch script</summary>
	public class FormatError
	{
		/// <summary>Classification of an error.</summary>
		/// <remarks>Classification of an error.</remarks>
		public enum Severity
		{
			WARNING,
			ERROR
		}

		private readonly byte[] buf;

		private readonly int offset;

		private readonly FormatError.Severity severity;

		private readonly string message;

		internal FormatError(byte[] buffer, int ptr, FormatError.Severity sev, string msg
			)
		{
			buf = buffer;
			offset = ptr;
			severity = sev;
			message = msg;
		}

		/// <returns>the severity of the error.</returns>
		public virtual FormatError.Severity GetSeverity()
		{
			return severity;
		}

		/// <returns>a message describing the error.</returns>
		public virtual string GetMessage()
		{
			return message;
		}

		/// <returns>the byte buffer holding the patch script.</returns>
		public virtual byte[] GetBuffer()
		{
			return buf;
		}

		/// <returns>
		/// byte offset within
		/// <see cref="GetBuffer()">GetBuffer()</see>
		/// where the error is
		/// </returns>
		public virtual int GetOffset()
		{
			return offset;
		}

		/// <returns>line of the patch script the error appears on.</returns>
		public virtual string GetLineText()
		{
			int eol = RawParseUtils.NextLF(buf, offset);
			return RawParseUtils.Decode(Constants.CHARSET, buf, offset, eol);
		}

		public override string ToString()
		{
			StringBuilder r = new StringBuilder();
			r.Append(GetSeverity().ToString().ToLower());
			r.Append(": at offset ");
			r.Append(GetOffset());
			r.Append(": ");
			r.Append(GetMessage());
			r.Append("\n");
			r.Append("  in ");
			r.Append(GetLineText());
			return r.ToString();
		}
	}
}
