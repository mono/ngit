using NGit;
using NGit.Errors;
using Sharpen;

namespace NGit
{
	/// <summary>A tree visitor for writing a directory tree to the git object database.</summary>
	/// <remarks>
	/// A tree visitor for writing a directory tree to the git object database. Blob
	/// data is fetched from the files, not the cached blobs.
	/// </remarks>
	[System.ObsoleteAttribute(@"Use  instead.")]
	public class WriteTree : TreeVisitorWithCurrentDirectory
	{
		private readonly ObjectInserter inserter;

		/// <summary>Construct a WriteTree for a given directory</summary>
		/// <param name="sourceDirectory"></param>
		/// <param name="db"></param>
		public WriteTree(FilePath sourceDirectory, Repository db) : base(sourceDirectory)
		{
			inserter = db.NewObjectInserter();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void VisitFile(FileTreeEntry f)
		{
			FilePath path = new FilePath(GetCurrentDirectory(), f.GetName());
			FileInputStream @in = new FileInputStream(path);
			try
			{
				long sz = @in.GetChannel().Size();
				f.SetId(inserter.Insert(Constants.OBJ_BLOB, sz, @in));
				inserter.Flush();
			}
			finally
			{
				inserter.Release();
				@in.Close();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void VisitSymlink(SymlinkTreeEntry s)
		{
			if (s.IsModified())
			{
				throw new SymlinksNotSupportedException(MessageFormat.Format(JGitText.Get().symlinkCannotBeWrittenAsTheLinkTarget
					, s.GetFullName()));
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void EndVisitTree(Tree t)
		{
			base.EndVisitTree(t);
			try
			{
				t.SetId(inserter.Insert(Constants.OBJ_TREE, t.Format()));
				inserter.Flush();
			}
			finally
			{
				inserter.Release();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void VisitGitlink(GitlinkTreeEntry s)
		{
			if (s.IsModified())
			{
				throw new GitlinksNotSupportedException(s.GetFullName());
			}
		}
	}
}
