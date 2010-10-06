using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	internal class BoundaryGenerator : Generator
	{
		internal const int UNINTERESTING = RevWalk.UNINTERESTING;

		internal Generator g;

		internal BoundaryGenerator(RevWalk w, Generator s)
		{
			g = new BoundaryGenerator.InitialGenerator(this, w, s);
		}

		internal override int OutputType()
		{
			return g.OutputType() | HAS_UNINTERESTING;
		}

		internal override void ShareFreeList(BlockRevQueue q)
		{
			g.ShareFreeList(q);
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		internal override RevCommit Next()
		{
			return g.Next();
		}

		private class InitialGenerator : Generator
		{
			private const int PARSED = RevWalk.PARSED;

			private const int DUPLICATE = RevWalk.TEMP_MARK;

			private readonly RevWalk walk;

			private readonly FIFORevQueue held;

			private readonly Generator source;

			internal InitialGenerator(BoundaryGenerator _enclosing, RevWalk w, Generator s)
			{
				this._enclosing = _enclosing;
				this.walk = w;
				this.held = new FIFORevQueue();
				this.source = s;
				this.source.ShareFreeList(this.held);
			}

			internal override int OutputType()
			{
				return this.source.OutputType();
			}

			internal override void ShareFreeList(BlockRevQueue q)
			{
				q.ShareFreeList(this.held);
			}

			/// <exception cref="NGit.Errors.MissingObjectException"></exception>
			/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			internal override RevCommit Next()
			{
				RevCommit c = this.source.Next();
				if (c != null)
				{
					foreach (RevCommit p in c.parents)
					{
						if ((p.flags & BoundaryGenerator.UNINTERESTING) != 0)
						{
							this.held.Add(p);
						}
					}
					return c;
				}
				FIFORevQueue boundary = new FIFORevQueue();
				boundary.ShareFreeList(this.held);
				for (; ; )
				{
					c = this.held.Next();
					if (c == null)
					{
						break;
					}
					if ((c.flags & BoundaryGenerator.InitialGenerator.DUPLICATE) != 0)
					{
						continue;
					}
					if ((c.flags & BoundaryGenerator.InitialGenerator.PARSED) == 0)
					{
						c.ParseHeaders(this.walk);
					}
					c.flags |= BoundaryGenerator.InitialGenerator.DUPLICATE;
					boundary.Add(c);
				}
				boundary.RemoveFlag(BoundaryGenerator.InitialGenerator.DUPLICATE);
				this._enclosing.g = boundary;
				return boundary.Next();
			}

			private readonly BoundaryGenerator _enclosing;
		}
	}
}
