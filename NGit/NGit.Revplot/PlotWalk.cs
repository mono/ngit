using System;
using System.Collections.Generic;
using System.IO;
using NGit;
using NGit.Revplot;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revplot
{
	/// <summary>Specialized RevWalk for visualization of a commit graph.</summary>
	/// <remarks>Specialized RevWalk for visualization of a commit graph.</remarks>
	public class PlotWalk : RevWalk
	{
		private IDictionary<AnyObjectId, ICollection<Ref>> reverseRefMap;

		public override void Dispose()
		{
			base.Dispose();
			reverseRefMap.Clear();
		}

		/// <summary>Create a new revision walker for a given repository.</summary>
		/// <remarks>Create a new revision walker for a given repository.</remarks>
		/// <param name="repo">the repository the walker will obtain data from.</param>
		public PlotWalk(Repository repo) : base(repo)
		{
			base.Sort(RevSort.TOPO, true);
			reverseRefMap = repo.GetAllRefsByPeeledObjectId();
		}

		public override void Sort(RevSort s, bool use)
		{
			if (s == RevSort.TOPO && !use)
			{
				throw new ArgumentException(JGitText.Get().topologicalSortRequired);
			}
			base.Sort(s, use);
		}

		protected internal override RevCommit CreateCommit(AnyObjectId id)
		{
			return new PlotCommit<PlotLane>(id);
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override RevCommit Next()
		{
			RevCommit pc = base.Next();
			PlotCommit commit = (PlotCommit)pc;
			if (pc != null)
			{
				commit.refs = GetTags(pc);
			}
			return pc;
		}

		private Ref[] GetTags(AnyObjectId commitId)
		{
			ICollection<Ref> list = reverseRefMap.Get(commitId);
			Ref[] tags;
			if (list == null)
			{
				tags = null;
			}
			else
			{
				tags = Sharpen.Collections.ToArray(list, new Ref[list.Count]);
				Arrays.Sort(tags, new PlotWalk.PlotRefComparator(this));
			}
			return tags;
		}

		internal class PlotRefComparator : IComparer<Ref>
		{
			public virtual int Compare(Ref o1, Ref o2)
			{
				try
				{
					RevObject obj1 = this._enclosing.ParseAny(o1.GetObjectId());
					RevObject obj2 = this._enclosing.ParseAny(o2.GetObjectId());
					long t1 = this.Timeof(obj1);
					long t2 = this.Timeof(obj2);
					if (t1 > t2)
					{
						return -1;
					}
					if (t1 < t2)
					{
						return 1;
					}
					return 0;
				}
				catch (IOException)
				{
					// ignore
					return 0;
				}
			}

			internal virtual long Timeof(RevObject o)
			{
				if (o is RevCommit)
				{
					return ((RevCommit)o).CommitTime;
				}
				if (o is RevTag)
				{
					RevTag tag = (RevTag)o;
					PersonIdent who = tag.GetTaggerIdent();
					return who != null ? who.GetWhen().GetTime() : 0;
				}
				return 0;
			}

			internal PlotRefComparator(PlotWalk _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly PlotWalk _enclosing;
		}
	}
}
