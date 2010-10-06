using System.Collections.Generic;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>Test cases for ReadTree operations as implemented in WorkDirCheckout</summary>
	public class WorkDirCheckout_ReadTreeTest : ReadTreeTest
	{
		private WorkDirCheckout wdc;

		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override void PrescanTwoTrees(Tree head, Tree merge)
		{
			wdc = new WorkDirCheckout(db, db.WorkTree, head, db.GetIndex(), merge);
			wdc.PrescanTwoTrees();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Checkout()
		{
			GitIndex index = db.GetIndex();
			wdc = new WorkDirCheckout(db, db.WorkTree, theHead, index, theMerge);
			wdc.Checkout();
			index.Write();
		}

		public override IList<string> GetRemoved()
		{
			return wdc.GetRemoved();
		}

		public override IDictionary<string, ObjectId> GetUpdated()
		{
			return wdc.updated;
		}

		public override IList<string> GetConflicts()
		{
			return wdc.GetConflicts();
		}
	}
}
