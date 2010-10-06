using System;
using System.Collections.Generic;
using System.IO;
using NGit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>Utility for reading reflog entries</summary>
	public class ReflogReader
	{
		/// <summary>Parsed reflog entry</summary>
		public class Entry
		{
			private ObjectId oldId;

			private ObjectId newId;

			private PersonIdent who;

			private string comment;

			internal Entry(byte[] raw, int pos)
			{
				oldId = ObjectId.FromString(raw, pos);
				pos += Constants.OBJECT_ID_STRING_LENGTH;
				if (raw[pos++] != ' ')
				{
					throw new ArgumentException(JGitText.Get().rawLogMessageDoesNotParseAsLogEntry);
				}
				newId = ObjectId.FromString(raw, pos);
				pos += Constants.OBJECT_ID_STRING_LENGTH;
				if (raw[pos++] != ' ')
				{
					throw new ArgumentException(JGitText.Get().rawLogMessageDoesNotParseAsLogEntry);
				}
				who = RawParseUtils.ParsePersonIdentOnly(raw, pos);
				int p0 = RawParseUtils.Next(raw, pos, '\t');
				// personident has no
				// \t
				if (p0 == -1)
				{
					throw new ArgumentException(JGitText.Get().rawLogMessageDoesNotParseAsLogEntry);
				}
				int p1 = RawParseUtils.NextLF(raw, p0);
				if (p1 == -1)
				{
					throw new ArgumentException(JGitText.Get().rawLogMessageDoesNotParseAsLogEntry);
				}
				comment = RawParseUtils.Decode(raw, p0, p1 - 1);
			}

			/// <returns>the commit id before the change</returns>
			public virtual ObjectId GetOldId()
			{
				return oldId;
			}

			/// <returns>the commit id after the change</returns>
			public virtual ObjectId GetNewId()
			{
				return newId;
			}

			/// <returns>user performin the change</returns>
			public virtual PersonIdent GetWho()
			{
				return who;
			}

			/// <returns>textual description of the change</returns>
			public virtual string GetComment()
			{
				return comment;
			}

			public override string ToString()
			{
				return "Entry[" + oldId.Name + ", " + newId.Name + ", " + GetWho() + ", " + GetComment
					() + "]";
			}
		}

		private FilePath logName;

		internal ReflogReader(Repository db, string refname)
		{
			logName = new FilePath(db.Directory, "logs/" + refname);
		}

		/// <summary>Get the last entry in the reflog</summary>
		/// <returns>the latest reflog entry, or null if no log</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual ReflogReader.Entry GetLastEntry()
		{
			IList<ReflogReader.Entry> entries = GetReverseEntries(1);
			return entries.Count > 0 ? entries[0] : null;
		}

		/// <returns>all reflog entries in reverse order</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual IList<ReflogReader.Entry> GetReverseEntries()
		{
			return GetReverseEntries(int.MaxValue);
		}

		/// <param name="max">max numer of entries to read</param>
		/// <returns>all reflog entries in reverse order</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual IList<ReflogReader.Entry> GetReverseEntries(int max)
		{
			byte[] log;
			try
			{
				log = IOUtil.ReadFully(logName);
			}
			catch (FileNotFoundException)
			{
				return Sharpen.Collections.EmptyList<ReflogReader.Entry>();
			}
			int rs = RawParseUtils.PrevLF(log, log.Length);
			IList<ReflogReader.Entry> ret = new AList<ReflogReader.Entry>();
			while (rs >= 0 && max-- > 0)
			{
				rs = RawParseUtils.PrevLF(log, rs);
				ReflogReader.Entry entry = new ReflogReader.Entry(log, rs < 0 ? 0 : rs + 2);
				ret.AddItem(entry);
			}
			return ret;
		}
	}
}
