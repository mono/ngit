using NGit;
using Sharpen;

namespace NGit.Diff
{
	/// <summary>Keeps track of diff related configuration options.</summary>
	/// <remarks>Keeps track of diff related configuration options.</remarks>
	public class DiffConfig
	{
		private sealed class _SectionParser_52 : Config.SectionParser<NGit.Diff.DiffConfig
			>
		{
			public _SectionParser_52()
			{
			}

			public NGit.Diff.DiffConfig Parse(Config cfg)
			{
				return new NGit.Diff.DiffConfig(cfg);
			}
		}

		/// <summary>
		/// Key for
		/// <see cref="NGit.Config.Get{T}(NGit.Config.SectionParser{T})">NGit.Config.Get&lt;T&gt;(NGit.Config.SectionParser&lt;T&gt;)
		/// 	</see>
		/// .
		/// </summary>
		public static readonly Config.SectionParser<NGit.Diff.DiffConfig> KEY = new _SectionParser_52
			();

		private readonly bool noPrefix;

		private readonly bool renames;

		private readonly int renameLimit;

		private DiffConfig(Config rc)
		{
			noPrefix = rc.GetBoolean("diff", "noprefix", false);
			renames = rc.GetBoolean("diff", "renames", false);
			renameLimit = rc.GetInt("diff", "renamelimit", 200);
		}

		/// <returns>true if the prefix "a/" and "b/" should be suppressed.</returns>
		public virtual bool IsNoPrefix()
		{
			return noPrefix;
		}

		/// <returns>true if rename detection is enabled by default.</returns>
		public virtual bool IsRenameDetectionEnabled()
		{
			return renames;
		}

		/// <returns>limit on number of paths to perform inexact rename detection.</returns>
		public virtual int GetRenameLimit()
		{
			return renameLimit;
		}
	}
}
