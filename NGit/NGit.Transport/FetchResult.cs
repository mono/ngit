using System.Collections.Generic;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Final status after a successful fetch from a remote repository.</summary>
	/// <remarks>Final status after a successful fetch from a remote repository.</remarks>
	/// <seealso cref="Transport.Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection{E})
	/// 	">Transport.Fetch(NGit.ProgressMonitor, System.Collections.Generic.ICollection&lt;E&gt;)
	/// 	</seealso>
	public class FetchResult : OperationResult
	{
		private readonly IList<FetchHeadRecord> forMerge;

		public FetchResult()
		{
			forMerge = new AList<FetchHeadRecord>();
		}

		internal virtual void Add(FetchHeadRecord r)
		{
			if (!r.notForMerge)
			{
				forMerge.AddItem(r);
			}
		}
	}
}
