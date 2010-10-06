using System.Collections.Generic;
using System.Text;
using NGit;
using NGit.Errors;
using NGit.Transport;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Indicates a base/common object was required, but is not found.</summary>
	/// <remarks>Indicates a base/common object was required, but is not found.</remarks>
	[System.Serializable]
	public class MissingBundlePrerequisiteException : TransportException
	{
		private const long serialVersionUID = 1L;

		private static string Format(IDictionary<ObjectId, string> missingCommits)
		{
			StringBuilder r = new StringBuilder();
			r.Append(JGitText.Get().missingPrerequisiteCommits);
			foreach (KeyValuePair<ObjectId, string> e in missingCommits.EntrySet())
			{
				r.Append("\n  ");
				r.Append(e.Key.Name);
				if (e.Value != null)
				{
					r.Append(" ").Append(e.Value);
				}
			}
			return r.ToString();
		}

		/// <summary>Constructs a MissingBundlePrerequisiteException for a set of objects.</summary>
		/// <remarks>Constructs a MissingBundlePrerequisiteException for a set of objects.</remarks>
		/// <param name="uri">URI used for transport</param>
		/// <param name="missingCommits">
		/// the Map of the base/common object(s) we don't have. Keys are
		/// ids of the missing objects and values are short descriptions.
		/// </param>
		public MissingBundlePrerequisiteException(URIish uri, IDictionary<ObjectId, string
			> missingCommits) : base(uri, Format(missingCommits))
		{
		}
	}
}
