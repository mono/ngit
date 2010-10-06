using ICSharpCode.SharpZipLib.Zip.Compression;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>This class keeps git repository core parameters.</summary>
	/// <remarks>This class keeps git repository core parameters.</remarks>
	public class CoreConfig
	{
		private sealed class _SectionParser_58 : Config.SectionParser<NGit.CoreConfig>
		{
			public _SectionParser_58()
			{
			}

			public NGit.CoreConfig Parse(Config cfg)
			{
				return new NGit.CoreConfig(cfg);
			}
		}

		/// <summary>
		/// Key for
		/// <see cref="Config.Get{T}(SectionParser{T})">Config.Get&lt;T&gt;(SectionParser&lt;T&gt;)
		/// 	</see>
		/// .
		/// </summary>
		public static readonly Config.SectionParser<NGit.CoreConfig> KEY = new _SectionParser_58
			();

		/// <summary>
		/// Permissible values for
		/// <code>core.autocrlf</code>
		/// .
		/// </summary>
		public enum AutoCRLF
		{
			FALSE,
			TRUE,
			INPUT
		}

		private readonly int compression;

		private readonly int packIndexVersion;

		private readonly bool logAllRefUpdates;

		private readonly CoreConfig.AutoCRLF autoCRLF;

		private CoreConfig(Config rc)
		{
			compression = rc.GetInt("core", "compression", Deflater.DEFAULT_COMPRESSION);
			packIndexVersion = rc.GetInt("pack", "indexversion", 2);
			logAllRefUpdates = rc.GetBoolean("core", "logallrefupdates", true);
			autoCRLF = rc.GetEnum("core", null, "autocrlf", CoreConfig.AutoCRLF.FALSE);
		}

		/// <returns>The compression level to use when storing loose objects</returns>
		public virtual int GetCompression()
		{
			return compression;
		}

		/// <returns>the preferred pack index file format; 0 for oldest possible.</returns>
		/// <seealso cref="NGit.Transport.IndexPack">NGit.Transport.IndexPack</seealso>
		public virtual int GetPackIndexVersion()
		{
			return packIndexVersion;
		}

		/// <returns>whether to log all refUpdates</returns>
		public virtual bool IsLogAllRefUpdates()
		{
			return logAllRefUpdates;
		}

		/// <returns>whether automatic CRLF conversion has been configured</returns>
		public virtual CoreConfig.AutoCRLF GetAutoCRLF()
		{
			return autoCRLF;
		}
	}
}
