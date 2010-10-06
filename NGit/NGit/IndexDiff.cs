using System.Collections.Generic;
using NGit;
using NGit.Dircache;
using NGit.Revwalk;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// Compares the index, a tree, and the working directory
	/// Ignored files are not taken into account.
	/// </summary>
	/// <remarks>
	/// Compares the index, a tree, and the working directory
	/// Ignored files are not taken into account.
	/// The following information is retrieved:
	/// <li> added files
	/// <li> changed files
	/// <li> removed files
	/// <li> missing files
	/// <li> modified files
	/// <li> untracked files
	/// </remarks>
	public class IndexDiff
	{
		private const int TREE = 0;

		private const int INDEX = 1;

		private const int WORKDIR = 2;

		private readonly Repository repository;

		private readonly RevTree tree;

		private readonly WorkingTreeIterator initialWorkingTreeIterator;

		private HashSet<string> added = new HashSet<string>();

		private HashSet<string> changed = new HashSet<string>();

		private HashSet<string> removed = new HashSet<string>();

		private HashSet<string> missing = new HashSet<string>();

		private HashSet<string> modified = new HashSet<string>();

		private HashSet<string> untracked = new HashSet<string>();

		/// <summary>Construct an IndexDiff</summary>
		/// <param name="repository"></param>
		/// <param name="revstr">
		/// symbolic name e.g. HEAD
		/// An EmptyTreeIterator is used if <code>revstr</code> cannot be resolved.
		/// </param>
		/// <param name="workingTreeIterator">iterator for working directory</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public IndexDiff(Repository repository, string revstr, WorkingTreeIterator workingTreeIterator
			)
		{
			this.repository = repository;
			ObjectId objectId = repository.Resolve(revstr);
			if (objectId != null)
			{
				tree = new RevWalk(repository).ParseTree(objectId);
			}
			else
			{
				tree = null;
			}
			this.initialWorkingTreeIterator = workingTreeIterator;
		}

		/// <summary>Construct an Indexdiff</summary>
		/// <param name="repository"></param>
		/// <param name="objectId">tree id. If null, an EmptyTreeIterator is used.</param>
		/// <param name="workingTreeIterator">iterator for working directory</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public IndexDiff(Repository repository, ObjectId objectId, WorkingTreeIterator workingTreeIterator
			)
		{
			this.repository = repository;
			if (objectId != null)
			{
				tree = new RevWalk(repository).ParseTree(objectId);
			}
			else
			{
				tree = null;
			}
			this.initialWorkingTreeIterator = workingTreeIterator;
		}

		/// <summary>Run the diff operation.</summary>
		/// <remarks>Run the diff operation. Until this is called, all lists will be empty</remarks>
		/// <returns>if anything is different between index, tree, and workdir</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual bool Diff()
		{
			bool changesExist = false;
			DirCache dirCache = repository.ReadDirCache();
			TreeWalk treeWalk = new TreeWalk(repository);
			treeWalk.Reset();
			treeWalk.Recursive = true;
			// add the trees (tree, dirchache, workdir)
			if (tree != null)
			{
				treeWalk.AddTree(tree);
			}
			else
			{
				treeWalk.AddTree(new EmptyTreeIterator());
			}
			treeWalk.AddTree(new DirCacheIterator(dirCache));
			treeWalk.AddTree(initialWorkingTreeIterator);
			treeWalk.Filter = TreeFilter.ANY_DIFF;
			treeWalk.Filter = AndTreeFilter.Create(new TreeFilter[] { new NotIgnoredFilter(WORKDIR
				), new SkipWorkTreeFilter(INDEX), TreeFilter.ANY_DIFF });
			while (treeWalk.Next())
			{
				AbstractTreeIterator treeIterator = treeWalk.GetTree<AbstractTreeIterator>(TREE);
				DirCacheIterator dirCacheIterator = treeWalk.GetTree<DirCacheIterator>(INDEX);
				WorkingTreeIterator workingTreeIterator = treeWalk.GetTree<WorkingTreeIterator>(WORKDIR
					);
				FileMode fileModeTree = treeWalk.GetFileMode(TREE);
				if (treeIterator != null)
				{
					if (dirCacheIterator != null)
					{
						if (!treeIterator.GetEntryObjectId().Equals(dirCacheIterator.GetEntryObjectId()))
						{
							// in repo, in index, content diff => changed
							changed.AddItem(dirCacheIterator.GetEntryPathString());
							changesExist = true;
						}
					}
					else
					{
						// in repo, not in index => removed
						if (!fileModeTree.Equals(FileMode.TYPE_TREE))
						{
							removed.AddItem(treeIterator.GetEntryPathString());
							changesExist = true;
						}
					}
				}
				else
				{
					if (dirCacheIterator != null)
					{
						// not in repo, in index => added
						added.AddItem(dirCacheIterator.GetEntryPathString());
						changesExist = true;
					}
					else
					{
						// not in repo, not in index => untracked
						if (workingTreeIterator != null && !workingTreeIterator.IsEntryIgnored())
						{
							untracked.AddItem(workingTreeIterator.GetEntryPathString());
							changesExist = true;
						}
					}
				}
				if (dirCacheIterator != null)
				{
					if (workingTreeIterator == null)
					{
						// in index, not in workdir => missing
						missing.AddItem(dirCacheIterator.GetEntryPathString());
						changesExist = true;
					}
					else
					{
						if (!dirCacheIterator.IdEqual(workingTreeIterator))
						{
							// in index, in workdir, content differs => modified
							modified.AddItem(dirCacheIterator.GetEntryPathString());
							changesExist = true;
						}
					}
				}
			}
			return changesExist;
		}

		/// <returns>list of files added to the index, not in the tree</returns>
		public virtual HashSet<string> GetAdded()
		{
			return added;
		}

		/// <returns>list of files changed from tree to index</returns>
		public virtual HashSet<string> GetChanged()
		{
			return changed;
		}

		/// <returns>list of files removed from index, but in tree</returns>
		public virtual HashSet<string> GetRemoved()
		{
			return removed;
		}

		/// <returns>list of files in index, but not filesystem</returns>
		public virtual HashSet<string> GetMissing()
		{
			return missing;
		}

		/// <returns>list of files on modified on disk relative to the index</returns>
		public virtual HashSet<string> GetModified()
		{
			return modified;
		}

		/// <returns>list of files on modified on disk relative to the index</returns>
		public virtual HashSet<string> GetUntracked()
		{
			return untracked;
		}
	}
}
