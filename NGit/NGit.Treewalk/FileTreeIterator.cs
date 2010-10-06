using NGit;
using NGit.Treewalk;
using NGit.Util;
using Sharpen;

namespace NGit.Treewalk
{
	/// <summary>Working directory iterator for standard Java IO.</summary>
	/// <remarks>
	/// Working directory iterator for standard Java IO.
	/// <p>
	/// This iterator uses the standard <code>java.io</code> package to read the
	/// specified working directory as part of a
	/// <see cref="TreeWalk">TreeWalk</see>
	/// .
	/// </remarks>
	public class FileTreeIterator : WorkingTreeIterator
	{
		/// <summary>the starting directory.</summary>
		/// <remarks>
		/// the starting directory. This directory should correspond to the root of
		/// the repository.
		/// </remarks>
		protected internal readonly FilePath directory;

		/// <summary>
		/// the file system abstraction which will be necessary to perform certain
		/// file system operations.
		/// </summary>
		/// <remarks>
		/// the file system abstraction which will be necessary to perform certain
		/// file system operations.
		/// </remarks>
		protected internal readonly FS fs;

		/// <summary>Create a new iterator to traverse the work tree and its children.</summary>
		/// <remarks>Create a new iterator to traverse the work tree and its children.</remarks>
		/// <param name="repo">the repository whose working tree will be scanned.</param>
		public FileTreeIterator(Repository repo) : this(repo.WorkTree, repo.FileSystem, WorkingTreeOptions
			.CreateConfigurationInstance(repo.GetConfig()))
		{
			InitRootIterator(repo);
		}

		/// <summary>Create a new iterator to traverse the given directory and its children.</summary>
		/// <remarks>Create a new iterator to traverse the given directory and its children.</remarks>
		/// <param name="root">
		/// the starting directory. This directory should correspond to
		/// the root of the repository.
		/// </param>
		/// <param name="fs">
		/// the file system abstraction which will be necessary to perform
		/// certain file system operations.
		/// </param>
		/// <param name="options">working tree options to be used</param>
		public FileTreeIterator(FilePath root, FS fs, WorkingTreeOptions options) : base(
			options)
		{
			directory = root;
			this.fs = fs;
			Init(Entries());
		}

		/// <summary>Create a new iterator to traverse a subdirectory.</summary>
		/// <remarks>Create a new iterator to traverse a subdirectory.</remarks>
		/// <param name="p">the parent iterator we were created from.</param>
		/// <param name="fs">
		/// the file system abstraction which will be necessary to perform
		/// certain file system operations.
		/// </param>
		/// <param name="root">
		/// the subdirectory. This should be a directory contained within
		/// the parent directory.
		/// </param>
		protected internal FileTreeIterator(NGit.Treewalk.FileTreeIterator p, FilePath root
			, FS fs) : base(p)
		{
			directory = root;
			this.fs = fs;
			Init(Entries());
		}

		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override AbstractTreeIterator CreateSubtreeIterator(ObjectReader reader)
		{
			return new NGit.Treewalk.FileTreeIterator(this, ((FileTreeIterator.FileEntry)Current
				()).file, fs);
		}

		private WorkingTreeIterator.Entry[] Entries()
		{
			FilePath[] all = directory.ListFiles();
			if (all == null)
			{
				return EOF;
			}
			WorkingTreeIterator.Entry[] r = new WorkingTreeIterator.Entry[all.Length];
			for (int i = 0; i < r.Length; i++)
			{
				r[i] = new FileTreeIterator.FileEntry(all[i], fs);
			}
			return r;
		}

		/// <summary>Wrapper for a standard Java IO file</summary>
		internal class FileEntry : WorkingTreeIterator.Entry
		{
			internal readonly FilePath file;

			private readonly FileMode mode;

			private long length = -1;

			private long lastModified;

			internal FileEntry(FilePath f, FS fs)
			{
				file = f;
				if (f.IsDirectory())
				{
					if (new FilePath(f, Constants.DOT_GIT).IsDirectory())
					{
						mode = FileMode.GITLINK;
					}
					else
					{
						mode = FileMode.TREE;
					}
				}
				else
				{
					if (fs.CanExecute(file))
					{
						mode = FileMode.EXECUTABLE_FILE;
					}
					else
					{
						mode = FileMode.REGULAR_FILE;
					}
				}
			}

			public override FileMode GetMode()
			{
				return mode;
			}

			public override string GetName()
			{
				return file.GetName();
			}

			public override long GetLength()
			{
				if (length < 0)
				{
					length = file.Length();
				}
				return length;
			}

			public override long GetLastModified()
			{
				if (lastModified == 0)
				{
					lastModified = file.LastModified();
				}
				return lastModified;
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override InputStream OpenInputStream()
			{
				return new FileInputStream(file);
			}

			/// <summary>Get the underlying file of this entry.</summary>
			/// <remarks>Get the underlying file of this entry.</remarks>
			/// <returns>the underlying file of this entry</returns>
			public virtual FilePath GetFile()
			{
				return file;
			}
		}

		/// <returns>The root directory of this iterator</returns>
		public virtual FilePath GetDirectory()
		{
			return directory;
		}

		/// <returns>
		/// The location of the working file. This is the same as
		/// <code>
		/// new
		/// File(getDirectory(), getEntryPath())
		/// </code>
		/// but may be faster by
		/// reusing an internal File instance.
		/// </returns>
		public virtual FilePath GetEntryFile()
		{
			return ((FileTreeIterator.FileEntry)Current()).GetFile();
		}
	}
}
