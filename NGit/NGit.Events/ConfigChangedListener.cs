using NGit.Events;
using Sharpen;

namespace NGit.Events
{
	/// <summary>
	/// Receives
	/// <see cref="ConfigChangedEvent">ConfigChangedEvent</see>
	/// s.
	/// </summary>
	public interface ConfigChangedListener : RepositoryListener
	{
		/// <summary>Invoked when any change is made to the configuration.</summary>
		/// <remarks>Invoked when any change is made to the configuration.</remarks>
		/// <param name="event">information about the changes.</param>
		void OnConfigChanged(ConfigChangedEvent @event);
	}
}
