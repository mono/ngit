using System;
using NGit.Events;
using Sharpen;

namespace NGit.Events
{
	/// <summary>Describes a change to one or more paths in the index file.</summary>
	/// <remarks>Describes a change to one or more paths in the index file.</remarks>
	public class IndexChangedEvent : RepositoryEvent<IndexChangedListener>
	{
		public override Type GetListenerType()
		{
			return typeof(IndexChangedListener);
		}

		public override void Dispatch(IndexChangedListener listener)
		{
			listener.OnIndexChanged(this);
		}
	}
}
