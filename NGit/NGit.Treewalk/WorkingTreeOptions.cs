using NGit;
using Sharpen;

namespace NGit.Treewalk
{
	/// <summary>Contains options used by the WorkingTreeIterator.</summary>
	/// <remarks>Contains options used by the WorkingTreeIterator.</remarks>
	public class WorkingTreeOptions
	{
		/// <summary>
		/// Creates default options which reflect the original configuration of Git
		/// on Unix systems.
		/// </summary>
		/// <remarks>
		/// Creates default options which reflect the original configuration of Git
		/// on Unix systems.
		/// </remarks>
		/// <returns>created working tree options</returns>
		public static NGit.Treewalk.WorkingTreeOptions CreateDefaultInstance()
		{
			return new NGit.Treewalk.WorkingTreeOptions(CoreConfig.AutoCRLF.FALSE);
		}

		/// <summary>Creates options based on the specified repository configuration.</summary>
		/// <remarks>Creates options based on the specified repository configuration.</remarks>
		/// <param name="config">repository configuration to create options for</param>
		/// <returns>created working tree options</returns>
		public static NGit.Treewalk.WorkingTreeOptions CreateConfigurationInstance(Config
			 config)
		{
			return new NGit.Treewalk.WorkingTreeOptions(config.Get(CoreConfig.KEY).GetAutoCRLF
				());
		}

		/// <summary>
		/// Indicates whether EOLs of text files should be converted to '\n' before
		/// calculating the blob ID.
		/// </summary>
		/// <remarks>
		/// Indicates whether EOLs of text files should be converted to '\n' before
		/// calculating the blob ID.
		/// </remarks>
		private readonly CoreConfig.AutoCRLF autoCRLF;

		/// <summary>Creates new options.</summary>
		/// <remarks>Creates new options.</remarks>
		/// <param name="autoCRLF">
		/// indicates whether EOLs of text files should be converted to
		/// '\n' before calculating the blob ID.
		/// </param>
		public WorkingTreeOptions(CoreConfig.AutoCRLF autoCRLF)
		{
			this.autoCRLF = autoCRLF;
		}

		/// <summary>
		/// Indicates whether EOLs of text files should be converted to '\n' before
		/// calculating the blob ID.
		/// </summary>
		/// <remarks>
		/// Indicates whether EOLs of text files should be converted to '\n' before
		/// calculating the blob ID.
		/// </remarks>
		/// <returns>true if EOLs should be canonicalized.</returns>
		public virtual CoreConfig.AutoCRLF GetAutoCRLF()
		{
			return autoCRLF;
		}
	}
}
