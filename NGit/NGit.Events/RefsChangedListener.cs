using NGit.Events;
using Sharpen;

namespace NGit.Events
{
	/// <summary>
	/// Receives
	/// <see cref="RefsChangedEvent">RefsChangedEvent</see>
	/// s.
	/// </summary>
	public interface RefsChangedListener : RepositoryListener
	{
		/// <summary>Invoked when any reference changes.</summary>
		/// <remarks>Invoked when any reference changes.</remarks>
		/// <param name="event">information about the changes.</param>
		void OnRefsChanged(RefsChangedEvent @event);
	}
}
