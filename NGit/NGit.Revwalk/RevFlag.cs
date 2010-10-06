using System;
using NGit;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>
	/// Application level mark bit for
	/// <see cref="RevObject">RevObject</see>
	/// s.
	/// <p>
	/// To create a flag use
	/// <see cref="RevWalk.NewFlag(string)">RevWalk.NewFlag(string)</see>
	/// .
	/// </summary>
	public class RevFlag
	{
		/// <summary>
		/// Uninteresting by
		/// <see cref="RevWalk.MarkUninteresting(RevCommit)">RevWalk.MarkUninteresting(RevCommit)
		/// 	</see>
		/// .
		/// <p>
		/// We flag commits as uninteresting if the caller does not want commits
		/// reachable from a commit to
		/// <see cref="RevWalk.MarkUninteresting(RevCommit)">RevWalk.MarkUninteresting(RevCommit)
		/// 	</see>
		/// .
		/// This flag is always carried into the commit's parents and is a key part
		/// of the "rev-list B --not A" feature; A is marked UNINTERESTING.
		/// <p>
		/// This is a static flag. Its RevWalk is not available.
		/// </summary>
		public static readonly NGit.Revwalk.RevFlag UNINTERESTING = new RevFlag.StaticRevFlag
			("UNINTERESTING", RevWalk.UNINTERESTING);

		internal readonly RevWalk walker;

		internal readonly string name;

		internal readonly int mask;

		internal RevFlag(RevWalk w, string n, int m)
		{
			walker = w;
			name = n;
			mask = m;
		}

		/// <summary>Get the revision walk instance this flag was created from.</summary>
		/// <remarks>Get the revision walk instance this flag was created from.</remarks>
		/// <returns>the walker this flag was allocated out of, and belongs to.</returns>
		public virtual RevWalk GetRevWalk()
		{
			return walker;
		}

		public override string ToString()
		{
			return name;
		}

		internal class StaticRevFlag : RevFlag
		{
			internal StaticRevFlag(string n, int m) : base(null, n, m)
			{
			}

			public override RevWalk GetRevWalk()
			{
				throw new NotSupportedException(MessageFormat.Format(JGitText.Get().isAStaticFlagAndHasNorevWalkInstance
					, ToString()));
			}
		}
	}
}
