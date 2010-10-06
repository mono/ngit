using System;
using NGit.Events;
using Sharpen;

namespace NGit.Events
{
	/// <summary>Describes a change to one or more keys in the configuration.</summary>
	/// <remarks>Describes a change to one or more keys in the configuration.</remarks>
	public class ConfigChangedEvent : RepositoryEvent<ConfigChangedListener>
	{
		public override Type GetListenerType()
		{
			return typeof(ConfigChangedListener);
		}

		public override void Dispatch(ConfigChangedListener listener)
		{
			listener.OnConfigChanged(this);
		}
	}
}
