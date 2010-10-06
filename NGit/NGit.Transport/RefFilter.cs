using System.Collections.Generic;
using NGit;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Filters the list of refs that are advertised to the client.</summary>
	/// <remarks>
	/// Filters the list of refs that are advertised to the client.
	/// <p>
	/// The filter is called by
	/// <see cref="ReceivePack">ReceivePack</see>
	/// and
	/// <see cref="UploadPack">UploadPack</see>
	/// to ensure
	/// that the refs are filtered before they are advertised to the client.
	/// <p>
	/// This can be used by applications to control visibility of certain refs based
	/// on a custom set of rules.
	/// </remarks>
	public abstract class RefFilter
	{
		private sealed class _RefFilter_61 : RefFilter
		{
			public _RefFilter_61()
			{
			}

			public override IDictionary<string, Ref> Filter(IDictionary<string, Ref> refs)
			{
				return refs;
			}
		}

		/// <summary>The default filter, allows all refs to be shown.</summary>
		/// <remarks>The default filter, allows all refs to be shown.</remarks>
		public static readonly RefFilter DEFAULT = new _RefFilter_61();

		/// <summary>
		/// Filters a
		/// <code>Map</code>
		/// of refs before it is advertised to the client.
		/// </summary>
		/// <param name="refs">the refs which this method need to consider.</param>
		/// <returns>the filtered map of refs.</returns>
		public abstract IDictionary<string, Ref> Filter(IDictionary<string, Ref> refs);
	}
}
