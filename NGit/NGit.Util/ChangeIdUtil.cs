using System.Text;
using NGit;
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	/// <summary>
	/// Utilities for creating and working with Change-Id's, like the one used by
	/// Gerrit Code Review.
	/// </summary>
	/// <remarks>
	/// Utilities for creating and working with Change-Id's, like the one used by
	/// Gerrit Code Review.
	/// <p>
	/// A Change-Id is a SHA-1 computed from the content of a commit, in a similar
	/// fashion to how the commit id is computed. Unlike the commit id a Change-Id is
	/// retained in the commit and subsequent revised commits in the footer of the
	/// commit text.
	/// </remarks>
	public class ChangeIdUtil
	{
		internal static readonly string CHANGE_ID = "Change-Id:";

		// package-private so the unit test can test this part only
		internal static string Clean(string msg)
		{
			return msg.ReplaceAll("(?i)(?m)^Signed-off-by:.*$\n?", string.Empty).ReplaceAll("(?m)^#.*$\n?"
				, string.Empty).ReplaceAll("(?m)\n\n\n+", "\\\n").ReplaceAll("\\n*$", string.Empty
				).ReplaceAll("(?s)\ndiff --git.*", string.Empty).Trim();
		}

		//
		//
		//
		//
		//
		//
		/// <summary>Compute a Change-Id.</summary>
		/// <remarks>Compute a Change-Id.</remarks>
		/// <param name="treeId">The id of the tree that would be committed</param>
		/// <param name="firstParentId">parent id of previous commit or null</param>
		/// <param name="author">
		/// the
		/// <see cref="NGit.PersonIdent">NGit.PersonIdent</see>
		/// for the presumed author and time
		/// </param>
		/// <param name="committer">
		/// the
		/// <see cref="NGit.PersonIdent">NGit.PersonIdent</see>
		/// for the presumed committer and time
		/// </param>
		/// <param name="message">The commit message</param>
		/// <returns>
		/// the change id SHA1 string (without the 'I') or null if the
		/// message is not complete enough
		/// </returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public static ObjectId ComputeChangeId(ObjectId treeId, ObjectId firstParentId, PersonIdent
			 author, PersonIdent committer, string message)
		{
			string cleanMessage = Clean(message);
			if (cleanMessage.Length == 0)
			{
				return null;
			}
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append(ObjectId.ToString(treeId));
			b.Append("\n");
			if (firstParentId != null)
			{
				b.Append("parent ");
				b.Append(ObjectId.ToString(firstParentId));
				b.Append("\n");
			}
			b.Append("author ");
			b.Append(author.ToExternalString());
			b.Append("\n");
			b.Append("committer ");
			b.Append(committer.ToExternalString());
			b.Append("\n\n");
			b.Append(cleanMessage);
			return new ObjectInserter.Formatter().IdFor(Constants.OBJ_COMMIT, Sharpen.Runtime.GetBytesForString
				(b.ToString(), Constants.CHARACTER_ENCODING));
		}

		private static readonly Sharpen.Pattern issuePattern = Sharpen.Pattern.Compile("^(Bug|Issue)[a-zA-Z0-9-]*:.*$"
			);

		private static readonly Sharpen.Pattern footerPattern = Sharpen.Pattern.Compile("(^[a-zA-Z0-9-]+:(?!//).*$)"
			);

		private static readonly Sharpen.Pattern includeInFooterPattern = Sharpen.Pattern.
			Compile("^[ \\[].*$");

		//
		/// <summary>Find the right place to insert a Change-Id and return it.</summary>
		/// <remarks>
		/// Find the right place to insert a Change-Id and return it.
		/// <p>
		/// The Change-Id is inserted before the first footer line but after a Bug
		/// line.
		/// </remarks>
		/// <param name="message"></param>
		/// <param name="changeId"></param>
		/// <returns>a commit message with an inserted Change-Id line</returns>
		public static string InsertId(string message, ObjectId changeId)
		{
			return InsertId(message, changeId, false);
		}

		/// <summary>Find the right place to insert a Change-Id and return it.</summary>
		/// <remarks>
		/// Find the right place to insert a Change-Id and return it.
		/// <p>
		/// If no Change-Id is found the Change-Id is inserted before
		/// the first footer line but after a Bug line.
		/// If Change-Id is found and replaceExisting is set to false,
		/// the message is unchanged.
		/// If Change-Id is found and replaceExisting is set to true,
		/// the Change-Id is replaced with
		/// <code>changeId</code>
		/// .
		/// </remarks>
		/// <param name="message"></param>
		/// <param name="changeId"></param>
		/// <param name="replaceExisting"></param>
		/// <returns>a commit message with an inserted Change-Id line</returns>
		public static string InsertId(string message, ObjectId changeId, bool replaceExisting
			)
		{
			if (message.IndexOf(CHANGE_ID) > 0)
			{
				if (replaceExisting)
				{
					int i = message.IndexOf(CHANGE_ID) + 10;
					while (message[i] == ' ')
					{
						i++;
					}
					string oldId = message.Length == (i + 40) ? Sharpen.Runtime.Substring(message, i)
						 : Sharpen.Runtime.Substring(message, i, i + 41);
					message = message.Replace(oldId, "I" + changeId.GetName());
				}
				return message;
			}
			string[] lines = message.Split("\n");
			int footerFirstLine = lines.Length;
			for (int i_1 = lines.Length - 1; i_1 > 1; --i_1)
			{
				if (footerPattern.Matcher(lines[i_1]).Matches())
				{
					footerFirstLine = i_1;
					continue;
				}
				if (footerFirstLine != lines.Length && lines[i_1].Length == 0)
				{
					break;
				}
				if (footerFirstLine != lines.Length && includeInFooterPattern.Matcher(lines[i_1])
					.Matches())
				{
					footerFirstLine = i_1 + 1;
					continue;
				}
				footerFirstLine = lines.Length;
				break;
			}
			int insertAfter = footerFirstLine;
			for (int i_2 = footerFirstLine; i_2 < lines.Length; ++i_2)
			{
				if (issuePattern.Matcher(lines[i_2]).Matches())
				{
					insertAfter = i_2 + 1;
					continue;
				}
				break;
			}
			StringBuilder ret = new StringBuilder();
			int i_3 = 0;
			for (; i_3 < insertAfter; ++i_3)
			{
				ret.Append(lines[i_3]);
				ret.Append("\n");
			}
			if (insertAfter == lines.Length && insertAfter == footerFirstLine)
			{
				ret.Append("\n");
			}
			ret.Append(CHANGE_ID);
			ret.Append(" I");
			ret.Append(ObjectId.ToString(changeId));
			ret.Append("\n");
			for (; i_3 < lines.Length; ++i_3)
			{
				ret.Append(lines[i_3]);
				ret.Append("\n");
			}
			return ret.ToString();
		}
	}
}
