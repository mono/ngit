using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// Implementation of IndexTreeVisitor that can be subclassed if you don't
	/// case about certain events
	/// </summary>
	/// <author>dwatson</author>
	[System.ObsoleteAttribute(@"Use  instead, with a as one of the members.")]
	public class AbstractIndexTreeVisitor : IndexTreeVisitor
	{
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void FinishVisitTree(Tree tree, Tree auxTree, string curDir)
		{
		}

		// Empty
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void FinishVisitTree(Tree tree, int i, string curDir)
		{
		}

		// Empty
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void VisitEntry(TreeEntry treeEntry, GitIndex.Entry indexEntry, FilePath
			 file)
		{
		}

		// Empty
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void VisitEntry(TreeEntry treeEntry, TreeEntry auxEntry, GitIndex.Entry
			 indexEntry, FilePath file)
		{
		}
		// Empty
	}
}
