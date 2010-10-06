using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>Visitor interface for traversing the index and two trees in parallel.</summary>
	/// <remarks>
	/// Visitor interface for traversing the index and two trees in parallel.
	/// When merging we deal with up to two tree nodes and a base node. Then
	/// we figure out what to do.
	/// A File argument is supplied to allow us to check for modifications in
	/// a work tree or update the file.
	/// </remarks>
	[System.ObsoleteAttribute(@"Use  instead, with a  as a member.")]
	public interface IndexTreeVisitor
	{
		/// <summary>Visit a blob, and corresponding tree and index entries.</summary>
		/// <remarks>Visit a blob, and corresponding tree and index entries.</remarks>
		/// <param name="treeEntry"></param>
		/// <param name="indexEntry"></param>
		/// <param name="file"></param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void VisitEntry(TreeEntry treeEntry, GitIndex.Entry indexEntry, FilePath file);

		/// <summary>Visit a blob, and corresponding tree nodes and associated index entry.</summary>
		/// <remarks>Visit a blob, and corresponding tree nodes and associated index entry.</remarks>
		/// <param name="treeEntry"></param>
		/// <param name="auxEntry"></param>
		/// <param name="indexEntry"></param>
		/// <param name="file"></param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void VisitEntry(TreeEntry treeEntry, TreeEntry auxEntry, GitIndex.Entry indexEntry
			, FilePath file);

		/// <summary>Invoked after handling all child nodes of a tree, during a three way merge
		/// 	</summary>
		/// <param name="tree"></param>
		/// <param name="auxTree"></param>
		/// <param name="curDir"></param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void FinishVisitTree(Tree tree, Tree auxTree, string curDir);

		/// <summary>Invoked after handling all child nodes of a tree, during two way merge.</summary>
		/// <remarks>Invoked after handling all child nodes of a tree, during two way merge.</remarks>
		/// <param name="tree"></param>
		/// <param name="i"></param>
		/// <param name="curDir"></param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void FinishVisitTree(Tree tree, int i, string curDir);
	}
}
