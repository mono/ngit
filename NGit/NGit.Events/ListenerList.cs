using System;
using System.Collections.Generic;
using NGit.Events;
using Sharpen;

namespace NGit.Events
{
	/// <summary>
	/// Manages a thread-safe list of
	/// <see cref="RepositoryListener">RepositoryListener</see>
	/// s.
	/// </summary>
	public class ListenerList
	{
		private readonly ConcurrentMap<Type, CopyOnWriteArrayList<ListenerHandle>> lists = 
			new ConcurrentHashMap<Type, CopyOnWriteArrayList<ListenerHandle>>();

		/// <summary>Register an IndexChangedListener.</summary>
		/// <remarks>Register an IndexChangedListener.</remarks>
		/// <param name="listener">the listener implementation.</param>
		/// <returns>handle to later remove the listener.</returns>
		public virtual ListenerHandle AddIndexChangedListener(IndexChangedListener listener
			)
		{
			return AddListener<IndexChangedListener>(listener);
		}

		/// <summary>Register a RefsChangedListener.</summary>
		/// <remarks>Register a RefsChangedListener.</remarks>
		/// <param name="listener">the listener implementation.</param>
		/// <returns>handle to later remove the listener.</returns>
		public virtual ListenerHandle AddRefsChangedListener(RefsChangedListener listener
			)
		{
			return AddListener<RefsChangedListener>(listener);
		}

		/// <summary>Register a ConfigChangedListener.</summary>
		/// <remarks>Register a ConfigChangedListener.</remarks>
		/// <param name="listener">the listener implementation.</param>
		/// <returns>handle to later remove the listener.</returns>
		public virtual ListenerHandle AddConfigChangedListener(ConfigChangedListener listener
			)
		{
			return AddListener<ConfigChangedListener>(listener);
		}

		/// <summary>Add a listener to the list.</summary>
		/// <remarks>Add a listener to the list.</remarks>
		/// <?></?>
		/// <param name="type">type of listener being registered.</param>
		/// <param name="listener">the listener instance.</param>
		/// <returns>a handle to later remove the registration, if desired.</returns>
		public virtual ListenerHandle AddListener<T>(T listener) where T:RepositoryListener
		{
			System.Type type = typeof(T);
			ListenerHandle handle = new ListenerHandle(this, type, listener);
			Add(handle);
			return handle;
		}

		/// <summary>Dispatch an event to all interested listeners.</summary>
		/// <remarks>
		/// Dispatch an event to all interested listeners.
		/// <p>
		/// Listeners are selected by the type of listener the event delivers to.
		/// </remarks>
		/// <param name="event">the event to deliver.</param>
		public virtual void Dispatch(RepositoryEvent @event)
		{
			IList<ListenerHandle> list = lists.Get(@event.GetListenerType());
			if (list != null)
			{
				foreach (ListenerHandle handle in list)
				{
					@event.Dispatch(handle.listener);
				}
			}
		}

		private void Add(ListenerHandle handle)
		{
			IList<ListenerHandle> list = lists.Get(handle.type);
			if (list == null)
			{
				CopyOnWriteArrayList<ListenerHandle> newList;
				newList = new CopyOnWriteArrayList<ListenerHandle>();
				list = lists.PutIfAbsent(handle.type, newList);
				if (list == null)
				{
					list = newList;
				}
			}
			list.AddItem(handle);
		}

		internal virtual void Remove(ListenerHandle handle)
		{
			IList<ListenerHandle> list = lists.Get(handle.type);
			if (list != null)
			{
				list.Remove(handle);
			}
		}
	}
}
