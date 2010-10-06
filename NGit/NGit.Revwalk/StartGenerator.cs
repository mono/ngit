using System;
using NGit;
using NGit.Revwalk;
using NGit.Revwalk.Filter;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>Initial RevWalk generator that bootstraps a new walk.</summary>
	/// <remarks>
	/// Initial RevWalk generator that bootstraps a new walk.
	/// <p>
	/// Initially RevWalk starts with this generator as its chosen implementation.
	/// The first request for a RevCommit from the RevWalk instance calls to our
	/// <see cref="Next()">Next()</see>
	/// method, and we replace ourselves with the best Generator
	/// implementation available based upon the current RevWalk configuration.
	/// </remarks>
	internal class StartGenerator : Generator
	{
		private readonly RevWalk walker;

		internal StartGenerator(RevWalk w)
		{
			walker = w;
		}

		internal override int OutputType()
		{
			return 0;
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		internal override RevCommit Next()
		{
			Generator g;
			RevWalk w = walker;
			RevFilter rf = w.GetRevFilter();
			TreeFilter tf = w.GetTreeFilter();
			AbstractRevQueue q = walker.queue;
			w.reader.WalkAdviceBeginCommits(w, w.roots);
			if (rf == RevFilter.MERGE_BASE)
			{
				// Computing for merge bases is a special case and does not
				// use the bulk of the generator pipeline.
				//
				if (tf != TreeFilter.ALL)
				{
					throw new InvalidOperationException(MessageFormat.Format(JGitText.Get().cannotCombineTreeFilterWithRevFilter
						, tf, rf));
				}
				MergeBaseGenerator mbg = new MergeBaseGenerator(w);
				walker.pending = mbg;
				walker.queue = AbstractRevQueue.EMPTY_QUEUE;
				mbg.Init(q);
				return mbg.Next();
			}
			bool uninteresting = q.AnybodyHasFlag(RevWalk.UNINTERESTING);
			bool boundary = walker.HasRevSort(RevSort.BOUNDARY);
			if (!boundary && walker is ObjectWalk)
			{
				// The object walker requires boundary support to color
				// trees and blobs at the boundary uninteresting so it
				// does not produce those in the result.
				//
				boundary = true;
			}
			if (boundary && !uninteresting)
			{
				// If we were not fed uninteresting commits we will never
				// construct a boundary. There is no reason to include the
				// extra overhead associated with that in our pipeline.
				//
				boundary = false;
			}
			DateRevQueue pending;
			int pendingOutputType = 0;
			if (q is DateRevQueue)
			{
				pending = (DateRevQueue)q;
			}
			else
			{
				pending = new DateRevQueue(q);
			}
			if (tf != TreeFilter.ALL)
			{
				rf = AndRevFilter.Create(rf, new RewriteTreeFilter(w, tf));
				pendingOutputType |= HAS_REWRITE | NEEDS_REWRITE;
			}
			walker.queue = q;
			g = new PendingGenerator(w, pending, rf, pendingOutputType);
			if (boundary)
			{
				// Because the boundary generator may produce uninteresting
				// commits we cannot allow the pending generator to dispose
				// of them early.
				//
				((PendingGenerator)g).canDispose = false;
			}
			if ((g.OutputType() & NEEDS_REWRITE) != 0)
			{
				// Correction for an upstream NEEDS_REWRITE is to buffer
				// fully and then apply a rewrite generator that can
				// pull through the rewrite chain and produce a dense
				// output graph.
				//
				g = new FIFORevQueue(g);
				g = new RewriteGenerator(g);
			}
			if (walker.HasRevSort(RevSort.TOPO) && (g.OutputType() & SORT_TOPO) == 0)
			{
				g = new TopoSortGenerator(g);
			}
			if (walker.HasRevSort(RevSort.REVERSE))
			{
				g = new LIFORevQueue(g);
			}
			if (boundary)
			{
				g = new BoundaryGenerator(w, g);
			}
			else
			{
				if (uninteresting)
				{
					// Try to protect ourselves from uninteresting commits producing
					// due to clock skew in the commit time stamps. Delay such that
					// we have a chance at coloring enough of the graph correctly,
					// and then strip any UNINTERESTING nodes that may have leaked
					// through early.
					//
					if (pending.Peek() != null)
					{
						g = new DelayRevQueue(g);
					}
					g = new FixUninterestingGenerator(g);
				}
			}
			w.pending = g;
			return g.Next();
		}
	}
}
