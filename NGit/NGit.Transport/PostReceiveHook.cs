using System.Collections.Generic;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>
	/// Hook invoked by
	/// <see cref="ReceivePack">ReceivePack</see>
	/// after all updates are executed.
	/// <p>
	/// The hook is called after all commands have been processed. Only commands with
	/// a status of
	/// <see cref="Result.OK">Result.OK</see>
	/// are passed into the hook. To get
	/// all commands within the hook, see
	/// <see cref="ReceivePack.GetAllCommands()">ReceivePack.GetAllCommands()</see>
	/// .
	/// <p>
	/// Any post-receive hook implementation should not update the status of a
	/// command, as the command has already completed or failed, and the status has
	/// already been returned to the client.
	/// <p>
	/// Hooks should execute quickly, as they block the server and the client from
	/// completing the connection.
	/// </summary>
	public abstract class PostReceiveHook
	{
		private sealed class _PostReceiveHook_64 : PostReceiveHook
		{
			public _PostReceiveHook_64()
			{
			}

			public override void OnPostReceive(ReceivePack rp, ICollection<ReceiveCommand> commands
				)
			{
			}
		}

		/// <summary>A simple no-op hook.</summary>
		/// <remarks>A simple no-op hook.</remarks>
		public static readonly PostReceiveHook NULL = new _PostReceiveHook_64();

		// Do nothing.
		/// <summary>Invoked after all commands are executed and status has been returned.</summary>
		/// <remarks>Invoked after all commands are executed and status has been returned.</remarks>
		/// <param name="rp">
		/// the process handling the current receive. Hooks may obtain
		/// details about the destination repository through this handle.
		/// </param>
		/// <param name="commands">
		/// unmodifiable set of successfully completed commands. May be
		/// the empty set.
		/// </param>
		public abstract void OnPostReceive(ReceivePack rp, ICollection<ReceiveCommand> commands
			);
	}
}
