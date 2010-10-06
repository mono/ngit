using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>The standard "transfer", "fetch" and "receive" configuration parameters.
	/// 	</summary>
	/// <remarks>The standard "transfer", "fetch" and "receive" configuration parameters.
	/// 	</remarks>
	public class TransferConfig
	{
		private sealed class _SectionParser_53 : Config.SectionParser<NGit.TransferConfig
			>
		{
			public _SectionParser_53()
			{
			}

			public NGit.TransferConfig Parse(Config cfg)
			{
				return new NGit.TransferConfig(cfg);
			}
		}

		/// <summary>
		/// Key for
		/// <see cref="Config.Get{T}(SectionParser{T})">Config.Get&lt;T&gt;(SectionParser&lt;T&gt;)
		/// 	</see>
		/// .
		/// </summary>
		public static readonly Config.SectionParser<NGit.TransferConfig> KEY = new _SectionParser_53
			();

		private readonly bool fsckObjects;

		private TransferConfig(Config rc)
		{
			fsckObjects = rc.GetBoolean("receive", "fsckobjects", false);
		}

		/// <returns>strictly verify received objects?</returns>
		public virtual bool IsFsckObjects()
		{
			return fsckObjects;
		}
	}
}
