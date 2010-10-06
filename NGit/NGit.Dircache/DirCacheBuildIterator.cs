using NGit;
using NGit.Dircache;
using NGit.Errors;
using NGit.Treewalk;
using Sharpen;

namespace NGit.Dircache
{
	/// <summary>
	/// Iterate and update a
	/// <see cref="DirCache">DirCache</see>
	/// as part of a <code>TreeWalk</code>.
	/// <p>
	/// Like
	/// <see cref="DirCacheIterator">DirCacheIterator</see>
	/// this iterator allows a DirCache to be used in
	/// parallel with other sorts of iterators in a TreeWalk. However any entry which
	/// appears in the source DirCache and which is skipped by the TreeFilter is
	/// automatically copied into
	/// <see cref="DirCacheBuilder">DirCacheBuilder</see>
	/// , thus retaining it in the
	/// newly updated index.
	/// <p>
	/// This iterator is suitable for update processes, or even a simple delete
	/// algorithm. For example deleting a path:
	/// <pre>
	/// final DirCache dirc = DirCache.lock(db);
	/// final DirCacheBuilder edit = dirc.builder();
	/// final TreeWalk walk = new TreeWalk(db);
	/// walk.reset();
	/// walk.setRecursive(true);
	/// walk.setFilter(PathFilter.create(&quot;name/to/remove&quot;));
	/// walk.addTree(new DirCacheBuildIterator(edit));
	/// while (walk.next())
	/// ; // do nothing on a match as we want to remove matches
	/// edit.commit();
	/// </pre>
	/// </summary>
	public class DirCacheBuildIterator : DirCacheIterator
	{
		private readonly DirCacheBuilder builder;

		/// <summary>Create a new iterator for an already loaded DirCache instance.</summary>
		/// <remarks>
		/// Create a new iterator for an already loaded DirCache instance.
		/// <p>
		/// The iterator implementation may copy part of the cache's data during
		/// construction, so the cache must be read in prior to creating the
		/// iterator.
		/// </remarks>
		/// <param name="dcb">
		/// the cache builder for the cache to walk. The cache must be
		/// already loaded into memory.
		/// </param>
		public DirCacheBuildIterator(DirCacheBuilder dcb) : base(dcb.GetDirCache())
		{
			builder = dcb;
		}

		internal DirCacheBuildIterator(NGit.Dircache.DirCacheBuildIterator p, DirCacheTree
			 dct) : base(p, dct)
		{
			builder = p.builder;
		}

		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override AbstractTreeIterator CreateSubtreeIterator(ObjectReader reader)
		{
			if (currentSubtree == null)
			{
				throw new IncorrectObjectTypeException(GetEntryObjectId(), Constants.TYPE_TREE);
			}
			return new NGit.Dircache.DirCacheBuildIterator(this, currentSubtree);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		public override void Skip()
		{
			if (currentSubtree != null)
			{
				builder.Keep(ptr, currentSubtree.GetEntrySpan());
			}
			else
			{
				builder.Keep(ptr, 1);
			}
			Next(1);
		}

		public override void StopWalk()
		{
			int cur = ptr;
			int cnt = cache.GetEntryCount();
			if (cur < cnt)
			{
				builder.Keep(cur, cnt - cur);
			}
		}
	}
}
