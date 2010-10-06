using System;
using NGit;
using NGit.Events;
using Sharpen;

namespace NGit.Events
{
	/// <summary>Describes a modification made to a repository.</summary>
	/// <remarks>Describes a modification made to a repository.</remarks>
	/// <?></?>
	public abstract class RepositoryEvent<T> : RepositoryEvent where T:RepositoryListener
	{
		private Repository repository;

		/// <summary>Set the repository this event occurred on.</summary>
		/// <remarks>
		/// Set the repository this event occurred on.
		/// <p>
		/// This method should only be invoked once on each event object, and is
		/// automatically set by
		/// <see cref="NGit.Repository.FireEvent(RepositoryEvent{T})">NGit.Repository.FireEvent(RepositoryEvent&lt;T&gt;)
		/// 	</see>
		/// .
		/// </remarks>
		/// <param name="r">the repository.</param>
		public virtual void SetRepository(Repository r)
		{
			if (repository == null)
			{
				repository = r;
			}
		}

		/// <returns>the repository that was changed.</returns>
		public virtual Repository GetRepository()
		{
			return repository;
		}

		/// <returns>type of listener this event dispatches to.</returns>
		public abstract Type GetListenerType();

		/// <summary>Dispatch this event to the given listener.</summary>
		/// <remarks>Dispatch this event to the given listener.</remarks>
		/// <param name="listener">listener that wants this event.</param>
		public abstract void Dispatch(T listener);

		void RepositoryEvent.Dispatch(RepositoryListener listener)
		{
			this.Dispatch((T) listener);
		}
		
		public override string ToString()
		{
			string type = GetType().Name;
			if (repository == null)
			{
				return type;
			}
			return type + "[" + repository + "]";
		}
	}
	
	public interface RepositoryEvent
	{
	    // Methods
	    void Dispatch(RepositoryListener listener);
	    Type GetListenerType();
	    void SetRepository(Repository r);
	}
}
