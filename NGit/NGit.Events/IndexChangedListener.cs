using NGit.Events;
using Sharpen;

namespace NGit.Events
{
	/// <summary>
	/// Receives
	/// <see cref="IndexChangedEvent">IndexChangedEvent</see>
	/// s.
	/// </summary>
	public interface IndexChangedListener : RepositoryListener
	{
		/// <summary>Invoked when any change is made to the index.</summary>
		/// <remarks>Invoked when any change is made to the index.</remarks>
		/// <param name="event">information about the changes.</param>
		void OnIndexChanged(IndexChangedEvent @event);
	}
}
