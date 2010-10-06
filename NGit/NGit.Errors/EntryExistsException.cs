using System.IO;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>Attempt to add an entry to a tree that already exists.</summary>
	/// <remarks>Attempt to add an entry to a tree that already exists.</remarks>
	[System.Serializable]
	public class EntryExistsException : IOException
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Construct an EntryExistsException when the specified name already
		/// exists in a tree.
		/// </summary>
		/// <remarks>
		/// Construct an EntryExistsException when the specified name already
		/// exists in a tree.
		/// </remarks>
		/// <param name="name">workdir relative file name</param>
		public EntryExistsException(string name) : base(MessageFormat.Format(JGitText.Get
			().treeEntryAlreadyExists, name))
		{
		}
	}
}
