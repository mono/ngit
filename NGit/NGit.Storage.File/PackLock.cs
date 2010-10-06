using NGit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>
	/// Keeps track of a
	/// <see cref="PackFile">PackFile</see>
	/// 's associated <code>.keep</code> file.
	/// </summary>
	public class PackLock
	{
		private readonly FilePath keepFile;

		private readonly FS fs;

		/// <summary>Create a new lock for a pack file.</summary>
		/// <remarks>Create a new lock for a pack file.</remarks>
		/// <param name="packFile">location of the <code>pack-*.pack</code> file.</param>
		/// <param name="fs">the filesystem abstraction used by the repository.</param>
		public PackLock(FilePath packFile, FS fs)
		{
			FilePath p = packFile.GetParentFile();
			string n = packFile.GetName();
			keepFile = new FilePath(p, Sharpen.Runtime.Substring(n, 0, n.Length - 5) + ".keep"
				);
			this.fs = fs;
		}

		/// <summary>Create the <code>pack-*.keep</code> file, with the given message.</summary>
		/// <remarks>Create the <code>pack-*.keep</code> file, with the given message.</remarks>
		/// <param name="msg">message to store in the file.</param>
		/// <returns>true if the keep file was successfully written; false otherwise.</returns>
		/// <exception cref="System.IO.IOException">the keep file could not be written.</exception>
		public virtual bool Lock(string msg)
		{
			if (msg == null)
			{
				return false;
			}
			if (!msg.EndsWith("\n"))
			{
				msg += "\n";
			}
			LockFile lf = new LockFile(keepFile, fs);
			if (!lf.Lock())
			{
				return false;
			}
			lf.Write(Constants.Encode(msg));
			return lf.Commit();
		}

		/// <summary>Remove the <code>.keep</code> file that holds this pack in place.</summary>
		/// <remarks>Remove the <code>.keep</code> file that holds this pack in place.</remarks>
		public virtual void Unlock()
		{
			keepFile.Delete();
		}
	}
}
