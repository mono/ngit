using NGit;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>
	/// A service exposed by
	/// <see cref="Daemon">Daemon</see>
	/// over anonymous <code>git://</code>.
	/// </summary>
	public abstract class DaemonService
	{
		private readonly string command;

		private readonly Config.SectionParser<DaemonService.ServiceConfig> configKey;

		private bool enabled;

		private bool overridable;

		internal DaemonService(string cmdName, string cfgName)
		{
			command = cmdName.StartsWith("git-") ? cmdName : "git-" + cmdName;
			configKey = new _SectionParser_65(this, cfgName);
			overridable = true;
		}

		private sealed class _SectionParser_65 : Config.SectionParser<DaemonService.ServiceConfig
			>
		{
			public _SectionParser_65(DaemonService _enclosing, string cfgName)
			{
				this._enclosing = _enclosing;
				this.cfgName = cfgName;
			}

			public DaemonService.ServiceConfig Parse(Config cfg)
			{
				return new DaemonService.ServiceConfig(this._enclosing, cfg, cfgName);
			}

			private readonly DaemonService _enclosing;

			private readonly string cfgName;
		}

		private class ServiceConfig
		{
			internal readonly bool enabled;

			internal ServiceConfig(DaemonService service, Config cfg, string name)
			{
				enabled = cfg.GetBoolean("daemon", name, service.IsEnabled());
			}
		}

		/// <returns>is this service enabled for invocation?</returns>
		public virtual bool IsEnabled()
		{
			return enabled;
		}

		/// <param name="on">true to allow this service to be used; false to deny it.</param>
		public virtual void SetEnabled(bool on)
		{
			enabled = on;
		}

		/// <returns>can this service be configured in the repository config file?</returns>
		public virtual bool IsOverridable()
		{
			return overridable;
		}

		/// <param name="on">
		/// true to permit repositories to override this service's enabled
		/// state with the <code>daemon.servicename</code> config setting.
		/// </param>
		public virtual void SetOverridable(bool on)
		{
			overridable = on;
		}

		/// <returns>name of the command requested by clients.</returns>
		public virtual string GetCommandName()
		{
			return command;
		}

		/// <summary>Determine if this service can handle the requested command.</summary>
		/// <remarks>Determine if this service can handle the requested command.</remarks>
		/// <param name="commandLine">input line from the client.</param>
		/// <returns>true if this command can accept the given command line.</returns>
		public virtual bool Handles(string commandLine)
		{
			return command.Length + 1 < commandLine.Length && commandLine[command.Length] == 
				' ' && commandLine.StartsWith(command);
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual void Execute(DaemonClient client, string commandLine)
		{
			string name = Sharpen.Runtime.Substring(commandLine, command.Length + 1);
			Repository db = client.GetDaemon().OpenRepository(name);
			if (db == null)
			{
				return;
			}
			try
			{
				if (IsEnabledFor(db))
				{
					Execute(client, db);
				}
			}
			finally
			{
				db.Close();
			}
		}

		private bool IsEnabledFor(Repository db)
		{
			if (IsOverridable())
			{
				return db.GetConfig().Get(configKey).enabled;
			}
			return IsEnabled();
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal abstract void Execute(DaemonClient client, Repository db);
	}
}
