using System.IO;
using System.Text;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Exception thrown if a conflict occurs during a merge checkout.</summary>
	/// <remarks>Exception thrown if a conflict occurs during a merge checkout.</remarks>
	[System.Serializable]
	public class CheckoutConflictException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Construct a CheckoutConflictException for the specified file</summary>
		/// <param name="file"></param>
		public CheckoutConflictException(string file) : base(MessageFormat.Format(JGitText
			.Get().checkoutConflictWithFile, file))
		{
		}

		/// <summary>Construct a CheckoutConflictException for the specified set of files</summary>
		/// <param name="files"></param>
		public CheckoutConflictException(string[] files) : base(MessageFormat.Format(JGitText
			.Get().checkoutConflictWithFiles, BuildList(files)))
		{
		}

		private static string BuildList(string[] files)
		{
			StringBuilder builder = new StringBuilder();
			foreach (string f in files)
			{
				builder.Append("\n");
				builder.Append(f);
			}
			return builder.ToString();
		}
	}
}
