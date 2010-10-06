using System;
using NGit;
using NGit.Dircache;
using NGit.Errors;
using NGit.Treewalk;
using Sharpen;

namespace NGit.Dircache
{
	/// <summary>
	/// Iterate a
	/// <see cref="DirCache">DirCache</see>
	/// as part of a <code>TreeWalk</code>.
	/// <p>
	/// This is an iterator to adapt a loaded <code>DirCache</code> instance (such as
	/// read from an existing <code>.git/index</code> file) to the tree structure
	/// used by a <code>TreeWalk</code>, making it possible for applications to walk
	/// over any combination of tree objects already in the object database, index
	/// files, or working directories.
	/// </summary>
	/// <seealso cref="NGit.Treewalk.TreeWalk">NGit.Treewalk.TreeWalk</seealso>
	public class DirCacheIterator : AbstractTreeIterator
	{
		/// <summary>The cache this iterator was created to walk.</summary>
		/// <remarks>The cache this iterator was created to walk.</remarks>
		protected internal readonly DirCache cache;

		/// <summary>The tree this iterator is walking.</summary>
		/// <remarks>The tree this iterator is walking.</remarks>
		private readonly DirCacheTree tree;

		/// <summary>First position in this tree.</summary>
		/// <remarks>First position in this tree.</remarks>
		private readonly int treeStart;

		/// <summary>Last position in this tree.</summary>
		/// <remarks>Last position in this tree.</remarks>
		private readonly int treeEnd;

		/// <summary>
		/// Special buffer to hold the ObjectId of
		/// <see cref="currentSubtree">currentSubtree</see>
		/// .
		/// </summary>
		private readonly byte[] subtreeId;

		/// <summary>
		/// Index of entry within
		/// <see cref="cache">cache</see>
		/// .
		/// </summary>
		protected internal int ptr;

		/// <summary>
		/// Next subtree to consider within
		/// <see cref="tree">tree</see>
		/// .
		/// </summary>
		private int nextSubtreePos;

		/// <summary>
		/// The current file entry from
		/// <see cref="cache">cache</see>
		/// .
		/// </summary>
		protected internal DirCacheEntry currentEntry;

		/// <summary>
		/// The subtree containing
		/// <see cref="currentEntry">currentEntry</see>
		/// if this is first entry.
		/// </summary>
		protected internal DirCacheTree currentSubtree;

		/// <summary>Create a new iterator for an already loaded DirCache instance.</summary>
		/// <remarks>
		/// Create a new iterator for an already loaded DirCache instance.
		/// <p>
		/// The iterator implementation may copy part of the cache's data during
		/// construction, so the cache must be read in prior to creating the
		/// iterator.
		/// </remarks>
		/// <param name="dc">the cache to walk. It must be already loaded into memory.</param>
		public DirCacheIterator(DirCache dc)
		{
			cache = dc;
			tree = dc.GetCacheTree(true);
			treeStart = 0;
			treeEnd = tree.GetEntrySpan();
			subtreeId = new byte[Constants.OBJECT_ID_LENGTH];
			if (!Eof())
			{
				ParseEntry();
			}
		}

		internal DirCacheIterator(NGit.Dircache.DirCacheIterator p, DirCacheTree dct) : base
			(p, p.path, p.pathLen + 1)
		{
			cache = p.cache;
			tree = dct;
			treeStart = p.ptr;
			treeEnd = treeStart + tree.GetEntrySpan();
			subtreeId = p.subtreeId;
			ptr = p.ptr;
			ParseEntry();
		}

		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override AbstractTreeIterator CreateSubtreeIterator(ObjectReader reader)
		{
			if (currentSubtree == null)
			{
				throw new IncorrectObjectTypeException(GetEntryObjectId(), Constants.TYPE_TREE);
			}
			return new NGit.Dircache.DirCacheIterator(this, currentSubtree);
		}

		public override EmptyTreeIterator CreateEmptyTreeIterator()
		{
			byte[] n = new byte[Math.Max(pathLen + 1, DEFAULT_PATH_SIZE)];
			System.Array.Copy(path, 0, n, 0, pathLen);
			n[pathLen] = (byte)('/');
			return new EmptyTreeIterator(this, n, pathLen + 1);
		}

		public override bool HasId()
		{
			if (currentSubtree != null)
			{
				return currentSubtree.IsValid();
			}
			return currentEntry != null;
		}

		public override byte[] IdBuffer()
		{
			if (currentSubtree != null)
			{
				return currentSubtree.IsValid() ? subtreeId : zeroid;
			}
			if (currentEntry != null)
			{
				return currentEntry.IdBuffer();
			}
			return zeroid;
		}

		public override int IdOffset()
		{
			if (currentSubtree != null)
			{
				return 0;
			}
			if (currentEntry != null)
			{
				return currentEntry.IdOffset();
			}
			return 0;
		}

		public override void Reset()
		{
			if (!First())
			{
				ptr = treeStart;
				if (!Eof())
				{
					ParseEntry();
				}
			}
		}

		public override bool First()
		{
			return ptr == treeStart;
		}

		public override bool Eof()
		{
			return ptr == treeEnd;
		}

		public override void Next(int delta)
		{
			while (--delta >= 0)
			{
				if (currentSubtree != null)
				{
					ptr += currentSubtree.GetEntrySpan();
				}
				else
				{
					ptr++;
				}
				if (Eof())
				{
					break;
				}
				ParseEntry();
			}
		}

		public override void Back(int delta)
		{
			while (--delta >= 0)
			{
				if (currentSubtree != null)
				{
					nextSubtreePos--;
				}
				ptr--;
				ParseEntry();
				if (currentSubtree != null)
				{
					ptr -= currentSubtree.GetEntrySpan() - 1;
				}
			}
		}

		private void ParseEntry()
		{
			currentEntry = cache.GetEntry(ptr);
			byte[] cep = currentEntry.path;
			if (nextSubtreePos != tree.GetChildCount())
			{
				DirCacheTree s = tree.GetChild(nextSubtreePos);
				if (s.Contains(cep, pathOffset, cep.Length))
				{
					// The current position is the first file of this subtree.
					// Use the subtree instead as the current position.
					//
					currentSubtree = s;
					nextSubtreePos++;
					if (s.IsValid())
					{
						s.GetObjectId().CopyRawTo(subtreeId, 0);
					}
					mode = FileMode.TREE.GetBits();
					path = cep;
					pathLen = pathOffset + s.NameLength();
					return;
				}
			}
			// The current position is a file/symlink/gitlink so we
			// do not have a subtree located here.
			//
			mode = currentEntry.GetRawMode();
			path = cep;
			pathLen = cep.Length;
			currentSubtree = null;
		}

		/// <summary>Get the DirCacheEntry for the current file.</summary>
		/// <remarks>Get the DirCacheEntry for the current file.</remarks>
		/// <returns>
		/// the current cache entry, if this iterator is positioned on a
		/// non-tree.
		/// </returns>
		public virtual DirCacheEntry GetDirCacheEntry()
		{
			return currentSubtree == null ? currentEntry : null;
		}
	}
}
