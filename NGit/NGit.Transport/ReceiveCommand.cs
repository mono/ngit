using NGit;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>
	/// A command being processed by
	/// <see cref="ReceivePack">ReceivePack</see>
	/// .
	/// <p>
	/// This command instance roughly translates to the server side representation of
	/// the
	/// <see cref="RemoteRefUpdate">RemoteRefUpdate</see>
	/// created by the client.
	/// </summary>
	public class ReceiveCommand
	{
		/// <summary>Type of operation requested.</summary>
		/// <remarks>Type of operation requested.</remarks>
		public enum Type
		{
			CREATE,
			UPDATE,
			UPDATE_NONFASTFORWARD,
			DELETE
		}

		/// <summary>Result of the update command.</summary>
		/// <remarks>Result of the update command.</remarks>
		public enum Result
		{
			NOT_ATTEMPTED,
			REJECTED_NOCREATE,
			REJECTED_NODELETE,
			REJECTED_NONFASTFORWARD,
			REJECTED_CURRENT_BRANCH,
			REJECTED_MISSING_OBJECT,
			REJECTED_OTHER_REASON,
			LOCK_FAILURE,
			OK
		}

		private readonly ObjectId oldId;

		private readonly ObjectId newId;

		private readonly string name;

		private ReceiveCommand.Type type;

		private Ref @ref;

		private ReceiveCommand.Result status;

		private string message;

		/// <summary>
		/// Create a new command for
		/// <see cref="ReceivePack">ReceivePack</see>
		/// .
		/// </summary>
		/// <param name="oldId">
		/// the old object id; must not be null. Use
		/// <see cref="NGit.ObjectId.ZeroId()">NGit.ObjectId.ZeroId()</see>
		/// to indicate a ref creation.
		/// </param>
		/// <param name="newId">
		/// the new object id; must not be null. Use
		/// <see cref="NGit.ObjectId.ZeroId()">NGit.ObjectId.ZeroId()</see>
		/// to indicate a ref deletion.
		/// </param>
		/// <param name="name">name of the ref being affected.</param>
		public ReceiveCommand(ObjectId oldId, ObjectId newId, string name)
		{
			this.oldId = oldId;
			this.newId = newId;
			this.name = name;
			type = ReceiveCommand.Type.UPDATE;
			if (ObjectId.ZeroId.Equals(oldId))
			{
				type = ReceiveCommand.Type.CREATE;
			}
			if (ObjectId.ZeroId.Equals(newId))
			{
				type = ReceiveCommand.Type.DELETE;
			}
			status = ReceiveCommand.Result.NOT_ATTEMPTED;
		}

		/// <returns>the old value the client thinks the ref has.</returns>
		public virtual ObjectId GetOldId()
		{
			return oldId;
		}

		/// <returns>the requested new value for this ref.</returns>
		public virtual ObjectId GetNewId()
		{
			return newId;
		}

		/// <returns>the name of the ref being updated.</returns>
		public virtual string GetRefName()
		{
			return name;
		}

		/// <returns>
		/// the type of this command; see
		/// <see cref="Type">Type</see>
		/// .
		/// </returns>
		public virtual ReceiveCommand.Type GetType()
		{
			return type;
		}

		/// <returns>the ref, if this was advertised by the connection.</returns>
		public virtual Ref GetRef()
		{
			return @ref;
		}

		/// <returns>the current status code of this command.</returns>
		public virtual ReceiveCommand.Result GetResult()
		{
			return status;
		}

		/// <returns>the message associated with a failure status.</returns>
		public virtual string GetMessage()
		{
			return message;
		}

		/// <summary>Set the status of this command.</summary>
		/// <remarks>Set the status of this command.</remarks>
		/// <param name="s">the new status code for this command.</param>
		public virtual void SetResult(ReceiveCommand.Result s)
		{
			SetResult(s, null);
		}

		/// <summary>Set the status of this command.</summary>
		/// <remarks>Set the status of this command.</remarks>
		/// <param name="s">new status code for this command.</param>
		/// <param name="m">optional message explaining the new status.</param>
		public virtual void SetResult(ReceiveCommand.Result s, string m)
		{
			status = s;
			message = m;
		}

		internal virtual void SetRef(Ref r)
		{
			@ref = r;
		}

		internal virtual void SetType(ReceiveCommand.Type t)
		{
			type = t;
		}

		public override string ToString()
		{
			return GetType().ToString() + ": " + GetOldId().Name + " " + GetNewId().Name + " "
				 + GetRefName();
		}
	}
}
