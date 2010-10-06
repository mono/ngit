using System.Collections.Generic;
using NGit;
using NGit.Dircache;
using Sharpen;

namespace NGit
{
	public class DirCacheCheckoutTest : ReadTreeTest
	{
		private DirCacheCheckout dco;

		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override void PrescanTwoTrees(Tree head, Tree merge)
		{
			DirCache dc = db.LockDirCache();
			try
			{
				dco = new DirCacheCheckout(db, head.GetTreeId(), dc, merge.GetTreeId());
				dco.PreScanTwoTrees();
			}
			finally
			{
				dc.Unlock();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Checkout()
		{
			DirCache dc = db.LockDirCache();
			try
			{
				dco = new DirCacheCheckout(db, theHead.GetTreeId(), dc, theMerge.GetTreeId());
				dco.Checkout();
			}
			finally
			{
				dc.Unlock();
			}
		}

		public override IList<string> GetRemoved()
		{
			return dco.GetRemoved();
		}

		public override IDictionary<string, ObjectId> GetUpdated()
		{
			return dco.GetUpdated();
		}

		public override IList<string> GetConflicts()
		{
			return dco.GetConflicts();
		}
	}
}
