using System;
using NGit.Events;
using Sharpen;

namespace NGit.Events
{
	/// <summary>
	/// Tracks a previously registered
	/// <see cref="RepositoryListener">RepositoryListener</see>
	/// .
	/// </summary>
	public class ListenerHandle
	{
		private readonly ListenerList parent;

		internal readonly Type type;

		internal readonly RepositoryListener listener;

		internal ListenerHandle(ListenerList parent, Type type, RepositoryListener listener
			)
		{
			this.parent = parent;
			this.type = type;
			this.listener = listener;
		}

		/// <summary>Remove the listener and stop receiving events.</summary>
		/// <remarks>Remove the listener and stop receiving events.</remarks>
		public virtual void Remove()
		{
			parent.Remove(this);
		}

		public override string ToString()
		{
			return type.Name + "[" + listener + "]";
		}
	}
}
