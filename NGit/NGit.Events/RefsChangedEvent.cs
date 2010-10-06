using System;
using NGit.Events;
using Sharpen;

namespace NGit.Events
{
	/// <summary>Describes a change to one or more references of a repository.</summary>
	/// <remarks>Describes a change to one or more references of a repository.</remarks>
	public class RefsChangedEvent : RepositoryEvent<RefsChangedListener>
	{
		public override Type GetListenerType()
		{
			return typeof(RefsChangedListener);
		}

		public override void Dispatch(RefsChangedListener listener)
		{
			listener.OnRefsChanged(this);
		}
	}
}
