using System.Collections.Generic;
using NGit;
using NGit.Treewalk;
using NGit.Util;
using Sharpen;

namespace NGit.Treewalk
{
	/// <summary>
	/// A
	/// <see cref="FileTreeIterator">FileTreeIterator</see>
	/// used in tests which allows to specify explicitly
	/// what will be returned by
	/// <see cref="GetEntryLastModified()">GetEntryLastModified()</see>
	/// . This allows to
	/// write tests where certain files have to have the same modification time.
	/// <p>
	/// This iterator is configured by a list of strictly increasing long values
	/// t(0), t(1), ..., t(n). For each file with a modification between t(x) and
	/// t(x+1) [ t(x) &lt;= time &lt; t(x+1) ] this iterator will report t(x). For files
	/// with a modification time smaller t(0) a modification time of 0 is returned.
	/// For files with a modification time greater or equal t(n) t(n) will be
	/// returned.
	/// <p>
	/// This class was written especially to test racy-git problems
	/// </summary>
	public class FileTreeIteratorWithTimeControl : FileTreeIterator
	{
		private TreeSet<long> modTimes;

		public FileTreeIteratorWithTimeControl(FileTreeIterator p, Repository repo, TreeSet
			<long> modTimes) : base(p, repo.WorkTree, repo.FileSystem)
		{
			this.modTimes = modTimes;
		}

		public FileTreeIteratorWithTimeControl(FileTreeIterator p, FilePath f, FS fs, TreeSet
			<long> modTimes) : base(p, f, fs)
		{
			this.modTimes = modTimes;
		}

		public FileTreeIteratorWithTimeControl(Repository repo, TreeSet<long> modTimes) : 
			base(repo)
		{
			this.modTimes = modTimes;
		}

		public FileTreeIteratorWithTimeControl(FilePath f, FS fs, TreeSet<long> modTimes)
			 : base(f, fs, new WorkingTreeOptions(CoreConfig.AutoCRLF.FALSE))
		{
			this.modTimes = modTimes;
		}

		public override AbstractTreeIterator CreateSubtreeIterator(ObjectReader reader)
		{
			return new NGit.Treewalk.FileTreeIteratorWithTimeControl(this, ((FileTreeIterator.FileEntry
				)Current()).file, fs, modTimes);
		}

		public override long GetEntryLastModified()
		{
			if (modTimes == null)
			{
				return 0;
			}
			long cutOff = Sharpen.Extensions.ValueOf(base.GetEntryLastModified() + 1);
			ICollection<long> head = modTimes.HeadSet(cutOff);
			return head.IsEmpty() ? 0 : head.Last();
		}
	}
}
