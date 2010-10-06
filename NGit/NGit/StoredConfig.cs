using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>Persistent configuration that can be stored and loaded from a location.</summary>
	/// <remarks>Persistent configuration that can be stored and loaded from a location.</remarks>
	public abstract class StoredConfig : Config
	{
		/// <summary>Create a configuration with no default fallback.</summary>
		/// <remarks>Create a configuration with no default fallback.</remarks>
		public StoredConfig() : base()
		{
		}

		/// <summary>Create an empty configuration with a fallback for missing keys.</summary>
		/// <remarks>Create an empty configuration with a fallback for missing keys.</remarks>
		/// <param name="defaultConfig">
		/// the base configuration to be consulted when a key is missing
		/// from this configuration instance.
		/// </param>
		public StoredConfig(Config defaultConfig) : base(defaultConfig)
		{
		}

		/// <summary>Load the configuration from the persistent store.</summary>
		/// <remarks>
		/// Load the configuration from the persistent store.
		/// <p>
		/// If the configuration does not exist, this configuration is cleared, and
		/// thus behaves the same as though the backing store exists, but is empty.
		/// </remarks>
		/// <exception cref="System.IO.IOException">the configuration could not be read (but does exist).
		/// 	</exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException">the configuration is not properly formatted.
		/// 	</exception>
		public abstract void Load();

		/// <summary>Save the configuration to the persistent store.</summary>
		/// <remarks>Save the configuration to the persistent store.</remarks>
		/// <exception cref="System.IO.IOException">the configuration could not be written.</exception>
		public abstract void Save();
	}
}
