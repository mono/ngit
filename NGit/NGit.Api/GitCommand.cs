using System;
using NGit;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Common superclass of all commands in the package
	/// <code>org.eclipse.jgit.api</code>
	/// <p>
	/// This class ensures that all commands fulfill the
	/// <see cref="Sharpen.Callable{V}">Sharpen.Callable&lt;V&gt;</see>
	/// interface.
	/// It also has a property
	/// <see cref="GitCommand{T}.repo">GitCommand&lt;T&gt;.repo</see>
	/// holding a reference to the git
	/// <see cref="NGit.Repository">NGit.Repository</see>
	/// this command should work with.
	/// <p>
	/// Finally this class stores a state telling whether it is allowed to call
	/// <see cref="Sharpen.Callable{V}.Call()">Sharpen.Callable&lt;V&gt;.Call()</see>
	/// on this instance. Instances of
	/// <see cref="GitCommand{T}">GitCommand&lt;T&gt;</see>
	/// can only be
	/// used for one single successful call to
	/// <see cref="Sharpen.Callable{V}.Call()">Sharpen.Callable&lt;V&gt;.Call()</see>
	/// . Afterwards this
	/// instance may not be used anymore to set/modify any properties or to call
	/// <see cref="Sharpen.Callable{V}.Call()">Sharpen.Callable&lt;V&gt;.Call()</see>
	/// again. This is achieved by setting the
	/// <see cref="GitCommand{T}.callable">GitCommand&lt;T&gt;.callable</see>
	/// property to false after the successful execution of
	/// <see cref="Sharpen.Callable{V}.Call()">Sharpen.Callable&lt;V&gt;.Call()</see>
	/// and to
	/// check the state (by calling
	/// <see cref="GitCommand{T}.CheckCallable()">GitCommand&lt;T&gt;.CheckCallable()</see>
	/// ) before setting of
	/// properties and inside
	/// <see cref="Sharpen.Callable{V}.Call()">Sharpen.Callable&lt;V&gt;.Call()</see>
	/// .
	/// </summary>
	/// <?></?>
	public abstract class GitCommand<T> : Callable<T>
	{
		/// <summary>The repository this command is working with</summary>
		protected internal readonly Repository repo;

		/// <summary>
		/// a state which tells whether it is allowed to call
		/// <see cref="Sharpen.Callable{V}.Call()">Sharpen.Callable&lt;V&gt;.Call()</see>
		/// on this
		/// instance.
		/// </summary>
		private bool callable = true;

		/// <summary>Creates a new command which interacts with a single repository</summary>
		/// <param name="repo">
		/// the
		/// <see cref="NGit.Repository">NGit.Repository</see>
		/// this command should interact with
		/// </param>
		protected internal GitCommand(Repository repo)
		{
			this.repo = repo;
		}

		/// <returns>
		/// the
		/// <see cref="NGit.Repository">NGit.Repository</see>
		/// this command is interacting with
		/// </returns>
		public virtual Repository GetRepository()
		{
			return repo;
		}

		/// <summary>
		/// Set's the state which tells whether it is allowed to call
		/// <see cref="Sharpen.Callable{V}.Call()">Sharpen.Callable&lt;V&gt;.Call()</see>
		/// on this instance.
		/// <see cref="GitCommand{T}.CheckCallable()">GitCommand&lt;T&gt;.CheckCallable()</see>
		/// will throw an exception when
		/// called and this property is set to
		/// <code>false</code>
		/// </summary>
		/// <param name="callable">
		/// if <code>true</code> it is allowed to call
		/// <see cref="Sharpen.Callable{V}.Call()">Sharpen.Callable&lt;V&gt;.Call()</see>
		/// on
		/// this instance.
		/// </param>
		protected internal virtual void SetCallable(bool callable)
		{
			this.callable = callable;
		}

		/// <summary>
		/// Checks that the property
		/// <see cref="GitCommand{T}.callable">GitCommand&lt;T&gt;.callable</see>
		/// is
		/// <code>true</code>
		/// . If not then
		/// an
		/// <see cref="System.InvalidOperationException">System.InvalidOperationException</see>
		/// is thrown
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		/// when this method is called and the property
		/// <see cref="GitCommand{T}.callable">GitCommand&lt;T&gt;.callable</see>
		/// is
		/// <code>false</code>
		/// </exception>
		protected internal virtual void CheckCallable()
		{
			if (!callable)
			{
				throw new InvalidOperationException(MessageFormat.Format(JGitText.Get().commandWasCalledInTheWrongState
					, this.GetType().FullName));
			}
		}

		public abstract T Call();
	}
}
