using System;
using NGit;
using Sharpen;

namespace NGit.Util.IO
{
	/// <summary>An OutputStream which always throws IllegalStateExeption during write.</summary>
	/// <remarks>An OutputStream which always throws IllegalStateExeption during write.</remarks>
	public sealed class DisabledOutputStream : OutputStream
	{
		/// <summary>The canonical instance which always throws IllegalStateException.</summary>
		/// <remarks>The canonical instance which always throws IllegalStateException.</remarks>
		public static readonly NGit.Util.IO.DisabledOutputStream INSTANCE = new NGit.Util.IO.DisabledOutputStream
			();

		public DisabledOutputStream()
		{
		}

		// Do nothing, but we want to hide our constructor to prevent
		// more than one instance from being created.
		/// <exception cref="System.IO.IOException"></exception>
		public override void Write(int b)
		{
			// We shouldn't be writing output at this stage, there
			// is nobody listening to us.
			//
			throw new InvalidOperationException(JGitText.Get().writingNotPermitted);
		}
	}
}
